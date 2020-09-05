using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace bosqmode
{
    /// <summary>
    /// Renderer component for the MarchingCubes
    /// </summary>
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MarchingCubes))]
    public class MCRenderer : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("One can get a bit more performance if recalculating Bounds,Normals and Tangents is not needed for the mesh")]
        private bool m_RecalculateBoundsNormalsTangets = true;

        private MeshRenderer m_meshRend;
        private MeshFilter m_filter;
        private Mesh m_mesh;
        private MarchingCubes m_manager;

        private void Awake()
        {
            m_manager = GetComponent<MarchingCubes>();
            m_mesh = new Mesh();
            //set 32bit indexbuffer for over 65535 vertex meshes
            m_mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            m_meshRend = GetComponent<MeshRenderer>();
            m_filter = GetComponent<MeshFilter>();
            m_filter.mesh = m_mesh;
            m_mesh.MarkDynamic();

            m_manager.OnUpdate += Updated;
        }

        private List<MeshPart> m_MeshList = new List<MeshPart>();

        private Thread m_thread = null;

        private void Updated(Node[] nodesUpdated)
        {
            if (!RunLock)
            {
                UpdateMesh();

                RunLock = true;

                m_MeshList.Clear();

                //copy nodes from the manager to the thread
                Node[] nodes = new Node[m_manager.CurrentNodes.Count];

                m_manager.CurrentNodes.Values.CopyTo(nodes, 0);

                m_thread = new Thread(ThreadWorker);
                m_thread.Start(nodes);
            }
        }

        private void OnDisable()
        {
            //lazily kill the thread when disabling
            if (m_thread.IsAlive)
            {
                m_thread.Abort();
                m_thread.Interrupt();
            }
        }

        private Vector3[] newverts;
        private int[] newtris = new int[0];
        private Color[] newcolors;

        /// <summary>
        /// Workerthread for offsetting the triangle indices
        /// </summary>
        /// <param name="nodes">Array of Nodes</param>
        private void ThreadWorker(object nodes)
        {
            Node[] n = (Node[])nodes;
            for (int i=0; i< n.Length; i++)
            {
                // add viable node's meshes to the list to process
                if (n[i].VacantNeighbours != Corners.None && n[i].VacantNeighbours != Corners.All)
                {
                    m_MeshList.Add(n[i].Mesh);
                }
            }

            List<Vector3> verts = new List<Vector3>();
            List<int> tris = new List<int>();
            List<Color> colors = new List<Color>();

            int startTri = 0;

            for (int i = 0; i < m_MeshList.Count; i++)
            {
                //deep copy the triangles to a new list, otherwise the base meshes triangles will change too
                int[] newTris = new int[m_MeshList[i].tris.Length];
                m_MeshList[i].tris.CopyTo(newTris, 0);

                //offset the triangle index
                for (int x = 0; x < newTris.Length; x++)
                {
                    newTris[x] += startTri;
                }

                verts.AddRange(m_MeshList[i].verts);

                //add the vertex-colors
                for (int c = 0; c < m_MeshList[i].verts.Length; c++)
                {
                    colors.Add(m_MeshList[i].color);
                }

                tris.AddRange(newTris);
                startTri = verts.Count;

            }

            newverts = new Vector3[verts.Count];
            newtris = new int[tris.Count];
            newcolors = new Color[colors.Count];

            newverts = verts.ToArray();
            newtris = tris.ToArray();
            newcolors = colors.ToArray();

            RunLock = false;
        }

        /// <summary>
        /// Updates the Unity-Mesh itself
        /// </summary>
        private void UpdateMesh()
        {
            m_mesh.Clear();

            if (newtris.Length >= 3)
            {
                m_mesh.vertices = newverts;
                m_mesh.triangles = newtris;
                m_mesh.colors = newcolors;

                if (m_RecalculateBoundsNormalsTangets)
                {
                    m_mesh.RecalculateBounds();
                    m_mesh.RecalculateNormals();
                    m_mesh.RecalculateTangents();
                }
            }
        }

        //Locking for the thread
        private bool m_running = false;
        private object _lockobj = new object();

        private bool RunLock
        {
            get
            {
                return m_running;
            }
            set
            {
                lock (_lockobj)
                {
                    m_running = value;
                }
            }
        }
    }

}
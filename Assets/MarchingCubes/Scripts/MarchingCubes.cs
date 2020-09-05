using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Courtesy to: http://paulbourke.net/geometry/polygonise/
/// and http://www.cs.carleton.edu/cs_comps/0405/shape/marching_cubes.html
/// </summary>

namespace bosqmode
{

#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(MarchingCubes))]
    public class MarchingCubesEditor : Editor
    {
        SerializedProperty mc;

        void OnEnable()
        {
            mc = serializedObject.FindProperty("m_Resolution");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(mc);
            mc.intValue = Mathf.NextPowerOfTwo(mc.intValue);
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif



    public class MarchingCubes : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Resolution of the grid, 1 equals to one unity unit, 2 == 0.5 and so on")]
        [Range(1,64)]
        private int m_Resolution = 2;
        public int Resolution
        {
            get
            {
                return Resolution;
            }
        }

        private Dictionary<Vector3, Node> m_Nodes = new Dictionary<Vector3, Node>();

        /// <summary>
        /// Currently initiated nodes
        /// </summary>
        public Dictionary<Vector3, Node> CurrentNodes
        {
            get
            {
                return m_Nodes;
            }
        }

        /// <summary>
        /// Gets a grid node if there is one
        /// </summary>
        /// <param name="position">wspace position</param>
        /// <returns>node</returns>
        public Node GetNode(Vector3 position)
        {
            return m_Nodes.ContainsKey(position) ? m_Nodes[position] : null;
        }

        private Dictionary<Corners, Vector3> m_RelativeNeighbourPositions = new Dictionary<Corners, Vector3>();

        /// <summary>
        /// Relative neighbour positions for the resolution set
        /// calculated on Awake
        /// </summary>
        public Dictionary<Corners, Vector3> RelativeNeighbourPositions
        {
            get
            {
                return m_RelativeNeighbourPositions;
            }
        }

        private Dictionary<Corners, MeshPart> m_Parts;

        /// <summary>
        /// List of (mesh)parts for the current resolution
        /// calculated on Awake()
        /// </summary>
        public Dictionary<Corners, MeshPart> Parts
        {
            get
            {
                return m_Parts;
            }
        }

        private List<Node> m_NodesChanged = new List<Node>();

        /// <summary>
        /// Gets a mesh part for given node
        /// </summary>
        /// <param name="node">Node to get mesh for</param>
        /// <param name="part">Part for the node</param>
        /// <returns>True if a part was found, false otherwise</returns>
        public bool TryGetMeshPart(Node node, out MeshPart part)
        {
            part = new MeshPart(Meshes.INVALID);
            part.color = node.Color;

            if (Parts.ContainsKey(node.VacantNeighbours))
            {
                part = new MeshPart(Parts[node.VacantNeighbours]);

                for (int i = 0; i < part.verts.Length; i++)
                {
                    part.verts[i] += node.NodePos;
                }
                return true;
            }

            return false;
        }

        private void Awake()
        {
            // Get parts for current resolution
            m_Parts = Meshes.GetPartSet(m_Resolution);

            transform.position = Vector3.zero;

            // calculate neighbour-positions beforehand
            m_RelativeNeighbourPositions.Add(Corners.v0, Utils.RoundToResolution(new Vector3(-1, -1, 1), m_Resolution) / m_Resolution);
            m_RelativeNeighbourPositions.Add(Corners.v1, Utils.RoundToResolution(new Vector3(1, -1, 1), m_Resolution) / m_Resolution);
            m_RelativeNeighbourPositions.Add(Corners.v2, Utils.RoundToResolution(new Vector3(1, -1, -1), m_Resolution) / m_Resolution);
            m_RelativeNeighbourPositions.Add(Corners.v3, Utils.RoundToResolution(new Vector3(-1, -1, -1), m_Resolution) / m_Resolution);
            m_RelativeNeighbourPositions.Add(Corners.v4, Utils.RoundToResolution(new Vector3(-1, 1, 1), m_Resolution) / m_Resolution);
            m_RelativeNeighbourPositions.Add(Corners.v5, Utils.RoundToResolution(new Vector3(1, 1, 1), m_Resolution) / m_Resolution);
            m_RelativeNeighbourPositions.Add(Corners.v6, Utils.RoundToResolution(new Vector3(1, 1, -1), m_Resolution) / m_Resolution);
            m_RelativeNeighbourPositions.Add(Corners.v7, Utils.RoundToResolution(new Vector3(-1, 1, -1), m_Resolution) / m_Resolution);

            m_RelativeNeighbourPositions.Add(Corners.bottom, Utils.RoundToResolution(new Vector3(0, -1, 0), m_Resolution) / m_Resolution);
            m_RelativeNeighbourPositions.Add(Corners.top, Utils.RoundToResolution(new Vector3(0, 1, 0), m_Resolution) / m_Resolution);
            m_RelativeNeighbourPositions.Add(Corners.left, Utils.RoundToResolution(new Vector3(-1, 0, 0), m_Resolution) / m_Resolution);
            m_RelativeNeighbourPositions.Add(Corners.right, Utils.RoundToResolution(new Vector3(1, 0, 0), m_Resolution) / m_Resolution);
            m_RelativeNeighbourPositions.Add(Corners.front, Utils.RoundToResolution(new Vector3(0, 0, -1), m_Resolution) / m_Resolution);
            m_RelativeNeighbourPositions.Add(Corners.back, Utils.RoundToResolution(new Vector3(0, 0, 1), m_Resolution) / m_Resolution);

            m_RelativeNeighbourPositions.Add(Corners.e0, Utils.RoundToResolution(new Vector3(0, -1, 1), m_Resolution) / m_Resolution);
            m_RelativeNeighbourPositions.Add(Corners.e1, Utils.RoundToResolution(new Vector3(1, -1, 0), m_Resolution) / m_Resolution);
            m_RelativeNeighbourPositions.Add(Corners.e2, Utils.RoundToResolution(new Vector3(0, -1, -1), m_Resolution) / m_Resolution);
            m_RelativeNeighbourPositions.Add(Corners.e3, Utils.RoundToResolution(new Vector3(-1, -1, 0), m_Resolution) / m_Resolution);
            m_RelativeNeighbourPositions.Add(Corners.e4, Utils.RoundToResolution(new Vector3(0, 1, 1), m_Resolution) / m_Resolution);
            m_RelativeNeighbourPositions.Add(Corners.e5, Utils.RoundToResolution(new Vector3(1, 1, 0), m_Resolution) / m_Resolution);
            m_RelativeNeighbourPositions.Add(Corners.e6, Utils.RoundToResolution(new Vector3(0, 1, -1), m_Resolution) / m_Resolution);
            m_RelativeNeighbourPositions.Add(Corners.e7, Utils.RoundToResolution(new Vector3(-1, 1, 0), m_Resolution) / m_Resolution);
            m_RelativeNeighbourPositions.Add(Corners.e8, Utils.RoundToResolution(new Vector3(-1, 0, 1), m_Resolution) / m_Resolution);
            m_RelativeNeighbourPositions.Add(Corners.e9, Utils.RoundToResolution(new Vector3(1, 0, 1), m_Resolution) / m_Resolution);
            m_RelativeNeighbourPositions.Add(Corners.e10, Utils.RoundToResolution(new Vector3(1, 0, -1), m_Resolution) / m_Resolution);
            m_RelativeNeighbourPositions.Add(Corners.e11, Utils.RoundToResolution(new Vector3(-1, 0, -1), m_Resolution) / m_Resolution);
        }

        /// <summary>
        /// Populates an array of worldspace positions
        /// </summary>
        /// <param name="positions">Worldspace position array</param>
        public void Populate(Vector3[] positions)
        {
            m_NodesChanged.Clear();

            for (int i = 0; i < positions.Length; i++)
            {
                PopulatePosition(positions[i], Color.white);
            }

            OnUpdate?.Invoke(m_NodesChanged.ToArray());
        }

        /// <summary>
        /// Populates an array of worldspace positions with vertex colors
        /// </summary>
        /// <param name="positions">Worldspace position array</param>
        /// <param name="colors">vertex colors</param>
        public void Populate(Vector3[] positions, Color[] colors)
        {
            m_NodesChanged.Clear();

            for (int i = 0; i < positions.Length; i++)
            {
                PopulatePosition(positions[i], colors[i]);
            }

            OnUpdate?.Invoke(m_NodesChanged.ToArray());
        }

        /// <summary>
        /// Deletes/Depopulates an array of worldspace positions
        /// </summary>
        /// <param name="positions">Worldspace position array</param>
        public void Delete(Vector3[] positions)
        {
            m_NodesChanged.Clear();

            for(int i=0; i<positions.Length; i++)
            {
                DeletePosition(positions[i]);
            }

            OnUpdate?.Invoke(m_NodesChanged.ToArray());
        }


        /// <summary>
        /// Populates a worldspace position
        /// </summary>
        /// <param name="position">Worldspace position</param>
        public void Populate(Vector3 position)
        {
            m_NodesChanged.Clear();

            PopulatePosition(position, Color.white);

            OnUpdate?.Invoke(m_NodesChanged.ToArray());
        }

        public void Populate(Vector3 position, Color col)
        {
            m_NodesChanged.Clear();

            PopulatePosition(position, col);

            OnUpdate?.Invoke(m_NodesChanged.ToArray());
        }

        /// <summary>
        /// Deletes a worldspace position
        /// </summary>
        /// <param name="position">Worldspace position</param>
        public void Delete(Vector3 position)
        {
            m_NodesChanged.Clear();

            DeletePosition(position);

            OnUpdate?.Invoke(m_NodesChanged.ToArray());
        }

        /// <summary>
        /// internal handling populating
        /// </summary>
        /// <param name="position">WSpace position</param>
        private void PopulatePosition(Vector3 position, Color color)
        {
            //Get the grid/node position for current resolution
            Vector3 roundedPosition = Utils.RoundToResolution(position, m_Resolution);

            //add a node if one was not found
            if (!m_Nodes.ContainsKey(roundedPosition))
            {
                Node newNode = new Node(this, roundedPosition, true);
                newNode.Color = color;
                m_Nodes.Add(roundedPosition, newNode);
                m_NodesChanged.Add(newNode);
                AddOrRecalculateNeighbours(newNode);
            }
            //otherwise add a point inside
            else
            {
                // only populate if there is no point already inside
                if (!m_Nodes[roundedPosition].PointInside)
                {
                    m_Nodes[roundedPosition].PointInside = true;
                    m_NodesChanged.Add(m_Nodes[roundedPosition]);
                    AddOrRecalculateNeighbours(m_Nodes[roundedPosition]);
                }
            }
        }

        /// <summary>
        /// internal handling of deleting/unpopulating a node
        /// </summary>
        /// <param name="position">WSpace position</param>
        private void DeletePosition(Vector3 position)
        {
            //Get the grid/node position
            Vector3 roundedPosition = Utils.RoundToResolution(position, m_Resolution);

            //remove the point inside of the node
            if (m_Nodes.ContainsKey(roundedPosition) && m_Nodes[roundedPosition].PointInside)
            {
                m_Nodes[roundedPosition].PointInside = false;
                m_NodesChanged.Add(m_Nodes[roundedPosition]);
                AddOrRecalculateNeighbours(m_Nodes[roundedPosition]);
            }
        }

        /// <summary>
        /// Recalculates the neighbours and their meshes
        /// </summary>
        /// <param name="node">Node to recalculate neighbours around</param>
        private void AddOrRecalculateNeighbours(Node node)
        {
            foreach (KeyValuePair<Corners, Vector3> item in m_RelativeNeighbourPositions)
            {
                Vector3 pos = node.NodePos + item.Value;
                if (!m_Nodes.ContainsKey(pos))
                {
                    Node neighbourNode = new Node(this, pos, color: node.Color);
                    m_Nodes.Add(pos, neighbourNode);
                    m_NodesChanged.Add(neighbourNode);
                }
                else
                {
                    m_Nodes[pos].Recalculate();
                    m_NodesChanged.Add(m_Nodes[pos]);
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(transform.position, Utils.RoundToResolution(new Vector3(-1, -1, 1), m_Resolution) / m_Resolution);
        }

        /// <summary>
        /// Event that is fired everytime the grid has been Populated or Deleted
        /// Provides an array of nodes that were changed
        /// </summary>
        public event UpdateEvent OnUpdate;
        public delegate void UpdateEvent(Node[] nodesUpdated);
    }
}
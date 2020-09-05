using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// //              v4_______e4_____________v5
//                  /|                    /|
//                 / |                   / |
//              e7/  |                e5/  |
//               /___|______e6_________/   |
//            v7|    |                 |v6 |e9
//              |    |                 |   |
//              |    |e8               |e10|
//           e11|    |                 |   |
//              |    |_________________|___|
//              |   / v0      e0       |   /v1
//              |  /                   |  /
//              | /e3                  | /e1
//              |/_____________________|/
//              v3         e2          v2
/// </summary>

namespace bosqmode
{
    /// <summary>
    /// Node implementation
    /// </summary>
    public class Node
    {
        private Vector3 m_NodePos;
        private MarchingCubes m_Manager;
        private MeshPart m_Mesh = new MeshPart(Meshes.INVALID);
        private Color? m_Color;

        public Color Color
        {
            get
            {
                if(m_Color.HasValue)
                    return m_Color.Value;
                return Color.white;
            }
            set
            {
                m_Color = value;
            }
        }

        public MeshPart Mesh
        {
            get
            {
                return m_Mesh;
            }
        }

        public Vector3 NodePos
        {
            get
            {
                return m_NodePos;
            }
        }

        private Corners m_VacantNeighbours = Corners.All;
        public Corners VacantNeighbours
        {
            get
            {
                return m_VacantNeighbours;
            }
        }

        private bool m_PointInside = false;
        public bool PointInside
        {
            get
            {
                return m_PointInside;
            }

            set
            {
                m_PointInside = value;
                Recalculate();
            }
        }

        public Node(MarchingCubes manager, Vector3 roundedPos, bool haspoint = false, Color? color = null)
        {
            if (color.HasValue)
                m_Color = color.Value;

            m_Manager = manager;
            m_NodePos = roundedPos;
            m_PointInside = haspoint;
            Recalculate();
        }

        /// <summary>
        /// Recalculates the node's neighbouring vacancy
        /// </summary>
        public void Recalculate()
        {
            Corners lastNeighbours = m_VacantNeighbours;

            // if there is a point inside -> all of the neighbours will be populated
            if (m_PointInside)
            {
                m_VacantNeighbours = Corners.None;
            }
            else
            {
                // by default none of the neighbours are populated
                m_VacantNeighbours = Corners.All;
                foreach (KeyValuePair<Corners, Vector3> item in m_Manager.RelativeNeighbourPositions)
                {
                    Node neighbourNode = m_Manager.GetNode(m_NodePos + item.Value);
                    if (neighbourNode != null)
                    {
                        // if a neighbouring node has a point inside, remove that neighbour from being vacant
                        if (neighbourNode.PointInside)
                        {
                            m_VacantNeighbours &= ~item.Key;
                        }
                    }
                }
            }

            // update mesh only if a change has happened
            if (lastNeighbours != m_VacantNeighbours)
            {
                m_Manager.TryGetMeshPart(this, out MeshPart part);
                m_Mesh = part;
                m_Mesh.color = Color;
            }
        }
    }

}
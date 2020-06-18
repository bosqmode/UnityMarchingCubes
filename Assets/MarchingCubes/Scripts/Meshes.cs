using System.Collections.Generic;
using UnityEngine;

namespace bosqmode
{
    public static class Meshes
    {
        public static Dictionary<Corners, MeshPart> GetPartSet(int resolution)
        {
            Dictionary<Corners, MeshPart> Parts = new Dictionary<Corners, MeshPart>();

            for (int i = 0; i < Tables.TriTable.Length; i++)
            {
                Corners corner = (Corners)i;

                int[] tris = Tables.TriTable[i];

                List<int> validTris = new List<int>();

                for (int x = 0; x < tris.Length; x++)
                {
                    if (tris[x] > -1)
                    {
                        validTris.Add(tris[x]);
                    }
                }

                MeshPart part = new MeshPart(Meshes.Edges, validTris.ToArray());
                part.verts.ScaleVerts(resolution);
                Parts.Add(corner, part);
            }

            return Parts;
        }

        public static readonly MeshPart INVALID = new MeshPart
        {
            verts = new Vector3[0],
            tris = new int[0]
        };

        public static readonly MeshPart Edges = new MeshPart
        {
            verts = new Vector3[]
            {
            new Vector3 (0,-1f,1f),
            new Vector3 (1f, -1f,0),
            new Vector3 (0, -1f, -1f),
            new Vector3 (-1f, -1f, 0),
            new Vector3 (0, 1f, 1f),
            new Vector3 (1f, 1f ,0),
            new Vector3 (0, 1f, -1f),
            new Vector3 (-1f, 1f, 0),
            new Vector3 (-1f, 0, 1f),
            new Vector3 (1f, 0, 1f),
            new Vector3 (1f, 0, -1f),
            new Vector3 (-1f, 0, -1f)
            },
            tris = new int[]
            {

            },
            color = Color.blue
        };
    }

    public struct MeshPart
    {
        public Vector3[] verts { get; set; }
        public int[] tris { get; set; }
        public Color32 color { get; set; }

        public MeshPart(MeshPart basemesh)
        {
            color = basemesh.color;

            verts = new Vector3[basemesh.verts.Length];
            basemesh.verts.CopyTo(verts, 0);

            tris = new int[basemesh.tris.Length];
            basemesh.tris.CopyTo(tris, 0);
        }

        public MeshPart(MeshPart basemesh, int[] newTris)
        {
            color = basemesh.color;

            verts = new Vector3[basemesh.verts.Length];
            basemesh.verts.CopyTo(verts, 0);

            tris = new int[newTris.Length];
            newTris.CopyTo(tris, 0);
        }

        public MeshPart(MeshPart basemesh, Color vertcolor)
        {
            color = vertcolor;

            verts = new Vector3[basemesh.verts.Length];
            basemesh.verts.CopyTo(verts, 0);

            tris = new int[basemesh.tris.Length];
            basemesh.tris.CopyTo(tris, 0);
        }
    }
}
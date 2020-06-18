using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bosqmode
{
    public static class Utils
    {
        /// <summary>
        /// Rounds a float to the resolution
        /// </summary>
        /// <param name="f">number</param>
        /// <param name="resolution">resolution to round to</param>
        /// <returns></returns>
        public static float RoundToResolution(float f, int resolution)
        {
            return Mathf.Round(f * resolution) / resolution;
        }

        /// <summary>
        /// Rounds a vector3 to the given resolution
        /// </summary>
        /// <param name="p">point</param>
        /// <param name="resolution">resolutions</param>
        /// <returns>Vector3 rounded to resolution</returns>
        public static Vector3 RoundToResolution(Vector3 p, int resolution)
        {
            return new Vector3(RoundToResolution(p.x, resolution), RoundToResolution(p.y, resolution), RoundToResolution(p.z, resolution));
        }

        /// <summary>
        /// Rotates a set of points around the given pivot by amount
        /// </summary>
        /// <param name="points">set of points</param>
        /// <param name="pivot">pivot point</param>
        /// <param name="amount">amount</param>
        /// <returns>rotated set of points</returns>
        public static Vector3[] RotateAroundAxis(this Vector3[] points, Vector3 pivot, Vector3 amount)
        {
            for (int i = 0; i < points.Length; i++)
            {
                points[i] = points[i].RotatePointAroundPivot(pivot, amount);
            }
            return points;
        }

        /// <summary>
        /// Rotates a point around a given pivot by amount
        /// </summary>
        /// <param name="point">point</param>
        /// <param name="pivot">pivot point</param>
        /// <param name="angles">amount</param>
        /// <returns>rotated set of points</returns>
        public static Vector3 RotatePointAroundPivot(this Vector3 point, Vector3 pivot, Vector3 angles)
        {
            point = Quaternion.Euler(angles) * (point - pivot) + pivot;
            return point;
        }

        /// <summary>
        /// Scales a set of vertices to resolution
        /// </summary>
        /// <param name="verts">set of vertices</param>
        /// <param name="resolution">resolution to scale to</param>
        /// <returns>Scaled version of the vertices</returns>
        public static Vector3[] ScaleVerts(this Vector3[] verts, int resolution)
        {
            for (int i = 0; i < verts.Length; i++)
            {
                verts[i] = (verts[i] / resolution) / 2f;
            }
            return verts;
        }
    }
}
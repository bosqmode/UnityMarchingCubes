using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace bosqmode
{
    /// <summary>
    /// And example populator for ARFoundation (4.0.2)
    /// </summary>
    public class ARFoundationPopulator : MonoBehaviour
    {
        [SerializeField]
        private MarchingCubes m_manager;

        [SerializeField]
        private ARPointCloudManager m_pointcloud;

        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("Points with confidence over this threshhold are taken into account")]
        private float m_confidenceThreshhold = 0.5f;

        private bool m_generate = false;

        /// <summary>
        /// starts/stops generating
        /// </summary>
        /// <param name="state">state</param>
        public void Generate(bool state)
        {
            m_generate = state;
        }

        private void Awake()
        {
            //hook to pointcloud update event
            m_pointcloud.pointCloudsChanged += PointcloudChanged;
        }

        private void OnDestroy()
        {
            m_pointcloud.pointCloudsChanged -= PointcloudChanged;
        }

        private void PointcloudChanged(ARPointCloudChangedEventArgs obj)
        {
            if (!m_generate)
                return;

            List<Vector3> addedPoints = new List<Vector3>();
            for (int i = 0; i < obj.updated.Count; i++)
            {
                ARPointCloud cloud = obj.updated[i];
                if (cloud.positions.HasValue)
                {
                    for (int x = 0; x < cloud.positions.Value.Length; x++)
                    {
                        // only allow points over the confidenceThreshhold
                        if (cloud.confidenceValues.Value[x] > m_confidenceThreshhold)
                        {
                            addedPoints.Add(cloud.positions.Value[x]);
                        }
                    }
                }
            }

            if(addedPoints.Count > 0)
            {
                m_manager.Populate(addedPoints.ToArray());
            }
        }
    }

}
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

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

        [Tooltip("Toggle on to incldue colors into the marching cubes, note that this is REALLY expensive")]
        [SerializeField]
        private bool m_includeColors = false;

        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("Points with confidence over this threshhold are taken into account")]
        private float m_confidenceThreshhold = 0.5f;

        private bool m_generate = false;

        private Texture2D m_cameraTexture;

        private ARCameraManager m_arcameramanager;
        private ARCameraManager CameraManager
        {
            get
            {
                if (m_arcameramanager == null)
                {
                    m_arcameramanager = FindObjectOfType<ARCameraManager>();
                }

                return m_arcameramanager;
            }
        }

        private Camera m_camera;
        private Camera cam
        {
            get
            {
                if (m_camera == null)
                {
                    m_camera = Camera.main;

                    // if camera is still null, try finding one this way
                    if (m_camera == null)
                    {
                        m_camera = FindObjectOfType<ARCameraManager>().GetComponent<Camera>();
                    }
                }

                return m_camera;
            }
        }

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
            {
                return;
            }

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

            if (addedPoints.Count > 0)
            {
                // only do color calculation if colormode is on
                if (m_includeColors)
                {

                    UpdateCameraTexture();
                    List<Color> pointColors = new List<Color>();

                    for (int i = 0; i < addedPoints.Count; i++)
                    {
                        Vector2 screenpos = cam.WorldToScreenPoint(addedPoints[i]);
                        Vector2 texturecoord = screenpos;
                        texturecoord.x = Mathf.Clamp01(texturecoord.x / Display.main.renderingWidth);
                        texturecoord.y = Mathf.Clamp01(texturecoord.y / Display.main.renderingHeight);

                        //  texturecoord.y = 1 - texturecoord.y;

                        // flip X
                        texturecoord.x = 1 - texturecoord.x;

                        pointColors.Add(m_cameraTexture.GetPixel((int)(texturecoord.y * m_cameraTexture.width), (int)(texturecoord.x * m_cameraTexture.height)));
                    }

                    m_manager.Populate(addedPoints.ToArray(), pointColors.ToArray());
                }
                else
                {
                    m_manager.Populate(addedPoints.ToArray());
                }
            }
        }

        private unsafe void UpdateCameraTexture()
        {
            // https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@4.0/manual/cpu-camera-image.html
            if (CameraManager.TryAcquireLatestCpuImage(out XRCpuImage img))
            {
                XRCpuImage.ConversionParams conversionParams = new XRCpuImage.ConversionParams
                {
                    // Get the entire image.
                    inputRect = new RectInt(0, 0, img.width, img.height),

                    // Downsample by 4.
                    outputDimensions = new Vector2Int(img.width / 4, img.height / 4),

                    // Choose RGBA format.
                    outputFormat = TextureFormat.RGBA32,

                    // Flip across the vertical axis (mirror image).
                    transformation = XRCpuImage.Transformation.MirrorY
                };


                // See how many bytes you need to store the final image.
                int size = img.GetConvertedDataSize(conversionParams);

                // Allocate a buffer to store the image.
                NativeArray<byte> buffer = new NativeArray<byte>(size, Allocator.Temp);

                // Extract the image data
                img.Convert(conversionParams, new IntPtr(buffer.GetUnsafePtr()), buffer.Length);

                // The image was converted to RGBA32 format and written into the provided buffer
                // so you can dispose of the XRCpuImage. You must do this or it will leak resources.
                img.Dispose();

                // At this point, you can process the image, pass it to a computer vision algorithm, etc.
                // In this example, you apply it to a texture to visualize it.

                // You've got the data; let's put it into a texture so you can visualize it.
                if (m_cameraTexture == null)
                {
                    m_cameraTexture = new Texture2D(
                        conversionParams.outputDimensions.x,
                        conversionParams.outputDimensions.y,
                        conversionParams.outputFormat,
                        false);
                }

                m_cameraTexture.LoadRawTextureData(buffer);
                m_cameraTexture.Apply();

                // Done with your temporary data, so you can dispose it.
                buffer.Dispose();
            }
        }
    }

}
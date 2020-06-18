using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bosqmode
{
    /// <summary>
    /// Example populator for the MarchingCubes.cs
    /// </summary>
    public class RandomPopulator : MonoBehaviour
    {
        [SerializeField]
        private MarchingCubes m_manager;

        private IEnumerator Start()
        {
            //randomly populate positions

            int count = 0;
            while (count < 1000)
            {
                yield return null;

                m_manager.Populate(transform.position + new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f)));

                count++;
                yield return null;


                //also remove some
                Vector3[] removes = new Vector3[]
                {
                    transform.position + new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f)),
                    transform.position + new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f)),
                    transform.position + new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f))
                };

                m_manager.Delete(removes);
            }

            yield return null;
        }
    }
}

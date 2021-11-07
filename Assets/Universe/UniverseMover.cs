using System.Collections;
using UnityEngine;

namespace Universe
{
    public class UniverseMover : MonoBehaviour
    {
        public GameObject centerOfTheWorldObject;
        public float frequency = 5;

        private void Start()
        {
            StartCoroutine(MoveUniversePeriodically());
        }

        private IEnumerator MoveUniversePeriodically()
        {
            while (true)
            {
                yield return new WaitForSeconds(frequency);
                if (centerOfTheWorldObject)
                {
                    var distance = centerOfTheWorldObject.transform.position;
                    centerOfTheWorldObject.transform.position = Vector3.zero;
                    transform.position -= distance;
                }
            }
        }
    }
}
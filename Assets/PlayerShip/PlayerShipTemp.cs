using UnityEngine;
using UnityEngine.UIElements;

namespace PlayerShip
{
    public class PlayerShipTemp : MonoBehaviour
    {
        // Start is called before the first frame update

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButton(0))
            {
                var rb = GetComponent<Rigidbody>();
                rb.AddForce(transform.forward * 1_000_000, ForceMode.Impulse);
            }
        
        }
    }
}

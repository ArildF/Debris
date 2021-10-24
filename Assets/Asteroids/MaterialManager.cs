using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class MaterialManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var mat = GetComponent<Renderer>().material;
        var onUnitSphere = Random.onUnitSphere;
        Debug.Log($"Setting _RandomPerInstanceVector to {onUnitSphere} for {transform.name}");
        mat.SetVector("_RandomPerInstanceVector", onUnitSphere);

    }
}

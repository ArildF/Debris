using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    public InputAction thrustAction;
    public InputAction reverseThrustAction;
    public float forwardForce = 1_000_000;
    public float reverseForce = 500_000;
    private Rigidbody _rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();

    }

    private void OnEnable()
    {
        foreach(var action in new[]{thrustAction, reverseThrustAction})
            action.Enable();
    }
    
    private void OnDisable()
    {
        foreach(var action in new[]{thrustAction, reverseThrustAction})
            action.Disable();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (thrustAction.phase == InputActionPhase.Started)
        {
            _rigidBody.AddForce(transform.forward * forwardForce, ForceMode.Impulse); 
        }
        if (reverseThrustAction.phase == InputActionPhase.Started)
        {
            _rigidBody.AddForce(transform.forward * -reverseForce, ForceMode.Impulse); 
        }
        
    }

}

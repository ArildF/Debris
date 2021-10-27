using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    public InputAction thrustAction;
    public InputAction reverseThrustAction;
    public InputAction lateralRollAction;
    public InputAction medialRollAction;
    public float forwardForce = 1_000_000;
    public float reverseForce = 500_000;
    public float lateralRollForce = 100_000;
    public float medialRollForce = 100_000;
    private Rigidbody _rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();

    }

    private void OnEnable()
    {
        foreach(var action in new[]{thrustAction, reverseThrustAction, lateralRollAction, medialRollAction})
            action.Enable();
    }
    
    private void OnDisable()
    {
        foreach(var action in new[]{thrustAction, reverseThrustAction, lateralRollAction, medialRollAction})
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

        if (lateralRollAction.phase == InputActionPhase.Started)
        {
            Debug.Log("Rolling");
            var roll = lateralRollAction.ReadValue<float>();
            _rigidBody.AddRelativeTorque(0, 0, -roll * lateralRollForce);
        }
        
        if (medialRollAction.phase == InputActionPhase.Started)
        {
            Debug.Log("Rolling");
            var roll = medialRollAction.ReadValue<float>();
            _rigidBody.AddRelativeTorque(roll * medialRollForce, 0, 0);
        }
    }

}

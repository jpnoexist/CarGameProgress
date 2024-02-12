using UnityEngine;
using System.Collections.Generic;

public class Car : MonoBehaviour
{
    private Rigidbody rigidbody;
    private float driveAxis, brakeAxis, turnAxis;
    public bool grounded = false;
   
    [Header("Suspension")]

    [SerializeField] List<Transform> wheels;
    
    [Tooltip("Radius used for wheel raycasts.")]
    [Range(0.1f, 1f)]
    [SerializeField] float wheelRadius = 0.4f;

    [Tooltip("Spring force constant k. Applies upward spring force proportional to wheel vertical offset.")]
    [Range(50f, 250f)]
    [SerializeField] float springStrength = 100f;

    [Tooltip("Spring damping value. Damps spring force proportional to point velocity.")]
    [Range(1f,5f)]
    [SerializeField] float springDamping = 3f;

    [Tooltip("Maximum speed the car is limited to.")]
    [Range(10f,50f)]
    [SerializeField]public float maxSpeed =20f;
    
    [Tooltip("Longitudinal friction coefficient.Used to Apply Oppositional longitudinal force proportional to velocity")]
    [Range(1f,5f)]
    [SerializeField]public float longitudinalFriction = 2f;

    [Tooltip("Lateral friction coefficient. Used to paply oppositional lateral force proportional to velocity.")]
    [Range(1f,5f)]
    [SerializeField]public float lateralFriction = 2f;

    [Header("Steering")]

    [Tooltip("Turn angle for wheels.")]
    [Range(10f,45f)]
    [SerializeField] float steeringAngle = 15f;

    [Tooltip("Damping coefficient for Y-Axis rotational velocity")]
    [Range(1f,10f)]
    [SerializeField] float turnDamping = 5f;

    #region Public Interface


     public void Drive(float driveAxis)
    {
        this.driveAxis = Mathf.Clamp(driveAxis, -1f, 1f);
    }

    public void Brake(float brakeAxis)
    {
        this.brakeAxis = Mathf.Clamp(brakeAxis, 0f, 1f);
    }

    public void Turn(float turnAxis)
    {
        this.turnAxis = Mathf.Clamp(turnAxis, -1f, 1f);
    }

    public bool GetGrounded() => grounded;
    #endregion

    #region MonoBehaviour Life Cycle
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        ApplySuspension();
        if (!grounded) return;
        ApplyLongitudinalForce();
        ApplyLateralForce();
        ApplyTurningForce();
    }

    #endregion

    #region Forces
    private void ApplySuspension()
    {
        bool tempGrounded = false;

        foreach (Transform wheel in wheels)
        {
            Vector3 origin = wheel.position;
            Vector3 direction = -wheel.up;
            RaycastHit hit;
            float offset = 0f;

            if (Physics.Raycast(origin, direction, out hit, wheelRadius))
            {
                // At least one of the wheel raycasts hit the ground
                tempGrounded = true;

                Vector3 end = origin + (direction * wheelRadius);
                offset = (end - hit.point).magnitude;

                float pointVelocity = Vector3.Dot(wheel.up, rigidbody.GetPointVelocity(wheel.position));
                float suspensionForce = (springStrength * offset) + (-pointVelocity * springDamping);
                rigidbody.AddForceAtPosition(wheel.up * suspensionForce, wheel.position);
            }
        }

        grounded = tempGrounded;
    }

    private void ApplyLongitudinalForce()
    {
        Vector3 force = Vector3.zero;
        float forwardVelocity = Vector3.Dot(transform.forward, rigidbody.velocity);
        float maxSpeedRatio = (1 - (Mathf.Abs(forwardVelocity) / maxSpeed));

        if (Mathf.Abs(driveAxis) > 0)
        {
            force = transform.forward * driveAxis * maxSpeed * maxSpeedRatio;
        }
        else
        {
            force = transform.forward * -forwardVelocity * longitudinalFriction;
        }

        rigidbody.AddForce(force);
    }

    private void ApplyLateralForce()
    {
        float rightVelocity = Vector3.Dot(transform.right, rigidbody.velocity);
        rigidbody.AddForce(transform.right * -rightVelocity * lateralFriction);
    }

    private void ApplyTurningForce()
    {
        float forwardVelocity = Vector3.Dot(transform.forward, rigidbody.velocity);
        float rotationalVelocity = Vector3.Dot(transform.up, rigidbody.angularVelocity);

        Vector3 rotationAxis = transform.up;
        float torque = forwardVelocity * turnAxis * (Mathf.Deg2Rad * steeringAngle);
        torque += -rotationalVelocity * turnDamping;

        rigidbody.AddTorque(rotationAxis * torque);
    }
    #endregion
}


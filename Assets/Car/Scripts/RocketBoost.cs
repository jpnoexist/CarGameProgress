using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketBoost : MonoBehaviour
{
    [SerializeField] ParticleSystem particleSystem;
    [SerializeField] private bool active = false;
    [SerializeField] private float boost;
    private Rigidbody rigidbody;
    [SerializeField] private float maxboost;
    [SerializeField] private float forceStrength;
private void Awake()
{
    rigidbody = GetComponent<Rigidbody>();
    boost = maxboost;
}

private void FixedUpdate()
{
     if (!active || boost <= 0f) 
    {
        particleSystem.Stop();
        return;
    }
    float forwardVelocity = Vector3.Dot(transform.forward, rigidbody.velocity);
    float speedRatio = (1- (forwardVelocity/ forceStrength));
    rigidbody.AddForce(transform.forward * forceStrength * speedRatio);
    boost-= Time.fixedDeltaTime;

}

    #region Interface
    public void ToggleBoost(bool newValue)
    {
        if (active == newValue) return;
        active = newValue;

        if (active && boost > 0f) particleSystem.Play();

    }
    public void Maxboost() => boost = maxboost;
    #endregion
}

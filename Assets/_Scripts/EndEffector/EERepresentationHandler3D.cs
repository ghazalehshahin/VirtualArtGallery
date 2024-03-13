using UnityEngine;

[RequireComponent(typeof(Rigidbody))] 
public class EERepresentationHandler3D : EERepresentationHandler 
{
    private Rigidbody rb; 

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter() 
    {
        NumberOfCollisions++;
        OnCollision?.Invoke(IsTouching());
    }

    protected override void FollowActualEndEffector()
    {
        Vector3 direction = EndEffectorActual.position - transform.position;
        Vector3 velocity = direction / Time.fixedDeltaTime;
        rb.velocity = velocity;
    }

    private void OnCollisionExit() 
    {
        NumberOfCollisions--;
        OnCollision?.Invoke(IsTouching());
    }
}
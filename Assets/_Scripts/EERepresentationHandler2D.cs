using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EERepresentationHandler2D : EERepresentationHandler
{
    private Rigidbody2D rb = new Rigidbody2D();
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    private void OnCollisionEnter2D()
    {
        NumberOfCollisions++;
        OnCollision?.Invoke(IsTouching());
    }

    protected override void FollowActualEndEffector()
    {
        Vector3 direction = EndEffectorActual.position - transform.position;
        Vector3 velocity = (direction / Time.fixedDeltaTime);
        rb.velocity = velocity;
    }

    private void OnCollisionExit2D()
    {
        NumberOfCollisions--;
        OnCollision?.Invoke(IsTouching());
    }
}

using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EERepresentationHandler2D : EERepresentationHandler
{
    #region Member Vars

    private Rigidbody2D rb = new();

    #endregion

    #region Unity Functions

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    private void OnCollisionEnter2D()
    {
        NumberOfCollisions++;
        OnCollision?.Invoke(IsTouching());
    }
    
    private void OnCollisionExit2D()
    {
        NumberOfCollisions--;
        OnCollision?.Invoke(IsTouching());
    }

    #endregion

    #region Protected Functions

    protected override void FollowActualEndEffector()
    {
        Vector3 direction = EndEffectorActual.position - transform.position;
        Vector3 velocity = (direction / Time.fixedDeltaTime);
        rb.velocity = velocity;
    }

    #endregion
}

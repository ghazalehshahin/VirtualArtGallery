using UnityEngine;
using UnityEngine.Events;

public class EERepresentationHandler : MonoBehaviour
{
    public UnityAction<bool> OnCollision;
    
    [SerializeField] protected Transform EndEffectorActual;

    protected int NumberOfCollisions = 0;
    
    private void FixedUpdate()
    {
        Vector3 direction = EndEffectorActual.position - transform.position;
        if (Mathf.Approximately(direction.sqrMagnitude, 0f)) return;
        FollowActualEndEffector();
    }

    protected virtual void FollowActualEndEffector()
    {
        Debug.LogError("Use the 2D or 3D inherited component, not this!!");
    }
    
    protected bool IsTouching()
    {
        return NumberOfCollisions > 0;
    }
}

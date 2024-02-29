using System;
using UnityEngine;
using UnityEngine.Events;

public class EERepresentationHandler : MonoBehaviour
{
    public UnityAction<bool> OnCollision;
    
    [SerializeField] private Transform endEffectorActual;

    private Rigidbody2D rb = new Rigidbody2D();
    private int numberOfCollisions = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Vector3 direction = endEffectorActual.position - transform.position;
        if (Mathf.Approximately(direction.sqrMagnitude, 0f)) return;
        FollowActualEndEffector();
    }

    private void FollowActualEndEffector()
    {
        Vector3 direction = endEffectorActual.position - transform.position;
        Vector3 velocity = (direction / Time.fixedDeltaTime);
        rb.velocity = velocity;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        numberOfCollisions++;
        OnCollision?.Invoke(IsTouching());
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        numberOfCollisions--;
        OnCollision?.Invoke(IsTouching());
    }
    
    private bool IsTouching()
    {
        return numberOfCollisions > 0;
    }
}

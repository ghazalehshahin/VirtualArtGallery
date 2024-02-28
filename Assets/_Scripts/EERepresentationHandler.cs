using System;
using UnityEngine;

// ReSharper disable InconsistentNaming
public class EERepresentationHandler : MonoBehaviour
{
    [SerializeField] private Transform endEffectorActual;
    [SerializeField] private float proportionalGain;
    [SerializeField] private float derivativeGain;
    [SerializeField] private float derivativeSmoothing;

    private Rigidbody2D rb = new Rigidbody2D();
    private Vector2 representationToActual = Vector2.zero;
    private Vector2 ForceDirection = Vector2.zero;
    private float distX = 0.0f;
    private float distY = 0.0f;
    private float buffX = 0.0f;
    private float buffY = 0.0f;

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
        Debug.Log("DUbbin");
    }
}

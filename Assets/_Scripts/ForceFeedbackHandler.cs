using UnityEngine;

public class ForceFeedbackHandler : MonoBehaviour
{
    [SerializeField] private EndEffectorManager endEffectorManager;
    [SerializeField] private GameObject endEffectorRepresentation;
    [SerializeField] private GameObject endEffectorActual;
    [SerializeField] private float proportionalGain;
    [SerializeField] private float derivativeGain;
    [SerializeField] private float derivativeSmoothing;
    
    private Vector2 representationToActual = Vector2.zero;
    private bool isTouchingSurface;
    private float distX;
    private float distY;
    private float buffX;
    private float buffY;
    private float diffX;
    private float diffY;
    private float oldX;
    private float oldY;
    private float xForce;
    private float yForce;
    
    private void OnEnable()
    {
        endEffectorRepresentation.GetComponent<EERepresentationHandler>().OnCollision += SetCollisionState;
    }

    private void OnDisable()
    {
        endEffectorRepresentation.GetComponent<EERepresentationHandler>().OnCollision -= SetCollisionState;
    }
    
    private void LateUpdate()
    {
        if (!isTouchingSurface)
        {
            FlushForces();
            return;
        }
        CalculateForces();
    }

    private void OnDestroy() => FlushForces();

    private void OnApplicationQuit() => FlushForces();
    
    private void SetCollisionState(bool state)
    {
        isTouchingSurface = state;
    }
    
    private void CalculateForces()
    {
        representationToActual = endEffectorActual.transform.position - endEffectorRepresentation.transform.position;
        distX = representationToActual.x;
        distY = representationToActual.y;
        buffX = (distX - oldX)/Time.deltaTime;
        buffY = (distY - oldY)/Time.deltaTime;
        diffX = derivativeSmoothing * diffX + (1 - derivativeSmoothing) * buffX;
        diffY = derivativeSmoothing * diffY + (1 - derivativeSmoothing) * buffY;
        xForce = proportionalGain * distX + derivativeGain * diffX;
        yForce = proportionalGain * distY + derivativeGain * diffY;
        endEffectorManager.SetForces(xForce, yForce);
        oldX = distX;
        oldY = distY;
    }
    
    private void FlushForces()
    {
        endEffectorManager.SetForces(0, 0);
    }
}

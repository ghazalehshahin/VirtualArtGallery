using UnityEngine;

public class ForceFeedbackHandler3D : ForceFeedbackHandler
{
    [SerializeField] private bool isVertical;
    private void LateUpdate()
    {
        
        if (!IsTouchingSurface)
        {
            FlushForces();
            return;
        }
        CalculateForces(is3D:true, isVertical: isVertical);
    }
}

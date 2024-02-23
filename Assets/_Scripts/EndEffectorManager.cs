using System;
using Haply.hAPI;
using UnityEngine;

public class EndEffectorManager : MonoBehaviour
{
    [SerializeField] private Device device;
    [SerializeField] private Board haplyBoard;
    [SerializeField] private SpriteRenderer endEffectorRepresentation;
    [SerializeField] private SpriteRenderer endEffectorActual;

    private object concurrentDataLock;
    private float[] angles;
    private float[] endEffectorPosition;
    private float[] endEffectorForce;
    private float[] torques;

    private void Awake()
    {
        concurrentDataLock = new object();
        angles = new float[2];
        endEffectorPosition = new float[2];
        endEffectorForce = new float[2];
        torques = new float[2];
    }

    private void LateUpdate()
    {
        if (!haplyBoard.HasBeenInitialized) return;
        SimulationStep();
    }
    
    private void SimulationStep ()
    {
        lock (concurrentDataLock)
        {
            if (haplyBoard.DataAvailable())
            {
                device.DeviceReadData();
                device.GetDeviceAngles( ref angles );
                device.GetDevicePosition( angles, endEffectorPosition );
                endEffectorPosition = DeviceToGraphics( endEffectorPosition );
            }
            device.SetDeviceTorques( endEffectorForce, torques );
            device.DeviceWriteTorques();
        }
    }
    
    private static float[] DeviceToGraphics ( float[] position )
    {
        return new[] {-position[0], -position[1]};
    }
}

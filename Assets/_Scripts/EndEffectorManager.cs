using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Haply.hAPI;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class EndEffectorManager : MonoBehaviour
{
    [SerializeField] private Device device;
    [SerializeField] private Board haplyBoard;
    [SerializeField] private Pantograph pantograph;
    [SerializeField] private SpriteRenderer endEffectorRepresentation;
    [Range(1,30)] [SerializeField] private float movementScalingFactor;
    [SerializeField] private SpriteRenderer endEffectorActual;
    
    private Task simulationLoopTask;
    private object concurrentDataLock;
    private Vector3 initialOffset;
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
        if (haplyBoard == null) FindObjectOfType<Board>();
        if (device == null) FindObjectOfType<Device>();
        if (pantograph == null) FindObjectOfType<Pantograph>();
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
        device.LoadConfig();
        haplyBoard.Initialize();
        device.DeviceSetParameters();
        angles = new float[2];
        torques = new float[2];
        endEffectorPosition = new float[2];
        endEffectorForce = new float[2];
        GetPosition();
        device.DeviceWriteTorques();
        initialOffset = endEffectorActual.transform.position;
        
        simulationLoopTask = new Task( SimulationLoop );
        simulationLoopTask.Start();

    }

    private void LateUpdate()
    {
        if (!haplyBoard.HasBeenInitialized) return;
        UpdateEndEffectorActual();
    }
    
    private void SimulationLoop()
    {
        TimeSpan length = TimeSpan.FromTicks( TimeSpan.TicksPerSecond / 1000 );
        Stopwatch sw = new Stopwatch();
        while (true)
        {
            sw.Start();
            Task simulationStepTask = new Task( SimulationStep );
            simulationStepTask.Start();
            simulationStepTask.Wait();
            while ( sw.Elapsed < length )
            {
                //limits speed of simulation
            }
            sw.Stop();
            sw.Reset();
        }
    }
    
    private void SimulationStep()
    {
        lock (concurrentDataLock)
        {
            GetPosition();
            device.SetDeviceTorques(endEffectorForce, torques );
            device.DeviceWriteTorques();
        }
    }

    private void GetPosition()
    {
        if (!haplyBoard.DataAvailable()) return;
        device.DeviceReadData();
        device.GetDeviceAngles(ref angles);
        device.GetDevicePosition(angles, endEffectorPosition);
        endEffectorPosition = DeviceToGraphics(endEffectorPosition);
    }

    private void UpdateEndEffectorActual()
    {
        Transform endEffectorTransform = endEffectorActual.transform;
        Vector3 position = endEffectorTransform.position;

        lock (concurrentDataLock)
        {
            position.x = endEffectorPosition[0];
            position.y = endEffectorPosition[1];
        }
        
        endEffectorTransform.position = position*movementScalingFactor + initialOffset;
    }
    
    private static float[] DeviceToGraphics(float[] position)
    {
        return new[] {-position[0], -position[1]};
    }
}

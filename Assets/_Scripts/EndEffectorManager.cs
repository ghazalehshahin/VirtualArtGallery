using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Haply.hAPI;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class EndEffectorManager : MonoBehaviour
{
    #region Sfield Vars

    [SerializeField] private Device device;
    [SerializeField] private Board haplyBoard;
    [SerializeField] private Pantograph pantograph;
    [SerializeField] private GameObject endEffectorRepresentation;
    [Range(1,30)] [SerializeField] private float movementScalingFactor;
    [SerializeField] private GameObject endEffectorActual;
    [SerializeField] private float proportionalGain;
    [SerializeField] private float derivativeGain;
    [SerializeField] private float derivativeSmoothing;

    #endregion

    #region Member Vars

    private Task simulationLoopTask;
    private Vector3 initialOffset;
    private Vector2 representationToActual = Vector2.zero;
    private object concurrentDataLock;
    private bool isTouchingSurface;
    private float[] angles;
    private float[] endEffectorPosition;
    private float[] endEffectorForce;
    private float[] torques;
    private float distX;
    private float distY;
    private float buffX;
    private float buffY;
    private float diffX;
    private float diffY;
    private float oldX;
    private float oldY;

    #endregion
    
    #region UnityFunctions

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
        if (!haplyBoard.HasBeenInitialized) return;
        UpdateEndEffectorActual();
        if (!isTouchingSurface)
        {
            FlushForces();
            return;
        }
        CalculateForces();
    }

    #endregion

    #region Simulation

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
    
    #endregion

    #region ForceFeedback

    private void SetCollisionState(bool state)
    {
        Debug.Log("Touching surface: " + state);
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
        endEffectorForce[0] = proportionalGain * distX + derivativeGain * diffX;
        endEffectorForce[1] = proportionalGain * distY + derivativeGain * diffY;
        oldX = distX;
        oldY = distY;
    }

    #endregion
    
    #region Utils
    
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

    private void FlushForces()
    {
        endEffectorForce[0] = 0f;
        endEffectorForce[1] = 0f;
    }

    #endregion
}

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Haply.hAPI;
using UnityEngine;
using UnityEngine.Events;

public class EndEffectorManager : MonoBehaviour
{
    #region Sfield Vars

    [SerializeField] private Device device;
    [SerializeField] private Board haplyBoard;
    [SerializeField] private Pantograph pantograph;
    [SerializeField] private GameObject endEffectorActual;
    [Range(1,60)] [SerializeField] private float movementScalingFactor;

    public UnityEvent ButtonPressed;
    public UnityEvent ButtonReleased;
    
    #endregion

    #region Member Vars

    private Task simulationLoopTask;
    private Vector3 initialOffset;
    private object concurrentDataLock;
    private float[] sensors;
    private bool isButtonFlipped;
    private float[] angles;
    private float[] endEffectorPosition;
    private float[] endEffectorForce;
    private float[] torques;

    #endregion

    #region Button
    
    private bool lastButtonState;
    private bool buttonEventReady;
    private int buttonDebounceThreshold;
    private int buttonDebouceCounter;

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
        sensors = new float[1];
        endEffectorPosition = new float[2];
        endEffectorForce = new float[2];
        GetPosition();
        device.DeviceWriteTorques();
        initialOffset = endEffectorActual.transform.position;

        isButtonFlipped = device.CheckButtonFlipped();
        lastButtonState = false;
        sensors[0] = 0f;
        buttonDebouceCounter = 0;
        ButtonPressed ??= new UnityEvent();
        ButtonReleased ??= new UnityEvent();

        simulationLoopTask = new Task( SimulationLoop );
        simulationLoopTask.Start();

    }

    private void LateUpdate()
    {
        if (!haplyBoard.HasBeenInitialized) return;
        UpdateEndEffectorActual();

        if (buttonEventReady)
        {
            if (lastButtonState) (isButtonFlipped ? ButtonReleased : ButtonPressed).Invoke();
            else (isButtonFlipped ? ButtonPressed : ButtonReleased).Invoke();
            buttonEventReady = false;
        }
    }

    #endregion

    public void SetForces(float xVal, float yVal)
    {
        endEffectorForce[0] = xVal;
        endEffectorForce[1] = yVal;
    }
    
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
            DoButton();
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

    private void DoButton()
    {
        if (haplyBoard.DataAvailable()) device.GetSensorData(ref sensors);
        bool buttonState = sensors[0] > 500;

        if (buttonState != lastButtonState)
        {
            if (buttonDebouceCounter > buttonDebounceThreshold)
            {
                lastButtonState = buttonState;
                buttonEventReady = true;
            }
            else buttonDebouceCounter++;
        }
        else buttonDebouceCounter = 0;
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

    #endregion
}

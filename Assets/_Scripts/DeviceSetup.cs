using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;

using TimeSpan = System.TimeSpan;
using Stopwatch = System.Diagnostics.Stopwatch;

namespace Haply.hAPI.Samples
{
    public class DeviceSetup : MonoBehaviour
    {
        [SerializeField]
        private Board haplyBoard;

        [SerializeField]
        private Device device;

        [SerializeField]
        private Pantograph pantograph;

        [Space]
        [SerializeField]
        private SpriteRenderer background;

        [SerializeField]
        private SpriteRenderer endEffectorAvatar;

        [Space]
        [SerializeField]
        private Vector2 worldSize = new Vector2( 10f, 6.5f );

        private Task simulationLoopTask;

        private object concurrentDataLock;

        private float[] angles;
        private float[] torques;

        private float[] endEffectorPosition;
        private float[] endEffectorForce;
        
        private float pixelsPerMeter;
        private float worldPixelWidth;

        private int steps;
        private int frames;

        private int drawSteps;
        private int drawFrames;

        #region Setup
        private void Awake ()
        {
            concurrentDataLock = new object();
        }

        private void Start ()
        {
            background.transform.localScale = new Vector3( worldSize.x, worldSize.y, 1f );

            Camera mainCam = Camera.main;

            Debug.Log( $"Screen.width: {Screen.width}" );

            pixelsPerMeter = Screen.width / (mainCam.aspect * mainCam.orthographicSize * 2f);
            worldPixelWidth = pixelsPerMeter * worldSize.x;

            Debug.Log( $"{nameof( pixelsPerMeter )}: {pixelsPerMeter}" );
            Debug.Log( $"{nameof( worldPixelWidth )}: {worldPixelWidth}" );

            Application.targetFrameRate = 60;

            device.LoadConfig();
            
            haplyBoard.Initialize();

            device.DeviceSetParameters();

            angles = new float[2];
            torques = new float[2];

            endEffectorPosition = new float[2];
            endEffectorForce = new float[2];
            
            simulationLoopTask = new Task( SimulationLoop );

            simulationLoopTask.Start();

            StartCoroutine( StepCountTimer() );
        }

        private IEnumerator StepCountTimer ()
        {
            while ( true )
            {
                yield return new WaitForSecondsRealtime( 1f );

                lock ( concurrentDataLock )
                {
                    drawSteps = steps;
                    steps = 0;
                }

                drawFrames = frames;
                frames = 0;

                Debug.Log( $"Simulation: {drawSteps} Hz,\t Rendering: {drawFrames} Hz" );
            }
        }
        #endregion

        #region Drawing
        private void LateUpdate ()
        {
            if (!haplyBoard.HasBeenInitialized) return;
            UpdateEndEffector();
            frames++;
        }

        private void OnGUI ()
        {
            GUI.color = Color.black;
            GUILayout.Label( $" Simulation: {drawSteps} Hz" );
            GUILayout.Label( $" Rendering: {drawFrames} Hz" );
            GUI.color = Color.white;
        }
        #endregion

        #region Simulation
        private void SimulationLoop ()
        {
            TimeSpan length = TimeSpan.FromTicks( TimeSpan.TicksPerSecond / 1000 );
            Stopwatch sw = new Stopwatch();

            while ( true )
            {
                sw.Start();

                Task simulationStepTask = new Task( SimulationStep );

                simulationStepTask.Start();

                simulationStepTask.Wait();

                while ( sw.Elapsed < length ) ;

                sw.Stop();
                sw.Reset();
            }
        }

        private void SimulationStep ()
        {
            lock ( concurrentDataLock )
            {
                if ( haplyBoard.DataAvailable() )
                {
                    device.DeviceReadData();

                    device.GetDeviceAngles( ref angles );
                    device.GetDevicePosition( angles, endEffectorPosition );
                    endEffectorPosition = DeviceToGraphics( endEffectorPosition );
                }

                device.SetDeviceTorques( endEffectorForce, torques );
                device.DeviceWriteTorques();

                steps++;
            }
        }
        #endregion

        #region Utilities
        private void UpdateEndEffector ()
        {
            Vector3 position = endEffectorAvatar.transform.position;

            if (endEffectorAvatar == null)
            {
                Debug.LogError("Is NULL!!!!");
            }
            
            lock ( concurrentDataLock )
            {
                position.x = pixelsPerMeter * endEffectorPosition[0];                               // Don't need worldPixelWidth/2, because Unity coordinate space is zero'd with display center
                position.y = pixelsPerMeter * endEffectorPosition[1] + worldSize.y / 2 + 0.5f;    // Offset is arbitrary to keep end effector avatar inside of workspace
            }

            //position *= m_WorldSize.x / 0.24f;

            endEffectorAvatar.transform.position = position;
        }

        private float[] DeviceToGraphics ( float[] position )
        {
            return new float[] { -position[0], -position[1] };
        }
        #endregion
    }
}
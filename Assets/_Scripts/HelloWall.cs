using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;

using TimeSpan = System.TimeSpan;
using Stopwatch = System.Diagnostics.Stopwatch;

namespace Haply.hAPI.Samples
{
    public class HelloWall : MonoBehaviour
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

        [SerializeField]
        private SpriteRenderer wallAvatar;

        [Space]
        [SerializeField]
        private Vector2 worldSize = new Vector2( 0.25f, 0.15f );

        [Space]
        [SerializeField]
        private float endEffectorRadius = 0.006f;

        [SerializeField]
        private float wallStiffness = 450f;

        [SerializeField]
        private Vector2 wallPosition = new Vector2( 0f, 0.07f );

        private Task simulationLoopTask;

        private object concurrentDataLock;

        private float[] angles;
        private float[] torques;
        private float[] sensors = new float[1];

        private float[] endEffectorPosition;
        private float[] endEffectorForce;
        
        private int steps;
        private int frames;

        private int drawSteps;
        private int drawFrames;

        private Vector2 wallForce = new Vector2( 0f, 0f );
        private Vector2 wallPenetration = new Vector2( 0f, 0f );

        #region Setup
        
        private void Awake ()
        {
            concurrentDataLock = new object();
            if (haplyBoard == null) FindObjectOfType<Board>();
            if (device == null) FindObjectOfType<Device>();
            if (pantograph == null) FindObjectOfType<Pantograph>();
        }

        private void Start ()
        {
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

            Camera.main.transform.position = new Vector3( 0f, -worldSize.y / 2f, -10f );
            background.transform.position = new Vector3( 0f, -worldSize.y / 2f - endEffectorRadius, 1f );
            background.transform.localScale = new Vector3( worldSize.x, worldSize.y, 1f );

            endEffectorAvatar.transform.localScale = new Vector3( endEffectorRadius, endEffectorRadius, 1f );

            float[] wallPosition = DeviceToGraphics( new float[2] { this.wallPosition.x, this.wallPosition.y } );

            wallAvatar.transform.position = new Vector3( wallPosition[0], wallPosition[1], 0f );
            wallAvatar.transform.localScale = new Vector3( worldSize.x, endEffectorRadius, 1f );
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
            GUILayout.Label($" Button: {sensors[0]}");
            //GUILayout.Label( $" End Effector: {m_EndEffectorPosition[0]}" );
            //GUILayout.Label( $" Wall: {m_WallPosition.y}" );
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
                    device.GetSensorData(ref sensors);

                    wallForce = Vector2.zero;
                    wallPenetration = new Vector2( 0f, wallPosition.y - (endEffectorPosition[1] + endEffectorRadius) );

                    if ( wallPenetration.y < 0f )
                    {
                        wallForce += wallPenetration * -wallStiffness;
                    }

                    endEffectorForce[0] = -wallForce[0];
                    endEffectorForce[1] = -wallForce[1];

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
            Transform endEffectorTransform = endEffectorAvatar.transform;
            Vector3 position = endEffectorTransform.position;

            lock ( concurrentDataLock )
            {
                position.x = endEffectorPosition[0];  // Don't need worldPixelWidth/2, because Unity coordinate space is zero'd with display center
                position.y = endEffectorPosition[1];  // Offset is arbitrary to keep end effector avatar inside of workspace
            }

            //position *= m_WorldSize.x / 0.24f;

            endEffectorTransform.position = position;
        }

        private float[] DeviceToGraphics ( float[] position )
        {
            return new[] { -position[0], -position[1] };
        }
        #endregion
    }
}
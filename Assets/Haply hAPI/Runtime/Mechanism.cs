using UnityEngine;

namespace Haply.hAPI
{
    public abstract class Mechanism : MonoBehaviour
    {
        public abstract void SetBoardType(BoardTypes boardTypes);
        
        /// <summary>
        /// Performs the forward kinematics physics calculation of a specific physical mechanism
        /// </summary>
        /// <param name="angles">angular inputs of physical mechanisms (array element length based
        /// on the degree of freedom of the mechanism in question</param>
        public abstract void ForwardKinematics ( float[] angles );
        
        /// <summary>
        /// Performs velocity calculations at the end-effector of the device
        /// </summary>
        /// <param name="angularVelocities">the angular velocities in (deg/s) of the active encoders</param>
        public abstract void VelocityCalculation ( float[] angularVelocities );

        /// <summary>
        /// Performs torque calculations that actuators need to output
        /// </summary>
        /// <param name="forces">force values calculated from physics simulation that needs to be counteracted</param>
        public abstract void TorqueCalculation ( float[] forces );

        /// <summary>
        /// Performs force calculations
        /// </summary>
        public abstract void ForceCalculation ();

        /// <summary>
        /// Performs calculations for position control
        /// </summary>
        /// <param name="position">position values for position control</param>
        public abstract void PositionControl ( float[] position );

        /// <summary>
        /// Performs inverse kinematics calculations
        /// </summary>
        public abstract void InverseKinematics ();

        /// <summary>
        /// Initializes or changes mechanisms parameters
        /// </summary>
        /// <param name="parameters">Mechanism Parameters</param>
        public abstract void SetMechanismParameters ( float[] parameters );

        /// <summary>
        /// Sets and updates sensor data that may be used by the mechanism
        /// </summary>
        /// <param name="data">sensor data from sensors attached to Haply board</param>
        public abstract void SetSensorData ( float[] data );

        /// <summary>
        /// get end-effector coordinates position
        /// </summary>
        /// <param name="buffer">buffer to receive end-effector coordinates position</param>
        /// <returns>end-effector coordinates position</returns>
        public abstract float[] GetCoordinate ( float[] buffer );

        /// <summary>
        /// get torque values from physics calculations
        /// </summary>
        /// <param name="buffer">buffer to receive torque values</param>
        /// <returns>torque values</returns>
        public abstract float[] GetTorque ( float[] buffer );

        /// <summary>
        /// get angle values from physics calculations
        /// </summary>
        /// <param name="buffer">buffer to receive angle values</param>
        /// <returns>angle values</returns>
        public abstract float[] GetAngle ( float[] buffer );

        /// <summary>
        /// get angular velocity values from physics calculations
        /// </summary>
        /// <param name="buffer">buffer to receive angular velocity values</param>
        /// <returns>angular velocity values</returns>
        public abstract float[] GetAngularVelocity ( float[] buffer );

        /// <summary>
        /// get velocity values from physics calculations
        /// </summary>
        /// <param name="buffer">buffer to receive velocity values</param>
        /// <returns>velocity values</returns>
        public abstract float[] GetVelocity ( float[] buffer );
    }
}
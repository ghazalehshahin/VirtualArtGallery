using UnityEngine;

namespace Haply.hAPI
{
    public class Actuator
    {
        public int ActuatorIndex { get; set; }
        
        public int Direction { get; set; }

        public float Torque { get; set; }

        public int Port { get; set; }

        /// <summary>
        /// Creates an Actuator using the given motor port position
        /// </summary>
        /// <param name="actuatorIndex">index of actuator, defaults to 0</param>
        /// <param name="direction">direction of actuator, defaults to 0</param>
        /// <param name="port">motor port position for actuator between 1 to 4, defaults to 1</param>
        public Actuator ( int actuatorIndex, int direction, int port )
        {
            if (actuatorIndex < 0)
            {
                Debug.LogWarning("Actuator Index can not be less than 0! Setting to 0");
                actuatorIndex = 0;
            }
            switch (direction)
            {
                case < 0:
                    Debug.LogWarning("Direction can not be less than 0! Setting to 0");
                    direction = 0; 
                    break;
                case > 1:
                    Debug.LogWarning("Direction can not be greater than 1! Setting to 1");
                    direction = 1;
                    break;
            }
            switch (port)
            {
                case < 1:
                    Debug.LogWarning("Port value can't be less than 0! Setting to 0");
                    port = 1;
                    break;
                case > 4:
                    Debug.LogWarning("Port value can't be greater than 4! Setting to 4");
                    port = 4;
                    break;
            }
            ActuatorIndex = actuatorIndex;
            Direction = direction;
            Port = port;
        }
    }
}
using UnityEngine;

namespace Haply.hAPI
{
    public class Actuator
    {
        private int actuatorIndex;
        private int direction;
        private float torque;
        private int port;

        public int ActuatorIndex
        {
            get => actuatorIndex;
            set
            {
                if (value < 0)
                {
                    Debug.LogWarning("Actuator Index can not be less than 0! Setting to 0");
                    actuatorIndex = 0;
                }
                else
                {
                    actuatorIndex = value;
                }
            }
        }

        public int Direction
        {
            get => direction;
            set => direction = value;
        }

        public float Torque
        {
            get => torque;
            set => torque = value;
        }

        public int Port
        {
            get => port;
            set => port = value;
        }

        /**
         * Creates an Actuator using the given motor port position
         *
         * @param	actuator actuator index
         * @param	port motor port position for actuator
         */
        public Actuator ( int actuatorIndex, int direction, int port )
        {
            ActuatorIndex = actuatorIndex;
            Direction = direction;
            Port = port;
        }
    }
}
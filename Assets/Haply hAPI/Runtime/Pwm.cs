namespace Haply.hAPI
{
    public class Pwm
    {
        public int Pin { get; set; }
        public int Value { get; set; }

        /// <summary>
        /// Constructs an empty PWM output for use
        /// </summary>
        public Pwm ()
        {
            Pin = 0;
            Value = 0;
        }

        /// <summary>
        /// Constructs a PWM output at the specified pin and at the desired percentage
        /// </summary>
        /// <param name="pin">pin to output pwm signal</param>
        /// <param name="pulseWidth">percent of pwm output, value between 0 to 100</param>
        public Pwm ( int pin, float pulseWidth )
        {
            Pin = pin;
            if ( pulseWidth > 100.0 )
            {
                Value = 255;
            }
            else
            {
                Value = (int) (pulseWidth * 255 / 100);
            }
        }

        /// <summary>
        /// Set the pulse width modulation (PWM) value.
        /// </summary>
        /// <param name="percent">duty cycle for the PWM signal between 0 and 100</param>
        public void SetPulse ( float percent )
        {
            if ( percent > 100.0 )
            {
                Value = 255;
            }
            else if ( percent < 0 )
            {
                Value = 0;
            }
            else
            {
                Value = (int) (percent * 255 / 100);
            }
        }

        /// <summary>
        /// get percent value of pwm signal
        /// </summary>
        /// <returns>percent value of pwm signal</returns>
        public float GetPulse ()
        {
            float percent = Value * 100f / 255f;
            return percent;
        }
    }
}
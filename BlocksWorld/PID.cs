using System;

namespace BlocksWorld
{
    public class PID
    {
        public float Proportial { get; set; } = 0.0f;

        public float Integral { get; set; } = 0.0f;

        public float Derivative { get; set; } = 0.0f;

        public float Scale { get; set; } = 1.0f;

        public float MaxIntegral { get; set; } = float.MaxValue;

        public Controller CreateController()
        {
            return new Controller(this);
        }

        public class Controller
        {
            private PID pid;

            private float previous_error = 0.0f;

            private float integral = 0.0f;

            public Controller(PID pid)
            {
                this.pid = pid;
            }

            public void Reset()
            {
                previous_error = 0.0f;
                integral = 0.0f;
            }

            /*
                error = setpoint - measured_value
                integral = integral + error * dt
                derivative = (error - previous_error)/dt
                output = Kp * error + Ki * integral + Kd * derivative
                previous_error = error
                wait(dt)
                goto start
            */

            public float GetControlValue(float current, float nominal, float timeStep)
            {
                float error = nominal - current;

                this.integral = this.integral + error * timeStep;
                this.integral = Math.Sign(this.integral) * Math.Min(Math.Abs(this.integral), this.pid.MaxIntegral);

                float derivative = (error - this.previous_error) / timeStep;

                float output =
                    this.pid.Proportial * error +
                    this.pid.Integral * this.integral +
                    this.pid.Derivative * derivative;

                this.previous_error = error;

                return this.pid.Scale * output;
            }
        }
    }
}
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;
using System;

namespace BlocksWorld
{
    public sealed class Player : RigidBody
    {
        public static readonly PID movementPid = new PID()
        {
            Derivative = 0.3f,
            Proportial = 1.5f,
            Integral = 0.15f,
            Scale = 1.0f,
            MaxIntegral = 10.0f,
        };


        PID.Controller moveXController = movementPid.CreateController();

        PID.Controller moveZController = movementPid.CreateController();

        public Player() :
            base(new CapsuleShape(0.9f, 0.4f))
        {
            this.Material = new Material()
            {
                StaticFriction = 0.05f,
                KineticFriction = 0.3f,
                Restitution = 0.1f
            };
            this.AllowDeactivation = false;
        }

        public override void PreStep(float timestep)
        {
            var current = this.LinearVelocity;

            float deltaX = this.moveXController.GetControlValue(current.X, this.WalkSpeed.X, timestep);
            float deltaZ = this.moveZController.GetControlValue(current.Z, this.WalkSpeed.Z, timestep);

            this.AddForce(new JVector(deltaX, 0, deltaZ));

            base.PreStep(timestep);
        }

        public JVector WalkSpeed { get; set; }
    }
}
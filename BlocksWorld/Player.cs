using Jitter.Collision;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;
using OpenTK;
using OpenTK.Input;
using System;

namespace BlocksWorld
{
    public sealed class Player : RigidBody, IUpdateable
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
        private readonly FirstPersonCamera camera;
        private readonly World world;

        public Player(World world) :
            base(new CapsuleShape(0.9f, 0.4f))
        {
            this.world = world;
            this.camera = new FirstPersonCamera(this);
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

        public override void PostStep(float timestep)
        {
            base.PostStep(timestep);

            if (this.Position.Y < -10)
            {
                // Spawn reset
                this.Position = new JVector(16, 4, 16);
                this.LinearVelocity = JVector.Zero;
                this.moveXController.Reset();
                this.moveZController.Reset();
            }
        }

        public void UpdateFrame(IGameInputDriver input, double time)
        {
            this.camera.Pan -= 0.5f * input.MouseMovement.X;
            this.camera.Tilt -= 0.5f * input.MouseMovement.Y;

            this.camera.Pan %= 360.0f;
            this.camera.Tilt = MathHelper.Clamp(this.camera.Tilt, -80, 80);

            Vector3 forward = Vector3.Zero;

            forward.X = 1.0f * (float)(Math.Sin(MathHelper.DegreesToRadians(this.camera.Pan)));
            forward.Y = 0.0f;
            forward.Z = 1.0f * (float)(Math.Cos(MathHelper.DegreesToRadians(this.camera.Pan)));

            Vector3 right = Vector3.Cross(forward, Vector3.UnitY);

            Vector3 move = Vector3.Zero;
            if (input.GetButton(Key.W)) move += forward;
            if (input.GetButton(Key.S)) move -= forward;
            if (input.GetButton(Key.D)) move += right;
            if (input.GetButton(Key.A)) move -= right;

            this.WalkSpeed = 10.0f * move.Jitter();

            if (input.GetButtonDown(Key.Space))
            {
                RaycastCallback callback = (b, n, f) =>
                {
                    return b.IsStatic;
                };
                RigidBody body;
                JVector normal;
                float friction;

                if (this.world.CollisionSystem.Raycast(
                    this.Position,
                    new JVector(0, -1, 0),
                    callback,
                    out body,
                    out normal,
                    out friction))
                {
                    if (friction < 0.9f)
                    {
                        this.AddForce(new JVector(0, 200, 0));
                        Console.WriteLine("{0} {1} {2}", body, normal, friction);
                    }
                }
            }

            if(input.GetMouseDown(MouseButton.Left))
            {
                this.Tool?.PrimaryUse(this.camera);
            }

            if (input.GetMouseDown(MouseButton.Right))
            {
                this.Tool?.SecondaryUse(this.camera);
            }
        }

        public JVector WalkSpeed { get; set; }

        public Camera Camera { get { return this.camera; } }

        public Tool Tool { get; set; }
    }
}
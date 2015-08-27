using System;
using OpenTK;
using Jitter.Dynamics;

namespace BlocksWorld
{
    public abstract class Camera
    {
        public abstract Matrix4 CreateProjectionMatrix(float aspect);

        public abstract Matrix4 CreateViewMatrix();

        public float ZNear { get; set; } = 0.01f;

        public float ZFar { get; set; } = 10000.0f;
    }

    public sealed class FirstPersonCamera : Camera
    {
        public RigidBody Body { get; private set; }
        public float EyeHeight { get; set; } = 0.8f;
        public float FieldOfView { get; set; } = 70.0f;

        public float Pan { get; set; }

        public float Tilt { get; set; }

        public FirstPersonCamera(RigidBody body)
        {
            this.Body = body;
        }

        public override Matrix4 CreateProjectionMatrix(float aspect)
        {
            return Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(this.FieldOfView),
                aspect,
                this.ZNear,
                this.ZFar);
        }

        public override Matrix4 CreateViewMatrix()
        {
            var eye = GetEye();
            Vector3 rot = this.GetForward();

            return Matrix4.LookAt(
                eye,
                eye + rot,
                Vector3.UnitY);
        }

        public Vector3 GetEye()
        {
            return this.Body.Position.TK() + this.EyeHeight * Vector3.UnitY;
        }

        public Vector3 GetForward()
        {
            Vector3 rot = Vector3.Zero;
            float pan = MathHelper.DegreesToRadians(this.Pan);
            float tilt = MathHelper.DegreesToRadians(this.Tilt);

            rot.X = (float)(Math.Cos(tilt) * Math.Sin(pan));
            rot.Y = (float)(Math.Sin(tilt));
            rot.Z = (float)(Math.Cos(tilt) * Math.Cos(pan));
            return rot;
        }
    }

    public sealed class StaticCamera : Camera
    {
        public Vector3 Eye { get; set; } = Vector3.Zero;
        public float FieldOfView { get; set; } = 50.0f;
        public Vector3 Target { get; set; } = Vector3.UnitZ;

        public override Matrix4 CreateProjectionMatrix(float aspect)
        {
            return Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(this.FieldOfView),
                aspect,
                this.ZNear,
                this.ZFar);
        }

        public override Matrix4 CreateViewMatrix()
        {
            return Matrix4.LookAt(
                this.Eye,
                this.Target,
                Vector3.UnitY);
        }
    }
}
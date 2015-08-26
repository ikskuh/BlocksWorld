using System;
using OpenTK;

namespace BlocksWorld
{
    public abstract class Camera
    {
        public abstract Matrix4 CreateProjectionMatrix(float aspect);

        public abstract Matrix4 CreateViewMatrix();

        public float ZNear { get; set; } = 0.01f;

        public float ZFar { get; set; } = 10000.0f;
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
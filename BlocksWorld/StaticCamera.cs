using OpenTK;

namespace BlocksWorld
{
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
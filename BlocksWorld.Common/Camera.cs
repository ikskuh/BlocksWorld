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

        public Vector3 WorldToScreen(Vector3 position, float aspect)
        {
            Matrix4 mat =
                this.CreateViewMatrix() *
                this.CreateProjectionMatrix(aspect);

            return Vector3.TransformPerspective(position, mat);
        }
    }
}
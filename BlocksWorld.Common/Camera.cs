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
}
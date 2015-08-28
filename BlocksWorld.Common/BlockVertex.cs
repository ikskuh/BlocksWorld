using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace BlocksWorld
{
    public struct BlockVertex
    {
        public static readonly int SizeInBytes =
            Vector3.SizeInBytes + // position
            Vector3.SizeInBytes + // normal
            Vector3.SizeInBytes + // color
            Vector3.SizeInBytes;  // uv

        public Vector3 position;

        public Vector3 normal;

        public Vector3 color;

        public Vector3 uv;

        /// <summary>
        /// Sets up all VertexAttribPointers on the current vertex array object for the use with BlockVertex.
        /// </summary>
        /// Also enables the needed vertex attrib arrays.
        public static void InitializeVertexBinding()
        {
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(
                0,
                3,
                VertexAttribPointerType.Float,
                false,
                BlockVertex.SizeInBytes,
                0);

            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(
                1,
                3,
                VertexAttribPointerType.Float,
                false,
                BlockVertex.SizeInBytes,
                Vector3.SizeInBytes);

            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(
                2,
                3,
                VertexAttribPointerType.Float,
                false,
                BlockVertex.SizeInBytes,
                2 * Vector3.SizeInBytes);

            GL.EnableVertexAttribArray(3);
            GL.VertexAttribPointer(
                3,
                3,
                VertexAttribPointerType.Float,
                false,
                BlockVertex.SizeInBytes,
                3 * Vector3.SizeInBytes);
        }
    }
}
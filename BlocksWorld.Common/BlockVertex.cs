using OpenTK;

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
    }
}
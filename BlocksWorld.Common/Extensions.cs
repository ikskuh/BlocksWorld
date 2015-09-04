using Jitter.LinearMath;
using OpenTK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlocksWorld
{
    public static class Extensions
    {
        public static JVector Jitter(this Vector3 tk)
        {
            return new JVector(tk.X, tk.Y, tk.Z);
        }

        public static JVector Jitter(this Assimp.Vector3D assimp)
        {
            return new JVector(assimp.X, assimp.Y, assimp.Z);
        }

        public static Vector3 TK(this JVector jitter)
        {
            return new Vector3(jitter.X, jitter.Y, jitter.Z);
        }

        public static Vector3 TK(this Assimp.Vector3D assimp)
        {
            return new Vector3(assimp.X, assimp.Y, assimp.Z);
        }

		public static Quaternion TK(this JQuaternion quat)
		{
			return new Quaternion(quat.X, quat.Y, quat.Z, quat.W);
		}

		public static JQuaternion Jitter(this Quaternion quat)
		{
			return new JQuaternion(quat.X, quat.Y, quat.Z, quat.W);
		}

		public static void Write(this BinaryWriter writer, Vector2 vector)
        {
            writer.Write(vector.X);
            writer.Write(vector.Y);
        }

        public static void Write(this BinaryWriter writer, Vector3 vector)
        {
            writer.Write(vector.X);
            writer.Write(vector.Y);
            writer.Write(vector.Z);
        }

        public static void Write(this BinaryWriter writer, Vector4 vector)
        {
            writer.Write(vector.X);
            writer.Write(vector.Y);
            writer.Write(vector.Z);
            writer.Write(vector.W);
		}

		public static void Write(this BinaryWriter writer, Quaternion vector)
		{
			writer.Write(vector.X);
			writer.Write(vector.Y);
			writer.Write(vector.Z);
			writer.Write(vector.W);
		}

		public static Vector2 ReadVector2(this BinaryReader reader)
        {
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            return new Vector2(x, y);
        }

        public static Vector3 ReadVector3(this BinaryReader reader)
        {
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            float z = reader.ReadSingle();
            return new Vector3(x, y, z);
        }

        public static Vector4 ReadVector4(this BinaryReader reader)
        {
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            float z = reader.ReadSingle();
            float w = reader.ReadSingle();
            return new Vector4(x, y, z, w);
		}

		public static Quaternion ReadQuaternion(this BinaryReader reader)
		{
			float x = reader.ReadSingle();
			float y = reader.ReadSingle();
			float z = reader.ReadSingle();
			float w = reader.ReadSingle();
			return new Quaternion(x, y, z, w);
		}
	}
}

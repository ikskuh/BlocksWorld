using Assimp;
using Jitter.LinearMath;
using OpenTK;
using System;
using System.Collections.Generic;
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

        public static JVector Jitter(this Vector3D assimp)
        {
            return new JVector(assimp.X, assimp.Y, assimp.Z);
        }

        public static Vector3 TK(this JVector jitter)
        {
            return new Vector3(jitter.X, jitter.Y, jitter.Z);
        }

        public static Vector3 TK(this Vector3D assimp)
        {
            return new Vector3(assimp.X, assimp.Y, assimp.Z);
        }
    }
}

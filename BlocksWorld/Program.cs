using Jitter.LinearMath;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlocksWorld
{
    static class Program
    {
        static int instanceCount = 0;

        static void Main(string[] args)
        {
            InitGame();
        }

        internal static void StartClient()
        {
            Thread thr = new Thread(InitGame);
            thr.IsBackground = false;
            thr.Start();
        }

        private static void InitGame()
        {
            Thread.CurrentThread.Name = "Game Instance " + (++instanceCount);
            Thread.CurrentThread.IsBackground = false;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InstalledUICulture;
            using (var game = new Game())
            {
                game.Run(60, 60);
            }
        }

        public static JVector Jitter(this Vector3 tk)
        {
            return new JVector(tk.X, tk.Y, tk.Z);
        }

        public static Vector3 TK(this JVector jitter)
        {
            return new Vector3(jitter.X, jitter.Y, jitter.Z);
        }
    }
}

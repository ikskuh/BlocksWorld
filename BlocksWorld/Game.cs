using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System;

namespace BlocksWorld
{
    public class Game : GameWindow, IGameInputDriver
    {
        private Scene scene;

        public Game() : 
            base(
                1280, 720,
                GraphicsMode.Default,
                "BlocksWorld",
                GameWindowFlags.Default,
                DisplayDevice.Default,
                3, 3,
                GraphicsContextFlags.Debug | GraphicsContextFlags.ForwardCompatible)
        {
            this.scene = new WorldScene();
        }

        protected override void OnLoad(EventArgs e)
        {
            GL.DebugMessageCallback(this.DebugProc, IntPtr.Zero);

            this.scene.Load();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            this.scene.UpdateFrame(this, e.Time);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Viewport(0, 0, this.Width, this.Height);

            GL.ClearColor(0.7f, 0.0f, 0.8f, 1.0f);
            GL.ClearDepth(1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            this.scene.RenderFrame(e.Time);

            this.SwapBuffers();
        }

        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
        }

        private void DebugProc(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
        {
            string msg = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(message, length);
            Console.WriteLine(
                "{0} {1} {2} {3}: {4}",
                source, type, id, severity, msg);
        }
    }
}
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;
using System.Collections.Generic;

namespace BlocksWorld
{
    public class Game : GameWindow
    {
        private Scene scene;
        private InputDriver input;

        WorldScene worldScene;
        ModelEditorScene modelEditorScene;

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
            this.modelEditorScene = new ModelEditorScene();
            this.worldScene = new WorldScene();
        }

        protected override void OnLoad(EventArgs e)
        {
            this.input = new InputDriver(this);

            GL.DebugMessageCallback(this.DebugProc, IntPtr.Zero);

            this.SetScene(this.worldScene);

            this.VSync = VSyncMode.On;
        }

        private HashSet<Scene> usedScenes = new HashSet<Scene>();

        private void SetScene(Scene scene)
        {
            if (this.scene != null)
                this.scene.Disable();
            this.scene = scene;
            if (this.scene != null)
            {
                if (this.scene.IsLoaded == false)
                    this.scene.Load();
                this.scene.Enable();
            }

            if (scene != null)
            {
                this.usedScenes.Add(scene);
            }
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (input.GetButtonDown(Key.F8))
                Program.StartClient();

            if (input.GetButtonDown(Key.F9))
                this.SetScene(this.scene == this.worldScene ? (Scene)this.modelEditorScene : (Scene)this.worldScene);

            this.scene.UpdateFrame(this.input, e.Time);

            this.input.Reset();
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
            foreach (var scene in this.usedScenes)
                scene.Dispose();
            this.usedScenes.Clear();
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
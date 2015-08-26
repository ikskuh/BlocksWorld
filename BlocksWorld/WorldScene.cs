using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;

namespace BlocksWorld
{
    public class WorldScene : Scene
    {
        World world;
        StaticCamera camera;
        WorldRenderer renderer;

        int objectShader;

        double totalTime = 0.0;

        public WorldScene()
        {
            this.camera = new StaticCamera()
            {
                Eye = new Vector3(-8, 6, -8),
                Target = new Vector3(16, 1, 16),
            };
            this.world = new World(32, 16, 32);
            this.renderer = new WorldRenderer(this.world);

            for(int x = 0; x < this.world.SizeX; x++)
            {
                for (int z = 0; z < this.world.SizeZ; z++)
                {
                    this.world[x, 0, z] = new BasicBlock(Vector3.UnitX);
                }
            }
            this.world[1, 1, 1] = new BasicBlock(Vector3.UnitY);
        }

        public override void Load()
        {
            this.objectShader = Shader.CompileFromResource(
                "BlocksWorld.Shaders.Object.vs",
                "BlocksWorld.Shaders.Object.fs");

            this.renderer.Load();
        }

        public override void UpdateFrame(double time)
        {
            this.totalTime += time;

            Vector3 offset = Vector3.Zero;

            offset.X = 24.0f * (float)(Math.Sin(this.totalTime));
            offset.Y = 8.0f;
            offset.Z = 24.0f * (float)(Math.Cos(this.totalTime));

            this.camera.Eye = this.camera.Target + offset;
        }

        public override void RenderFrame(double time)
        {

            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);

            Matrix4 worldViewProjection =
                Matrix4.Identity *
                this.camera.CreateViewMatrix() *
                this.camera.CreateProjectionMatrix(1280.0f / 720.0f); // HACK: Hardcoded aspect

            GL.UseProgram(this.objectShader);
            int loc = GL.GetUniformLocation(this.objectShader, "uWorldViewProjection");
            if(loc >= 0)
            {
                GL.UniformMatrix4(loc, false, ref worldViewProjection);
            }

            this.renderer.Render(this.camera, time);

            GL.UseProgram(0);
        }
    }
}
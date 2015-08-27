using Jitter.Collision;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;
using System.IO;

namespace BlocksWorld
{
    public class WorldScene : Scene
    {
        World world;
        WorldRenderer renderer;
        DebugRenderer debug;

        int objectShader;

        double totalTime = 0.0;
        private Player player;
        private TextureArray textures;

        public WorldScene()
        {
            this.world = new World();
            if (File.Exists("world.dat"))
            {
                this.world.Load("world.dat");
            }
            else
            {
                for (int x = 0; x <= 32; x++)
                {
                    for (int z = 0; z < 32; z++)
                    {
                        this.world[x, 0, z] = new BasicBlock(2);
                    }
                }

                this.world[1, 1, 1] = new BasicBlock(1);

                for (int x = 0; x < 32; x++)
                {
                    for (int y = 1; y < 4; y++)
                    {
                        if ((x != 16) || (y >= 3))
                            this.world[x, y, 8] = new BasicBlock(3);
                    }
                }
            }

            this.debug = new DebugRenderer();
            this.renderer = new WorldRenderer(this.world);

            // Create player
            {
                this.player = new Player(this.world);
                this.player.Tool = new BlockPlaceTool(this.world);
                this.player.Position = new JVector(16, 4, 16);
                
                this.world.AddBody(this.player);
                this.world.AddConstraint(new Jitter.Dynamics.Constraints.SingleBody.FixedAngle(this.player));
            }
        }

        public override void Load()
        {
            this.objectShader = Shader.CompileFromResource(
                "BlocksWorld.Shaders.Object.vs",
                "BlocksWorld.Shaders.Object.fs");
            
            this.textures = TextureArray.LoadFromResource("BlocksWorld.Textures.Blocks.png");

            this.debug.Load();
            this.renderer.Load();
        }

        public override void UpdateFrame(IGameInputDriver input, double time)
        {
            this.totalTime += time;

            this.player.UpdateFrame(input, time);
            
            this.world.Step((float)time, false);

            if (input.GetButtonDown(Key.F5))
                this.world.Save("world.dat");
        }

        public override void RenderFrame(double time)
        {
            this.debug.Reset();

            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);
            GL.ClearColor(Color4.LightSkyBlue);
            GL.ClearDepth(1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Matrix4 worldViewProjection =
                Matrix4.Identity *
                this.player.Camera.CreateViewMatrix() *
                this.player.Camera.CreateProjectionMatrix(1280.0f / 720.0f); // HACK: Hardcoded aspect
            
            // Draw world
            {
                GL.UseProgram(this.objectShader);
                int loc = GL.GetUniformLocation(this.objectShader, "uWorldViewProjection");
                if (loc >= 0)
                {
                    GL.UniformMatrix4(loc, false, ref worldViewProjection);
                }

                loc = GL.GetUniformLocation(this.objectShader, "uTextures");
                if (loc >= 0)
                {
                    GL.Uniform1(loc, 0);
                }

                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2DArray, this.textures.ID);

                this.renderer.Render(this.player.Camera, time);

                GL.BindTexture(TextureTarget.Texture2DArray, 0);
                GL.UseProgram(0); 
            }

            foreach(RigidBody body in this.world.RigidBodies)
            {
                body.DebugDraw(this.debug);
            }

            /*
            if (this.focus != null)
            {
                this.debug.DrawPoint(this.focus.Position);
                this.debug.DrawLine(this.focus.Position, this.focus.Position + 0.25f * this.focus.Normal);
            }
            */
            
            this.debug.Render(this.player.Camera, time);
        }
    }
}
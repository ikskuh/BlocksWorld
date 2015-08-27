using Jitter.Collision;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

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
        
        private Network network;

        public WorldScene()
        {
            this.world = new World();


            this.debug = new DebugRenderer();
            this.renderer = new WorldRenderer(this.world);

            this.network = new Network(new TcpClient("localhost", 4523));

            this.network[NetworkPhrase.LoadWorld] = this.LoadWorldFromNetwork;
            this.network[NetworkPhrase.SpawnPlayer] = this.SpawnPlayer;
            this.network[NetworkPhrase.RemoveBlock] = this.RemoveBlock;
            this.network[NetworkPhrase.SetBlock] = this.SetBlock;
        }

        private void SetBlock(BinaryReader reader)
        {
            int x = reader.ReadInt32();
            int y = reader.ReadInt32();
            int z = reader.ReadInt32();
            string typeName = reader.ReadString();
            Type type = Type.GetType(typeName);
            if (type == null)
                throw new InvalidDataException();
            Block block = Activator.CreateInstance(type) as Block;
            block.Deserialize(reader);
            this.world[x, y, z] = block;
        }

        private void RemoveBlock(BinaryReader reader)
        {
            int x = reader.ReadInt32();
            int y = reader.ReadInt32();
            int z = reader.ReadInt32();
            this.world[x, y, z] = null;
        }

        private void SpawnPlayer(BinaryReader reader)
        {
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            float z = reader.ReadSingle();
            this.CreatePlayer(new JVector(x, y, z));
        }

        private void LoadWorldFromNetwork(BinaryReader reader)
        {
            this.world.Load(reader.BaseStream, true);
        }

        void CreatePlayer(JVector spawn)
        {
            this.player = new Player(this.world);
            this.player.Tool = new BlockPlaceTool(this.network, this.world);
            this.player.Position = spawn;

            this.world.AddBody(this.player);
            this.world.AddConstraint(new Jitter.Dynamics.Constraints.SingleBody.FixedAngle(this.player));
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

            this.network.Dispatch();

            if (this.player != null)
                this.player.UpdateFrame(input, time);

            lock (typeof(World))
            {
                this.world.Step((float)time, true);
            }

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

            var cam = this.player?.Camera ?? new StaticCamera()
            {
                Eye = new Vector3(0, 8, 0),
                Target = new Vector3(16, 2, 16)
            };

            Matrix4 worldViewProjection =
                Matrix4.Identity *
                cam.CreateViewMatrix() *
                cam.CreateProjectionMatrix(1280.0f / 720.0f); // HACK: Hardcoded aspect

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

                this.renderer.Render(cam, time);

                GL.BindTexture(TextureTarget.Texture2DArray, 0);
                GL.UseProgram(0);
            }

            foreach (RigidBody body in this.world.RigidBodies)
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

            this.debug.Render(cam, time);
        }
    }
}
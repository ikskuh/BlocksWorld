using Jitter.Collision;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;

namespace BlocksWorld
{
    public class WorldScene : Scene
    {
        class Focus
        {
            public JVector Position
            {
                get; set;
            }

            public JVector Normal
            {
                get; set;
            }
        }
        
        World world;
        Camera camera;
        WorldRenderer renderer;
        DebugRenderer debug;

        int objectShader;

        double totalTime = 0.0;
        private RigidBody player;
        private Focus focus;
        private TextureArray textures;

        public WorldScene()
        {
            this.camera = new StaticCamera()
            {
                Eye = new Vector3(-8, 6, -8),
                Target = new Vector3(16, 1, 16),
            };

            this.world = new World(32, 16, 32);

            this.debug = new DebugRenderer();
            this.renderer = new WorldRenderer(this.world);

            for (int x = 0; x < this.world.SizeX; x++)
            {
                for (int z = 0; z < this.world.SizeZ; z++)
                {
                    this.world[x, 0, z] = new BasicBlock(2);
                }
            }

            this.world[1, 1, 1] = new BasicBlock(1);

            for (int x = 0; x < this.world.SizeX; x++)
            {
                for (int y = 1; y < 4; y++)
                {
                    if ((x != 16) || (y >= 3))
                        this.world[x, y, 8] = new BasicBlock(3);
                }
            }
            
            {
                var shape = new CapsuleShape(0.9f, 0.4f);

                this.player = new RigidBody(shape);
                this.player.Position = new JVector(16, 4, 16);
                this.player.Material = new Material()
                {
                    StaticFriction = 0.0f,
                    KineticFriction = 0.3f,
                    Restitution = 0.1f
                };
                this.player.AllowDeactivation = false;

                this.world.AddBody(this.player);
                this.world.AddConstraint(new Jitter.Dynamics.Constraints.SingleBody.FixedAngle(this.player));
            }

            this.camera = new FirstPersonCamera(this.player);
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

            {
                var cam = this.camera as StaticCamera;
                if (cam != null)
                {
                    Vector3 offset = Vector3.Zero;

                    offset.X = 24.0f * (float)(Math.Sin(this.totalTime));
                    offset.Y = 8.0f;
                    offset.Z = 24.0f * (float)(Math.Cos(this.totalTime));

                    cam.Eye = cam.Target + offset;
                }
            }
            {
                var cam = this.camera as FirstPersonCamera;
                if (cam != null)
                {
                    cam.Pan -= 0.5f * input.MouseMovement.X;
                    cam.Tilt -= 0.5f * input.MouseMovement.Y;

                    cam.Pan %= 360.0f;
                    cam.Tilt = MathHelper.Clamp(cam.Tilt, -80, 80);

                    Vector3 forward = Vector3.Zero;

                    forward.X = 1.0f * (float)(Math.Sin(MathHelper.DegreesToRadians(cam.Pan)));
                    forward.Y = 0.0f;
                    forward.Z = 1.0f * (float)(Math.Cos(MathHelper.DegreesToRadians(cam.Pan)));

                    Vector3 right = Vector3.Cross(forward, Vector3.UnitY);

                    Vector3 move = Vector3.Zero;
                    if (input.GetButton(Key.W)) move += forward;
                    if (input.GetButton(Key.S)) move -= forward;
                    if (input.GetButton(Key.D)) move += right;
                    if (input.GetButton(Key.A)) move -= right;

                    var vel = this.player.LinearVelocity;
                    vel.X = 150.0f * move.X * (float)time;
                    vel.Z = 150.0f * move.Z * (float)time;
                    this.player.LinearVelocity = vel;
                }
            }

            this.focus = this.TraceFromScreen(this.camera as FirstPersonCamera);

            if (input.GetButtonDown(Key.Space))
            {
                RaycastCallback callback = (b, n, f) =>
                {
                    return b.IsStatic;
                };
                RigidBody body;
                JVector normal;
                float friction;

                if (this.world.CollisionSystem.Raycast(
                    this.player.Position,
                    new JVector(0, -1, 0),
                    callback,
                    out body,
                    out normal,
                    out friction))
                {
                    if (friction < 0.9f)
                    {
                        this.player.AddForce(new JVector(0, 200, 0));
                        Console.WriteLine("{0} {1} {2}", body, normal, friction);
                    }
                }
            }

            if((this.focus != null) && input.GetMouseDown(MouseButton.Left))
            {
                JVector block = this.focus.Position + 0.5f * this.focus.Normal + 0.5f * JVector.One;

                int x = (int)block.X;
                int y = (int)block.Y;
                int z = (int)block.Z;

                this.world[x, y, z] = new BasicBlock(0);
            }
            if ((this.focus != null) && input.GetMouseDown(MouseButton.Right))
            {
                JVector block = this.focus.Position - 0.5f * this.focus.Normal + 0.5f * JVector.One;

                int x = (int)block.X;
                int y = (int)block.Y;
                int z = (int)block.Z;

                this.world[x, y, z] = null;
            }

            this.world.Step((float)time, false);


            if(this.player.Position.Y < -10)
            {
                this.player.Position = new JVector(16, 4, 16);
                this.player.LinearVelocity = JVector.Zero;
            }
        }

        private Focus TraceFromScreen(FirstPersonCamera firstPersonCamera)
        {
            if (firstPersonCamera == null)
                return null;

            RaycastCallback callback = (b, n, f) =>
            {
                return b.IsStatic;
            };
            RigidBody body;
            JVector normal;
            float friction;

            var from = firstPersonCamera.GetEye().Jitter();
            var dir = firstPersonCamera.GetForward().Jitter();

            if (this.world.CollisionSystem.Raycast(
                from,
                dir,
                callback,
                out body,
                out normal,
                out friction))
            {
                return new Focus()
                {
                    Position = from + friction * dir,
                    Normal = normal
                };
            }
            return null;
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
                this.camera.CreateViewMatrix() *
                this.camera.CreateProjectionMatrix(1280.0f / 720.0f); // HACK: Hardcoded aspect
            
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

                this.renderer.Render(this.camera, time);

                GL.BindTexture(TextureTarget.Texture2DArray, 0);
                GL.UseProgram(0); 
            }

            // this.debug.DrawLine(new JVector(10, 4, 10), new JVector(20, 4, 20));

            foreach(RigidBody body in this.world.RigidBodies)
            {
                body.DebugDraw(this.debug);
            }

            if (this.focus != null)
            {
                this.debug.DrawPoint(this.focus.Position);
                this.debug.DrawLine(this.focus.Position, this.focus.Position + 0.25f * this.focus.Normal);
            }
            
            this.debug.Render(this.camera, time);
        }
    }
}
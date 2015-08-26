using Jitter.Collision;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;

namespace BlocksWorld
{
    public class WorldScene : Scene
    {
        Jitter.World pworld;
        World world;
        Camera camera;
        WorldRenderer renderer;

        int objectShader;

        double totalTime = 0.0;
        private RigidBody player;

        public WorldScene()
        {
            this.camera = new StaticCamera()
            {
                Eye = new Vector3(-8, 6, -8),
                Target = new Vector3(16, 1, 16),
            };

            this.world = new World(32, 16, 32);

            this.renderer = new WorldRenderer(this.world);

            for (int x = 0; x < this.world.SizeX; x++)
            {
                for (int z = 0; z < this.world.SizeZ; z++)
                {
                    this.world[x, 0, z] = new BasicBlock(new Vector3(0.54f, 0.27f, 0.07f));
                }
            }

            this.world[1, 1, 1] = new BasicBlock(Vector3.UnitY);

            for (int x = 0; x < this.world.SizeX; x++)
            {
                for (int y = 1; y < 4; y++)
                {
                    if ((x != 16) || (y >= 3))
                        this.world[x, y, 8] = new BasicBlock(0.8f * Vector3.One);
                }
            }



            this.pworld = new Jitter.World(new Jitter.Collision.CollisionSystemPersistentSAP());

            for (int x = 0; x < this.world.SizeX; x++)
            {
                for (int y = 0; y < this.world.SizeY; y++)
                {
                    for (int z = 0; z < this.world.SizeZ; z++)
                    {
                        var block = this.world[x, y, z];
                        if (block == null)
                            continue;

                        RigidBody rb = new RigidBody(new BoxShape(1.0f, 1.0f, 1.0f));
                        rb.Position = new JVector(x, y, z);
                        rb.IsStatic = true;
                        this.pworld.AddBody(rb);
                    }
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

                this.pworld.AddBody(this.player);
                this.pworld.AddConstraint(new Jitter.Dynamics.Constraints.SingleBody.FixedAngle(this.player));
            }

            this.camera = new FirstPersonCamera(this.player);
        }

        public override void Load()
        {
            this.objectShader = Shader.CompileFromResource(
                "BlocksWorld.Shaders.Object.vs",
                "BlocksWorld.Shaders.Object.fs");

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
                    cam.Pan -= 0.5f * input.Mouse.XDelta;
                    cam.Tilt -= 0.5f * input.Mouse.YDelta;

                    cam.Pan %= 360.0f;
                    cam.Tilt = MathHelper.Clamp(cam.Tilt, -80, 80);

                    Vector3 forward = Vector3.Zero;

                    forward.X = 1.0f * (float)(Math.Sin(MathHelper.DegreesToRadians(cam.Pan)));
                    forward.Y = 0.0f;
                    forward.Z = 1.0f * (float)(Math.Cos(MathHelper.DegreesToRadians(cam.Pan)));

                    Vector3 right = Vector3.Cross(forward, Vector3.UnitY);

                    Vector3 move = Vector3.Zero;
                    if (input.Keyboard[Key.W]) move += forward;
                    if (input.Keyboard[Key.S]) move -= forward;
                    if (input.Keyboard[Key.D]) move += right;
                    if (input.Keyboard[Key.A]) move -= right;

                    this.player.IsActive = true;
                    // this.player.AddForce((5000.0f * move * (float)time).Jitter());

                    var vel = this.player.LinearVelocity;
                    vel.X = 150.0f * move.X * (float)time;
                    vel.Z = 150.0f * move.Z * (float)time;
                    this.player.LinearVelocity = vel;
                }
            }

            if (input.Keyboard[Key.Space])
            {
                RaycastCallback callback = (b, n, f) =>
                {
                    return b.IsStatic;
                };
                RigidBody body;
                JVector normal;
                float friction;

                if (this.pworld.CollisionSystem.Raycast(
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

            this.pworld.Step((float)time, false);
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
            if (loc >= 0)
            {
                GL.UniformMatrix4(loc, false, ref worldViewProjection);
            }

            this.renderer.Render(this.camera, time);

            GL.UseProgram(0);
        }
    }
}
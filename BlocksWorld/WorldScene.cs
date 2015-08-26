using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace BlocksWorld
{
    public class WorldScene : Scene
    {
        World world;
        Camera camera;
        WorldRenderer renderer;

        int objectShader;

        public WorldScene()
        {
            this.camera = new StaticCamera()
            {
                Eye = new Vector3(-8, 6, -8),
                Target = new Vector3(8, 1, 8),
            };
            this.world = new World(32, 16, 32);
            this.renderer = new WorldRenderer(this.world);

            for(int x = 0; x < this.world.SizeX; x++)
            {
                for (int z = 0; z < this.world.SizeZ; z++)
                {
                    this.world[x, 0, z] = new Block()
                    {

                    };
                }
            }
            this.world[1, 1, 1] = new Block()
            {

            };
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
            base.UpdateFrame(time);
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
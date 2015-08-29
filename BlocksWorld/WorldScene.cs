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
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace BlocksWorld
{
    public class WorldScene : Scene, IInteractiveEnvironment
    {
        World world;
        WorldRenderer renderer;
        UIRenderer ui;
        DebugRenderer debug;

        int objectShader;

        double totalTime = 0.0;
        private Player player;
        private TextureArray textures;

        private Network network;
        private BasicReceiver receiver;

        MeshModel playerModel;

        Dictionary<int, Proxy> proxies = new Dictionary<int, Proxy>();

        Dictionary<string, MeshModel> models = new Dictionary<string, MeshModel>();

        int currentTool = 0;
        List<Tuple<int, Tool>> tools = new List<Tuple<int, Tool>>();

        int networkUpdateCounter = 0;
        private PhraseTranslator sender;

        public WorldScene()
        {
            this.world = new World();
            this.world.DetailInterationTriggered += World_DetailInterationTriggered;
            this.debug = new DebugRenderer();
            this.renderer = new WorldRenderer(this.world);
            this.ui = new UIRenderer();

            this.network = new Network(new TcpClient("localhost", 4523));
            this.receiver = new BasicReceiver(this.network, this.world);
            this.sender = new PhraseTranslator(this.network);

            this.network[NetworkPhrase.LoadWorld] = this.LoadWorldFromNetwork;
            this.network[NetworkPhrase.SpawnPlayer] = this.SpawnPlayer;
            this.network[NetworkPhrase.UpdateProxy] = this.UpdateProxy;
            this.network[NetworkPhrase.DestroyProxy] = this.DestroyProxy;

            this.network[NetworkPhrase.CreateDetail] = this.CreateDetail;
            this.network[NetworkPhrase.UpdateDetail] = this.UpdateDetail;
            this.network[NetworkPhrase.DestroyDetail] = this.DestroyDetail;

            this.network[NetworkPhrase.SetInteractions] = this.SetInteractions;
        }

        private void SetInteractions(BinaryReader reader)
        {
            int id = reader.ReadInt32();
            int count = reader.ReadInt32();
            List<string> strings = new List<string>();
            for (int i = 0; i < count; i++)
            {
                strings.Add(reader.ReadString());
            }
            var detail = this.world.GetDetail(id);
            if (detail == null)
                return;

            detail.Interactions.Clear();
            foreach (var i in strings)
                detail.Interactions.Add(i);
        }

        private void World_DetailInterationTriggered(object sender, DetailInteractionEventArgs e)
        {
            this.sender.TriggerInteraction(e.Detail, e.Interaction);
        }

        private void CreateDetail(BinaryReader reader)
        {
            int id = reader.ReadInt32();

            string model = reader.ReadString();
            var pos = reader.ReadVector3();
            float rot = reader.ReadSingle();

            DetailObject obj = new DetailObject(id);
            obj.Position = pos;
            obj.Rotation = rot;
            obj.Model = model;

            this.world.RegisterDetail(obj);
        }

        private void DestroyDetail(BinaryReader reader)
        {
            int id = reader.ReadInt32();
            this.world.RemoveDetail(id);
        }

        private void UpdateDetail(BinaryReader reader)
        {
            int id = reader.ReadInt32();
            var pos = reader.ReadVector3();
            float rot = reader.ReadSingle();

            DetailObject obj = this.world.GetDetail(id);
            if (obj == null)
                return;
            obj.Position = pos;
            obj.Rotation = rot;
        }

        private MeshModel GetModelFromName(string model)
        {
            if (this.models.ContainsKey(model) == false)
            {
                this.models[model] = MeshModel.LoadFromFile("./Models/" + Path.GetFileNameWithoutExtension(model) + ".bwm");
            }
            return this.models[model];
        }

        private void UpdateProxy(BinaryReader reader)
        {
            int id = reader.ReadInt32();
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            float z = reader.ReadSingle();
            float rot = reader.ReadSingle();

            // Takes creation into account as well
            var pt = new Vector3(x, y, z);
            if (proxies.ContainsKey(id) == false)
                proxies.Add(id, new Proxy(pt, rot));
            proxies[id].Set(pt, rot);
        }

        private void DestroyProxy(BinaryReader reader)
        {
            int id = reader.ReadInt32();
            proxies.Remove(id);
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
            this.tools.Add(new Tuple<int, Tool>(1, new BlockPlaceTool(this)));
            this.tools.Add(new Tuple<int, Tool>(2, new SpawnTool(this)));

            this.player = new Player(this.world);
            this.player.Position = spawn;

            this.world.AddBody(this.player);
            this.world.AddConstraint(new Jitter.Dynamics.Constraints.SingleBody.FixedAngle(this.player));
        }

        protected override void OnLoad()
        {
            this.objectShader = Shader.CompileFromResource(
                "BlocksWorld.Shaders.Object.vs",
                "BlocksWorld.Shaders.Object.fs");

            this.textures = TextureArray.LoadFromResource("BlocksWorld.Textures.Blocks.png");

            this.playerModel = MeshModel.LoadFromResource(
                "BlocksWorld.Models.Player.bwm");

            this.debug.Load();
            this.renderer.Load();
            this.ui.Load();
        }

        public override void UpdateFrame(IGameInputDriver input, double time)
        {
            this.totalTime += time;

            this.network.Dispatch();

            foreach (var proxy in this.proxies)
                proxy.Value.UpdateFrame(input, time);

            if (input.GetButtonDown(Key.Q) && (this.currentTool > 0))
                this.currentTool -= 1;
            if (input.GetButtonDown(Key.E) && (this.currentTool < (this.tools.Count - 1)))
                this.currentTool += 1;

            if (this.player != null)
            {
                this.player.Tool = this.tools[this.currentTool].Item2;
                this.player.UpdateFrame(input, time);
            }
            lock (typeof(World))
            {
                this.world.Step((float)time, true);
            }

            if (input.GetButtonDown(Key.F5))
                this.world.Save("world.dat");

            this.networkUpdateCounter++;
            if (this.networkUpdateCounter > 5)
            {
                this.sender.SetPlayer(this.player.FeetPosition, this.player.BodyRotation);
                this.networkUpdateCounter = 0;
            }
        }

        public override void RenderFrame(double time)
        {
            this.ui.Reset();
            this.debug.Reset();

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);

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

                int loc = GL.GetUniformLocation(this.objectShader, "uTextures");
                if (loc >= 0)
                {
                    GL.Uniform1(loc, 0);
                }

                loc = GL.GetUniformLocation(this.objectShader, "uWorldViewProjection");
                if (loc >= 0)
                {
                    GL.UniformMatrix4(loc, false, ref worldViewProjection);
                }

                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2DArray, this.textures.ID);

                this.renderer.Render(cam, time);

                foreach (var detail in this.world.Details)
                {
                    if (detail.Model == null)
                        continue;
                    this.RenderDetail(cam, detail, loc, time);
                }

                foreach (var player in this.proxies)
                {
                    RenderPlayer(cam, player.Value.CurrentPosition, player.Value.CurrentBodyRotation, loc, time);
                }
                this.RenderPlayer(cam, this.player.FeetPosition, this.player.BodyRotation, loc, time);

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

            {
                for (int i = 0; i < this.tools.Count; i++)
                {
                    Vector2 pos = new Vector2(32 + 64 * i, 32);
                    if (i == this.currentTool)
                        this.ui.Draw(3, pos, new Vector2(64, 64), ImageAnchor.TopLeft);
                    this.ui.Draw(this.tools[i].Item1, pos, new Vector2(64, 64), ImageAnchor.TopLeft);
                }
                this.ui.Draw(0, new Vector2(640, 360), new Vector2(64, 64), ImageAnchor.MiddleCenter);
            }
            this.ui.Render(cam, time);
        }

        private void RenderDetail(Camera cam, DetailObject detail, int loc, double time)
        {
            Matrix4 worldViewProjection =
                Matrix4.CreateRotationY(detail.Rotation) *
                Matrix4.CreateTranslation(detail.Position) *
                cam.CreateViewMatrix() *
                cam.CreateProjectionMatrix(1280.0f / 720.0f); // HACK: Hardcoded aspect
            if (loc >= 0)
            {
                GL.UniformMatrix4(loc, false, ref worldViewProjection);
            }

            var model = this.GetModelFromName(detail.Model);
            if (model != null)
            {
                model.Render(cam, time);
            }
        }

        private void RenderPlayer(Camera cam, Vector3 position, float rotation, int loc, double time)
        {
            Matrix4 worldViewProjection =
                Matrix4.CreateRotationY(rotation) *
                Matrix4.CreateTranslation(position) *
                cam.CreateViewMatrix() *
                cam.CreateProjectionMatrix(1280.0f / 720.0f); // HACK: Hardcoded aspect
            if (loc >= 0)
            {
                GL.UniformMatrix4(loc, false, ref worldViewProjection);
            }

            this.playerModel.Render(cam, time);
        }

        public Network Network
        {
            get { return this.network; }
        }

        public PhraseTranslator Server
        {
            get { return this.sender; }
        }

        public World World
        {
            get { return this.world; }
        }
    }
}
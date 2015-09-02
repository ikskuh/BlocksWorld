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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace BlocksWorld
{
	public sealed class WorldScene : Scene, IInteractiveEnvironment
	{
		World world;
		WorldRenderer renderer;
		UIRenderer ui;
		DebugRenderer debug;

		Shader objectShader;

		double totalTime = 0.0;
		private Player player;
		private TextureArray blockTextures;
		private TextureArray modelTextures;

		private Network network;
		private BasicReceiver receiver;

		MeshModel playerModel;

		Dictionary<int, Proxy> proxies = new Dictionary<int, Proxy>();

		Dictionary<string, MeshModel> models = new Dictionary<string, MeshModel>();

		int currentTool = 0;
		List<Tuple<int, Tool>> tools = new List<Tuple<int, Tool>>();

		int networkUpdateCounter = 0;
		private PhraseTranslator sender;
		private int fps;
		private Font largeFont;
		private Image uiTextures;
		private Font smallFont;
		
		private DetailObject selectedDetail;
		private int selectedDetailInteraction;
		private HashSet<DetailObject> visibleDetails = new HashSet<DetailObject>();

		public WorldScene()
		{
			this.world = new World();
			this.world.DetailInterationTriggered += World_DetailInterationTriggered;
			this.debug = new DebugRenderer();
			this.renderer = new WorldRenderer(this.world);

			using (var stream = typeof(WorldScene).Assembly.GetManifestResourceStream("BlocksWorld.Textures.UI.png"))
			{
				this.uiTextures = Image.FromStream(stream);
			}

			this.largeFont = new Font(FontFamily.GenericSansSerif, 16.0f);
			this.smallFont = new Font(FontFamily.GenericSansSerif, 8.0f);
			this.ui = new UIRenderer(1280, 720);
			this.ui.Paint += Ui_Paint;

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

		private void Ui_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			var g = e.Graphics;
			g.Clear(Color.Transparent);


			foreach (var detail in this.visibleDetails)
			{
				this.PaintDetailUI(g, e.ClipRectangle.Size, detail, Brushes.DimGray, -1);
			}
			if (this.selectedDetail != null)
			{
				this.PaintDetailUI(g, e.ClipRectangle.Size, this.selectedDetail, Brushes.LightGray, this.selectedDetailInteraction);
			}

			g.DrawString(string.Format("{0} FPS", this.fps), this.largeFont, Brushes.White, new Point(32, 32));

			int c = this.tools.Count;
			var rectFull = new Rectangle(32, 32, 64 * c, 64);
			var rectCurrent = new Rectangle(32 + 64 * this.currentTool, 32, 64, 64);
			g.FillRectangle(Brushes.White, rectFull);
			g.FillRectangle(Brushes.Wheat, rectCurrent);

			for (int i = 0; i < c; i++)
			{
				var rectItem = new Rectangle(32 + 64 * i, 32, 64, 64);
				e.Graphics.DrawImage(
					this.uiTextures,
					rectItem,
					new Rectangle(0, 256 * this.tools[i].Item1, 256, 256),
					GraphicsUnit.Pixel);
			}

			g.DrawRectangle(Pens.Black, rectFull);

			g.DrawImage(this.uiTextures,
				new Rectangle(640 - 32, 360 - 32, 64, 64),
				new Rectangle(0, 0, 256, 256),
				GraphicsUnit.Pixel);
		}

		private void PaintDetailUI(Graphics g, Size screen, DetailObject detail, Brush background, int selection)
		{
			if (this.player == null)
				return;

			if (detail.Interactions.Count == 0)
				return;

			var screenSpace = this.player.Camera.WorldToScreen(detail.Position, this.Aspect);
			if ((screenSpace.Z < 0.0f) || (screenSpace.Z > 1.0f))
				return;

			int x = (int)(screen.Width * (0.5f + 0.5f * screenSpace.X));
			int y = (int)(screen.Height * (0.5f - 0.5f * screenSpace.Y));

			var state = g.Transform;
			g.TranslateTransform(x, y);

			var font = this.smallFont;
			RectangleF area = new RectangleF();
			var interactions = detail.Interactions.ToArray();
			for (int i = 0; i < interactions.Length; i++)
			{
				var size = g.MeasureString(interactions[i].Name, font);

				area.Width = Math.Max(size.Width, area.Width);
				area.Height += size.Height + 1;
			}

			g.FillRectangle(background, area);
			float pointer = 0.0f;
			for (int i = 0; i < interactions.Length; i++)
			{
				var text = interactions[i].Name;
				var size = g.MeasureString(text, font);

				if (i == selection)
				{
					var sel = new RectangleF(0, pointer, area.Width, size.Height);
					g.FillRectangle(Brushes.LightGreen, sel);
				}

				g.DrawString(text, font, Brushes.Black, 0, pointer);
				pointer += size.Height;
			}
			g.DrawRectangle(Pens.Black, area.X, area.Y, area.Width, area.Height);

			g.Transform = state;
		}

		private void SetInteractions(BinaryReader reader)
		{
			int id = reader.ReadInt32();
			int count = reader.ReadInt32();
			List<Interaction> strings = new List<Interaction>();
			for (int i = 0; i < count; i++)
			{
				int iid = reader.ReadInt32();
				string name = reader.ReadString();
				strings.Add(new Interaction(iid, name));
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
			var rot = reader.ReadVector3();

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
			var rot = reader.ReadVector3();

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

			this.blockTextures = TextureArray.LoadFromResource(
				"BlocksWorld.Textures.Blocks.png");
			this.modelTextures = TextureArray.LoadFromResource(
				"BlocksWorld.Textures.Models.png");

			this.playerModel = MeshModel.LoadFromResource(
				"BlocksWorld.Models.Player.bwm");

			this.debug.Load();
			this.renderer.Load();
			this.ui.Load();
		}

		public override void UpdateFrame(IGameInputDriver input, double time)
		{
			this.fps = (int)(1.0f / time);
			this.totalTime += time;

			this.network.Dispatch();

			foreach (var proxy in this.proxies)
				proxy.Value.UpdateFrame(input, time);

			if (input.GetButtonDown(Key.Q))
			{
				this.currentTool = (this.currentTool + 1) % this.tools.Count;
			}

			if (this.player != null)
			{
				this.player.Tool = this.tools[this.currentTool].Item2;
				this.player.UpdateFrame(input, time);

				var previous = this.selectedDetail;
				this.selectedDetail = null;
				this.visibleDetails.Clear();

				var from = this.player.Eye;
				foreach (var detail in this.world.Details)
				{
					// Check for visibility with trace
					var to = detail.Position;
					var dir = (to - from).Normalized();

					var hit = this.world.Trace(from.Jitter(), dir.Jitter(), TraceOptions.IgnoreDynamic);

					if ((hit != null) && ((hit.Body?.Tag as DetailObject) != detail))
						continue;

					var dist = (detail.Position - from).Length;
					if (dist > 3.5f) // TODO: Implement settings value
						continue;

					this.visibleDetails.Add(detail);

					if (this.selectedDetail != null)
					{
						var center = this.player.Camera.WorldToScreen(detail.Position, this.Aspect).Xy.Length;
						var currentCenter = this.player.Camera.WorldToScreen(this.selectedDetail.Position, this.Aspect).Xy.Length;

						if (currentCenter < center)
							continue;
					}
					this.selectedDetail = detail;
				}
				this.visibleDetails.Remove(this.selectedDetail);

				// Reset interaction on change
				if (previous != this.selectedDetail)
					this.selectedDetailInteraction = 0;

				if (this.selectedDetail != null)
				{

					// Detail interaction selection with moues wheel.
					int c = this.selectedDetail.Interactions.Count;
					if (c > 1)
					{
						if (input.MouseWheel > 0)
							this.selectedDetailInteraction -= 1;
						if (input.MouseWheel < 0)
							this.selectedDetailInteraction += 1;

						while (this.selectedDetailInteraction < 0)
							this.selectedDetailInteraction += c;
						while (this.selectedDetailInteraction >= c)
							this.selectedDetailInteraction -= c;
					}

					// Detail interaction with Key.E
					if (input.GetButtonDown(Key.E))
					{
						this.Server.TriggerInteraction(
							this.selectedDetail, 
							this.selectedDetail.Interactions[this.selectedDetailInteraction]);
					}
				}

			}
			lock (typeof(World))
			{
				this.world.Step((float)time, true);
			}

			this.networkUpdateCounter++;
			if (this.networkUpdateCounter > 5)
			{
				this.sender.SetPlayer(this.player.FeetPosition, this.player.BodyRotation);
				this.networkUpdateCounter = 0;
			}
		}

		public override void RenderFrame(double time)
		{
			this.debug.Reset();

			GL.Enable(EnableCap.CullFace);
			GL.CullFace(CullFaceMode.Back);
			GL.Disable(EnableCap.Blend);
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

			Matrix4 world = Matrix4.Identity;
            Matrix4 worldViewProjection =
				world *
				cam.CreateViewMatrix() *
				cam.CreateProjectionMatrix(this.Aspect);

			// Draw world
			{
				this.objectShader.UseProgram();
				GL.Uniform1(this.objectShader["uTextures"], 0);
				GL.UniformMatrix4(this.objectShader["uWorld"], false, ref world);
				GL.UniformMatrix4(this.objectShader["uWorldViewProjection"], false, ref worldViewProjection);

				GL.ActiveTexture(TextureUnit.Texture0);
				GL.BindTexture(TextureTarget.Texture2DArray, this.blockTextures.ID);

				this.renderer.Render(cam, time);
				
				GL.ActiveTexture(TextureUnit.Texture0);
				GL.BindTexture(TextureTarget.Texture2DArray, this.modelTextures.ID);

				foreach (var detail in this.world.Details)
				{
					if (detail.Model == null)
						continue;
					this.RenderDetail(cam, detail, time);
				}

				foreach (var player in this.proxies)
				{
					RenderPlayer(cam, player.Value.CurrentPosition, player.Value.CurrentBodyRotation, time);
				}
				this.RenderPlayer(cam, this.player.FeetPosition, this.player.BodyRotation, time);

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

			this.ui.Render(cam, time);
		}

		private void RenderDetail(Camera cam, DetailObject detail, double time)
		{
			Matrix4 world =
				Matrix4.CreateRotationX(detail.Rotation.X) *
				Matrix4.CreateRotationY(detail.Rotation.Y) *
				Matrix4.CreateRotationZ(detail.Rotation.Z) *
				Matrix4.CreateTranslation(detail.Position);

			Matrix4 worldViewProjection =
				world *
				cam.CreateViewMatrix() *
				cam.CreateProjectionMatrix(this.Aspect);

			GL.UniformMatrix4(this.objectShader["uWorld"], false, ref world);
			GL.UniformMatrix4(this.objectShader["uWorldViewProjection"], false, ref worldViewProjection);

			var model = this.GetModelFromName(detail.Model);
			if (model != null)
			{
				model.Render(cam, time);
			}
		}

		private void RenderPlayer(Camera cam, Vector3 position, float rotation, double time)
		{
			Matrix4 world =
				Matrix4.CreateRotationY(rotation) *
				Matrix4.CreateTranslation(position);
			Matrix4 worldViewProjection =
				world * 
				cam.CreateViewMatrix() *
				cam.CreateProjectionMatrix(this.Aspect);

			GL.UniformMatrix4(this.objectShader["uWorld"], false, ref world);
			GL.UniformMatrix4(this.objectShader["uWorldViewProjection"], false, ref worldViewProjection);

			this.playerModel.Render(cam, time);
		}

		protected override void Dispose(bool disposing)
		{
			this.objectShader?.Dispose();
			this.network?.Dispose();
			this.playerModel?.Dispose();
			this.ui?.Dispose();
			this.debug?.Dispose();
			this.renderer?.Dispose();
			foreach (var model in this.models.Values)
				model.Dispose();
			this.blockTextures?.Dispose();
			base.Dispose(disposing);
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

		public float Aspect { get; } = 1280.0f / 720.0f; // HACK: Still hardcoded aspect, but better
	}
}
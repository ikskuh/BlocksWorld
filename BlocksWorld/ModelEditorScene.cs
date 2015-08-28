using Jitter.LinearMath;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System.Windows.Forms;
using System;
using System.Linq;
using System.Drawing;
using System.IO;

namespace BlocksWorld
{
    public sealed class ModelEditorScene : Scene
    {
        DebugRenderer debug;

        int objectShader;
        TextureArray textures;
        MeshModel model;
        StaticCamera camera;

        Form form;
        Panel modelData;

        float pan = 40, tilt = 30;

        public ModelEditorScene()
        {
            this.debug = new DebugRenderer();

            this.camera = new StaticCamera()
            {
                Eye = new Vector3(16, 6, 16),
                Target = new Vector3(0, 0, 0)
            };
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.form = new Form()
            {
                FormBorderStyle = FormBorderStyle.FixedToolWindow,
                Text = "Model Editor",
                ClientSize = new System.Drawing.Size(480, 640),
                ControlBox = false
            };

            this.form.Controls.Add(this.modelData = new Panel()
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(8)
            });

            var menu = new MenuStrip()
            {
                Dock = DockStyle.Top,
            };
            var fileMenu = menu.Items.Add("File") as ToolStripMenuItem;
            fileMenu.DropDownItems.Add("Open", null, this.OpenFile);
            fileMenu.DropDownItems.Add("Save", null, this.SaveFileAs);
            this.form.Controls.Add(menu);

        }

        private void SaveFileAs(object sender, EventArgs e)
        {
            if(this.model == null)
            {
                MessageBox.Show(
                    this.form,
                    "No model opened.",
                    this.form.Text,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            var sfd = new SaveFileDialog();
            sfd.Filter = "BlocksWorld Mesh (*.bwm)|*.bwm";

            if (sfd.ShowDialog() != DialogResult.OK)
                return;

            using (var fs = File.Open(sfd.FileName, FileMode.Create, FileAccess.Write))
            {
                this.model.Serialize(fs);
            }
        }

        private void InitializeModelUI(MeshModel model)
        {
            this.modelData.Controls.Clear();

            var meshes = model.Meshes.ToArray();
            for(int i = 0; i < meshes.Length; i++)
            {
                var mesh = meshes[i];
                GroupBox box = new GroupBox()
                {
                    Dock = DockStyle.Top,
                    Text = "Mesh " + (i.ToString())
                };

                var table = new TableLayoutPanel()
                {
                    Dock = DockStyle.Fill
                };
                table.ColumnCount = 2;
                table.RowCount = 2;
                table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 0.5f));
                table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 0.5f));

                // Texture Editor
                {
                    var label = new Label()
                    {
                        Dock = DockStyle.Fill,
                        AutoSize = false,
                        TextAlign = ContentAlignment.MiddleLeft,
                        Text = "Texture:"
                    };
                    var editor = new NumericUpDown()
                    {
                        Dock = DockStyle.Fill,
                        Minimum = 0,
                        Maximum = this.textures.Count,
                        Value = mesh.Texture,
                        Increment = 1,
                    };

                    editor.ValueChanged += (s, e) =>
                    {
                        mesh.SetTexture((int)editor.Value);
                    };

                    table.Controls.Add(label);
                    table.Controls.Add(editor);

                    table.SetCellPosition(label, new TableLayoutPanelCellPosition(0, 0));
                    table.SetCellPosition(editor, new TableLayoutPanelCellPosition(1, 0));

                    table.RowStyles.Add(new RowStyle(SizeType.Absolute, editor.Height + 2));
                }

                box.Controls.Add(table);
                this.modelData.Controls.Add(box);
            }
        }

        private void OpenFile(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "All Files (*.*)|*.*";
            if (ofd.ShowDialog() != DialogResult.OK)
                return;
            MeshModel model;
            try {
                model = MeshModel.LoadFromFile(ofd.FileName);
            }
            catch(Exception ex)
            {
                MessageBox.Show(
                    this.form,
                    ex.Message,
                    this.form.Text,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
            
            if (this.model != null)
                this.model.Dispose();
            this.model = model;

            this.InitializeModelUI(this.model);
        }

        protected override void OnEnable()
        {
            this.form.Show();
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            this.form.Hide();
            base.OnDisable();
        }

        protected override void OnLoad()
        {
            this.objectShader = Shader.CompileFromResource(
                "BlocksWorld.Shaders.Object.vs",
                "BlocksWorld.Shaders.Object.fs");

            this.textures = TextureArray.LoadFromResource(
                "BlocksWorld.Textures.Blocks.png");

            this.debug.Load();
        }

        public override void UpdateFrame(IGameInputDriver input, double time)
        {
            Application.DoEvents();

            // Move camera
            if (input.GetMouse(OpenTK.Input.MouseButton.Right))
            {
                this.pan -= input.MouseMovement.X;
                this.tilt -= input.MouseMovement.Y;
                this.tilt = MathHelper.Clamp(this.tilt, -89, 89);
            }

            this.camera.Eye = this.camera.Target - 8.0f * this.GetCameraDirection();
        }

        public Vector3 GetCameraDirection()
        {
            Vector3 rot = Vector3.Zero;
            float pan = MathHelper.DegreesToRadians(this.pan);
            float tilt = MathHelper.DegreesToRadians(this.tilt);

            rot.X = (float)(Math.Cos(tilt) * Math.Sin(pan));
            rot.Y = (float)(Math.Sin(tilt));
            rot.Z = (float)(Math.Cos(tilt) * Math.Cos(pan));
            return rot;
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

                if(this.model != null)
                    this.model.Render(this.camera, time);

                GL.BindTexture(TextureTarget.Texture2DArray, 0);
                GL.UseProgram(0);
            }

            // X-Z Cross
            this.debug.DrawLine(
                new JVector(-50, 0, 0),
                new JVector(50, 0, 0));
            this.debug.DrawLine(
                new JVector(0, 0, -50),
                new JVector(0, 0, 50));
            
            // Ground Box Plane
            this.debug.DrawLine(
                new JVector(-0.5f, 0, -0.5f),
                new JVector( 0.5f, 0, -0.5f));
            this.debug.DrawLine(
                new JVector( 0.5f, 0, -0.5f),
                new JVector( 0.5f, 0,  0.5f));
            this.debug.DrawLine(
                new JVector( 0.5f, 0,  0.5f),
                new JVector(-0.5f, 0,  0.5f));
            this.debug.DrawLine(
                new JVector(-0.5f, 0,  0.5f),
                new JVector(-0.5f, 0, -0.5f));

            this.debug.Render(this.camera, time);
        }
    }
}
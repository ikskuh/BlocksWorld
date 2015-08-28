using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlocksWorld
{
    public sealed class TextureArray
    {
        private int id;

        private TextureArray(int id)
        {
            this.id = id;
        }

        public static TextureArray LoadFromFile(string fileName)
        {
            using (var s = File.Open(fileName, FileMode.Open, FileAccess.Read))
            {
                return LoadFromStream(s);
            }
        }

        public static TextureArray LoadFromResource(string resourceName)
        {
            using (var s = typeof(TextureArray).Assembly.GetManifestResourceStream(resourceName))
            {
                return LoadFromStream(s);
            }
        }

        public static TextureArray LoadFromStream(Stream stream)
        {
            using (var bmp = (Bitmap)Image.FromStream(stream))
            {
                var id = GL.GenTexture();

                GL.BindTexture(TextureTarget.Texture2DArray, id);

                var data = bmp.LockBits(
                    new Rectangle(0, 0, bmp.Width, bmp.Height),
                    System.Drawing.Imaging.ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                int count = bmp.Height / bmp.Width;
                GL.TexImage3D(
                    TextureTarget.Texture2DArray,
                    0,
                    PixelInternalFormat.Rgba,
                    bmp.Width,
                    bmp.Width,
                    count,
                    0,
                    PixelFormat.Bgra,
                    PixelType.UnsignedByte,
                    data.Scan0);

                bmp.UnlockBits(data);

                GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);

                GL.GenerateMipmap(GenerateMipmapTarget.Texture2DArray);

                GL.BindTexture(TextureTarget.Texture2DArray, 0);

                return new TextureArray(id)
                {
                    Count = count
                };
            }
        }

        public int ID
        {
            get
            {
                return id;
            }
        }

        public TextureTarget Target { get { return TextureTarget.Texture2DArray; } }

        public int Count { get; private set; }
    }
}

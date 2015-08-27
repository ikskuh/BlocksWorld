using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlocksWorld
{
    public sealed class TextureArray
    {
        int id;

        public TextureArray()
        {
            this.id = GL.GenTexture();
        }
    }
}

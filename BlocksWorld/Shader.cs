using OpenTK.Graphics.OpenGL4;
using System;
using System.IO;
using System.Text;

namespace BlocksWorld
{
    internal class Shader
    {
        internal static int CompileFromResource(string vertexName, string fragmentName)
        {
            string vertexSource, fragmentSource;
            using (var s = new StreamReader(OpenResource(vertexName), Encoding.UTF8))
            {
                vertexSource = s.ReadToEnd();
            }
            using (var s = new StreamReader(OpenResource(fragmentName), Encoding.UTF8))
            {
                fragmentSource= s.ReadToEnd();
            }
            return Shader.CompileFromSource(vertexSource, fragmentSource);
        }

        private static int CompileFromSource(string vertexSource, string fragmentSource)
        {
            int status, vs, fs, prog;

            vs = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vs, 1, new[] { vertexSource }, new[] { vertexSource.Length });
            GL.CompileShader(vs);
            GL.GetShader(vs, ShaderParameter.CompileStatus, out status);
            if (status == 0)
            {
                string log = GL.GetShaderInfoLog(vs);
                GL.DeleteShader(vs);
                throw new InvalidOperationException(log);
            }

            fs = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fs, 1, new[] { fragmentSource }, new[] { fragmentSource.Length });
            GL.CompileShader(fs);
            GL.GetShader(fs, ShaderParameter.CompileStatus, out status);
            if (status == 0)
            {
                GL.DeleteShader(vs);
                string log = GL.GetShaderInfoLog(fs);
                GL.DeleteShader(fs);
                throw new InvalidOperationException(log);
            }

            prog = GL.CreateProgram();
            GL.AttachShader(prog, vs);
            GL.AttachShader(prog, fs);
            GL.LinkProgram(prog);
            GL.GetProgram(prog, GetProgramParameterName.LinkStatus, out status);
            if(status == 0)
            {
                GL.DeleteShader(fs);
                GL.DeleteShader(vs);

                string log = GL.GetProgramInfoLog(prog);
                GL.DeleteProgram(prog);
                throw new InvalidOperationException(log);
            }
            /*
            GL.DetachShader(prog, vs);
            GL.DetachShader(prog, fs);
            GL.DeleteShader(vs);
            GL.DeleteShader(fs);
            */
            return prog;
        }

        private static Stream OpenResource(string fragmentName)
        {
            return typeof(Shader).Assembly.GetManifestResourceStream(fragmentName);
        }
    }
}
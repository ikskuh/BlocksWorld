using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlocksWorld
{
    partial class WorldRenderer
    {
        static readonly Vertex[] topSideTemplate = new[]
        {
            // 0 - 1 - 2
            new Vertex() { position = new Vector3(-0.5f,  0.5f,  0.5f), normal = Vector3.UnitY},
            new Vertex() { position = new Vector3( 0.5f,  0.5f,  0.5f), normal = Vector3.UnitY},
            new Vertex() { position = new Vector3(-0.5f,  0.5f, -0.5f), normal = Vector3.UnitY},
            
            // 1 - 2 - 3
            new Vertex() { position = new Vector3( 0.5f,  0.5f,  0.5f), normal = Vector3.UnitY},
            new Vertex() { position = new Vector3(-0.5f,  0.5f, -0.5f), normal = Vector3.UnitY},
            new Vertex() { position = new Vector3( 0.5f,  0.5f, -0.5f), normal = Vector3.UnitY},
        };

        static readonly Vertex[] bottomSideTemplate = new[]
        {
            // 0 - 1 - 2
            new Vertex() { position = new Vector3(-0.5f, -0.5f,  0.5f), normal = -Vector3.UnitY},
            new Vertex() { position = new Vector3( 0.5f, -0.5f,  0.5f), normal = -Vector3.UnitY},
            new Vertex() { position = new Vector3(-0.5f, -0.5f, -0.5f), normal = -Vector3.UnitY},
            
            // 1 - 2 - 3
            new Vertex() { position = new Vector3( 0.5f, -0.5f,  0.5f), normal = -Vector3.UnitY},
            new Vertex() { position = new Vector3(-0.5f, -0.5f, -0.5f), normal = -Vector3.UnitY},
            new Vertex() { position = new Vector3( 0.5f, -0.5f, -0.5f), normal = -Vector3.UnitY},
        };

        static readonly Vertex[] negativeXSideTemplate = new[]
        {
            // 0 - 1 - 2
            new Vertex() { position = new Vector3(-0.5f, -0.5f,  0.5f), normal = -Vector3.UnitX},
            new Vertex() { position = new Vector3(-0.5f,  0.5f,  0.5f), normal = -Vector3.UnitX},
            new Vertex() { position = new Vector3(-0.5f, -0.5f, -0.5f), normal = -Vector3.UnitX},
            
            // 1 - 2 - 3
            new Vertex() { position = new Vector3(-0.5f,  0.5f,  0.5f), normal = -Vector3.UnitX},
            new Vertex() { position = new Vector3(-0.5f, -0.5f, -0.5f), normal = -Vector3.UnitX},
            new Vertex() { position = new Vector3(-0.5f,  0.5f, -0.5f), normal = -Vector3.UnitX},
        };

        static readonly Vertex[] positiveXSideTemplate = new[]
        {
            // 0 - 1 - 2
            new Vertex() { position = new Vector3( 0.5f, -0.5f,  0.5f), normal = Vector3.UnitX},
            new Vertex() { position = new Vector3( 0.5f,  0.5f,  0.5f), normal = Vector3.UnitX},
            new Vertex() { position = new Vector3( 0.5f, -0.5f, -0.5f), normal = Vector3.UnitX},
            
            // 1 - 2 - 3
            new Vertex() { position = new Vector3( 0.5f,  0.5f,  0.5f), normal = Vector3.UnitX},
            new Vertex() { position = new Vector3( 0.5f, -0.5f, -0.5f), normal = Vector3.UnitX},
            new Vertex() { position = new Vector3( 0.5f,  0.5f, -0.5f), normal = Vector3.UnitX},
        };

        static readonly Vertex[] negativeZSideTemplate = new[]
        {
            // 0 - 1 - 2
            new Vertex() { position = new Vector3(-0.5f,  0.5f, -0.5f), normal = -Vector3.UnitZ},
            new Vertex() { position = new Vector3( 0.5f,  0.5f, -0.5f), normal = -Vector3.UnitZ},
            new Vertex() { position = new Vector3(-0.5f, -0.5f, -0.5f), normal = -Vector3.UnitZ},
            
            // 1 - 2 - 3
            new Vertex() { position = new Vector3( 0.5f,  0.5f, -0.5f), normal = -Vector3.UnitZ},
            new Vertex() { position = new Vector3(-0.5f, -0.5f, -0.5f), normal = -Vector3.UnitZ},
            new Vertex() { position = new Vector3( 0.5f, -0.5f, -0.5f), normal = -Vector3.UnitZ},
        };

        static readonly Vertex[] positiveZSideTemplate = new[]
        {
            // 0 - 1 - 2
            new Vertex() { position = new Vector3(-0.5f,  0.5f,  0.5f), normal = Vector3.UnitZ},
            new Vertex() { position = new Vector3( 0.5f,  0.5f,  0.5f), normal = Vector3.UnitZ},
            new Vertex() { position = new Vector3(-0.5f, -0.5f,  0.5f), normal = Vector3.UnitZ},
            
            // 1 - 2 - 3
            new Vertex() { position = new Vector3( 0.5f,  0.5f,  0.5f), normal = Vector3.UnitZ},
            new Vertex() { position = new Vector3(-0.5f, -0.5f,  0.5f), normal = Vector3.UnitZ},
            new Vertex() { position = new Vector3( 0.5f, -0.5f,  0.5f), normal = Vector3.UnitZ},
        };
    }
}

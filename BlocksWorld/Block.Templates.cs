using OpenTK;

namespace BlocksWorld
{
    partial class BasicBlock
    {
        static readonly WorldRenderer.Vertex[] topSideTemplate = new[]
        {
            // 0 - 1 - 2
            new WorldRenderer.Vertex() { position = new Vector3(-0.5f,  0.5f,  0.5f), normal = Vector3.UnitY, uv = Vector3.Zero },
            new WorldRenderer.Vertex() { position = new Vector3( 0.5f,  0.5f,  0.5f), normal = Vector3.UnitY, uv = Vector3.UnitY },
            new WorldRenderer.Vertex() { position = new Vector3(-0.5f,  0.5f, -0.5f), normal = Vector3.UnitY, uv = Vector3.UnitX },
            
            // 1 - 2 - 3
            new WorldRenderer.Vertex() { position = new Vector3( 0.5f,  0.5f,  0.5f), normal = Vector3.UnitY, uv = Vector3.UnitY },
            new WorldRenderer.Vertex() { position = new Vector3(-0.5f,  0.5f, -0.5f), normal = Vector3.UnitY, uv = Vector3.UnitX },
            new WorldRenderer.Vertex() { position = new Vector3( 0.5f,  0.5f, -0.5f), normal = Vector3.UnitY, uv = Vector3.One },
        };

        static readonly WorldRenderer.Vertex[] bottomSideTemplate = new[]
        {
            // 0 - 1 - 2
            new WorldRenderer.Vertex() { position = new Vector3(-0.5f, -0.5f,  0.5f), normal = -Vector3.UnitY, uv = Vector3.Zero },
            new WorldRenderer.Vertex() { position = new Vector3( 0.5f, -0.5f,  0.5f), normal = -Vector3.UnitY, uv = Vector3.UnitY },
            new WorldRenderer.Vertex() { position = new Vector3(-0.5f, -0.5f, -0.5f), normal = -Vector3.UnitY, uv = Vector3.UnitX },
            
            // 1 - 2 - 3
            new WorldRenderer.Vertex() { position = new Vector3( 0.5f, -0.5f,  0.5f), normal = -Vector3.UnitY, uv = Vector3.UnitY },
            new WorldRenderer.Vertex() { position = new Vector3(-0.5f, -0.5f, -0.5f), normal = -Vector3.UnitY, uv = Vector3.UnitX },
            new WorldRenderer.Vertex() { position = new Vector3( 0.5f, -0.5f, -0.5f), normal = -Vector3.UnitY, uv = Vector3.One },
        };

        static readonly WorldRenderer.Vertex[] negativeXSideTemplate = new[]
        {
            // 0 - 1 - 2
            new WorldRenderer.Vertex() { position = new Vector3(-0.5f, -0.5f,  0.5f), normal = -Vector3.UnitX, uv = Vector3.Zero },
            new WorldRenderer.Vertex() { position = new Vector3(-0.5f,  0.5f,  0.5f), normal = -Vector3.UnitX, uv = Vector3.UnitY },
            new WorldRenderer.Vertex() { position = new Vector3(-0.5f, -0.5f, -0.5f), normal = -Vector3.UnitX, uv = Vector3.UnitX },
            
            // 1 - 2 - 3
            new WorldRenderer.Vertex() { position = new Vector3(-0.5f,  0.5f,  0.5f), normal = -Vector3.UnitX, uv = Vector3.UnitY },
            new WorldRenderer.Vertex() { position = new Vector3(-0.5f, -0.5f, -0.5f), normal = -Vector3.UnitX, uv = Vector3.UnitX },
            new WorldRenderer.Vertex() { position = new Vector3(-0.5f,  0.5f, -0.5f), normal = -Vector3.UnitX, uv = Vector3.One },
        };

        static readonly WorldRenderer.Vertex[] positiveXSideTemplate = new[]
        {
            // 0 - 1 - 2
            new WorldRenderer.Vertex() { position = new Vector3( 0.5f, -0.5f,  0.5f), normal = Vector3.UnitX, uv = Vector3.Zero },
            new WorldRenderer.Vertex() { position = new Vector3( 0.5f,  0.5f,  0.5f), normal = Vector3.UnitX, uv = Vector3.UnitY },
            new WorldRenderer.Vertex() { position = new Vector3( 0.5f, -0.5f, -0.5f), normal = Vector3.UnitX, uv = Vector3.UnitX },
            
            // 1 - 2 - 3
            new WorldRenderer.Vertex() { position = new Vector3( 0.5f,  0.5f,  0.5f), normal = Vector3.UnitX, uv = Vector3.UnitY },
            new WorldRenderer.Vertex() { position = new Vector3( 0.5f, -0.5f, -0.5f), normal = Vector3.UnitX, uv = Vector3.UnitX },
            new WorldRenderer.Vertex() { position = new Vector3( 0.5f,  0.5f, -0.5f), normal = Vector3.UnitX, uv = Vector3.One },
        };

        static readonly WorldRenderer.Vertex[] negativeZSideTemplate = new[]
        {
            // 0 - 1 - 2
            new WorldRenderer.Vertex() { position = new Vector3(-0.5f,  0.5f, -0.5f), normal = -Vector3.UnitZ, uv = Vector3.Zero },
            new WorldRenderer.Vertex() { position = new Vector3( 0.5f,  0.5f, -0.5f), normal = -Vector3.UnitZ, uv = Vector3.UnitX },
            new WorldRenderer.Vertex() { position = new Vector3(-0.5f, -0.5f, -0.5f), normal = -Vector3.UnitZ, uv = Vector3.UnitY },
            
            // 1 - 2 - 3
            new WorldRenderer.Vertex() { position = new Vector3( 0.5f,  0.5f, -0.5f), normal = -Vector3.UnitZ, uv = Vector3.UnitX },
            new WorldRenderer.Vertex() { position = new Vector3(-0.5f, -0.5f, -0.5f), normal = -Vector3.UnitZ, uv = Vector3.UnitY },
            new WorldRenderer.Vertex() { position = new Vector3( 0.5f, -0.5f, -0.5f), normal = -Vector3.UnitZ, uv = Vector3.One },
        };

        static readonly WorldRenderer.Vertex[] positiveZSideTemplate = new[]
        {
            // 0 - 1 - 2
            new WorldRenderer.Vertex() { position = new Vector3(-0.5f,  0.5f,  0.5f), normal = Vector3.UnitZ, uv = Vector3.Zero },
            new WorldRenderer.Vertex() { position = new Vector3( 0.5f,  0.5f,  0.5f), normal = Vector3.UnitZ, uv = Vector3.UnitX },
            new WorldRenderer.Vertex() { position = new Vector3(-0.5f, -0.5f,  0.5f), normal = Vector3.UnitZ, uv = Vector3.UnitY },
            
            // 1 - 2 - 3
            new WorldRenderer.Vertex() { position = new Vector3( 0.5f,  0.5f,  0.5f), normal = Vector3.UnitZ, uv = Vector3.UnitX },
            new WorldRenderer.Vertex() { position = new Vector3(-0.5f, -0.5f,  0.5f), normal = Vector3.UnitZ, uv = Vector3.UnitY },
            new WorldRenderer.Vertex() { position = new Vector3( 0.5f, -0.5f,  0.5f), normal = Vector3.UnitZ, uv = Vector3.One },
        };
    }
}

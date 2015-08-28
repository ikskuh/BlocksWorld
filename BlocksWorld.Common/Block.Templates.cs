using OpenTK;

namespace BlocksWorld
{
    partial class BasicBlock
    {
        static readonly BlockVertex[] topSideTemplate = new[]
        {
            // 0 - 1 - 2
            new BlockVertex() { position = new Vector3(-0.5f,  0.5f,  0.5f), normal = Vector3.UnitY, uv = Vector3.Zero },
            new BlockVertex() { position = new Vector3( 0.5f,  0.5f,  0.5f), normal = Vector3.UnitY, uv = Vector3.UnitY },
            new BlockVertex() { position = new Vector3(-0.5f,  0.5f, -0.5f), normal = Vector3.UnitY, uv = Vector3.UnitX },
            
            // 1 - 2 - 3
            new BlockVertex() { position = new Vector3( 0.5f,  0.5f,  0.5f), normal = Vector3.UnitY, uv = Vector3.UnitY },
            new BlockVertex() { position = new Vector3( 0.5f,  0.5f, -0.5f), normal = Vector3.UnitY, uv = Vector3.One },
            new BlockVertex() { position = new Vector3(-0.5f,  0.5f, -0.5f), normal = Vector3.UnitY, uv = Vector3.UnitX },
        };

        static readonly BlockVertex[] bottomSideTemplate = new[]
        {
            // 0 - 1 - 2
            new BlockVertex() { position = new Vector3(-0.5f, -0.5f,  0.5f), normal = -Vector3.UnitY, uv = Vector3.Zero },
            new BlockVertex() { position = new Vector3(-0.5f, -0.5f, -0.5f), normal = -Vector3.UnitY, uv = Vector3.UnitX },
            new BlockVertex() { position = new Vector3( 0.5f, -0.5f,  0.5f), normal = -Vector3.UnitY, uv = Vector3.UnitY },
            
            // 1 - 2 - 3
            new BlockVertex() { position = new Vector3( 0.5f, -0.5f,  0.5f), normal = -Vector3.UnitY, uv = Vector3.UnitY },
            new BlockVertex() { position = new Vector3(-0.5f, -0.5f, -0.5f), normal = -Vector3.UnitY, uv = Vector3.UnitX },
            new BlockVertex() { position = new Vector3( 0.5f, -0.5f, -0.5f), normal = -Vector3.UnitY, uv = Vector3.One },
        };

        static readonly BlockVertex[] negativeXSideTemplate = new[]
        {
            // 0 - 1 - 2
            new BlockVertex() { position = new Vector3(-0.5f, -0.5f,  0.5f), normal = -Vector3.UnitX, uv = Vector3.Zero },
            new BlockVertex() { position = new Vector3(-0.5f,  0.5f,  0.5f), normal = -Vector3.UnitX, uv = Vector3.UnitY },
            new BlockVertex() { position = new Vector3(-0.5f, -0.5f, -0.5f), normal = -Vector3.UnitX, uv = Vector3.UnitX },
            
            // 1 - 2 - 3
            new BlockVertex() { position = new Vector3(-0.5f,  0.5f,  0.5f), normal = -Vector3.UnitX, uv = Vector3.UnitY },
            new BlockVertex() { position = new Vector3(-0.5f,  0.5f, -0.5f), normal = -Vector3.UnitX, uv = Vector3.One },
            new BlockVertex() { position = new Vector3(-0.5f, -0.5f, -0.5f), normal = -Vector3.UnitX, uv = Vector3.UnitX },
        };

        static readonly BlockVertex[] positiveXSideTemplate = new[]
        {
            // 0 - 1 - 2
            new BlockVertex() { position = new Vector3( 0.5f, -0.5f,  0.5f), normal = Vector3.UnitX, uv = Vector3.Zero },
            new BlockVertex() { position = new Vector3( 0.5f, -0.5f, -0.5f), normal = Vector3.UnitX, uv = Vector3.UnitX },
            new BlockVertex() { position = new Vector3( 0.5f,  0.5f,  0.5f), normal = Vector3.UnitX, uv = Vector3.UnitY },
            
            // 1 - 2 - 3
            new BlockVertex() { position = new Vector3( 0.5f,  0.5f,  0.5f), normal = Vector3.UnitX, uv = Vector3.UnitY },
            new BlockVertex() { position = new Vector3( 0.5f, -0.5f, -0.5f), normal = Vector3.UnitX, uv = Vector3.UnitX },
            new BlockVertex() { position = new Vector3( 0.5f,  0.5f, -0.5f), normal = Vector3.UnitX, uv = Vector3.One },
        };

        static readonly BlockVertex[] negativeZSideTemplate = new[]
        {
            // 0 - 1 - 2
            new BlockVertex() { position = new Vector3(-0.5f,  0.5f, -0.5f), normal = -Vector3.UnitZ, uv = Vector3.Zero },
            new BlockVertex() { position = new Vector3( 0.5f,  0.5f, -0.5f), normal = -Vector3.UnitZ, uv = Vector3.UnitX },
            new BlockVertex() { position = new Vector3(-0.5f, -0.5f, -0.5f), normal = -Vector3.UnitZ, uv = Vector3.UnitY },
            
            // 1 - 2 - 3
            new BlockVertex() { position = new Vector3( 0.5f,  0.5f, -0.5f), normal = -Vector3.UnitZ, uv = Vector3.UnitX },
            new BlockVertex() { position = new Vector3( 0.5f, -0.5f, -0.5f), normal = -Vector3.UnitZ, uv = Vector3.One },
            new BlockVertex() { position = new Vector3(-0.5f, -0.5f, -0.5f), normal = -Vector3.UnitZ, uv = Vector3.UnitY },
        };

        static readonly BlockVertex[] positiveZSideTemplate = new[]
        {
            // 0 - 1 - 2
            new BlockVertex() { position = new Vector3(-0.5f,  0.5f,  0.5f), normal = Vector3.UnitZ, uv = Vector3.Zero },
            new BlockVertex() { position = new Vector3(-0.5f, -0.5f,  0.5f), normal = Vector3.UnitZ, uv = Vector3.UnitY },
            new BlockVertex() { position = new Vector3( 0.5f,  0.5f,  0.5f), normal = Vector3.UnitZ, uv = Vector3.UnitX },
            
            // 1 - 2 - 3
            new BlockVertex() { position = new Vector3( 0.5f,  0.5f,  0.5f), normal = Vector3.UnitZ, uv = Vector3.UnitX },
            new BlockVertex() { position = new Vector3(-0.5f, -0.5f,  0.5f), normal = Vector3.UnitZ, uv = Vector3.UnitY },
            new BlockVertex() { position = new Vector3( 0.5f, -0.5f,  0.5f), normal = Vector3.UnitZ, uv = Vector3.One },
        };
    }
}

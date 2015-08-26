namespace BlocksWorld
{
    public class World
    {
        private Block[,,] blocks;

        public World(int sizeX, int sizeY, int sizeZ)
        {
            this.blocks = new Block[sizeX, sizeY, sizeZ];
        }

        public Block this[int x, int y, int z]
        {
            get
            {
                if ((x < 0) || (y < 0) || (z < 0))
                    return null;
                if ((x >= this.SizeX) || (y >= this.SizeY) || (z >= this.SizeZ))
                    return null;
                return this.blocks[x, y, z];
            }
            set { this.blocks[x, y, z] = value; }
        }

        public int SizeX { get { return this.blocks.GetLength(0); } }
        public int SizeY { get { return this.blocks.GetLength(1); } }
        public int SizeZ { get { return this.blocks.GetLength(2); } }
    }
}
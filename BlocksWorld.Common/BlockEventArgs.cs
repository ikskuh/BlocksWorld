using System;

namespace BlocksWorld
{
    public class BlockEventArgs : EventArgs
    {
        public BlockEventArgs(Block block, int x, int y, int z)
        {
            this.Block = block;
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public Block Block { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Z { get; private set; }
    }
}
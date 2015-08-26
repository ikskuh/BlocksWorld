using System;

namespace BlocksWorld
{
    public class Block
    {
        internal static bool IsSimilar(Block block1, Block block2)
        {
            if ((block1 == null) && (block2 == null))
                return true;
            if ((block1 != null) && (block2 != null))
                return true;
            // TODO: Extend behaviour
            return false;
        }
    }
}
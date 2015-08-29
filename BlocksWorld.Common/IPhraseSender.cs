using System.IO;

namespace BlocksWorld
{
    public delegate void PhraseSender(BinaryWriter writer);

    public interface IPhraseSender
    {
        void Send(NetworkPhrase phrase, PhraseSender sender);
    }
}
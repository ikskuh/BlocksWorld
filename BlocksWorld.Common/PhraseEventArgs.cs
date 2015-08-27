using System;
using System.IO;

namespace BlocksWorld
{
    public class PhraseEventArgs : EventArgs
    {
        public PhraseEventArgs(NetworkPhrase phrase, BinaryReader reader)
        {
            this.Phrase = phrase;
            this.Reader = reader;
        }

        public NetworkPhrase Phrase { get; private set; }
        public BinaryReader Reader { get; private set; }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BlocksWorld
{
    public delegate void PhraseHandler(BinaryReader reader);

    public sealed partial class Network : IPhraseSender, IDisposable
    {
        private readonly TcpClient client;
        private readonly Dictionary<NetworkPhrase, PhraseHandler> dispatcher = new Dictionary<NetworkPhrase, PhraseHandler>();
        private BinaryReader reader;
        private BinaryWriter writer;

        public event EventHandler<PhraseEventArgs> UnknownPhraseReceived;

        public Network(TcpClient client)
        {
            this.client = client;

            var stream = this.client.GetStream();
            this.reader = new BinaryReader(stream, Encoding.UTF8);
            this.writer = new BinaryWriter(stream, Encoding.UTF8);
        }

        ~Network()
        {
            this.Dispose();
        }

        public void Disconnect()
        {
            this.writer = BinaryWriter.Null;
            this.reader = null;
            this.client.Close();
        }

        public void Dispatch()
        {
            if (this.reader == null)
                return;
            while (this.client.Available > 0)
            {
                NetworkPhrase phrase = (NetworkPhrase)reader.ReadInt32();
                if (this.dispatcher.ContainsKey(phrase))
                {
                    this.dispatcher[phrase](this.reader);
                }
                else
                {
                    System.Diagnostics.Trace.WriteLine("Unknown phrase: " + phrase.ToString(), "Network.Dispatch");
                    this.OnUnknownPhraseReceived(phrase);
                }
            }
        }

        public void Send(NetworkPhrase phrase, PhraseSender sender)
        {
            lock (this.writer)
            {
                try
                {
                    this.writer.Write((int)phrase);
                    this.writer.Flush();
                    sender(this.writer);
                }
                catch (IOException ex) when (ex.HResult == -2146232800) // Connection closed
                {
                    // just ignore the exception
                }
            }
        }

        private void OnUnknownPhraseReceived(NetworkPhrase phrase)
        {
            if (this.UnknownPhraseReceived != null)
                this.UnknownPhraseReceived(this, new PhraseEventArgs(phrase, this.reader));
        }

        public void Dispose()
        {
            this.reader.Dispose();
            this.writer.Dispose();
            ((IDisposable)this.client).Dispose();
        }

        public PhraseHandler this[NetworkPhrase phrase]
        {
            get
            {
                if (this.dispatcher.ContainsKey(phrase))
                    return this.dispatcher[phrase];
                else
                    return null;
            }
            set
            {
                this.dispatcher[phrase] = value;
            }
        }

        public BinaryReader Reader
        {
            get
            {
                return reader;
            }
        }

        public BinaryWriter Writer
        {
            get
            {
                return writer;
            }
        }
    }
}

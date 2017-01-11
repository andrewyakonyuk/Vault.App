using System;
using System.Threading.Tasks;
using Orleans.Streams;

namespace Vault.Activity.Streams
{
    public class InMemoryCheckpointer : IStreamQueueCheckpointer<string>
    {
        string _checkpoint = null;

        public bool CheckpointExists
        {
            get
            {
                return !string.IsNullOrEmpty(_checkpoint);
            }
        }

        public Task<string> Load()
        {
            return Task.FromResult(_checkpoint);
        }

        public void Update(string offset, DateTime utcNow)
        {
            _checkpoint = offset;
        }
    }
}
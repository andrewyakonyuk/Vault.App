namespace Vault.Shared.NEventStore
{
    public class InMemoryCheckpointRepository : ICheckpointRepository
    {
        string _checkpointToken;

        public string LoadCheckpoint()
        {
            return _checkpointToken;
        }

        public void SaveCheckpoint(string checkpointToken)
        {
            _checkpointToken = checkpointToken;
        }
    }
}
namespace Vault.Shared.NEventStore
{
    public interface ICheckpointRepository
    {
        string LoadCheckpoint();

        void SaveCheckpoint(string checkpointToken);
    }
}
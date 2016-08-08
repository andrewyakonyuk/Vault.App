namespace Vault.Shared.EventSourcing.NEventStore
{
    public interface ICheckpointRepository
    {
        string LoadCheckpoint();

        void SaveCheckpoint(string checkpointToken);
    }
}
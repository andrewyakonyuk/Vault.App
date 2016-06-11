using System;
using Vault.Shared.TransientFaultHandling;

namespace Vault.Shared.Connectors
{
    public class PullConnectionErrorDetectionStrategy : ITransientErrorDetectionStrategy
    {
        public bool IsTransient(Exception ex)
        {
            return ex is PullConnectionException || ex is System.Net.WebException;
        }
    }
}
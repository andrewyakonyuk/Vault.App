using System;
using Vault.Shared.TransientFaultHandling;

namespace Vault.Activity.Services.Connectors
{
    public class ConnectionErrorDetectionStrategy : ITransientErrorDetectionStrategy
    {
        public bool IsTransient(Exception ex)
        {
            return ex is System.Net.WebException
                || ex is System.Net.HttpListenerException;
        }
    }
}
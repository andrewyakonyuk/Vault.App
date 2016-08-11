using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;
using Vault.Shared.Commands;
using Vault.Shared.EventSourcing;
using Vault.Shared.TransientFaultHandling;

namespace Vault.Shared.Connectors
{
    [PersistJobDataAfterExecution]
    public class PullConnectionJob : IJob, IDisposable
    {
        readonly IDictionary<string, IPullConnectionProvider> _pullProviders;
        readonly CancellationTokenSource _tokenSource;
        readonly ICommandBuilder _commandBuilder;
        readonly ILogger<PullConnectionJob> _logger;

        public string ProviderName { get; set; }
        public string ProviderKey { get; set; }
        public int Iteration { get; set; }
        public int OwnerId { get; set; }

        public PullConnectionJob(
            IEnumerable<IPullConnectionProvider> pullProviders,
            ICommandBuilder commandBuilder,
            ILogger<PullConnectionJob> logger)
        {
            _pullProviders = pullProviders.ToDictionary(t => t.Name);
            _commandBuilder = commandBuilder;
            _logger = logger;
            _tokenSource = new CancellationTokenSource();
        }

        public void Execute(IJobExecutionContext context)
        {
            Task.Factory.StartNew((c) => ExecuteAsync((IJobExecutionContext)c), context, TaskCreationOptions.AttachedToParent);
        }

        public async Task ExecuteAsync(IJobExecutionContext context)
        {
            IPullConnectionProvider provider;
            if (!_pullProviders.TryGetValue(ProviderName, out provider))
                return;

            var retryStrategy = new Incremental(5, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5));
            var retryPolicy = new RetryPolicy<PullConnectionErrorDetectionStrategy>(retryStrategy);

            var connectionContext = new PullConnectionContext(new UserInfo(ProviderKey, OwnerId), _tokenSource.Token)
            {
                LastRunTimeUtc = context.PreviousFireTimeUtc,
                Iteration = Iteration,
                RecoveryMode = context.Recovering
            };

            try
            {
                while (true)
                {
                    var result = await retryPolicy.ExecuteAsync(() => provider.PullAsync(connectionContext), _tokenSource.Token);

                    foreach (var command in result)
                    {
                        _commandBuilder.Execute((dynamic)command);
                    }

                    if (result.IsCancellationRequested)
                    {
                        context.MergedJobDataMap["iteration"] = 0;
                        break;
                    }

                    connectionContext.Iteration++;
                    context.MergedJobDataMap["iteration"] = connectionContext.Iteration;
                }
            }
            catch (PullConnectionException ex)
            {
                _logger.LogError(ex.Message);
                throw new JobExecutionException(ex);
            }
            catch (Exception ex)
            {
                var message = $"Critical exception occur while executing '{ProviderName}' "
                    + $"connection for user '{ProviderKey}' on '{connectionContext?.Iteration}'. ";
                _logger.LogCritical(message);
                throw new JobExecutionException(message, ex);
            }
        }

        public void Dispose()
        {
            //_tokenSource.Dispose();
        }
    }
}
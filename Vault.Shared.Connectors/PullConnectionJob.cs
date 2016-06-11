using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vault.Shared.NEventStore;
using Vault.Shared.TransientFaultHandling;

namespace Vault.Shared.Connectors
{
    [PersistJobDataAfterExecution]
    public class PullConnectionJob : IJob, IDisposable
    {
        readonly IDictionary<string, IPullConnectionProvider> _pullProviders;
        readonly CancellationTokenSource _tokenSource;
        readonly IEventedUnitOfWorkFactory _eventedUnitOfWorkFactory;
        readonly ILogger<PullConnectionJob> _logger;

        public string ProviderName { get; set; }
        public string ProviderKey { get; set; }
        public int Iteration { get; set; }
        public int OwnerId { get; set; }

        public PullConnectionJob(
            IEnumerable<IPullConnectionProvider> pullProviders,
            IEventedUnitOfWorkFactory eventedUnitOfWorkFactory,
            ILogger<PullConnectionJob> logger)
        {
            _pullProviders = pullProviders.ToDictionary(t => t.Name);
            _eventedUnitOfWorkFactory = eventedUnitOfWorkFactory;
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

            var connectionContext = new PullConnectionContext(new UserInfo(ProviderKey), _tokenSource.Token)
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

                    SaveConnectionResult(result);

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

        void SaveConnectionResult(PullConnectionResult result)
        {
            using (var unitOfWork = _eventedUnitOfWorkFactory.Create())
            {
                var stream = unitOfWork.GetStream("activity-" + OwnerId);

                foreach (var item in result)
                {
                    item.OwnerId = OwnerId;
                    stream.Add(item);
                }

                unitOfWork.Save(stream);
                unitOfWork.Commit();
            }
        }

        public void Dispose()
        {
            //_tokenSource.Dispose();
        }
    }
}
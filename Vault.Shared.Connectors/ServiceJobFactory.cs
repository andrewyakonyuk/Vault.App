using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Simpl;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Vault.Shared.Connectors
{
    public class ServiceJobFactory : PropertySettingJobFactory
    {
        IServiceProvider _serviceProvider;

        public ServiceJobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var jobDetail = bundle.JobDetail;
            var jobType = jobDetail.JobType;
            IJob job = null;
            try
            {
                job = (IJob)_serviceProvider.GetRequiredService(jobType);
            }
            catch (Exception e)
            {
                throw new SchedulerException(string.Format(CultureInfo.InvariantCulture, "Problem instantiating class '{0}'", jobDetail.JobType.FullName), e);
            }

            var jobDataMap = new JobDataMap();
            jobDataMap.PutAll(scheduler.Context);
            jobDataMap.PutAll(bundle.JobDetail.JobDataMap);
            jobDataMap.PutAll(bundle.Trigger.JobDataMap);

            SetObjectProperties(job, jobDataMap);

            return job;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vault.Activity.Client;
using Vault.Activity.Api.Models;
using Vault.Activity.Api.Mvc;

namespace Vault.Activity.Api.Controllers
{
    [Route("api/[controller]")]
    public class StreamsController : Controller
    {
        readonly IActivityClient _client;

        public StreamsController(IActivityClient client)
        {
            _client = client;
        }
        
        [HttpGet("{bucket}/{streamid}")]
        public async Task<IEnumerable<CommitedActivityEvent>> Get(string bucket, Guid streamId, long checkpointToken = 0, int maxCount = 100)
        {
            var stream = await _client.GetStreamAsync(bucket, streamId);
            return await stream.ReadEventsAsync(checkpointToken, maxCount);
        }
        
        [HttpPost("{bucket}/{streamid}")]
        public async Task Post(string bucket, Guid streamId, [FromBody]ActivityEventInputModel model)
        {
            var stream = await _client.GetStreamAsync(bucket, streamId);

            var @event = new ActivityEventAttempt
            {
                Actor = model.Actor,
                Content = model.Content,
                Id = model.Id,
                MetaBag = model.MetaBag,
                Provider = model.Provider,
                Published = model.Published,
                Target = model.Target,
                Title = model.Title,
                Uri = model.Uri,
                Verb = model.Verb
            };
            await stream.PushActivityAsync(@event);
        }
    }
}

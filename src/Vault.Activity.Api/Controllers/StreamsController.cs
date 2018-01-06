using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vault.Activity.Client;
using Vault.Activity.Api.Models;
using Vault.Activity.Api.Mvc;
using Vault.Shared.Activity;
using System.ComponentModel.DataAnnotations;

namespace Vault.Activity.Api.Controllers
{
    [Route("api/[controller]")]
    public class StreamsController : Controller
    {
        readonly IActivityClient _client;

        public StreamsController(IActivityClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        [HttpGet("{bucket:maxlength(50)}/{streamid:maxlength(50)}")]
        public async Task<IEnumerable<CommitedActivityEvent>> Get(
            string bucket,
            string streamId,
            [FromQuery] string query = null,
            [FromQuery] long checkpointToken = 0,
            [FromQuery] int maxCount = 100)
        {
            var stream = await _client.GetStreamAsync(bucket, streamId);
            return await stream.ReadEventsAsync(query, checkpointToken, maxCount);
        }

        [HttpPost("{bucket:maxlength(50)}/{streamid:maxlength(50)}")]
        public async Task<IActionResult> Post(
            string bucket,
            string streamId,
            [FromBody] ActivityEventInputModel model)
        {
            if (model == null)
                return new BadRequestResult();

            var stream = await _client.GetStreamAsync(bucket, streamId);

            var @event = new ActivityEventAttempt
            {
                Actor = model.Actor,
                Content = model.Content,
                Id = model.Id,
                MetaBag = new DynamicJsonObject(model.MetaBag),
                Provider = model.Provider,
                Published = model.Published,
                Target = model.Target,
                Title = model.Title,
                Uri = model.Uri,
                Verb = model.Verb
            };
            await stream.PushActivityAsync(@event);

            return Ok();
        }
    }
}

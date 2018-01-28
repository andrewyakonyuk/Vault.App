using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using System;
using System.Linq;
using System.Threading;
using StreamInsights.Abstractions;

namespace StreamInsights.Persistance
{
    public class SqlAppendOnlyActivityStore : IAppendOnlyActivityStore
    {
        readonly ISqlConnectionFactory _connectionFactory;

        public SqlAppendOnlyActivityStore(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task AppendAsync(IReadOnlyCollection<UncommitedActivity> events, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var connection = _connectionFactory.Open())
            using (var transaction = connection.BeginTransaction())
            {
                await connection.ExecuteAsync(new CommandDefinition(
                    string.Intern(@"INSERT INTO activities
                            (bucket, ""streamId"", id, type, attachment, ""attributedTo"", audience, context, generator, 
                            ""inReplyTo"", location, tag, updated, url, annotations, name, ""nameMap"", content, ""contentMap"",
                            ""endTime"", image, preview, ""startTime"", summary, ""summaryMap"", ""mediaType"", actor, 
                            instrument, object, origin, result, target, published)
                            VALUES (@bucket, @streamId, @id, @type, @attachment, @attributedTo, @audience, 
                            @context, @generator, @inReplyTo, @location, @tag, @updated, @url, @annotations, @name, @nameMap, 
                            @content, @contentMap, @endTime, @image, @preview, @startTime, @summary, @summaryMap,
                            @mediaType, @actor, @instrument, @object, @origin, @result, @target, @published); ")
                    , events.ToArray()
                    , transaction, cancellationToken: cancellationToken));

                transaction.Commit();
            }
        }

        public async Task<IReadOnlyCollection<CommitedActivity>> ReadAsync(
            string streamId,
            string bucket,
            long checkpointToken,
            int maxCount,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (maxCount <= 0)
                return new CommitedActivity[0];

            using (var connection = _connectionFactory.Open())
            {
                var results = await connection.QueryAsync<CommitedActivity>(new CommandDefinition(
                    string.Intern(@"SELECT bucket, ""streamId"", id, type, attachment, ""attributedTo"", 
                        audience, context, generator, ""inReplyTo"", location, tag, updated, url, annotations,
                        name, ""nameMap"", content, ""contentMap"", ""endTime"", image, preview, 
                        ""startTime"", summary, ""summaryMap"", ""mediaType"",
                        actor, instrument, object, origin, result, target, published, ""checkpointToken""
                        FROM activities
                        WHERE ""checkpointToken"" > @checkpointToken 
                            AND ""streamId"" = @streamId 
                            AND bucket = @bucket 
                        ORDER BY ""checkpointToken"" ASC
                        LIMIT @maxCount"), new { streamId, bucket, checkpointToken, maxCount }, cancellationToken: cancellationToken));

                return results.ToList();
            }
        }

        public async Task<IReadOnlyCollection<CommitedActivity>> ReadAsync(long checkpointToken, int maxCount, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (maxCount <= 0)
                return new CommitedActivity[0];

            using (var connection = _connectionFactory.Open())
            {
                var results = await connection.QueryAsync<CommitedActivity>(new CommandDefinition(
                    string.Intern(@"SELECT bucket, ""streamId"", id, type, attachment, ""attributedTo"", 
                        audience, context, generator, ""inReplyTo"", location, tag, updated, url, annotations,
                        name, ""nameMap"", content, ""contentMap"", ""endTime"", image, preview, 
                        ""startTime"", summary, ""summaryMap"", ""mediaType"",
                        actor, instrument, object, origin, result, target, published, ""checkpointToken""
                        FROM activities
                        WHERE ""checkpointToken"" > @checkpointToken
                        ORDER BY ""checkpointToken"" ASC
                        LIMIT @maxCount"), new { checkpointToken, maxCount }, cancellationToken: cancellationToken));

                return results.ToList();
            }
        }
    }
}

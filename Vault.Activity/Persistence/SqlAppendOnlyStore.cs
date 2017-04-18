using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Vault.Shared;
using System;

namespace Vault.Activity.Persistence
{
    public class SqlAppendOnlyStore : IAppendOnlyStore
    {
        readonly ISqlConnectionFactory _connectionFactory;
        readonly JsonSerializer _serializer;

        public SqlAppendOnlyStore(
            ISqlConnectionFactory connectionFactory,
            JsonSerializer serializer)
        {
            _connectionFactory = connectionFactory;
            _serializer = serializer;
        }

        public async Task AppendAsync(IEnumerable<UncommitedActivityEvent> events)
        {
            using (var connection = _connectionFactory.Open())
            using (var transaction = connection.BeginTransaction())
            {
                var result = await connection.ExecuteAsync(@"INSERT INTO public.activitylogs
                    (bucketid, streamid, id, verb, actor, target, provider, url, published, metabag, title, content)
                    VALUES (@bucketid, @streamid, @id, @verb, @actor, @target, @provider, @url, @published, @metabag, @title, @content); ",
                    events.Select(t => new
                    {
                        id = t.Id,
                        bucketid = t.Bucket,
                        streamid = t.StreamId.ToString("N"),
                        verb = t.Verb,
                        actor = t.Actor,
                        target = t.Target,
                        published = t.Published,
                        provider = t.Provider,
                        url = t.Uri,
                        title = t.Title,
                        content = t.Content,
                        metabag = _serializer.Serialize((object)t.MetaBag)
                    }).ToArray(), transaction);

                transaction.Commit();
            }
        }

        public async Task<IReadOnlyCollection<CommitedActivityEvent>> ReadRecordsAsync(IList<long> checkpointTokens)
        {
            using (var connection = _connectionFactory.Open())
            {
                var records = await connection.QueryAsync(
                            @"SELECT bucketid, streamid, id, verb, target, provider,
                            url, published, metabag, checkpointnumber, actor,
                            title, content
                            FROM public.activitylogs
                            WHERE checkpointnumber IN @checkpointTokens
                            ORDER BY checkpointnumber", new { checkpointTokens });

                var result = new List<CommitedActivityEvent>();

                foreach (var item in records)
                {
                    result.Add(new CommitedActivityEvent
                    {
                        Actor = item.actor,
                        Bucket = item.bucketid,
                        CheckpointToken = item.checkpointnumber,
                        Id = item.id,
                        Provider = item.provider,
                        Published = item.published,
                        StreamId = Guid.Parse((string)item.streamid),
                        Target = item.target,
                        Uri = item.url,
                        Verb = item.verb,
                        Title = item.title,
                        Content = item.content,
                        MetaBag = ReferenceEquals(item.metabag, null)
                            ? new DynamicDictionary()
                            : _serializer.Deserialize<DynamicDictionary>((byte[])item.metabag)
                    });
                }

                return result.AsReadOnly();
            }
        }

        public async Task<IReadOnlyCollection<CommitedActivityEvent>> ReadRecordsAsync(long checkpointToken, int maxCount)
        {
            using (var connection = _connectionFactory.Open())
            {
                var records = await connection.QueryAsync(
                            @"SELECT bucketid, streamid, id, verb, target, provider,
                            url, published, metabag, checkpointnumber, actor,
                            title, content
                            FROM public.activitylogs
                            WHERE checkpointnumber > @checkpointToken
                            ORDER BY checkpointnumber ASC
                            LIMIT @maxCount", new { checkpointToken, maxCount });

                var result = new List<CommitedActivityEvent>(maxCount);

                foreach (var item in records)
                {
                    result.Add(new CommitedActivityEvent
                    {
                        Actor = item.actor,
                        Bucket = item.bucketid,
                        CheckpointToken = item.checkpointnumber,
                        Id = item.id,
                        Provider = item.provider,
                        Published = item.published,
                        StreamId = Guid.Parse((string)item.streamid),
                        Target = item.target,
                        Uri = item.url,
                        Verb = item.verb,
                        Title = item.title,
                        Content = item.content,
                        MetaBag = ReferenceEquals(item.metabag, null)
                            ? new DynamicDictionary()
                            : _serializer.Deserialize<DynamicDictionary>((byte[])item.metabag)
                    });
                }

                return result.AsReadOnly();
            }
        }

        public async Task<IReadOnlyCollection<CommitedActivityEvent>> ReadRecordsAsync(Guid streamId, string bucket, long checkpointToken, int maxCount)
        {
            using (var connection = _connectionFactory.Open())
            {
                var records = await connection.QueryAsync(
                            @"SELECT bucketid, streamid, id, verb, target, provider,
                            url, published, metabag, checkpointnumber, actor,
                            title, content
                            FROM public.activitylogs
                            WHERE checkpointnumber > @checkpointToken AND streamid = @streamId AND bucketid = @bucket
                            ORDER BY checkpointnumber asc
                            LIMIT @maxCount", new { streamId = streamId.ToString("N"), bucket, checkpointToken, maxCount });

                var result = new List<CommitedActivityEvent>(maxCount);

                foreach (var item in records)
                {
                    result.Add(new CommitedActivityEvent
                    {
                        Actor = item.actor,
                        Bucket = item.bucketid,
                        CheckpointToken = item.checkpointnumber,
                        Id = item.id,
                        Provider = item.provider,
                        Published = item.published,
                        StreamId = Guid.Parse((string)item.streamid),
                        Target = item.target,
                        Uri = item.url,
                        Verb = item.verb,
                        Title = item.title,
                        Content = item.content,
                        MetaBag = ReferenceEquals(item.metabag, null)
                            ? new DynamicDictionary()
                            : _serializer.Deserialize<DynamicDictionary>((byte[])item.metabag)
                    });
                }

                return result.AsReadOnly();
            }
        }
    }
}

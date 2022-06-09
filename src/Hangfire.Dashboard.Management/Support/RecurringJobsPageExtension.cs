using Hangfire.Annotations;
using Hangfire.Common;
using Hangfire.Storage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hangfire.Dashboard.Management.Pages
{
    public static class RecurringJobsPageExtension
    {
        public static List<RecurringJobDto> GetRecurringJobs(
            [NotNull] this JobStorageConnection connection,
            int startingFrom,
            int endingAt)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));

            var ids = connection.GetRangeFromSet("recurring-jobs", startingFrom, endingAt);
            return GetRecurringJobDtos(connection, ids);
        }

        public static List<RecurringJobDto> GetRecurringJobs([NotNull] this IStorageConnection connection)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));

            var ids = connection.GetAllItemsFromSet("recurring-jobs");
            return GetRecurringJobDtos(connection, ids);
        }

        public static RecurringJobDto GetRecurringJob([NotNull] this IStorageConnection connection, string jobId)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));

            var ids = new[] { jobId };
            return GetRecurringJobDtos(connection, ids)?.FirstOrDefault();
        }

        private static List<RecurringJobDto> GetRecurringJobDtos(IStorageConnection connection, IEnumerable<string> ids)
        {
            var result = new List<RecurringJobDto>();
            foreach (var id in ids)
            {
                var hash = connection.GetAllEntriesFromHash($"recurring-job:{id}");

                if (hash == null)
                {
                    result.Add(new RecurringJobDto { Id = id, Removed = true });
                    continue;
                }

                var dto = new RecurringJobDto
                {
                    Id = id,
                    Cron = hash["Cron"]
                };

                try
                {
                    if (hash.TryGetValue("Job", out var payload) && !String.IsNullOrWhiteSpace(payload))
                    {
                        var invocationData = InvocationData.DeserializePayload(payload);
                        dto.Job = invocationData.DeserializeJob();
                    }
                }
                catch (JobLoadException ex)
                {
                    dto.LoadException = ex;
                }

                if (hash.ContainsKey("NextExecution"))
                {
                    dto.NextExecution = JobHelper.DeserializeNullableDateTime(hash["NextExecution"]);
                }

                if (hash.ContainsKey("LastJobId") && !string.IsNullOrWhiteSpace(hash["LastJobId"]))
                {
                    dto.LastJobId = hash["LastJobId"];

                    var stateData = connection.GetStateData(dto.LastJobId);
                    if (stateData != null)
                    {
                        dto.LastJobState = stateData.Name;
                    }
                }

                if (hash.ContainsKey("Queue"))
                {
                    dto.Queue = hash["Queue"];
                }

                if (hash.ContainsKey("LastExecution"))
                {
                    dto.LastExecution = JobHelper.DeserializeNullableDateTime(hash["LastExecution"]);
                }

                if (hash.ContainsKey("TimeZoneId"))
                {
                    dto.TimeZoneId = hash["TimeZoneId"];
                }

                if (hash.ContainsKey("CreatedAt"))
                {
                    dto.CreatedAt = JobHelper.DeserializeNullableDateTime(hash["CreatedAt"]);
                }
                if (hash.ContainsKey("PauseState"))
                {
                    dto.PauseState = SerializationHelper.Deserialize<bool>(hash["PauseState"]);
                }
                if (hash.TryGetValue("Error", out var error))
                {
                    dto.Error = error;
                }

                result.Add(dto);
            }

            return result;
        }

        public static long GetRecurringJobCount([NotNull] this JobStorageConnection connection)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            return connection.GetSetCount("recurring-jobs");
        }

        public static void SetPauseState([NotNull] this IStorageConnection connection, [NotNull] string jobId, [NotNull] bool value)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            //var recurringJob = connection.GetRecurringJob(jobId);
            var hash = connection.GetAllEntriesFromHash($"recurring-job:{jobId}") ?? new Dictionary<string, string>();
            var nextExecution = hash.ContainsKey("NextExecution") ? hash["NextExecution"] : string.Empty;
            if (string.IsNullOrWhiteSpace(nextExecution))
                nextExecution = hash.ContainsKey("PauseNextExecution") ? hash["PauseNextExecution"] : string.Empty;
            if (value)
            {
                connection.SetRangeInHash("recurring-job:" + jobId, new[] {
                    new KeyValuePair<string, string>("PauseState", SerializationHelper.Serialize(value)),
                    new KeyValuePair<string, string>("NextExecution", string.Empty),
                    new KeyValuePair<string, string>("PauseNextExecution", nextExecution),
                });
            }
            else
            {
                connection.SetRangeInHash("recurring-job:" + jobId, new[] {
                    new KeyValuePair<string, string>("PauseState", SerializationHelper.Serialize(value)),
                    new KeyValuePair<string, string>("NextExecution", nextExecution),
                    new KeyValuePair<string, string>("PauseNextExecution", string.Empty),
                });
            }

            //Dictionary<string, string> dictionary = connection.GetAllEntriesFromHash("recurring-job:" + jobId);
            //if (dictionary == null || dictionary.Count == 0)
            //{
            //    return;
            //}

            //if (!dictionary.TryGetValue(nameof(Job), out var jobDetail))
            //{
            //    return;
            //}

            //var RecurringJob = InvocationData.DeserializePayload(jobDetail).DeserializeJob();

            ////var job = CodingUtil.FromJson<HttpJobItem>(RecurringJob.Args.FirstOrDefault()?.ToString());

            ////if (job == null) return;

            //using (var tran = connection.CreateWriteTransaction())
            //{
            //    //拿到所有的设置
            //    var conts = connection.GetAllItemsFromSet($"JobPauseOf:{jobId}");

            //    //有就先清掉
            //    foreach (var pair in conts)
            //    {
            //        tran.RemoveFromSet($"JobPauseOf:{jobId}", pair);
            //    }

            //    var cron = conts.FirstOrDefault(r => r.StartsWith("Cron:"));
            //    if (!string.IsNullOrEmpty(cron)) cron = cron.Replace("Cron:", "");
            //    //如果包含有true 的 说明已经设置了 暂停 要把改成 启动 并且拿到 Cron 进行更新
            //    if (conts.Contains("true"))
            //    {
            //        tran.AddToSet($"JobPauseOf:{jobId}", "false");
            //        if (!string.IsNullOrEmpty(cron))
            //        {
            //            //job.Cron = cron;
            //            //AddHttprecurringjob(job);
            //        }
            //    }
            //    else
            //    {
            //        tran.AddToSet($"JobPauseOf:{jobId}", "true");
            //        //tran.AddToSet($"JobPauseOf:{jobId}", "Cron:" + job.Cron);
            //        //job.Cron = "";
            //        //AddHttprecurringjob(job);
            //    }

            //    tran.Commit();
            //}
        }
    }

    public class RecurringJobDto : Hangfire.Storage.RecurringJobDto
    {
        public RecurringJobDto(Dictionary<string, string> hash = null)
        {
            Hash = hash;
        }

        public bool PauseState { get; set; }
        public Dictionary<string, string> Hash { get; }
    }
}
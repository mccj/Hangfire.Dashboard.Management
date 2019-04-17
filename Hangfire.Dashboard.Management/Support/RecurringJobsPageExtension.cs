using System;
using System.Collections.Generic;
using Hangfire.Annotations;
using Hangfire.Common;
using Hangfire.Dashboard;
using Hangfire.Storage;

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

                var dto = new RecurringJobDto(hash)
                {
                    Id = id,
                    Cron = hash["Cron"]
                };

                try
                {
                    var invocationData = SerializationHelper.Deserialize<InvocationData>(hash["Job"]);
                    dto.Job = invocationData.DeserializeJob();
                }
                catch (JobLoadException ex)
                {
                    dto.LoadException = ex;
                }

                if (hash.ContainsKey("NextExecution"))
                {
                    dto.NextExecution = JobHelper.DeserializeDateTime(hash["NextExecution"]);
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
                    dto.LastExecution = JobHelper.DeserializeDateTime(hash["LastExecution"]);
                }

                if (hash.ContainsKey("TimeZoneId"))
                {
                    dto.TimeZoneId = hash["TimeZoneId"];
                }

                if (hash.ContainsKey("CreatedAt"))
                {
                    dto.CreatedAt = JobHelper.DeserializeDateTime(hash["CreatedAt"]);
                }
                if (hash.ContainsKey("PauseState"))
                {
                    dto.PauseState = SerializationHelper.Deserialize<bool>(hash["PauseState"]);
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
            connection.SetRangeInHash("recurring-job:" + jobId, new[] { new KeyValuePair<string, string>("PauseState", SerializationHelper.Serialize(value)) });
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
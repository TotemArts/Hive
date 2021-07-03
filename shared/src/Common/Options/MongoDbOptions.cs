using System;

namespace Hive.Shared.Common.Options
{
    [Serializable]
    public class MongoDbOptions
    {
        public string? Password { get; set; }
        public string? Url { get; set; }
        public string? Username { get; set; }
    }
}
using System;
using System.Security.Cryptography;
using System.Text;

namespace Hive.Shared.Common.Context
{
    public class HiveContextProvider : IHiveContextProvider
    {
        public Guid HiveId { get; }

        public HiveContextProvider()
        {
            HiveId = new Guid(MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("HiveHardwareId") ?? "debug-hardware-id")));
        }
    }
}
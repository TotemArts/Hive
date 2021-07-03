using System;

namespace Hive.Shared.Common.Context
{
    public interface IHiveContextProvider
    {
        Guid HiveId { get; }
    }
}
﻿using System;

namespace sentry_dotnet_transaction_addon.Extensions
{
    internal static class GuidExtensions
    {
        internal static string LimitLength(this Guid guid, int length = 16)
        {
            var output = guid.ToString().Replace("-", "");
            return output.Length > length ? output.Remove(length) : output;
        }
    }
}

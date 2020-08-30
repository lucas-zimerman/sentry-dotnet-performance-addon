﻿using sentry_dotnet_health_addon.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace sentry_dotnet_health_addon.Extensibility
{
    class DisabledSession : ISession
    {
        public static DisabledSession Instance = new DisabledSession();
        public DateTime? Started { get; private set; }

        public DateTime? Timestamp { get; private set; }

        public int? ErrorCount { get; private set; }

        public string DistinctId { get; private set; }

        public Guid SessionId { get; private set; }

        public bool? Init { get; private set; }

        public SessionState Status { get; set; }

        public long? Sequence { get; private set; }

        public long? Duration { get; private set; }

        public SessionAttributes Attributes { get; private set; }

        public void End(DateTime? timestamp) {}

        public void RegisterError() {}

        public DisabledSession() {}

    }
}

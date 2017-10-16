using System;
using ListApp.Contracts.Interfaces;

namespace ListApp.Services.Helpers
{
    internal class TimeService : ITimeService
    {
        public DateTime GetCurrentTime()
            => DateTime.UtcNow;
    }
}
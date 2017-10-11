using System;
using ListApp.Contracts.Interfaces;

namespace ListApp.Services.Helpers
{
    internal class TimeHelper : ITimeHelper
    {
        public DateTime GetCurrentTime()
            => DateTime.UtcNow;
    }
}
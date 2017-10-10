using System;
using ListApp.Contracts.Interfaces;

namespace ListApp.Services.TimeHelper
{
    internal class TimeHelper : ITimeHelper
    {
        public DateTime GetCurrentTime() 
            => DateTime.UtcNow;
    }
}

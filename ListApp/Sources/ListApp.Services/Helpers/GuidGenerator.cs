using System;
using ListApp.Contracts.Interfaces;

namespace ListApp.Services.Helpers
{
    internal class GuidGenerator : IGuidGenerator
    {
        public Guid GenerateGuid()
            => Guid.NewGuid();
    }
}
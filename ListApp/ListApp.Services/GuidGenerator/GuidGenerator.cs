using System;
using ListApp.Contracts.Interfaces;

namespace ListApp.Services.GuidGenerator
{
    internal class GuidGenerator : IGuidGenerator
    {
        public Guid GenerateGuid()
        {
            return Guid.NewGuid();
        }
    }
}

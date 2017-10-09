using System;
using ListApp.Contracts.Interfaces;

namespace ListApp.Api.Services.GuidGenerator
{
    internal class GuidGenerator : IGuidGenerator
    {
        public Guid GenerateGuid()
        {
            return Guid.NewGuid();
        }
    }
}

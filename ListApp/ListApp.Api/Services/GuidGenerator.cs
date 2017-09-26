using System;
using ListApp.Contracts.Interfaces;

namespace ListApp.Api.Services
{
    public class GuidGenerator : IGuidGenerator
    {
        public Guid GenerateGuid()
        {
            return Guid.NewGuid();
        }
    }
}
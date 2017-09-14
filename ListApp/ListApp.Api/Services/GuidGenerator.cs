using System;
using ListApp.Api.Interfaces;

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
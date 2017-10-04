using System;

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

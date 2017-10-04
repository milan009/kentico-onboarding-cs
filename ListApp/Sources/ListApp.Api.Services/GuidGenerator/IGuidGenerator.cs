using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListApp.Api.Services.GuidGenerator
{
    public interface IGuidGenerator
    {
        Guid GenerateGuid();
    }
}

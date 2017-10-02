﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;

namespace ListApp.Repositories.Bootstrapper
{
    public interface IBootstrapper
    {
        IUnityContainer RegisterListItemRepository(IUnityContainer container);
    }
}

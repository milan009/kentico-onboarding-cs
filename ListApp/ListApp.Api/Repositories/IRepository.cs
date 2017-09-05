﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListApp.Api.Repositories
{
    public interface IRepository<TItemType, TKeyType>
    {
        IEnumerable<TKeyType> GetKeys();
        IEnumerable<TItemType> GetAll(Func<TItemType, bool> predicate = null);
        TItemType Get(TKeyType key);
        void Add(TKeyType key, TItemType entity);
        void Delete(TKeyType key);
        void Clear();
    }
}
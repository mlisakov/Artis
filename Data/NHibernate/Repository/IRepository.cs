﻿using System;

namespace Artis.Data
{
    internal interface IRepository<T>
    {
        void Add(T item);
        void Update(T item);
        void Remove(T item);
        T GetById(long itemGuid);
        T GetByName(string name,string field="Name");
    }
}
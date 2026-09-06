
using System;
using System.Collections.Generic;
using UnityCommander.Ribbon.Abstractions.Models;

namespace UnityCommander.Services.Interfaces.Ribbon
{
    public class RibbonService : IRibbonService
    {
        private readonly List<RibbonModel> _models = new();

        //public event Action<RibbonChangedEvent> Changed;

        public void Add(RibbonModel model)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<RibbonModel> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Remove(string id)
        {
            throw new NotImplementedException();
        }
    }
}

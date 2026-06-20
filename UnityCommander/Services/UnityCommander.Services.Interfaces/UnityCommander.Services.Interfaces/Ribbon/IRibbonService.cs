using System;
using System.Collections.Generic;
using UnityCommander.Ribbon.Abstractions.Models;
namespace UnityCommander.Services.Interfaces.Ribbon
{
    public interface IRibbonService
    {
        void Add(RibbonModel model);
        void Remove(string id);
        IReadOnlyList<RibbonModel> GetAll();

        //event Action<RibbonChangedEvent> Changed;
    }
}

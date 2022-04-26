
using System.Collections.Generic;
using UnityCommander.Common;

namespace UnityCommander.Core
{
    public class CommandParametersManager
    {
        public List<XParamViewModel> Params { get; set; }

        public CommandParametersManager()
        {
            Params = new List<XParamViewModel>();
        }

        public void AddParam(object dataContext, string propertyName)
        {
            Params.Add(new XParamViewModel(dataContext, propertyName));
        }
    }
}

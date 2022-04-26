using System;
using System.Collections.Generic;
using System.Text;
using UnityCommander.Common.Models;

namespace UnityCommander.Common
{
    public interface IGlobalCommandProvider
    {
        IGlobalCommandManager GetCommandManager();
    }
}

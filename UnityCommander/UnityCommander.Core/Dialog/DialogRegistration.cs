
using System;

namespace UnityCommander.Core.Dialog
{
    public sealed record DialogRegistration(
         string Id,
         Type ViewType,
         Type ViewModelType);
}

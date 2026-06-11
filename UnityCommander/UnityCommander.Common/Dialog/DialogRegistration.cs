
using System;

namespace UnityCommander.Common.Dialog
{
    public sealed record DialogRegistration(
         string Id,
         Type ViewType,
         Type ViewModelType);
}

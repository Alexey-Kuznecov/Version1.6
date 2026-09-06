
using System;

namespace UnityCommander.Core.Registrar
{
    public sealed record DialogRegistration(
         string Id,
         Type ViewType,
         Type ViewModelType);
}

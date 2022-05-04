
namespace UnityCommander.Common.Plugins
{
    using System.Collections.Generic;

    /// <summary>
    /// The PluginServiceRegister interface.
    /// </summary>
    public interface IPluginServicesRegister
    {
        /// <summary>
        /// Регистрирует службы подключаемых модулей для управления настройками,
        /// получения описания и внедрения реализации расширяющий функционал программы. 
        /// </summary>
        /// <param name="instance">
        /// Экземпляр класса подключаемого модуля который реализует контракт, например <see cref="IDialogService"/>.
        /// </param>
        /// <typeparam name="TI">
        /// Тип службы наследует общий для всех служб <see cref="IPluginService"/> контракт.
        /// </typeparam>
        /// <typeparam name="TS">
        /// Общий контракт служб, на данный момент нужен лишь для того чтобы регистрировать службы в одной категории.
        /// А так же избежать ошибок связанных приведением к типу.
        /// </typeparam>
        public void Register<TI, TS>(IEnumerable<TS> instance)
            where TI : TS;
    }
}

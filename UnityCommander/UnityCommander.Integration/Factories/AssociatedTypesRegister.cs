

namespace UnityCommander.Integration.Factories
{
    using UnityCommander.Integration.Options;

    /// <summary>
    /// Регистрация типов используемых плагином, каждый метод этого класса связан с определенным контрактом
    /// который реализуется плагином, эти типы будут передаваться в качестве аргументов метода реализуемых
    /// разработчиком подключаемого модуля.
    /// </summary>
    public class AssociatedTypesRegister
    {
        /// <summary>
        /// Тип для регистрации ассоциации типов.
        /// </summary>
        private readonly AssociatedTypes associatedTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssociatedTypesRegister"/> class.
        /// </summary>
        /// <param name="associatedTypes">
        /// The associated types.
        /// </param>
        public AssociatedTypesRegister(AssociatedTypes associatedTypes)
        {
            this.associatedTypes = associatedTypes;
        }

        /// <summary>
        /// Регистрирует тип для настроек плагина, тип который зарегистрированный будет передаваться в параметрах
        /// <see cref="IPluginSettings.OnSettingsChanged"/> метода.
        /// </summary>
        /// <param name="pluginService">
        /// Экземпляр класса подключаемого модуля который использует настройку для своих классов.
        /// </param>
        /// <typeparam name="TSet">
        /// Тип настроек для класса реализующий интерфейс <see cref="IPluginSettings"/>.
        /// </typeparam>
        public void RegisterSettings<TSet>(IPluginSettings pluginService) where TSet : new()
        {
            this.associatedTypes.Types.Add(pluginService, new TSet());
        }
    }
}

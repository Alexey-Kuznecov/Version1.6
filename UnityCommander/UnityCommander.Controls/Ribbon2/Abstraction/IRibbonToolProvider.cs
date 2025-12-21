
using System.Collections.Generic;

namespace UnityCommander.Controls.Ribbon2.Abstraction
{
    public interface IRibbonToolProvider
    {
        /// Возвращает набор моделей или добавляет их в target model.
        IEnumerable<RibbonToolModel> GetTools();
    }
}

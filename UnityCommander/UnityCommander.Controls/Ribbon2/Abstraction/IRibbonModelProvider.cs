using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Controls.Ribbon2.Abstraction
{
    public interface IRibbonModelProvider
    {
        /// Позволяет плагину добавить/модифицировать модель ленты при инициализации
        void Configure(RibbonModel model);
    }
}

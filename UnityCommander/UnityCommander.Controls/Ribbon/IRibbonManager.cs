
namespace UnityCommander.Controls.Ribbon
{
    using System.Windows.Input;

    public interface IRibbonManager
    {
        void Collapse(ICommand command);
        void Initial(Ribbon ribbon);
        void Configure(RibbonConfig config);
    }
}

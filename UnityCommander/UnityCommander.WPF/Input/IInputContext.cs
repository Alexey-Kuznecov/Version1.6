namespace UnityCommander.WPF.Input
{
    public interface IInputContext
    {
        bool Handle(InputEvent e);
    }
}

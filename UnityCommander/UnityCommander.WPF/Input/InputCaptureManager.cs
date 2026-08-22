namespace UnityCommander.WPF.Input
{
    public sealed class InputCaptureManager : IInputCaptureManager
    {
        private readonly Stack<IInputContext> _stack = new();

        public void Push(IInputContext context)
        {
            _stack.Push(context);
        }

        public void Pop()
        {
            if (_stack.Count > 0)
                _stack.Pop();
        }

        public bool TryHandle(InputEvent e)
        {
            if (_stack.TryPeek(out var ctx))
                return ctx.Handle(e);

            return false;
        }

        public bool HasCapture => _stack.Count > 0;
    }
}

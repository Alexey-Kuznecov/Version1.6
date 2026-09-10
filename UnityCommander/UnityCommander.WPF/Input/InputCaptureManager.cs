namespace UnityCommander.WPF.Input
{
    public sealed class InputCaptureManager : IInputCaptureManager
    {
        private readonly Stack<IInputContext> _stack = new();

        public void Push(IInputContext context)
        {
            _stack.Push(context);
        }

        public void Pop(IInputContext context)
        {
            if (_stack.Count == 0)
                return;

            if (ReferenceEquals(_stack.Peek(), context))
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

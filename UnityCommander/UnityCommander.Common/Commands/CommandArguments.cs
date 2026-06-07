
namespace UnityCommander.Common.Commands
{
    public class CommandArguments
    {
        private readonly object?[] _args;

        public CommandArguments(params object?[] args)
        {
            _args = args;
        }

        public T? Get<T>(int index)
        {
            if (index >= _args.Length)
                return default;

            return (T?)_args[index];
        }

        public object? this[int index] => _args[index];

        public int Count => _args.Length;
    }
}

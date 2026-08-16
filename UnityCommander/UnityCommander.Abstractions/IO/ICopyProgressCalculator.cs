
namespace UnityCommander.Abstractions.IO
{
    public interface ICopyProgressCalculator
    {
        ProgressModel Calculate(OperationState state);
    }
}

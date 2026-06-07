
namespace UnityCommander.Controls.Layout
{
    public enum SizeType
    {
        Auto,
        Fixed,
        Flex
    }

    public struct SizeSpec
    {
        public SizeType Type;
        public double Value;
        public double Flex;

        public SizeSpec(SizeType type, double value = 0, double flex = 1)
        {
            Type = type;
            Value = value;
            Flex = flex;
        }
    }
}

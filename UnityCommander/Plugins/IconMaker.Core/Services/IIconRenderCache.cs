
namespace IconMaker.Core.Services
{
    public interface IIconRenderCache
    {
        void Invalidate();

        void Invalidate(string iconName);

        //RenderedIcon Get(...);
    }
}

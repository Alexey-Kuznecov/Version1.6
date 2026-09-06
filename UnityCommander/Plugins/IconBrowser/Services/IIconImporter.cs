
using IconMaker.Core.Models;

namespace IconBrowser.Services
{
    public interface IIconImporter
    {
        IconDefinition Import(string path, string name);
    }
}

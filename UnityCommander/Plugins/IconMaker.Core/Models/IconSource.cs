using System.IO;

namespace IconMaker.Core.Models
{
    public class IconSource
    {
        internal static readonly string DocumentName = Directory.GetCurrentDirectory() + @"\plugins\IconBrowser\Data\IconsData.xml";
        
        internal static readonly string Root = Directory.GetCurrentDirectory() + @"\plugins\IconBrowser\Data\";
    }
}

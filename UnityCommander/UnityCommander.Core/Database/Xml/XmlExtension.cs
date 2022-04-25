
namespace UnityCommander.Core.Database.Xml
{
    using System.Collections.Generic;
    using System.Xml.Linq;

    /// <summary>
    /// The class has methods a lot extending the vanilla feature of the language linq.
    /// </summary>
    public static class XmlExtension
    {
        /// <summary>
        /// The extension method extracts all elements defined nesting level. 
        /// </summary>
        /// <param name="element"> The root element from which the level count begins. </param>
        /// <param name="level"> The level where elements is position. </param>
        /// <returns> All elements one's level. </returns>
        public static IEnumerable<XElement> GetElements(this XElement element, byte level)
        {
            byte count = 0;
            var container = element;
            var result = container.Elements();

            while (count != level)
            {
                var newContainer = new List<XContainer>();

                foreach (var item in result)
                {
                    newContainer.Add(item);
                }

                container.RemoveNodes();

                foreach (var item in newContainer.Elements())
                {
                    container.Add(item);
                }

                count++;
            }

            return result;
        }

        /// <summary>
        /// Calculates the number of children within a parent.
        /// </summary>
        /// <param name="element"> The parent element. </param>
        /// <returns> The total number of items in the current node. </returns>
        public static int Total(this XElement element)
        {
            int count = 0;

            foreach (var item in element.Elements())
            {
                count++;
            }

            return count;
        }
    }
}

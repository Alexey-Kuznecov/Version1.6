
namespace IconBrowser.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using System.Xml.Linq;

    using AlexLibWpf.Models;
    using AlexLibWpf.Help;

    class IconsDataReader : IDisposable
    {
        private const string DocumentName = @"d:\Resources\IconsData.xml";
        private static IEnumerable<XElement> _elementIcons; // Gets all element named Icon
        private static IEnumerable<XElement> _elementCollection; // Gets all collection element

        private static void InitialFields()
        {
            XElement root = XElement.Load(DocumentName);
            _elementCollection    = from element in root.Elements() select element;
            _elementIcons         = from icon in _elementCollection.Elements() select icon;
        }
        /// <summary>
        /// Loads xml document and get icon categories from xml file.
        /// </summary>
        /// <returns>Returns names of categories in collection.</returns>
        public List<string> GetCollection()
        {
            // It's a field not initialized. Do it.
            if (_elementIcons == null)
                InitialFields();

            List<string> collection = new List<string>();
            foreach (var cat in _elementCollection)
                collection.Add(cat.FirstAttribute.Value);
            return collection;
        }
        /// <summary>
        /// Searches a icon in document that been specified value argument passing.
        /// </summary>
        /// <param name="name">Icon name that need be find.</param>
        /// <returns>If the icon is found then the method returns path otherwise throw exception.</returns>
        public static Path GetIconPath(string name)
        {
            Path paths = new Path();
            string str = " ";

            // It's a field not initialized. Do it.
            if (_elementIcons == null)
                InitialFields();

            var queryPath = from icon in _elementIcons
                where icon.Attribute("Name")?.Value == name
                select icon;
            // Concatenate paths to string.
            foreach (var path in queryPath)
                str += path.Value;
            // Converted string to geomentry.
            paths.Data = Geometry.Parse(str);
            return paths;
        }
        /// <summary>
        /// Extract icon attribute values from an xml file.
        /// Icon attribute repacking from xml markup to icon type.
        /// </summary>
        /// <param name="collectionName"></param>
        /// <returns>The collection IconModel type.</returns>
        public List<IconModel> GetIcons(string collectionName)
        {
            XElement root = XElement.Load(DocumentName);
            var queryCollection = from collect in root.Elements()
                                  where collect.FirstAttribute.Value == collectionName
                                  select collect;

            var iconList = new List<IconModel>();

            foreach (var element in queryCollection.Elements())
            {
                #region Create object by model Icon using data xml file.

                iconList.Add(new IconModel
                {
                    // ReSharper disable once PossibleNullReferenceException
                    Id = ushort.Parse(element.Attribute("Id").Value),
                    Name = element.Attribute("Name")?.Value,
                    FgroundColor = element.Attribute("Foreground")?.Value.StringFormatToSolidColor(),
                    BgroundColor = element.Attribute("Background")?.Value.StringFormatToSolidColor(),
                    // ReSharper disable once AssignNullToNotNullAttribute
                    Scale = int.Parse(element.Attribute("Scale")?.Value),
                    CollectionName = element.Parent?.FirstAttribute.Value,
                    Path = CreatePathGeometry(element.Elements("Path").ToList()),
                    Brush = CreateBrush(element.Elements("Path").ToList())
                });
                #endregion
            }

            return iconList;
        }

        /// <summary>
        /// Checks if the collection contains icons.
        /// </summary>
        public static bool ContainsIcons(string collectionName)
        {
            XElement root = XElement.Load(DocumentName);
            var query = from collect in root.Elements()
                        where collect.FirstAttribute.Value == collectionName
                        select collect;
            int count = query.Count(p => p.HasElements);
            if (count == 0)
                return false;
            return true;
        }

        #region Methods Gets Icon Data To Packing

        /// <summary>
        /// Creates the brush on base of xml data.
        /// </summary>
        /// <param name="pathElements">Node named Path.</param>
        /// <returns></returns>
        private static DrawingBrush CreateBrush(List<XElement> pathElements)
        {
            DrawingBrush dBrush = new DrawingBrush();
            DrawingGroup group = new DrawingGroup();
            
            foreach (var path in pathElements)
            {
                GeometryDrawing geometryDrawing = new GeometryDrawing();
                geometryDrawing.Geometry = Geometry.Parse(path.Value);
                geometryDrawing.Brush = path.Attribute("Fill")?.Value.StringFormatToSolidColor();
                group.Children.Add(geometryDrawing);
            }
            dBrush.Drawing = group;
            dBrush.Stretch = Stretch.Uniform;
            return dBrush;
        }

        /// <summary>
        /// Extracts all elements named Path, if the paths are larger than one, 
        /// path are merged then the value is converted into Data.
        /// </summary>
        /// <param name="pathElements">Node named Paths.</param>
        /// <returns>Returns path as icon.</returns>
        private static Path CreatePathGeometry(List<XElement> pathElements)
        {
            Path path = new Path();
            string pathCancat = " ";

            foreach (var xpath in pathElements)
            {
                pathCancat = pathCancat + xpath.Value;
                path.Fill = xpath.FirstAttribute.Value.StringFormatToSolidColor();
            }
            path.Data = Geometry.Parse(pathCancat);
             
            return path;
        }

        #endregion
        
        /// <summary>
        /// Clear fields after build object.
        /// </summary>
        public void Dispose()
        {
            _elementIcons = null;
            _elementCollection = null;
        }
    }
}

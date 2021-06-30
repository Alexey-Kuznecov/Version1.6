
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Linq;
using AIconBrowser.Help;

namespace AIconBrowser.Models
{
    /// <summary>
    /// The icons data writer.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1400:AccessModifierMustBeDeclared", Justification = "Reviewed. Suppression is OK here.")]
    class IconsDataWriter : IDisposable
    {
        /// <summary>
        /// The document name.
        /// </summary>
        private const string DocumentName = @"d:\Resources\IconsData.xml";

        /// <summary>
        /// The id.
        /// </summary>
        private static int id;

        /// <summary>
        /// The root.
        /// </summary>
        private static XElement root;

        /// <summary>
        /// The id list.
        /// </summary>
        private static ArrayList idList = new ArrayList();

        #region Function responsable for Icon.

        /// <summary>
        /// Create xml element the Icon on base IconModel object. 
        /// </summary>
        /// <param name="iconModel">Icon object.</param>
        [DebuggerStepperBoundary]
        public static void Save(IconModel iconModel)
        {
            XElement root = XElement.Load(DocumentName);
            var queryCategory = from name in root.Elements().Attributes("Name")
                                where name.Value == iconModel.CollectionName
                                select name.Parent;

            foreach (var element in queryCategory)
            {
                element.Add(
                        new XElement("Icon",
                        new XAttribute("Id", GetLastId().ToString()),
                        new XAttribute("Name", iconModel.Name),
                        new XAttribute("Scale", iconModel.Scale),
                        new XAttribute("Background", iconModel.BgroundColor),
                        new XAttribute("Foreground", iconModel.FgroundColor)));
                SetMultiPath(element, iconModel);
            }
            root.Save(DocumentName);
        }

        /// <summary>
        /// Create xml path element on base collection of paths.
        /// </summary>
        /// <param name="element"> Current xml collection.</param>
        /// <param name="iconModel">Wait pathList property of object IconModel.</param>
        public static void SetMultiPath(XElement element, IconModel iconModel)
        {
            var query = from icon in element.Elements()
                        where icon.FirstAttribute.Value == id.ToString()
                        select icon;

            foreach (var icon in query)
            { 
                foreach (var path in iconModel.PathList)
                {
                    icon.Add(new XElement("Path",
                            new XAttribute("Fill", path.Fill),
                            path.Data.ToString().Substring(2).Replace(',', '.').Replace(';', ',')));
                }
            }
        }

        /// <summary>
        /// Replace icon name on new name.
        /// </summary>
        /// <param name="oldName">Old name icons for finding it in a collection.</param>
        /// <param name="newName">New name.</param>
        public static void SetName(string oldName, string newName)
        {
            XElement root = XElement.Load(DocumentName);
            var queryName = from icon in root.Elements().Elements()
                            where icon.Attribute("Name")?.Value == oldName
                            select icon;

            foreach (var icon in queryName)
                if (icon != null)
                    // ReSharper disable once PossibleNullReferenceException
                    icon.Attribute("Name").Value = newName;
            root.Save(DocumentName);
        }
        
        /// <summary>
        /// Перемешает иконку из исходной коллекции в другую.
        /// </summary>
        /// <param name="id">Индентификатор иконки.</param>
        /// <param name="source">Исходная коллекция иконки.</param>
        /// <param name="target">Целевая коллекция для иконки.</param>
        public static void IconReplace(ushort id, string source, string target)
        {
            XElement root = XElement.Load(DocumentName);
            var queryCollect = from col in root.Elements() where col.FirstAttribute.Value == source select col;
            var queryTCollect = from col in root.Elements() where col.FirstAttribute.Value == target select col;

            foreach (var s in queryCollect.Elements())
                if (s.FirstAttribute.Value == id.ToString())
                    // ReSharper disable once PossibleMultipleEnumeration
                    foreach (var t in queryTCollect)
                    {
                        t.Add(s);
                        s.Remove();
                        break;
                    }
            root.Save(DocumentName);
        }
        #endregion

        #region Method for modification the collecton 

        private static void LoadDocument()
        {
            root = XElement.Load(DocumentName);
        }

        /// <summary>
        /// Adds new icons collection in the xml file.
        /// </summary>
        /// <param name="name">Name new collection.</param>
        public void AddNewCollection(string name)
        {
            LoadDocument();          
           root.Add(new XElement("Collection", new XAttribute("Name", name)));
           root.Save(DocumentName);
        }
        
        /// <summary>
        /// Delete a collection without touching the icons.
        /// </summary>
        /// <param name="name">Collection Name.</param>
        public void RemoveCollection(string name)
        {
            if (IconsDataReader.ContainsIcons(name))
                ReplaceAllIcon(name);
            LoadDocument();

            var query = from collect in root.Elements()
                        where collect.FirstAttribute.Value == name
                        select collect;
                 
            query.Remove();
            root.Save(DocumentName);
        }

        /// <summary>
        /// Move all the icons from the specified collection to the unsigned collection.
        /// </summary>
        /// <param name="source">Source collection.</param>
        public static void ReplaceAllIcon(string source)
        {
            LoadDocument();
            bool isExist = IsExistCollection(NamesEnum.Unsigned.GetName());
            if (!isExist) new IconsDataWriter().AddNewCollection(NamesEnum.Unsigned.GetName());

            // Find target collection.
            var queryTarget = from collect in root.Elements()
                              where collect.FirstAttribute.Value == NamesEnum.Unsigned.GetName()
                              select collect;
            // Find source collection.
            var querySource = from collect in root.Elements()
                              where collect.FirstAttribute.Value == source
                              select collect;

            foreach (var elementS in querySource.Elements())
                foreach (var elementTa in queryTarget)
                    elementTa.Add(elementS);
            root.Save(DocumentName);
        }

        /// <summary>
        /// Checks if there is a collection with the given name.
        /// </summary>
        /// <param name="collectionName">Collection name.</param>
        private static bool IsExistCollection(string collectionName)
        {
            XElement root = XElement.Load(DocumentName);
            var query = from collect in root.Elements()
                where collect.FirstAttribute.Value == collectionName
                select collect;
           
            if (!query.Any())
                return false;
            return true;
        }

        /// <summary>
        /// Changes old name on new.
        /// </summary>
        /// <param name="oldName">Old collection name.</param>
        /// <param name="newName">New collection name.</param>
        public void RenameCollection(string oldName, string newName)
        {
            XElement root = XElement.Load(DocumentName);
            var queryCollection = from collect in root.Elements()
                                  where collect.FirstAttribute.Value == oldName
                                  select collect;
            foreach (var col in queryCollection)
                col.FirstAttribute.Value = newName;
            root.Save(DocumentName);
        }
        #endregion

        #region Additional Functions
        
        /// <summary>
        /// Find element with the most id value and increment on one.
        /// </summary>
        /// <returns></returns>
        public static int GetLastId()
        {
            XElement root = XElement.Load(DocumentName);
            IEnumerable<XElement> queryCollectElements = from collect in root.Elements() select collect,
                queryIconElements = from icon in queryCollectElements.Elements() select icon;

            foreach (var list in queryIconElements.Attributes())
                if (list.Name == "Id")
                    idList.Add(value: int.Parse(list.Value));
            id = idList.MaxValue() + 1;
            return id;
        }
   
        public void Dispose()
        {
            idList = new ArrayList();
            root = null;
        }
        
        #endregion
    }
}

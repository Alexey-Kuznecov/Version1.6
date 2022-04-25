using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using AIconBrowser.Contracts;

namespace AIconBrowser
{
    [DebuggerStepThrough]
    public class XamlFileService : IFileService
    {
        /// <summary>
        /// Загружает корневой элемент из файла словаря ресурсов.
        /// </summary>
        /// <param name="filepath">Путь к файлу ресурсов.</param>
        /// <returns>Корневой элемент.</returns>
        public object Open(string filepath)
        {
            DependencyObject rootXaml;
            try
            {
                using (FileStream fs = new FileStream(filepath, FileMode.Open))
                    rootXaml = (DependencyObject)XamlReader.Load(fs);
                return rootXaml;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                throw;
            }
        }
        /// <summary>
        /// Сохраняет(сериализует) словарь ресурсов. Словарь ресурсов обязательно должен 
        /// состоять из корневого элемента ResourceDictionary.
        /// </summary>
        /// <param name="resPath">Путь к словаря ресурсов.</param>
        /// <param name="resDict">Словарь корнем которого ожидается элемент ResourceDictionary</param>
        public void Save(string resPath, object resDict)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(resPath))
                {
                    XamlWriter.Save(resDict, writer);
                } 
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                throw;
            }   
        }
    }
}

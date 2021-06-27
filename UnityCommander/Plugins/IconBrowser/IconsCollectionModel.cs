
namespace IconBrowser
{
    using System;
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using AlexLibWpf.Mvvm.Base;
    using AlexLibWpf.Components.InputBox;

    using IconBrowser.Models;

    /// <summary>
    /// Class of model that responsible to way display icons collection.
    /// </summary>
    public class IconCollectionBase : PropertiesChanged
    {
        public static event Action<ushort, string> OnCollectionChanged;
        public static DragEventHandler DragDrop = MoveIcon;

        /// <summary>
        /// Contains contextmenu of the icons collection.
        /// </summary>
        public ContextMenu CollectionContextMenu { get; set; }
        public ContextMenu ContextMenu { get; set; }

        /// <summary>
        /// Contains name of the icons collection.
        /// </summary>
        public string CollectionName { get; set; }

        /// <summary>
        /// Command to add new icons.
        /// </summary>
        public static ICommand AddNewCollection => new RelayCommand(name =>
        {
            using (IconsDataWriter dataWriter = new IconsDataWriter())
            {
                dataWriter.AddNewCollection((string) name);
                OnCollectionChanged?.Invoke(0, null);
                InputBox.Close();
            }
        });

        /// <summary>
        /// Command rename the icon collection.
        /// </summary>
        private ICommand RenameCollection => new RelayCommand(oldName =>
        {
            using (IconsDataWriter dataWriter = new IconsDataWriter())
            {
                dataWriter.RenameCollection(this.CollectionName, (string) oldName);
                OnCollectionChanged?.Invoke(0, null);
                InputBox.Close();
            }
        });

        /// <summary>
        /// Command moves the icon to another collection.
        /// </summary>
        /// <param name="sender">Expected the TextBlock object.</param>
        /// <param name="e">Expected the ContentControl object.</param>
        private static void MoveIcon(object sender, DragEventArgs e)
        {
            TextBlock collcetion = sender as TextBlock;
            ContentControl button = (ContentControl)e.Data.GetData(typeof(ContentControl));

            string targetName = collcetion?.Text;
            string sourceName = button?.Uid;
            if (button?.Tag != null)
            {
                ushort id = ushort.Parse(button.Tag.ToString());
                IconsDataWriter.IconReplace(id, sourceName, targetName);

                OnCollectionChanged?.Invoke(id, sourceName);

                if (sourceName == NamesEnum.Unsigned.GetName())
                    OnCollectionChanged?.Invoke(0, null);
            }
        }

        /// <summary>
        /// Remove collection. If the collection already contains icons 
        /// then moves icons to the Unsigned collection.
        /// </summary>
        private void RemoveCollection(object obj)
        {
            using (IconsDataWriter dataWriter = new IconsDataWriter())
            {
                dataWriter.RemoveCollection(this.CollectionName);
                OnCollectionChanged?.Invoke(0, null);
            }
        }

        /// <summary>
        /// Create context menu for collection names.
        /// </summary>
        protected IconCollectionBase()
        {
            this.CollectionContextMenu = new ContextMenu();
            this.ContextMenu = new ContextMenu();
            this.ContextMenu.Items.Add(new MenuItem { Header = "Добавить категорию", Command = new RelayCommand(obj => InputBox.Show(AddNewCollection, Actions.Add))});
            this.CollectionContextMenu.Items.Add(new MenuItem { Header = "Переименовать", Command = new RelayCommand(obj => InputBox.Show(this.RenameCollection, Actions.Change, this.CollectionName))});
            this.CollectionContextMenu.Items.Add(new MenuItem { Header = "Удалить", Command = new RelayCommand(RemoveCollection) });
        }
    }

    public class IconsCollectionModel : IconCollectionBase
    {
        /// <summary>
        /// Adds headers of icons collection.
        /// </summary>
        /// <returns>Retruns collection objects which contain 
        /// icon collection names and it context menu.</returns>
        public static ObservableCollection<IconsCollectionModel> GetCollection()
        {
            var cat = new ObservableCollection<IconsCollectionModel>();
            using (IconsDataReader dataReader = new IconsDataReader())
            {
                foreach (var name in dataReader.GetCollection())
                {
                    if (name == NamesEnum.Unsigned.GetName())
                    {
                        if (!IconsDataReader.ContainsIcons(NamesEnum.Unsigned.GetName()))
                            continue;
                        cat.Add(new IconsCollectionModel { CollectionName = name, CollectionContextMenu = new ContextMenu() });
                    }
                    else
                    {
                        cat.Add(new IconsCollectionModel { CollectionName = name });
                    }
                }
            }
            return cat;
        }
    }
}

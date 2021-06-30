using System.Collections.Generic;

namespace AIconBrowser.Components.InputBox
{
    class ActionsDictionary
    {
        private readonly Dictionary<Actions, string> _tableCode = new Dictionary<Actions, string>(5)
        {
            { Actions.Add,  "Добавить" },
            { Actions.Change,  "Изменить" },
            { Actions.Cancal,  "Отменить" },
            { Actions.Delete,  "Удалить" }
        };
        public Actions GetKey(string key)
        {
            foreach (var item in _tableCode)
                if (key == item.Value)
                    return item.Key;
            return Actions.Null;
        }
        public string GetValue(Actions value)
        {
            foreach (var item in _tableCode)
                if (value == item.Key)
                    return item.Value;
            return null;
        }
    }
}

# 🧠 Архитектура UnityCommander

Проект **UnityCommander** использует модульную архитектуру с гибким интерфейсом и поддержкой плагинов. Архитектура разделена на несколько ключевых частей:

- **Основная бизнес-логика**
- **UI** (через WPF и MVVM)
- **Система плагинов** с контейнерами и изоляцией
- **Искусственный интеллект** для автоматизации управления файлами

---

## 🛠️ Модуль плагинов

UnityCommander поддерживает систему плагинов для расширяемости и кастомизации функционала. Плагины загружаются динамически и могут быть изолированы для предотвращения конфликтов.

### Основные компоненты:
- **PluginContainer** — класс-обертка для управления плагинами.
- **ExtendedPluginContainer** — расширенная версия контейнера с поддержкой `PluginLoader`, что позволяет загружать и выгружать плагины по мере необходимости.
- **PluginLoader** — служит для загрузки плагинов в память и выполнения их операций.
- **InvalidPluginException** — собственное исключение, выбрасываемое в случае ошибок загрузки плагинов (например, если файл плагина невалидный).

### Пример работы с плагинами

Когда UnityCommander запускается, он ищет плагины в заранее определённой папке, загружая все валидные файлы `.dll`. Каждый плагин выполняет свои функции, и их можно изолировать, чтобы избежать конфликтов между ними.

Пример кода для загрузки плагинов:

```csharp
public void LoadPlugins(string pluginsDir)
{
    foreach (var dir in Directory.GetDirectories(pluginsDir))
    {
        string dirName = Path.GetFileName(dir);
        string pluginDll = Path.Combine(dir, "net9.0-windows", $"{dirName}.dll");

        if (File.Exists(pluginDll))
        {
            var pluginLoader = new PluginLoader();
            pluginLoader.ExecuteAndUnload(pluginDll);
            PluginLoaders.Add(pluginLoader);
        }
        else
        {
            throw new InvalidPluginException($"Плагин не найден: {pluginDll}");
        }
    }
}

🔒 Изоляция плагинов
Для безопасности плагинов, каждый плагин загружается и выполняется в своём изолированном контейнере. Это позволяет предотвратить нежелательные взаимодействия между плагинами, а также упрощает их выгрузку.

Пример использования контейнера плагинов:

public class PluginContainer
{
    private List<IPluginLoader> _pluginLoaders = new List<IPluginLoader>();

    public void AddPlugin(IPluginLoader pluginLoader)
    {
        _pluginLoaders.Add(pluginLoader);
    }

    public void RemovePlugin(IPluginLoader pluginLoader)
    {
        _pluginLoaders.Remove(pluginLoader);
    }
}
🤖 Искусственный интеллект
В будущем UnityCommander будет использовать ИИ для автоматизации некоторых задач, таких как:

Категоризация файлов: анализ файлов и автоматическое распределение по категориям (например, документы, изображения, код).

Предсказание нужных файлов: на основе активности пользователя ИИ будет предсказывать, какие файлы могут понадобиться в ближайшее время.

Очистка мусора: обнаружение и удаление дублирующихся или неиспользуемых файлов.

Пример возможной работы с ИИ:


public class FileCategorizer
{
    public FileCategory CategorizeFile(string filePath)
    {
        // Логика категоризации
        if (filePath.EndsWith(".jpg") || filePath.EndsWith(".png"))
        {
            return FileCategory.Image;
        }

        return FileCategory.Other;
    }
}
🔄 Перезагрузка и выгрузка плагинов
Плагины в UnityCommander можно не только загружать, но и выгружать, а также перезагружать при необходимости. Это позволяет гибко управлять их состоянием.

Пример кода для перезагрузки плагина:


public class PluginLoader
{
    public void ExecuteAndUnload(string pluginDll)
    {
        // Логика загрузки плагина
        LoadPlugin(pluginDll);
    }

    private void LoadPlugin(string pluginDll)
    {
        // Загрузка плагина из DLL
    }

    public void UnloadPlugin()
    {
        // Логика выгрузки плагина
    }
}
🚀 Будущие планы
Интеграция ИИ для предсказания и очистки файлов

Обработка ошибок плагинов (добавление более сложных проверок и восстановления)

Поддержка кроссплатформенности (через Avalonia или другой фреймворк)

Теперь у нас есть **ARCHITECTURE.md**, который подробно объясняет архитектуру проекта, работу с плаги
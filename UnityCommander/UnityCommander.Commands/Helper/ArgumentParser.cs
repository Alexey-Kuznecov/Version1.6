using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Commands.Helper
{
    public class ArgumentParser
    {
        private readonly Dictionary<string, string> _values = new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> _flags = new(StringComparer.OrdinalIgnoreCase);

        // Алиасы: {"--img", "--image", "-i"} -> "image"
        private readonly Dictionary<string, string> _aliasMap;

        public ArgumentParser(string[] args, Dictionary<string, string[]> aliases)
        {
            // Создаем маппинг всех алиасов
            _aliasMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var kv in aliases)
            {
                foreach (var alias in kv.Value)
                    _aliasMap[alias] = kv.Key;
            }

            Parse(args);
        }

        private void Parse(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                string raw = args[i];

                // --- ФЛАГИ ---
                if (raw.StartsWith("--") && !raw.Contains("="))
                {
                    // Может быть флаг или аргумент с отдельным значением

                    // Если следующий элемент не существует или следующий тоже ключ
                    if (i + 1 >= args.Length || args[i + 1].StartsWith("-"))
                    {
                        _flags.Add(NormalizeName(raw));
                    }
                    else
                    {
                        // Аргумент --key value
                        string key = NormalizeName(raw);
                        string val = args[++i];
                        _values[key] = val;
                    }

                    continue;
                }

                // --- Формат --key=value ---
                if (raw.Contains("=") && raw.StartsWith("--"))
                {
                    var parts = raw.Split("=", 2);
                    string key = NormalizeName(parts[0]);
                    string val = parts[1];
                    _values[key] = val;
                    continue;
                }

                // --- Однобуквенные флаги (-v) ---
                if (raw.StartsWith("-") && raw.Length == 2)
                {
                    string key = NormalizeName(raw);
                    _flags.Add(key);
                }
            }
        }

        private string NormalizeName(string raw)
        {
            return _aliasMap.TryGetValue(raw, out var canonical)
                ? canonical
                : raw.TrimStart('-');
        }

        public string? GetValue(string name)
        {
            name = name.TrimStart('-');
            return _values.TryGetValue(name, out var v) ? v : null;
        }

        public bool HasFlag(string name)
        {
            name = name.TrimStart('-');
            return _flags.Contains(name);
        }
    }
}

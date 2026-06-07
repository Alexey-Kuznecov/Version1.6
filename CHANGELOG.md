# 📜 CHANGELOG

Все заметные изменения этого проекта будут задокументированы в этом файле.

Формат основан на [Keep a Changelog](https://keepachangelog.com/ru/1.0.0/), и этот проект придерживается [Semantic Versioning](https://semver.org/lang/ru/).

---

## [0.1.0] - 2025-04-05
### Добавлено
- `PluginContainer` — класс-обертка для плагинов
- `ExtendedPluginContainer` с поддержкой `PluginLoader`
- Исключение `InvalidPluginException` для обработки некорректных плагинов
- Тесты:
  - `Plugin_Handles_Invalid_Assembly` (пройден)
  - `Plugin_Loads_Correctly`, `Plugin_Throws_On_Invalid_Path`
- Структурированный `README.md`
- Начато документирование кода и ведение журналов

### Исправлено
- Исправлена загрузка плагинов при наличии некорректного `dll`

---
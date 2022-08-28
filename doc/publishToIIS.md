### Публикация приложения в IIS

Заполнить `src\UI\appsettings.json`

Создать на основе `build\iis\config.example.pds1` файл `build\iis\config.psd1` и заполнить параметры
```powershell
.\build\iis\publish.ps1
```

Скрипт остановит текущее приложение, сделает его бекап в указанную в `config.psd1` папку, обновит приложение и запустит его
<p align="center">
  <img src="https://www.oa.edu.ua/bootstrap5/im/logo.svg" width="120" alt="Ostroh Academy Logo"/>
</p>
<h1 align="center">C# Labs Repository — Якість повітря (Air Quality)</h1>
<p align="center">
  Лабораторні роботи з курсу програмування мовою C# (.NET)
</p>
<p align="center">
  <img src="https://img.shields.io/badge/C%23-.NET-blue" />
  <img src="https://img.shields.io/badge/platform-.NET%2010.0-purple" />
  <img src="https://img.shields.io/badge/status-completed-brightgreen" />
</p>

---

## 📚 Про репозиторій

Даний проєкт реалізує **Лабораторну роботу №1: Робота з файловою системою та форматами даних** відповідно до принципів **Clean Architecture** (Чиста архітектура).

### 🏛️ Архітектура рішення
Проєкт розділено на 4 рівні:
1. **`lab1.Domain`** — сутності (`AirQualityRecord`, `MonthlyMeasurements`, `SyncItem`) та переліки (`DataFormat`, `AirQualitySortBy`, `SortDirection`, `FileChangeType`, `SyncDirection`). Не має зовнішніх залежностей.
2. **`lab1.Application`** — інтерфейси репозиторіїв та провайдерів, сервіси бізнес-логіки (`CsvAirQualityService`, `JsonAirQualityService`, `XmlAirQualityService`, `XlsxAirQualityService`, `DataAnalyticsService`, `ChartDataService`, `ReportService`, `FolderSyncService`), DTO.
3. **`lab1.Infrastructure`** — конкретні реалізації провайдерів даних з використанням бібліотек:
   - `CsvHelper` — потокове читання/запис CSV (`aqi_data.csv`).
   - `System.Text.Json` — потокова асинхронна робота з JSON (`air_quality.json`).
   - `System.Xml.Serialization` — серіалізація та десеріалізація XML (`air_quality.xml`).
   - `ClosedXML` — обробка таблиць Excel XLSX (`air_quality.xlsx`) та генерація зведених звітів.
   - `ScottPlot 5` — побудова графіків у форматі PNG (стовпчасті, кругові, лінійні).
   - `DocumentFormat.OpenXml` — автоматична генерація оформленого звіту до лабораторної роботи у форматі DOCX (Word).
   - `TestFileGenerator` — генерація тестових дерев каталогів та файлів з випадковими атрибутами файлової системи.
4. **`lab1.WebApi`** — ASP.NET Core REST API контролери, OpenAPI / NSwag Swagger UI:
   - `CsvController` (`/api/v2/csv`) — імпорт та експорт CSV.
   - `JsonController` (`/api/v2/json`) — імпорт та експорт JSON.
   - `XmlController` (`/api/v2/xml`) — імпорт та експорт XML.
   - `XlsxController` (`/api/v2/xlsx`) — імпорт та експорт Excel XLSX.
   - `RecordsController` (`/api/v2/records`) — пошук, фільтрація, сортування та аналітика (Sum, Median, Min, Max, Count).
   - `ChartsController` (`/api/v2/charts`) — генерація візуалізацій ScottPlot.
   - `ReportsController` (`/api/v2/reports`) — формування звітів XLSX та DOCX.
   - `TestFilesController` (`/api/v1/test-files`) — генерація тестових файлів.
   - `FolderSyncController` (`/api/v1/folder-sync`) — аналіз відмінностей та синхронізація папок.

---

## 🚀 Запуск проєкту

```bash
dotnet restore
dotnet build
dotnet run --project src/lab1/lab1.WebApi/lab1.WebApi.csproj
```

Після запуску документація Swagger UI доступна за адресою:
`http://localhost:5112/swagger` або `https://localhost:7202/swagger`

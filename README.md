# VetClinic

**VetClinic** — учебный fullstack-проект для автоматизации работы ветеринарной клиники. Backend реализован на ASP.NET Core Web API, клиентская часть — на Blazor WebAssembly, а общий контракт данных вынесен в отдельную shared-библиотеку. Проект включает основные учебные сценарии для работы ветеринарной клиники: роли пользователей, питомцев, записи на приём, медицинские записи, вакцинации, склад, счета и уведомления.

## Возможности

- Регистрация и авторизация пользователей с JWT-аутентификацией.
- Ролевая модель доступа: администратор, ветеринарный врач, владелец питомца и ассистент.
- Базовое управление питомцами, владельцами, сотрудниками и услугами клиники.
- Поддержка сценариев для записей на приём, медицинских записей, вакцинаций и операций.
- Работа со стационаром: госпитализации и журналы ухода.
- Работа со складским учётом: позиции склада, остатки, сроки годности и складские операции.
- Базовая работа со счетами и административными отчётами.
- Уведомления через REST API и SignalR Hub.
- Клиентская часть с навигацией и сценариями, зависящими от роли пользователя.

## Стек технологий

**Backend**

- C# / .NET 8
- ASP.NET Core Web API
- ASP.NET Core Identity
- JWT Bearer Authentication
- Entity Framework Core
- PostgreSQL через Npgsql
- SignalR
- Swagger / OpenAPI

**Frontend**

- Blazor WebAssembly
- Razor Components
- HttpClient для обращения к REST API
- SignalR Client
- Bootstrap и собственные CSS-стили

**Shared**

- Общие DTO для request/response-моделей
- Enum-типы для ролей, статусов и категорий

## Скриншоты

### Главная страница

![Главная страница](documentation/screenshots/public-home.png)

### Авторизация

![Авторизация](documentation/screenshots/public-login.png)

### Панель администратора

![Панель администратора](documentation/screenshots/admin-dashboard.png)

### Кабинет владельца питомца

![Кабинет владельца питомца](documentation/screenshots/owner-profile-or-pets.png)

### Рабочее пространство ветеринара

![Рабочее пространство ветеринара](documentation/screenshots/veterinarian-workspace.png)

## Архитектура проекта

Решение разделено на три проекта:

- `VetClinic.Api` — backend на ASP.NET Core Web API. Здесь находятся контроллеры, EF Core `DbContext`, модели базы данных, сервисы бизнес-логики, миграции, Identity, JWT-настройки и SignalR Hub.
- `VetClinic.Client` — клиентская часть на Blazor WebAssembly. Клиент обращается к API через сервисы, хранит данные авторизации в `localStorage` и отображает страницы для разных пользовательских ролей.
- `VetClinic.Shared` — общая библиотека с DTO, request/response-моделями и enum-типами, которые используются backend и frontend.

Основной поток работы:

1. Пользователь регистрируется или входит через `/api/auth`.
2. API выдаёт JWT-токен с ролью пользователя.
3. Blazor-клиент добавляет токен к защищённым API-запросам.
4. Backend проверяет доступ через `[Authorize]` и роли.
5. Уведомления доставляются через REST API и SignalR Hub `/hubs/notifications`.

## Структура репозитория

```text
VetClinic/
├── VetClinic.Api/
│   ├── Controllers/          # REST API controllers
│   ├── Data/                 # AppDbContext и инициализация данных
│   ├── Hubs/                 # SignalR Hub для уведомлений
│   ├── Interfaces/           # Интерфейсы сервисов
│   ├── Migrations/           # EF Core migrations
│   ├── Models/               # Entity-модели
│   ├── Services/             # Бизнес-логика
│   ├── Program.cs            # Конфигурация API
│   └── appsettings*.json     # Конфигурация приложения
├── VetClinic.Client/
│   ├── Components/           # Переиспользуемые UI-компоненты
│   ├── Layout/               # Layout и навигация
│   ├── Pages/                # Страницы Blazor
│   ├── Services/             # API-клиенты и клиентские сервисы
│   └── wwwroot/              # Статика клиента
├── VetClinic.Shared/
│   ├── Enums/                # Роли, статусы и категории
│   ├── Requests/             # DTO запросов
│   └── Responses/            # DTO ответов
├── documentation/
│   ├── screenshots/          # Скриншоты интерфейса
│   ├── ТЗ.pdf
│   ├── УчебкаОтчетСмирнов.pdf
│   └── УчебкаОтчетСмирнов.docx
├── README.md
└── VetClinic.slnx
```

## Запуск проекта локально

### Требования

- .NET SDK 8
- PostgreSQL
- `dotnet-ef` для применения миграций EF Core

Установка `dotnet-ef`, если инструмент ещё не установлен:

```bash
dotnet tool install --global dotnet-ef
```

### 1. Клонировать репозиторий

```bash
git clone https://github.com/elon1te9/vet-clinic.git
cd vet-clinic
```

### 2. Восстановить зависимости

```bash
dotnet restore VetClinic.slnx
```

### 3. Настроить PostgreSQL и локальную конфигурацию API

Создайте базу данных PostgreSQL, например `vetclinicdb_db`.

Для локального запуска можно создать `VetClinic.Api/appsettings.Development.json` на основе `VetClinic.Api/appsettings.Development.example.json` и указать свои значения:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=vetclinicdb_db;Username=<your_username>;Password=<your_password>"
  },
  "Jwt": {
    "Issuer": "VetClinic.Api",
    "Audience": "VetClinic.Client",
    "Key": "<your_jwt_key>",
    "ExpiresMinutes": 120
  }
}
```

Для локальных секретов также можно использовать User Secrets или переменные окружения. Реальные пароли, токены и JWT-ключи не следует хранить в публичной конфигурации.

### 4. Применить миграции

Из корня репозитория:

```bash
dotnet ef database update --project VetClinic.Api
```

### 5. Запустить backend API

```bash
dotnet run --project VetClinic.Api --launch-profile https
```

В режиме разработки API доступен по адресам:

- `https://localhost:7096`
- Swagger UI: `https://localhost:7096/swagger`

### 6. Запустить Blazor WebAssembly клиент

В отдельном терминале:

```bash
dotnet run --project VetClinic.Client --launch-profile https
```

Клиент доступен по адресу:

- `https://localhost:7131`

В `VetClinic.Client/Program.cs` базовый адрес API задан как `https://localhost:7096`.

## Конфигурация

Основные локальные значения:

- `ConnectionStrings:DefaultConnection` — строка подключения к PostgreSQL.
- `Jwt:Issuer` — издатель JWT-токена, например `VetClinic.Api`.
- `Jwt:Audience` — аудитория JWT-токена, например `VetClinic.Client`.
- `Jwt:Key` — длинный локальный секретный ключ: `<your_jwt_key>`.
- `Jwt:ExpiresMinutes` — время жизни access token в минутах.

Дополнительно `DbInitializer` создаёт базовые роли и демонстрационные записи для локальной работы с приложением. Демонстрационные данные предназначены для локальной разработки. Пароли и секретные значения следует задавать только в локальной конфигурации, User Secrets или переменных окружения.

## API

Swagger-документация доступна в Development-режиме по адресу `https://localhost:7096/swagger`.

Основные группы маршрутов:

- `/api/auth` — регистрация владельца, регистрация сотрудника администратором, вход и получение текущего пользователя.
- `/api/users`, `/api/owners`, `/api/staff` — пользователи, владельцы, сотрудники и управление ролями.
- `/api/pets` — питомцы и данные владельцев.
- `/api/appointments` — записи на приём и статусы приёмов.
- `/api/medical-records` — медицинские записи питомцев.
- `/api/vaccinations` — вакцинации, предстоящие и просроченные вакцинации.
- `/api/services` — услуги клиники.
- `/api/inventory` и `/api/inventory-transactions` — складские позиции и операции.
- `/api/surgeries` — операции.
- `/api/hospitalizations` и `/api/care-logs` — стационар и журналы ухода.
- `/api/invoices` — счета.
- `/api/reports` — отчёты по выручке, загрузке врачей, услугам, складу и приёмам.
- `/api/notifications` — уведомления пользователя.
- `/api/admin/dashboard` — сводка для административной панели.
- `/hubs/notifications` — SignalR Hub для уведомлений.

## Полученные навыки

- Проектирование REST API на ASP.NET Core с разделением на контроллеры, сервисы и DTO.
- Работа с Entity Framework Core, PostgreSQL, миграциями и связями между сущностями.
- Настройка ASP.NET Core Identity, JWT-аутентификации и ролевой авторизации.
- Организация fullstack-решения с отдельными backend, frontend и shared-проектами.
- Создание клиентской части на Blazor WebAssembly с API-сервисами и ролевой навигацией.
- Интеграция SignalR для уведомлений в web-приложении.
- Подготовка административных и предметных сценариев для домена ветеринарной клиники.

## Возможные улучшения

- Добавить автотесты для сервисов, контроллеров и ключевых пользовательских сценариев.
- Добавить Docker Compose для API, клиента и PostgreSQL.
- Настроить GitHub Actions workflow для автоматической сборки и проверки проекта.
- Расширить обработку ошибок и клиентскую валидацию форм.
- Добавить пагинацию, фильтрацию и сортировку для больших списков.
- Расширить управление сессиями, например refresh tokens.
- Расширить API-документацию примерами запросов и ответов.

## Документация

В папке `documentation` находятся дополнительные материалы учебного проекта:

- `documentation/ТЗ.pdf` — техническое задание.
- `documentation/УчебкаОтчетСмирнов.pdf` — отчёт по учебной практике.
- `documentation/УчебкаОтчетСмирнов.docx` — редактируемая версия отчёта.
- `documentation/screenshots` — скриншоты интерфейса.

# VetClinic

**VetClinic** — учебное веб-приложение для управления ветеринарной клиникой.  
Реализовано с использованием **Blazor WebAssembly**, **ASP.NET Core Web API**, **Entity Framework Core** и **PostgreSQL**.  
---

## 🚀 Основные возможности

### Для администратора
- Управление пользователями, владельцами и питомцами
- Управление врачами и ассистентами
- Настройка услуг и расписания клиники
- Контроль склада и материалов
- Финансовые отчёты
- Административная панель с аналитикой
- Уведомления о критических остатках и событиях

### Для ветеринарного врача
- Просмотр своих приёмов
- Создание и ведение медицинских карт
- Планирование вакцинаций и операций
- Послеоперационный уход
- Просмотр информации о стационаре и складе
- Уведомления о новых приёмах и изменениях

### Для владельца питомца
- Регистрация и авторизация
- Добавление и редактирование питомцев
- Онлайн-запись на приём
- Просмотр медицинской истории
- Просмотр вакцинаций
- Просмотр счетов и статуса оплаты
- Уведомления о предстоящих приёмах и вакцинациях

### Для ассистента
- Работа со складом (приход, расход, списание)
- Контроль критических остатков и сроков годности
- Записи ухода за питомцами в стационаре
- Просмотр госпитализаций
- Получение уведомлений о стационаре и складе

---

## 🏗 Технологический стек

**Backend**
- C#, ASP.NET Core Web API, REST API
- Entity Framework Core + PostgreSQL
- ASP.NET Core Identity + JWT
- SignalR для real-time уведомлений
- Swagger / OpenAPI
- Логирование: Serilog / ILogger

**Frontend**
- Blazor WebAssembly
- Razor Components
- HttpClient для REST API
- SignalR Client
- JWT авторизация
- Role-based UI

**Shared**
- DTO-запросы и ответы
- Enum-типы

---

## ⚙️ Запуск проекта

1. Клонировать репозиторий:

```bash
git clone https://github.com/elon1te9/VetClinic.git
cd VetClinic
```

2. Настроить подключение к базе данных в `appsettings.json`.

3. Применить миграции:

```bash
cd VetClinic.Api
dotnet ef database update
```

4. Запустить сервер:

```bash
dotnet run
```

5. Запустить клиент:

```bash
cd ../VetClinic.Client
dotnet run
```

6. Открыть [http://localhost:5000](http://localhost:5000)

---

## 👥 Тестовые пользователи

| Роль        | Email                  | Пароль       |
|------------|-----------------------|-------------|
| Admin      | admin@vetclinic.local | Password123! |
| Veterinarian | doctor@vetclinic.local | Password123! |
| Owner      | owner@vetclinic.local | Password123! |
| Assistant  | assistant@vetclinic.local | Password123! |

---

## 📦 REST API

Основные endpoints: `/api/auth`, `/api/pets`, `/api/appointments`, `/api/medical-records`, `/api/vaccinations`, `/api/inventory`, `/api/surgeries`, `/api/hospitalizations`, `/api/invoices`, `/api/reports`, `/api/notifications`, `/api/admin/dashboard`.

Полный список доступен через Swagger на сервере.

---

## 💡 Особенности реализации

- Разграничение доступа по ролям через ASP.NET Core Identity
- JWT-аутентификация
- SignalR уведомления в реальном времени
- Ролевая навигация в Blazor WebAssembly
- Простая и понятная архитектура учебного проекта
- Упрощённые уведомления и расчёт вакцинаций для демонстрации

---

## 📚 Литература и справка

- Техническое задание и отчёт по учебной практике проекта VetClinic представлены в папке documentation.

namespace VetClinic.Client.Components;

public static class UiText
{
    public const string Empty = "—";

    public static string Value(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? Empty : value;
    }

    public static string Date(DateTime? value, bool withTime = true)
    {
        if (value is null)
        {
            return Empty;
        }

        return value.Value.ToLocalTime().ToString(withTime ? "dd.MM.yyyy HH:mm" : "dd.MM.yyyy");
    }

    public static string Money(decimal value)
    {
        return $"{value:0.00} ₽";
    }

    public static string Quantity(decimal value, string? unit)
    {
        var unitText = Value(unit);

        return $"{value:0.00} {unitText}";
    }

    public static string Role(object? role)
    {
        return role?.ToString() switch
        {
            "Admin" => "Администратор",
            "Veterinarian" => "Ветеринарный врач",
            "Owner" => "Владелец питомца",
            "Assistant" => "Ассистент",
            _ => Value(role?.ToString())
        };
    }

    public static string Gender(object? gender)
    {
        return gender?.ToString() switch
        {
            "Male" => "Самец",
            "Female" => "Самка",
            "Unknown" => "Не указан",
            _ => Value(gender?.ToString())
        };
    }

    public static string Category(object? category)
    {
        return category?.ToString() switch
        {
            "Medicine" => "Лекарство",
            "Vaccine" => "Вакцина",
            "Food" => "Корм",
            "Material" => "Материал",
            _ => Value(category?.ToString())
        };
    }

    public static string NotificationType(object? type)
    {
        return type?.ToString() switch
        {
            "Appointment" => "Запись на приём",
            "Vaccination" => "Вакцинация",
            "Inventory" => "Склад",
            "Surgery" => "Операция",
            "Hospitalization" => "Стационар",
            "Finance" => "Финансы",
            "System" => "Система",
            _ => Value(type?.ToString())
        };
    }

    public static string Status(object? status)
    {
        return status?.ToString() switch
        {
            "Planned" => "Запланировано",
            "Confirmed" => "Подтверждено",
            "Completed" => "Завершено",
            "Cancelled" => "Отменено",
            "Active" => "Активно",
            "Paid" => "Оплачено",
            "Unpaid" => "Не оплачено",
            "Overdue" => "Просрочено",
            "InProgress" => "В процессе",
            "Incoming" => "Приход",
            "Outgoing" => "Расход",
            "WriteOff" => "Списание",
            "Correction" => "Корректировка",
            "Critical" => "Критично",
            "New" => "Новое",
            "Read" => "Прочитано",
            _ => Value(status?.ToString())
        };
    }

    public static string StatusTone(object? status, string? text = null)
    {
        var value = status?.ToString() ?? text ?? string.Empty;

        return value switch
        {
            "Paid" or "Active" or "Confirmed" or "Incoming" or "Read" => "success",
            "Completed" or "Cancelled" or "Overdue" or "Critical" or "WriteOff" => "danger",
            "Planned" or "Unpaid" or "InProgress" or "Outgoing" or "Correction" or "New" => "warning",
            _ => "neutral"
        };
    }
}

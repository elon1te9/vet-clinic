namespace VetClinic.Shared.Requests;

public class RegisterStaffRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Specialization { get; set; }
    public string Role { get; set; } = string.Empty;
}

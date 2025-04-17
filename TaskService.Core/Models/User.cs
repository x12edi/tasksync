namespace TaskService.Core.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty; // Hashed password
    public string Role { get; set; } = "User"; // User or Manager
    public string RefreshToken { get; set; } = string.Empty; // For refresh
    public DateTime RefreshTokenExpiry { get; set; }
}
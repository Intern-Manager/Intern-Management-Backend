namespace InternManagement.Infrastructure.Auth;

public class JwtOptions
{
    public string Issuer { get; set; } = "InternManagement";
    public string Audience { get; set; } = "InternManagement";
    public string Key { get; set; } = string.Empty;
    public int AccessTokenMinutes { get; set; } = 60;
}


namespace TechChallenge.JwtService;

public class JwtSettings
{
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = "TechChallengeAuthServer";
    public string Audience { get; set; } = "TechChallengeUsers";
    public int ExpireHours { get; set; } = 8;
}

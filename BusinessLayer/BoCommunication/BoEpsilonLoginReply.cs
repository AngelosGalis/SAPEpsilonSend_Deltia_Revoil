using System;

public class BoEpsilonLoginReply
{
    public string jwt { get; set; }
    public DateTime jwtExpiration { get; set; }
    public string jwtRefreshToken { get; set; }
    public DateTime jwtRefreshTokenExpiration { get; set; }
    public string url1 { get; set; }
}

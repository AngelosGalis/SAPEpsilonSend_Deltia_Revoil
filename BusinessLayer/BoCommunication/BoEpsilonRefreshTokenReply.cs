using System;

public class BoEpsilonRefreshTokenReply
{
    public string jwt { get; set; }
    public DateTime jwtExpiration { get; set; }
    public string jwtRefreshToken { get; set; }
    public DateTime jwtRefreshTokenExpiration { get; set; }
}

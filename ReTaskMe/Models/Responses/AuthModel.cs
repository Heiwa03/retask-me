

namespace ReTaskMe.Models.Responses;

public class AuthModel{
    public string? message {get; set; }
    public bool isVerified {get; set; }
    public string? token {get; set; }
    public string? refreshToken {get; set; }
}
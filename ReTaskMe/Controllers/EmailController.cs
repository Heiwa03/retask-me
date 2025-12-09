using Microsoft.AspNetCore.Mvc;
using HelperLayer.Security.Token;
using DataAccessLayerCore.Repositories.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

[ApiController]
[Route("api/v1/[controller]")]
public class EmailController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly RsaSecurityKey _publicKey;

    public EmailController(
        IUserRepository userRepository,
        IConfiguration configuration)
    {
        _userRepository = userRepository;

        _issuer = configuration["Jwt:Issuer"] ?? throw new ArgumentNullException("Jwt:Issuer");
        _audience = configuration["Jwt:Audience"] ?? throw new ArgumentNullException("Jwt:Audience");
        Console.WriteLine($"Issuer from config: '{_issuer}'");
        Console.WriteLine($"Audience from config: '{_audience}'");


        // Load public key
        string publicKeyPem = System.IO.File.ReadAllText(configuration["Jwt:PublicKeyPem"] ?? "public_key.pem");
        var rsa = RSA.Create();
        rsa.ImportFromPem(publicKeyPem.ToCharArray());
        _publicKey = new RsaSecurityKey(rsa);
    }

    [HttpGet("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromQuery] string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return BadRequest("Token is required.");

        Guid userUuid;
        try
        {
            userUuid = TokenHelper.ValidateJwtToken(token, _issuer, _audience, _publicKey);
        }
        catch
        {
            return BadRequest("Invalid or expired token.");
        }

        var user = await _userRepository.GetUserByUuidAsync(userUuid); // Use UUID instead of email

        if (user == null)
            return NotFound("User not found.");

        if (user.IsVerified)
            return Ok("Email already verified.");

        user.IsVerified = true;
        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();

        return Ok("Email successfully verified!");
    }
}

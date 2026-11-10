using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Dapper;
using FlashcardBackend.Data;
using FlashcardBackend.Dtos;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.Tokens;

namespace FlashcardBackend.Helpers;

public class AuthHelper
{
    private readonly IConfiguration _config;
    private readonly DataContextDapper _dapper;

    public AuthHelper(IConfiguration config)
    {
        _config = config;
        _dapper = new DataContextDapper(config);
    }

    public bool SetPassword(UserRegistrationDto userRegDetails)
    {
        byte[] passwordSalt = new byte[128 / 8];
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            rng.GetNonZeroBytes(passwordSalt);
        }

        byte[] passwordHash = GetPasswordHash(userRegDetails.Password, passwordSalt);

        string sql = "spUsers_Register";

        var sqlParamaters = new DynamicParameters();
        sqlParamaters.Add("@Name", userRegDetails.Name);
        sqlParamaters.Add("@Email", userRegDetails.Email);
        sqlParamaters.Add("@PasswordHash", passwordHash);
        sqlParamaters.Add("@PasswordSalt", passwordSalt);

        return _dapper.ExecuteSql(sql, sqlParamaters, System.Data.CommandType.StoredProcedure);
    }

    public byte[] GetPasswordHash(string password, byte[] passwordSalt)
    {
        string passwordSaltPlusString = _config.GetSection("AppSettings:PasswordKey").Value + 
        Convert.ToBase64String(passwordSalt);

        return KeyDerivation.Pbkdf2(
            password: password,
            salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 1000000,
            numBytesRequested: 256 / 8
        );
    }

    public string CreateToken(int userId)
    {
        Claim[] claims = {
            new Claim("userId", userId.ToString())
        };

        string tokenKeyString = _config["AppSettings:TokenKey"]
            ?? throw new InvalidOperationException("JTW signing key is missing");

        SymmetricSecurityKey tokenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKeyString));

        SigningCredentials credentials = new SigningCredentials(
            tokenKey,
            SecurityAlgorithms.HmacSha512Signature
        );

        SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(claims),
            SigningCredentials = credentials,
            Expires = DateTime.UtcNow.AddDays(1),

            Issuer = "FlashcardAPI",
            Audience = "FlashcardAPI"
        };

        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

        SecurityToken token = tokenHandler.CreateToken(descriptor);

        return tokenHandler.WriteToken(token);
    }
}
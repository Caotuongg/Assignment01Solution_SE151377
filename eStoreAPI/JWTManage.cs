using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace eStoreAPI
{
    public class JwtTokens
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
    public class JWTManage
    {
        private static IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();

        public static JwtTokens GetToken(string role, string id)
        {
            return GenerateJWTToken(role, id);
        }
        public static JwtTokens GEtRefreshToken(string role, string id)
        {
            return GenerateJWTToken(role, id);
        }
        private static JwtTokens GenerateJWTToken(string role, string id)
        {
            // tạo mới một token
            var tokenHandler = new JwtSecurityTokenHandler();
            // lấy chuỗi "Key" từ appsettings để mã hóa
            var tokenKey = Encoding.UTF8.GetBytes(config["JWT:Key"]);
            // Cung cấp token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                // Claim là các đặc trưng của người dùng 
                Subject = new System.Security.Claims.ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Role, role),
                    new Claim(ClaimTypes.NameIdentifier, id),
                }),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var refreshToken = GenerateRefreshToken();
            return new JwtTokens
            {
                // lấy cái chuỗi token được trả về
                Token = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken,
            };
        }

        public static ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenKey = Encoding.UTF8.GetBytes(config["JWT:Key"]);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,// ko kiểm tra token hết hạn
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(tokenKey),
                ClockSkew = TimeSpan.Zero
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            // check : Accesstoken valid format
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken;
            // Check alg(thuật toán)
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }

        // Hàm để lấy token khi đăng nhập hết hạn
        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
    }
}


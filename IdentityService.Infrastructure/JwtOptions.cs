namespace IdentityService.Infrastructure
{
    public class JwtOptions
    {
        public const string JwtOptionSection = "JwtOptions";

        public string Issuer { get; set; }

        public string Audience { get; set; }

        public string SecretKey { get; set; }

        public int Lifetime { get; set; }
    }
}

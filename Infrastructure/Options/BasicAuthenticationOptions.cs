

namespace LoggingMicroservice.Infrastructure.Options
{
    public class BasicAuthenticationOptions
    {
        /// <summary>
        /// Валидный логин
        /// </summary>
        public required string Username { get; set; }
        
        /// <summary>
        /// Валидный пароль
        /// </summary>
        public required string Password { get; set; }
        
    }
}
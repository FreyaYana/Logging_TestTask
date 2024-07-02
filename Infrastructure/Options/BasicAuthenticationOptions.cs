

namespace LoggingMicroservice.Infrastructure.Options
{
    public class BasicAuthenticationOptions
    {
        /// <summary>
        /// Валидный логин
        /// </summary>
        public string Username { get; set; }
        
        /// <summary>
        /// Валидный пароль
        /// </summary>
        public string Password { get; set; }
        
    }
}
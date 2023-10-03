namespace blogpessoal.Security
{
    public class Settings
    {
        private static string secret = "44a2f3c79a1f121f7e8c0b083facbb87571524550c54c3f28e29b9435d358393";

        public static string Secret { get => secret; set => secret = value; } 
    }
}

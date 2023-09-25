using System.ComponentModel.DataAnnotations.Schema;

namespace blogpessoal.Model
{
    public class Auditable
    {
        public DateTimeOffset? Data { get; set; }
    }
}

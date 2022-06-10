using System.Text.Json.Serialization;

namespace Capstone.Models
    {
    public class RequestLine
        {
        public int Id { get; set; } = 0;
        public int RequestId { get; set; } = 0;
        public int ProductId { get; set; } = 0;
        public int Quantity { get; set; } = 1;

        public virtual Product? Product { get; set; } = null!;
        [JsonIgnore]
        public virtual Request? Request { get; set; } = null!;
      


        }
    }

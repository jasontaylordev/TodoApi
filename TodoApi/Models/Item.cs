using System.ComponentModel.DataAnnotations;

namespace TodoApi.Models
{
    public class Item
    {
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        public bool Done { get; set; }
    }
}
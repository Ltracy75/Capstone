using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Capstone.Models
    {
    [Index(nameof(Code), IsUnique = true)]
    public class Vendor
        {
        public int Id { get; set; } = 0;
        [StringLength(30)]
        public string Code { get; set; } = string.Empty;
        [StringLength(30)]
        public string Name { get; set; } = string.Empty;
        [StringLength(30)]
        public string Address { get; set; } = string.Empty;
        [StringLength(30)]
        public string? City { get; set; }
        [StringLength(2)]
        public string? State { get; set; }
        [StringLength(5)]
        public string? Zip { get; set; }
        [StringLength(12)]
        public string? Phone { get; set; }
        [StringLength(255)]
        public string? Email { get; set; }
        }
    }

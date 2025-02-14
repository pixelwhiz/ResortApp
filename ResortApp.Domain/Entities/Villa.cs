using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResortApp.Domain.Entities
{
    public class Villa
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public double Price { get; set; }
        public int Sqft { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime? CratedDate { get; set; }
        public DateTime? UpdatedTime { get; set; }
    }
}

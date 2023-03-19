using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Theatre.Data.Models.Enums;

namespace Theatre.Data.Models
{
    public class Play
    {
       
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50,MinimumLength =4)]
        public string Title { get; set; }

        [Required]
       // [DataType(DataType.Time)]
        [Column(TypeName ="time")]       
        public TimeSpan Duration { get; set; }


        [Required]
        [Range(typeof(float),"0.00","10.00")]
        public float Rating { get; set; }

        [Required]
        public Genre Genre { get; set; }


        [Required]
        [StringLength(700)]
        public string Description { get; set; }


        [Required]
        [StringLength(30,MinimumLength =4)]
        public string Screenwriter { get; set; }



        public ICollection<Cast> Casts { get; set; } = new HashSet<Cast>();

        public ICollection<Ticket> Tickets { get; set; } = new HashSet<Ticket>();  
    }
}

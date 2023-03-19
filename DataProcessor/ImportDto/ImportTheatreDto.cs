using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Theatre.DataProcessor.ImportDto
{
    [JsonObject]
    public class ImportTheatreDto
    {
        [Required(AllowEmptyStrings =false)]
        [StringLength(30, MinimumLength = 4)]
        public string Name { get; set; }

        [Required]
        [Range(typeof(sbyte),"1", "10")]
        public sbyte NumberOfHalls { get; set; }

        [StringLength(30, MinimumLength = 4)]
        [Required(AllowEmptyStrings =false)]
        public string Director { get; set; }

        public ICollection<ImportTicketDto> Tickets { get; set; } = new HashSet<ImportTicketDto>();

    }
}

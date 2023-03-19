using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Theatre.DataProcessor.ExportDto
{
    [JsonObject]
    public class ExportTheatreDto
    {        

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Halls")]
        public sbyte Halls { get; set; }

        [JsonProperty("TotalIncome")]
        public decimal TotalIncome { get; set; }

        [JsonProperty("Tickets")]
        public ICollection<ExportTicketDto> Tickets { get; set; } = new HashSet<ExportTicketDto>();

    }
}

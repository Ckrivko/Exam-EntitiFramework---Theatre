using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Theatre.DataProcessor.ExportDto
{
    [JsonObject]
    public class ExportTicketDto
    {
        
        [JsonProperty("Price")]
        public decimal Price { get; set; }

        [JsonProperty("RowNumber")]
        public sbyte RowNumber { get; set; }

    }
}

namespace Theatre.DataProcessor
{
    using Newtonsoft.Json;
    using System;
    using System.Linq;
    using System.Text;
    using Theatre.Data;
    using Theatre.DataProcessor.ExportDto;
    using System.Xml.Serialization;
    using System.IO;
    using Microsoft.EntityFrameworkCore;
    using System.Globalization;

    public class Serializer
    {
        public static string ExportTheatres(TheatreContext context, int numbersOfHalls)
        {

            var theatres = context.Theatres
                .ToList()
                .Where(x => x.NumberOfHalls >= numbersOfHalls && x.Tickets.Count >= 20)
                .OrderByDescending(x => x.NumberOfHalls)
                .ThenBy(x => x.Name)
                .Select(x => new ExportTheatreDto
                {
                    Name = x.Name,
                    Halls = x.NumberOfHalls,
                    TotalIncome = x.Tickets.Where(a => a.RowNumber >= 1 && a.RowNumber <= 5)
                    .Sum(a => a.Price),
                    Tickets = x.Tickets.Where(a => a.RowNumber >= 1 && a.RowNumber <= 5)
                    .Select(a => new ExportTicketDto
                    {
                        Price = a.Price,
                        RowNumber = a.RowNumber,
                    })
                    .OrderByDescending(x => x.Price)
                    .ToList()


                }).ToList();

            var json = JsonConvert.SerializeObject(theatres, Formatting.Indented);
            return json;

        }

        public static string ExportPlays(TheatreContext context, double rating)
        {
            StringBuilder sb = new StringBuilder();

            // Order the result by play title(ascending), 
            // then by genre(descending).Order actors by their full name descending.

            var playsDto = context.Plays
                .Include(x=>x.Casts)
                .ToArray()
                .Where(x => x.Rating <= (float)rating)
                .Select(x => new ExportPlayDto
                {
                    Title = x.Title,
                    Duration = x.Duration.ToString("c"),
                    Rating = x.Rating > 0 ? x.Rating.ToString(CultureInfo.InvariantCulture) : "Premier",
                    Genre = x.Genre.ToString(),
                    Actors = x.Casts.Where(a => a.IsMainCharacter).Select(a => new ExportActorDto
                    {
                        FullName = a.FullName,
                        MainCharacter = $"Plays main character in '{x.Title}'."

                    })
                    .OrderByDescending(x=>x.FullName)
                    .ToArray()

                })
                .OrderBy(x => x.Title)
                .ThenByDescending(x => x.Genre)
                .ToArray();
          


            XmlRootAttribute root = new XmlRootAttribute("Plays");
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "");

            var xmlSerializer = new XmlSerializer(typeof(ExportPlayDto[]), root);
            using StringWriter writer = new StringWriter(sb);

            xmlSerializer.Serialize(writer, playsDto, namespaces);

            return sb.ToString().Trim();

        }
    }
}

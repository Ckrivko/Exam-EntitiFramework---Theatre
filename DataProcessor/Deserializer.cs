namespace Theatre.DataProcessor
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Xml.Serialization;
    using Theatre.Data;
    using Theatre.Data.Models;
    using Theatre.Data.Models.Enums;
    using Theatre.DataProcessor.ImportDto;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfulImportPlay
            = "Successfully imported {0} with genre {1} and a rating of {2}!";

        private const string SuccessfulImportActor
            = "Successfully imported actor {0} as a {1} character!";

        private const string SuccessfulImportTheatre
            = "Successfully imported theatre {0} with #{1} tickets!";

        public static string ImportPlays(TheatreContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();
            XmlRootAttribute root = new XmlRootAttribute("Plays");
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportPlayDto[]), root);

            using (StringReader reader = new StringReader(xmlString))
            {
                var import = (ImportPlayDto[])xmlSerializer.Deserialize(reader);

                var plays = new List<Play>();


                foreach (var playDto in import)
                {

                    if (!IsValid(playDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (!Enum.TryParse(typeof(Genre), playDto.Genre, out var genre))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }


                    var duration = TimeSpan.ParseExact(playDto.Duration, "c", CultureInfo.InvariantCulture);
                    if (duration.Hours < 1)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var play = new Play
                    {
                        Title = playDto.Title,
                        Duration = duration,
                        Rating = playDto.Rating,
                        Genre = (Genre)genre,
                        Description = playDto.Description,
                        Screenwriter = playDto.Screenwriter
                    };

                    plays.Add(play);

                    sb.AppendLine(String.Format(SuccessfulImportPlay, playDto.Title, playDto.Genre,playDto.Rating));

                }

                context.Plays.AddRange(plays);
                context.SaveChanges();


                return sb.ToString().Trim();
            }
                      
        }

        public static string ImportCasts(TheatreContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();
            XmlRootAttribute root = new XmlRootAttribute("Casts");
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportCastDto[]), root);

            using (StringReader reader = new StringReader(xmlString))
            {
                var import = (ImportCastDto[])xmlSerializer.Deserialize(reader);

                var casts = new List<Cast>();


                foreach (var castDto in import)
                {
                    if (!IsValid(castDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    //if (!ValidPhoneNumbers.validPhoneNumbers.Contains(castDto.PhoneNumber))
                    //{
                    //    sb.AppendLine(ErrorMessage);
                    //    continue;
                    //}

                    var isMainCharacter = true;
                    var character = "main";
                    if (castDto.IsMainCharacter == "false")
                    { 
                    isMainCharacter= false;
                        character = "lesser";
                    }


                    var cast = new Cast
                    {
                        FullName = castDto.FullName,
                        PhoneNumber = castDto.PhoneNumber,
                        IsMainCharacter = isMainCharacter,
                        PlayId = castDto.PlayId

                    };

                    casts.Add(cast);
                    sb.AppendLine(String.Format(SuccessfulImportActor, castDto.FullName, character));

                }

                context.Casts.AddRange(casts);
                context.SaveChanges();

                return sb.ToString().Trim();

            }

        }

        public static string ImportTtheatersTickets(TheatreContext context, string jsonString)
        {

            var sb = new StringBuilder();

            var import = JsonConvert.DeserializeObject<List<ImportTheatreDto>>(jsonString);

            var result = new List<Theatre>();

            foreach (var theatreDto in import)
            {

                if (!IsValid(theatreDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var currTheatre = new Theatre
                {
                    Name = theatreDto.Name,
                    NumberOfHalls = theatreDto.NumberOfHalls,
                    Director = theatreDto.Director
                };

                foreach (var ticket in theatreDto.Tickets)
                {
                    if (!IsValid(ticket))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var currTicket = new Ticket
                    {
                        Price = ticket.Price,
                        RowNumber = ticket.RowNumber,
                        PlayId = ticket.PlayId
                    };

                    currTheatre.Tickets.Add(currTicket);
                }

                result.Add(currTheatre);
                sb.AppendLine(String.Format(SuccessfulImportTheatre, currTheatre.Name, currTheatre.Tickets.Count));

            }
            context.Theatres.AddRange(result);
            context.SaveChanges();

            return sb.ToString().Trim();

        }


        private static bool IsValid(object obj)
        {
            var validator = new ValidationContext(obj);
            var validationRes = new List<ValidationResult>();

            var result = Validator.TryValidateObject(obj, validator, validationRes, true);
            return result;
        }
    }
}

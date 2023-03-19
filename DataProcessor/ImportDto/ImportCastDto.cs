using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Theatre.DataProcessor.ImportDto
{
    [XmlType("Cast")]
    public  class ImportCastDto
    {
        [Required]
        [StringLength(30, MinimumLength = 4)]
        [XmlElement("FullName")]
        public string FullName { get; set; }


        [Required]
        [XmlElement("IsMainCharacter")]
        public string IsMainCharacter { get; set; }

        [Required]
        [StringLength(15)]
        [RegularExpression(@"^\+44-\d{2}-\d{3}-\d{4}$")]
        [XmlElement("PhoneNumber")]
        public string PhoneNumber { get; set; }

        [Required]
        [XmlElement("PlayId")]
        public int PlayId { get; set; }

    }
}

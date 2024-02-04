using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Medicines.DataProcessor.ImportDtos
{
    [XmlType("Pharmacy")]
    public class ImportPharmacyDto
    {
        [Required]
        [MinLength(2)]
        [MaxLength(50)]
        [XmlElement("Name")]
        public string Name { get; set; } = null!;

        [Required]
        [MinLength(14)]
        [MaxLength(14)]
        [RegularExpression(@"^\(\d{3}\) \d{3}-\d{4}$")]
        [XmlElement("PhoneNumber")]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        [XmlAttribute("non-stop")]
        public string IsNonStop { get; set; } = null!;

        [XmlArray("Medicines")]
        public ImportMedicineDto[] Medicines { get; set; }
    }
    //•	Name – text with length[2, 50] (required)
    //•	PhoneNumber – text with length 14. (required)
    //o   All phone numbers must have the following structure: three digits enclosed in parentheses, followed by a space, three more digits, a hyphen, and four final digits: 
    //	Example -> (123) 456-7890 
    //•	IsNonStop – bool (required)
}

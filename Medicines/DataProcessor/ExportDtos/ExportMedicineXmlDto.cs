using System.Xml.Serialization;

namespace Medicines.DataProcessor.ExportDtos
{
    [XmlType("Medicine")]
    public class ExportMedicineXmlDto
    {
        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("Price")]
        public string Price { get; set; }

        [XmlElement("Producer")]
        public string Producer {  get; set; }

        [XmlElement("BestBefore")]
        public string BestBefore { get; set; }

        [XmlAttribute("Category")]
        public string Category { get; set; }
    }
}

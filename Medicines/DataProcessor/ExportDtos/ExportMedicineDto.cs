namespace Medicines.DataProcessor.ExportDtos
{
    public class ExportMedicineDto
    {
        public string Name { get; set; }

        public string Price { get; set; }

        public ExportPharmacyDto Pharmacy { get; set; }
    }
}

namespace Medicines.DataProcessor
{
    using Medicines.Data;
    using Medicines.DataProcessor.ExportDtos;
    using Medicines.JsonXml;
    using System.Globalization;

    public class Serializer
    {
        public static string ExportMedicinesFromDesiredCategoryInNonStopPharmacies(MedicinesContext context, int medicineCategory)
        {
            var medicines = context.Medicines
                .Where(x => (int)x.Category == medicineCategory && x.Pharmacy.IsNonStop)
                .OrderBy(x => x.Price)
                .ThenBy(x => x.Name)
                .Select(x => new ExportMedicineDto()
                {
                    Name = x.Name,
                    Price = x.Price.ToString("0.00"),
                    Pharmacy = new ExportPharmacyDto()
                    {
                        Name = x.Pharmacy.Name,
                        PhoneNumber = x.Pharmacy.PhoneNumber
                    }
                })               
                .ToList();

            return Json.SerializeObject(medicines);
        }

        public static string ExportPatientsWithTheirMedicines(MedicinesContext context, string date)
        {
            var patients = context.Patients
                .Where(x => x.PatientsMedicines.Any(x => x.Medicine.ProductionDate > DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture)))
                .Select(x => new ExportPatientDto() 
                {
                    Name = x.FullName,
                    Gender = x.Gender.ToString().ToLower(),
                    AgeGroup = x.AgeGroup.ToString(),
                    Medicines = x.PatientsMedicines
                    .Where(x => x.Medicine.ProductionDate > DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture))
                    .OrderByDescending(x => x.Medicine.ExpiryDate)
                    .ThenBy(x => x.Medicine.Price)
                    .Select(m => new ExportMedicineXmlDto()
                    {
                        Name = m.Medicine.Name,
                        Price = m.Medicine.Price.ToString("0.00"),
                        Category = m.Medicine.Category.ToString().ToLower(),
                        Producer = m.Medicine.Producer,
                        BestBefore = m.Medicine.ExpiryDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
                    })
                    .ToArray()
                })
                .OrderByDescending(x => x.Medicines.Count())
                .ThenBy(x => x.Name)
                .ToList();

            return Xml.SerializeObject(patients, "Patients");
        }
    }
}

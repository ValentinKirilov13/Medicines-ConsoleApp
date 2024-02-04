namespace Medicines.DataProcessor
{
    using Medicines.Data;
    using Medicines.Data.Models;
    using Medicines.Data.Models.Enums;
    using Medicines.DataProcessor.ImportDtos;
    using Medicines.JsonXml;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Text;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid Data!";
        private const string SuccessfullyImportedPharmacy = "Successfully imported pharmacy - {0} with {1} medicines.";
        private const string SuccessfullyImportedPatient = "Successfully imported patient - {0} with {1} medicines.";

        public static string ImportPharmacies(MedicinesContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            var pharmaciesDtos = Xml.DeserializeObject<List<ImportPharmacyDto>>(xmlString, "Pharmacies");

            List<Pharmacy> pharmaciesForDb = new List<Pharmacy>();

            foreach (var dto in pharmaciesDtos)
            {
                if (!IsValid(dto) || (dto.IsNonStop != "true" && dto.IsNonStop != "false"))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Pharmacy pharmacy = new Pharmacy()
                {
                    Name = dto.Name,
                    PhoneNumber = dto.PhoneNumber,
                    IsNonStop = bool.Parse(dto.IsNonStop),
                };

                List<Medicine> medicines = new List<Medicine>();

                foreach (var medicineDto in dto.Medicines)
                {
                    if (!IsValid(medicineDto) || DateTime.ParseExact(medicineDto.ProductionDate, "yyyy-MM-dd", CultureInfo.InvariantCulture) >= DateTime.ParseExact(medicineDto.ExpiryDate, "yyyy-MM-dd", CultureInfo.InvariantCulture))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    bool exist = false;

                    foreach (var med in medicines)
                    {
                        if (med.Name == medicineDto.Name && med.Producer == medicineDto.Producer)
                        {
                            sb.AppendLine(ErrorMessage);
                            exist = true;
                        }
                    }

                    if (exist) { continue; }

                    Medicine medicine = new Medicine()
                    {
                        Name = medicineDto.Name,
                        Price = medicineDto.Price,
                        Category = (Category)medicineDto.Category,
                        ProductionDate = DateTime.ParseExact(medicineDto.ProductionDate, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                        ExpiryDate = DateTime.ParseExact(medicineDto.ExpiryDate, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                        Producer = medicineDto.Producer,
                    };

                    medicines.Add(medicine);
                }

                pharmacy.Medicines = medicines;
                pharmaciesForDb.Add(pharmacy);
                sb.AppendLine(string.Format(SuccessfullyImportedPharmacy, pharmacy.Name, pharmacy.Medicines.Count));
            }

            context.AddRange(pharmaciesForDb);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }


        public static string ImportPatients(MedicinesContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            var patientsDtos = Json.DeserializeObject<List<ImportPatientDto>>(jsonString);

            List<Patient> patientsForDb = new List<Patient>();

            //int[] medicinesIds = context.Medicines.Select(m => m.Id).ToArray();

            foreach (var dto in patientsDtos)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Patient patient = new Patient()
                {
                    FullName = dto.FullName,
                    AgeGroup = (AgeGroup)dto.AgeGroup,
                    Gender = (Gender)dto.Gender,
                };

                List<PatientMedicine> patientMedicines = new List<PatientMedicine>();

                foreach (var medDtoID in dto.Medicines)
                {
                    if (patientMedicines.Select(x => x.MedicineId).Contains(medDtoID))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    PatientMedicine patientMedicine = new PatientMedicine()
                    {
                        MedicineId = medDtoID,
                        Patient = patient
                    };

                    patientMedicines.Add(patientMedicine);
                }

                patient.PatientsMedicines = patientMedicines;
                patientsForDb.Add(patient);
                sb.AppendLine(string.Format(SuccessfullyImportedPatient, patient.FullName, patient.PatientsMedicines.Count));
            }

            context.AddRange(patientsForDb);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }


        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}

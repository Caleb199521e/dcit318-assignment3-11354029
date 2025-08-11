using System;
using System.Collections.Generic;
using System.Linq;

namespace HealthCareSystem
{
    // Generic Repository
    public class Repository<T>
    {
        private readonly List<T> items = new();

        public void Add(T item) => items.Add(item);
        public List<T> GetAll() => new List<T>(items);
        public T? GetById(Func<T, bool> predicate) => items.FirstOrDefault(predicate);
        public bool Remove(Func<T, bool> predicate)
        {
            var item = items.FirstOrDefault(predicate);
            if (item == null) return false;
            items.Remove(item);
            return true;
        }
    }

    // Patient class
    public class Patient
    {
        public int Id { get; }
        public string Name { get; }
        public int Age { get; }
        public string Gender { get; }

        public Patient(int id, string name, int age, string gender)
        {
            Id = id;
            Name = name;
            Age = age;
            Gender = gender;
        }

        public override string ToString() => $"Patient {Id}: {Name}, Age {Age}, Gender {Gender}";
    }

    // Prescription class
    public class Prescription
    {
        public int Id { get; }
        public int PatientId { get; }
        public string MedicationName { get; }
        public DateTime DateIssued { get; }

        public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
        {
            Id = id;
            PatientId = patientId;
            MedicationName = medicationName;
            DateIssued = dateIssued;
        }

        public override string ToString() => $"Prescription {Id} for Patient {PatientId}: {MedicationName} ({DateIssued:d})";
    }

    public class HealthSystemApp
    {
        private readonly Repository<Patient> _patientRepo = new();
        private readonly Repository<Prescription> _prescriptionRepo = new();
        private readonly Dictionary<int, List<Prescription>> _prescriptionMap = new();

        public void SeedData()
        {
            // Add patients
            _patientRepo.Add(new Patient(1, "Caleb Appiah", 28, "Female"));
            _patientRepo.Add(new Patient(2, "Frederick Johnson", 45, "Male"));
            _patientRepo.Add(new Patient(3, "Asimah Brown", 52, "Female"));

            // Add prescriptions
            _prescriptionRepo.Add(new Prescription(101, 1, "Paracetamol", DateTime.Now.AddDays(-10)));
            _prescriptionRepo.Add(new Prescription(102, 1, "Tabea Herbal Mixture", DateTime.Now.AddDays(-5)));
            _prescriptionRepo.Add(new Prescription(201, 2, "Lisinopril", DateTime.Now.AddDays(-30)));
            _prescriptionRepo.Add(new Prescription(301, 3, "Atorvastatin", DateTime.Now.AddDays(-1)));
            _prescriptionRepo.Add(new Prescription(302, 3, "Aspirin", DateTime.Now.AddDays(-2)));
        }

        public void BuildPrescriptionMap()
        {
            _prescriptionMap.Clear();
            foreach (var p in _prescriptionRepo.GetAll())
            {
                if (!_prescriptionMap.ContainsKey(p.PatientId))
                    _prescriptionMap[p.PatientId] = new List<Prescription>();

                _prescriptionMap[p.PatientId].Add(p);
            }
        }

        public void PrintAllPatients()
        {
            Console.WriteLine("All patients:");
            foreach (var patient in _patientRepo.GetAll())
            {
                Console.WriteLine(patient);
            }
        }

        public List<Prescription> GetPrescriptionsByPatientId(int patientId)
        {
            return _prescriptionMap.ContainsKey(patientId) ? new List<Prescription>(_prescriptionMap[patientId]) : new List<Prescription>();
        }

        public void PrintPrescriptionsForPatient(int id)
        {
            var prescriptions = GetPrescriptionsByPatientId(id);
            Console.WriteLine($"\nPrescriptions for patient {id}:");
            if (!prescriptions.Any())
            {
                Console.WriteLine("No prescriptions found.");
                return;
            }
            foreach (var p in prescriptions)
                Console.WriteLine(p);
        }

        public static void Main()
        {
            var app = new HealthSystemApp();
            app.SeedData();
            app.BuildPrescriptionMap();
            app.PrintAllPatients();

            // choose a patient to display prescriptions
            app.PrintPrescriptionsForPatient(1);
            app.PrintPrescriptionsForPatient(3);
        }
    }
}

using BloodDatabaseASP.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;

namespace BloodDatabaseASP.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private const string bloodTypeFilePath = "Data/bloodtypeentity.json";
        private const string patientFilePath = "Data/patiententity.json";
        private const string recordFilePath = "Data/recordentity.json";

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult BloodTypeEntity(BloodTypeEntity model)
        {
            var bloodTypeList = JsonSerializer.Deserialize<List<BloodTypeEntity>>(System.IO.File.ReadAllText(bloodTypeFilePath));

            return View(bloodTypeList);
        }

        public IActionResult PatientEntity()
        {
            var bloodTypesList = JsonSerializer.Deserialize<List<BloodTypeEntity>>(System.IO.File.ReadAllText(bloodTypeFilePath));
            var patientList = JsonSerializer.Deserialize<List<PatientEntity>>(System.IO.File.ReadAllText(patientFilePath));

            var patientListFull = patientList
                .Join(
                    bloodTypesList,
                    patient => patient.BloodTypeID,
                    bloodType => bloodType.BloodTypeID,
                    (patient, bloodType) => new
                    {
                        patient.PatientID,
                        patient.PatientName,
                        patient.PatientGender,
                        bloodType.BloodTypeName
                    }
                );

            return View(patientListFull);
        }

        public IActionResult AddRecordForm()
        {
            var patients = JsonSerializer.Deserialize<List<PatientEntity>>(System.IO.File.ReadAllText(patientFilePath));

            return View(patients);
        }

        [HttpPost]
        public IActionResult AddRecord(RecordEntity record)
        {
            var records = JsonSerializer.Deserialize<List<RecordEntity>>(System.IO.File.ReadAllText(recordFilePath)) ?? new List<RecordEntity>();

            int newId = records.Any() ? records.Max(r => r.RecordID) + 1 : 1;
            record.RecordID = newId;

            records.Add(record);

            System.IO.File.WriteAllText(recordFilePath, JsonSerializer.Serialize(records));

            TempData["SuccessMessage"] = "Record was added successfully";

            return RedirectToAction("AddRecordForm");
        }

        [HttpGet]
        public IActionResult DeleteRecordForm()
        {
            var records = JsonSerializer.Deserialize<List<RecordEntity>>(System.IO.File.ReadAllText(recordFilePath)) ?? new List<RecordEntity>();

            return View(records);
        }

        [HttpPost]
        public IActionResult DeleteRecords(int[] selectedIds)
        {
            var records = JsonSerializer.Deserialize<List<RecordEntity>>(System.IO.File.ReadAllText(recordFilePath)) ?? new List<RecordEntity>();

            records = records.Where(r => !selectedIds.Contains(r.RecordID)).ToList();

            System.IO.File.WriteAllText(recordFilePath, JsonSerializer.Serialize(records));

            TempData["SuccessMessage"] = "Selected record(s) were deleted successfully";

            return RedirectToAction("DeleteRecordForm");
        }

        [HttpGet]
        public IActionResult Summary()
        {
            var records = JsonSerializer.Deserialize<List<RecordEntity>>(System.IO.File.ReadAllText(recordFilePath)) ?? new List<RecordEntity>();
            var patients = JsonSerializer.Deserialize<List<PatientEntity>>(System.IO.File.ReadAllText(patientFilePath)) ?? new List<PatientEntity>();
            var bloodTypes = JsonSerializer.Deserialize<List<BloodTypeEntity>>(System.IO.File.ReadAllText(bloodTypeFilePath)) ?? new List<BloodTypeEntity>();

            ViewBag.Patients = patients;
            ViewBag.BloodTypes = bloodTypes;

            return View(records);
        }

        [HttpPost]
        public IActionResult Summary(string? patientId, string? bloodTypeId, DateTime? date)
        {
            var records = JsonSerializer.Deserialize<List<RecordEntity>>(System.IO.File.ReadAllText(recordFilePath)) ?? new List<RecordEntity>();
            var patients = JsonSerializer.Deserialize<List<PatientEntity>>(System.IO.File.ReadAllText(patientFilePath)) ?? new List<PatientEntity>();

            int totalQuantity = 0;

            if (!string.IsNullOrEmpty(patientId))
            {
                int pid = int.Parse(patientId);
                totalQuantity = records.Where(r => r.PatientID == pid).Sum(r => r.Quantity);
            }
            else if (!string.IsNullOrEmpty(bloodTypeId))
            {
                int btid = int.Parse(bloodTypeId);
                var patientIdsWithBloodType = patients.Where(p => p.BloodTypeID == btid).Select(p => p.PatientID).ToList();
                totalQuantity = records.Where(r => patientIdsWithBloodType.Contains(r.PatientID)).Sum(r => r.Quantity);
            }
            else if (date.HasValue)
            {
                totalQuantity = records.Where(r => r.DateTime.Date == date.Value.Date).Sum(r => r.Quantity);
            }

            var bloodTypes = JsonSerializer.Deserialize<List<BloodTypeEntity>>(System.IO.File.ReadAllText(bloodTypeFilePath)) ?? new List<BloodTypeEntity>();
            ViewBag.Patients = patients;
            ViewBag.BloodTypes = bloodTypes;
            ViewBag.TotalQuantity = totalQuantity;

            return View(records);
        }


        [HttpGet]
        public IActionResult RecordEntity()
        {
            var records = JsonSerializer.Deserialize<List<RecordEntity>>(System.IO.File.ReadAllText(recordFilePath)) ?? new List<RecordEntity>();
            var patients = JsonSerializer.Deserialize<List<PatientEntity>>(System.IO.File.ReadAllText(patientFilePath)) ?? new List<PatientEntity>();
            var bloodTypes = JsonSerializer.Deserialize<List<BloodTypeEntity>>(System.IO.File.ReadAllText(bloodTypeFilePath)) ?? new List<BloodTypeEntity>();

            ViewBag.Patients = patients;
            ViewBag.BloodTypes = bloodTypes;

            return View(records);
        }

        [HttpPost]
        public IActionResult RecordEntity(string? patientId, string? bloodTypeId, DateTime? date)
        {
            var records = JsonSerializer.Deserialize<List<RecordEntity>>(System.IO.File.ReadAllText(recordFilePath)) ?? new List<RecordEntity>();
            var patients = JsonSerializer.Deserialize<List<PatientEntity>>(System.IO.File.ReadAllText(patientFilePath)) ?? new List<PatientEntity>();
            var bloodTypes = JsonSerializer.Deserialize<List<BloodTypeEntity>>(System.IO.File.ReadAllText(bloodTypeFilePath)) ?? new List<BloodTypeEntity>();

            if (!string.IsNullOrEmpty(patientId))
            {
                int pid = int.Parse(patientId);
                records = records.Where(r => r.PatientID == pid).ToList();
            }
            if (!string.IsNullOrEmpty(bloodTypeId))
            {
                int btid = int.Parse(bloodTypeId);
                var patientIdsWithBloodType = patients.Where(p => p.BloodTypeID == btid).Select(p => p.PatientID).ToList();
                records = records.Where(r => patientIdsWithBloodType.Contains(r.PatientID)).ToList();
            }
            if (date.HasValue)
            {
                records = records.Where(r => r.DateTime.Date == date.Value.Date).ToList();
            }

            ViewBag.Patients = patients;
            ViewBag.BloodTypes = bloodTypes;

            return View(records);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

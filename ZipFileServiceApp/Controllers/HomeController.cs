using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using ZipFileServiceApp.Data;
using ZipFileServiceApp.Models;

namespace ZipFileServiceApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult UploadFiles()
        {
            // Get the files from  Dropzone
            var files = HttpContext.Request.Form.Files;

            // Remove the files from FileRepository just in case there are files
            FileRepository.ResetModel();

            // Read the files and add them to FileRepository
            foreach (var file in files)
            {
                using (var binaryReader = new BinaryReader(file.OpenReadStream()))
                {
                    var bytes = binaryReader.ReadBytes((int)file.Length);
                    FileRepository.AddFiles(new FormFileModel
                    {
                        Data = bytes,
                        File = file,
                    });
                }
            }
            return RedirectToAction("Index");
        }

        public FileResult DownloadZip(MultipleFilesModel model)
        {
            var zipName = $"ZipFiles-{DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss")}.zip";

            // Get the files from FileRepository class
            var files = FileRepository.GetModel().Files;

            // Read the files and create ZipArchive
            using (MemoryStream ms = new MemoryStream())
            {
                using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, true))
                {
                    foreach (var file in files)
                    {
                        var entry = zip.CreateEntry(file.File.FileName);
                        using (var fileStream = new MemoryStream(file.Data))
                        using (var entryStream = entry.Open())
                        {
                            fileStream.CopyTo(entryStream);
                        }
                    }
                }
                var zipFolder = new FileContentResult(ms.ToArray(), "application/zip")
                {
                    FileDownloadName = zipName
                };

                // Remove the files from FileRepository
                FileRepository.ResetModel();

                return zipFolder;
            }
        }

        public IActionResult Clear()
        {
            FileRepository.ResetModel();
            return RedirectToAction("Index");
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
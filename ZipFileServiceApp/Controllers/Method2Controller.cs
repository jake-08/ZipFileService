using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;

namespace ZipFileServiceApp.Controllers
{
    public class Method2Controller : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public Method2Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult UploadFiles()
        {
            // Create "Files" folder
            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "Files");
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            // Empty the folder just in case there are files
            emptyFiles(filePath);

            // Get the files from DropZone
            var files = HttpContext.Request.Form.Files;

            // Copy the files to wwwroot folder
            foreach (var file in files)
            {
                var fileName = file.FileName;
                using (var fileStreams = new FileStream(Path.Combine(filePath, fileName), FileMode.Create))
                {
                    file.CopyTo(fileStreams);
                }
            }
            return RedirectToAction("Index");
        }

        public IActionResult DownloadZip(IFormFile filedata)
        {
            // set zip output file path and zip folder name
            string zipOutputPath = Path.Combine(_webHostEnvironment.WebRootPath, @"result.zip");

            // give the files path 
            string filesPath = Path.Combine(_webHostEnvironment.WebRootPath, @"Files");

            // if zip folder already exists, delete the zip folder
            if (System.IO.File.Exists(zipOutputPath))
            {
                System.IO.File.Delete(zipOutputPath);
            }

            // Create Zip Folder
            ZipFile.CreateFromDirectory(filesPath, zipOutputPath);

            // Empty the files in the Files folder
            emptyFiles(filesPath);

            // Read the Zip Folder into bytes
            byte[] zipFolder = System.IO.File.ReadAllBytes(zipOutputPath);
           
            // Convert to FileContentResult
            var zipResult = new FileContentResult(zipFolder.ToArray(), "application/zip")
            {
                FileDownloadName = $"ZipFiles-{DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss")}.zip"
            };

            // Delete the Zip folder
            System.IO.File.Delete(zipOutputPath);

            return zipResult;
        }

        public IActionResult Clear()
        {
            string filesPath = Path.Combine(_webHostEnvironment.WebRootPath, @"Files");
            emptyFiles(filesPath);
            return RedirectToAction("Index");
        }

        private void emptyFiles(string FolderName)
        {
            DirectoryInfo dir = new DirectoryInfo(FolderName);

            foreach (FileInfo file in dir.GetFiles())
            {
                file.Delete();
            }
        }
    }
}

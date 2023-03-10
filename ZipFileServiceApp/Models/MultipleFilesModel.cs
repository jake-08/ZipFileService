namespace ZipFileServiceApp.Models;

public class MultipleFilesModel
{
    public List<FormFileModel> Files { get; set; } = new List<FormFileModel>();
}

public class FormFileModel
{
    public byte[] Data { get; set; }
    public IFormFile File { get; set; }
}


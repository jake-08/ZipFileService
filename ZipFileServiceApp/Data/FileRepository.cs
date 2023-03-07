using ZipFileServiceApp.Models;

namespace ZipFileServiceApp.Data
{
    public class FileRepository
    {
        private static MultipleFilesModel model;

        static FileRepository() { model = new MultipleFilesModel(); }

        public static void AddFiles(FormFileModel file)
        {
            model.Files.Add(file);
        }

        public static MultipleFilesModel GetModel()
        {
            return model;
        }

        public static void ResetModel()
        {
            model = new MultipleFilesModel();
        }
    }
}

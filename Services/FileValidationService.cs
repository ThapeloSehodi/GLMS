namespace GLMS.Services
{
    public class FileValidationService
    {
        public void Validation(string fileName)
        {
           var extension = Path.GetExtension(fileName);
            if (extension != ".pdf")
            {
                throw new Exception("only pdf files are allowed");
            }
        }
    }
}

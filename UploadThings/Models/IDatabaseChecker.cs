namespace UploadThings.Models
{
    public interface IDatabaseChecker
    {
        Exception CheckAndCreateDatabase();
    }

}

namespace Hua.DDNS.Common
{
    public class FileHelper
    {
        public static void DeleteIfExists(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}

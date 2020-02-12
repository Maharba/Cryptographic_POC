using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Cryptography_POC.Helpers
{
    public static class EmbeddedFileHelper
    {
        /*public static async Task<byte[]> GetResourceBytes(string resourceName)
        {
            byte[] results = null; 
            Stream stream = null;
            MemoryStream mr = new MemoryStream(); 

            var assembly = typeof(CacheService).GetTypeInfo().Assembly;

            var resourcePath = $"EY.Mobile.Speed.CoreLib.Assets.{resourceName}";
            try
            {
                stream = assembly.GetManifestResourceStream(resourcePath);
                if (stream != null && stream.Length > 0)
                {
                    await stream.CopyToAsync(mr);
                    results = mr.ToArray();
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                mr?.Dispose();
                mr = null;

                stream?.Dispose();
                stream = null;
            }
            return results;
        }*/
        
        public static string GetFileNameByUrl(string url)
        {
            try
            {
                var splitUrl = url.Split('/');
                var filename = splitUrl[splitUrl.Length - 1];
                do
                {
                    filename = filename.Replace("%20", "_");
                } while (filename.Contains("%20"));

                filename = filename.Replace("%2C", "_");
                filename = filename.Replace("%26", "");
                filename = filename.Replace("%C3%B6", "_");
                return filename;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return string.Empty;
        }

        public static string GetFilePathByItem(string prefix, string url)
        {
            try
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), $"{prefix}_{GetFileNameByUrl(url)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return string.Empty;
        }
        
        public static string GetFilePathByItemForFOP(string prefix)
        {
            try
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), $"{prefix}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return string.Empty;
        }

        public static string GetFilePath(string filename)
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), filename);
        }
    }
}
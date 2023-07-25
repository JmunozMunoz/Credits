using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace Sc.Credits.Helpers.ObjectsUtils
{
    /// <summary>
    /// Zip manager helper
    /// </summary>
    public static class ZipManagerHelper
    {
        /// <summary>
        /// Process zip stream
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static IEnumerable<byte[]> ProcessZipStream(Stream file)
        {
            List<byte[]> docsList = new List<byte[]>();

            using (file)
            {
                var zipArchive = new ZipArchive(file, ZipArchiveMode.Read, false);
                foreach (ZipArchiveEntry doc in zipArchive.Entries)
                {
                    ZipArchiveEntry document = zipArchive.GetEntry(doc.FullName);
                    Stream documentStream = document.Open();
                    docsList.Add(GetBytesStream(documentStream));
                }
                return docsList;
            }
        }

        /// <summary>
        /// Get bytes stream
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static byte[] GetBytesStream(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}
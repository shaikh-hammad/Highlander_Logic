using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;

namespace Highlander_Logic
{
    public static class Logger
    {
        static List<string> logList = new List<string>();

        public static void logEvent(string message)
        {
            logList.Add(message);
        }
        public static async void clearLogs()
        {
            logList.Clear();
            StorageFolder localFolder = KnownFolders.DocumentsLibrary;

            // Create (or open if it exists) a file in the local folder
            StorageFile sampleFile = await localFolder.CreateFileAsync("eventLog.txt", CreationCollisionOption.OpenIfExists);

            // Clear the content of the file by writing an empty string
            await FileIO.WriteTextAsync(sampleFile, string.Empty);
        }
        public static async void writeEvents()
        {
            string logFilePath = Path.Combine("C: \\Users\\Server\\source\\repos\\Highlander\\Highlander", "eventLog.txt");
            // Get the app's local folder
            StorageFolder localFolder = KnownFolders.DocumentsLibrary;

            // Create (or open) a file in the local folder
            StorageFile sampleFile = await localFolder.CreateFileAsync("eventLog.txt", CreationCollisionOption.OpenIfExists);
            await FileIO.WriteTextAsync(sampleFile, string.Empty);
            // Write to the file using a stream
            using (var stream = await sampleFile.OpenStreamForWriteAsync())
            {
                using (var writer = new StreamWriter(stream))
                {
                    foreach (string message in logList)
                    {
                        writer.Write(message + "\n");
                    }

                }
            }
        }
    }
}

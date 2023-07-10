using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace Design
{
    class Disk
    {


        public static string WindowsInstalledDrive()
        {
            string windowsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
            string systemDrive = Path.GetPathRoot(windowsDirectory);

            DriveInfo[] drives = DriveInfo.GetDrives();

            DriveInfo windowsDrive = drives.FirstOrDefault(d => d.RootDirectory.FullName.Equals(systemDrive, StringComparison.OrdinalIgnoreCase));

            if (windowsDrive != null)
            {
                //Debug.WriteLine("Windows is installed on drive: " + windowsDrive.Name);
                //Debug.WriteLine("Drive type: " + GetDriveType(windowsDrive));
                return /*"Windows is installed on drive: " + windowsDrive.Name + " | " + "Drive type: " + */GetDriveType(windowsDrive);
            }
            else
            {
                return "Fail!!";
            }
        }

        private static string GetDriveType(DriveInfo drive)
        {
            switch (drive.DriveType)
            {
                case DriveType.Fixed:
                    return IsSSD(drive) ? "HDD" : "SSD";
                case DriveType.Removable:
                    return "Removable";
                case DriveType.Network:
                    return "Network";
                case DriveType.CDRom:
                    return "CD-ROM";
                case DriveType.Ram:
                    return "RAM Disk";
                default:
                    return "Unknown";
            }
        }

        static bool IsSSD(DriveInfo drive)
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Model FROM Win32_DiskDrive WHERE DeviceID = '" + drive.Name.Replace("\\", "\\\\") + "'");
            ManagementObjectCollection drives = searcher.Get();
            string model = drives.OfType<ManagementObject>().FirstOrDefault()?["Model"]?.ToString();

            // Check if the drive model indicates it's an SSD
            return model?.IndexOf("SSD", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static string GetReadWriteSpeed()
        {
            string testFolder = "DiskTest";

            // Create a test folder if it doesn't exist
            if (!Directory.Exists(testFolder))
            {
                Directory.CreateDirectory(testFolder);
            }

            int fileSizeMin = 1; // Minimum file size in MB
            int fileSizeMax = 100; // Maximum file size in MB
            int numFiles = 10; // Number of files to generate

            // Generate random files and measure the disk response time
            double totalResponseTime = 0;
            for (int i = 0; i < numFiles; i++)
            {
                // Generate a random file size between fileSizeMin and fileSizeMax
                int fileSize = new Random().Next(fileSizeMin, fileSizeMax + 1);

                string filePath = Path.Combine(testFolder, $"File{i}.bin");

                // Write the random file
                Stopwatch writeTimer = Stopwatch.StartNew();
                GenerateRandomFile(filePath, fileSize);
                writeTimer.Stop();

                // Read the file to measure the disk response time
                Stopwatch readTimer = Stopwatch.StartNew();
                ReadFile(filePath);
                readTimer.Stop();

                totalResponseTime += writeTimer.ElapsedMilliseconds + readTimer.ElapsedMilliseconds;

                // Delete the file
                File.Delete(filePath);
            }

            // Calculate the average disk response time in MB/s
            double averageResponseTime = totalResponseTime / (2 * numFiles); // Multiply by 2 for write and read operations
            double averageSpeed = (numFiles * fileSizeMax) / (averageResponseTime / 1000); // Divide by 1000 to convert milliseconds to seconds

            Debug.WriteLine($"Average hard disk response time: {averageSpeed} MB/s");

            // Delete the test folder
            Directory.Delete(testFolder);
            return $"Average hard disk response time: {averageSpeed} MB/s";
        }

        static void GenerateRandomFile(string filePath, int fileSizeInMB)
        {
            const int bufferSize = 1024 * 1024; // 1 MB buffer

            using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                byte[] buffer = new byte[bufferSize];
                Random random = new Random();

                for (int i = 0; i < fileSizeInMB; i++)
                {
                    random.NextBytes(buffer);
                    fileStream.Write(buffer, 0, bufferSize);
                }
            }
        }

        static void ReadFile(string filePath)
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = new byte[1024];
                while (fileStream.Read(buffer, 0, buffer.Length) > 0)
                {
                    // Perform any operation with the read data (optional)
                }
            }
        }


        public static void Speed()
        {
            // Rastgele dosya adı oluşturulması
            string fileName = Path.GetRandomFileName();
            string filePath = Path.Combine(Path.GetTempPath(), fileName);

            // Dosya boyutu
            long fileSize = GenerateRandomFileSize(800, 1000); // MB olarak

            // Dosya oluşturulması
            Debug.WriteLine($"Rastgele oluşturulan dosya: {filePath}");
            Debug.WriteLine($"SIZE: {fileSize / (1024)}");
            CreateRandomFile(filePath, fileSize);

            // Dosyanın okunması
            double readSpeed = MeasureDiskReadSpeed(filePath);
            Debug.WriteLine($"Ortalama harddisk tepki süresi: {readSpeed} MB/sn");

            // Dosyanın silinmesi
            File.Delete(filePath);

            Debug.WriteLine("Program sonlandı. Dosya silindi.");
            
        }

        static long GenerateRandomFileSize(int minMB, int maxMB)
        {
            Random random = new Random();
            return random.Next(minMB, maxMB + 1) * 1024 * 1024; // MB to byte
        }

        static void CreateRandomFile(string filePath, long fileSize)
        {
            using (FileStream stream = File.OpenWrite(filePath))
            {
                byte[] buffer = new byte[1024];
                Random random = new Random();

                long bytesWritten = 0;
                while (bytesWritten < fileSize)
                {
                    random.NextBytes(buffer);
                    int bytesToWrite = (int)Math.Min(buffer.Length, fileSize - bytesWritten);
                    stream.Write(buffer, 0, bytesToWrite);
                    bytesWritten += bytesToWrite;
                }
            }
        }

        static double MeasureDiskReadSpeed(string filePath)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            using (FileStream stream = File.OpenRead(filePath))
            {
                byte[] buffer = new byte[1024];
                int bytesRead = 0;

                while (stream.Read(buffer, 0, buffer.Length) > 0)
                {
                    bytesRead += buffer.Length;
                }
            }

            stopwatch.Stop();
            long fileSize = new FileInfo(filePath).Length;
            double speedMBPerSec = fileSize / (stopwatch.Elapsed.TotalSeconds * 1024 * 1024);
            return speedMBPerSec;
        }

        public static string MeasureDiskSpeed(string filePath, int fileSizeInMB)
        {
            // Generate a sample file with the specified size
            GenerateSampleFile(filePath, fileSizeInMB);

            // Measure disk read speed
            Stopwatch readTimer = new Stopwatch();
            using (FileStream fileStream = File.OpenRead(filePath))
            {
                byte[] buffer = new byte[1024];
                readTimer.Start();
                while (fileStream.Read(buffer, 0, buffer.Length) > 0)
                {
                    // Read file content without performing any operation
                }
                readTimer.Stop();
            }

            // Measure disk write speed
            Stopwatch writeTimer = new Stopwatch();
            using (FileStream fileStream = File.OpenWrite(filePath))
            {
                byte[] buffer = new byte[1024];
                writeTimer.Start();
                for (int i = 0; i < fileSizeInMB * 1024; i++)
                {
                    fileStream.Write(buffer, 0, buffer.Length);
                }
                writeTimer.Stop();
            }

            // Display results
            string s = (2 * fileSizeInMB / readTimer.Elapsed.TotalSeconds).ToString("0.00") + "|" + (2 * fileSizeInMB / writeTimer.Elapsed.TotalSeconds).ToString("0.00"); 
            Debug.WriteLine("Disk Read Speed: {0} MB/s"+ (2*fileSizeInMB / readTimer.Elapsed.TotalSeconds).ToString("0.00"));
            Debug.WriteLine("Disk Write Speed: {0} MB/s"+ (2*fileSizeInMB / writeTimer.Elapsed.TotalSeconds).ToString("0.00"));
            File.Delete(filePath);
            return s;
        }

        private static void GenerateSampleFile(string filePath, int fileSizeInMB)
        {
            byte[] buffer = new byte[1024];
            using (FileStream fileStream = File.Create(filePath))
            {
                for (int i = 0; i < fileSizeInMB * 1024; i++)
                {
                    fileStream.Write(buffer, 0, buffer.Length);
                }
            }
        }


    }
}

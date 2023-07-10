using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace Design
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string OK = "This Computer is Ok for Ihsan Hızlı Teklif Program\n";
        const string RAM_PROBLEM = "You have X gb ram. please upgrade it to at least 8 GB\n"; // 
        const string CPU_POBLEM = "Your CPU's benchmark score is  X1. It Should be at least Y1\n"; // 
        const string NETWORK_POBLEM = "Your internet connection is relatively slow, please check it to use program more effectively\n"; // 
        const int minRam = 8;
        const int minCPUBenchMark = 4000;
        const int minDonwloadSpeed = 16;
        const string acenteName = "SIGORTA ACENTESININ ADI";
        public static List<string> sigortaWebSites = new List<string>();
        public static List<int> threads = new List<int>();
        public static int threadNum = 0;
       
        public MainWindow()
        {
            InitializeComponent();
            EvaluateTheResult(); //Ana işi yapan fonksiyon
            SigortaSirketleri sg = new SigortaSirketleri(this);
            DataContext = new MainWindowViewModel();
            Task.Run(() => InternetSpeedFromPowerShell());
        }

        void InternetSpeedFromPowerShell()
        {
            string currentDirectory = Environment.CurrentDirectory;

            // PowerShell command to execute
            string powerShellCommand = $"cd {currentDirectory}; .\\speedtest.exe";

            // Create a new process start info
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-Command \"{powerShellCommand}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            // Create a new process
            Process process = new Process
            {
                StartInfo = processStartInfo
            };

            // Start the process
            process.Start();

            // Read the output and error streams
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            // Wait for the process to exit
            process.WaitForExit();

            // Display the output and error
            Debug.WriteLine("Output:");
            Debug.WriteLine(output);
            Debug.WriteLine("Error:");
            Debug.WriteLine(error);
            //Degbug.ReadLine();

            // Extract upload and download speeds using regular expressions
            string downloadSpeedPattern = @"Download:\s+([\d.]+)\s+Mbps";
            string uploadSpeedPattern = @"Upload:\s+([\d.]+)\s+Mbps";

            Match downloadMatch = Regex.Match(output, downloadSpeedPattern);
            Match uploadMatch = Regex.Match(output, uploadSpeedPattern);

            if (downloadMatch.Success && uploadMatch.Success)
            {
                string downloadSpeed = downloadMatch.Groups[1].Value;
                string uploadSpeed = uploadMatch.Groups[1].Value;

                Debug.WriteLine("Download Speed: " + downloadSpeed + " Mbps");
                Debug.WriteLine("Upload Speed: " + uploadSpeed + " Mbps");
                Application.Current.Dispatcher.Invoke(() =>
                {
                    networkDownloadSpeed.Text = downloadSpeed + " Mbps";
                    networkUploadSpeed.Text = uploadSpeed+ " Mbps";
                });
            }
        }


        public async Task Initialize()
        {
            SirketSiteleri();
            setRamAmount();
            setRamSlot();
            setCPUName();
            setOsDiskType();
            setDiskWriteSpeed();
            setDiskReadSpeed();
            await setCPUBenchMarkScore();
        }
        public void CreateReport()
        {
            // Create an instance of the PdfCreator class
            Report pdfCreator = new Report();
            //while (true)
            //{
            //    if (cpuBenchMarkScore.Text != "Calculating..." && networkDownloadSpeed.Text != "Calculating..." && networkUploadSpeed.Text!= "Calculating...") break;
            //}
            // Provide the file path and the text to be written in the PDF file
            string[] arr = Environment.CurrentDirectory.Split('\\');
            string filePath = arr[0]+ '\\' + arr[1]+ '\\' + arr[2]+ '\\' + arr[3]+"\\output.pdf";
            string text = acenteName + "\n\n";
            text += "CPU: " + cpuName.Text + "\n";
            text += "CPU Benchmark: " + cpuBenchMarkScore.Text + "\n";
            text += "\n";
            text += "Ram: " + ramAmount.Text + "\n";
            text += "Ram Total Slot Number: " + totalSlotNum.Text + "\n";
            text += "Ram Empty Slot Number: " + emptySlotNum.Text + "\n";
            text += "\n";
            text += "Disk Read Speed (MB/S): " + diskReadSpeed.Text + "\n";
            text += "Disk Write Speed (MB/S): " + diskWriteSpeed.Text + "\n";
            text += "\n";
            text += "Network Download Speed: " + networkDownloadSpeed.Text + "\n";
            text += "Network Upload Speed: " + networkUploadSpeed.Text + "\n";
            text += "\n";
            text += yorum.Text + "\n";
            text += "\n";
            text += "\n";
            text += "Insurance companies websites response times" +  "\n";
            //while (true)
            //{
            //    if (sigortaWebSites.Count >= threadNum) 
            //        break;
            //}
            foreach (string str in sigortaWebSites)
            {
                text += str;
            }


            // Create the PDF file
            pdfCreator.CreatePdf(filePath, text);

            Debug.WriteLine("PDF file created successfully.");
        }
        private async void SirketSiteleri()
        {
            await SigortaSirketleri.TepkiSureleri();
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            // Close the window
            Close();
        }
        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private async void EvaluateTheResult()
        {
            await Initialize();

            bool ok = true;
            yorum.Text = "";
            //cpuBenchMarkScore.Text = "4000";
            //cpu benchmark 
            while (true) if (cpuBenchMarkScore.Text != "Calculating...") break;
            if (cpuBenchMarkScore.Text!="" && Convert.ToInt32(cpuBenchMarkScore.Text) < minCPUBenchMark)
            {
                string temp = CPU_POBLEM.Replace("X1", cpuBenchMarkScore.Text);
                yorum.Text = temp.Replace("Y1", minCPUBenchMark.ToString());
                ok = false;
            }
            //ramAmount.Text = "4";
            if (Convert.ToInt32(ramAmount.Text) < minRam)
            {
                yorum.Text += RAM_PROBLEM.Replace("X", ramAmount.Text);
                ok = false;
            }
            //if (Convert.ToDouble(networkDownloadSpeed.Text) < minDonwloadSpeed)
            //{
            //    yorum.Text += NETWORK_POBLEM;
            //    ok = false;
            //}
            if (ok) yorum.Text = OK;    // no problem
        }
        //public static void temp()
        //{
        //    setReportInfoText(this);
        //}
        //public void setReportInfoText()
        //{
        //    reportInfo.Text = string.Empty; 
        //}
        public void setNetworkUploadSpeed(string speed)
        {
            networkUploadSpeed.Text = speed; //"Network.UploadSpeed();";
        }
        public void setNetworkDownloadSpeed(string speed)
        {
            networkDownloadSpeed.Text = speed;//"Network.DownloadSpeed();"; 

        }
        public void setRamAmount()
        {
            ramAmount.Text = Ram.GetMemory();

        }
        public void setRamSlot()
        {
            string[] arr = Ram.GetSlotInfo().Split('|');
            totalSlotNum.Text = arr[0];
            emptySlotNum.Text = arr[1];

        }
        public void setCPUName()
        {
            cpuName.Text = CPU.GetCpuName();
        }
        public async Task setCPUBenchMarkScore()
        {
            string s = await CPU.GetCpuRank(CPU.GetCpuName());
            cpuBenchMarkScore.Text = s.Split('|')[0];
        }
        public void setDiskWriteSpeed()
        {
            string s = Disk.MeasureDiskSpeed(Environment.CurrentDirectory+"\\testfile.bin", 1000);
            diskWriteSpeed.Text = s.Split("|")[1] + " Mb/s";
            //Disk.Speed();
        }
        public void setOsDiskType()
        {
           OsDiskType.Text = Disk.WindowsInstalledDrive();
        }
        public void setDiskReadSpeed()
        {
            string s = Disk.MeasureDiskSpeed(Environment.CurrentDirectory + "\\testfile.bin", 1000);
            diskReadSpeed.Text = s.Split("|")[0] + " Mb/s";
        }


    }
}

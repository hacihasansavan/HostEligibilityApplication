using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Design
{
    class CPU
    {
        public static void writeSomething()
        {
            Console.WriteLine("BABA BURDA");
        }
        private static RegistryKey GetRegister
        {
            get
            {
                //RegistryKey regVal = Registry.CurrentUser.CreateSubKey("Software");
                //regVal = regVal.CreateSubKey("Microsoft");
                //regVal = regVal.CreateSubKey("RegistryOpenSubKeyExample");
                //string AltFolder = "Genel Ayarlar";
                //regVal = regVal.CreateSubKey(AltFolder);
                RegistryKey regVal = Registry.CurrentUser.CreateSubKey($"Software\\Microsoft\\RegistryOpenSubKeyExample\\BurayaKoyacak");
                return regVal;
            }
        }
        public static string GetCpuName(bool getRegister = true)
        {
            string CPU_Name = string.Empty;
            if (getRegister)
            {
                CPU_Name = GetRegister.GetValue("CPU_Name", "").ToString();

            }
            CPU_Name = string.Empty;
            //CPU_Name="Intel()"
            if (CPU_Name == string.Empty)
            {
                try
                {
                    ManagementObjectSearcher MOS = new ManagementObjectSearcher("SELECT Name FROM Win32_Processor");


                    foreach (ManagementObject MO in MOS.Get())
                    {
                        CPU_Name = MO["Name"].ToString();
                    }
                    if (CPU_Name != string.Empty)
                    {
                        GetRegister.SetValue("CPU_Name", CPU_Name);
                    }
                }
                catch (Exception ex)
                {

                }
            }
            //Console.WriteLine("Win32_Processor name alma süresi :" + sw.ElapsedMilliseconds + " " + sw.ElapsedTicks);
            return CPU_Name;
        }


        public static async Task<string> GetCpuRank(string cpuName)
        {
            string sonuc = string.Empty;
            string htmlData = string.Empty;
            string link = string.Empty;
            try
            {
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

                cpuName = cpuName.Replace(" ", "+").Replace("@", "%40");
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://www.google.com.tr/search?q=" + cpuName + "+site:www.cpubenchmark.net%2Fcpu.php");
                request.Method = "GET";
                using (var response = await request.GetResponseAsync())
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    htmlData = reader.ReadToEnd();
                    if (htmlData.IndexOf("q=http://www.cpubenchmark.net/cpu.php%3Fcpu%3D") != -1 || htmlData.IndexOf("q=https://www.cpubenchmark.net/cpu.php%3Fcpu%3D") != -1)
                    {
                        int yer1 = htmlData.IndexOf("q=http://www.cpubenchmark.net/cpu.php%3Fcpu%3D");
                        if (yer1 == -1)
                        {
                            yer1 = htmlData.IndexOf("q=https://www.cpubenchmark.net/cpu.php%3Fcpu%3D");
                        }
                        int yer2 = htmlData.IndexOf("&amp;sa=", yer1);
                        link = htmlData.Substring(yer1 + 2, yer2 - yer1 - 2);
                        link = link.Replace("%3F", "?").Replace("%3D", "=").Replace("%2B", "+").Replace("%25", "%").Replace("%26", "&");
                    }
                }
                if (link != string.Empty)
                {
                    request = (HttpWebRequest)WebRequest.Create(link);
                    request.Method = "GET";
                    using (var response = await request.GetResponseAsync())
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        htmlData = reader.ReadToEnd();
                        if (htmlData.IndexOf("Single Thread Rating") != -1)
                        {
                            int yer1 = htmlData.IndexOf("Single Thread Rating");
                            yer1 = htmlData.LastIndexOf("\">", yer1);
                            int yer2 = htmlData.IndexOf("</span>", yer1);
                            string rate = htmlData.Substring(yer1 + 2, yer2 - yer1 - 2);
                            //if (rate.IsNumeric() && Convert.ToInt32(rate) >= 10000) rate += " Canavar";
                            yer1 = htmlData.IndexOf("CPU First Seen on Charts:");
                            yer1 = htmlData.IndexOf(">", yer1) + 1;
                            yer2 = htmlData.IndexOf("<", yer1);
                            string releaseDate = htmlData.Substring(yer1, yer2 - yer1).Replace("&nbsp;", "").Trim();
                            sonuc = rate + "|" + releaseDate + "|" + link;
                        }
                    }
                }
            }
            catch (WebException webex)
            {
                Console.WriteLine("ERROR1!!: ", webex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR2!!: ", ex.Message);
            }
            return sonuc;
        }






    }

}

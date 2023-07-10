using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.IO;
namespace Design
{
    class SigortaSirketleri
    {
        //private static Microsoft.Web.WebView2.WinForms.WebView2 EdgeWebView;

        private static string[] sigortaArray = new string[] {
            "https://sat2.aksigorta.com.tr/auth/login", //AkSigorta
            "https://atis.acnturk.com.tr/WebInsurance.Live/Account/Login",  //AcnTurk
            "https://cas.allianz.com.tr/cas/login",    //Allianz
            "https://giris.anadolusigorta.com.tr/singleSignOnApi/login", // Anadolu
            "https://auth.anasigorta.com.tr/Account/Login",
            "https://online.ankarasigorta.com.tr/",
            "https://expertus.arexsigorta.com.tr/WebInsurance.Live/Account/Login",
            "https://acente.atlasmutuel.com.tr/Login.aspx",
            "https://auth.aveonglobalsigorta.com/Account/Login",
            "https://axatek.axasigorta.com.tr/login",
            "https://nareks.bereket.com.tr/Login.aspx",
            "https://sigorta.corpussigorta.com.tr/Login.aspx",
            "https://adaauth.dogasigorta.com/Account/Login",
            "https://pathica.ethicasigorta.com/Login.aspx?ReturnUrl=%2fMain.aspx",
            "https://ap.eurekosigorta.com.tr",
            "https://eacentesatis.hdifibaemeklilik.com.tr/#/auth/login",
            "https://portal.generali.com.tr/sign-in",
            "https://auth.grisigorta.com.tr/Account/Login",
            "https://anka.groupama.com.tr/Login.aspx",
            "https://portal.gulfsigorta.com.tr/Account/Login",
            "https://acente.hepiyi.com.tr/HiAccount/Login",
            "https://auth.korusigortaportal.com/Account/Login",
            "https://portal.magdeburger.com.tr/",
            "https://map.mapfre.com.tr",
            "https://sigorta.neova.com.tr:5443/NonLife/Policy/ViewPolicy.aspx",
            "https://neoport.neova.com.tr/_layouts/15/Neova.Authentication/CustomLogin.aspx",
            "https://sistem.orientsigorta.com.tr/Login.aspx",
            "https://quicksigorta.com/",
            "https://rayexpress.raysigorta.com.tr/",
            "https://galaksi.turknippon.com"

        };


        private static MainWindow mw;

        private static async Task SiteSuresi(string url, int i)
        {

            using (HttpClient client = new HttpClient())
            {
                Stopwatch stopwatch = new Stopwatch();
                try
                {
                    //Console.WriteLine($"Opening {i}. website: {url}...");

                    stopwatch.Start();

                    HttpResponseMessage response = await client.GetAsync(url);

                    stopwatch.Stop();

                    if (response.IsSuccessStatusCode)
                    {
                        Debug.WriteLine($"{i}.Website {url} opened successfully in {stopwatch.ElapsedMilliseconds} milliseconds.");
                        MainWindow.sigortaWebSites.Add($"Website {url} opened successfully in {stopwatch.ElapsedMilliseconds} milliseconds.\n");
                    }
                    else
                    {
                        Debug.WriteLine($"Failed to open website. Probably the site is down. Status code: {response.StatusCode}");
                        MainWindow.sigortaWebSites.Add($"Failed to open website {url}. Probably the site is down.\n");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"An error occurred while opening the website:({i}) {url}  -   {ex.Message}");
                    return;
                }
                //Console.WriteLine($"All websites are oppened!! - {i}");
            }

        }

        public SigortaSirketleri(MainWindow mwi)
        {
            mw = mwi;
        }


        //sigorta şirketlerinin sayfalarının yükleneme süreleri ölçülecek
        // thread fonksiyonlarına sigorta şirketlerinin login sayfalarının linkeri gönderilecek
        public static async Task TepkiSureleri()
        {

            Task[] threadTasks = new Task[sigortaArray.Length];
            Debug.WriteLine($"Toplam - {sigortaArray.Length} websitesi var");
            MainWindow.threadNum = sigortaArray.Length;
            for (int i = 0; i < sigortaArray.Length; i++)
            {
                threadTasks[i] = ThreadFunctionAsync(sigortaArray[i], i);
            }

            await Task.WhenAll(threadTasks);
            MainWindow temp = new MainWindow();
            
            temp.CreateReport();

            //mw.reportInfo.Text = "Report Created!!";

            Debug.WriteLine("Report Created (changed)!!");
            Debug.WriteLine("All threads completed.");

            // Keep the console application running to see the output
            //Console.ReadLine();

        }

        private static async Task ThreadFunctionAsync(string parameter, int i)
        {
            //Console.WriteLine($"Thread function called with parameter: {parameter}");

            // Simulate some asynchronous work being done by the thread
            await SiteSuresi(parameter, i);
            //MainWindow.threads.Add(1);
            //await Task.Delay(2000);

            //Console.WriteLine($"Thread function completed for parameter: {parameter}");
        }
    }
}
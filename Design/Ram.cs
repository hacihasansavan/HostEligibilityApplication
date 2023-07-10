using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace Design
{
    class Ram
    {
        public static string GetMemory()
        {
            string ramal = "ram";
            ManagementObjectSearcher Search = new ManagementObjectSearcher("Select * From Win32_ComputerSystem");
            foreach (ManagementObject Mobject in Search.Get())
            {
                double Ram_Bytes = (Convert.ToDouble(Mobject["TotalPhysicalMemory"]));
                double ramgb = Ram_Bytes / 1073741824;
                double islem = Math.Ceiling(ramgb);
                //string mesaj = " GB";
                ramal = (islem.ToString()/* + mesaj*/);
            }
            return ramal;
        }

        public static string GetSlotInfo()
        {
            string result = "ERROR";
            try
            {
                // Create a new management scope
                ManagementScope scope = new ManagementScope("\\\\.\\ROOT\\CIMV2");

                // Retrieve the memory slots
                ObjectQuery slotQuery = new ObjectQuery("SELECT * FROM Win32_PhysicalMemory");
                ManagementObjectSearcher slotSearcher = new ManagementObjectSearcher(scope, slotQuery);
                ManagementObjectCollection slotResults = slotSearcher.Get();
                int totalSlots = slotResults.Count;

                // Retrieve the number of populated memory slots
                int populatedSlots = 0;
                foreach (ManagementObject slotObj in slotResults)
                {
                    ulong capacityBytes = (ulong)slotObj["Capacity"];
                    if (capacityBytes > 0)
                    {
                        populatedSlots++;
                    }
                }

                // Calculate the number of empty RAM slots
                int emptySlots = totalSlots - populatedSlots;

                // Display the result
                //Console.WriteLine("Total Memory Slots: " + totalSlots);
                //Console.WriteLine("Populated Slots: " + populatedSlots);
                //Console.WriteLine("Empty Slots: " + emptySlots);

                result =  totalSlots.ToString();
                //result += "\nPopulated Slots: " + populatedSlots;
                result += "|" + emptySlots.ToString();
            }
            catch (ManagementException ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
            return result;
        }
    }
}

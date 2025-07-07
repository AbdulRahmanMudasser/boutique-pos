using System;
using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace MaqboolFashionPOS
{
    // Ensures the app is bound to a single PC using motherboard serial number
    public class DeviceBinding
    {
        // Generates a unique machine ID based on motherboard serial number
        public static string GetMachineId()
        {
            try
            {
                string motherboardId = "";
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        motherboardId = obj["SerialNumber"].ToString();
                        break;
                    }
                }

                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(motherboardId);
                    byte[] hash = sha256.ComputeHash(bytes);
                    return Convert.ToBase64String(hash);
                }
            }
            catch
            {
                throw new Exception("Failed to generate machine ID.");
            }
        }

        // Verifies if the current machine matches the stored device ID
        public static bool IsValidDevice(string storedDeviceId)
        {
            return GetMachineId() == storedDeviceId;
        }
    }
}
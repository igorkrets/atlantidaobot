using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Windows.Forms;

namespace Atlatntida_o_Bot.Libs
{
    class Hard
    {
        public static string GetHardSerial()
        {
            string HoldingAdress = "";
            try
            {
                string drive = Environment.GetFolderPath(Environment.SpecialFolder.System).Substring(0, 1);
                ManagementObject disk = new ManagementObject("win32_logicaldisk.deviceid=\"" + drive + ":\"");
                disk.Get();
                string diskLetter = (disk["VolumeSerialNumber"].ToString());
                HoldingAdress = diskLetter;

            }
            catch (Exception)
            {
                MessageBox.Show("Critical error, application automatically exit", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            return HoldingAdress;
        }

    }
}

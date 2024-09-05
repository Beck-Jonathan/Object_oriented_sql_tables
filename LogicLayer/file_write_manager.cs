using Data_Access;
using Data_Access_Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer
{
    public class file_write_manager : iFile_write_manager
    {
        iFile_Write writer;
        public static string SeparatePath;
        public static string SettingsPath;
        public static string SettingsPath2;
        public file_write_manager(iFile_Write writer)
        {
            this.writer = writer;
            SeparatePath = writer.getSeparatePath();
            SettingsPath = writer.getSettingsPath();
            SettingsPath2 = writer.getSettingsPath2();
        }
        public file_write_manager()
        {
            this.writer = new file_write();
            SeparatePath = writer.getSeparatePath();
            SettingsPath = writer.getSettingsPath();
            SettingsPath2 = writer.getSettingsPath2();
        }

        public void startUp(DirectoryInfo directoryInfo) {
            try
            {
                writer.startUp(directoryInfo);
            }
            catch (Exception ex)
            {

                throw new IOException("unable to write file"+ex.Message);
            }
        
            return;
        }

        public void fileWrite(string output, string table, string type, string method) {
            try
            {
                writer.fileWrite(output, table, type, method);
            }
            catch (Exception ex)
            {

                throw new IOException("unable to write file" + ex.Message);
            }
        
            return;
        }
    }
}

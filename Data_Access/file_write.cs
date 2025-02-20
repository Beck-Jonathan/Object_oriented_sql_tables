using Data_Access_Interfaces;
using System;
using System.IO;
namespace Data_Access
{
    public class file_write : iFile_Write
    {
        //the output file is in the same folder as the input folder, just with "sql" appended to the file name
        //static string newPath = "C:\\Users\\jjbec\\Desktop\\Letter_B\\Table_Gen\\output_File\\outputsqlSPs.txt";
        //static string codePath = "C:\\Users\\jjbec\\Desktop\\Letter_B\\Table_Gen\\output_File\\outputcsharp.txt";
        //static string XAMLPath = "C:\\Users\\jjbec\\Desktop\\Letter_B\\Table_Gen\\output_File\\outputXAML.txt";
        //public static string SettingsPath = "C:\\Users\\jjbec\\Desktop\\Letter_B\\Table_Gen\\output_File\\settings.txt";
        //public static string SettingsPath2 = "C:\\Users\\jjbec\\Desktop\\Letter_B\\Table_Gen\\output_File\\settings2.txt";
        //static string JavaPath = "C:\\Users\\jjbec\\Desktop\\Letter_B\\Table_Gen\\output_File\\outputJava.txt";
        //public static string FilesPath = "C:\\Users\\jjbec\\Desktop\\Letter_B\\Table_Gen\\sql_files\\";
        //static string jspPath = "C:\\Users\\jjbec\\Desktop\\Letter_B\\Table_Gen\\output_File\\outputJSP.txt";
        //static string servletPath = "C:\\Users\\jjbec\\Desktop\\Letter_B\\Table_Gen\\output_File\\outputservlet.txt";
        //public static StreamWriter WriteBuddy = new StreamWriter(newPath);
        //public static StreamWriter CSharpBuddy = new StreamWriter(codePath);
        //public static StreamWriter XAMLBuddy = new StreamWriter(XAMLPath);
        //public static StreamWriter SettingsBuddy = new StreamWriter(SettingsPath);
        //public static StreamWriter sqlBuddy2 = new StreamWriter(newPath);
        //public static StreamWriter SettingsBuddy = new StreamWriter(SettingsPath2);
        //public static StreamWriter JavaBuddy = new StreamWriter(JavaPath);
        //public static StreamWriter BatchBuddy = new StreamWriter(FilesPath + "Create_DB.bat");
        //public static StreamWriter JSPBuddy = new StreamWriter(jspPath);
        //public static StreamWriter ServletBuddy = new StreamWriter(servletPath);
        public static string SeparatePath = "C:\\Table_Gen\\";
        public void startUp(DirectoryInfo directoryInfo)
        {
            try
            {
                _ = System.IO.Directory.CreateDirectory(SeparatePath);
                foreach (FileInfo file in directoryInfo.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo subfolder in directoryInfo.GetDirectories())
                {
                    if (!subfolder.Name.Equals("temp"))
                    {
                        startUp(subfolder);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new IOException("unable to create folders", ex);
            }
        }
        public void fileWrite(string output, string table, string type, string method)
        {
            try
            {
                table = table.Replace("?", "");
                _ = System.IO.Directory.CreateDirectory(SeparatePath + type + "\\");
                StreamWriter writer = new StreamWriter(SeparatePath + type + "\\" + table + "_" + method + ".txt", true);
                writer.Write(output + "\n");
                writer.Flush();
                writer.Close();
            }
            catch (Exception ex)
            {
                throw new IOException("unable to write file", ex);
            }
        }
        public string getSeparatePath()
        {
            return SeparatePath;
        }
        public string getSettingsPath()
        {
            return "";
        }
        public string getSettingsPath2()
        {
            return "";
        }
    }
}

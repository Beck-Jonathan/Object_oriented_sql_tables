using appData2;
using System.IO;

namespace Data_Access
{
    public static class file_write
    {
        //the output file is in the same folder as the input folder, just with "sql" appended to the file name
        static string newPath = settings.path.Substring(0, settings.path.Length - 4) + "sql.txt";
        public static StreamWriter WriteBuddy = new StreamWriter(newPath);
    }
}

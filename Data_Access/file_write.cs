using appData2;
using System.IO;

namespace Data_Access
{
    public static class file_write
    {
        //the output file is in the same folder as the input folder, just with "sql" appended to the file name
        static string newPath = "C:\\Users\\jjbec\\Desktop\\Letter_B\\Table_Gen\\output_File\\outputsqlSPs.txt";
        static string codePath = "C:\\Users\\jjbec\\Desktop\\Letter_B\\Table_Gen\\output_File\\outputcsharp.txt";
        public static StreamWriter WriteBuddy = new StreamWriter(newPath);
        public static StreamWriter CSharpBuddy = new StreamWriter(codePath);
    }
}

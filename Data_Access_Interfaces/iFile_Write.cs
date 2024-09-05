using System;
using System.IO;

namespace Data_Access_Interfaces
{
    public interface iFile_Write
    {
        String getSeparatePath();
        String getSettingsPath();
        String getSettingsPath2();
        void startUp(DirectoryInfo directoryInfo);
        void fileWrite(string output, string table, string type, string method);
    }
}

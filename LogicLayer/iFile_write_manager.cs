using System.IO;
namespace LogicLayer
{
    public interface iFile_write_manager
    {
        void startUp(DirectoryInfo directoryInfo);
        void fileWrite(string output, string table, string type, string method);
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer
{
    public interface iFile_write_manager
    {
        void startUp(DirectoryInfo directoryInfo);


        void fileWrite(string output, string table, string type, string method);
       
    }
}

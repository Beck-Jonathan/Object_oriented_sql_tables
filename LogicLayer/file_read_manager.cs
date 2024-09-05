using Data_Access;
using Data_Access_Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LogicLayer
{
    
    public class file_read_manager
    {
        iFile_Read reader;
        public file_read_manager(iFile_Read reader)
        {
            this.reader = reader;
        }
        public file_read_manager()
        {
            this.reader = new file_read();
        }

        public void readdata() {
        reader.readdata();
        }
        public void saveLocaiton() {
            reader.saveLocaiton();
        }
        public string readlocaiton() { 
        return reader.readlocaiton();
        }
        public void clearLocation() {
            reader.clearLocation();
        }

    }
}

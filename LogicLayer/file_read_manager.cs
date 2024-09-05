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
    
    public class file_read_manager : iFile_read_manager
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
            try
            {
                reader.readdata();
            }
            catch (Exception ex )
            {

                throw ex;
            }
        
        }
        public void saveLocaiton() {
            try
            {
                reader.saveLocaiton();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }
        public string readlocaiton() {
            try
            {
                return reader.readlocaiton();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        
        }
        public void clearLocation() {
            try
            {
                reader.clearLocation();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }

    }
}

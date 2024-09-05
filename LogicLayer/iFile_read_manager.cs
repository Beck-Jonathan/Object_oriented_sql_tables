using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer
{
    public interface iFile_read_manager
    {
        void readdata();

        void saveLocaiton();

        string readlocaiton();

        void clearLocation();
        
    }
}

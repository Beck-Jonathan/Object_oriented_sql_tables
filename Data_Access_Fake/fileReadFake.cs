using Data_Access_Interfaces;
using System;
using System.Runtime.CompilerServices;
using Data_Objects;
using System.Collections.Generic;
using appData2;

namespace Data_Access_Fake
{
    public class fileReadFake : iFile_Read
    {
        public void clearLocation()
        {
            return;
        }

        public void readdata()
        {   
            //to make the first fake table
            List<Column> u_columns = new List<Column>();
            Column u_column1 = new Column("user_id", "int", 0, "", "",100000 , 1, 'n', "no", 'n', 'y', "no", "","","The user_Id");
            Column u_column2 = new Column("user_name", "nvarchar", 100, "", "", 0,0, 'n', "no", 'y', 'n', "no", "", "", "The user handle");
            Column u_column3 = new Column("user_pass", "nvarchar", 100,"", "", 0, 0, 'n', "no", 'n', 'n', "no", "", "", "The user handle");
            u_columns.Add(u_column1);
            u_columns.Add(u_column2);
            u_columns.Add(u_column3);

            MySqlTable user = new MySqlTable("user", u_columns);
            data_tables.all_tables.Add(user);
            List<Column> t_columns = new List<Column>();
            Column t_column1 = new Column("user_id", "int", 0, "", "", 100000, 1, 'n', "no", 'n', 'y', "no", "", "", "The user_Id");
            Column t_column2 = new Column("user_name", "nvarchar", 100, "", "", 0, 0, 'n', "no", 'y', 'n', "no", "", "", "The user handle");
            Column t_column3 = new Column("user_pass", "nvarchar", 100, "", "", 0, 0, 'n', "no", 'n', 'n', "no", "", "", "The user handle");
            t_columns.Add(u_column1);
            t_columns.Add(u_column2);
            t_columns.Add(u_column3);

            MySqlTable tranactions = new MySqlTable("user", t_columns);
            data_tables.all_tables.Add(tranactions);
            settings.table_count = data_tables.all_tables.Count;

        }

        public string readlocaiton()
        {
            return "";
        }

        public void saveLocaiton()
        {
            return ;
        }
    }
}

using appData2;
using System;

namespace Data_Objects
{
    public class header
    {
        //to create the top few lines of a table
        public String table_name { get; set; }
        public String description { get; set; }
        public string full_header { get; private set; }


        public header(String table_name, String description)
        {
            full_header = "";
        }
        //create the header for a standardad table
        public String full_header_gen()
        {
            full_header = "";
            if (settings.TSQLMode)
            {
                full_header = commentBox.genCommentBox(table_name, Component_Enum.SQL_table) +
                "\n\n" +

                "CREATE TABLE " + table_name + "(\n"
                ;
            }

            else
            {
                full_header = commentBox.genCommentBox(table_name, Component_Enum.SQL_table) +
                "\n\n" +
                "DROP TABLE IF EXISTS " + table_name + ";\n" +
                "CREATE TABLE " + table_name + "(\n"
                ;
            }

            return full_header;
        }
        //creae the header for an audit table
        public string audit_header_gen()
        {
            String audit_header = "";
            audit_header = commentBox.genCommentBox(table_name, Component_Enum.SQL_Audit_Table) +
                "\n\n" +
                "DROP TABLE IF EXISTS " + table_name + "_audit;\n\n" +
                "CREATE TABLE " + table_name + "_audit(\n\n"
                ;


            return audit_header;

        }

    }
}

using System;

namespace Data_Objects
{
    public class header
    {
        public String table_name { get; set; }
        public String description { get; set; }
        public string full_header { get; private set; }


        public header(String table_name, String description)
        {
            full_header = "";
        }
        public String full_header_gen()
        {
            full_header = "";
            full_header = comment_box_gen.comment_box(table_name, 1) +
                "\n\n" +
                "DROP TABLE IF EXISTS " + table_name + ";\n\n" +
                "CREATE TABLE " + table_name + "(\n\n"
                ;


            return full_header;
        }

        public string audit_header_gen()
        {
            String audit_header = "";
            audit_header = comment_box_gen.comment_box(table_name, 2) +
                "\n\n" +
                "DROP TABLE IF EXISTS " + table_name + "_audit;\n\n" +
                "CREATE TABLE " + table_name + "_audit(\n\n"
                ;


            return audit_header;

        }

    }
}

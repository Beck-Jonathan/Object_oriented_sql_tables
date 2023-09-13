using appData2;
using System;

namespace Data_Objects
{
    public static class database
    {
        public static String print_database_header()
        {
            //Databae header to ensure we don't run into conflicts.
            String database_name = settings.database_name;
            String header_text = "DROP DATABASE IF EXISTS " + database_name + ";\n"
                + "CREATE DATABASE " + database_name + ";\n"
                + "USE " + database_name + ";\n";
            return header_text;
        }
    }
}

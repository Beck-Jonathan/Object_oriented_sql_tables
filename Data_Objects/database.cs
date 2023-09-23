using appData2;
using System;

namespace Data_Objects
{
    public static class database
    {
        public static String print_database_header()
        {
            //Databae header to ensure we don't run into conflicts.
            String go = "";
            if (settings.TSQLMode) { go = "\nGO"; }
            String database_name = settings.database_name;
            String header_text = "DROP DATABASE IF EXISTS " + database_name + ";" + go + "\n"
                + "CREATE DATABASE " + database_name + ";" + go + "\n"
                + "USE " + database_name + ";" + go + "\n";
            return header_text;
        }
    }
}

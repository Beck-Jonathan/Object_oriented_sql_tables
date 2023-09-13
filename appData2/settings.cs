using System;
using System.Collections.Generic;
using System.Data;


namespace appData2
{
    public class settings
    {
        public static String path;
        public static String database_name;
        public static int table_count;
        public static List<List<Boolean>> all_options = new List<List<Boolean>>();
        public static List<String> table_names = new List<String>();

        public static void generate_options() {
            for (int i = 0; i < table_count; i++) {
                List<Boolean> options = new List<Boolean>();
                for (int j = 0; j < 3; j++) {
                    options.Add(true);
                }
                for (int k = 3; k < 12; k++) {
                    options.Add(false);
                }
                all_options.Add(options);
            }
        
        }
}

    }
    


using System;
using System.Collections.Generic;
namespace appData2
{
    public class settings
    {
        public static bool TSQLMode;
        public static String app_path;
        public static String path;
        public static String database_name;
        public static int table_count;
        public static List<List<Boolean>> all_options = new List<List<Boolean>>();
        public static List<String> table_names = new List<String>();
        public static int page_size;
        // to set all tables to have the first 3 options selected by default
        public static void generate_options()
        {
            for (int i = 0; i < table_count; i++)
            {
                List<Boolean> options = new List<Boolean>();
                for (int j = 0; j < 9; j++)
                {
                    if (j == 3) { options.Add(false); continue; }
                    options.Add(true);
                }
                for (int k = 9; k < 18; k++)
                {
                    options.Add(false);
                }
                all_options.Add(options);
            }
        }
    }
}

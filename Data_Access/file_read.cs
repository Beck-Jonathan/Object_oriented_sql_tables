using appData2;
using Data_Objects;
using System;
using System.Collections.Generic;
using System.IO;

namespace Data_Access
{
    public class file_read
    {
        String readpath = settings.path;
        
        public static void readdata()
        {
            int skip_count = 0;
            StreamReader SqlBuddy = new StreamReader(settings.path);
            string ln;
            string tablename = " ";
            string description;
            char[] separator = { '\t' };
            char[] audit_seperator = { ' ' };
            List<Row> rows = new List<Row>();
            int count = 0;
            SqlBuddy.ReadLine(); //skip first line since it's just heading data
            while ((ln = SqlBuddy.ReadLine()) != null)
            {

                string[] parts;
                parts = ln.Split(separator);
                if (parts[0].Length == 0 && parts[1].Length == 0)
                {
                    skip_count++;
                    continue; 

                }

            

                if (parts[1].Length == 0 && count == 0)
                {
                    tablename = parts[0];
                    description = parts[14];
                    count++;
                }
                else if (parts[1].Length == 0 && count > 0)
                {
                    table t = new table(tablename, rows);
                    header h = new header(tablename, parts[14]);
                    t.Header = h;
                    List<Row> rowsfortable = new List<Row>();
                    foreach (Row _row in t.rows)
                    {
                        rowsfortable.Add(_row);
                    }
                    t.rows = rowsfortable;

                    rows.Clear();
                    tablename = parts[0];
                    description = parts[14];
                    data_tables.all_tables.Add(t);
                    count++;
                }
                if (parts[1].Length > 0)
                {
                    String row_name = parts[0].Replace("\"", "");
                    int length;
                    int start;
                    int increment;
                    char nullable=' ';
                    char unique = ' ';
                    char primary_key = ' ';

                    String data_type = parts[1];
                    Int32.TryParse(parts[2],out  length);
                    String default_value = parts[3];
                    String identity = parts[4];
                    Int32.TryParse(parts[5],out  start);
                    Int32.TryParse(parts[6], out  increment);
                    char[] chararray = parts[7].ToCharArray();
                    if (chararray.Length != 0)
                    {
                       nullable = chararray[0];
                    }

                    
                    string index = parts[8];
                    char[] chararray2 = parts[9].ToCharArray();
                    if (chararray2.Length!=0) {                 
                        unique = chararray2[0];
                    }
                    char[] chararray3 = parts[10].ToCharArray();
                    if (chararray3.Length!=0)
                    {
                        primary_key = chararray3[0];
                    }

                    String foreign_key = parts[11];
                    String integrity = parts[12];
                    String references = parts[13];

                    description = parts[14];
                    Row _row = new Row(row_name, data_type, length, default_value, identity, start, increment,
                         nullable, index, unique, primary_key, foreign_key, integrity, references, description);
                    rows.Add(_row);
                }

}
        }
    }
}

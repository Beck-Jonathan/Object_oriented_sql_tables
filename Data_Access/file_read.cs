using appData2;
using Data_Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;

namespace Data_Access
{
    public class file_read
    {
        String readpath = settings.path;

        public static void readdata()
        {
            int skip_count = 0;
            // Read the file selected by user and setup various varables
            StreamReader SqlBuddy = new StreamReader(settings.path);
            string ln;
            string tablename = " ";
            string description;
            char[] separator = { '\t' };
            char[] audit_seperator = { ' ' };
            List<Row> rows = new List<Row>();
            int count = 0;
            SqlBuddy.ReadLine();
            //skip first line since it's just heading data
            // read until the end
            while ((ln = SqlBuddy.ReadLine()) != null)
            {

                string[] parts;
                //split each line in the file into it's component parts
                parts = ln.Split(separator);
                try
                {
                    //catch a fully blank line and ignore it
                    if (parts[0].Length == 0 && parts[1].Length == 0)
                    {
                        skip_count++;
                        continue;

                    }
                }
                catch { IndexOutOfRangeException e; }


                try
                {
                    //parts[1] is blank for the first row of a table, so this catches the table name
                    if (parts[1].Length == 0 && count == 0)
                    {

                        tablename = parts[0];
                        description = parts[14];
                        count++;
                    }

                    //this means we are moving onto a new table, so it creates the old table, then clears the rows array

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
                }
                catch { IndexOutOfRangeException ex; }
                //if parts[1] is greater than zero, that means we are reading a row object which has a data type. 
                //read all row objects and place them into an array of rows, to later be turned into a table

                if (parts[1].Length > 0)
                {
                    //remove any extra punctionation excel added and setup various variables
                    String row_name = parts[0].Replace("\"", "");
                    int length;
                    int start;
                    int increment;
                    char nullable = ' ';
                    char unique = ' ';
                    char primary_key = ' ';

                    //read each part of the row 
                    String data_type = parts[1].Replace("\"","");
                    Int32.TryParse(parts[2], out length);
                    String default_value = parts[3];
                    String identity = parts[4];
                    Int32.TryParse(parts[5], out start);
                    Int32.TryParse(parts[6], out increment);
                    char[] chararray = parts[7].ToCharArray();
                    if (chararray.Length != 0)
                    {
                        nullable = chararray[0];
                    }


                    string index = parts[8];
                    char[] chararray2 = parts[9].ToCharArray();
                    if (chararray2.Length != 0)
                    {
                        unique = chararray2[0];
                    }
                    char[] chararray3 = parts[10].ToCharArray();
                    if (chararray3.Length != 0)
                    {
                        primary_key = chararray3[0];
                    }

                    String foreign_key = parts[11];
                    String integrity = parts[12];
                    String references = parts[13];

                    description = parts[14];
                    //create the row
                    Row _row = new Row(row_name, data_type, length, default_value, identity, start, increment,
                         nullable, index, unique, primary_key, foreign_key, integrity, references, description);
                    //add row to row array
                    rows.Add(_row);
                }

            }
            //figure out how many tables were created
            settings.table_count = data_tables.all_tables.Count;
            //generate default options for all tables
            settings.generate_options();
        }
    }
}


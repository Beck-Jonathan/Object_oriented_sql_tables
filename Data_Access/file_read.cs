﻿using appData2;
using Data_Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Data_Access
{
    public class file_read
    {
        static String readpath = settings.path;
        static List<foreignKey> keys = new List<foreignKey>();

        public static void readdata()
        {
            saveLocaiton();
            int skip_count = 0;
            bool working = false;
            // Read the file selected by user and setup various varables
            StreamReader SqlBuddy = null;
            while (!working)
            {
                try
                {
                    SqlBuddy = new StreamReader(settings.path);
                }
                catch (Exception)
                {

                    working = false;
                    Thread.Sleep(5000);
                }
                working = true;
            }
            string ln;
            string tablename = " ";
            string description;
            char[] separator = { '\t' };
            char[] audit_seperator = { ' ' };
            List<Column> rows = new List<Column>();
            int count = 0;
            SqlBuddy.ReadLine();
            //skip first line since it's just heading data
            // read until the end
            while ((ln = SqlBuddy.ReadLine()) != null)
            {

                string[] parts;
                //split each line in the file into it's component parts
                parts = ln.Split(separator);
                //if (parts[0].Equals("dummy")) { break; }
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
                        table t;
                        if (settings.TSQLMode)
                        {
                            t = new TSqlTable(tablename, rows);
                        }
                        else
                        {
                            t = new MySqlTable(tablename, rows);
                        }

                        header h = new header(tablename, parts[14]);
                        t.Header = h;
                        List<Column> rowsfortable = new List<Column>();
                        foreach (Column _row in t.columns)
                        {
                            rowsfortable.Add(_row);
                        }
                        t.columns = rowsfortable;

                        rows.Clear();
                        tablename = parts[0];
                        description = parts[14];
                        if (t.name.ToLower() != "dummy")
                        {
                            data_tables.all_tables.Add(t);
                        }
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
                    String data_type = parts[1].Replace("\"", "");
                    Int32.TryParse(parts[2], out length);
                    String default_value = parts[3];
                    String identity = parts[4];
                    Int32.TryParse(parts[5], out start);
                    Int32.TryParse(parts[6], out increment);
                    char[] chararray = parts[7].ToCharArray();
                    if (chararray.Length != 0)
                    {
                        char check = chararray[0];
                        if (check == 'y' || check == 'Y')
                        {
                            nullable = 'y';
                        }
                        else
                        {
                            nullable = 'n';
                        }

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
                    Column _row = new Column(row_name, data_type, length, default_value, identity, start, increment,
                         nullable, index, unique, primary_key, foreign_key, integrity, references, description);
                    //add row to row array
                    rows.Add(_row);
                    //add any key
                    if (_row.foreign_key != "")
                    {
                        foreignKey _key = new foreignKey();
                        _key.mainTable = tablename;
                        String[] chunks = _row.references.Split('.');
                        _key.referenceTable = chunks[0];
                        _key.dataType = data_type;
                        _key.fieldName = row_name;
                        string length_text = "";
                        if (data_type.Equals("nvarchar") && length == 0) { length = 50; }
                        if (data_type.Equals("date")) { length = 0; }
                        if (length == 0)
                        {
                            length_text = "";
                        }
                        if (length > 0)
                        {
                            length_text = "(" + length + ")";
                        };
                        _key.lengthText = length_text;
                        data_tables.all_foreignKey.Add(_key);

                    }
                }

            }
            //figure out how many tables were created
            settings.table_count = data_tables.all_tables.Count;
            //generate default options for all tables
            settings.generate_options();
        }
        public static void saveLocaiton()
        {
            file_write.SettingsBuddy.Write(settings.path);
            file_write.SettingsBuddy.Flush();


        }
        public static string readlocaiton()
        {
            StreamReader streamReader = new StreamReader(file_write.SettingsPath);
            return streamReader.ReadLine();

        }
    }
}


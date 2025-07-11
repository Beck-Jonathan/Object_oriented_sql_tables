﻿using appData2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.Eventing.Reader;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
namespace Data_Objects
{
    public class table
    {
        /// <summary>
        /// A variable that determines if this object is a candidate for a View Model.
        /// Gets set to true if this table is Foreign Key'ed to a parent table
        /// </summary>
        bool hasVM = false;
        string servletName = "";
        /// <summary>
        /// Typical spacing for python indents
        /// </summary>
        string fourSpaces = "    ";
        /// <summary>
        /// a random generator used to create the fake data in <see cref="genFakeData(Column)"/>
        /// </summary>
        private Random rand = new Random();
        /// <summary>
        /// the name of the table
        /// </summary>

        public String name { set; get; }
        public header Header { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public List<Column> columns { set; get; }
        public List<String> primary_keys { set; get; }
        public List<String> foreign_keys { set; get; }
        public List<String> alternate_keys { set; get; }
        public table(String name, List<Column> columns)
        {
            if (settings.TSQLMode)
            {
                this.name = "[dbo].[" + name + "]";
            }
            else
            {
                this.name = name;
            }
            this.columns = columns;
        }
        /// <summary>
        /// Reads through each <see cref="Column"/>   object associated with the <see cref="table"/> Object and
        /// generates an data access layer interface for c#
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that represents the data access layer interface in c#</returns>
        public String gen_IThingAccessor()
        {
            string comment = commentBox.genCommentBox(name, Component_Enum.CSharp_IAccessor);
            string header = "public interface I" + name + "Accessor \n{\n";
            string addThing = "int add" + name + "(" + name + " _" + name + ");\n";
            string selectThingbyPK = name + " select" + name + "ByPrimaryKey(string " + name + "ID);\n";
            string selectallThing = "List<" + name + "> selectAll" + name + "(";
            foreach (Column r in columns)
            {
                if (r.foreign_key != "")
                {
                    selectallThing += "," + r.data_type.toCSharpDataType() + " " + r.column_name;
                }
            }
            selectallThing += ");\n";
            string updateThing = "int update" + name + "(";
            updateThing = updateThing + name + "_old" + name + " , " + name + " _new" + name;
            updateThing += ");\n";
            string deleteThing = "int delete" + name + "(" + name + " _" + name + ");\n";
            string undeleteThing = "int undelete" + name + "(" + name + " _" + name + ");\n";
            string dropdownThing = "";
            List<foreignKey> all_foreignKey = data_tables.all_foreignKey;
            foreach (foreignKey fk in all_foreignKey)
            {
                if (fk.mainTable == name)
                {
                    dropdownThing = dropdownThing + "List<String> selectDistinct" + fk.referenceTable + "ForDropDown();\n";
                }
            }
            string selectfkThing = "";
            foreach (Column r in columns)
            {
                if (r.references != "")
                {
                    string[] parts = r.references.Split('.');
                    string fk_table = parts[0];
                    string fk_name = parts[1];
                    selectfkThing = selectfkThing + "select" + name + "by" + fk_table + "(" + r.data_type.toCSharpDataType() + " " + fk_name + ", int limit, int offset);";
                }
            }

            string fileWrite = "int write" + name + "sToFile(List<" + name + "> " + name + "s, string path);\n";
            string fileRead = "List<" + name + "> read" + name + "sFromFile(string path);\n";
            string batchAdd = "int addBatchOf" + name + "(List<" + name + "> " + name.ToLower() + "s);\n";
            string count = "int count" + name + "(";
            string comma = "";
            foreach (Column r in columns)
            {
                if (r.foreign_key != "")
                {
                    count += comma + r.data_type.toCSharpDataType() + " " + r.column_name;
                    comma = ",";
                }
            }
            count += ");\n";
            string genTemplate = "FileStream get" + name + "TemplateFile();\n";
            string export = "FileStream export" + name + "ToFile();\n";

            string output = comment + header + addThing + selectThingbyPK + selectallThing + selectfkThing + updateThing + deleteThing + undeleteThing + dropdownThing + fileWrite + fileRead+ batchAdd+count+genTemplate+export+"}\n\n";
            return output;
        }
        /// <summary>
        /// Reads through each <see cref="Column"/>   object associated with the <see cref="table"/> Object and
        /// generates an data access layer  for c#
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that represents the data access layer  in c# </returns>
        public String gen_ThingCharpDatabaseAccessor()
        {

            string comment = commentBox.genCommentBox(name, Component_Enum.CSharp_Accessor);

            string header = gencCSharpDatabaseAccessorClassHeader();

            string addThing = genCharpDatabaseAccessorAdd();

            string selectThingbyPK = genCharpDatabaseAccessorretrieveByKey();

            string selectallThing = genCharpDatabaseAccessorretrieveAll();

            string selectbyFK = genCharpDatabaseAccessorretrievefk();

            string updateThing = genCharpDatabaseAccessorUpdate();

            string deleteThing = genCharpDatabaseAccessorDelete();

            string undeleteThing = genCharpDatabaseAccessorUndelete();
            string distinctThing = genCharpDatabaseAccessorDistinct();
            string writeThing = genCharpDatabaseAccessorWrite();
            string readThing = genCharpDatabaseAccessorRead();
            string addBatchThing = genCSharpDatabaseAccessorBatchAdd();
            string count = genCSharpDatabaseAccessorCount();
            string template = genCSharpDatabaseAccessoTemplate();
            string export = genCSharpDatabaseAccessorExport();
            string output = comment + header + addThing + selectThingbyPK + selectallThing + selectbyFK + updateThing + deleteThing + undeleteThing + distinctThing +writeThing+readThing+ addBatchThing +count+template+export+ "}\n\n";
            //good
            return output;
        }
        public String gen_ThingCharpFileAccessor()
        {

            string comment = commentBox.genCommentBox(name, Component_Enum.CSharp_Accessor);

            string header = gencCSharpFileAccessorClassHeader();  //done
             
            string addThing = genCharpFileAccessorAdd();    //done

            string selectThingbyPK = genCharpFileAccessorretrieveByKey();   //done

            string selectallThing = genCharpFileAccessorretrieveAll();  //done

            string selectbyFK = genCharpFileAccessorretrievefk();
             
            string updateThing = genCharpFileAccessorUpdate(); //done

            string deleteThing = genCharpFileAccessorDelete(); //done

            string undeleteThing = genCharpFileAccessorUndelete(); //done
            string distinctThing = genCharpFileAccessorDistinct();
            string writeThing = genCharpFileAccessorWrite(); //done
            string readThing = genCharpFileAccessorRead();
            string addBatchThing = genCSharpFileAccessorBatchAdd(); //done
            string count = genCSharpFileAccessorCount();
            string template = genCSharpFileAccessoTemplate() ;
            string export = genCSharpFileAccessorExport();
            string output = comment + header + readThing + addThing + selectThingbyPK + selectallThing + selectbyFK + updateThing + deleteThing + undeleteThing + distinctThing + writeThing  + addBatchThing + count+export+template+"}\n}\n";
            //good
            return output;
        }

        private string gencCSharpFileAccessorClassHeader() {
            string result = "using DataObjects;\n";
            result += "using iDataAccessLayer;\n";
            result += "\n";
            result += "namespace DataAccessLayer{\n\n";
            result += "public class "+name+"Accessor : I"+name+"Accessor\n{\n";
            result += "private List<"+name+"> "+name.ToLower()+"s;\n";
            
            return result;
        
        }
        private string genCharpFileAccessorAdd()
        {
            string result = "public int add"+name+"("+name+ " _"+name.ToLower()+")\n{\n";
            result += "if (" + name.ToLower() + "s ==null)\n{\n";
            result += "getAll" + name + "s();\n";
            result += "}\n";
            result += name.ToLower() + "s.Add(_" + name.ToLower() + ");\n";
            result += "string filePath = AppData.AppPath + \"\\\\\" + \"Data\\\\"+name.ToLower()+"s.txt\";\n";
            result += "try{\n";
            result += "using (StreamWriter w = File.AppendText(filePath))\n";
            result += "{\n";
            result += "w.WriteLine(";
            string plus = "";
            foreach (Column r in columns) {
                result += plus+" _" + name.ToLower() + "." + r.column_name ;
                    plus = "+\"\\t\"+";
            }
            result += ");\n";
            result += "}\n";
            result += "}\n";
            result += "catch (Exception)\n";
            result += "{\n";
            result += "throw new ApplicationException(\"unable to append to file\");\n";
            result += "}\n";
            result += "return 1;\n";
            result += "}\n";

            return result;

        }
        private string genCharpFileAccessorretrieveByKey()
        {
            string firstLetter = name.Substring(0, 1).ToLower();
            string result = "public "+name+ " select"+name+"ByPrimaryKey("+name+" "+name.ToLower()+")\n{\n";
            result +=name+ " _"+name.ToLower()+" = null;\n";
            result += "try\n{\n";
            result += "foreach (" + name + " " + firstLetter + " in " + name.ToLower() + "s)\n{\n";
            result += "if(";
            string andand = "";
            foreach (Column r in columns) {
                if (r.primary_key == 'y' || r.primary_key == 'Y') {
                    result += andand+ firstLetter + "." + r.column_name + ".Equals(" + name.ToLower() + "." + r.column_name + ")";
                    andand = " && ";
                }
            }
            result += ")\n{\n";
            result += "_"+name.ToLower()+ " = new "+name+"("+firstLetter+");\n";
            result += "break;";
            result += "}\n";
            result += "}\n";
            result += "if (_"+name.ToLower()+" ==null)\n{\n";
            result += "throw new ApplicationException(\"unable to find "+name+"\");";
            result += "}\n";
            result += "}\n";
            result += "catch (Exception ex)";
            result += "{\n";
            result += "throw new ApplicationException(\"Unable to find "+name+"\", ex);";
            result += "}\n";
            result += "return _"+name.ToLower()+";\n";
            result += "}\n";
            return result;

        }
        private string genCharpFileAccessorretrieveAll()
        {
            string firstLetter = name.Substring(0, 1).ToLower();
            int fkCount = 0;
            string result = "public List<" + name + "> SelectAll" + name + "(int offset, int limit";
            foreach (Column r in columns) {
                if (r.references != "") {
                    string[] parts = r.references.Split('.');
                    string fk_table = parts[0];
                    
                    result += ",string " + fk_table.ToLower();
                    fkCount++;
                }
            }
            result += ")\n{\n";
            result += "List<" + name + "> result = new List<" + name + ">();\n";
            result += "if ("+name.ToLower()+"s ==null)\n{\n";
            result += "getAll"+name+"s();\n";
            result += "}\n";
            result += "foreach (" + name + " " + firstLetter + " in " + name.ToLower() + "s)\n{";
            string andand = "";
            if (fkCount == 0)
            {                
                result += "result.Add("+firstLetter+";\n";

            }
            else {
                result += "if (";
                foreach (Column r in columns) {
                    if (r.references != "") {
                        string[] parts = r.references.Split('.');
                        string fk_table = parts[0];
                        result += andand + firstLetter + "." + r.column_name + ".Equals(" +fk_table.ToLower()+")";
                        andand = "&&";
                    }
                }
                result += ")\n{result.Add(" + firstLetter + ");\n";
                result += "}\n";

            }
            result += "}";
            result += "for (int i = 0; i < offset; i++)\n";
            result += "{\n";
            result += "result.RemoveAt(0);";
            result += "}\n";
            result += "if (limit >0)\n";
            result += "{\n";
            result += "for (int i = limit; i < result.Count; i++)";
            result += "{\n";
            result += "result.RemoveAt(limit);";
            result += "}\n";
            result += "}\n";
            result += "return result;\n";
            result += "}\n";

            return result;

        }
        private string genCharpFileAccessorretrievefk()
        {
            string result = "";

            return result;

        }
        private string genCharpFileAccessorUpdate()
        {
            string result = "public int update"+name+"("+name+" old"+name+", "+name+" new"+name+")\n{\n";
            result += "int result = 0;\n";
            result += "try\n";
            result += "{\n";
            result += "result += delete"+name+"(old"+name+");\n";
            result += "if (result != 1)";
            result += "{\n";
            result += "throw new ApplicationException(\"unable to delete "+name.ToLower() + "\");\n";
            result += "}\n";
            result += "result += add"+name+"(new"+name+");";
            result += "if (result != 2)";
            result += "{\n";
            result += "throw new ApplicationException(\"unable to add " + name.ToLower() + "\");\n";
            result += "}\n";
            result += "}\n";
            result += "catch (Exception ex) {\n";
            result += "throw new ApplicationException(\"unable to update "+name.ToLower()+"\", ex);\n";
            result += "}\n";
            result += "return result;\n";
            result += "}\n";


            return result;

        }
        private string genCharpFileAccessorDelete()
        {
            string firstLetter = name.Substring(0, 1).ToLower();
            string result = "public int delete" + name + "(" + name + " " + name.ToLower() + ")\n{\n";
            result += "int result=0;\n";
            result += "try\n{\n";
            result += "foreach (" + name + " " + firstLetter + " in " + name.ToLower() + "s)\n{\n";
            result += "if(";
            string andand = "";
            foreach (Column r in columns)
            {
                if (r.primary_key == 'y' || r.primary_key == 'Y')
                {
                    result += andand + firstLetter + "." + r.column_name + ".Equals(" + name.ToLower() + "." + r.column_name + ")";
                    andand = " && ";
                }
            }
            result += ")\n{\n";
            result += name.ToLower() + "s.Remove(" + firstLetter + ");\n";
            result += "result=1;\n";
            result += "break;";
            result += "}\n";
            result += "}\n";
            result += "if (result==0)\n{\n";
            result += "throw new ApplicationException(\"unable to delete " + name + "\");";
            result += "}\n";
            result += "else {\n";
            result += "rewriteDataFile("+name.ToLower()+"s);\n";
            result += "}\n";
            result += "";
            result += "}\n";
            result += "catch (Exception ex)";
            result += "{\n";
            result += "throw new ApplicationException(\"Unable to delete " + name + "\", ex);";
            result += "}\n";
            result += "return result;\n";
            result += "}\n";
            return result;

        }
        private string genCharpFileAccessorUndelete()
        {
            string firstLetter = name.Substring(0, 1).ToLower();
            string result = "public int undelete" + name + "(" + name + " " + name.ToLower() + ")\n{\n";
            result += "int result=0;\n";
            result += "try\n{\n";
            result += "foreach (" + name + " " + firstLetter + " in " + name.ToLower() + "s)\n{\n";
            result += "if(";
            string andand = "";
            foreach (Column r in columns)
            {
                if (r.primary_key == 'y' || r.primary_key == 'Y')
                {
                    result += andand + firstLetter + "." + r.column_name + ".Equals(" + name.ToLower() + "." + r.column_name + ")";
                    andand = " && ";
                }
            }
            result += ")\n{\n";
            result += firstLetter + ".isActive=true;\n";
            result += "result=1;\n";
            result += "break;";
            result += "}\n";
            result += "}\n";
            result += "if (result==0)\n{\n";
            result += "throw new ApplicationException(\"unable to undelete " + name + "\");";
            result += "}\n";
            result += "}\n";
            result += "catch (Exception ex)";
            result += "{\n";
            result += "throw new ApplicationException(\"Unable to undelete " + name + "\", ex);";
            result += "}\n";
            result += "return result;\n";
            result += "}\n";
            return result;

        }
        private string genCharpFileAccessorDistinct()
        {
            string result = "";

            return result;

        }
        private string genCharpFileAccessorWrite()
        {
            string result = "private bool rewriteDataFile(List<"+name+"> "+name+"s)\n";
            result += "{\n";
            result += "string filePath = AppData.AppPath + \"\\\\\" + \"Data\\\\"+name.ToLower()+"s.txt\";\n";
            result += "try\n";
            result += "{\n";
            result += "StreamWriter fileBuddy = new StreamWriter(filePath);\n";
            result += "foreach ("+name+" _"+name.ToLower()+" in "+name+"s)\n";
            result += "{\n";
            result += "fileBuddy.WriteLine(";
            string tabplus = "";
            foreach (Column r in columns) {
                result += tabplus+"_"+name.ToLower()+"."+r.column_name+"\n";
                 tabplus = "+\"\t\" +";
            }
            result += ");\n";
            result += "}\n";
            result += "fileBuddy.Close();\n";
            result += "}\n";
            result += "catch (Exception)";
            result += "{\n";
            result += "return false;\n";
            result += "}\n";
            result += "return true;\n";
            result += "}\n";
            

            return result;

        }
        private string genCharpFileAccessorRead()
        {
            string result = "private void getAll"+name+"s()\n";
            result += "{\n";
            result += name.ToLower()+"s = new List<"+name+">();\n";
            result += "string filePath = AppData.AppPath + \"\\\\\" + \"Data\\\\"+name.ToLower()+"s.txt\";";
            result += "try\n";
            result += "{\n";
            result += "StreamReader fileReader = new StreamReader(filePath);\n";
            result += "//the first line is heading data\n";
            result += "fileReader.ReadLine();\n";
            result += "char[] separator = { '\t' };\n";
            result += "while (fileReader.EndOfStream == false)\n";
            result += "{\n";
            result += "string line = fileReader.ReadLine();\n";
            result += "string[] parts;\n";
            result += "if (line.Length > 3)\n";
            result += "{\n";
            result += "parts = line.Split(separator);\n";
            result += "if (parts.Count() > "+(columns.Count-1)+")  //are all model parts present\n";
            result += "{\n"; //pick each part
            int i = 0;
            foreach (Column r in columns)
            {
                if (r.increment == 0)
                {
                    if (r.data_type.toCSharpDataType().ToLower().Equals("string"))
                    {
                        result += r.data_type.toCSharpDataType() + " " + r.column_name.ToLower() + "= parts[" + i + "];\n";

                    }
                    else if (r.data_type.toCSharpDataType().ToLower().Equals("bool"))
                    {
                        result += r.data_type.toCSharpDataType() + " " + r.column_name.ToLower() + "= Boolean.Parse(parts[" + i + "]);\n";

                    }
                    else if (r.data_type.toCSharpDataType().ToLower().Equals("int"))
                    {
                        result += r.data_type.toCSharpDataType() + " " + r.column_name.ToLower() + "= Int32.Parse(parts[" + i + "]);\n";

                    }
                    else if (r.data_type.toCSharpDataType().ToLower().Equals("DateTime"))
                    {

                        result += r.data_type.toCSharpDataType() + " " + r.column_name.ToLower() + "= DateTime.Parse(parts[" + i + "]);\n";
                    }
                    else if (r.data_type.toCSharpDataType().ToLower().Equals("decimal"))
                    {

                        result += r.data_type.toCSharpDataType() + " " + r.column_name.ToLower() + "= Decimal.Parse(parts[" + i + "]);\n";
                    }

                    else
                    {
                        result += r.data_type + " " + r.column_name.ToLower() + "=(" + r.data_type + ") parts[" + i + "];\n";
                    }

                        
                    i++;
                }
            }
            result += name + " " + name.ToLower() + " = new " + name + "(";
            string comma = "";
            foreach (Column r in columns)
            {
                result += comma + r.column_name.ToLower();
                comma = ", ";
            }
            result += ");\n";
            result += name.ToLower()+"s.Add(" + name.ToLower() + ");\n";


            result += "}\n";
            result += "}\n";
            result += "}\n";
            result += "fileReader.Close();\n";
            result += "if ("+name.ToLower()+"s == null || "+name.ToLower()+"s.Count == 0)\n";
            result += "{\n";
            result += "throw new Exception(\"Unable to load "+name.ToLower()+"s\");\n";
            result += "}\n";
            result += "}\n";
            result += "catch (Exception ex)\n";
            result += "{\n";
            result += "throw new ApplicationException(\""+name+"s not found\", ex);\n";
            result += "}\n";
            string orderBy = ".OrderBy(o => o.";
            result += name.ToLower()+"s = "+name.ToLower()+"s";
            foreach (Column r in columns) {
                result += orderBy + r.column_name + ")";
                orderBy = ".ThenBy( o => o.";
            }
            result += ".ToList();\n";
            result += "}\n";
            return result;

        }
        private string genCSharpFileAccessorBatchAdd()
        {
            string result = "";
            string firstLetter = name.Substring(0, 1).ToLower();
            result += "public int AddBatchOf" + name + "(List<" + name + "> _"+name.ToLower()+"s)\n{\n";
            result += "int result=0;";
            result += "try\n{\n";
            result += "foreach (" + name + " " + firstLetter + " in _" + name.ToLower() + "s){\n";
            result += "result += add" + name + "(" + firstLetter + ");\n";
            result += "}\n";
            result += " }catch (Exception ex)\n";
            result += "{\n";
            result += "throw new ApplicationException(\"unable to append to file\",ex);";
            result += "}\n";
            result += "return result;\n";
            result += "}\n";
            return result;

        }

        private string genCSharpFileAccessorCount() {
            string result = "";
            result += "";
            result += "";
            result += "";
            result += "";
            result += "";
            result += "";
            return result;
        
        }

        private string genCSharpFileAccessoTemplate() {
            string result = "";
            result += "";
            result += "";
            result += "";
            result += "";
            result += "";
            result += "";
            return result;
        }

        private string genCSharpFileAccessorExport() {
            string result = "";
            result += "";
            result += "";
            result += "";
            result += "";
            result += "";
            result += "";
            return result;
        }


        /// <summary>
        /// Reads through each <see cref="Column"/>   object associated with the <see cref="table"/> Object and
        /// generates a logic layer interface for c#
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that represents the data access layer interface in c# </returns>
        public String gen_IThingManager()
        {
            _ = commentBox.GenXMLClassComment(this, XMLClassType.CSharpIManager);
            string comment = commentBox.genCommentBox(name, Component_Enum.CSharp_IManager);
            string header = "public interface I" + name + "Manager \n{\n";
            string addThing = "int add" + name + "(" + name + " _" + name + ");\n";
            string getThingbyPK = name + " get" + name + "ByPrimaryKey(string " + name + "ID);\n";
            string getallThing = "List<" + name + "> getAll" + name + "();\n" +
                                   "List<" + name + "> getAll" + name + "(int offset);\n" +
                                   "List<" + name + "> getAll" + name + "(int limit, int offset";
            foreach (Column r in columns)
            {
                if (r.foreign_key != "")
                {
                    getallThing += "," + r.data_type.toCSharpDataType() + " " + r.column_name;
                }
            }

            getallThing += ");\n";
            List<foreignKey> all_foreignKey = data_tables.all_foreignKey;
            string getfkThing = "";
            foreach (Column r in columns)
            {
                if (r.references != "")
                {
                    string[] parts = r.references.Split('.');
                    string fk_table = parts[0];
                    string fk_name = parts[1];
                    getfkThing = getfkThing + "List<" + name + "> get" + name + "by" + fk_table + "(" + fk_name + ");\n" +
                                   "List<" + name + "> getAll" + name + "by" + fk_table + "(" + fk_name + ",int offset);\n" +
                                   "List<" + name + "> getAll" + name + "by" + fk_table + "(" + fk_name + ",int limit, int offset);\n";
                }
            }
            string editThing = "int edit" + name + "(";
            editThing = editThing + name + " _old" + name + " , " + name + " _new" + name;
            editThing += ");\n";
            string purgeThing = "int purge" + name + "(string " + name + "ID);\n";
            string unPurgeThing = "int unpurge" + name + "(string " + name + "ID);\n";
            string dropdownThing = "";
            foreach (foreignKey fk in all_foreignKey)
            {
                if (fk.mainTable == name)
                {
                    dropdownThing = dropdownThing + "List<String> getDistinct" + fk.referenceTable + "ForDropDown();\n";
                }
            }

            string fk_thing = "";
            foreach (foreignKey key in data_tables.all_foreignKey)
            {
                if (key.referenceTable == name)
                {
                    fk_thing = fk_thing + "List<" + key.mainTable + "> getAll" + key.mainTable + "by" + name + "();\n";
                }
            }

            string fileWrite = "int write"+name+"sToFile(List<"+name+"> "+name+"s, string path);\n";
            string fileRead = "List<" + name + "> read" + name + "sFromFile(string path);\n";
            string batchAdd = "int addBatchOf" + name + "(List<" + name + "> " + name.ToLower() + "s);\n";
            string count = "int count" + name + "(";
            string comma = "";
            foreach (Column r in columns)
            {
                if (r.foreign_key != "")
                {
                    count += comma + r.data_type.toCSharpDataType() + " " + r.column_name;
                    comma = ",";
                }
            }
            count += ");\n";
            string genTemplate = "FileStream get"+name+"TemplateFile();\n";
            string export = "FileStream export" + name + "ToFile();\n";

            string output = comment + header + addThing + getThingbyPK + getallThing + getfkThing + editThing + purgeThing + unPurgeThing + dropdownThing + fk_thing+ fileRead+fileWrite+batchAdd+count+genTemplate+export+"}\n\n";
            
            return output;
        }
        /// <summary>
        /// Reads through each <see cref="Column"/>   object associated with the <see cref="table"/> Object and
        /// generates a logic layer  for c#
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that represents the data access layer interface in c# </returns>
        public String gen_ThingMananger()
        {
            string result = "";
            string header = genManagerHeader();
            string Add = genManagerAdd();
            string Delete = genManagerDelete();
            string unDelete = genManagerUnDelete();
            string retrieveByPK = genManagerPK();
            string retrieveByFK = genManagerFK();
            string RetrieveAll = genManagerAll();
            string Update = genManagerUpdate();
            string fileWrite = genManagerFileWrite();
            string fileRead = genManagerFileRead();
            string batchAdd = genManagerBatchAdd();
            string validator = genCSharpValidationMethod();
            string genTempalte = genManagerTemplateFile();
            string export = genManagerExport();
            string count = genManagerCount();
            //String dropdown = genManagerDropDown();
            string footer = "\n}\n";
            result = result
                + header
                + Add
                + Delete
                + unDelete
                + retrieveByPK
                + retrieveByFK
                + RetrieveAll
                + Update
                + fileWrite
                + fileRead
                + batchAdd
                + validator
                +genTempalte
                + export
                +count
                // + dropdown
                + footer
                ;
            return result;
        }
        /// <summary>
        /// Generates a dependency-inversion based header for the Logic layer manager, which incorportates a constructor
        /// that takes the data access object.
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is c# code for the header of the logic layer </returns>
        private string genManagerHeader()
        {
            string comment = commentBox.genCommentBox(name, Component_Enum.CSharp_IManager);
            String result = "";
            result = result + "public class " + name + "Manager : I" + name + "Manager\n";
            result += "{\n";
            result = result + "private I" + name + "Accessor _" + name.ToLower() + "Accessor=null;\n";
            result += "//default constuctor uses the database\n";
            result = result + "public " + name + "Manager()\n";
            result += "{\n";
            result = result + "_" + name.ToLower() + "Accessor = new " + name + "Accessor();\n";
            result += "}\n";
            result += "//the optional constuctor can accept any data provider\n";
            result = result + "public " + name + "Manager(I" + name + "Accessor " + name.ToLower() + "Accessor)\n";
            result += "{\n";
            result = result + "_" + name.ToLower() + "Accessor = " + name.ToLower() + "Accessor;\n";
            result += "}\n";
            return comment + result;
        }
        /// <summary>
        /// Generates a logic layer method that takes in an object this <see cref="table"/> reprents and passes it to the data
        /// access layer for adding to the database.
        /// Jonathan Beck
        /// </summary>
        /// <returns>A logic layer add method, in c#. </returns>
        private String genManagerAdd()
        {
            string comment = commentBox.GenXMLMethodComment(this, XML_Method_Type.CSharp_Manager_Add);
            String result = "\n";
            result = result + "public bool Add" + name + "(" + name + " " + "_" + name + "){\n";
            result += "bool result = false;;\n";
            result += "bool isValid = false;\n";
            result += "ValidateModel(_" + name + ",ref isValid);\n";
            result += "if (isValid){\n";
            result += "try\n{";
            result = result + "result = (1 == _" + name.firstCharLower() + "Accessor.insert" + name + "(_" + name + "));\n";
            result += "}\n";
            result += "catch (Exception ex)\n";
            result += "{\n";
            result = result + "throw new ApplicationException(\"" + name + " not added\" + ex.InnerException.Message, ex);;\n";
            result += "}\n";
            result += "}\n";
            result += "return result;\n";
            result += "}\n";
            result += "\n";
            return comment + result;
        }
        private String genManagerBatchAdd()
        {
            string comment = commentBox.GenXMLMethodComment(this, XML_Method_Type.CSharp_Manager_Add);
            String result = "\n";
            result = result + "public int addBatchOf" + name + "(List<" + name + "> " + "_" + name + "s){\n";
            result += "int result = 0;\n";
            result += "try\n{";
            result +="result =  _" + name.firstCharLower() + "Accessor.addBatchOf" + name + "(_" + name + "s));\n";
            result += "}\n";
            result += "catch (Exception ex)\n";
            result += "{\n";
            result +=  "throw new ApplicationException(\"" + name + " not added\" + ex.InnerException.Message, ex);\n";
            result += "}\n";
            result += "return result;\n";
            result += "}\n";
            result += "\n";
            return comment + result;
        }
        /// <summary>
        /// Generates a logic layer method that takes in an object this <see cref="table"/> reprents and passes it to the data
        /// access layer for deleting
        /// Jonathan Beck
        /// </summary>
        /// <returns>A logic layer delete method, in c#. </returns>
        private String genManagerDelete()
        {
            string comment = commentBox.GenXMLMethodComment(this, XML_Method_Type.CSharp_Manager_Delete);
            string purgeThing = "public int purge" + name + "(" + name + " " + name.ToLower() + "){\n";
            purgeThing += "int result = 0;\n";
            purgeThing += "try{\n";
            purgeThing += "result = _" + name.ToLower() + "Accessor.delete" + name + "(" + name.ToLower() + "." + name + "Id);\n";
            purgeThing += "if (result == 0){\n";
            purgeThing += "throw new ApplicationException(\"Unable to Delete " + name + "\" );\n";
            purgeThing += "}\n";
            purgeThing += "}\n";
            purgeThing += "catch (Exception ex){\n";
            purgeThing += "throw ex;\n";
            purgeThing += "}\n";
            purgeThing += "return result;\n}\n";
            return comment + purgeThing;
        }
        /// <summary>
        /// Generates a logic layer method that takes in an object this <see cref="table"/> reprents and passes it to the data
        /// access layer for undeleting
        /// Jonathan Beck
        /// </summary>
        /// <returns>A logic layer undelete method, in c#. </returns>
        private String genManagerUnDelete()
        {
            string comment = commentBox.GenXMLMethodComment(this, XML_Method_Type.CSharp_Manager_Undelete);
            string purgeThing = "public int unpurge" + name + "(" + name + " " + name.ToLower() + "){\n";
            purgeThing += "int result = 0;\n";
            purgeThing += "try{\n";
            purgeThing = purgeThing + "result = _" + name.ToLower() + "Accessor.undelete" + name + "(" + name.ToLower() + "." + name + "Id);\n";
            purgeThing += "if (result == 0){\n";
            purgeThing = purgeThing + "throw new ApplicationException(\"Unable to restore " + name + "\" );\n";
            purgeThing += "}\n";
            purgeThing += "}\n";
            purgeThing += "catch (Exception ex){\n";
            purgeThing += "throw ex;\n";
            purgeThing += "}\n";
            purgeThing += "return result;\n}\n";
            return comment + purgeThing;
        }
        /// <summary>
        /// Generates a logic layer method that takes in an object ID and passes it to the data
        /// access layer for accessing a record by primary key
        /// Jonathan Beck
        /// </summary>
        /// <returns>A logic layer retrieve by PK method, in c#. </returns>
        private string genManagerPK()
        {
            string comment = commentBox.GenXMLMethodComment(this, XML_Method_Type.CSharp_Manager_retrieve_By_PK);
            string retrieveThing = "public " + name + " get" + name + "ByPrimaryKey(string " + name + "ID){\n";
            retrieveThing = retrieveThing + name + " result =null ;\n";
            retrieveThing += "try{\n";
            retrieveThing = retrieveThing + "result = _" + name.ToLower() + "Accessor.select" + name + "ByPrimaryKey(" + name + "ID);\n";
            retrieveThing += "if (result == null){\n";
            retrieveThing = retrieveThing + "throw new ApplicationException(\"Unable to retrieve " + name + "\" );\n";
            retrieveThing += "}\n";
            retrieveThing += "}\n";
            retrieveThing += "catch (Exception ex){\n";
            retrieveThing += "throw ex;\n";
            retrieveThing += "}\n";
            retrieveThing += "return result;\n}\n";
            return comment + retrieveThing;
        }
        /// <summary>
        /// Generates a logic layer method that takes in no paramaters and passes a request to the data
        /// access layer for accessing all records for this <see cref="table"/>
        /// Jonathan Beck
        /// </summary>
        /// <returns>A logic layer retrieve by all method, in c#. </returns>
        private string genManagerAll()
        {
            string comment = commentBox.genCommentBox(name, Component_Enum.CSharp_Manager_retrieve_All_No_Param);
            comment += commentBox.GenXMLMethodComment(this, XML_Method_Type.CSharp_Manager_retrieve_All_No_Param);
            string retrieveAll = comment + "\npublic List<" + name + "> get" + name + "ByAll(){\n";
            retrieveAll = retrieveAll + "return get" + name + "ByAll(0," + appData2.settings.page_size + ");\n}\n";
            comment = commentBox.genCommentBox(name, Component_Enum.CSharp_Manager_retrieve_All_One_Param);
            comment += commentBox.GenXMLMethodComment(this, XML_Method_Type.CSharp_Manager_retrieve_All_One_Param);
            retrieveAll = retrieveAll + comment + "\npublic List<" + name + "> get" + name + "ByAll(int offset){\n";
            retrieveAll = retrieveAll + "return get" + name + "ByAll(offset, " + appData2.settings.page_size + ");\n}\n";
            comment = commentBox.genCommentBox(name, Component_Enum.CSharp_Manager_retrieve_All_Two_Param);
            comment += commentBox.GenXMLMethodComment(this, XML_Method_Type.CSharp_Manager_retrieve_All_Two_Param);
            retrieveAll = retrieveAll + comment + "\npublic List<" + name + "> get" + name + "ByAll(int offset, int limit";
            foreach (Column r in columns)
            {
                if (r.foreign_key != "")
                {
                    retrieveAll += "," + r.data_type.toCSharpDataType() + " " + r.column_name;
                }
            }
            retrieveAll += "){\n";
            retrieveAll = retrieveAll + "List<" + name + "> result =new List<" + name + ">();\n";
            retrieveAll += "try{\n";
            retrieveAll = retrieveAll + "result = _" + name.ToLower() + "Accessor.selectAll" + name + "(offset,limit";
            foreach (Column r in columns)
            {
                if (r.foreign_key != "")
                {
                    retrieveAll += ", " + r.column_name;
                }
            }
            retrieveAll += ");\n";
            retrieveAll += "if (result.Count == 0){\n";
            retrieveAll = retrieveAll + "throw new ApplicationException(\"Unable to retrieve " + name + "s\" );\n";
            retrieveAll += "}\n";
            retrieveAll += "}\n";
            retrieveAll += "catch (Exception ex){\n";
            retrieveAll += "throw ex;\n";
            retrieveAll += "}\n";
            retrieveAll += "return result;\n}\n";
            return retrieveAll;
        }
        /// <summary>
        /// Generates a logic layer method that takes in an object ID and passes it to the data
        /// access layer for accessing a record by foreign key
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string representing logic layer retrieve by FK method, in c#.</returns>
        private string genManagerFK()
        {
            string getfkThing = "";
            foreach (Column r in columns)
            {
                if (r.references != "")
                {
                    string[] parts = r.references.Split('.');
                    string fk_table = parts[0];
                    string fk_name = parts[1];
                    getfkThing += commentBox.genCommentBox(name, Component_Enum.CSharp_Manager_retrieve_By_FK_No_Param);
                    getfkThing = getfkThing + "\npublic List<" + name + "> get" + name + "by" + fk_table + "(" + r.data_type.toCSharpDataType() + " " + fk_name + "){\n" +
                        "return getAll" + name + "by" + fk_table + "(" + fk_name + "," + appData2.settings.page_size + ",0);" +
                        "\n}\n";
                    getfkThing += commentBox.genCommentBox(name, Component_Enum.CSharp_Manager_retrieve_By_FK_One_Param);
                    getfkThing = getfkThing +
                    "\npublic List<" + name + "> getAll" + name + "by" + fk_table + "(" + r.data_type.toCSharpDataType() + " " + fk_name + ",int offset){\n" +
                 "return getAll" + name + "by" + fk_table + "(" + fk_name + "," + appData2.settings.page_size + ",offset);" +
                "\n}\n";
                    getfkThing += commentBox.genCommentBox(name, Component_Enum.CSharp_Manager_retrieve_By_FK_Two_Param);
                    getfkThing = getfkThing + "public List<" + name + "> getAll" + name + "by" + fk_table + "(" + r.data_type.toCSharpDataType() + " " + fk_name + ",int limit, int offset){\n";
                    getfkThing = getfkThing + "List<" + name + "> result =new List<" + name + ">();\n";
                    getfkThing += "try{\n";
                    getfkThing = getfkThing + "result = _" + name.ToLower() + "Accessor.select" + name + "by" + fk_table + "(" + fk_name + ",offset,limit);\n";
                    getfkThing += "if (result.Count == 0){\n";
                    getfkThing = getfkThing + "throw new ApplicationException(\"Unable to retrieve " + name + "s\" );\n";
                    getfkThing += "}\n";
                    getfkThing += "}\n";
                    getfkThing += "catch (Exception ex){\n";
                    getfkThing += "throw ex;\n";
                    getfkThing += "}\n";
                    getfkThing += "return result;\n}\n";
                }
            }
            return getfkThing;
        }

        private string genManagerFileWrite() {
            


            string result = "int write" + name + "sToFile(List<" + name + "> "+name+"s, string path){\n";
            result += "int result = 0;;\n";
            result += "try\n{";
            result += "result = _"+name.ToLower()+"Accessor.write" + name + "sToFile(" + name + "s, path);\n";
            result += "}\n";
            result += "catch (Exception ex)\n";
            result += "{\n";
            result = result + "throw new ApplicationException(\"" + name + "s not written to File\" + ex.InnerException.Message, ex);;\n";
            result += "}\n";
            result += "return result;\n";
            result += "}\n";
            result += "\n";

            return result;
        }
        private string genManagerFileRead() {
            
            string result = "List<" + name + "> read" + name + "sFromFile(string path){\n";
            result += "List<"+name+"> results = new List<"+name+">();\n";
            result += "try\n{";
            result = result + "results = _"+name.ToLower()+"Accessor.read" + name + "sFromFile(path);\n";
            result += "}\n";
            result += "catch (Exception ex)\n";
            result += "{\n";
            result = result + "throw new ApplicationException(\"" + name + "s not read from File\" + ex.InnerException.Message, ex);;\n";
            result += "}\n";
            result += "return results;\n";
            result += "}\n";
            result += "\n";
            return result;
        }

        /// <summary>
        /// Generates a logic layer method that takes in two instances of an object (old and new) and passes them to the data
        /// access layer for updating
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string representing logic layer update, in c#. </returns>
        private string genManagerUpdate()
        {
            string comment = commentBox.GenXMLMethodComment(this, XML_Method_Type.CSharp_Manager_Update);
            string updateThing = "public int update" + name + "( " + name + " old" + name + ", " + name + " new" + name + "){\n";
            updateThing += "int result =0 ;\n";
            updateThing += "bool oldIsValid = false;\n";
            updateThing += "bool newIsValid = false;\n";
            updateThing += "ValidateModel(old" + name + ",ref oldIsValid);\n";
            updateThing += "ValidateModel(new" + name + ",ref newIsValid);\n";
            updateThing += "if (oldIsValid&&newIsValid){\n";
            updateThing += "try{\n";
            updateThing = updateThing + "result = _" + name.ToLower() + "Accessor.update" + name + "(old" + name + ", new" + name + ");\n";
            updateThing += "if (result == 0){\n";
            updateThing = updateThing + "throw new ApplicationException(\"Unable to update " + name + "\" );\n";
            updateThing += "}\n";
            updateThing += "}\n";
            updateThing += "catch (Exception ex){\n";
            updateThing += "throw ex;\n";
            updateThing += "}\n";
            updateThing += "}\n";
            updateThing += "return result;\n}\n";
            return comment + updateThing;
        }

        private string genManagerTemplateFile() {
            String result = "\n";
            result = result + "public FileStream get" + name + "TempalteFile(){\n";
            result += "FileStream result = null;;\n";
            
            
            result += "try\n{";
            result = result + "result = "+name+"Accessor.get" + name + "TempalteFile();\n";
            result += "}\n";
            result += "catch (Exception ex)\n";
            result += "{\n";
            result = result + "throw new ApplicationException(\"" + name + " tempalte not generated\" + ex.InnerException.Message, ex);;\n";
            result += "}\n";
            
            result += "return result;\n";
            result += "}\n";
            result += "\n";
            return  result;

        }
        private string genManagerExport() {
            String result = "\n";
            result = result + "public FileStream export" + name + "ToFile(){\n";
            result += "FileStream result = null;;\n";


            result += "try\n{";
            result = result + "result = " + name + "Accessor.export" + name + "ToFile();\n";
            result += "}\n";
            result += "catch (Exception ex)\n";
            result += "{\n";
            result = result + "throw new ApplicationException(\"" + name + " export not generated\" + ex.InnerException.Message, ex);;\n";
            result += "}\n";
            
            result += "return result;\n";
            result += "}\n";
            result += "\n";
            return result;


        }
        private string genManagerCount() {
            string retrieveAll = "";
            retrieveAll = retrieveAll + "\npublic int count" + name + "(";
            string comma = "";
            foreach (Column r in columns)
            {
                if (r.foreign_key != "")
                {
                    retrieveAll += comma + r.data_type.toCSharpDataType() + " " + r.column_name;
                    comma = ",";
                }
            }
            retrieveAll += "){\n";
            retrieveAll = retrieveAll + "int result =0;\n";
            retrieveAll += "try{\n";
            retrieveAll = retrieveAll + "result = _" + name.ToLower() + "Accessor.count" + name + "("; 
            comma = ""; 
            foreach (Column r in columns)
            {
                if (r.foreign_key != "")
                {
                    retrieveAll += comma +  r.column_name;
                    comma = ",";
                }
            }
            
            retrieveAll += ");\n";
            retrieveAll += "if (result == 0){\n";
            retrieveAll = retrieveAll + "throw new ApplicationException(\"Unable to retrieve " + name + "s\" );\n";
            retrieveAll += "}\n";
            retrieveAll += "}\n";
            retrieveAll += "catch (Exception ex){\n";
            retrieveAll += "throw ex;\n";
            retrieveAll += "}\n";
            retrieveAll += "return result;\n}\n";
            return retrieveAll;

        }
        /// Generates a c# data object with markup reflecting min and max length, etc.
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string representing c# data object </returns>
        public String gen_CSharpDataObject()
        {
            _ = commentBox.GenXMLClassComment(this, XMLClassType.CSharpDataObject);
            string output = "public class " + name + "\n{\n";
            

            int count = 0;
            foreach (Column r in columns)
            {
                String DataAnnotationRequired = "";
                String DataAnnotationLength = "";
                if (r.nullable == 'n' || r.nullable == 'N')
                {
                    DataAnnotationRequired = "[Required(ErrorMessage = \"Please enter " + r.column_name.bracketStrip() + " \")]\n";
                }
                if (r.length != 0)
                {
                    DataAnnotationLength = "[MaxLength(" + r.length + "),MinLength(3)]\n";
                }
                else {
                    DataAnnotationLength = "[Range(0, 100)]\n";
                }
                String DataAnnotationDisplayName = "[Display(Name = \"" + r.column_name.bracketStrip() + "\")]\n";
                String add = "public " + r.data_type.toCSharpDataType() + " " + r.column_name.bracketStrip() + "{ set; get; }\n";
                output = output + DataAnnotationDisplayName + DataAnnotationRequired + DataAnnotationLength + add;
                count++;
            }
            output += "\n}\n";
            if (foreign_keys.Count > 0)
            {
                output = output + "public class " + name + "VM: " + name + "\n{\n";
                foreach (Column r in columns)
                {
                    if (r.foreign_key == "y" || r.foreign_key == "Y")
                    {
                        int dotPosition = r.references.IndexOf('.');
                        string keytype = r.references.Substring(0, dotPosition);
                        String DataAnnotationRequired = "";
                        String DataAnnotationLength = "";
                        if (r.nullable == 'n' || r.nullable == 'N')
                        {
                            DataAnnotationRequired = "[Required(ErrorMessage = \"Please enter " + r.column_name + " \")]\n";
                        }
                        if (r.length != 0)
                        {
                            DataAnnotationLength = "[StringLength(" + r.length + ")]\n";
                        }
                        String DataAnnotationDisplayName = "[Display(Name = \"" + r.column_name + "\")]\n";
                        output = output + "public " + keytype + " _" + keytype.ToLower() + "{ get ; set; }\n";
                    }
                }
                output += "}\n";
            }
            output += genCSharpConstructor();
            return output;
        }
        private string genCSharpConstructor() {
            //default
            string defaultConstructor = "\npublic " + name + "(){}\n";
            //param
            string ParamConsctructor = "\npublic " + name + "(";
            string comma = "";
            foreach (Column r in columns)
            {
                ParamConsctructor += comma + r.data_type.toCSharpDataType() + " " + r.column_name.ToLower();
                comma = ", ";
            }
            ParamConsctructor += ") {\n";
            foreach (Column r in columns)
            {
                ParamConsctructor += "\n" + r.column_name + " = " + r.column_name.ToLower() + ";";
            }
            ParamConsctructor += "\n}\n";

            //param2

            string ParamConstructor2 = "\npublic " + name + "(";
            comma = "";
            foreach (Column r in columns)
            {
                if (r.primary_key == 'y' || r.primary_key == 'Y' || r.unique == 'y' || r.unique == 'Y')
                {

                    ParamConstructor2 = ParamConstructor2 + comma + r.data_type.toCSharpDataType() + " " + r.column_name.ToLower();
                    comma = ", ";
                }
            }

            ParamConstructor2 += ") {\n";
            foreach (Column r in columns)
            {
                if (r.primary_key == 'y' || r.primary_key == 'Y' || r.unique == 'y' || r.unique == 'Y')
                {
                    ParamConstructor2 +=  "\n." + r.column_name + " = " + r.column_name.ToLower() + ";";
                }
            }
            ParamConstructor2 += "\n}\n";

            string result = defaultConstructor + ParamConsctructor + ParamConstructor2;
            return result;
        }
        //Not implemented
        public String gen_functions()
        {
            string x = " ";
            return x;
        }
        /// <summary>
        /// Generates a header for the data access layer accessor, 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is c# code for the header of the data access layer </returns>
        private string gencCSharpDatabaseAccessorClassHeader()
        {
            _ = commentBox.GenXMLClassComment(this, XMLClassType.CSharpAccessor);
            string header = "public class " + name + "Accessor : I" + name + "Accessor {\n";
            return header;
        }
        /// <summary>
        /// Generates a header for functions of the  accessor, 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is c# code for the header of the data access layer methods </returns>
        private String genSPHeaderA(string commandText)
        {
            //for update, insert, delete
            string output = "int rows = 0;\n"
+ "// start with a connection object\n"
+ "var conn = SqlConnectionProvider.GetConnection();\n"
+ "// set the command text\n"
+ "var commandText = \"" + commandText + "\";\n"
+ "// create the command object\n"
+ "var cmd = new SqlCommand(commandText, conn);\n"
+ "// set the command type\n"
+ "cmd.CommandType = CommandType.StoredProcedure;\n"
+ "// we need to add parameters to the command\n";
            return output;
        }
        /// <summary>
        /// Generates a header for functions of the  accessor, 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is c# code for the header of the data access layer methods </returns>
        private String genSPHeaderB(string DataObject, string commandText)
        {
            //for single data object
            string output = DataObject + " output = new " + DataObject + "();\n"
+ "// start with a connection object\n"
+ "var conn = SqlConnectionProvider.GetConnection();\n"
+ "// set the command text\n"
+ "var commandText = \"" + commandText + "\";\n"
+ "// create the command object\n"
+ "var cmd = new SqlCommand(commandText, conn);\n"
+ "// set the command type\n"
+ "cmd.CommandType = CommandType.StoredProcedure;\n"
+ "// we need to add parameters to the command\n";
            return output;
        }
        /// <summary>
        /// Generates a header for functions of the  accessor, 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is c# code for the header of the data access layer methods </returns>
        private String genSPHeaderC(string DataObject, string commandText)
        {
            //for list of data object
            string output = "List<" + DataObject + "> output = new " + "List<" + DataObject + ">();\n"
+ "// start with a connection object\n"
+ "var conn = SqlConnectionProvider.GetConnection();\n"
+ "// set the command text\n"
+ "var commandText = \"" + commandText + "\";\n"
+ "// create the command object\n"
+ "var cmd = new SqlCommand(commandText, conn);\n"
+ "// set the command type\n"
+ "cmd.CommandType = CommandType.StoredProcedure;\n"
+ "// There are no parameters to set or add\n";
            return output;
        }
        /// <summary>
        /// Generates a footer for functions of the  accessor, 
        /// Jonathan Beck
        /// </summary>
        /// <param name="mode"></param>
        /// <returns>A string that is c# code for the footer of the data access layer methods </returns>
        private string genSPfooter(int mode)
        {
            string returntype = "output";
            if (mode == 2||mode==4) { returntype = "rows"; }
            string output = " \ncatch (Exception ex)\n"
+ "{\n"
+ "    throw ex;\n"
+ "}\n"
+ "finally\n"
+ "{\n"
+ "    conn.Close();\n"
+ "}\n";
            if (mode == 4) {
                output += "}\n";
            }
output+="return " + returntype + ";\n}\n";
            return output;
        }
        /// <summary>
        /// Generates an method for the data access layer add method for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is c# code for adding to the database </returns>
        private string genCharpDatabaseAccessorAdd()
        {
            string createThing = commentBox.GenXMLMethodComment(this, XML_Method_Type.CSharp_Accessor_Add);
            createThing += "\npublic int add" + name + "(" + name + " _" + name.ToLower();
            createThing += "){\n";
            createThing += genSPHeaderA("sp_insert_" + name);
            //add parameters
            
            foreach (Column r in columns)
            {
                if (r.increment == 0)
                {
                    createThing = createThing + "cmd.Parameters.Add(\"@" + r.column_name.bracketStrip() + "\", SqlDbType." + r.data_type.bracketStrip().toSQLDBType(r.length) + ");\n";
                }
            }
            //setting parameters
            createThing += "\n //We need to set the parameter values\n";
            foreach (Column r in columns)
            {
                if (r.increment == 0)
                {
                    createThing = createThing + "cmd.Parameters[\"@" + r.column_name.bracketStrip() + "\"].Value = " + "_" + name.ToLower() + "." + r.column_name.bracketStrip() + ";\n";
                }
            }
            //excute the quuery
            createThing += "try \n { \n //open the connection \n conn.Open();  ";
            createThing += "//execute the command and capture result\n";
            createThing += "rows = cmd.ExecuteNonQuery();\n}\n";
            //capture reuslts
            createThing += "";
            //cath block and onwards
            createThing += genSPfooter(2);
            return createThing;
        }

        private string genCSharpDatabaseAccessorBatchAdd() {
            string createThing = commentBox.GenXMLMethodComment(this, XML_Method_Type.CSharp_Accessor_Add);
            createThing += "\npublic int addBatchOf" + name + "(List<" + name + "> _" + name.ToLower();
            createThing += "s){\n";
            createThing += genSPHeaderA("sp_insert_" + name);
            createThing += "foreach (" + name + " _" + name.ToLower() + " in " + name + "s){\n";
            //add parameters
            foreach (Column r in columns)
            {
                if (r.increment == 0)
                {
                    createThing = createThing + "cmd.Parameters.Add(\"@" + r.column_name.bracketStrip() + "\", SqlDbType." + r.data_type.bracketStrip().toSQLDBType(r.length) + ");\n";
                }
            }
            //setting parameters
            createThing += "\n //We need to set the parameter values\n";
            foreach (Column r in columns)
            {
                if (r.increment == 0)
                {
                    createThing = createThing + "cmd.Parameters[\"@" + r.column_name.bracketStrip() + "\"].Value = " + "_" + name.ToLower() + "." + r.column_name.bracketStrip() + ";\n";
                }
            }
            //excute the quuery
            createThing += "try \n { \n //open the connection \n conn.Open();  ";
            createThing += "//execute the command and capture result\n";
            createThing += "rows += cmd.ExecuteNonQuery();\n}\n";
            //capture reuslts
            createThing += "\n";
            //cath block and onwards
            createThing += genSPfooter(4);
            return createThing;


        }

        /// <summary>
        /// Generates an method for the data access layer retrieve by PK method for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is c# code for retreiving by PK from the database </returns>
        private string genCharpDatabaseAccessorretrieveByKey()
        {
            string retrieveThing = commentBox.GenXMLMethodComment(this, XML_Method_Type.CSharp_Accessor_retrieve_By_PK);
            int count = 0;
            string comma = "";
            retrieveThing += "\npublic " + name + " select" + name + "ByPrimaryKey(";
            foreach (Column r in columns)
            {
                if (r.primary_key.Equals('y') || r.primary_key.Equals('Y'))
                {
                    if (count > 0) { comma = "  "; }
                    String add = comma + r.data_type.toCSharpDataType() + " " + r.column_name.bracketStrip();
                    retrieveThing += add;
                    count++;
                }
            }
            retrieveThing += "){\n";
            retrieveThing += genSPHeaderB(name, "sp_retrieve_by_pk_" + name);
            //add parameters
            foreach (Column r in columns)
            {
                if (r.primary_key.Equals('y') || r.primary_key.Equals('Y'))
                {
                    retrieveThing = retrieveThing + "cmd.Parameters.Add(\"@" + r.column_name.bracketStrip() + "\", SqlDbType." + r.data_type.bracketStrip().toSQLDBType(r.length) + ");\n";
                }
            }
            //setting parameters
            retrieveThing += "\n //We need to set the parameter values\n";
            foreach (Column r in columns)
            {
                if (r.primary_key.Equals('y') || r.primary_key.Equals('Y'))
                {
                    retrieveThing = retrieveThing + "cmd.Parameters[\"@" + r.column_name.bracketStrip() + "\"].Value = " + r.column_name.bracketStrip() + ";\n";
                }
            }
            //excute the quuery
            retrieveThing += "try \n { \n //open the connection \n conn.Open();  ";
            retrieveThing += "//execute the command and capture result\n";
            retrieveThing += "var reader = cmd.ExecuteReader();\n";
            //capture reuslts
            retrieveThing += "//process the results\n";
            retrieveThing += "if (reader.HasRows)\n if (reader.Read())\n{";
            count = 0;
            foreach (Column r in columns)
            {
                retrieveThing = getCSharpOrdinal(r);
                count++;
            }
            foreach (foreignKey fk in data_tables.all_foreignKey)
            {
                if (fk.mainTable.Equals(name))
                {
                    foreach (table t in data_tables.all_tables)
                    {
                        if (t.name.Equals(fk.referenceTable))
                        {
                            retrieveThing = retrieveThing + "Output." + t.name + "= new" + t.name + "();\n";
                            foreach (Column r in t.columns)
                            {
                                if (count > 0) { comma = ","; }
                                retrieveThing += getCSharpOrdinal(t, r);
                                count++;
                            }
                        }
                    }
                }
            }
            retrieveThing += "\n}\n";
            retrieveThing = retrieveThing + "else \n { throw new ArgumentException(\"" + name + " not found\");\n}\n}";
            //cath block and onwards
            retrieveThing += genSPfooter(0);
            return retrieveThing;
        }
        /// <summary>
        /// Generates an method for the data access layer retrieve by all method for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is c# code for retreving all from the database 
        private string genCharpDatabaseAccessorretrieveAll()
        {
            string retrieveAllThing = commentBox.GenXMLMethodComment(this, XML_Method_Type.CSharp_Manager_retrieve_All_Two_Param);
            retrieveAllThing += "\npublic List<" + name + "> selectAll" + name + "(int limit, int offset";
            foreach (Column r in columns)
            {
                if (r.foreign_key != "")
                {
                    retrieveAllThing += "," + r.data_type.toCSharpDataType() + " " + r.column_name;
                }
            }

            retrieveAllThing += "){\n";
            retrieveAllThing += genSPHeaderC(name, "sp_retrieve_by_all_" + name);
            //no paramaters to set or add
            retrieveAllThing += "cmd.Parameters.Add(\"@limit SqlDbType.Int);\n";
            retrieveAllThing += "cmd.Parameters.Add(\"@offset SqlDbType.Int);\n";
            foreach (Column r in columns)
            {
                if (r.foreign_key != "")
                {
                    retrieveAllThing += "cmd.Parameters.Add(\"@" + r.column_name.bracketStrip() + "\", SqlDbType." + r.data_type.bracketStrip().toSQLDBType(r.length) + ");\n";
                }
            }
            retrieveAllThing += "cmd.Parameters[\"@limit\"].Value = limit ;\n";
            retrieveAllThing += "cmd.Parameters[\"@offset\"].Value = offset ;\n";

            foreach (Column r in columns)
            {
                if (r.foreign_key != "")
                {
                    retrieveAllThing += "cmd.Parameters[\"@" + r.column_name.bracketStrip() + "\"].Value = " + r.column_name.bracketStrip() + ";\n";
                }
            }
            //excute the quuery
            retrieveAllThing += "try \n { \n //open the connection \n conn.Open();  ";
            retrieveAllThing += "//execute the command and capture result\n";
            retrieveAllThing += "var reader = cmd.ExecuteReader();\n";
            //capture reuslts
            retrieveAllThing += "//process the results\n";
            retrieveAllThing += "if (reader.HasRows)\n while (reader.Read())\n{";
            retrieveAllThing = retrieveAllThing + "var _" + name + "= new " + name + "();\n";
            int count = 0;
            foreach (Column r in columns)
            {
                retrieveAllThing += getCSharpOrdinal(r);
                count++;
            }
            foreach (foreignKey fk in data_tables.all_foreignKey)
            {
                if (fk.mainTable.Equals(name))
                {
                    foreach (table t in data_tables.all_tables)
                    {
                        if (t.name.Equals(fk.referenceTable))
                        {
                            retrieveAllThing = retrieveAllThing + "Output." + t.name + "= new" + t.name + "();\n";
                            foreach (Column r in t.columns)
                            {
                                if (count > 0)
                                {
                                }
                                retrieveAllThing += getCSharpOrdinal(t, r);
                                count++;
                            }
                        }
                    }
                }
            }
            retrieveAllThing = retrieveAllThing + "output.Add(_" + name + ");";
            retrieveAllThing += "\n}\n}";
            //cath block and onwards
            retrieveAllThing += genSPfooter(0);
            return retrieveAllThing;
        }
        /// <summary>
        /// Generates an method for the data access layer retrieve by FK method for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is c# code for retereiving by FK from the database
        private string genCharpDatabaseAccessorretrievefk()
        {
            string retrieveAllThing = "";
            foreach (Column q in columns)
            {
                if (q.references != "")
                {
                    string[] parts = q.references.Split('.');
                    string fk_table = parts[0];
                    string fk_name = parts[1];
                    int count = 0;
                    retrieveAllThing += commentBox.GenXMLMethodComment(this, XML_Method_Type.CSharp_Accessor_retrieve_By_FK_Two_Param);
                    retrieveAllThing += "\npublic List<" + name + "> select" + name + "by" + fk_table + "(" + q.data_type.toCSharpDataType() + " " + fk_name + ",int limit, int offset){\n";
                    retrieveAllThing += genSPHeaderC(name, "sp_retrieve_" + name + "_by_" + q.column_name.bracketStrip());
                    //no paramaters to set or add
                    retrieveAllThing += "cmd.Parameters.Add(\"@" + q.column_name.bracketStrip() + "\", SqlDbType." + q.data_type.bracketStrip().toSQLDBType(q.length) + ");\n";
                    retrieveAllThing += "cmd.Parameters.Add(\"@limit SqlDbType.Int);\n";
                    retrieveAllThing += "cmd.Parameters.Add(\"@offset SqlDbType.Int);\n";
                    retrieveAllThing += "cmd.Parameters[\"@" + q.column_name.bracketStrip() + "\"].Value = " + fk_name + " ;\n";
                    retrieveAllThing += "cmd.Parameters[\"@limit\"].Value = limit ;\n";
                    retrieveAllThing += "cmd.Parameters[\"@offset\"].Value = offset ;\n";
                    //excute the quuery
                    retrieveAllThing += "try \n { \n //open the connection \n conn.Open();  ";
                    retrieveAllThing += "//execute the command and capture result\n";
                    retrieveAllThing += "var reader = cmd.ExecuteReader();\n";
                    //capture reuslts
                    retrieveAllThing += "//process the results\n";
                    retrieveAllThing += "if (reader.HasRows)\n while (reader.Read())\n{";
                    retrieveAllThing = retrieveAllThing + "var _" + name + "= new " + name + "();\n";
                    count = 0;
                    foreach (Column r in columns)
                    {
                        retrieveAllThing += getCSharpOrdinal(r);
                        count++;
                    }
                    foreach (foreignKey fk in data_tables.all_foreignKey)
                    {
                        if (fk.mainTable.Equals(name))
                        {
                            foreach (table t in data_tables.all_tables)
                            {
                                if (t.name.Equals(fk.referenceTable))
                                {
                                    retrieveAllThing = retrieveAllThing + "Output." + t.name + "= new" + t.name + "();\n";
                                    foreach (Column r in t.columns)
                                    {
                                        if (count > 0)
                                        {
                                        }
                                        retrieveAllThing += getCSharpOrdinal(t, r);
                                        count++;
                                    }
                                }
                            }
                        }
                    }
                    retrieveAllThing = retrieveAllThing + "output.Add(_" + name + ");";
                    retrieveAllThing += "\n}\n}";
                    //cath block and onwards
                    retrieveAllThing += genSPfooter(0);
                }
            }
            return retrieveAllThing;
        }
        /// <summary>
        /// Generates an method for the data access layer update method method for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is c# code for updating a record on the database
        private string genCharpDatabaseAccessorUpdate()
        {
            string updateThing = commentBox.GenXMLMethodComment(this, XML_Method_Type.CSharp_Accessor_Update);
            updateThing += "\npublic int update" + name + "(";
            updateThing = updateThing + name + " _old" + name + " , " + name + " _new" + name;
            updateThing += "){\n";
            updateThing += genSPHeaderA("sp_update_" + name);
            //add parameters
            foreach (Column r in columns)
            {
                updateThing = updateThing + "cmd.Parameters.Add(\"@old" + r.column_name.bracketStrip() + "\", SqlDbType." + r.data_type.bracketStrip().toSQLDBType(r.length) + ");\n";
                if (r.primary_key != 'y' && r.primary_key != 'Y' && r.increment == 0)
                {
                    updateThing = updateThing + "cmd.Parameters.Add(\"@new" + r.column_name.bracketStrip() + "\", SqlDbType." + r.data_type.bracketStrip().toSQLDBType(r.length) + ");\n";
                }
            }
            //setting parameters
            updateThing += "\n //We need to set the parameter values\n";
            foreach (Column r in columns)
            {
                updateThing = updateThing + "cmd.Parameters[\"@old" + r.column_name.bracketStrip() + "\"].Value = _old" + name + "." + r.column_name.bracketStrip() + ";\n";
                if (r.primary_key != 'y' && r.primary_key != 'Y')
                {
                    updateThing = updateThing + "cmd.Parameters[\"@new" + r.column_name.bracketStrip() + "\"].Value = _new" + name + "." + r.column_name.bracketStrip() + ";\n";
                }
            }
            //excute the quuery
            updateThing += "try \n { \n //open the connection \n conn.Open();  ";
            updateThing += "//execute the command and capture result\n";
            updateThing += "rows = cmd.ExecuteNonQuery();\n";
            updateThing += "if (rows == 0) {\n //treat failed update as exception \n ";
            updateThing += "throw new ArgumentException(\"invalid values, update failed\");\n}\n}";
            //capture reuslts
            updateThing += "";
            //cath block and onwards
            updateThing += genSPfooter(2);
            return updateThing;
        }
        /// <summary>
        /// Generates an method for the data access layer delete method for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is c# code for deleting from the database
        private string genCharpDatabaseAccessorDelete()
        {
            string deleteThing = commentBox.GenXMLMethodComment(this, XML_Method_Type.CSharp_Accessor_Delete);
            deleteThing += "\npublic int delete" + name + "(" + name + " _" + name.ToLower() + "){\n";
            deleteThing += genSPHeaderA("sp_delete_" + name);
            //add parameters bit
            foreach (Column r in columns)
            {
                deleteThing = deleteThing + "cmd.Parameters.Add(\"@" + r.column_name.bracketStrip() + "\", SqlDbType." + r.data_type.bracketStrip().toSQLDBType(r.length) + ");\n";
            }
            //setting parameters
            deleteThing += "\n //We need to set the parameter values\n";
            foreach (Column r in columns)
            {
                deleteThing = deleteThing + "cmd.Parameters[\"@" + r.column_name.bracketStrip() + "\"].Value = " + "_" + name.ToLower() + "." + r.column_name.bracketStrip() + ";\n";
            }
            deleteThing += "try\n { \n conn.Open();\n rows = cmd.ExecuteNonQuery();";
            deleteThing += "if (rows == 0){\n";
            deleteThing += "//treat failed delete as exepction\n throw new ArgumentException(\"Invalid Primary Key\");\n}\n}";
            deleteThing += genSPfooter(2);
            return deleteThing;
        }
        /// <summary>
        /// Generates an method for the data access layer undelete method for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is c# code for undeleting from the database
        private string genCharpDatabaseAccessorUndelete()
        {
            string deleteThing = commentBox.GenXMLMethodComment(this, XML_Method_Type.CSharp_Accessor_Undelete);
            deleteThing = deleteThing + "\n public int undelete" + name + "(" + name + " _" + name.ToLower() + "){\n";
            deleteThing += genSPHeaderA("sp_undelete_" + name);
            //add parameters bit
            //add parameters
            foreach (Column r in columns)
            {
                deleteThing = deleteThing + "cmd.Parameters.Add(\"@" + r.column_name.bracketStrip() + "\", SqlDbType." + r.data_type.bracketStrip().toSQLDBType(r.length) + ");\n";
            }
            //setting parameters
            deleteThing += "\n //We need to set the parameter values\n";
            foreach (Column r in columns)
            {
                deleteThing = deleteThing + "cmd.Parameters[\"@" + r.column_name.bracketStrip() + "\"].Value = " + "_" + name.ToLower() + "." + r.column_name.bracketStrip() + ";\n";
            }
            deleteThing += "try\n { \n conn.Open();\n rows = cmd.ExecuteNonQuery();";
            deleteThing += "if (rows == 0){\n";
            deleteThing += "//treat failed delete as exepction\n throw new ArgumentException(\"Invalid Primary Key\");\n}\n}";
            deleteThing += genSPfooter(2);
            return deleteThing;
        }
        //Generates the get distinct for drop downs componoent of the accessor
        private string genCharpDatabaseAccessorDistinct()
        {
            string retrieveAllThing = "";
            List<foreignKey> all_foreignKey = data_tables.all_foreignKey;
            foreach (foreignKey fk in all_foreignKey)
            {
                if (fk.mainTable == name)
                {
                    retrieveAllThing += commentBox.GenXMLMethodComment(this, XML_Method_Type.CSharp_Accessor_Select_Distinct_For_Dropdown);
                    retrieveAllThing = retrieveAllThing + "public List<String> selectDistinct" + fk.referenceTable + "ForDropDown(){\n";
                    retrieveAllThing += genSPHeaderC("String", "sp_select_distinct_and_active_" + fk.referenceTable + "_for_dropdown");
                    //no paramaters to set or add
                    //excute the quuery
                    retrieveAllThing += "try \n { \n //open the connection \n conn.Open();  ";
                    retrieveAllThing += "//execute the command and capture result\n";
                    retrieveAllThing += "var reader = cmd.ExecuteReader();\n";
                    //capture reuslts
                    retrieveAllThing += "//process the results\n";
                    retrieveAllThing += "if (reader.HasRows)\n while (reader.Read())\n{";
                    retrieveAllThing = retrieveAllThing + "String _" + fk.referenceTable + "= reader.Get" + columns[0].data_type.toSqlReaderDataType() + "(0);\n";
                    retrieveAllThing = retrieveAllThing + "output.Add(_" + fk.referenceTable + ");";
                    retrieveAllThing += "\n}\n}";
                    //cath block and onwards
                    retrieveAllThing += genSPfooter(0);
                }
            }
            return retrieveAllThing;
        }
        private string genCharpDatabaseAccessorWrite() {
            string result = "public int write" + name + "sToFile(List<"+name+"> "+name+"s,string path){\n";
            result += "int result=0;\n";
            result += "try\n{";
            result += "StreamWriter streamWriter = new StreamWriter(path);\n";
            result += "foreach (" + name + " " + name.ToLower() + " in " + name + "s){\n";
            result += "streamWriter.WriteLine(\n";
            string tab = "";
            foreach (Column r in columns) {
                result += tab + name.ToLower() + ".get" + r.column_name + "()\n";
                tab = "+\"\\t\"+";
            }
            result += "}\n";
            result += "streamWriter.Close();\n";
            result += "}\n";
            result += "catch (Exception ex)\n";
            result += "{\n";
            result = result + "throw new ApplicationException(\"" + name + "s not written to File\" + ex.InnerException.Message, ex);;\n";
            result += "}\n";
            result += "return result;\n";
            result += "}\n";
            result += "\n";

            return result;
        }

        private string genCharpDatabaseAccessorRead() {
            string result = "public List<"+name+"> read"+name+"sFromFile(string path){\n";
            result += "List<name> " + name + "s = new List<" + name + ">();\n";
            result += "try\n{";
            result += "StreamReader fileReader = new StreamReader(path);\n";
            result += "char[] separator = { '\t' };\n";
            result += "while (fileReader.EndOfStream == false)\n";
            result += "{\n";
            result += "string line = fileReader.ReadLine();\n";
            result += "string[] parts;\n";
            result += " if (line.Length > "+(columns.Count-1)+") //is this line long enough?\n";
            result += "{\n";
            result += "parts = line.Split(separator);\n";
            result += "if (parts.Count() > "+ (columns.Count - 1) + ")  //are all properties present?\n";
            int i = 0;
            foreach (Column r in columns) {
                if (r.increment == 0)
                {
                    result += r.data_type.toCSharpDataType()+" " + r.column_name.ToLower() + "=parts[" + i + "]\n";
                    i++;
                }
            }
            result += name+" "+name.ToLower()+" = new "+ name+"(";
            string comma = "";
            foreach (Column r in columns)
            {
                result += comma+r.column_name.ToLower();
                comma = ", ";
            }
            result += ");\n";
            result += name+"s.Add("+name.ToLower()+");\n";
            result += "}\n";
            result += "}\n";
            result += "}\n";
            result += "fileReader.Close();\n";
            result += "}\n";
            result += "catch (Exception ex)\n";
            result += "{\n";
            result = result + "throw new ApplicationException(\"" + name + "s not read fom File\" + ex.InnerException.Message, ex);;\n";
            result += "}\n";
            result += "return "+name+"s;\n";
            result += "}\n";
            result += "\n";

            return result;

        }

        private string genCSharpDatabaseAccessorCount()
        {
            string result = "";
            result += "";
            result += "";
            result += "";
            result += "";
            result += "";
            result += "";
            return result;

        }

        private string genCSharpDatabaseAccessoTemplate()
        {
            string result = "";
            result += "";
            result += "";
            result += "";
            result += "";
            result += "";
            result += "";
            return result;
        }

        private string genCSharpDatabaseAccessorExport()
        {
            string result = "";
            result += "";
            result += "";
            result += "";
            result += "";
            result += "";
            result += "";
            return result;
        }

        /// <summary>
        /// Generates a rudamentary xaml window for creating records for  this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is xaml window for adding to this table
        public string genXAMLWindow()
        {
            string comment = commentBox.genCommentBox(name, Component_Enum.CSharp_XAML_Window);
            string WindowCode = comment;
            int rows = columns.Count + 3;
            int width = 4;
            int height = rows * 50 + 100;
            WindowCode += "< !--set window height to " + height + "-- >\n";
            WindowCode += "< Menu Grid.Row = \"0\" Padding = \"20px, 0px\" >\n";
            WindowCode += " < MenuItem x: Name = \"mnuFile\" Header = \"File\" >\n";
            WindowCode += "< MenuItem x: Name = \"mnuExit\" Header = \"Exit\" Click = \"mnuExit_Click\" />\n";
            WindowCode += "</ MenuItem >\n";
            WindowCode += " < MenuItem x: Name = \"mnuHelp\" Header = \"Help\" >\n";
            WindowCode += "< MenuItem x: Name = \"mnuAbout\" Header = \"About\" />\n";
            WindowCode += "</ MenuItem > \n </Menu>";
            WindowCode += "<Grid>\n";
            WindowCode += "<Grid.RowDefinitions>\n";
            for (int i = 0; i < rows; i++)
            {
                WindowCode += "<RowDefinition Height=\"50\"/>\n";
            }
            WindowCode += "</Grid.RowDefinitions>\n";
            WindowCode += "<Grid.ColumnDefinitions>\n";
            for (int i = 0; i < width; i++)
            {
                WindowCode += "<ColumnDefinition />\n";
            }
            WindowCode += "</Grid.ColumnDefinitions>\n";
            for (int i = 0; i < columns.Count; i++)
            {
                WindowCode += "< Label x:Name = \"lbl" + name + columns[i].column_name.bracketStrip() + "\" Grid.Column = \"1\" Grid.Row = \"" + (i + 1) + "\" Content = \"" + columns[i].column_name + " \" />\n";
                if (columns[i].foreign_key == "y" || columns[i].foreign_key == "Y")
                {
                    WindowCode += "< ComboBox x:Name = \"cbx" + name + columns[i].column_name.bracketStrip() + "\" Grid.Column = \"2\" Grid.Row = \"" + (i + 1) + "\" />\n";
                }
                else if (columns[i].column_name.bracketStrip().ToLower() == "active")
                {
                    WindowCode += "< CheckBox x:Name = \"chk" + name + columns[i].column_name.bracketStrip() + "\" Grid.Column = \"2\" Grid.Row = \"" + (i + 1) + "\" />\n";
                }
                else
                {
                    WindowCode += "< TextBox x:Name = \"tbx" + name + columns[i].column_name.bracketStrip() + "\" Grid.Column = \"2\" Grid.Row = \"" + (i + 1) + "\" />\n";
                }
            }
            WindowCode += "<Button x:Name=\"btnUpdate" + name + "\" Grid.Column=\"2\" Grid.Row=\"" + (rows - 1) + "\" Content=\"Edit " + name + "\" Height=\"40px\" Width=\"200px\"/>\n";
            WindowCode += "<Button x:Name=\"btnAdd" + name + "\" Grid.Column=\"3\" Grid.Row=\"" + (rows - 1) + "\" Content=\"Add " + name + "\" Height=\"40px\" Width=\"200px\"/>\n";
            WindowCode += "< StatusBar Grid.Row =" + rows + ">\n";
            WindowCode += "< StatusBarItem x: Name = \"statMessage\" Content = \"Welcome, please login to continue\" Padding = \"20px, 0px\" />\n </ StatusBar >\n </Grid>\n";
            return WindowCode;
        }
        /// <summary>
        /// Generates a rudamentary c# codebase for controlling the XAML window for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is c# to control the XAML window for this table
        public string genWindowCSharp()
        {
            string result = "";
            result += commentBox.genCommentBox(name, Component_Enum.CSharp_Window_Control);
            result += genStaticVariables();
            result += genConstructor();
            result += genWinLoad();
            result += genAddButton();
            result += genEditButton();
            return result;
        }
        //Generates the add button for the XAML window
        private string genAddButton()
        {
            string result = "";
            //else if (columns[i].column_name.bracketStrip().ToLower() == "active") 
            // else   //this means textbox
            result = result + "private void btnAdd" + name + "_click(object sender, RoutedEventArgs e)\n";
            result = result + "if((string)btnAdd" + name + ".Content == \"Add " + name + "\")\n";
            foreach (Column c in columns)
            {
                if (c.foreign_key == "y" || c.foreign_key == "Y")
                {
                    result = result + "cbx" + name + c.column_name.bracketStrip() + ".IsEnabled=true;\n";
                    result = result + "cbx" + name + c.column_name.bracketStrip() + ".SelectedItem=null;\n";
                }
                else if (c.column_name.bracketStrip().ToLower() == "active")
                {
                    result = result + "chk" + name + c.column_name.bracketStrip() + ".IsEnabled=true;\n";
                    result = result + "chk" + name + c.column_name.bracketStrip() + ".IsChecked=true;\n";
                }
                else
                {
                    result = result + "tbx" + name + c.column_name.bracketStrip() + ".IsEnabled=true;\n";
                    result = result + "tbx" + name + c.column_name.bracketStrip() + ".Text=\"\";\n";
                }
            }
            result += "else\n{\nif (validInputs())\n{\n";
            result = result + name + " new" + name + " = new " + name + "();\n";
            foreach (Column c in columns)
            {
                if (c.foreign_key == "y" || c.foreign_key == "Y")
                {
                    result = result + "new" + name + "." + c.column_name.bracketStrip() + " = " + "cbx" + name + c.column_name.bracketStrip() + ".Text;\n";
                }
                else if (c.column_name.bracketStrip().ToLower() == "active")
                {
                    result = result + "new" + name + "." + c.column_name.bracketStrip() + " =  true;\n";
                }
                else
                {
                    result = result + "new" + name + "." + c.column_name.bracketStrip() + " = " + "tbx" + name + c.column_name.bracketStrip() + ".Text;\n";
                }
            }
            result += "try\n{\n";
            result = result + "bool result = _" + name.Substring(0, 1).ToLower() + "m.add" + name + "(new" + name + ");\n";
            result += "if (result)\n{\n MessageBox.Show(\"added!\");\n";
            result += "this.DialogResult=true;\n}\n";
            result += "else \n { throw new ApplicationException();\n}\n}\n";
            result += "catch (Exception ex)\n{\n MessageBox.Show(\"add failed\");";
            result += "this.DialogResult=false\n}\n}";
            result += "\nelse{\nMessageBox.Show(\"invalid inputs\");\n}\n}\n}";
            return result;
        }
        //Generates the edit button of the window
        private string genEditButton()
        {
            string result = "";
            return result;
        }
        //Generates the input validator for the window
        private string genValidInputs()
        {
            string result = "";
            return result;
        }
        //Generates the input constructor for the window
        private string genConstructor()
        {
            string result = "public " + name + "AddEditDelete(" + name + " " + name.Substring(0, 1).ToLower();
            result += ")\n{\n";
            result += " InitializeComponent();\n";
            result = result + "_" + name.ToLower() + "=" + name.Substring(0, 1).ToLower() + ";\n";
            result = result + "_" + name.Substring(0, 1).ToLower() + "m = new" + name + "Manager();\n}\n";
            return result;
        }
        //Generates the input winload events for the window
        private string genWinLoad()
        {
            string result = "";
            return result;
        }
        //Generates the get static variables method for the window
        private string genStaticVariables()
        {
            _ = "public " + name + " _" + name.ToLower() + "= null;\n";
            string result = "public " + name + "Manager+ _" + name.Substring(0, 1) + "m = null;\n";
            return result;
        }
        /// <summary>
        /// Generates a rudamentary Java data object for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is Java code for this object.

        public string genJavaModel()
        {
            string result = "";
            result += genJavaHeader();
            result += genJavaInstanceVariables();
            result += genJavaContructor();
            result += genJavaSetterAndGetter();
            result += genJavaComparable();
            result += genJavaFooter();
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string genJavaModelNM()
        {
            string result = "";
            result += genJavaVMHeader();
            result += genJavaVMInstanceVariables();
            result += genJavaVMContructor();
            result += genJavaVMSetterAndGetter();
            result += genJavaFooter();
            return result;
        }
        /// <summary>
        /// Generates a the header for a rudamentary Java data object for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is the header Java code for this object.
        private string genJavaHeader()
        {
            string result = commentBox.GenXMLClassComment(this, XMLClassType.JavaDataObject);
            result = result + "\n public class " + name + " implements Comparable<" + name + "> {\n";
            return result;
        }

        private string genJavaVMHeader()
        {
            string result = commentBox.GenXMLClassComment(this, XMLClassType.JavaDataObject);
            result = result + "\n public class " + name + "_VM extends " + name + " {\n";
            return result;
        }
        /// <summary>
        /// Generates the instance variables for Java data object for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is  Java code for this object's instance variables.
        private string genJavaInstanceVariables()
        {
            string result = "";
            foreach (Column r in columns)
            {
                result = result + "private " + r.data_type.toJavaDataType() + " " + r.column_name + ";\n";
            }
            return result;
        }
        private string genJavaVMInstanceVariables()
        {
            string result = "";
            foreach (Column r in columns)
            {
                if (r.references != null && r.references != "")
                {
                    string[] parts = r.references.Split('.');
                    string fk_table = parts[0];
                    string fk_name = parts[1];

                    result += "private " + fk_table + " " + fk_table + ";\n";
                }
            }

            foreach (foreignKey key in data_tables.all_foreignKey)
            {
                if (key.referenceTable.ToLower().Equals(name.ToLower()))
                {
                    string child_table = key.mainTable;

                    result += "private List<" + child_table + "> " + child_table + "s;\n";
                }
            }

            return result;
        }
        /// <summary>
        /// Generates the constructor for the Java data object for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is  Java code for this object's constructor.
        ///
        private string genJavaContructor()
        {
            //default
            string defaultConstructor = "\npublic " + name + "(){}\n";
            //param
            string ParamConsctructor = "\npublic " + name + "(";
            string comma = "";
            foreach (Column r in columns)
            {
                ParamConsctructor += comma + r.data_type.toJavaDataType() + " " + r.column_name;
                comma = ", ";
            }
            ParamConsctructor += ") {\n";
            foreach (Column r in columns)
            {
                ParamConsctructor += "\nthis." + r.column_name + " = " + r.column_name + ";";
            }
            ParamConsctructor += "\n}\n";

            //param2

            string ParamConstructor2 = "\npublic " + name + "(";
            comma = "";
            foreach (Column r in columns)
            {
                if (r.primary_key == 'y' || r.primary_key == 'Y' || r.unique == 'y' || r.unique == 'Y')
                {

                    ParamConstructor2 = ParamConstructor2 + comma + r.data_type.toJavaDataType() + " " + r.column_name;
                    comma = ", ";
                }
            }

            ParamConstructor2 += ") {\n";
            foreach (Column r in columns)
            {
                if (r.primary_key == 'y' || r.primary_key == 'Y' || r.unique == 'y' || r.unique == 'Y')
                {
                    ParamConstructor2 = ParamConstructor2 + "\nthis." + r.column_name + " = " + r.column_name + ";";
                }
            }
            ParamConstructor2 += "\n}\n";

            string result = defaultConstructor + ParamConsctructor + ParamConstructor2;
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string genJavaVMContructor()
        {
            //default
            string defaultConstructor = "\npublic " + name + "_VM(){}\n";
            //param
            string ParamConsctructor = "\npublic " + name + "_VM(" + name + " " + name.ToLower() + "){\n";
            string comma = "";
            ParamConsctructor += "super(";

            foreach (Column r in columns)
            {
                ParamConsctructor += comma + name.ToLower() + ".get" + r.column_name + "()";
                comma = ", ";
            }
            ParamConsctructor += ");\n}\n";

            //param2 having vm hold it's parents

            string ParamConsctructor2 = "\npublic " + name + "_VM(" + name + " " + name.ToLower();
            comma = ",";

            foreach (Column r in columns)
            {
                if (r.references != null && r.references != "")
                {
                    string[] parts = r.references.Split('.');
                    string fk_table = parts[0];
                    string fk_name = parts[1];

                    ParamConsctructor2 += comma + fk_table + " " + fk_table.ToLower();
                }
            }
            comma = "";

            ParamConsctructor2 += "){\n";
            ParamConsctructor2 += "super(";
            foreach (Column r in columns)
            {
                ParamConsctructor2 += comma + " " + name.ToLower() + ".get" + r.column_name + "()";
                comma = ", ";
            }
            ParamConsctructor2 += ");\n";

            foreach (Column r in columns)
            {
                if (r.references != null && r.references != "")
                {
                    string[] parts = r.references.Split('.');
                    string fk_table = parts[0];
                    string fk_name = parts[1];

                    ParamConsctructor2 += "this." + fk_table + " = " + fk_table.ToLower() + ";\n";
                }
            }

            ParamConsctructor2 += "\n}\n";

            //param3, having vm hold it's children

            string ParamConsctructor3 = "\npublic " + name + "_VM(" + name + " " + name.ToLower();
            comma = ",";

            foreach (foreignKey key in data_tables.all_foreignKey)
            {
                if (key.referenceTable.ToLower().Equals(name.ToLower()))
                {
                    string child_table = key.mainTable;

                    ParamConsctructor3 += comma + "List<" + child_table + "> " + child_table.ToLower() + "s";
                }
            }
            comma = "";

            ParamConsctructor3 += "){\n";
            ParamConsctructor3 += "super(";
            foreach (Column r in columns)
            {
                ParamConsctructor3 += comma + " " + name.ToLower() + ".get" + r.column_name + "()";
                comma = ", ";
            }
            ParamConsctructor3 += ");\n";

            foreach (foreignKey key in data_tables.all_foreignKey)
            {
                if (key.referenceTable.ToLower().Equals(name.ToLower()))
                {
                    string child_table = key.mainTable;
                    ParamConsctructor3 += "this." + child_table + "s = " + child_table.ToLower() + "s;\n";
                }
            }

            ParamConsctructor3 += "\n}\n";

            string result = defaultConstructor + ParamConsctructor + ParamConsctructor2 + ParamConsctructor3;
            return result;
        }
        /// <summary>
        /// Generates the setters and getters for the  instance variables for Java data object for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is  Java code for this object's setters and getters for this object's instance variables.
        ///
        private string genJavaSetterAndGetter()
        {
            string result = "";
            foreach (Column r in columns)
            {
                string getter = commentBox.genJavaGetterJavaDoc(name, r);
                getter += "public " + r.data_type.bracketStrip().toJavaDataType() + " get" + r.column_name + "() {\n return " + r.column_name + ";\n}";
                string setter = commentBox.genJavaSetterJavaDoc(name, r);
                 setter += "public void set" + r.column_name + "(" + r.data_type.bracketStrip().toJavaDataType() + " " + r.column_name + ")";
                if (r.data_type.Equals("datetime"))
                {
                    setter += "throws ParseException";
                }
                setter += " {\n";
                if (r.data_type.Equals("nvarchar"))
                {
                    setter = setter + r.column_name + " = " + r.column_name + ".replaceAll(\"[^.,!()A-Za-z0-9 - ]\",\"\");\n";
                    setter = setter + "if(" + r.column_name + ".length()<4){\n";
                    setter = setter + "throw new IllegalArgumentException(\"" + r.column_name + " is too short.\");\n}\n";
                    setter = setter + "if(" + r.column_name + ".length()>" + r.length + "){\n";
                    setter = setter + "throw new IllegalArgumentException(\"" + r.column_name + " is too long.\");\n}\n";
                }
                if (r.data_type.Equals("int"))
                {
                    setter += "if (" + r.column_name + "<0||" + r.column_name + ">10000){\n";
                    setter += "throw new IllegalArgumentException(\"" + r.column_name + " Can Not Be Negative\");\n";
                    setter += "}\n";
                }
                if (r.data_type.Equals("decimal"))
                {
                    setter += "if (" + r.column_name + "<0||" + r.column_name + ">10000){\n";
                    setter += "throw new IllegalArgumentException(\"" + r.column_name + " Can Not Be Negative\");\n";
                    setter += "}\n";
                }
                if (r.data_type.Equals("datetime"))
                {
                    setter += "String minDate = \"01/01/1991\";\n";
                    setter += "DateFormat df = new SimpleDateFormat(\"dd/MM/yyyy\");\n";
                    setter += "Date _minDate = df.parse(minDate);\n";
                    setter += "String maxDate = \"12/31/2100\";\n";
                    setter += "Date _maxDate = df.parse(maxDate);\n";
                    setter += "if (" + r.column_name + ".compareTo(_minDate)<0){\n";
                    setter += "throw new IllegalArgumentException(\"" + r.column_name + " Can Not Be Before 1991\");\n";
                    setter += "}\n";
                    setter += "if (" + r.column_name + ".compareTo(_maxDate))>0{\n";
                    setter += "throw new IllegalArgumentException(\"" + r.column_name + " Can Not Be after 2100\");\n";
                    setter += "}\n";

                }
                setter = setter + "this." + r.column_name + " = " + r.column_name + ";\n}";
                result = result + getter + "\n" + setter + "\n";
            }
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string genJavaVMSetterAndGetter()
        {
            string result = "";
            foreach (Column r in columns)
            {

                if (r.references != null && r.references != "")
                {
                    string[] parts = r.references.Split('.');
                    string fk_table = parts[0];
                    string fk_name = parts[1];

                    string getter = "public " + fk_table + " get" + fk_table + "() {\n return " + fk_table + ";\n}";
                    string setter = "public void set" + fk_table + "(" + fk_table + " _" + fk_table.ToLower() + ") {\n";

                    setter = setter + "this." + fk_table + " = _" + fk_table.ToLower() + ";\n}";
                    result += getter + "\n" + setter + "\n";
                }
            }

            foreach (foreignKey key in data_tables.all_foreignKey)
            {
                if (key.referenceTable.ToLower().Equals(name.ToLower()))
                {
                    string child_table = key.mainTable;

                    string getter = "public List<" + child_table + "> get" + child_table + "s() {\n return " + child_table + "s;\n}";
                    string setter = "public void set" + child_table + "s(List<" + child_table + "> _" + child_table.ToLower() + "s) {\n";

                    setter = setter + "this." + child_table + "s = _" + child_table.ToLower() + "s;\n}";
                    result += getter + "\n" + setter + "\n";

                }
            }

            return result;
        }
        /// <summary>
        /// Generates the compareTo() method for this Java data object for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is  Java code for this object's compareTo() method
        ///

        private string genJavaComparable()
        {
            string result = "";
            result += "@Override\n";
            result += "public int compareTo(@NotNull " + name + " o) {\n";
            foreach (Column r in columns)
            {
                if (!r.data_type.toCSharpDataType().Equals("bool"))
                {
                    result += "if (this." + r.column_name + ".compareTo(o." + r.column_name + ")<0){\n";
                    result += "return -1;\n";
                    result += "}\n";
                    result += "else if(this." + r.column_name + ".compareTo(o." + r.column_name + ") > 0){\n";
                    result += "return 1;\n";
                    result += "}\n";
                }
                else
                {
                    result += "if (!this." + r.column_name + "&&o." + r.column_name + "){\n";
                    result += "return -1;\n";
                    result += "}\n";
                    result += "if (this." + r.column_name + "&&!o." + r.column_name + "){\n";
                    result += "return 1;\n";
                    result += "}\n";
                }
            }

            result += "return 0;\n";
            result += "}\n";
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string genJavaFooter()
        {
            string result = "\n}\n";
            return result;
        }
        /// <summary>
        /// Generates the Java DAO object for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is  Java code for this object's associated DAO object.
       
        public string genJavaDAO()
        {
            string result = "";
            result += genJavaDAOHeader(settings.database_name);        //works
            result += genJavaDAOCreate();       //returns ""
            result += genJavaDAOAddBatch();
            result += genJavaDAOretrieveByKey(); // works
            result += genJavaDAOretrieveAll(); // wokring on it now
            result += genJavaDAORetriveActive(); // working on it now
            result += genJavaDAOretrieveDistinct();
            result += genJavaDAORetriveByFK();
            result += genJavaDAOUpdate(); //rturns ""
            result += genJavaDelete(); //returns ""
            result += genJavaunDelete(); //returns ""
            result += genJavaDAOCount();
            result += genJavaDAOFileRead();  //done
            result += genJavaDAOFileWrite(); //pending
            result += genJavaDAOSQLFileWrite();
            result += genJavaDAOFooter(); //work

            return result;
        }
        /// <summary>
        /// Generates the Java DAO interface for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is  Java code for this object's associated DAO interface.

        public string genJavaiDAO()
        {
            String result = commentBox.GenXMLClassComment(this, XMLClassType.JavaDAO);
            result += "public interface i" + name + "DAO{\n";

            //add
            result += commentBox.GenJavaDocMethodComment(this, JavaDoc_Method_Type.Java_DAO_Add);
            result += "int add (" + name + " _" + name.ToLower() + ") throws SQLException;\n";
            //get by pk
            result += commentBox.GenJavaDocMethodComment(this, JavaDoc_Method_Type.Java_DAO_retrieve_By_FK);
            result += name + " get" + name + "ByPrimaryKey(" + name + " _" + name.ToLower() + ") throws SQLException;\n";
            //get by fk
            foreach (Column r in columns)
            {
                if (r.references != "")
                {
                    string[] parts = r.references.Split('.');
                    string fk_table = parts[0];
                    string fk_name = parts[1];
                    result += commentBox.GenJavaDocMethodComment(this, JavaDoc_Method_Type.Java_DAO_retrieve_By_FK);
                    result = result + "public List<" + name + "> get" + name + "by" + fk_table + "(" + r.data_type.toJavaDataType() + " " + fk_name + ") throws SQLException; \n";
                }
            }

            //update
            result += commentBox.GenJavaDocMethodComment(this, JavaDoc_Method_Type.Java_DAO_Update);
            result += "int update(" + name + " old" + name + ", " + name + " new" + name + ") throws SQLException;\n";
            //get all
            result += commentBox.GenJavaDocMethodComment(this, JavaDoc_Method_Type.Java_DAO_retrieve_All_);
            result += "List<" + name + "> getAll" + name + "(int offset, int limit, String search";
            foreach (Column r in columns)
            {
                if (r.foreign_key != "")
                {
                    result += "," + r.data_type.toJavaDataType() + " " + r.column_name;
                }
            }

            result += ") throws SQLException;\n";
            // get active
            result += commentBox.GenJavaDocMethodComment(this, JavaDoc_Method_Type.Java_DAO_retrieve_All_);
            result += "List<" + name + "> getActive" + name + "() throws SQLException;\n";
            //get for dropdown
            result += commentBox.GenJavaDocMethodComment(this, JavaDoc_Method_Type.Java_DAO_Get_Distinct);
            result += "List<" + name + "> getDistinct" + name + "ForDropdown() throws SQLException;\n";

            //delete
            int count = 0;
            result += commentBox.GenJavaDocMethodComment(this, JavaDoc_Method_Type.Java_DAO_Delete);
            result += "int delete" + name + "(";
            string comma;
            foreach (Column r in columns)
            {
                if (count > 0)
                {
                    comma = ",";
                }
                else
                {
                    comma = "";
                }
                if (r.primary_key == 'Y' || r.primary_key == 'y')
                {
                    result += comma + " " + r.data_type.toJavaDAODataType() + " " + r.column_name;
                    count++;
                }
            }
            result += ") throws SQLException;\n";

            //undelete
            count = 0;
            result += commentBox.GenJavaDocMethodComment(this, JavaDoc_Method_Type.Java_DAO_Undelete);
            result += "int undelete" + name + "(";
            foreach (Column r in columns)
            {
                if (count > 0)
                {
                    comma = ",";
                }
                else
                {
                    comma = "";
                }
                if (r.primary_key == 'Y' || r.primary_key == 'y')
                {
                    result += comma + " " + r.data_type.toJavaDAODataType() + " " + r.column_name;
                    count++;
                }
            }
            result += ") throws SQLException;\n";

            //gen count
            result += "int get" + name + "Count(String Search_term";
            foreach (Column r in columns) {
                if (r.foreign_key != "") {
                    result += ", String " + r.column_name;
                }
            }
            result += ") throws SQLException;\n";

            //file read
            result += "List<" + name + "> read" + name + "sFromFile(File uploadedFile) throws Exception;\n";
            //add batch
            result += "int addBatchOf" + name + "s(List<" + name + "> " + name.ToLower() + "s) throws SQLException;\n";
            //write to file
            result += "int write" + name + "ToFile(List<" + name + "> " + name + "s, String path) throws IOException;\n";
            //write to mySQL
            result += "int write" + name + "ToSQLInsert(List<" + name + "> " + name + "s, String path) throws IOException;\n";
            
            result += "}\n";

            return result;

        }
        /// <summary>
        /// Generates the header for the Java DAO object for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is  Java code for this object's associated DAO object header.
        private string genJavaDAOHeader(String projectName)
        {
            string result = commentBox.GenXMLClassComment(this, XMLClassType.JavaDAO);
            result = result + "import com." + settings.owner_name + "." + projectName + ".models." + name + ";\n" +
                "import java.sql.CallableStatement;\n" +
                "import java.sql.Connection;\n" +
                "import java.sql.ResultSet;\n" +
                "import java.sql.SQLException;\n" +
                "import java.util.ArrayList;\n" +
                "import java.util.List;\n" +
                "import java.time.LocalDate;\n" +
                "import static com." + settings.owner_name + "." + projectName + ".idata.i" + name + "DAO;\n" +
            "import static com." + settings.owner_name + "." + projectName + ".data.Database.getConnection;\n";
            result += commentBox.GenXMLClassComment(this, XMLClassType.JavaDAO);
            result += "public class " + name + "DAO implements i" + name + "DAO{\n\n";
            return result;
        }
        /// <summary>
        /// Generates the footer for the Java DAO object for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is  Java code for this object's associated DAO object footer.
        private string genJavaDAOFooter()
        {
            string result = "\n}\n";
            return result;
        }
        /// <summary>
        /// Generates the retrieve by PK for the Java DAO object for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is  Java code for this object's associated DAO object retrieve by PK function.
        private string genJavaDAOretrieveByKey()
        {
            string result = commentBox.GenJavaDocMethodComment(this, JavaDoc_Method_Type.Java_DAO_retrieve_By_PK);
            string nullValue = "";
            string comma = "";
            result = result + "public " + name + " get" + name + "ByPrimaryKey(" + name + " _" + name.ToLower() + ") throws SQLException{\n";
            result = result + name + " result = null;\n";
            result += "try(Connection connection = getConnection()) {\n";
            result = result + "try(CallableStatement statement = connection.prepareCall(\"{CALL sp_retrieve_by_pk_" + name + "(?)}\")) {\n";
            for (int i = 0; i < columns.Count; i++)
                if (columns[i].primary_key == 'y' || columns[i].primary_key == 'Y')
                {
                    result = result + "statement.setString(1, _" + name.ToLower() + ".get" + columns[i].column_name.bracketStrip() + "().toString());\n";
                }
            result += "\ntry (ResultSet resultSet = statement.executeQuery()){";
            result += "\nif(resultSet.next()){";
            foreach (Column r in columns)
            {
                result = result + r.data_type.toJavaDataType() + " " + r.column_name.bracketStrip() + " = resultSet.get" + r.data_type.toJavaDAODataType() + "(\"" + name + "_" + r.column_name.bracketStrip() + "\");\n";
                if ((r.nullable == 'y' || r.nullable == 'Y') && r.data_type.toJavaDataType().Equals("String"))
                {
                    result = result + "if(resultSet.wasNull()){\n" + r.column_name + "=" + nullValue + ";}\n";
                }
            }
            foreach (foreignKey fk in data_tables.all_foreignKey)
            {
                if (fk.mainTable.Equals(name))
                {
                    foreach (table t in data_tables.all_tables)
                    {
                        if (t.name.Equals(fk.referenceTable))
                        {
                            foreach (Column r in t.columns)
                            {
                                result = result + r.data_type.toJavaDataType() + " " + t.name + "_" + r.column_name.bracketStrip() + " = resultSet.get" + r.data_type.toJavaDAODataType() + "(\"" + t.name + "_" + r.column_name.bracketStrip() + "\");\n";
                                if ((r.nullable == 'y' || r.nullable == 'Y') && r.data_type.toJavaDataType().Equals("String"))
                                {
                                    result = result + "if(resultSet.wasNull()){\n" + t.name + "_" + r.column_name.bracketStrip() + "=" + nullValue + ";}\n";
                                }
                            }
                        }
                    }
                }
            }
            result = result + "result = new " + name + "(";
            foreach (Column r in columns)
            {
                result = result + comma + " " + r.column_name.bracketStrip();
                comma = ",";
            }
            result += ");";
            result += "}\n}\n}\n";
            result += "} catch (SQLException e) {\n";
            result += " throw new RuntimeException(e);\n}\n";
            result += "return result;\n}\n";
            return result;
        }
        /// <summary>
        /// Generates the retrieve by all for the Java DAO object for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is  Java code for this object's associated DAO object retrieve all.
        private string genJavaDAOretrieveAll()
        {
            string result = commentBox.GenJavaDocMethodComment(this, JavaDoc_Method_Type.Java_DAO_retrieve_All_);
            string comma = "";
            string nullValue = "";
            result = result + "public List<" + name + "> getAll" + name + "() {\n";
            result = result + "return getAll" + name + "(" + appData2.settings.page_size + ",0,\"\");";
            result += "}\n";
            result = result + "public static List<" + name + "> getAll" + name + "(int pagesize) {\n";
            result = result + "return getAll" + name + "(pagesize" + ",0,\"\");";
            result += "}\n";
            result = result + "public static List<" + name + "> getAll" + name + "(int limit, int offset, String search";
            foreach (Column r in columns)
            {
                if (r.foreign_key != "")
                {
                    result += "," + r.data_type.toJavaDataType() + " " + r.column_name;
                }
            }

            result += ") {\n";
            result = result + "List<" + name + "> result = new ArrayList<>();\n";
            result += "try (Connection connection = getConnection()) { \n";
            result += "if (connection != null) {\n";
            result = result + "try(CallableStatement statement = connection.prepareCall(\"{CALL sp_retrieve_by_all_" + name + "(?,?,?)}\")) {\n" +
                "statement.setInt(1,limit)\n;" +
                "statement.setInt(2,offset);\n" +
                 "statement.setString(3,search);\n";
            int count = 4;
            foreach (Column r in columns)
            {
                if (r.foreign_key != "")
                {
                    result += "statement.set" + r.data_type.toJavaDAODataType() + "(" + count + "," + r.column_name + ");\n";
                    count++;
                }
            }
            result += "try(ResultSet resultSet = statement.executeQuery()) {\n";
            result += "while (resultSet.next()) {\n";
            foreach (Column r in columns)
            {

                if (r.data_type.toJavaDataType().Equals("Integer"))
                {
                    nullValue = "0";
                }
                else { nullValue = "\"\""; }
                result = result + r.data_type.toJavaDataType() + " " + r.column_name.bracketStrip() + " = resultSet.get" + r.data_type.toJavaDAODataType() + "(\"" + name + "_" + r.column_name.bracketStrip() + "\");\n";
                if (r.nullable == 'y' || r.nullable == 'Y')
                {
                    result = result + "if(resultSet.wasNull()){\n" + r.column_name + "=" + nullValue + ";}\n";
                }
            }
            foreach (foreignKey fk in data_tables.all_foreignKey)
            {
                if (fk.mainTable.Equals(name))
                {
                    foreach (table t in data_tables.all_tables)
                    {
                        if (t.name.Equals(fk.referenceTable))
                        {
                            foreach (Column r in t.columns)
                            {
                                result = result + r.data_type.toJavaDataType() + " " + t.name + "_" + r.column_name.bracketStrip() + " = resultSet.get" + r.data_type.toJavaDAODataType() + "(\"" + t.name + "_" + r.column_name.bracketStrip() + "\");\n";
                                if ((r.nullable == 'y' || r.nullable == 'Y') && r.data_type.toJavaDataType().Equals("String"))
                                {
                                    result = result + "if(resultSet.wasNull()){\n" + t.name + "_" + r.column_name.bracketStrip() + "=" + nullValue + ";}\n";
                                }
                            }
                        }
                    }
                }
            }
            result = result + " " + name + " _" + name.ToLower() + " = new " + name + "(";
            foreach (Column r in columns)
            {
                result = result + comma + " " + r.column_name.bracketStrip();
                comma = ",";
            }
            result += ");";
            result = result + "\n result.add(_" + name.ToLower() + ");\n}\n}\n}\n}\n";
            result += "} catch (SQLException e) {\n";
            result = result + "throw new RuntimeException(\"Could not retrieve " + name + "s. Try again later\");\n";
            result += "}\n";
            result += "return result;}\n";
            return result;
        }
        private string genJavaDAOretrieveDistinct()
        {
            {
                string result = commentBox.GenJavaDocMethodComment(this, JavaDoc_Method_Type.Java_DAO_retrieve_All_);
                string comma = "";

                result = result + "public static List<" + name + "> selectDistinct" + name + "ForDropdown() {\n";
                result = result + "List<" + name + "> result = new ArrayList<>();\n";
                result += "try (Connection connection = getConnection()) { \n";
                result += "if (connection != null) {\n";
                result = result + "try(CallableStatement statement = connection.prepareCall(\"{CALL sp_select_distinct_and_active_" + name + "_for_dropdown" + "()}\")) {\n";

                result += "try(ResultSet resultSet = statement.executeQuery()) {\n";
                result += "while (resultSet.next()) {\n";
                foreach (Column r in columns)
                {
                    if (r.primary_key == 'y' || r.primary_key == 'Y' || r.unique == 'y' || r.unique == 'Y')
                    {
                        string nullValue;
                        if (r.data_type.toJavaDataType().Equals("Integer"))
                        {
                            nullValue = "0";
                        }
                        else { nullValue = "\"\""; }
                        result = result + r.data_type.toJavaDataType() + " " + r.column_name.bracketStrip() + " = resultSet.get" + r.data_type.toJavaDAODataType() + "(\"" + name + "_" + r.column_name.bracketStrip() + "\");\n";
                        if (r.nullable == 'y' || r.nullable == 'Y')
                        {
                            result = result + "if(resultSet.wasNull()){\n" + r.column_name + "=" + nullValue + ";}\n";
                        }
                    }
                }
                result = result + " " + name + " _" + name.ToLower() + " = new " + name + "(";
                foreach (Column r in columns)
                {
                    if (r.primary_key == 'y' || r.primary_key == 'Y' || r.unique == 'y' || r.unique == 'Y')
                    {
                        result = result + comma + " " + r.column_name.bracketStrip();
                        comma = ",";
                    }
                }
                result += ");";
                result = result + "\n result.add(_" + name.ToLower() + ");\n}\n}\n}\n}\n";
                result += "} catch (SQLException e) {\n";
                result = result + "throw new RuntimeException(\"Could not retrieve " + name + "s. Try again later\");\n";
                result += "}\n";
                result += "return result;}\n";
                return result;
            }
        }
        /// <summary>
        /// Generates the retrieve active for the Java DAO object for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is  Java code for this object's associated DAO object retrieve active function.
        public String genJavaDAORetriveActive()
        {
            string result = commentBox.GenJavaDocMethodComment(this, JavaDoc_Method_Type.Java_DAO_retrieve_All_);
            string comma = "";
            result = result + "public List<" + name + "> getActive" + name + "() {\n";
            result = result + "List<" + name + "> result = new ArrayList<>();\n";
            result += "try (Connection connection = getConnection()) { \n";
            result += "if (connection != null) {\n";
            result = result + "try(CallableStatement statement = connection.prepareCall(\"{CALL sp_retrieve_by_active_" + name + "()}\"))\n {";
            result += "try(ResultSet resultSet = statement.executeQuery()) {\n";
            result += "while (resultSet.next()) {\n";
            foreach (Column r in columns)
            {
                string nullValue;
                if (r.data_type.toJavaDataType().Equals("Integer"))
                {
                    nullValue = "0";
                }
                else { nullValue = "\"\""; }
                result = result + r.data_type.toJavaDataType() + " " + r.column_name.bracketStrip() + " = resultSet.get" + r.data_type.toJavaDAODataType() + "(\"" + name + "_" + r.column_name.bracketStrip() + "\");\n";
                if (r.nullable == 'y' || r.nullable == 'Y')
                {
                    result = result + "if(resultSet.wasNull()){\n" + r.column_name + "=" + nullValue + ";}\n";
                }
            }
            result = result + " " + name + " _" + name.ToLower() + " = new " + name + "(";
            foreach (Column r in columns)
            {
                result = result + comma + " " + r.column_name.bracketStrip();
                comma = ",";
            }
            result += ");";
            result = result + "\n result.add(_" + name.ToLower() + ");\n}\n}\n}\n}\n";
            result += "} catch (SQLException e) {\n";
            result = result + "throw new RuntimeException(\"Could not retrieve " + name + "s. Try again later\");\n";
            result += "}\n";
            result += "return result;}\n";
            return result;
        }
        /// <summary>
        /// Generates the retrieve by FK for the Java DAO object for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is  Java code for this object's associated DAO object retrieve by FK.
        public String genJavaDAORetriveByFK()
        {
            string result = commentBox.GenJavaDocMethodComment(this, JavaDoc_Method_Type.Java_DAO_retrieve_By_FK);
            string comma = "";
            string nullValue = "";
            foreach (Column s in columns)
            {
                if (s.references != "")
                {
                    string[] parts = s.references.Split('.');
                    string fk_table = parts[0];
                    string fk_name = parts[1];
                    result = result + "public List<" + name + "> get" + name + "by" + fk_table + "(" + s.data_type.toJavaDataType() + " " + fk_name + ") {\n";
                    result = result + "return get" + name + "by" + fk_table + "(" + s.data_type.toJavaDataType() + " " + fk_name + "," + appData2.settings.page_size + ",0);";
                    result += "}\n";
                    result = result + "public static List<" + name + "> get" + name + "by" + fk_table + "(" + s.data_type.toJavaDataType() + " " + fk_name + "int pagesize) {\n";
                    result = result + "return get" + name + "by" + fk_table + "(" + s.data_type.toJavaDataType() + " " + fk_name + ",pagesize" + ",0);";
                    result += "}\n";
                    result = result + "public static List<" + name + "> get" + name + "by" + fk_table + "(" + s.data_type.toJavaDataType() + " " + fk_name + "int pagesize,int offset) {\n";
                    result = result + "List<" + name + "> result = new ArrayList<>();\n";
                    result += "try (Connection connection = getConnection()) { \n";
                    result += "if (connection != null) {\n";
                    result = result + "try(CallableStatement statement = connection.prepareCall(\"{CALL sp_retrieve_" + name + "_by" + fk_table + "(?,?,?)}\")) {\n" +
                        "statement.set" + s.data_type.toJavaDAODataType() + "(1," + fk_name + ")\n;" +
                        "statement.setInt(2,limit)\n;" +
                        "statement.setInt(3,offset);\n";
                    result += "try(ResultSet resultSet = statement.executeQuery()) {\n";
                    result += "while (resultSet.next()) {\n";
                    foreach (Column r in columns)
                    {

                        if (r.data_type.toJavaDataType().Equals("Integer"))
                        {
                            nullValue = "0";
                        }
                        else { nullValue = "\"\""; }
                        result = result + r.data_type.toJavaDataType() + " " + r.column_name.bracketStrip() + " = resultSet.get" + r.data_type.toJavaDAODataType() + "(\"" + name + "_" + r.column_name.bracketStrip() + "\");\n";
                        if (r.nullable == 'y' || r.nullable == 'Y')
                        {
                            result = result + "if(resultSet.wasNull()){\n" + r.column_name + "=" + nullValue + ";}\n";
                        }
                    }
                    foreach (foreignKey fk in data_tables.all_foreignKey)
                    {
                        if (fk.mainTable.Equals(name))
                        {
                            foreach (table t in data_tables.all_tables)
                            {
                                if (t.name.Equals(fk.referenceTable))
                                {
                                    foreach (Column r in t.columns)
                                    {
                                        result = result + r.data_type.toJavaDataType() + " " + t.name + "_" + r.column_name.bracketStrip() + " = resultSet.get" + r.data_type.toJavaDAODataType() + "(\"" + t.name + "_" + r.column_name.bracketStrip() + "\");\n";
                                        if ((r.nullable == 'y' || r.nullable == 'Y') && r.data_type.toJavaDataType().Equals("String"))
                                        {
                                            result = result + "if(resultSet.wasNull()){\n" + t.name + "_" + r.column_name.bracketStrip() + "=" + nullValue + ";}\n";
                                        }
                                    }
                                }
                            }
                        }
                    }
                    result = result + " " + name + " _" + name.ToLower() + " = new " + name + "(";
                    foreach (Column r in columns)
                    {
                        result = result + comma + " " + r.column_name.bracketStrip();
                        comma = ",";
                    }
                    result += ");";
                    result = result + "\n result.add(_" + name.ToLower() + ");\n}\n}\n}\n}\n";
                    result += "} catch (SQLException e) {\n";
                    result = result + "throw new RuntimeException(\"Could not retrieve " + name + "s. Try again later\");\n";
                    result += "}\n";
                    result += "return result;\n}\n";
                }
            }
            return result;
        }
        /// <summary>
        /// Generates the update function for the Java DAO object for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is  Java code for this object's associated DAO object update function.
        private string genJavaDAOUpdate()
        {
            string result = commentBox.GenJavaDocMethodComment(this, JavaDoc_Method_Type.Java_DAO_Update);
            result = result + "\n public int update(" + name + " old" + name + ", " + name + " new" + name + ") throws SQLException{\n";
            result += "int result = 0;\n";
            result += "try (Connection connection = getConnection()) {\n";
            result += "if (connection !=null){\n";
            result = result + "try(CallableStatement statement = connection.prepareCall(\"{CALL sp_update_" + name + "(";
            string comma = "";
            foreach (Column r in columns)
            {
                if (r.primary_key == 'y' || r.primary_key == 'Y')
                {
                    result = result + comma + "? ";
                    comma = ",";
                }
                else
                {
                    result = result + comma + "?,?";
                }
            }
            result += ")}\"))\n {\n";
            int count = 1;
            foreach (Column r in columns)
            {
                result = result + "statement.set" + r.data_type.toJavaDAODataType() + "(" + count + ",old" + name + ".get" + r.column_name.bracketStrip() + "());\n";
                count++;
                if (r.primary_key != 'y' && r.primary_key != 'Y')
                {
                    result = result + "statement.set" + r.data_type.toJavaDAODataType() + "(" + count + ",new" + name + ".get" + r.column_name.bracketStrip() + "());\n";
                    count++;
                }
            }
            result += "result=statement.executeUpdate();\n";
            result += "} catch (SQLException e) {\n";
            result = result + "throw new RuntimeException(\"Could not update " + name + " . Try again later\");\n";
            result += "}\n}\n}\n return result;\n}\n";
            return result;
        }
        /// <summary>
        /// Generates the delete method for the Java DAO object for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is  Java code for this object's associated DAO object delete method.
        
        private string genJavaDelete()
        {
            string result = commentBox.GenJavaDocMethodComment(this, JavaDoc_Method_Type.Java_DAO_Delete);
            int count = 0;
            result += "int delete" + name + "(";
            foreach (Column r in columns)
            {
                string comma;
                if (count > 0)
                {
                    comma = ",";
                }
                else
                {
                    comma = "";
                }
                if (r.primary_key == 'Y' || r.primary_key == 'y')
                {
                    result += comma + " " + r.data_type.toJavaDAODataType() + " " + r.column_name;
                    count++;
                }
            }
            result += ") throws SQLException{\n";
            result += "int rowsAffected=0;\n";
            result += "try (Connection connection = getConnection()) {\n";
            result += "if (connection != null) {\n";
            result = result + "try (CallableStatement statement = connection.prepareCall(\"{CALL sp_Delete_" + name + "( ?)}\")){\n";
            result = result + "statement.setInt(1," + name.ToLower() + "ID);\n";
            result += "rowsAffected = statement.executeUpdate();\n";
            result += "if (rowsAffected == 0) {\n";
            result = result + "throw new RuntimeException(\"Could not Delete " + name + ". Try again later\");\n";
            result += "}\n";
            result += "}\n";
            result += "}\n";
            result += "} catch (SQLException e) {\n";
            result = result + "throw new RuntimeException(\"Could not Delete " + name + ". Try again later\");\n";
            result += "}\n";
            result += "return rowsAffected;\n";
            result += "}\n";
            result += "";
            return result;
        }
        /// <summary>
        /// Generates the undelete method for the Java DAO object for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is  Java code for this object's associated DAO object undelete method.
        ///
        private string genJavaunDelete()
        {
            string result = commentBox.GenJavaDocMethodComment(this, JavaDoc_Method_Type.Java_DAO_Undelete);
            int count = 0;
            result += "int undelete" + name + "(";
            foreach (Column r in columns)
            {
                string comma;
                if (count > 0)
                {
                    comma = ",";
                }
                else
                {
                    comma = "";
                }
                if (r.primary_key == 'Y' || r.primary_key == 'y')
                {
                    result += comma + " " + r.data_type.toJavaDAODataType() + " " + r.column_name;
                    count++;
                }
            }
            result += ") throws SQLException{\n";
            result += "int rowsAffected=0;\n";
            result += "try (Connection connection = getConnection()) {\n";
            result += "if (connection != null) {\n";
            result = result + "try (CallableStatement statement = connection.prepareCall(\"{CALL sp_unDelete_" + name + "( ?)}\")){\n";
            result = result + "statement.setInt(1," + name.ToLower() + "ID);\n";
            result += "rowsAffected = statement.executeUpdate();\n";
            result += "if (rowsAffected == 0) {\n";
            result = result + "throw new RuntimeException(\"Could not Restore " + name + ". Try again later\");\n";
            result += "}\n";
            result += "}\n";
            result += "}\n";
            result += "} catch (SQLException e) {\n";
            result = result + "throw new RuntimeException(\"Could not Restore " + name + ". Try again later\");\n";
            result += "}\n";
            result += "return rowsAffected;\n";
            result += "}\n";
            result += "";
            return result;
        }
        /// <summary>
        /// Generates the create method for the Java DAO object for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is  Java code for this object's associated DAO object create method.
        ///
        private string genJavaDAOCreate()
        {
            string result = commentBox.GenJavaDocMethodComment(this, JavaDoc_Method_Type.Java_DAO_Add);
            string comma = "";
            result = result + "public int add(" + name + " _" + name.ToLower() + ") {\n";
            result += "int numRowsAffected=0;\n";
            result += "try (Connection connection = getConnection()) {\n";
            result += "if (connection != null) {\n";
            result = result + "try (CallableStatement statement = connection.prepareCall(\"{CALL sp_insert_" + name + "(";
            foreach (Column r in columns)
            {
                if (r.identity == "" && r.default_value == "")
                {
                    result = result + comma + " ?";
                    comma = ",";
                }
            }
            result += ")}\")){\n";
            int count = 1;
            foreach (Column r in columns)
            {
                if (r.identity == "" && r.default_value == "")
                {
                    result = result + "statement.set" + r.data_type.toJavaDAODataType() + "(" + count + ",_" + name.ToLower() + ".get" + r.column_name.bracketStrip() + "());\n";
                    count++;
                }
            }
            result += "numRowsAffected = statement.executeUpdate();\n";
            result += "if (numRowsAffected == 0) {\n";
            result = result + "throw new RuntimeException(\"Could not add " + name + ". Try again later\");\n}\n";
            result += "} \n}\n";
            result += "} catch (SQLException e) {\n";
            result = result + "throw new RuntimeException(\"Could not add " + name + ". Try again later\");\n}\n";
            result += "return numRowsAffected;\n}";
            result += "\n";
            return result;
        }
        private string genJavaDAOCount() {
            string result = commentBox.GenJavaDocMethodComment(this, JavaDoc_Method_Type.Java_DAO_retrieve_All_);
            string comma = "";
            string nullValue = "";
            
            result = result + "public int get" + name + "Count(String Search_term";
            foreach (Column r in columns)
            {
                if (r.foreign_key != "")
                {
                    result += "," + r.data_type.toJavaDataType() + " " + r.column_name;
                }
            }

            result += ") {\n";
            result = result + "int result =0;\n";
            result += "try (Connection connection = getConnection()) { \n";
            result += "if (connection != null) {\n";
            result += "try(CallableStatement statement = connection.prepareCall(\"{CALL sp_count_by_all_" + name + "(?,?,?)}\")) {\n";
            result += "statement.setString(1,Search_term);\n";
            int count = 2;
            foreach (Column r in columns)
            {
                if (r.foreign_key != "")
                {
                    result += "statement.set" + r.data_type.toJavaDAODataType() + "(" + count + "," + r.column_name + ");\n";
                    count++;
                }
            }
            result += "try(ResultSet resultSet = statement.executeQuery()) {\n";
            result += "while (resultSet.next()) {\n";
            result += "result = resultSet.getInt(1);\n";
            result+="\n}\n}\n}\n";
            result += "} catch (SQLException e) {\n";
            result = result + "throw new RuntimeException(\"Could not retrieve " + name + "s. Try again later\");\n";
            result += "}\n";
            result += "return result;\n}\n";
            return result;

        }
        private string genJavaDAOAddBatch() {
            string result = "";
            string comma = "";
            result += "public int addBatchOf" + name + "s(List<" + name + "> " + name.ToLower() + "s) throws SQLException{\n";
            result += "int numRowsAffected=0;\n";
            result += "try (Connection connection = getConnection()) {\n";
            result += "if (connection != null) {\n";
            result += "for (" + name + " _" + name.ToLower() + " : " + name.ToLower() + "s) {\n";
            result = result + "try (CallableStatement statement = connection.prepareCall(\"{CALL sp_insert_" + name + "(";
            foreach (Column r in columns)
            {
                if (r.identity == "" && r.default_value == "")
                {
                    result = result + comma + " ?";
                    comma = ",";
                }
            }
            result += ")}\")){\n";
            int count = 1;
            foreach (Column r in columns)
            {
                if (r.identity == "" && r.default_value == "")
                {
                    result = result + "statement.set" + r.data_type.toJavaDAODataType() + "(" + count + ",_" + name.ToLower() + ".get" + r.column_name.bracketStrip() + "());\n";
                    count++;
                }
            }
            result += "numRowsAffected += statement.executeUpdate();\n";
            
            result += "} catch (SQLException e) {\n";
            result = result + "continue;\n";
            result += "}\n";
            result += "}\n";
            result += "}\n";
            result += "} catch (Exception e) {\n";
            result += "throw new SQLException(e);\n";
            result += "}\n";
            result += "return numRowsAffected;\n}";
            result += "\n";
            return result;
            
        }


        private string genJavaDAOFileRead() {
            string result = "public List<"+name+"> get"+name+"sFromFile(File file) throws FileNotFoundException {\n";
            result += "List<"+name+"> results = new ArrayList<>();\n";
            result += "BufferedReader reader;\n";
            result += "try {\n";
            result += "reader = new BufferedReader(new FileReader(file));\n";
            result += "//first line is just heading data\n";
            result += "String line = reader.readLine();\n";
            result += "line = reader.readLine();\n";
            result += "while (line!=null){\n";
            result += "ArrayList parts = new ArrayList(List.of(line.split(\"\\t\")));\n";
            int i = 0;
            foreach (Column r in columns)
            {
                if (r.increment == 0)
                {
                    result += r.data_type.toJavaDataType() + " " + r.column_name.ToLower() + "=("+r.data_type.toJavaDataType()+") parts.get(" + i + ");\n";
                    i++;
                }
            }
            result += name + " " + name.ToLower() + " = new " + name + "(";
            string comma = "";
            foreach (Column r in columns)
            {
                result += comma + r.column_name.ToLower();
                comma = ", ";
            }
            result += ");\n";
            result += "results.add(" + name.ToLower() + ");\n";
            result += "line = reader.readLine();\n";
            
            result += "}\n";
            result += "} catch (Exception e) {\n";
            result += "throw new RuntimeException(e.getMessage()+\"\\n\\nCould Not Read " + name + "s. Please Check your file formatting;\");\n";
            result += "}\n";
            result += "return results;\n";

            result += "}\n";


            return result;
        }

        private string genJavaDAOFileWrite()
        {
            string result = "";
            result += "int write" + name + "ToFile(List<" + name + "> " + name + "s, String path){\n";
            result += "int result = 0;\n";
            result += "File file = new File(path);\n";
            result += "if (!file.exists()) {\n";
            result += "file.getParentFile().mkdirs();\n";
            result += "file.createNewFile();\n";
            result += "}";
            result += "PrintWriter writer = new PrintWriter(path, StandardCharsets.UTF_8);\n";
            result += "writer.println(\"";
            string tab = "";
            foreach (Column r in columns) {
                result += tab + r.column_name;
                tab = "\\t";
            }
            tab = "";
            result += "\");\n";
            result += "for (" + name + " " + "_" + name.ToLower() + " : " + name + "s) {\n";
            foreach (Column r in columns) {
                result += "writer.print("+tab + "_" + name.ToLower() + ".get" + r.column_name + "());\n";
                tab = "\"\\t\"+";
            }
            result += "writer.print(\"\\n\")\n";
            result += "result ++;\n";
            result += "}\n";
            result += "writer.close();\n";
            result += "return result;\n";
            result += "}\n";
            return result;
        }
        
        private string genJavaDAOSQLFileWrite()
        {
            string result = "";
            result += "int write" + name + "ToSQLInsert(List<" + name + "> " + name + "s, String path){\n";
            result += "int result = 0;\n";
            result += "File file = new File(path);\n";
            result += "if (!file.exists()) {\n";
            result += "file.getParentFile().mkdirs();\n";
            result += "file.createNewFile();\n";
            result += "}";

            result += "PrintWriter writer = new PrintWriter(path, StandardCharsets.UTF_8);\n";
            result += "writer.println(\"INSERT\\t INTO \\t"+name+"\\t(";
            string tab = "";
            foreach (Column r in columns)
            {
                result += tab + r.column_name;
                tab = ",\\t";
            }
            tab = "";
            result += ")\\n\");\n";
            result += "writer.println(\"VALUES\\n\");\n";
            result += "for (" + name + " " + "_" + name.ToLower() + " : " + name + "s) {\n";
            result += "writer.print(\"(\");\n";
            foreach (Column r in columns)
            {
                if (r.data_type.ToLower().Equals("int") || r.data_type.ToLower().Equals("decimal"))
                {
                    result += "writer.print(" + tab + "_" + name.ToLower() + ".get" + r.column_name + "());\n";
                    
                }
                else {
                    result += "writer.print(" + tab+"\"'\""+ "_" + name.ToLower() + ".get" + r.column_name + "()+\"'\");\n";

                }
                tab = "\" , \"+";
            }
            
            result += "writer.print(\"),\\n\");\n";
            result += "result ++;\n";
            result += "}\n";
            result += "writer.close();\n";
            result += "return result;\n";
            result += "}\n";
            return result;
        }

        /// <summary>
        /// Generates the batch command to load the <see cref="database"/>
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is  a batch command to load this <see cref="database"/>
        public string getBatch()
        {
            string result = "";
            result = result + "sqlcmd -S localhost -E -i " + name + ".sql\n";
            result += "ECHO .\n";
            return result;
        }
        /// <summary>
        /// Generates the create Servlet for the Java  object for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is  Java code for this object's associated create servlet.
       
        public string genCreateServelet()
        {
            string result = "";
            result += importStatements(name, settings.database_name);
            //do get
            result += commentBox.genCommentBox(name, Component_Enum.Java_Servlet_Add);
            result += genServletJavaDoc(ServletType.CreateServlet);
            result = result + "\n@WebServlet(\"/add" + name + "\")\n";
            result = result + "public class Add" + name + "Servlet extends HttpServlet{\n";

            result += initMethod();
            result += "\n @Override\n";
            result += "protected void doGet(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {\n";
            result += privLevelStatement();
            result += "session.setAttribute(\"currentPage\",req.getRequestURL());\n";
            result = result + "req.setAttribute(\"pageTitle\", \"Add " + name + "\");\n";
            foreach (Column r in columns)
            {
                if (r.foreign_key != "")
                {
                    string[] parts = r.references.Split('.');
                    //grab a list of the parents, assign them to the already existing static variable
                    result = result + "all" + parts[0] + "s = " + parts[0] + "DAO.getDistinct" + parts[0] + "ForDropdown();\n";
                    //set them to the req attribute
                    result = result + "req.setAttribute(\"" + parts[0] + "s\", all" + parts[0] + "s);\n";
                }
            }
            result = result + "req.getRequestDispatcher(\"WEB-INF/" + settings.database_name + "/Add" + name + ".jsp\").forward(req, resp);\n";
            result += "}\n";
            //this only creates the doPost method
            result += "\n";
            //gen header
            result += "@Override\n";
            result += "  protected void doPost(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {\n";
            result += privLevelStatement();
            foreach (Column r in columns)
            {
                if (r.foreign_key != "")
                {
                    string[] parts = r.references.Split('.');
                    //grab a list of the parents, assign them to the already existing static variable
                    result = result + "all" + parts[0] + "s = " + parts[0] + "DAO.getDistinct" + parts[0] + "ForDropdown();\n";
                    //set them to the req attribute
                    result = result + "req.setAttribute(\"" + parts[0] + "s\", all" + parts[0] + "s);\n";
                }
            }
            //get data
            foreach (Column r in columns)
            {
                if (r.increment == 0)
                {
                    string fieldname = "input" + name.ToLower() + r.column_name;
                    result = result + "String _" + r.column_name + " = req.getParameter(\"" + fieldname + "\");\n";
                    result += "if (_" + r.column_name + "!=null) {\n";
                    result = result + "_" + r.column_name + "=_" + r.column_name + ".trim();\n";
                    result += "}\n";
                }
            }
            //toss it in a hashmap
            result += "Map<String, String> results = new HashMap<>();\n";
            foreach (Column r in columns)
            {
                if (r.increment == 0)
                {
                    result = result + "results.put(\"" + r.column_name + "\",_" + r.column_name + ");\n";
                }
            }
            //generate an object, and set errors
            result = result + name + " " + name.ToLower() + " = new " + name + "();\n";
            result += "int errors =0;\n";
            foreach (Column r in columns)
            {
                if (r.increment == 0 && r.default_value == "")
                {
                    string errorname = name.ToLower() + r.column_name + "error";
                    result += "try {\n";
                    if (r.data_type.ToLower().Equals("int"))
                    {
                        result = result + name.ToLower() + ".set" + r.column_name + "(Integer.valueOf(_" + r.column_name + "));\n";
                    }
                    else if (r.data_type.ToLower().Contains("date") || r.data_type.ToLower().Contains("time"))
                    {
                        result = result + name.ToLower() + ".set" + r.column_name + "(LocalDate.parse(_" + r.column_name + "));\n";
                    }
                    else if (r.data_type.ToLower().Contains("decimal")) {
                        result = result + name.ToLower() + ".set" + r.column_name + "(Double.valueOf(_" + r.column_name + "));\n";

                    }
                    else
                    {
                        result = result + name.ToLower() + ".set" + r.column_name + "(_" + r.column_name + ");\n";
                    }
                    result += "} catch(Exception e) {";
                    result = result + "results.put(\"" + errorname + "\", e.getMessage());\n";
                    result += "errors++;\n";
                    result += "}\n";
                }
            }
            //add it to the databsae, maybe?
            result += "int result=0;\n";
            result += "if (errors==0){\n";
            result = result + "try{\nresult=" + name.ToLower() + "DAO.add(" + name.ToLower() + ");\n}";
            result += "catch(Exception ex){\n";
            result += "results.put(\"dbError\",\"Database Error\");\n";
            result += "}\n";
            result += "if (result>0){\n";
            result = result + "results.put(\"dbStatus\",\"" + name + " Added\");\n";
            result += "req.setAttribute(\"results\",results);\n";
            result = result + "resp.sendRedirect(\"all-" + name + "s\");\n";
            result += "return;\n";
            result += "} else {\n";
            result = result + "results.put(\"dbStatus\",\"" + name + " Not Added\");\n";
            result += "\n}\n";
            //set db message
            result += "}\n";
            //send it back
            result += "req.setAttribute(\"results\", results);\n";
            result = result + "req.setAttribute(\"pageTitle\", \"Add " + name + "\");\n";
            result = result + "req.getRequestDispatcher(\"WEB-INF/" + settings.database_name + "/Add" + name + ".jsp\").forward(req, resp);\n";
            result += "\n}\n}\n";
            //get_buttons
            //get_footer
            return result;
        }
        /// <summary>
        /// Generates the View All JSP for the Java  object for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is  Java code for this object's associated View all JSP.</returns>
        public string genviewAllJSP()
        {
            //comment box
            string result = commentBox.genCommentBox(name, Component_Enum.Java_JSP_ViewAll);
            //header comment
            //gen header
            result += "<%@include file=\"/WEB-INF/" + settings.database_name + "/personal_top.jsp\"%>\n";
            //gen form
            result += "<div class = \"container\">\n";
            result += "<div class=\"row\">\n";
            result += "<div class=\"col-12\">\n";
            result +=  "<h1>All " + settings.database_name + " " + name.Replace("_", " ") + "s</h1>\n";
            result +=  "<p>There ${" + name + "s.size() eq 1 ? \"is\" : \"are\"}&nbsp;${" + name + "s.size()} " + name.Replace("_", " ") + "${" + name + "s.size() ne 1 ? \"s\" : \"\"}</p>\n";
            result +=  "Add " + name.Replace("_", " ") + "   <a href=\"add" + name + "\">Add</a>\n";
            result +=  "<c:if test=\"${" + name + "s.size() > 0}\">\n";

            result += "<div class=\"search-container\">\n";
            result += "<form action=\"all-"+name+"\">\n";
            result += "<input type=\"text\" placeholder=\"Search..\" id=\"searchBox\" name=\"search\">\n";
            foreach (Column r in columns)
            {
                if (r.foreign_key != "")
                {
                    string[] parts = r.foreign_keys[0].Split('.');
                    string fieldname = "input" + name.ToLower() + r.column_name;
                    string errorname = name.ToLower() + r.column_name + "error";
                    
                    result = result + "<label for=\"" + fieldname + "\" class=\"form-label\">" + r.column_name + "</label>\n";
                    result += "<div class=\"input-group input-group-lg\">\n";
                    result = result + "<select  class=\"<c:if test=\"${not empty results." + errorname + "}\">is-invalid</c:if> form-control border-0 bg-light rounded-end ps-1\" placeholder=\"" + r.column_name + "\" id=\"" + fieldname + "\" name=\"" + fieldname + "\" value=\"${fn:escapeXml(results." + r.column_name + ")}\">\n";
                    result = result + "<c:forEach items=\"${" + parts[0] + "s}\" var=\"" + parts[0] + "\">\n";
                    result = result + "<option value=\"${" + parts[0] + "." + parts[1].firstCharLower() + "}\">${" + parts[0] + ".name}   </option>\n";
                    result += "</c:forEach>\n";
                    result += "</select>\n";
                    result += "";
                    result = result + "<c:if test=\"${not empty results." + errorname + "}\">\n";
                    result = result + "<div class=\"invalid-feedback\">${results." + errorname + "}</div>\n";
                    result += "</c:if>\n";
                    
                    result += "</div>\n";
                }
            }
            result += "<button type=\"submit\"><i class=\"fa fa-search\"></i></button>\n";
            result += "</form>\n";
            result += "</div>\n";
            result += "";


            result += "Export " + name.Replace("_", " ") + "   <a href=\"export" + name + "?mode=export\">Add</a>\n";
            result += "Write To SQL File " + name.Replace("_", " ") + "   <a href=\"export" + name + "?mode=SQL\">Add</a>\n";
            result += "<div class=\"table-responsive\">";
            result += "<table class=\"table table-bordered\">\n";
            result += "<thead>\n";
            result += "<tr>\n";
            foreach (Column r in columns)
            {
                if (r.default_value.ToLower().Contains("uuid") || r.increment != 0)
                {
                    result = result + "<th scope=\"col\"> Details </th>\n";
                }
                else
                {
                    result = result + "<th scope=\"col\">" + r.column_name + "</th>\n";
                }
            }
            result += "<th scope=\"col\">Edit</th>\n";
            result += "<th scope=\"col\">Delete</th>\n";
            result += "</tr>\n";
            result += "</thead>\n";
            result += "<tbody>\n";
            result = result + "<c:forEach items=\"${" + name + "s}\" var=\"" + name.ToLower() + "\">\n";
            result += "<tr id=\"${"+name.ToLower()+"."+name.ToLower()+"_ID}row\">\n";
            foreach (Column r in columns)
            {
                //https://stackoverflow.com/questions/21755757/first-character-of-string-lowercase-c-sharp
                if (!r.data_type.ToLower().Equals("bit"))
                {
                    if (r.primary_key.Equals('y') || r.primary_key.Equals('Y'))
                    {
                        if (r.default_value.ToLower().Contains("uuid") || r.increment != 0)
                        {
                            result = result + "<td><a href = \"edit" + name.ToLower() + "?" + name.ToLower() + "id=${" + name.ToLower() + "." + name.ToLower() + "_ID}&mode=view\"> Details </a></td>\n";

                        }
                        else {
                            result = result + "<td><a href = \"edit" + name.ToLower() + "?" + name.ToLower() + "id=${" + name.ToLower() + "." + name.ToLower() + "_ID}&mode=view\">${fn:escapeXml(" + name.ToLower() + "." + name.ToLower() + "_ID)}</a></td>\n";
                        }
                    }
                    else
                    {
                        result = result + "<td>${fn:escapeXml(" + name.ToLower() + "." + r.column_name.firstCharLower() + ")}</td>\n";
                    }
                }
                else
                {
                    result = result + "<td><input type=\"checkbox\" disabled <c:if test=\"${" + name.ToLower() + ".is_active}\">checked</c:if>></td>\n";
                }
            }
            result = result + "<td><a href = \"edit" + name.ToLower() + "?" + name.ToLower() + "id=${" + name.ToLower() + "." + name.ToLower() + "_ID}&mode=edit\" > Edit </a></td>\n";
            result = result + "<td><a href = \"delete" + name.ToLower() + "?" + name.ToLower() + "id=${" + name.ToLower() + "." + name.ToLower() + "_ID}" +
                "&mode=" +
                "<c:choose>" +
                "<c:when test=\"${" + name.ToLower() + ".is_active}\">0</c:when>\n" +
                "\t\t\t\t\t\t<c:otherwise>1</c:otherwise>\n" +
                "\t\t\t\t\t\t</c:choose>\">\n" +
                "<c:if test=\"${!" + name.ToLower() + ".is_active}\">un</c:if>Delete </a></td> \n";
            result += "<td>\n<div> \n";
            result += "<button class=\"delButton\" href=\"${"+name.ToLower()+"."+name.ToLower()+"_ID}\" >Delete</button> </div>\n";
            result += "<div style=\"display: none;\" id=\"" + name.ToLower()+"."+name.ToLower()+"_IDStatus\"></div>\n";

            result += " </td>\n";
            result += "</tr>\n";
            result += "</c:forEach>\n";
            result += "</tbody>\n";
            result += "</table>\n";
            result += "</div>\n";
            result += "</c:if>\n";
            result += "</div>\n";
            result += "</div>\n";
            result += "</div>\n";
            result += "<%--For displaying Previous link except for the 1st page --%>\n";
            result += "<c:if test=\"${currentPage != 1}\">\n";
            result += "<form action=\"all-"+name+"s\" method=\"get\">\n";
            foreach (Column r in columns)
            {
                if (r.foreign_key != "")
                {
                    result += "<input type=\"hidden\" name=\"" + r.column_name.ToLower() + "\" value=\"${" + r.column_name.ToLower() + "}\">\n";

                }
            }
            result += "<input type=\"hidden\" name=\"page\" value=\"${currentPage-1}\">\n";
            result += "<br/><br/>\n";
            result += "<input type=\"submit\" value=\"Previous Page\" />\n";
            result += "</form>\n";
            result += "</c:if>\n";
            result += "<%--For displaying Page numbers.\n";
            result += "The when condition does not display a link for the current page--%>";
            result += "<form action=\"all-"+name+"s\" method=\"get\" >";
            foreach (Column r in columns)
            {
                if (r.foreign_key != "")
                {
                    result += "<input type=\"hidden\" name=\"" + r.column_name.ToLower() + "\" value=\"${" + r.column_name.ToLower() + "}\">\n";

                }
            }
            result += "Select a page :\n";
            result += "<select name=\"page\" onchange=\"this.form.submit()\">\n";
            result += "<c:forEach var=\"i\" begin=\"1\" end=\"${noOfPages}\">\n";
            result += "<option value=${i}  ${currentPage == i ? ' selected' : ''} >${i}</option>\n";
            result += "</c:forEach>\n";
            result += "</select>\n";
            result += "<br/><br/>\n";
            result += "<input type=\"submit\" value=\"Submit\" />\n";
            result += "</form>\n";
            result += "<%--For displaying Next link --%>\n";
            result += "<c:if test=\"${currentPage lt noOfPages}\">\n";
            result += "<form action=\"all-Transactions\" method=\"get\">\n";
            foreach (Column r in columns)
            {
                if (r.foreign_key != "")
                {
                    result += "<input type=\"hidden\" name=\"" + r.column_name.ToLower() + "\" value=\"${" + r.column_name.ToLower() + "}\">\n";

                }
            }
            result += "<input type=\"hidden\" name=\"page\" value=\"${currentPage+1}\">\n";
            result += "<br/><br/>\n";
            result += "<input type=\"submit\" value=\"Next Page\" />\n";
            result += "</form>\n";
            result += "</c:if>\n";
            
            result += "<div id=\"dialog\" title=\"Confirmation Required\">\n";
            result += "Are you sure about this?\n";
            result += "</div>\n";
            result += "</main>\n";
            result += "<%@include file=\"/WEB-INF/" + settings.database_name + "/personal_bottom.jsp\"%>\n";            //gen_header
            //gen_fileds
            //get_buttons
            //get_footer
            return result;
        }
        /// <summary>
        /// Generates the create JSP for the Java object for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is  Java code for this object's associated create jsp.
        public string genCreateJSP()
        {
            int rowcount = 0;
            //comment box
            string result = commentBox.genCommentBox(name, Component_Enum.Java_JSP_Add);
            //header comment
            //gen header
            result += "<%@include file=\"/WEB-INF/" + settings.database_name + "/personal_top.jsp\"%>\n";
            //gen form
            result += "<div class = \"container\">\n";
            result = result + "<form method=\"post\" action=\"${appURL}/add" + name + "\" id = \"add" + name + "\" >\n";
            //gen a button for each line item
            foreach (Column r in columns)
            {
                if (!r.column_name.ToLower().Contains("active"))
                {
                    int i = 0;
                    if (r.increment == 0 && r.default_value == "")
                    {
                        if (r.foreign_keys.Count < 1 || r.foreign_keys[i] == "")
                        {
                            if (r.data_type != "bit")
                            {
                                string inputType = "text";
                                if (r.data_type == "datetime") { inputType = "date"; }
                                string fieldname = "input" + name.ToLower() + r.column_name;
                                string errorname = name.ToLower() + r.column_name + "error";
                                result = result + "<!-- " + r.column_name + " -->\n";
                                result = result + "<div class =\"row\" id = \"row" + rowcount + "\">\n";
                                result = result + "<label for=\"" + fieldname + "\" class=\"form-label\">" + r.column_name + "</label>\n";
                                result += "<div class=\"input-group input-group-lg\">\n";
                                result = result + "<input type=\"" + inputType + "\" class=\"<c:if test=\"${not empty results." + errorname + "}\">is-invalid</c:if> form-control border-0 bg-light rounded-end ps-1\" placeholder=\"" + r.column_name + "\" id=\"" + fieldname + "\" name=\"" + fieldname + "\" value=\"${fn:escapeXml(results." + r.column_name + ")}\">\n";
                                result = result + "<c:if test=\"${not empty results." + errorname + "}\">\n";
                                result = result + "<div class=\"invalid-feedback\">${results." + errorname + "}</div>\n";
                                result += "</c:if>\n";
                                result += "</div>\n";
                                result += "</div>\n";
                                rowcount++;
                            }
                            else {
                                
                                string fieldname = "input" + name.ToLower() + r.column_name;
                                string errorname = name.ToLower() + r.column_name + "error";
                                result +=   "<!-- " + r.column_name + " -->\n";
                                result +=   "<div class =\"row\" id = \"row" + rowcount + "\">\n";
                                result +=  "<label for=\"" + fieldname + "\" class=\"form-label\">" + r.column_name + "</label>\n";
                                result += "<div class=\"input-group input-group-lg\">\n";
                                result += "<input type=\"radio\" id=\"" + fieldname + "true\" name=\"" + fieldname + "\" value=\"true\">";
                                result += "<label for=\"" + fieldname + "true\">true</label><br>";
                                result += "<input type=\"radio\" id=\""+fieldname+"false\" name=\""+fieldname+"\" value=\"false\">";
                                result += "<label for=\""+fieldname+"false\">false</label><br>";
                                result +=  "<c:if test=\"${not empty results." + errorname + "}\">\n";
                                result +=  "<div class=\"invalid-feedback\">${results." + errorname + "}</div>\n";
                                result += "</c:if>\n";
                                result += "</div>\n";
                                result += "</div>\n";
                                rowcount++;

                            }
                        }
                        else
                        {
                            string[] parts = r.foreign_keys[i].Split('.');
                            string fieldname = "input" + name.ToLower() + r.column_name;
                            string errorname = name.ToLower() + r.column_name + "error";
                            result = result + "<!-- " + r.column_name + " -->\n";
                            result = result + "<div class =\"row\" id = \"row" + rowcount + "\">\n";
                            result = result + "<label for=\"" + fieldname + "\" class=\"form-label\">" + r.column_name + "</label>\n";
                            result += "<div class=\"input-group input-group-lg\">\n";
                            result = result + "<select  class=\"<c:if test=\"${not empty results." + errorname + "}\">is-invalid</c:if> form-control border-0 bg-light rounded-end ps-1\" placeholder=\"" + r.column_name + "\" id=\"" + fieldname + "\" name=\"" + fieldname + "\" value=\"${fn:escapeXml(results." + r.column_name + ")}\">\n";
                            result = result + "<c:forEach items=\"${" + parts[0] + "s}\" var=\"" + parts[0] + "\">\n";
                            result = result + "<option value=\"${" + parts[0] + "." + parts[1].firstCharLower() + "}\">${" + parts[0] + ".name}   </option>\n";
                            result += "</c:forEach>\n";
                            result += "</select>\n";
                            result += "";
                            result = result + "<c:if test=\"${not empty results." + errorname + "}\">\n";
                            result = result + "<div class=\"invalid-feedback\">${results." + errorname + "}</div>\n";
                            result += "</c:if>\n";
                            result += "</div>\n";
                            result += "</div>\n";
                            rowcount++;
                            i++;
                        }
                    }
                }
            }
            //get_buttons
            result += "<div class=\"align-items-center mt-0\">\n";
            result += "<div class=\"d-grid\">";
            result = result + "<button id=\"submitButton\" class=\"btn btn-orange mb-0\" type=\"submit\">Create " + name + "  </button></div>\n";
            result += "<c:if test=\"${not empty results.dbStatus}\"\n>";
            result += "<p>${results.dbStatus}</p>\n";
            result += "</c:if>\n";
            result += "</div>\n";
            result += "</form>\n";
            result += "</div>\n";
            //get_footer
            result += "<%@include file=\"/WEB-INF/" + settings.database_name + "/personal_bottom.jsp\"%>\n";
            return result;
        }
        /// <summary>
        /// Generates the View all Servlet for the Java  object for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is  Java code for this object's associated view all servlet.</returns>
        
        public string genviewAllServlet()
        {
            //this only creates the doGet method
            string result = commentBox.genCommentBox(name, Component_Enum.Java_Servlet_ViewAll);
            //gen header
            result += importStatements(name, settings.database_name);
            result += genServletJavaDoc(ServletType.ViewAllSErvlet);

            result += "@WebServlet(\"/all-" + name + "s\")\n";
            result += "public class All" + name + "sServlet extends HttpServlet {";
            result += initMethod();
            result += "@Override\n";
            result += "  protected void doGet(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {\n";
            result += privLevelStatement();
            result += "int errors = 0;\n";
            result += "HashMap<String,String> results = new HashMap<>();\n";
            foreach (Column r in columns)
            {
                if (r.foreign_key != "")
                {
                    result += "String _" + r.column_name.ToLower() + " = req.getParameter(\"" + r.column_name.ToLower() + "\")\n";
                    result += "if (_" + r.column_name.ToLower() + ".isEmpty()){\n";
                    result += "_" + r.column_name.ToLower() + " = \"\";\n";
                    result += "}\n";
                }
            }

            result += "int " + name.ToLower() + "count=0;\n";
            result += "int page_number=1;\n";
            result += "int page_size = 20;\n";
            result += "try {\n";
            result += "page_number = Integer.parseInt(req.getParameter(\"page\"));\n";
            result += "} catch (Exception e){\n";
            result += "page_number=1;\n";
            result += "}\n";
            result += "session.setAttribute(\"" + name.ToLower() + "_page_number\",page_number);\n";
            result += "int offset=(page_number-1)*(page_size);\n";
            result += "String search_term = req.getParameter(\"search\");\n";
            result += "if (search_term==null){\n";
            result += "search_term =\"\";\n";
            result += "}\n";
            result += "if (!search_term.equals(\"\") && (search_term.length()<2||search_term.length()>100)){\n";
            result += "errors++;\n ";
            result += "results.put(\"searchError\",\"Invalid search term\");\n";
            result += "}\n";
            result += "session.setAttribute(\"currentPage\",req.getRequestURL());\n";
            result = result + "List<" + name + "> " + name.ToLower() + "s = null;\n";
            result += "try {\n";
            foreach (Column r in columns)
            {
                if (r.foreign_key != "")
                {
                    string[] parts = r.references.Split('.');
                    //grab a list of the parents, assign them to the already existing static variable
                    result += "List<"+parts[0]+"> all" + parts[0] + "s = " + parts[0] + "DAO.getDistinct" + parts[0] + "ForDropdown();\n";
                    //set them to the req attribute
                    result +=  "req.setAttribute(\"" + parts[0] + "s\", all" + parts[0] + "s);\n";
                }
            }
            result += name.ToLower() + "_count = " + name.ToLower() + "DAO.get" + name + "Count(search_term";
            foreach (Column r in columns)
            {
                if (r.foreign_key != "")
                {
                    result += "," + r.data_type.toJavaDataType() + " " + r.column_name;
                }
            }
            result += ");\n";
            result = result + name.ToLower() + "s =" + name.ToLower() + "DAO.getAll" + name + "(page_size,offset,search_term";
            foreach (Column r in columns)
            {
                if (r.foreign_key != "")
                {
                    result += ", " + r.column_name;
                }
            }

            result += ");\n";
            result += "} catch (Exception e) {\n";
            result += name.ToLower() + "s = new ArrayList<>();\n";

            result += "}\n";
            result += "int total_pages = ("+name.ToLower()+"_count/page_size)+1;\n";
            result += "req.setAttribute(\"noOfPages\", total_pages);\n";
            result += "req.setAttribute(\"currentPage\", page_number);";

            result = result + "req.setAttribute(\"" + name + "s\", " + name.ToLower() + "s);\n";
            result = result + "req.setAttribute(\"pageTitle\", \"All " + name + "s\");\n";
            result = result + "req.getRequestDispatcher(\"WEB-INF/" + settings.database_name + "/all-" + name + "s.jsp\").forward(req,resp);\n";
            result += "\n}\n}";
            //header comment
            //gen_header
            //gen_fileds
            //get_buttons
            //get_footer
            return result;
        }
        /// <summary>
        /// Generates the delete Servlet for the Java  object for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is  Java code for this object's associated delete servlet.</returns>
        
        public string genDeleteServlet()
        {
            string result = commentBox.genCommentBox(name, Component_Enum.Java_Servlet_Delete);
            result += importStatements(name, settings.database_name);
            result += genServletJavaDoc(ServletType.DeleteServlet);
            result = result + "@WebServlet(\"/delete" + name.ToLower() + "\")\n";
            result = result + "public class Delete" + name + "Servlet extends HttpServlet {\n";
            result += initMethod();
            result += "@Override\n";
            result += "  protected void doGet(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {\n";
            result += "Map<String, String> results = new HashMap<>();\n";
            result += privLevelStatement();
            result += "session.setAttribute(\"currentPage\",req.getRequestURL());\n";
            result = result + "req.setAttribute(\"pageTitle\", \"Delete " + name + "\");\n";
            result = result + "int " + name + "ID = Integer.valueOf(req.getParameter(\"" + name.ToLower() + "id\"));\n";
            result += "int mode = Integer.valueOf(req.getParameter(\"mode\"));\n";
            result += "boolean _ajax=false;\n";
            result += "String AJAX = req.getParameter(\"AJAX\");\n";
            result += "try { \n";
            result += "_ajax = Boolean.parseBoolean(AJAX);\n";
            result += "} catch (Exception e){\n";
            result += "_ajax = false;\n";
            result += "}\n";
            result += "int result = 0;\n";
            result += "if (mode==0){\n";
            result += "try{\n";
            result = result + "result = " + name.ToLower() + "DAO.delete" + name + "(" + name + "ID);\n";
            result += "}\n";
            result += "catch(Exception ex){\n";
            result += "results.put(\"dbStatus\",ex.getMessage());\n";
            result += "}\n";
            result += "}\n";
            result += "else {\n";
            result += "try{\n";
            result = result + "result = " + name.ToLower() + "DAO.undelete" + name + "(" + name + "ID);\n";
            result += "}\n";
            result += "catch(Exception ex){\n";
            result += "results.put(\"dbStatus\",ex.getMessage());\n";
            result += "}\n";
            result += "}\n";
            result += "if (_ajax){\n";
            result += "resp.setStatus(200);\n";
            result += "resp.setContentType(\"text/plain\");\n";
            result += "PrintWriter writer=resp.getWriter();\n";
            result += "writer.write(result.toString());\n";
            result += "writer.flush();\n";
            result += "writer.close();\n";
            result += "return;\n";
            result += "}\n";
            result = result + "List<" + name + "> " + name.ToLower() + "s = null;\n";
            result = result + name.ToLower() + "s = " + name.ToLower() + "DAO.getAll" + name + "();\n";
            result += "req.setAttribute(\"results\",results);\n";
            result = result + "req.setAttribute(\"" + name + "s\", " + name.ToLower() + "s);\n";
            result = result + "req.setAttribute(\"pageTitle\", \"All " + name + "\");\n";
            result = result + "req.getRequestDispatcher(\"WEB-INF/" + settings.database_name + "/all-" + name + "s.jsp\").forward(req, resp);\n";
            result += "}\n";
            result += "}\n";
            return result;
        }
        /// <summary>
        /// Generates the edit Servlet for the Java  object for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is  Java code for this object's associated edit servlet.</returns>
        ///
        public string genViewEditServlet()
        {
            //do get
            string result = "";
            result += importStatements(name, settings.database_name);
            //do get
            result += commentBox.genCommentBox(name, Component_Enum.Java_Servlet_ViewEdit);
            result += genServletJavaDoc(ServletType.ViewEditSErvlet);
            result = result + "\n@WebServlet(\"/edit" + name + "\")\n";
            result = result + "public class Edit" + name + "Servlet extends HttpServlet{\n";
            foreach (Column r in columns)
            {
                if (r.foreign_key != "")
                {
                    string[] parts = r.references.Split('.');
                    result = result + "static List<" + parts[0] + "> all" + parts[0] + "s = " + parts[0] + "DAO.getDistinct" + parts[0] + "ForDropdown();\n";
                    //grab a list of the parents, assign and create a static variable
                }
            }
            result += initMethod();
            result += "\n @Override\n";
            result += "protected void doGet(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {\n";
            result += privLevelStatement();
            result += "String mode = req.getParameter(\"mode\");\n";
            result += "int primaryKey = -1;\n";
            result += "try{\n";
            result = result + "primaryKey = Integer.parseInt(req.getParameter(\"" + name.ToLower() + "id\"));\n";
            result += "}catch (Exception e) {\n";
            result += "req.setAttribute(\"dbStatus\",e.getMessage());\n";
            result += "}";
            result = result + name + " " + name.ToLower() + "= new " + name + "();\n";
            result += "try{\n";
            result = result + name.ToLower() + ".set" + name + "_ID(primaryKey);\n";
            result += "} catch (Exception e){\n";
            result += "req.setAttribute(\"dbStatus\",e.getMessage());\n";
            result += "resp.sendRedirect(\"all-" + name.ToLower() + "s\");\n";
            result += "return;\n";
            result += "}\n";
            result += "try{\n";
            result = result + name.ToLower() + "=" + name.ToLower() + "DAO.get" + name + "ByPrimaryKey(" + name.ToLower() + ");\n";
            result += "} catch (SQLException e) {\n";
            result += "req.setAttribute(\"dbStatus\",e.getMessage());\n";
            result += name.ToLower() + "= null;\n";
            result += "}\n";
            result += "if (" + name.ToLower() + "==null || " + name.ToLower() + ".get" + columns[0].column_name + "==null){\n";
            result += "resp.sendRedirect(\"all-" + name + "s\");\n";
            result += "return;\n";
            result += "}\n";
            result = result + "session.setAttribute(\"" + name.ToLower() + "\"," + name.ToLower() + ");\n";
            result += "req.setAttribute(\"mode\",mode);\n";
            result += "session.setAttribute(\"currentPage\",req.getRequestURL());\n";
            result = result + "req.setAttribute(\"pageTitle\", \"Edit " + name + "\");\n";
            foreach (Column r in columns)
            {
                if (r.foreign_key != "")
                {
                    string[] parts = r.references.Split('.');
                    //grab a list of the parents, assign them to the already existing static variable
                    result = result + " all" + parts[0] + "s = " + parts[0] + "DAO.getDistinct" + parts[0] + "ForDropdown();\n";
                    //set them to the req attribute
                    result = result + "req.setAttribute(\"" + parts[0] + "s\", all" + parts[0] + "s);\n";
                }
            }
            result = result + "req.getRequestDispatcher(\"WEB-INF/" + settings.database_name + "/Edit" + name + ".jsp\").forward(req, resp);\n";
            result += "}\n";
            result += " @Override\n";
            result += "protected void doPost(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {\n";
            result += privLevelStatement();
            result += "Map<String, String> results = new HashMap<>();\n";
            result += "String mode = req.getParameter(\"mode\");\n";
            result += "req.setAttribute(\"mode\",mode);\n";
            result += "//to set the drop downs\n";
            foreach (Column r in columns)
            {
                if (r.foreign_key != "")
                {
                    string[] parts = r.references.Split('.');
                    //grab a list of the parents, assign them to the already existing static variable
                    result = result + " all" + parts[0] + "s = " + parts[0] + "DAO.getDistinct" + parts[0] + "ForDropdown();\n";                    //set them to the req attribute
                    result = result + "req.setAttribute(\"" + parts[0] + "s\", all" + parts[0] + "s);\n";
                }
            }
            result = result + "//to get the old " + name + "\n";

            result = result + name + " _old" + name + "= (" + name + ")session.getAttribute(\"" + name.ToLower() + "\");\n";
            result += "if (_old"+name+"==null){\n";
            result += "resp.sendRedirect(\"all-"+name.ToLower()+"s\");\n";
            result += "return;\n";
            result += "}\n";
            

            result += "//to get the new event's info\n";
            foreach (Column r in columns)
            {
                if (r.increment == 0)
                {
                    string fieldname = "input" + name.ToLower() + r.column_name;
                    result = result + "String _" + r.column_name + " = req.getParameter(\"" + fieldname + "\");\n";
                    result += "if (_" + r.column_name + "!=null){\n";
                    result = result + "_" + r.column_name + "=_" + r.column_name + ".trim();\n";
                    result += "}\n";
                }
            }
            //toss it in a hashmap
            foreach (Column r in columns)
            {
                if (r.increment == 0)
                {
                    result = result + "results.put(\"" + r.column_name + "\",_" + r.column_name + ");\n";
                }
            }
            //generate the object
            result = result + name + " _new" + name + " = new " + name + "();\n";
            result += "int errors =0;\n";
            foreach (Column r in columns)
            {
                if (r.increment == 0)
                {
                    string errorname = name.ToLower() + r.column_name + "error";
                    result += "try {\n";
                    if (r.data_type.ToLower().Equals("int"))
                    {
                        result = result + " _new" + name + ".set" + r.column_name + "(Integer.valueOf(_" + r.column_name + "));\n";
                    }
                    else if (r.data_type.ToLower().Contains("date")|| r.data_type.ToLower().Contains("time"))
                    {
                        result = result + " _new" + name + ".set" + r.column_name + "(LocalDate.parse(_" + r.column_name + "));\n";
                    }
                    else if (r.data_type.ToLower().Contains("decimal") )
                    {
                        result = result + " _new" + name + ".set" + r.column_name + "(Double.valueOf(_" + r.column_name + "));\n";
                    }

                    else
                    {
                        result = result + " _new" + name + ".set" + r.column_name + "(_" + r.column_name + ");\n";
                    }
                    result += "} catch(Exception e) {";
                    result = result + "results.put(\"" + errorname + "\", e.getMessage());\n";
                    result += "errors++;\n";
                    result += "}\n";
                }
            }

            result += "//to update the database\n";
            result += "int result=0;\n";
            result += "if (errors==0){\n";
            result += "try{\n";
            result = result + "result=" + name.ToLower() + "DAO.update(_old" + name + ",_new" + name + ");\n";
            result += "}catch(Exception ex){\n";
            result += "results.put(\"dbError\",\"Database Error\");\n";
            result += "}\n";
            result += "if (result>0){\n";
            result = result + "results.put(\"dbStatus\",\"" + name + " updated\");\n";
            result += "req.setAttribute(\"results\",results);\n";
            result = result + "resp.sendRedirect(\"all-" + name + "s\");\n";
            result += "return;\n";
            result += "} else {\n";
            result = result + "results.put(\"dbStatus\",\"" + name + " Not Updated\");\n";
            result += "}\n}\n";
            result += "//standard\n";
            result += "req.setAttribute(\"results\", results);\n";
            result = result + "req.setAttribute(\"pageTitle\", \"Edit " + name + "\");\n";
            result = result + "req.getRequestDispatcher(\"WEB-INF/" + settings.database_name + "/Edit" + name + ".jsp\").forward(req, resp);\n";
            result += "}\n}\n";
            return result;
        }
        /// <summary>
        /// Generates the View Single/Edit JSP for the Java  object for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is  Java code for this object's associated View Single/Edit JSP.</returns>
        
        public string genViewEditJSP()
        {
            int rowcount = 0;
            //comment box
            string result = commentBox.genCommentBox(name, Component_Enum.Java_JSP_ViewEdit);
            //header comment
            //gen header
            result += "<%@include file=\"/WEB-INF/" + settings.database_name + "/personal_top.jsp\"%>\n";
            //gen form
            result += "<div class = \"container\">\n";
            result = result + "<form method=\"post\" action=\"${appURL}/edit" + name + "\" id = \"edit" + name + "\" >\n";
            //gen a button for each line item
            foreach (Column r in columns)
            {
                if (!r.column_name.ToLower().Contains("active"))
                {
                    int i = 0;
                    if (r.increment != 0 || r.default_value.ToLower().Contains("uuid")) {
                        result = result + "<!-- " + r.column_name + " -->\n";
                        result = result + "<div class =\"row\" id = \"row" + rowcount + "\">\n";
                        result = result + "<h2>" + columns[i+1].column_name + "  :  \n";
                        result = result + " ${fn:escapeXml(" + name.ToLower() + "." + columns[i+1].column_name.firstCharLower() + ")}</h2>\n";
                        result += "</div>\n";
                        rowcount++;

                        continue;
                    }
                    if (r.primary_key.Equals('Y') || r.primary_key.Equals('y'))
                    {
                        result = result + "<!-- " + r.column_name + " -->\n";
                        result = result + "<div class =\"row\" id = \"row" + rowcount + "\">\n";
                        result = result + "<h2>" + r.column_name + "  :  \n";
                        result = result + " ${fn:escapeXml(" + name.ToLower() + "." + r.column_name.firstCharLower() + ")}</h2>\n";
                        result += "</div>\n";
                        rowcount++;
                        continue;
                    }
                    if ((r.foreign_keys.Count < 1 || r.foreign_keys[i] == ""))
                    {
                        if (r.data_type != "bit")
                        {
                            string inputType = "text";
                            if (r.data_type == "datetime") { inputType = "date"; }
                            string fieldname = "input" + name.ToLower() + r.column_name;
                            string errorname = name.ToLower() + r.column_name + "error";
                            result = result + "<!-- " + r.column_name + " -->\n";
                            result = result + "<div class =\"row\" id = \"row" + rowcount + "\">\n";
                            result = result + "<label for=\"" + fieldname + "\" class=\"form-label\">" + r.column_name + "</label>\n";
                            result += "<div class=\"input-group input-group-lg\">\n";
                            result = result + "<input type=\"" + inputType + "\" class=\"<c:if test=\"${not empty results." + errorname + "}\">is-invalid</c:if> form-control border-0 bg-light rounded-end ps-1\" placeholder=\"" + r.column_name + "\" <c:if test=\"${mode eq 'view'}\"> disabled </c:if>  id=\"" + fieldname + "\" name=\"" + fieldname + "\" value=\"${fn:escapeXml(" + name.ToLower() + "." + r.column_name.firstCharLower() + ")}\">\n";
                            result = result + "<c:if test=\"${not empty results." + errorname + "}\">\n";
                            result = result + "<div class=\"invalid-feedback\">${results." + errorname + "}</div>\n";
                            result += "</c:if>\n";
                            result += "</div>\n";
                            result += "</div>\n";
                            rowcount++;
                        }
                        else {
                            string fieldname = "input" + name.ToLower() + r.column_name;
                            string errorname = name.ToLower() + r.column_name + "error";
                            result += "<!-- " + r.column_name + " -->\n";
                            result += "<div class =\"row\" id = \"row" + rowcount + "\">\n";
                            result += "<label for=\"" + fieldname + "\" class=\"form-label\">" + r.column_name + "</label>\n";
                            result += "<div class=\"input-group input-group-lg\">\n";
                            result += "<input type=\"radio\" id=\"" + fieldname + "true\" name=\"" + fieldname + "\" value=\"true\">";
                            result += "<label for=\"" + fieldname + "true\">true</label><br>";
                            result += "<input type=\"radio\" id=\"" + fieldname + "false\" name=\"" + fieldname + "\" value=\"false\">";
                            result += "<label for=\"" + fieldname + "false\">false</label><br>";
                            result += "<c:if test=\"${not empty results." + errorname + "}\">\n";
                            result += "<div class=\"invalid-feedback\">${results." + errorname + "}</div>\n";
                            result += "</c:if>\n";
                            result += "</div>\n";
                            result += "</div>\n";
                            rowcount++;
                        }
                    }
                    else
                    {
                        string[] parts = r.foreign_keys[i].Split('.');
                        string fieldname = "input" + name.ToLower() + r.column_name;
                        string errorname = name.ToLower() + r.column_name + "error";
                        result = result + "<!-- " + r.column_name + " -->\n";
                        result = result + "<div class =\"row\" id = \"row" + rowcount + "\">\n";
                        result = result + "<label for=\"" + fieldname + "\" class=\"form-label\">" + r.column_name + "</label>\n";
                        result += "<div class=\"input-group input-group-lg\">\n";
                        result = result + "<select  class=\"<c:if test=\"${not empty results." + errorname + "}\">is-invalid</c:if> form-control border-0 bg-light rounded-end ps-1\"  <c:if test=\"${mode eq 'view'}\"> disabled </c:if>  id=\"" + fieldname + "\" name=\"" + fieldname + "\" value=\"${fn:escapeXml(" + name.ToLower() + "." + r.column_name.firstCharLower() + ")}\">\n";
                        result = result + "<c:forEach items=\"${" + parts[0] + "s}\" var=\"" + parts[0] + "\">\n";
                        result = result + "<option value=\"${" + parts[0] + "." + parts[1].firstCharLower() + "}\"" +
                        "<c:if test=\"${" + name.ToLower() + "." + parts[1].firstCharLower() + " eq " + parts[0] + "." + parts[1].firstCharLower() + "}\"> selected </c:if>>${" + parts[0] + ".name}   </option>\n";
                        result += "</c:forEach>\n";
                        result += "</select>\n";
                        result += "";
                        result = result + "<c:if test=\"${not empty results." + errorname + "}\">\n";
                        result = result + "<div class=\"invalid-feedback\">${results." + errorname + "}</div>\n";
                        result += "</c:if>\n";
                        result += "</div>\n";
                        result += "</div>\n";
                        rowcount++;
                        i++;
                    }
                }
            }
            //get_buttons
            result += "<div class=\"align-items-center mt-0\">\n";
            result += "<div class=\"d-grid\">";
            result = result + "<button id=\"submitButton\" class=\"btn btn-orange mb-0\" type=\"submit\">Edit " + name + " </button></div>\n";
            result += "<c:if test=\"${not empty results.dbStatus}\"\n>";
            result += "<p>${results.dbStatus}</p>\n";
            result += "</c:if>\n";
            result += "</div>\n";
            result += "</form>\n";
            result += "</div>\n";
            //get_footer
            result += "<%@include file=\"/WEB-INF/" + settings.database_name + "/personal_bottom.jsp\"%>\n";
            return result;
        }

        public string genUploadJSP() {
            string result = "";
            
            return result;
        }

        public string genUploadServlet()
        {
            string result = "";
            result += importStatements(name, settings.database_name);
            
            //change comment box
            //settings
            result += commentBox.genCommentBox(name, Component_Enum.Java_Servlet_Add);
            result += genServletJavaDoc(ServletType.UploadServlet);
            result = result + "\n@WebServlet(\"/upload" + name + "\")\n";
            result += "@MultipartConfig(\n";
            result += "fileSizeThreshold = 1024 * 1024, // 1 MB\n";
            result += "maxFileSize = 1024 * 1024 * 10,      // 10 MB\n";
            result += "maxRequestSize = 1024 * 1024 * 100   // 100 MB\n";
            result += ")\n";
            result += "public class upload_" + name + "servlet extends HttpServlet {\n";
            result += "private static final String UPLOAD_DIR = \"uploads\";\n";
            result += "private i" + name + "DAO " + name.ToLower() + "DAO;\n";
            //init
            result += "@Override\n";
            result += "public void init()  {\n";
            result += name.ToLower() + "DAO = new " + name + "DAO();\n";
            result += "}\n";
            result += "public void init( i"+name+"DAO "+name.ToLower()+"DAO){\n";
            result += "this."+name.ToLower()+"DAO = "+name.ToLower()+"DAO;\n";
            result += "}\n";

            //doget
            result += "@Override\n";
            result += "protected void doGet(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {\n";
            result += privLevelStatement();
            result += "session.setAttribute(\"currentPage\",req.getRequestURL());\n";
            result = result + "req.setAttribute(\"pageTitle\", \"Upload " + name + "\");\n";
            result += "req.getRequestDispatcher(\"WEB-INF/"+settings.database_name+"/upload_"+name+".jsp\").forward(req, resp);\n";
            result += "}\n";

            //dopost
            result += "@Override\n";
            result += "protected void doPost(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {\n";
            result += privLevelStatement();
            result += "String applicationPath = req.getServletContext().getRealPath(\"\");\n";
            result += "String uploadFilePath = applicationPath +  UPLOAD_DIR;\n";
            result += "File fileSaveDir = new File(uploadFilePath);\n";
            result += "if (!fileSaveDir.exists()) {\n";
            result += "fileSaveDir.mkdirs();\n";
            result += "}\n";
            result += "Map<String, String> results = new HashMap<>();\n";
            result += "String fileName = \"\";\n";
            result += "Part filePart = req.getPart(\"upload_transactions\");\n";
            result += "Collection<Part> x = req.getParts();\n";
            result += "if (filePart!=null) {\n";
            result += "fileName = filePart.getSubmittedFileName();\n";
            result += "File checkFile = new File(uploadFilePath + File.separator + fileName);\n";
            result += "if (checkFile.exists()) {\n";
            result += "checkFile.delete();\n";
            result += "}\n";
            result += "}\n";
            result += "else {\n";
            result += "results.put(\"FileEmptyError\", \"File is empty\");";
            result += "}\n";
            result += "try { \n";
            result += "for (Part part : req.getParts()) {\n";
            result += "part.write(uploadFilePath + File.separator + fileName);\n";
            result += "}\n";
            result += "} catch (Exception ex){\n";
            result += "results.put(\"dbStatus\",ex.getMessage());\n";
            result += "req.setAttribute(\"results\", results);\n";
            result += "req.setAttribute(\"pageTitle\", \"Upload a file \");\n";
            result += "req.getRequestDispatcher(\"WEB-INF/"+settings.database_name+"/upload_"+name.ToLower()+".jsp\").forward(req, resp);\n";
            result += "return;\n";
            result += "}\n"; //line 93
            result += "File uploadedFile = new File(uploadFilePath + File.separator + fileName);\n";
            result += "List<"+name+"> "+name.ToLower()+"s = null;\n";
            result += "try {\n";
            result += name.ToLower()+"s = "+name.ToLower()+"DAO.read"+name+"sFromFile(uploadedFile);\n";
            result += "} catch (Exception ex){\n";
            result += "results.put(\"dbStatus\",ex.getMessage());\n";
            result += "req.setAttribute(\"results\", results);\n";
            result += "req.setAttribute(\"pageTitle\", \"Upload a file \");\n";
            result += "req.getRequestDispatcher(\"WEB-INF/"+settings.database_name+"/upload_" + name.ToLower() + ".jsp\").forward(req, resp);\n";
            result += "return;\n";
            result += "}\n"; //line 106
            result += "int new"+name.ToLower()+"s = 0;\n";
            result += "int old" + name.ToLower() + "s = 0;\n";
            result += "int total" + name.ToLower() + "s = " + name.ToLower() + "s.size();\n";
            result += "try { \n";
            result += "new" + name.ToLower() + "s= " + name.ToLower() + "DAO.addBatchOf"+name+"s(" + name.ToLower() + "s);\n";
            result += "} catch (Exception e) {\n";
            result += "results.put(\"dbStatus\",e.getMessage());\n";
            result += "}\n"; //line 116
            result += "old"+name.ToLower()+"s= total"+name.ToLower()+"s- new"+name.ToLower()+"s;\n";
            result += "uploadedFile.delete();\n";
            result += "results.put(\"AddedCount\",\"You uploaded \"+total"+name.ToLower()+"s+\" "+name.ToLower()+"s. \"+new"+ name.ToLower() + "s+\" of them were new. \"+ old"+ name.ToLower() + "s+\" of them were old, and not added to the database.\");\n";
            result += "session.setAttribute(\"results\",results);\n";
            result += "resp.sendRedirect(\"home\");\n";
            result += "return;\n";
            result += "}\n";
            result += "}\n";




            //result += "session.setAttribute(\"currentPage\",req.getRequestURL());\n";
            //result = result + "req.setAttribute(\"pageTitle\", \"Upload " + name + "\");\n";
            //result += "req.getRequestDispatcher(\"WEB-INF/Budget_App/upload_" + name + ".jsp\").forward(req, resp);";

            return result;
        }

        

        public string genExportServlet()
        {
            string result = "";
            result += importStatements(name, settings.database_name);
            
            //change comment box
            //settings
            result += commentBox.genCommentBox(name, Component_Enum.Java_Servlet_Add);
            result += genServletJavaDoc(ServletType.ExportServlet);
            result += "\n@WebServlet(\"/export" + name + "\")\n";
            result += "public class Export"+name+"Servlet extends HttpServlet {\n";
            result += "private i"+name+"DAO "+name.ToLower()+"DAO;\n";
            result += "private static final String UPLOAD_DIR = \"uploads\";\n";
            result += "private ServletFileUpload uploader = null;\n";
            result += "@Override\n";
            result += "public void init() throws ServletException {\n";
            result += name.ToLower() + "DAO = new " + name + "DAO();\n";
            result += "DiskFileItemFactory fileFactory = new DiskFileItemFactory();\n";
            result += "File filesDir = (File) getServletContext().getAttribute(\"FILES_DIR_FILE\");\n";
            result += "fileFactory.setRepository(filesDir);\n";
            result += "this.uploader = new ServletFileUpload(fileFactory);\n";
            result += "}\n";
            result += "public void init(i" + name + "DAO " + name.ToLower() + "DAO) {\n"; ;
            result += "this."+name.ToLower()+"DAO = "+name.ToLower()+"DAO;\n";
            result += "}\n";
            result += "";
            //do get
            result += "@Override\n";
            result += "protected void doGet(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {\n";
            result += "String applicationPath = req.getServletContext().getRealPath(\"\");\n";
            result += "String uploadFilePath = applicationPath + File.separator + UPLOAD_DIR;\n";
            result += privLevelStatement();
            result += "String filename = \"output_\"+user.getUser_Name()+\"_"+name+"\"+\".txt\";\n";
            result += "String full_file = uploadFilePath + File.separator + filename;\n";
            result += "String mode = req.getParameter(\"mode\");\n";
            result += "session.setAttribute(\"currentPage\",req.getRequestURL());\n";
            result += "List<"+name+"> "+name.ToLower()+"s = null;\n";
            result += "boolean error = false;\n";
            result += "try {\n";
            result += name.ToLower()+"s = "+name.ToLower()+"DAO.getAll"+name+"();\n";
            result += "} catch (SQLException e) {\n";
            result += "error = true;\n";
            result += "}\n";
            result += "try {\n";
            result += "if (mode.equals(\"export\")){\n";
            result += name.ToLower()+"DAO.write"+name+"ToFile("+name.ToLower()+"s, full_file);\n";
            result += "}\n";
            result += "if (mode.equals(\"SQL\")){\n";
            result += name.ToLower() + "DAO.write" + name + "ToSQLInsert(" + name.ToLower() + "s, full_file);\n";
            result += "}\n";
            result += "} catch (Exception e) {\n";
            result += " error = true;\n";
            result += "}\n";
            
            result += "resp.setContentType(\"APPLICATION/OCTET-STREAM\");\n";
            result += "resp.setHeader(\"Content-Disposition\", \"attachment; filename=\\\"\" + filename + \"\\\"\");\n";
            result += "java.io.FileInputStream fileInputStream = new java.io.FileInputStream(uploadFilePath +File.separator+ filename);\n";
            result += "int i;\n";
            result += "while ((i=fileInputStream.read()) != -1) {\n";
            result += "resp.getOutputStream().write(i);\n";
            result += "}\n";
            result += "fileInputStream.close();\n";
            result += "File delFile = new File(uploadFilePath+File.separator+filename);\n";
            result += "delFile.delete();\n";
            result += "}\n";
            result += "}\n";




            return result;
        }

        /// <summary>
        /// Generates the View all with line items JSP for the Java  object for this <see cref="table"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is  JSP code for this object's associated view edit with line items.</returns>

        public string genViewEditWithLineItemsJSP()
        {
            int rowcount = 0;
            //comment box
            string result = commentBox.genCommentBox(name, Component_Enum.Java_JSP_ViewEdit);
            //header comment
            //gen header
            result += "<%@include file=\"/WEB-INF/" + settings.database_name + "/personal_top.jsp\"%>\n";
            //gen form for main item
            result += "<div class = \"container\">\n";
            result = result + "<form method=\"post\" action=\"${appURL}/edit" + name + "\" id = \"edit" + name + "\" >\n";
            //gen a button for each line item
            foreach (Column r in columns)
            {
                if (!r.column_name.ToLower().Contains("active"))
                {
                    int i = 0;
                    if (r.primary_key.Equals('Y') || r.primary_key.Equals('y'))
                    {
                        result = result + "<!-- " + r.column_name + " -->\n";
                        result = result + "<div class =\"row\" id = \"row" + rowcount + "\">\n";
                        result = result + "<h2>" + r.column_name + "  :  \n";
                        result = result + " ${fn:escapeXml(" + name.ToLower() + "." + r.column_name.firstCharLower() + ")}</h2>\n";
                        result += "</div>\n";
                        rowcount++;
                        continue;
                    }
                    if ((r.foreign_keys.Count < 1 || r.foreign_keys[i] == ""))
                    {
                        string inputType = "text";
                        if (r.data_type == "datetime") { inputType = "date"; }
                        string fieldname = "input" + name.ToLower() + r.column_name;
                        string errorname = name.ToLower() + r.column_name + "error";
                        result = result + "<!-- " + r.column_name + " -->\n";
                        result = result + "<div class =\"row\" id = \"row" + rowcount + "\">\n";
                        result = result + "<label for=\"" + fieldname + "\" class=\"form-label\">" + r.column_name + "</label>\n";
                        result += "<div class=\"input-group input-group-lg\">\n";
                        result = result + "<input type=\"" + inputType + "\" class=\"<c:if test=\"${not empty results." + errorname + "}\">is-invalid</c:if> form-control border-0 bg-light rounded-end ps-1\" placeholder=\"" + r.column_name + "\" <c:if test=\"${mode eq 'view'}\"> disabled </c:if>  id=\"" + fieldname + "\" name=\"" + fieldname + "\" value=\"${fn:escapeXml(" + name.ToLower() + "." + r.column_name.firstCharLower() + ")}\">\n";
                        result = result + "<c:if test=\"${not empty results." + errorname + "}\">\n";
                        result = result + "<div class=\"invalid-feedback\">${results." + errorname + "}</div>\n";
                        result += "</c:if>\n";
                        result += "</div>\n";
                        result += "</div>\n";
                        rowcount++;
                    }
                    else
                    {
                        string[] parts = r.foreign_keys[i].Split('.');
                        string fieldname = "input" + name.ToLower() + r.column_name;
                        string errorname = name.ToLower() + r.column_name + "error";
                        result = result + "<!-- " + r.column_name + " -->\n";
                        result = result + "<div class =\"row\" id = \"row" + rowcount + "\">\n";
                        result = result + "<label for=\"" + fieldname + "\" class=\"form-label\">" + r.column_name + "</label>\n";
                        result += "<div class=\"input-group input-group-lg\">\n";
                        result = result + "<select  class=\"<c:if test=\"${not empty results." + errorname + "}\">is-invalid</c:if> form-control border-0 bg-light rounded-end ps-1\"  <c:if test=\"${mode eq 'view'}\"> disabled </c:if>  id=\"" + fieldname + "\" name=\"" + fieldname + "\" value=\"${fn:escapeXml(" + name.ToLower() + "." + r.column_name.firstCharLower() + ")}\">\n";
                        result = result + "<c:forEach items=\"${" + parts[0] + "s}\" var=\"" + parts[0] + "\">\n";
                        result = result + "<option value=\"${" + parts[0] + "." + parts[1].firstCharLower() + "}\"" +
                        "<c:if test=\"${" + name.ToLower() + "." + parts[1].firstCharLower() + " eq " + parts[0] + "." + parts[1].firstCharLower() + "}\"> selected </c:if>>${" + parts[0] + ".name}   </option>\n";
                        result += "</c:forEach>\n";
                        result += "</select>\n";
                        result += "";
                        result = result + "<c:if test=\"${not empty results." + errorname + "}\">\n";
                        result = result + "<div class=\"invalid-feedback\">${results." + errorname + "}</div>\n";
                        result += "</c:if>\n";
                        result += "</div>\n";
                        result += "</div>\n";
                        rowcount++;
                        i++;
                    }
                }
            }
            //get_buttons
            result += "<div class=\"align-items-center mt-0\">\n";
            result += "<div class=\"d-grid\">";
            result = result + "<button class=\"btn btn-orange mb-0\" type=\"submit\">Edit " + name + " </button></div>\n";
            result += "<c:if test=\"${not empty results.dbStatus}\"\n>";
            result += "<p>${results.dbStatus}</p>\n";
            result += "</c:if>\n";
            result += "</div>\n";
            result += "</form>\n";
            result += "</div>\n";
            //to gen line items
            foreach (foreignKey key in data_tables.all_foreignKey)
            {
                if (key.referenceTable.ToLower().Equals(name.ToLower()))
                {
                    result += "<div class = \"container\">\n";
                    result += "<div class=\"row\">\n";
                    result += "<div class=\"col-12\">\n";
                    result = result + "<h1>All " + settings.database_name + " " + name.Replace("_", " ") + " " + key.mainTable.Replace("_", " ") + "s</h1>\n";
                    result = result + "<p>There ${" + name + "." + key.mainTable + "s.size() eq 1 ? \"is\" : \"are\"}&nbsp;${" + name + "." + key.mainTable + "s.size()} " + name.Replace("_", " ") + "${" + name + "." + key.mainTable + "s.size() ne 1 ? \"s\" : \"\"}</p>\n";
                    result =
                    result = result + "<c:if test=\"${" + name + "." + key.mainTable + "s.size() > 0}\">\n";
                    result += "<div class=\"table-responsive\">";
                    result += "<table class=\"table table-bordered\">\n";
                    result += "<thead>\n";
                    result += "<tr>\n";
                    foreach (table t in data_tables.all_tables)
                    {
                        if (t.name.ToLower().Equals(key.mainTable.ToLower()))
                        {
                            foreach (Column r in t.columns)
                            {
                                result = result + "<th scope=\"col\">" + r.column_name + "</th>\n";
                            }
                            result += "<th scope=\"col\">Edit</th>\n";
                            result += "<th scope=\"col\">Delete</th>\n";
                            result += "</tr>\n";
                            result += "</thead>\n";
                            result += "<tbody>\n";
                            result = result + "<c:forEach items=\"${" + name + "." + key.mainTable + "s}\" var=\"" + key.mainTable.ToLower() + "\">\n";
                            result += "<tr>\n";
                            result += "<form method=\"post\" action=\"${appURL}/delete" + t.name + "\">";
                            foreach (Column r in t.columns)
                            {

                                //https://stackoverflow.com/questions/21755757/first-character-of-string-lowercase-c-sharp
                                if (!r.data_type.ToLower().Equals("bit"))
                                {
                                    result = result + "<td>${fn:escapeXml(" + key.mainTable.ToLower() + "." + r.column_name.firstCharLower() + ")}</td>\n";
                                }
                                else
                                {
                                    result = result + "<td><input type=\"checkbox\" disabled <c:if test=\"${" + key.mainTable.ToLower() + ".is_active}\">checked</c:if>></td>\n";
                                }
                            }
                            result = result + "<td><a href = \"edit" + key.mainTable.ToLower() + "?" + name.ToLower() + "id=${" + name.ToLower() + "." + name.ToLower() + "_ID}&mode=edit\" > Edit </a></td>\n";
                            result = result + "<td><button class=\"btn btn-orange mb-0\" type=\"submit\">Delete  </button> </td>\n";

                            result += "</tr>\n";
                            result += "</c:forEach>\n";
                            //the adding row
                            result += "<tr>\n";
                            result = result + "<form method=\"post\" action=\"${appURL}/add" + t.name + "\" id = \"add" + name + "\" >\n";
                            //gen a button for each line item
                            foreach (Column r in t.columns)
                            {
                                result += "<td>";

                                int i = 0;
                                if (r.increment == 0)
                                {
                                    if (r.foreign_keys.Count < 1 || r.foreign_keys[i] == "")
                                    {
                                        string inputType = "text";
                                        if (r.data_type == "datetime") { inputType = "date"; }
                                        string fieldname = "input" + name.ToLower() + r.column_name;
                                        string errorname = name.ToLower() + r.column_name + "error";
                                        result = result + "<!-- " + r.column_name + " -->\n";

                                        result = result + "<input type=\"" + inputType + "\" class=\"<c:if test=\"${not empty results." + errorname + "}\">is-invalid</c:if> form-control border-0 bg-light rounded-end ps-1\" placeholder=\"" + r.column_name + "\" id=\"" + fieldname + "\" name=\"" + fieldname + "\" value=\"${fn:escapeXml(results." + r.column_name + ")}\">\n";
                                        result = result + "<c:if test=\"${not empty results." + errorname + "}\">\n";
                                        result = result + "<div class=\"invalid-feedback\">${results." + errorname + "}</div>\n";
                                        result += "</c:if>\n";

                                        rowcount++;
                                    }
                                    else
                                    {
                                        string[] parts = r.foreign_keys[i].Split('.');
                                        string fieldname = "input" + name.ToLower() + r.column_name;
                                        string errorname = name.ToLower() + r.column_name + "error";
                                        result = result + "<!-- " + r.column_name + " -->\n";

                                        result = result + "<select  class=\"<c:if test=\"${not empty results." + errorname + "}\">is-invalid</c:if> form-control border-0 bg-light rounded-end ps-1\" placeholder=\"" + r.column_name + "\" id=\"" + fieldname + "\" name=\"" + fieldname + "\" value=\"${fn:escapeXml(results." + r.column_name + ")}\">\n";
                                        result = result + "<c:forEach items=\"${" + parts[0] + "s}\" var=\"" + parts[0] + "\">\n";
                                        result = result + "<option value=\"${" + parts[0] + "." + parts[1].firstCharLower() + "}\">${" + parts[0] + ".name}   </option>\n";
                                        result += "</c:forEach>\n";
                                        result += "</select>\n";
                                        result += "";
                                        result = result + "<c:if test=\"${not empty results." + errorname + "}\">\n";
                                        result = result + "<div class=\"invalid-feedback\">${results." + errorname + "}</div>\n";
                                        result += "</c:if>\n";

                                        rowcount++;
                                        i++;

                                    }
                                    result += "</td>\n";
                                }
                            }
                            //get_buttons

                            result = result + "<td><button class=\"btn btn-orange mb-0\" type=\"submit\">Create " + t.name + "  </button></td>\n";
                            result += "</form>\n";
                            result += "</tr>\n";

                            result += "</tbody>\n";
                            result += "</table>\n";
                            result += "</div>\n";
                            result += "</c:if>\n";
                            result += "</div>\n";
                            result += "</div>\n";
                            result += "</div>\n";
                        }
                    }
                }
            }

            result += "</main>\n";
            result += "<%@include file=\"/WEB-INF/" + settings.database_name + "/personal_bottom.jsp\"%>\n";            //gen_header
            //gen_fileds
            //get_buttons
            //get_footer
            return result;

        }
        /// <summary>
        /// Generates the index JSP with links to each tables view all JSP for this <see cref="database"/> 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is  JSP for a project homepage.</returns>
        public string genIndexJSP()
        {
            string result = "<%@include file=\"/WEB-INF/personal-project/personal_top.jsp\"%>\n";
            result += "<div class=\"table-responsive\"><table class=\"table table-bordered\">\n";
            result += "<thead>\n";
            result += "<tr>\n";
            result += "<th scope=\"col\">Table</th>\n";
            result += " <th scope=\"col\">Action</th>\n";
            result += "</tr>\n";
            result += "</thead>\n";
            result += "<tbody>\n";
            foreach (table t in data_tables.all_tables)
            {
                result = result + "<tr><td>View all " + t.name + "</td><td><a href=\"all-" + t.name + "s\"> View </a> </td></tr>\n";
            }
            result += "</tbody>\n";
            result += "</table>\n";
            result += "</div>\n";
            result += "<%@include file=\"/WEB-INF/personal-project/personal_bottom.jsp\"%>\n";
            return result;
        }
        /// <summary>
        /// Generates the standard important statements for the Java Servlet. 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is  Java code for standard import statements.</returns>
        private string importStatements(string objectname, string projectName)
        {
            string result = "\n";
            result = result + "import com." + settings.owner_name + "." + projectName + ".data." + name + "DAO;\n";
            result = result + "import com." + settings.owner_name + "." + projectName + ".models." + name + ";\n";
            result += "import com." + settings.owner_name + "." + projectName + ".models.User;\n";
            result += "import com." + settings.owner_name + "." + projectName + ".iData.i" + objectname + "DAO;\n";
            result += "import jakarta.servlet.ServletException;\n";
            result += "import jakarta.servlet.annotation.WebServlet;\n";
            result += "import jakarta.servlet.http.HttpServlet;\n";
            result += "import jakarta.servlet.http.HttpServletRequest;\n";
            result += "import jakarta.servlet.http.HttpServletResponse;\n";
            result += "import jakarta.servlet.http.HttpSession;\n";
            result += "import java.io.IOException;\n";
            result += "import java.util.HashMap;\n";
            result += "import java.util.List;\n";
            result += "import java.util.Map;\n";

            return result;
        }

        private string usingStatements(string objectname, string projectName)
        {
            string result = "\n";
            result = "using DataObjects;\n";
            result += "using iDataAccessLayer";


            result += "namespace DataAccessFakes\n{";

            return result;
        }
        /// <summary>
        /// Generates the standard access level control statements for the Java Servlet. 
        /// Jonathan Beck
        /// </summary>
        /// <returns>A string that is  Java code for standard access level control on the servlet. </returns>
        private string privLevelStatement()
        {
            string result = "\n//To restrict this page based on privilege level\n";
            result += "int PRIVILEGE_NEEDED = 0;\n";
            result += "List<String> ROLES_NEEDED = new ArrayList<>();\n";
            result += "//add roles here\n";
            result += "HttpSession session = req.getSession();\n";
            result += "User user = (User)session.getAttribute(\"User\");\n";
            result += "if (user==null||user.getPrivilege_ID()<PRIVILEGE_NEEDED||!user.isInRole(ROLES_NEEDED)){\n";
            result += "resp.sendRedirect(\"/" + settings.database_name + "Login\");\n";
            result += "return;\n";
            result += "}\n";
            result += "\n";
            return result;
        }
        /// <summary>
        /// Generates the retrieve line for C# data access layers, using only a column name
        /// Jonathan Beck
        /// </summary>
        /// <param name="r"> The column that you are requesting data from </param>
        /// <returns>A string that is  C# code for retrieveing a particular </returns>
        private string getCSharpOrdinal(Column r)
        {
            String retrieveThing = "";
            if (!r.nullable.Equals('n') || !r.nullable.Equals("N"))
            {
                retrieveThing = retrieveThing + "output." + r.column_name.bracketStrip() + " = reader.Get" + r.data_type.toSqlReaderDataType() + "(reader.GetOrdinal(\"" + name + "_" + r.column_name.bracketStrip() + "\"));\n";
            }
            else
            {
                retrieveThing = retrieveThing + "output." + r.column_name.bracketStrip() + " = reader.IsDBNull(" + "(reader.GetOrdinal(\"" + name + "_" + r.column_name.bracketStrip() + "\")) ? \"\" : reader.Get" + r.data_type.toSqlReaderDataType() + "(reader.GetOrdinal(\"" + name + "_" + r.column_name.bracketStrip() + "\"));\n";
            }
            return retrieveThing;
        }
        /// <summary>
        /// Generates the retrieve line for C# data access layers, using  a column name and a table name
        /// Jonathan Beck
        /// </summary>
        /// <param name="r"> The column that you are requesting data from </param>
        /// /// <param name="t"> The table you are requesting data from </param>
        /// <returns>A string that is  C# code for retrieveing a particular </returns>
        private string getCSharpOrdinal(table t, Column r)
        {
            String retrieveThing = "";
            if (!r.nullable.Equals('n') || !r.nullable.Equals("N"))
            {
                retrieveThing = retrieveThing + "output." + t.name + "." + r.column_name.bracketStrip() + " = reader.Get" + r.data_type.toSqlReaderDataType() + "(reader.GetOrdinal(\"" + t.name + "_" + r.column_name.bracketStrip() + "\"));\n";
            }
            else
            {
                retrieveThing = retrieveThing + "output." + t.name + "." + r.column_name.bracketStrip() + " = reader.IsDBNull(" + "(reader.GetOrdinal(\"" + t.name + "_" + r.column_name.bracketStrip() + "\")) ? \"\" : reader.Get" + r.data_type.toSqlReaderDataType() + "(reader.GetOrdinal(\"" + t.name + "_" + r.column_name.bracketStrip() + "\"));\n";
            }
            return retrieveThing;
        }
        /// <summary>
        /// Generates the rudamentary jQuery validation for this <see cref="table"/>.
        /// Jonathan Beck
        /// </summary>
        /// <param name="r"> The column that you are requesting data from </param>
        /// /// <param name="t"> The table you are requesting data from </param>
        /// <returns>A string that is jQuery code for rudamentary data validation for this table.</returns>
        public string jQueryValidation()
        {
            //to start the js file
            string result = "$(document).ready(function() {\n";
            result += "let submitbutton = document.getElementById(\"submitButton\")\n";
            result += "submitbutton.disabled=true;\n";
            result += "let total_errors=0;\n";
            foreach (Column r in columns) {
                if (!r.identity.Equals("Y") && !r.identity.Equals("y") && r.default_value.Equals("")) {
                    result += "let " + r.column_name + "_error=0;\n";
                }
            }
            result += "";
            //to make the submit button
            result += "";
            result += "";
            //to assign variables
            //to loop through each input field
            foreach (Column r in columns)
            {
                if (!r.identity.Equals("Y") && !r.identity.Equals("y")&&r.default_value.Equals(""))
                {
                    string fieldname = "input" + name.ToLower() + r.column_name;
                    string jsname = r.column_name + "_input";
                    result += "// to clean the field, then set event listener for validating the input for " + r.column_name + "\n";
                    result += "var " + jsname + "= document.getElementById(\"" + fieldname + "\");\n";
                    
                    result += jsname + ".addEventListener('blur',function(){\n";
                    result += jsname+".value = "+jsname + ".value.trim();\n";
                    //if for numeric check
                    if (r.length == 0)
                    {
                        result += "if (" + jsname + ".value!=\"\"&& $.isNumeric(" + jsname + ".value)){\n";
                    }
                    //if for varchar check
                    else
                    {
                        result += "if (" + jsname + ".value!=\"\"&& " + jsname + ".value.length>1 && " + jsname + ".value.length<=" + r.length + "){\n";
                    }
                    //good input
                    result += "$(" + jsname + ").addClass(\"ui-state-highlight\");\n";
                    result += "$(" + jsname + ").removeClass(\"ui-state-error\");\n";
                    result += r.column_name + "_error=0;";
                    //bad input
                    result += "}\n else {\n";
                    result += "$(" + jsname + ").removeClass(\"ui-state-highlight\");\n";
                    result += "$(" + jsname + ").addClass(\"ui-state-error\");\n";
                    result += r.column_name + "_error=1;";
                    result += "}\n";
                    result += "total_errors = ";
                    string plus = "";
                    foreach (Column s in columns) {
                        result += plus + r.column_name + "_error";
                        plus = "+ ";
                    }
                    result += ";\n";
                    result += "if (total_errors ==0){\n";
                    result += "submitbutton.disabled=false;\n";
                    result += "} else {\n";
                    result += "submitbutton.disabled=true;\n";
                    result += "}\n";


                    result +=    "}\n);\n";
                }
            }
            // to end the js file
            result += "\n}\n)\n";
            return result;
        }
        public string jQueryDelete() {
            string result = "$(document).ready(function() {\r\n";
            result += "normalizeHeight();\n";
            result += "$(\"#dialog\").dialog({\n";
            result += "modal: true,\n";
            result += "bgiframe: true,\n";
            result += "autoOpen: false,\n";
            result += "width: 500,\n";
            result += "height: 400,\n";
            result += "});\n";
            
            result += "$(\".delButton\").click(function(e) {\n";
            result += "e.preventDefault();\n";
            result += "var headers = document.getElementsByClassName('table-responsive')[0].childNodes[0].childNodes[1].childNodes[1].childNodes;\r\n";
            result += "var parentrow = this.parentElement.parentElement.parentElement.children;";
            result += "var rowid =\"#\"+ targetUrl+\"row\";\n";
            result += "var text = \"\";\n";
            result += "for (i=1;i<headers.length-2;i=i+2){\n";
            result += "text +=headers[i].textContent+\": \"+parentrow[(i-1)/2].innerHTML+\"</br>\";\n";
            result += "}\n";
            result += "document.getElementById(\"dialog\").innerHTML=text;";
            
            result += "var targetUrl = $(this).attr(\"href\");\n";
            result += "$('#dialog').dialog('option', 'title', 'Delete '+parentrow[1].innerHTML+\"???\");\n";
            result += "$(\"#dialog\").dialog({\n";
            result += "hide: {\n";
            result += "effect: \"explode\",\n";
            result += "duration: 300\n";
            result += "},\n";
            result += "show: {\n";
            result += "effect: \"explode\",\n";
            result += "duration: 300\n";
            result += "},\n";
            result += "buttons : {\n";
            result += "\"Delete For Real\" : function() {\n";
            result += "console.log(\"try\");\n";
            result += "$.ajax({\n";
            result += "url: 'delete"+name+"',\n";
            result += "data: \""+name.ToLower()+"id=\" + targetUrl+\"&AJAX=true\" ,\n";
            result += "type: 'post',\n";
            result += "async: true,\n";
            result += "success: function (response) {\n";
            result += "if (response==1){\n";
            result += "$(rowid).slideUp();";
            result += "}\n";
            result += "else {\n ";
            result += "$(rowid).addClass(\"ui-state-error\");\n";
            result += "}\n";
            result += "}})\n";
            result += "$(this).dialog(\"close\");\n";
            result += "var rowid =\"#\"+ targetUrl+\"row\";\n";
            result += "$(rowid).slideUp();\n";
            result += "},\n";
            
            result += "\"Let It Stay\" : function() {\n";
            result += " $(this).dialog(\"close\");\n";
            result += "}\n";
            result += "}\n";
            result += "});\n";
            result += "$(\"#dialog\").dialog(\"open\");";
            result += " });\n})\n";

            result += normalizeHeight();
            
            return result;
        }
        private string normalizeHeight() {
            string result = "function normalizeHeight() {\n";
            result += "var cards = jQuery(\"span.card\");\n";
            result += "var big = 0;\n";
            result += "cards.each(function (index, el) {\n";
            result += "if (jQuery(el).height() > big)\n";
            result += "big = jQuery(el).height(); //find the largest height\n";
            result += "});\n";
            result += "cards.each(function (index, el) {\n";
            result += "jQuery(el).css(\"height\", big + \"px\"); //assign largest height to all the divs\r\n";
            result += "});";
            result += "}\n";
        
            return result;
        }
        private string initMethod()
        {
            string result = "";
            result += "private i" + name + "DAO " + name.ToLower() + "DAO;\n";
            foreach (Column r in columns)
            {
                if (r.foreign_key != "")
                {
                    string[] parts = r.references.Split('.');
                    string fk_table = parts[0];
                    string fk_name = parts[1];
                    result += "private i" + fk_table + "_DAO " + fk_table.ToLower() + "DAO;\n";
                }
            }
            result += "@Override\n";
            result += "public void init() throws ServletExecption{\n";
            result += name.ToLower() + "DAO = new " + name + "_DAO();\n";
            foreach (Column r in columns)
            {
                if (r.foreign_key != "")
                {
                    string[] parts = r.references.Split('.');
                    string fk_table = parts[0];
                    string fk_name = parts[1];
                    result += fk_table.ToLower() + "DAO = new " + fk_table + "_DAO();\n";
                }
            }
            result += "}\n";
            result += "public void init(";
            result += "i" + name + "_DAO " + name.ToLower() + "DAO";
            foreach (Column r in columns)
            {

                if (r.foreign_key != "")
                {
                    string[] parts = r.references.Split('.');
                    string fk_table = parts[0];
                    string fk_name = parts[1];
                    result += ",i" + fk_table + "_DAO " + fk_table.ToLower() + "DAO";

                }
            }
            result += "){\n";
            result += "this." + name.ToLower() + "DAO = " + name.ToLower() + "DAO;\n";
            foreach (Column r in columns)
            {

                if (r.foreign_key != "")
                {
                    string[] parts = r.references.Split('.');
                    string fk_table = parts[0];
                    string fk_name = parts[1];
                    result += "this." + fk_table.ToLower() + "DAO = " + fk_table.ToLower() + "DAO;\n";
                }
            }
            result += "}\n";
            return result;
        }
        /// <summary>
        /// Creates a list of common stored procedure definitions. Will write method signatures for basic CRRRUDD functions.
        /// </summary>
        /// <returns>a string</returns>
        public string sp_definitions()
        {
            bool has_is_active = false;
            foreach (Column r in columns)
            {
                if (r.column_name.ToLower().Equals("is_active"))
                {
                    has_is_active = true;
                    break;
                }
            }

            string result = "";
            result += sp_header(); //good
            result += sp_insert(); //done
            result += sp_retrieve_by_key(); //done
            result += sp_retrieve_by_all(); //done
            result += sp_retrieve_by_fk();
            result += sp_update();  //done
            if (has_is_active)
            {
                result += sp_deactivate(); //done
                result += sp_activate();//done
            }
            foreach (Column r in columns)
            {
                if (r.data_type.toCSharpDataType().Equals("bool") && !r.column_name.ToLower().Equals("is_active"))
                {
                    result += sp_make_bool_true(r.column_name);
                    result += sp_make_bool_false(r.column_name);
                }
            }
            result += sp_delete();//done
            result += sp_distinct(); //done
            result += sp_count(); //done
            return result;

        }
        private string sp_header()
        {
            string Name = name + "\n";
            string creator = "Initial Creator : Jonathan Beck\n";
            string date = DateTime.Today.ToLongDateString() + "\n\n";

            string result = Name + creator + date;
            return result;
        }
        private string sp_insert()
        {
            string header = "\nsp_insert_" + name + ":\n";
            string table_used = sp_tables_used_gen(0);
            string parameters = sp_paramater_gen(3);

            string returns = "\n\tReturns:\t@@" + name + "_ID\n";
            string result = header + table_used + parameters + returns;
            return result;

        }

        private string sp_retrieve_by_key()
        {
            string header = "\nsp_retrieve_by_key_" + name + ":\n";
            string table_used = sp_tables_used_gen(1);
            string parameters = sp_paramater_gen(0);
            string returns = sp_return_fields_gen();
            string result = header + table_used + parameters + returns;
            return result;

        }
        private string sp_retrieve_by_all()
        {
            string header = "\nsp_retrieve_by_all_" + name + ":\n";
            string table_used = sp_tables_used_gen(1);
            string parameters = sp_paramater_gen(2);
            string returns = sp_return_fields_gen();

            string result = header + table_used + parameters + returns;
            return result;

        }
        private string sp_retrieve_by_fk()
        {
            string header = "";
            string table_used = "";
            string parameters = "";
            string returns = "";
            string result = header + table_used + parameters + returns;
            return result;

        }
        private string sp_update()
        {
            string header = "\nsp_update_" + name + ":\n";
            string table_used = sp_tables_used_gen(0);
            string parameters = sp_paramater_gen(1);
            string returns = "\n\tReturns: \tint(@@rowsAffected)\n";
            string result = header + table_used + parameters + returns;
            return result;

        }
        private string sp_deactivate()
        {
            string header = "\nsp_deactivate_" + name + ":\n";
            string table_used = sp_tables_used_gen(0);
            string parameters = sp_paramater_gen(0);
            string returns = "\n\tReturns:\tint(@@RowsAffected)\n";
            string result = header + table_used + parameters + returns;
            return result;

        }
        private string sp_activate()
        {
            string header = "\nsp_activate_" + name + ":\n";
            string table_used = sp_tables_used_gen(0);
            string parameters = sp_paramater_gen(0);
            string returns = "\n\tReturns:\tint(@@RowsAffected)\n";
            string result = header + table_used + parameters + returns;
            return result;

        }

        private string sp_make_bool_true(string column_name)
        {
            string header = "\nsp_set_" + column_name + "_true:\n";
            string table_used = sp_tables_used_gen(0);
            string parameters = sp_paramater_gen(0);
            string returns = "\n\tReturns:\tint(@@RowsAffected)\n";
            string result = header + table_used + parameters + returns;
            return result;

        }
        private string sp_make_bool_false(string column_name)
        {
            string header = "\nsp_set_" + column_name + "_false:\n";
            string table_used = sp_tables_used_gen(0);
            string parameters = sp_paramater_gen(0);
            string returns = "\n\tReturns:\tint(@@RowsAffected)\n";
            string result = header + table_used + parameters + returns;
            return result;

        }
        private string sp_delete()
        {
            string header = "\nsp_delete_" + name + ":\n";
            string table_used = sp_tables_used_gen(0);
            string parameters = sp_paramater_gen(0);
            string returns = "\n\tReturns:\tint(@@RowsAffected)\n";
            string result = header + table_used + parameters + returns;
            return result;

        }

        private string sp_distinct()
        {
            string header = "\nsp_select_distinct_and_active_" + name + "_for_dropdown:\n";
            string table_used = sp_tables_used_gen(0);
            string parameters = sp_paramater_gen(4);
            string returns = "\tReturns: \t" + name + "_ID\n";
            string result = header + table_used + parameters + returns;
            return result;

        }

        private string sp_count()
        {
            string header = "\nsp_retrieve_" + name + "_count:\n";
            string table_used = sp_tables_used_gen(0);
            string parameters = sp_paramater_gen(4);
            string returns = "\tReturns:\tCOUNT(" + name + ")\n";
            string result = header + table_used + parameters + returns;
            return result;

        }

        private string sp_return_fields_gen()
        {
            string comma = "";
            string returns = "\n\tReturns:\t";
            foreach (Column r in columns)
            {
                returns += comma + name + "." + r.column_name;
                comma = ", ";
            }
            foreach (foreignKey fk in data_tables.all_foreignKey)
            {
                if (fk.mainTable.Equals(name))
                {
                    foreach (table t in data_tables.all_tables)
                    {
                        if (t.name.Equals(fk.referenceTable))
                        {
                            foreach (Column r in t.columns)
                            {
                                returns += comma + t.name + "." + r.column_name;
                            }
                        }
                    }
                }
            }
            returns += "\n";
            return returns;
        }

        private string sp_tables_used_gen(int mode)
        {
            string result = "\tTables:\t\t" + name;
            if (mode == 1)
            {
                foreach (Column r in columns)
                {
                    if (r.foreign_key != "")
                    {
                        string[] parts = r.references.Split('.');
                        result += ", " + parts[0];
                    }
                }
            }

            return result;
        }

        private string sp_paramater_gen(int mode)
        {

            string result = "";
            //retrieve pk
            if (mode == 0)
            {
                string comma = "";
                result = "\n\tParameters:\t";
                foreach (Column r in columns)
                {
                    if (r.primary_key == 'y' || r.primary_key == 'Y')
                    {
                        result += comma + "@" + r.column_name;
                        comma = ", ";
                    }
                }
            }
            //update
            if (mode == 1)
            {
                result = "\n\tParameters:\t";
                string comma = "";
                foreach (Column r in columns)
                {
                    result += comma + "@old" + r.column_name;
                    comma = ", ";
                    if (r.primary_key != 'y' && r.primary_key != 'Y')
                    {
                        result += comma + "@new" + r.column_name;
                    }
                }
            }
            //retrieve by all
            if (mode == 2)
            {
                result = "\n\tParameters:\t @limit_param, @offset_param";
                foreach (Column r in columns)
                {
                    if (r.foreign_key != "" && !r.foreign_key.Equals("no") & r.references.Contains("."))
                    {
                        string[] parts = r.references.Split('.');
                        result += ", @" + parts[1];
                    }
                }
            }
            //add
            if (mode == 3)
            {
                result = "\n\tParameters:\t";

                string comma = "";
                foreach (Column r in columns)
                {
                    result += comma + "@" + r.column_name;
                    comma = ", ";
                }
            }
            if (mode == 4)
            {
                result = "\n\tParameters:\tNONE\n";
            }

            return result;
        }

        public string genCSharpManagerTests() {
            string result = "";
            string header = genManagerTestHeader();
            string AddGood = genManagerTestAddCanAdd();
            string AddFail = genManagerTestAddCanFail();
            string DeleteGood = genManagerTestDeleteCanDelete();
            string DeleteFail = genManagerTestDeleteCanFail();
            string unDeleteGood = genManagerTestUnDeleteCanUndelete();
            string unDeleteFail = genManagerTestUnDeleteCanFail();
            string retrieveByPKGood = genManagerTestRetreiveByPKCanWork();
            string retrieveByPKFail = genManagerTestRetreiveByPKCanFail();
            string retrieveByFKGood = genManagerGetByFKCanWork();
            string retrieveByFKFail = genManagerGetByFKCanFail();
            string RetrieveAllGood = genManagerTestGetAllCanWork();
            string RetreiveAllFilter = genManagerTestGetAllCanFilter();
            string RetrieveAllfail = genManagerTestGetAllCanFail();
            string UpdateGood = genManagerTestUpdateCanWork();
            string UpdateFail = genManagerTestUpdateCanFail();
            string batchAddCanWork = genManagerTestBatchAddCanWork();
            string batchAddFail = genManagerTestBatchAddCanFail();
            string ValidationMethod = genCSharpValidationMethod();
            //String dropdown = genManagerDropDown();
            string footer = "\n}\n";
            result = result
                + header
                + AddGood + AddFail
                + DeleteGood + DeleteFail
                + unDeleteGood + unDeleteFail
                + retrieveByPKGood + retrieveByPKFail
                + retrieveByFKGood + retrieveByFKFail
                + RetrieveAllGood + RetrieveAllfail + RetreiveAllFilter
                + UpdateGood  + UpdateFail                
                + batchAddCanWork + batchAddFail
                + ValidationMethod+footer
                ;
            return result;

        }

        private string genManagerTestHeader() {

            string result = "";
            result += "using System;\n";
            result += "using System.Collections.Generic;\n";
            result += "using System.Linq;\n";
            result += "using System.Text;\n";
            result += "using System.Threading.Tasks;\n";
            result += "using DataObjects;\n";
            result += "using LogicLayer;\n";
            result += "using DataAccessFakes;\n";
            result += "using System.ComponentModel.DataAnnotations;\n";
            result += "\n";
            result += "namespace LogicLayerTests\n";

            result += "{\n";
            result += "[TestClass]\n";

            result += "public class " + name + "ManagerTests\n";
            result += "{\n";
            result += name + "Manager _" + name.ToLower() + "Manager;\n";
            result += "[TestInitialize]\n";
            result += "public void setup() {\n";
            result += " _" + name.ToLower() + "Manager = new " + name + "Manager(new "+name+"AccessorFake());\n";
            result += "}\n";
            result += "[TestCleanup]\n";
            result += "public void tearDown() {";
            result += "_" + name.ToLower() + "Manager = null;\n";
            result += "}\n";
            return result;
        }
        private string genManagerTestAddCanAdd()
        {
            string result = "[TestMethod]\n";
            result += "public void TestAdd"+name+"CanAdd(){\n";
            result += "//arrage\n";
            result += "int expected = 1;\n";
            result += "int actual = 0;\n";
            result += name + " _" + name.ToLower() + " = new " + name + "();\n";
            foreach (Column r in columns)
            {
                if (r.increment == 0)
                {
                    if (r.data_type.toCSharpDataType().Equals("string"))
                    {
                        result += "_"+name.ToLower()+"."+r.column_name+" = \"TestValue\";\n";

                    }
                    if (r.data_type.toCSharpDataType().Equals("int"))
                    {
                        result += "_" + name.ToLower() + "." + r.column_name + " = 406;\n";

                    }
                    if (r.data_type.toCSharpDataType().Equals("bool"))
                    {
                        result += "_" + name.ToLower() + "." + r.column_name + " = true;\n";

                    }
                    if (r.data_type.toCSharpDataType().ToLower().Contains("date") || r.data_type.toCSharpDataType().Contains("time"))
                    {
                        result += "_" + name.ToLower() + "." + r.column_name + " = new DateTime();\n";

                    }

                    if (r.data_type.toCSharpDataType().ToLower().Contains("decimal"))
                    {
                        result += "_" + name.ToLower() + "." + r.column_name + " = 12.12m;\n";

                    }
                }
            }
            result += "//act\n";
            result += "actual =  _" + name.ToLower() + "Manager.Add(_"+name.ToLower()+");\n";
            result += "//assert\n";
            result += "Assert.AreEqual(expected,actual);\n";
            result += "}\n";
            return result;
        }
        private string genManagerTestAddCanFail()
        {
            string result = "[TestMethod]\n";
            result += "[ExpectedException(typeof(ApplicationException))]\n";
            result += "public void TestAdd"+name+"CanFail(){\n";
            result += "//arrage\n";
            result += "int expected = 1;\n";
            result += "int actual = 0;\n";
            result += name + " _" + name.ToLower() + " = new " + name + "();\n";            
            result += "//act\n";
            result += "actual =  _" + name.ToLower() + "Manager.Add(_"+name.ToLower()+");\n";
            result += "//assert - Nothing To do\n";
            result += "";
            result += "}\n";
            return result;

        }
        private string genManagerTestDeleteCanDelete()
        {
            string result = "[TestMethod]\n";
            result += "public void TestDelete"+name+"CanDelete(){\n";
            result += "//arrage\n";
            result += "int expected = 1;\n";
            result += "int actual = 0;\n";
            result += name + " _" + name.ToLower() + " = new " + name + "();\n";
            foreach (Column r in columns)
            {
                if (r.primary_key.Equals('y') || r.primary_key.Equals('Y'))
                {
                    if (r.data_type.toCSharpDataType().Equals("string"))
                    {
                        result += "_" + name.ToLower() + "." + r.column_name + " = \"TestValue\";\n";

                    }
                    if (r.data_type.toCSharpDataType().Equals("int"))
                    {
                        result += "_" + name.ToLower() + "." + r.column_name + " = 406;\n";

                    }
                    if (r.data_type.toCSharpDataType().Equals("bool"))
                    {
                        result += "_" + name.ToLower() + "." + r.column_name + " = true;\n";

                    }
                    if (r.data_type.toCSharpDataType().ToLower().Contains("date") || r.data_type.toCSharpDataType().Contains("time"))
                    {
                        result += "_" + name.ToLower() + "." + r.column_name + " = new DateTime();\n";

                    }

                    if (r.data_type.toCSharpDataType().ToLower().Contains("decimal"))
                    {
                        result += "_" + name.ToLower() + "." + r.column_name + " = 12.12m;\n";

                    }
                }
            }
            
               
            result += "//act\n";
            result += "actual =  _" + name.ToLower() + "Manager.Delete(_" + name.ToLower() + ");\n";
            result += "//assert\n";
            result += "Assert.AreEqual(expected,actual);\n";
            result += "}\n";
            return result;

        }
        private string genManagerTestDeleteCanFail()
        {
            string result = "[TestMethod]\n";
            result += "[ExpectedException(typeof(ApplicationException))]\n";
            result += "public void TestDelete"+name+"CanFail(){\n";
            result += "//arange\n";
            result += "int expected = 1;\n";
            result += "int actual = 0;\n";
            result += name + " _" + name.ToLower() + " = new " + name + "();\n";          
            result += "//act\n";
            result += "actual =  _" + name.ToLower() + "Manager.Delete(_" + name.ToLower() + ");\n"; 
            result += "\n";
            result += "//assert -- nothing to do\n";
            result += "\n";
            result += "}\n";
            return result;

        }
        private string genManagerTestUnDeleteCanUndelete()
        {
            string result = "[TestMethod]\n";
            result += "public void TestUnDelete"+name+"CanUnDelete(){\n";
            result += "//arrage\n";
            result += "int expected = 1;\n";
            result += "int actual = 0;\n";
            result += name + " _" + name.ToLower() + " = new " + name + "();\n";
            foreach (Column r in columns)
            {
                if (r.primary_key.Equals('y') || r.primary_key.Equals('Y'))
                {
                    if (r.data_type.toCSharpDataType().Equals("string"))
                    {
                        result += "_" + name.ToLower() + "." + r.column_name + " = \"TestValue\";\n";

                    }
                    if (r.data_type.toCSharpDataType().Equals("int"))
                    {
                        result += "_" + name.ToLower() + "." + r.column_name + " = 406;\n";

                    }
                    if (r.data_type.toCSharpDataType().Equals("bool"))
                    {
                        result += "_" + name.ToLower() + "." + r.column_name + " = true;\n";

                    }
                    if (r.data_type.toCSharpDataType().ToLower().Contains("date") || r.data_type.toCSharpDataType().Contains("time"))
                    {
                        result += "_" + name.ToLower() + "." + r.column_name + " = new DateTime();\n";

                    }

                    if (r.data_type.toCSharpDataType().ToLower().Contains("decimal"))
                    {
                        result += "_" + name.ToLower() + "." + r.column_name + " = 12.12m;\n";

                    }
                }
            }

            result += "//act\n";
            result += "actual =  _" + name.ToLower() + "Manager.UnDelete(_" + name.ToLower() + ");\n";
            result += "//assert\n";
            result += "Assert.AreEqual(expected,actual);\n";
            result += "}\n";
            return result;

        }
        private string genManagerTestUnDeleteCanFail()
        {
            string result = "[TestMethod]\n";
            result += "[ExpectedException(typeof(ApplicationException))]\n";
            result += "public void TestUnDelete"+name+"CanFail(){\n";
            result += "//arange\n";
            result += "int expected = 1;\n";
            result += "int actual = 0;\n";
            result += name + " _" + name.ToLower() + " = new " + name + "();\n";
            result += "//act\n";
            result += "actual =  _" + name.ToLower() + "Manager.UnDelete(_" + name.ToLower() + ");\n";
            result += "\n";
            result += "//assert -- nothing to do\n";
            result += "\n";
            result += "}\n";
            return result;

        }
        private string genManagerTestRetreiveByPKCanWork()
        {
            string result = "[TestMethod]\n";
            result += "public void TestRetreive"+name+"ByPKCanRetreiveByPK(){\n";
            result += "//arrage\n";
            
            result += name+" expected = new "+name+"();\n";
            foreach (Column r in columns)
            {
                if (r.increment == 0)
                {
                    if (r.data_type.toCSharpDataType().Equals("string"))
                    {
                        result += "expected." + r.column_name + " = \"TestValue\";\n";

                    }
                    if (r.data_type.toCSharpDataType().Equals("int"))
                    {
                        result += "expected." + r.column_name + " = 406;\n";

                    }
                    if (r.data_type.toCSharpDataType().Equals("bool"))
                    {
                        result += "expected." + r.column_name + " = true;\n";

                    }
                    if (r.data_type.toCSharpDataType().ToLower().Contains("date") || r.data_type.toCSharpDataType().Contains("time"))
                    {
                        result += "expected." + r.column_name + " = new DateTime();\n";

                    }

                    if (r.data_type.toCSharpDataType().ToLower().Contains("decimal"))
                    {
                        result += "expected." + r.column_name + " = 12.12m;\n";

                    }
                }
            }
            result += name+ " actual = new "+name+"();\n";
            
            foreach (Column r in columns)
            {
                if (r.primary_key.Equals('y') || r.primary_key.Equals('Y'))
                {
                    if (r.data_type.toCSharpDataType().Equals("string"))
                    {
                        result += "actual." + r.column_name + " = \"TestValue\";\n";

                    }
                    if (r.data_type.toCSharpDataType().Equals("int"))
                    {
                        result += "actual." + r.column_name + " = 406;\n";

                    }
                    if (r.data_type.toCSharpDataType().Equals("bool"))
                    {
                        result += "actual." + r.column_name + " = true;\n";

                    }
                    if (r.data_type.toCSharpDataType().ToLower().Contains("date") || r.data_type.toCSharpDataType().Contains("time"))
                    {
                        result += "actual." + r.column_name + " = new DateTime();\n";

                    }

                    if (r.data_type.toCSharpDataType().ToLower().Contains("decimal"))
                    {
                        result += "actual." + r.column_name + " = 12.12m;\n";

                    }
                }
            }

            result += "//act\n";
            result += "actual =  _" + name.ToLower() + "Manager.GetByPK(actual);\n";
            result += "//assert\n";
            foreach (Column r in columns) {
                result += "Assert.AreEqual(expected."+r.column_name+",actual."+r.column_name+");\n";
            }
            
            result += "}\n";
            return result;
            

        }
        private string genManagerTestRetreiveByPKCanFail()
        {
            string result = "[TestMethod]\n";
            result += "[ExpectedException(typeof(ApplicationException))]\n";
            result += "public void TestRetreive" + name + "ByPKCanRetreiveByFail(){\n";
            
            result += "//arrage\n";
            result += name + " expected = new " + name + "();\n";
            foreach (Column r in columns)
            {
                if (r.increment == 0)
                {
                    if (r.data_type.toCSharpDataType().Equals("string"))
                    {
                        result += "expected." + r.column_name + " = \"TestValue\";\n";

                    }
                    if (r.data_type.toCSharpDataType().Equals("int"))
                    {
                        result += "expected." + r.column_name + " = 406;\n";

                    }
                    if (r.data_type.toCSharpDataType().Equals("bool"))
                    {
                        result += "expected." + r.column_name + " = true;\n";

                    }
                    if (r.data_type.toCSharpDataType().ToLower().Contains("date") || r.data_type.toCSharpDataType().Contains("time"))
                    {
                        result += "expected." + r.column_name + " = new DateTime();\n";

                    }

                    if (r.data_type.toCSharpDataType().ToLower().Contains("decimal"))
                    {
                        result += "expected." + r.column_name + " = 12.12m;\n";

                    }
                }
            }
            result += name + " actual = new " + name + "();\n";           

            result += "//act\n";
            result += "actual =  _" + name.ToLower() + "Manager.GetByPK(actual);\n";
            result += "//assert -- nothing to do\n";
            result += "\n";
            result += "}\n";
            return result;

        }
        private string genManagerGetByFKCanWork()
        {
            string result = "[TestMethod]\n";
            result += "public void TestRetreive" + name + "ByFKCanRetreiveByFK(){\n";
            result += "//arrage\n";
            result += "";
            result += "//act\n";
            result += "";
            result += "//assert\n";
            result += "Assert.AreEqual(expected,actual);\n";
            result += "}\n";
            return result;

        }
        private string genManagerGetByFKCanFail()
        {
            string result = "[TestMethod]\n";
            result += "public void TestRetreive" + name + "ByFKCanFail(){\n";
            result += "public void Test";
            result += "//arrage\n";
            result += "";
            result += "//act\n";
            result += "";
            result += "//assert -- nothing to do\n";
            result += "";
            result += "}\n";
            return result;

        }
        private string genManagerTestGetAllCanWork()
        {

            string result = "[TestMethod]\n";
            result += "public void TestRetreiveAll" + name + "CanWork(){\n";
            result += "//arrage\n";
            result += "int expected = 4;\n";
            result += "int actual =0;\n";
            result += "//act\n";
            result += "actual =  _" + name.ToLower() + "Manager.GetAll(0,-1,\"\").Count;\n";
            result += "//assert\n";
            result += "Assert.AreEqual(expected,actual);\n";
            result += "}\n";
            return result;
        }

        private string genManagerTestGetAllCanFilter()
        {
            string result = "";
            foreach (foreignKey key in data_tables.all_foreignKey)
            {
                if (key.referenceTable.ToLower().Equals(name.ToLower()))
                {
                    result += "[TestMethod]\n";
                    result += "public void TestRetreiveAll" + name + "CanFilterBy"+key.referenceTable+"(){\n";
                    result += "//arrage\n";
                    result += "int expected = 4;\n";
                    result += "int actual =0;\n";
                    result += "//act\n";
                    result += "actual =  _" + name.ToLower() + "Manager.GetAll(0,-1,\"\");\n";
                    result += "//assert\n";
                    result += "Assert.AreEqual(expected,actual);\n";
                    result += "}\n";
                }
            }
            return result;
        }
        private string genManagerTestGetAllCanFail()
        {
            string result = "[TestMethod]\n";
            result += "[ExpectedException(typeof(ApplicationException))]\n";
            result += "public void TestRetreiveAll" + name + "CanFail(){\n";
            result += "//arrage\n";
            result += "int expected = 4;\n";
            result += "int actual =0;\n";
            result += "//act\n";
            result += "actual =  _" + name.ToLower() + "Manager.GetAll(-1,-1,\"\");\n";
            result += "//assert -- nothing to do\n";
            result += "\n";
            result += "}\n";
            return result;

        }
        private string genManagerTestUpdateCanWork()
        {

            string result = "[TestMethod]\n";
            result += "public void TestUpdate"+name+"CanUpdate(){\n";
            result += "//arrage\n";
            result += "int expected = 1;\n";
            result += "int actual = 0;\n";
            result += name + " old"+name+" = new " + name + "();\n";
            result += name + " new" + name + " = new " + name + "();\n";
            foreach (Column r in columns)
            {
                
                    if (r.data_type.toCSharpDataType().Equals("string"))
                    {
                        result += "old"+name+"." + r.column_name + " = \"TestValue\";\n";
                    result += "new" + name + "." + r.column_name + " = \"TestValue1\";\n";

                }
                    if (r.data_type.toCSharpDataType().Equals("int"))
                    {
                        result += "old" + name + "." + r.column_name + " = 406;\n";
                    result += "new" + name + "." + r.column_name + " = 407;\n";

                }
                    if (r.data_type.toCSharpDataType().Equals("bool"))
                    {
                        result += "old" + name + "." + r.column_name + " = true;\n";
                    result += "new" + name + "." + r.column_name + " = false;\n";

                }
                    if (r.data_type.toCSharpDataType().ToLower().Contains("date") || r.data_type.toCSharpDataType().Contains("time"))
                    {
                        result += "old" + name + "." + r.column_name + " = new DateTime();\n";
                    result += "new" + name + "." + r.column_name + " = true;\n";

                }

                    if (r.data_type.toCSharpDataType().ToLower().Contains("decimal"))
                    {
                        result += "old" + name + "." + r.column_name + " = 12.12m;\n";
                    result += "new" + name + "." + r.column_name + " = 12.13m;\n";

                }
                
            }
            result += "";
            result += "//act\n";
            result += "actual =  _" + name.ToLower() + "Manager.update(old"+name+",new"+name+");\n";
            result += "//assert\n";
            result += "Assert.AreEqual(expected,actual);\n";
            result += "}\n";
            return result; ;
        }
        private string genManagerTestUpdateCanFail()
        {
            string result = "[TestMethod]\n";
            result += "[ExpectedException(typeof(ApplicationException))]\n";
            result += "public void TestUpdate" + name + "CanFail(){\n";
            result += "//arrage\n";
            result += "int expected = 1;\n";
            result += "int actual = 0;\n";
            result += name + " old" + name + " = new " + name + "();\n";
            result += name + " new" + name + " = new " + name + "();\n";
            foreach (Column r in columns)
            {

                if (r.data_type.toCSharpDataType().Equals("string"))
                {
                    result += "old" + name + "." + r.column_name + " = \"TestValue\";\n";
                    

                }
                if (r.data_type.toCSharpDataType().Equals("int"))
                {
                    result += "old" + name + "." + r.column_name + " = 406;\n";
                    

                }
                if (r.data_type.toCSharpDataType().Equals("bool"))
                {
                    result += "old" + name + "." + r.column_name + " = true;\n";
                    

                }
                if (r.data_type.toCSharpDataType().ToLower().Contains("date") || r.data_type.toCSharpDataType().Contains("time"))
                {
                    result += "old" + name + "." + r.column_name + " = new DateTime();\n";
                    

                }

                if (r.data_type.toCSharpDataType().ToLower().Contains("decimal"))
                {
                    result += "old" + name + "." + r.column_name + " = 12.12m;\n";
                   ;

                }

            }
            result += "";
            result += "//act\n";
            result += "actual =  _" + name.ToLower() + "Manager.update(old" + name + ",new" + name + ");\n";
            result += "//assert -- nothing to do\n";
            result += "";
            result += "}\n";
            return result;

        }
        private string genManagerTestBatchAddCanWork()
        {
            string result = "[TestMethod]\n";
            result += "public void TestAddBatch"+name+"CanWork(){\n";
            result += "//arrage\n";
            result += "";
            result += "//act\n";
            result += "";
            result += "//assert\n";
            result += "Assert.AreEqual(expected,actual);\n";
            result += "}\n";
            return result;

        }
        private string genManagerTestBatchAddCanFail()
        {

            string result = "[TestMethod]\n";
            result += "[ExpectedException(typeof(ApplicationException))]\n";
            result += "public void TestAddBatch" + name + "CanFail(){\n";
            result += "//arrage\n";
            result += "";
            result += "//act\n";
            result += "";
            result += "//assert -- nothing to do\n";
            result += "";
            result += "}\n";
            return result;
        }
        



        public string createCSharpModelTests()
        {
            string result = "";
            result += testCSharpModelInitialize(); // done

            result += testCSharpDefaultConstructor();  // done
            result += testCSharpParameterizedConstructor();  // done
            result += testCSharpKeyedParameterizedConstructor();  // done

            //result += testJavaVMDefaultConstructor();
            //result += testVMParameterizedConstructor();
            foreach (Column r in columns)
            {
                result += createCSharpColumnTests(r);  // done
            }
            result += testCSharpCompareTo(); //not done 
            result += genCSharpValidationMethod();
            result += "\n}\n";
            return result;
        }

        public string createCSharpModelVMTests()
        {
            string result = "";
            result += testCSharpVMInitialize(); // done
            result += testCSharpVMDefaultConstructor();// done
            result += testCSharpVMParameterizedConstructors(); // done

            foreach (Column r in columns)
            {
                if (r.references != null && r.references != "")
                {
                    string[] parts = r.references.Split('.');
                    string fk_table = parts[0];
                    string fk_name = parts[1];
                    result += testCSharpObjectSet(fk_table); // done
                }
            }
            foreach (foreignKey key in data_tables.all_foreignKey)
            {
                if (key.referenceTable.ToLower().Equals(name.ToLower()))
                {
                    result += testCSharpListObjectSet(key.mainTable); // done
                }
            }
            //result += testVMCompareTo();
            result += "\n}\n";
            return result;
        }
        private string createCSharpColumnTests(Column r)
        {
            string result = "";

            if (r.data_type.toCSharpDataType().Equals("string"))
            {

                result += testCSharpTooShort(r);  // done
                result += testCSharpTooLong(r); // done
                result += testCSharpStringSet(r); // done
            }
            if (r.data_type.toCSharpDataType().Equals("int"))
            {
                result += testCSharpIntTooSmall(r); // done
                result += testCSharpIntTooBig(r); //  done
                result += testCSharpIntSet(r); // done
            }
            if (r.data_type.Equals("decimal"))
            {
                result += testCSharpDecimalTooSmall(r); // done
                result += testCSharpDecimalTooBig(r); //  done
                result += testCSharpDecimalSet(r); // done
            }
            if (r.data_type.toCSharpDataType().Equals("bool"))
            {
                result += testCSharpBoolSetFalse(r); // done
                result += testCSharpBoolSetTrue(r); // done
            }
            if (r.data_type.Equals("datetime"))
            {
                result += testCSharpDatetimeTooSmall(r); // done
                result += testCSharpDatetimeTooBig(r); //  done
                result += testCSharpDatetimeSet(r); // done
            }

            return result;

        }

        private string testCSharpModelInitialize() {
            string result = "";
            result += "using System;\n";
            result += "using System.Collections.Generic;\n";
            result += "using System.Linq;\n";
            result += "using System.Text;\n";
            result += "using System.Threading.Tasks;\n";
            result += "using DataObjects;\n";
            result += "using System.ComponentModel.DataAnnotations;\n";
            result += "\n";
            result += "namespace DataObjectTests\n";
            
            result += "{\n";
            result += "[TestClass]\n";

            result += "public class "+name+"Tests\n";
            result += "{\n";
            result += name + " _" + name.ToLower() + ";\n";
            result += "[TestInitialize]\n";
            result += "public void setup() {\n";
            result += " _"+name.ToLower()+" = new "+name+"();\n";
            result += "}\n";
            result += "[TestCleanup]\n";
            result += "public void tearDown() {";
            result += "_"+name.ToLower() + "= null;\n";
            result += "}\n";
            return result;
        
        }
        
        private string testCSharpDefaultConstructor()
        {
            string result = commentBox.genJavaTestJavaDoc(JavaTestType.DefaultConstructor, this);
            result += "[TestMethod]\n";
            result += "public void test" + name + "DefaultConstructorSetsNoVariables(){\n";
            result += name + " _" + name.ToLower() + "= new " + name + "();\n";
            foreach (Column r in columns)
            {
                if (r.data_type.toCSharpDataType().Equals("string"))
                {
                    result += "Assert.IsNull(_" + name.ToLower() + "." + r.column_name + ");\n";
                }
                else if (r.data_type.toCSharpDataType().Equals("bool"))
                {
                    result += "Assert.IsFalse(_" + name.ToLower() + "." + r.column_name + ");\n";
                }
                else if (r.data_type.toCSharpDataType().Equals("int"))
                {
                    result += "Assert.AreEqual(_" + name.ToLower() + "." + r.column_name + ",0);\n";
                }
                else
                {
                    result += "Assert.IsNull(_" + name.ToLower() + "." + r.column_name + ");\n";
                }
            }
            result += "}\n";
            return result;

        }
        private string testCSharpParameterizedConstructor()
        {
            string result = commentBox.genJavaTestJavaDoc(JavaTestType.ParamartizedConstructor, this);
            result += "[TestMethod]\n";
            //generate random values for each
            ArrayList array = new ArrayList(columns.Count + 1);

            foreach (Column r in columns)
            {

                if (r.data_type.toCSharpDataType().Equals("string"))
                {
                    if (r.default_value.ToLower().Contains("uuid"))
                    {
                        _ = array.Add(generateRandomString(r, 0));
                    }
                    else
                    {
                        _ = array.Add(generateRandomString(r, -2));
                    }

                    _ = Task.Delay(1);
                }
                else if (r.data_type.toCSharpDataType().Equals("bool"))
                {
                    _ = array.Add(true);
                }
                else if (r.data_type.toCSharpDataType().Equals("int"))
                {
                    _ = array.Add(rand.Next(0, 10000));
                    _ = Task.Delay(1);
                }
                else if (r.data_type.Equals("decimal"))
                {
                    double toAdd = rand.Next(0, 10000) / 100.0;
                    _ = array.Add(toAdd);
                    _ = Task.Delay(1);
                }
                else if (r.data_type.ToLower().Contains("date"))
                {
                    int year = rand.Next(2015, 2027);
                    int month = rand.Next(1, 11);
                    int day = rand.Next(1, 28);
                    DateTime toAdd = new DateTime(year, month, day);
                    _ = array.Add(toAdd);
                    _ = Task.Delay(1);
                }
                else
                {
                    _ = array.Add(null);
                }
                _ = Task.Delay(5);

            }

            //method signature
            result += "public void test" + name + "ParameterizedConstructorSetsAllVariables(){\n";
            //constructor
            result += name + " _" + name.ToLower() + "= new " + name + "(\n";
            //assing a value to each variable
            string comma = "";
            int i = 0;
            foreach (Column r in columns)
            {

                if (r.data_type.toCSharpDataType().Equals("string"))
                {
                    result += comma + "\"" + array[i] + "\"";
                }
                else if (r.data_type.toCSharpDataType().Equals("bool"))
                {
                    result += comma + array[i];
                }
                else if (r.data_type.toCSharpDataType().Equals("int"))
                {
                    result += comma + array[i];
                }
                else if (r.data_type.Equals("decimal"))
                {
                    result += comma + array[i];
                }
                else if (r.data_type.ToLower().Contains("decimal"))
                {
                    result += comma + array[i];
                }
                else
                {
                    result += comma + "new " + r.data_type + "()\n";
                }
                comma = ",\n ";

                i++;
            }
            result += "\n);\n";
            i = 0;
            //test each variable
            foreach (Column r in columns)
            {
                if (r.data_type.toCSharpDataType().Equals("string"))
                {
                    result += "Assert.AreEqual(\"" + array[i] + "\",_" + name.ToLower() + "." + r.column_name + ");\n";
                }
                else if (r.data_type.toCSharpDataType().Equals("bool"))
                {
                    result += "Assert.AreEqual(_" + name.ToLower() + "." + r.column_name + ");\n";
                }
                else if (r.data_type.toCSharpDataType().Equals("int"))
                {
                    result += "Assert.AreEqual(" + array[i] + ",_" + name.ToLower() + "." + r.column_name + ");\n";
                }
                else if (r.data_type.Equals("decimal"))
                {
                    result += "Assert.AreEqual(" + array[i] + ",_" + name.ToLower() + "." + r.column_name + ");\n";
                }
                else if (r.data_type.ToLower().Contains("decimal"))
                {
                    result += "Assert.AreEqual(" + array[i] + ",_" + name.ToLower() + "." + r.column_name + ");\n";
                }

                else
                {
                    result += "Assert.AreEqual(new " + r.data_type + "(),_" + name.ToLower() + "." + r.column_name + ");\n";
                }
                i++;

            }
            result += "}\n";
            return result;

        }
        private string testCSharpKeyedParameterizedConstructor()
        {
            string result = commentBox.genJavaTestJavaDoc(JavaTestType.ParamartizedConstructor, this);
            result += "[TestMethod]\n";
            //generate random values for each
            ArrayList array = new ArrayList(columns.Count + 1);

            foreach (Column r in columns)
            {
                if (r.primary_key == 'y' || r.primary_key == 'Y' || r.unique == 'y' || r.unique == 'Y')
                {

                    if (r.data_type.toCSharpDataType().Equals("string"))
                    {
                        if (r.default_value.ToLower().Contains("uuid"))
                        {
                            _ = array.Add(generateRandomString(r, 0));

                        }
                        else
                        {
                            _ = array.Add(generateRandomString(r, -2));
                        }

                        _ = Task.Delay(1);
                    }
                    else if (r.data_type.toCSharpDataType().Equals("bool"))
                    {
                        _ = array.Add(true);
                    }
                    else if (r.data_type.toCSharpDataType().Equals("int"))
                    {
                        _ = array.Add(rand.Next(0, 10000));
                        _ = Task.Delay(1);
                    }
                    else
                    {
                        _ = array.Add(null);
                    }
                    _ = Task.Delay(5);
                }
            }

            //method signature
            result += "public void test" + name + "KeyedParameterizedConstructorSetsKeyedVariables(){\n";
            //constructor
            result += name + " _" + name.ToLower() + "= new " + name + "(\n";
            //assing a value to each variable
            string comma = "";
            int i = 0;
            foreach (Column r in columns)
            {
                if (r.primary_key == 'y' || r.primary_key == 'Y' || r.unique == 'y' || r.unique == 'Y')
                {

                    if (r.data_type.toCSharpDataType().Equals("string"))
                    {
                        result += comma + "\"" + array[i] + "\"";
                    }
                    else if (r.data_type.toCSharpDataType().Equals("bool"))
                    {
                        result += comma + array[i];
                    }
                    else if (r.data_type.toCSharpDataType().Equals("int"))
                    {
                        result += comma + array[i];
                    }
                    else
                    {
                        result += comma + "new " + r.data_type + "()\n";
                    }
                    comma = ",\n ";

                    i++;
                }
            }
            result += "\n);\n";
            i = 0;
            //test each variable
            foreach (Column r in columns)
            {
                if (r.primary_key == 'y' || r.primary_key == 'Y' || r.unique == 'y' || r.unique == 'Y')
                {

                    if (r.data_type.toCSharpDataType().Equals("string"))
                    {
                        result += "Assert.AreEqual(\"" + array[i] + "\",_" + name.ToLower() + "." + r.column_name + ");\n";
                    }
                    else if (r.data_type.toCSharpDataType().Equals("bool"))
                    {
                        result += "Assert.IsTrue(_" + name.ToLower() + "." + r.column_name + ");\n";
                    }
                    else if (r.data_type.toCSharpDataType().Equals("int"))
                    {
                        result += "Assert.AreEqual(" + array[i] + ",_" + name.ToLower() + "." + r.column_name + ");\n";
                    }
                    else
                    {
                        result += "Assert.AreEqual(new " + r.data_type + "(),_" + name.ToLower() + "." + r.column_name + ");\n";
                    }
                    i++;
                }
                else
                {
                    if (r.data_type.toCSharpDataType().Equals("string"))
                    {
                        result += "Assert.IsNull(_" + name.ToLower() + "." + r.column_name + ");\n";
                    }
                    else if (r.data_type.toCSharpDataType().Equals("bool"))
                    {
                        result += "Assert.IsFalse(_" + name.ToLower() + "." + r.column_name + ");\n";
                    }
                    else if (r.data_type.toCSharpDataType().Equals("int"))
                    {
                        result += "Assert.AreEqual(_" + name.ToLower() + "." + r.column_name + ",0);\n";
                    }
                    else
                    {
                        result += "Assert.IsNull(_" + name.ToLower() + "." + r.column_name + ");\n";
                    }
                }
            }
            result += "}\n";
            return result;

        }
        private string testCSharpCompareTo()
        {
            return "";

        }
        private string testCSharpVMInitialize()
        {
            string result = "";
            result += "using System;\n";
            result += "using System.Collections.Generic;\n";
            result += "using System.Linq;\n";
            result += "using System.Text;\n";
            result += "using System.Threading.Tasks;\n";
            result += "using DataObjects;\n";
            result += "\n";
            result += "namespace DataObjectTests\n";
            
            result += "{\n";
            result += "[TestClass]\n";

            result += "public class " + name + "VMTests\n";
            result += "{\n";
            result += name + "VM _" + name.ToLower() + ";\n";
            result += "[TestInitialize]\n";
            result += "public void setup() {\n";
            result +=  " _" + name.ToLower() + " = new " + name + "();\n";
            result += "}\n";
            result += "[TestCleanup]\n";
            result += "public void tearDown() {";
            result += "_" + name.ToLower() + "= null;\n";
            result += "}\n";
            return result;

        }
        private string testCSharpVMDefaultConstructor()
        {
            string result = commentBox.genJavaTestJavaDoc(JavaTestType.ParamartizedConstructor, this);
            result += "@Test\n"; ;
            result += "public void test" + name + "DefaultConstructorSetsNoVariables(){\n";
            result += name + "_VM _" + name.ToLower() + "VM= new " + name + "_VM();\n";
            foreach (Column r in columns)
            {
                if (r.data_type.toCSharpDataType().Equals("string"))
                {
                    result += "Assert.AreEqual(_" + name.ToLower() + "VM." + r.column_name + ");\n";
                }
                else if (r.data_type.toCSharpDataType().Equals("bool"))
                {
                    result += "Assert.IsFalse(_" + name.ToLower() + "VM." + r.column_name + ");\n";
                }
                else if (r.data_type.toCSharpDataType().Equals("int"))
                {
                    result += "Assert.AreEqual(_" + name.ToLower() + "VM." + r.column_name + ");\n";
                }
                else
                {
                    result += "Assert.AreEqual(_" + name.ToLower() + "VM." + r.column_name + ");\n";
                }
            }
            foreach (Column r in columns)
            {

                if (r.references != null && r.references != "")
                {
                    string[] parts = r.references.Split('.');
                    string fk_table = parts[0];
                    string fk_name = parts[1];
                    result += "Assert.IsNull(_" + name.ToLower() + "VM." + fk_table + ");\n";
                }
            }
            foreach (foreignKey key in data_tables.all_foreignKey)
            {
                if (key.referenceTable.ToLower().Equals(name.ToLower()))
                {
                    string child_table = key.mainTable;

                    result += "Assert.IsNull(_" + name.ToLower() + "VM." + child_table + "s);\n";
                }
            }
            result += "}\n";
            return result;

        }
        private string testCSharpVMParameterizedConstructors()
        {
            string result = "";
            bool hasParent = false;
            bool hasChild = false;
            foreach (Column r in columns)
            {
                if (r.references != "")
                {
                    hasParent = true;

                }
            }
            foreach (foreignKey key in data_tables.all_foreignKey)
            {
                if (key.referenceTable.ToLower().Equals(name.ToLower()))
                {
                    hasChild = true;
                }
            }

            for (int j = 0; j < 3; j++)
            {

                //testing can set as super

                //generate random values for each
                ArrayList array = new ArrayList(columns.Count + 1);
                bool hasdate = false;

                foreach (Column r in columns)
                {
                    if (r.column_name.ToLower().Contains("date"))
                    {
                        hasdate = true;
                        break;
                    }
                }

                foreach (Column r in columns)
                {
                    if (r.data_type.toCSharpDataType().Equals("string"))
                    {
                        if (r.default_value.ToLower().Contains("uuid"))
                        {
                            array.Add(generateRandomString(r, 0));
                        }
                        else
                        {
                            array.Add(generateRandomString(r, -2));

                        }

                        _ = Task.Delay(1);
                    }
                    else if (r.data_type.toCSharpDataType().Equals("bool"))
                    {
                        _ = array.Add(true);
                    }
                    else if (r.data_type.toCSharpDataType().Equals("int"))
                    {
                        _ = array.Add(rand.Next(0, 10000));
                        _ = Task.Delay(1);
                    }
                    else if (r.data_type.Equals("decimal"))
                    {
                        double toAdd = rand.Next(0, 10000) / 100.0;
                        _ = array.Add(toAdd);
                        _ = Task.Delay(1);
                    }
                    else if (r.data_type.ToLower().Contains("date"))
                    {
                        int year = rand.Next(2015, 2027);
                        int month = rand.Next(1, 11);
                        int day = rand.Next(1, 28);
                        int hour = rand.Next(1, 23);
                        int minute = rand.Next(1, 55);
                        int second = rand.Next(2, 45);
                        DateTime toAdd = new DateTime(year, month, day, hour, minute, second);
                        _ = array.Add(toAdd);
                        _ = Task.Delay(1);
                    }
                    else
                    {
                        _ = array.Add(null);
                    }
                    _ = Task.Delay(5);
                }
                //method signature
                result += "\n";
                result += commentBox.genJavaTestJavaDoc(JavaTestType.ParamartizedConstructor, this);
                result += "[TestMethod]\n";
                switch (j)
                {

                    case 0:
                        result += "public void testSuper" + name + "ParameterizedVMConstructorSetsAllVariables()";
                        break;
                    case 1:
                        if (hasParent)
                        {
                            result += "public void testSuper" + name + "ParameterizedVMConstructorSetsAllVariablesAndParent()";
                        }
                        break;
                    case 2:
                        if (hasChild)
                        {
                            result += "public void testSuper" + name + "ParameterizedVMConstructorSetsAllVariablesAndChildren()";
                        }
                        break;

                }
               
                result += "{\n";
                if (hasdate)
                {
                    DateTime today = DateTime.Today;
                    result += "String strDate = \"" + today.Day + "/" + today.Month + "/" + today.Year + "\";\n";
                    result += "Date date = DateTime.ParseExact(strDate,\"dd/mm/yyyy\");\n";
                    
                }
                //constructor
                result += name + " _" + name.ToLower() + "= new " + name + "(\n";
                //assing a value to each variable
                string comma = "";
                int i = 0;
                foreach (Column r in columns)
                {

                    if (r.data_type.toCSharpDataType().Equals("string"))
                    {
                        result += comma + "\"" + array[i] + "\"";
                    }
                    else if (r.data_type.toCSharpDataType().Equals("bool"))
                    {
                        result += comma + array[i];
                    }
                    else if (r.data_type.toCSharpDataType().Equals("int"))
                    {
                        result += comma + array[i];
                    }
                    else if (r.data_type.Equals("decimal"))
                    {
                        result += comma + array[i];
                    }
                    else if (r.data_type.ToLower().Contains("date"))
                    {
                        result += comma + "DateTime.ParseExact(\"" + array[i] + "\")";
                    }
                    else
                    {
                        result += comma + "new " + r.data_type + "()";
                    }
                    comma = ",\n ";

                    i++;
                }
                result += "\n);\n";
                switch (j)
                {
                    case 0:
                        result += "_" + name.ToLower() + "VM = new " + name + "_VM(_" + name.ToLower() + ");\n";
                        break;
                    case 1:
                        foreach (Column r in columns)
                        {
                            if (r.references != "")
                            {
                                string[] parts = r.references.Split('.');
                                string fk_table = parts[0];
                                string fk_name = parts[1];
                                result += fk_table + " _" + fk_table.ToLower() + " = new " + fk_table + "();\n";
                            }
                        }
                        result += "_" + name.ToLower() + "VM = new " + name + "_VM(_" + name.ToLower();
                        foreach (Column r in columns)
                        {
                            if (r.references != "")
                            {
                                string[] parts = r.references.Split('.');
                                string fk_table = parts[0];
                                string fk_name = parts[1];
                                result += ", _" + fk_table.ToLower();
                            }
                        }
                        result += ");\n";
                        break;
                    case 2:
                        foreach (foreignKey key in data_tables.all_foreignKey)
                        {

                            if (key.referenceTable.ToLower().Equals(name.ToLower()))
                            {

                                string fk_table = key.mainTable;

                                result += "List<" + fk_table + "> _" + fk_table.ToLower() + "s = new ArrayList<>();\n";
                            }
                        }
                        result += "_" + name.ToLower() + "VM = new " + name + "_VM(_" + name.ToLower();
                        foreach (foreignKey key in data_tables.all_foreignKey)
                        {

                            if (key.referenceTable.ToLower().Equals(name.ToLower()))
                            {

                                string fk_table = key.mainTable;

                                result += ", " + fk_table.ToLower() + "s";
                            }
                        }
                        result += ");\n";
                        break;
                }
                i = 0;
                //test each variable
                foreach (Column r in columns)
                {
                    if (r.data_type.toCSharpDataType().Equals("string"))
                    {
                        result += "Assert.AreEqual(\"" + array[i] + "\",_" + name.ToLower() + "VM." + r.column_name + ");\n";
                    }
                    else if (r.data_type.toCSharpDataType().Equals("bool"))
                    {
                        result += "Assert.IsTrue(_" + name.ToLower() + "VM." + r.column_name + ");\n";
                    }
                    else if (r.data_type.toCSharpDataType().Equals("int"))
                    {
                        result += "Assert.AreEqual(" + array[i] + ",_" + name.ToLower() + "VM." + r.column_name + ");\n";
                    }
                    else if (r.data_type.Equals("decimal"))
                    {
                        result += "Assert.AreEqual(" + array[i] + ",_" + name.ToLower() + "VM." + r.column_name + ");\n";
                    }
                    else if (r.data_type.ToLower().Contains("date"))
                    {
                        result += "Assert.AreEqual(df.parse(\"" + array[i] + "\"),_" + name.ToLower() + "VM." + r.column_name + ");\n";
                    }

                    else
                    {
                        result += "Assert.AreEqual(new " + r.data_type + "(),_" + name.ToLower() + "VM." + r.column_name + ");\n";
                    }
                    i++;
                }
                switch (j)
                {
                    case 0: break;
                    case 1:
                        foreach (Column k in columns)
                        {
                            if (k.references != "")
                            {
                                string[] parts = k.references.Split('.');
                                string fk_table = parts[0];
                                string fk_name = parts[1];
                                result += "Assert.AreEqual(_" + fk_table.ToLower() + " ,_" + name.ToLower() + "VM." + fk_table + ");\n";

                            }
                        }
                        break;
                    case 2:
                        foreach (foreignKey key in data_tables.all_foreignKey)
                        {
                            if (key.referenceTable.ToLower().Equals(name.ToLower()))
                            {

                                string fk_table = key.mainTable;

                                result += "Assert.AreEqual(_" + fk_table.ToLower() + "s,_" + name.ToLower() + "VM." + fk_table + "s);\n";

                            }
                        }
                        break;
                }

                result += "}\n";

            }

            return result;

        }


        private string testCSharpObjectSet(string objectname)
        {
            string result = "[TestMethod]\n";
            result += "public void testSet" + objectname + "Sets" + objectname + "(){\n";
            result += objectname + " _" + objectname.ToLower() + " = new " + objectname + "();\n";
            result += "_" + name.ToLower() + "VM." + objectname + "= _" + objectname.ToLower() + ";\n";
            result += "Assert.AreEqual(_" + objectname.ToLower() + ",_" + name.ToLower() + "VM." + objectname + ");\n";
            result += "}\n";
            return result;


        }
        private string testCSharpListObjectSet(string objectname)
        {
            string result = "[TestMethod]\n";
            result += "public void testSet" + objectname + "sSets" + objectname + "s(){\n";
            result += "List<" + objectname + "> _" + objectname.ToLower() + "s = new List<" + objectname + ">();\n";
            result += "_" + name.ToLower() + "VM." + objectname + "s = _" + objectname.ToLower() + "s;\n";
            result += "Assert.AreEqual(_" + objectname.ToLower() + "s,_" + name.ToLower() + "VM.get" + objectname + "s());\n";
            result += "}\n";
            return result;

        }
        private string testCSharpTooShort(Column r)
        {
            string result = commentBox.genJavaTestJavaDoc(JavaTestType.SetterThrowsException, this, r);
            result += "[TestMethod]\n";
            //result += "[ExpectedException(typeof(ArgumentOutOfRangeException))]\n";
            result += "public void  test" + name + "FailsValidationIf" + r.column_name + "TooShort(){\n";
            result += "String " + r.column_name + " = \"";
            String dummy = generateRandomString(r, 2 - r.length);
            result += dummy; ;
            result += "\";\n";
            result += "_" + name.ToLower() + "." + r.column_name + "=" + r.column_name + ";\n";
            result += "bool result = false;";
            result += "var lstErrors = ValidateModel(_"+name.ToLower()+",ref result);\n";
            result += "Assert.IsTrue(lstErrors.Where(x => x.ErrorMessage.Contains(\""+r.column_name+"\")).Count() > 0);\r\n";
            result += "}\n";
            return result;

        }
        private string testCSharpTooLong(Column r)
        {
            string result = commentBox.genJavaTestJavaDoc(JavaTestType.SetterThrowsException, this, r);
            result += "[TestMethod]\n";
            //result += "[ExpectedException(typeof(ArgumentOutOfRangeException))]\n";
            result += "public void  test" + name + "FailsValidationIf" + r.column_name + "TooLong(){\n";
            result += "String " + r.column_name + " = \"";
            String dummy = generateRandomString(r, +2);
            result += dummy;
            result += "\";\n";
            result += "_" + name.ToLower() + "." + r.column_name + "=" + r.column_name + ";\n";
            result += "bool result = false;";
            result += "var lstErrors = ValidateModel(_" + name.ToLower() + ",ref result);\n";
            result += "Assert.IsTrue(lstErrors.Where(x => x.ErrorMessage.Contains(\"" + r.column_name + "\")).Count() > 0);\r\n";
            result += "}\n";
            return result;

        }
        private string testCSharpStringSet(Column r)
        {
            String dummy = "";

            if (r.default_value.ToLower().Contains("uuid"))
            {
                dummy = generateRandomString(r, 0);
            }
            else
            {
                dummy = generateRandomString(r, -2);
            }

            string result = commentBox.genJavaTestJavaDoc(JavaTestType.SetterWorks, this, r);
            result += "[TestMethod]\n";
            result += "public void testSet" + r.column_name + "Sets" + r.column_name + "(){\n";
            result += "String " + r.column_name + " = \"" + dummy + "\";\n";
            result += "_" + name.ToLower() + "." + r.column_name +" = " + r.column_name + ";\n";
            result += "Assert.AreEqual(" + r.column_name + ",_" + name.ToLower() + "." + r.column_name + ");\n";
            result += "}\n";
            return result;


        }
        private string testCSharpIntTooSmall(Column r)
        {
            string result = commentBox.genJavaTestJavaDoc(JavaTestType.SetterThrowsException, this, r);
            result += "[TestMethod]\n";
            //result += "[ExpectedException(typeof(ArgumentOutOfRangeException))]\n";
            result += "public void test" + name + "FailsValidationIf" + r.column_name + "TooSmall(){\n";
            result += "int " + r.column_name + " = -1;\n";
            result += "_" + name.ToLower() + "." + r.column_name + " = " + r.column_name + ";\n";
            result += "bool result = false;";
            result += "var lstErrors = ValidateModel(_" + name.ToLower() + ",ref result);\n";
            result += "Assert.IsTrue(lstErrors.Where(x => x.ErrorMessage.Contains(\"" + r.column_name + "\")).Count() > 0);\r\n";
            result += "}\n";
            return result;
            

        }
        private string testCSharpIntTooBig(Column r)
        {
            string result = commentBox.genJavaTestJavaDoc(JavaTestType.SetterThrowsException, this, r);
            result += "[TestMethod]\n";
            //result += "[ExpectedException(typeof(ArgumentOutOfRangeException))]\n";
            result += "public void test" + name + "FailsValidationIf" + r.column_name + "TooBig(){\n";
            result += "int " + r.column_name + " = 10001;\n";
            result += "_" + name.ToLower() + "." + r.column_name + "=" + r.column_name + ";\n";
            result += "bool result = false;";
            result += "var lstErrors = ValidateModel(_" + name.ToLower() + ",ref result);\n";
            result += "Assert.IsTrue(lstErrors.Where(x => x.ErrorMessage.Contains(\"" + r.column_name + "\")).Count() > 0);\r\n";
            result += "}\n";
            return result;

        }
        private string testCSharpIntSet(Column r)
        {
            int numberToTest = rand.Next(1, 10000);
            string result = commentBox.genJavaTestJavaDoc(JavaTestType.SetterWorks, this, r);
            result += "[TestMethod]\n";
            result += "public void test" + name + "Sets" + r.column_name + "(){\n";
            result += "int " + r.column_name + " = " + numberToTest + ";\n";
            result += "_" + name.ToLower() + "." + r.column_name + " = " + r.column_name + ";\n";
            result += "Assert.AreEqual(" + r.column_name + ", _" + name.ToLower() + "." + r.column_name + ");\n";
            result += "}\n";

            return result;

        }
        private string testCSharpDecimalTooSmall(Column r)
        {
            string result = commentBox.genJavaTestJavaDoc(JavaTestType.SetterThrowsException, this, r);
            result += "[TestMethod]\n";
            //result += "[ExpectedException(typeof(ArgumentOutOfRangeException))]\n";
            result += "public void test" + name + "FailsValidationIf" + r.column_name + "TooSmall(){\n";
            result += "double " + r.column_name + " = -1;\n";
            result += "_" + name.ToLower() + "." + r.column_name + " = " + r.column_name + ";\n";
            result += "bool result = false;";
            result += "var lstErrors = ValidateModel(_" + name.ToLower() + ",ref result);\n";
            result += "Assert.IsTrue(lstErrors.Where(x => x.ErrorMessage.Contains(\"" + r.column_name + "\")).Count() > 0);\r\n";
            result += "}\n";
            return result;

        }
        private string testCSharpDecimalTooBig(Column r)
        {
            string result = commentBox.genJavaTestJavaDoc(JavaTestType.SetterThrowsException, this, r);
            result += "[TestMethod]\n";
            //result += "[ExpectedException(typeof(ArgumentOutOfRangeException))]\n";
            result += "public void test" + name + "FailsValidationIf" + r.column_name + "TooBig(){\n";
            result += "double " + r.column_name + " = 10001;\n";
            result += "_" + name.ToLower() + "." + r.column_name + " = " + r.column_name + ";\n";
            result += "bool result = false;";
            result += "var lstErrors = ValidateModel(_" + name.ToLower() + ",ref result);\n";
            result += "Assert.IsTrue(lstErrors.Where(x => x.ErrorMessage.Contains(\"" + r.column_name + "\")).Count() > 0);\r\n";
            result += "}\n";
            return result;

        }
        private string testCSharpDecimalSet(Column r)
        {
            int numberToTest = rand.Next(1, 10000);
            string result = commentBox.genJavaTestJavaDoc(JavaTestType.SetterWorks, this, r);
            result += "[TestMethod]\n";
            
            result += "public void test" + name + "Sets" + r.column_name + "(){\n";
            result += "double " + r.column_name + " = " + numberToTest + ";\n";
            result += "_" + name.ToLower() + ".set" + r.column_name + "(" + r.column_name + ");\n";
            result += "Assert.AreEqual(" + r.column_name + ", _" + name.ToLower() + "." + r.column_name + ");\n";
            result += "}\n";

            return result;

        }
        private string testCSharpBoolSetFalse(Column r)
        {
            string result = commentBox.genJavaTestJavaDoc(JavaTestType.SetterWorks, this, r);
            result += "[TestMethod]\n";
            result += "public void test" + name + "Sets" + r.column_name + "asFalse(){\n";
            result += "boolean status = false;\n";
            result += "_" + name.ToLower() + "." + r.column_name + "= status;\n";
            result += "Assert.IsFalse( _" + name.ToLower() + "." + r.column_name + ");\n";
            result += "}\n";
            return result;

        }
        private string testCSharpBoolSetTrue(Column r)
        {
            string result = commentBox.genJavaTestJavaDoc(JavaTestType.SetterWorks, this, r);
            result += "[TestMethod]\n";
            result += "public void test" + name + "Sets" + r.column_name + "asTrue(){\n";
            result += "boolean status = true;\n";
            result += "_" + name.ToLower() + "." + r.column_name + "= status;\n";
            result += "Assert.IsTrue( _" + name.ToLower() + "." + r.column_name + ");\n";
            result += "}\n";
            return result;

        }
        private string testCSharpDatetimeTooSmall(Column r)
        {
            string result = commentBox.genJavaTestJavaDoc(JavaTestType.SetterThrowsException, this, r);
            result += "[TestMethod]\n";
            //result += "[ExpectedException(typeof(ArgumentOutOfRangeException))]\n";
            result += "public void test" + name + "FailsValidationIf" + r.column_name + "TooSmall() throws ParseException{\n";
            result += "String strDate = \"03/03/1990\";\n";
            result += "Date date = DateTime.ParseExact(strDate,\"dd/mm/yyyy\");\n";
            result += "_" + name.ToLower() + "." + r.column_name + " = date;\n";
            result += "bool result = false;";
            result += "var lstErrors = ValidateModel(_" + name.ToLower() + ",ref result);\n";
            result += "Assert.IsTrue(lstErrors.Where(x => x.ErrorMessage.Contains(\"" + r.column_name + "\")).Count() > 0);\r\n";
            result += "}\n";
            return result;

        }
        private string testCSharpDatetimeTooBig(Column r)
        {
            string result = commentBox.genJavaTestJavaDoc(JavaTestType.SetterThrowsException, this, r);
            result += "[TestMethod]\n";
            //result += "[ExpectedException(typeof(ArgumentOutOfRangeException))]\n";
            result += "public void test" + name + "FailsValidationIf" + r.column_name + "TooBig() throws ParseException{\n";
            result += "String strDate = \"01/01/2190\";\n";            
            result += "Date date = DateTime.ParseExact(strDate,\"dd/mm/yyyy\");\n";
            result += "_" + name.ToLower() + "." + r.column_name + " = date;\n";
            result += "bool result = false;";
            result += "var lstErrors = ValidateModel(_" + name.ToLower() + ",ref result);\n";
            result += "Assert.IsTrue(lstErrors.Where(x => x.ErrorMessage.Contains(\"" + r.column_name + "\")).Count() > 0);\r\n";
            result += "}\n";
            return result;

        }
        private string testCSharpDatetimeSet(Column r)
        {
            
            string result = commentBox.genJavaTestJavaDoc(JavaTestType.SetterWorks, this, r);
            result += "[TestMethod]\n";
            
            result += "public void test" + name + "Sets" + r.column_name + "() throws ParseException{\n";
            DateTime today = DateTime.Today;
            result += "String strDate = \"" + today.Day + "/" + today.Month + "/" + today.Year + "\";\n";

            result += "Date date = DateTime.ParseExact(strDate,\"dd/mm/yyyy\");\n";
            result += "_" + name.ToLower() + "." + r.column_name + " = date;\n";
            result += "Assert.AreEqual(date, _" + name.ToLower() + "." + r.column_name + ");\n";
            result += "}\n";

            return result;

        }

        private string genCSharpValidationMethod() {
            string result = "private IList<ValidationResult> ValidateModel(object model, ref bool result)\n";
            result += "{\b";
            result += "var validationResults = new List<ValidationResult>();\n";
            result += "var ctx = new ValidationContext(model, null, null);\n";
            result += "result = Validator.TryValidateObject(model, ctx, validationResults, true);\n";
            result += "return validationResults;\n";
            result += "}\n";
            
            return result;
        
        
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string createJavaModelTests()
        {
            string result = "";
            result += testJavaModelInitialize(); //done
            
            result += testJavaDefaultConstructor();
            result += testJavaParameterizedConstructor();
            result += testJavaKeyedParameterizedConstructor();

            //result += testJavaVMDefaultConstructor();
            //result += testVMParameterizedConstructor();
            foreach (Column r in columns)
            {
                result += createJavaColumnTests(r);
            }
            result += testJavaCompareTo();
            result += "\n}\n";
            return result;
        }
        public string createJavaModelVMTests()
        {
            string result = "";
            result += testJavaVMInitialize(); //done
            result += testJavaVMDefaultConstructor();
            result += testJavaVMParameterizedConstructors();

            foreach (Column r in columns)
            {
                if (r.references != null && r.references != "")
                {
                    string[] parts = r.references.Split('.');
                    string fk_table = parts[0];
                    string fk_name = parts[1];
                    result += testJavaObjectSet(fk_table);
                }
            }
            foreach (foreignKey key in data_tables.all_foreignKey)
            {
                if (key.referenceTable.ToLower().Equals(name.ToLower()))
                {
                    result += testJavaListObjectSet(key.mainTable);
                }
            }
            //result += testVMCompareTo();
            result += "\n}\n";
            return result;
        }
        private string createJavaColumnTests(Column r)
        {
            string result = "";

            if (r.data_type.toCSharpDataType().Equals("string"))
            {
                
                result += testJavaTooShort(r);  //done
                result += testJavaTooLong(r); //done
                result += testJavaStringSet(r); //done
            }
            if (r.data_type.toCSharpDataType().Equals("int"))
            {
                result += testJavaIntTooSmall(r); //done
                result += testJavaIntTooBig(r); // done
                result += testJavaIntSet(r); //done
            }
            if (r.data_type.Equals("decimal"))
            {
                result += testJavaDecimalTooSmall(r); //done
                result += testJavaDecimalTooBig(r); // done
                result += testJavaDecimalSet(r); //done
            }
            if (r.data_type.toCSharpDataType().Equals("bool"))
            {
                result += testJavaBoolSetFalse(r); //done
                result += testJavaBoolSetTrue(r); //done
            }
            if (r.data_type.Equals("datetime"))
            {
                result += testJavaDatetimeTooSmall(r); //done
                result += testJavaDatetimeTooBig(r); // done
                result += testJavaDatetimeSet(r); //done
            }

            return result;

        }
        private string testJavaModelInitialize()
        {
            string result = "";
            result += "import org.junit.jupiter.api.AfterEach;\n";
            result += "import org.junit.jupiter.api.Assertions;\n";
            result += "import org.junit.jupiter.api.BeforeEach;\n";
            result += "import org.junit.jupiter.api.Test;\n";
            result += "import static org.junit.jupiter.api.Assertions.*;\n";
            result += "class " + name + "Test {\n";
            result += "private " + name + " " + "_" + name.ToLower() + ";\n";
            result += "@BeforeEach\n";
            result += "public void setup(){\n";
            result += "_" + name.ToLower() + " = new " + name + "();\n";
            result += "}\n";
            result += "@AfterEach\n";
            result += "public void teardown(){\n";
            result += "_" + name.ToLower() + " = null;\n";
            result += "}\n";

            return result;

        }

        private string testJavaVMInitialize()
        {
            string result = "";
            result += "import org.junit.jupiter.api.AfterEach;\n";
            result += "import org.junit.jupiter.api.Assertions;\n";
            result += "import org.junit.jupiter.api.BeforeEach;\n";
            result += "import org.junit.jupiter.api.Test;\n";
            result += "import static org.junit.jupiter.api.Assertions.*;\n";
            result += "class " + name + "_VMTest {\n";
            result += "private " + name + "_VM " + "_" + name.ToLower() + "VM;\n";
            result += "@BeforeEach\n";
            result += "public void setup(){\n";
            result += "_" + name.ToLower() + "VM = new " + name + "_VM();\n";
            result += "}\n";
            result += "@AfterEach\n";
            result += "public void teardown(){\n";
            result += "_" + name.ToLower() + "VM = null;\n";
            result += "}\n";

            return result;

        }

        private string testJavaParameterizedConstructor()
        {
            string result = commentBox.genJavaTestJavaDoc(JavaTestType.ParamartizedConstructor, this);
            result += "@Test\n";
            //generate random values for each
            ArrayList array = new ArrayList(columns.Count + 1);

            foreach (Column r in columns)
            {

                if (r.data_type.toCSharpDataType().Equals("string"))
                {
                    if (r.default_value.ToLower().Contains("uuid"))
                    {
                        _ = array.Add(generateRandomString(r, 0));
                    }
                    else {
                        _ = array.Add(generateRandomString(r, -2));
                    }
                    
                    _ = Task.Delay(1);
                }
                else if (r.data_type.toCSharpDataType().Equals("bool"))
                {
                    _ = array.Add(true);
                }
                else if (r.data_type.toCSharpDataType().Equals("int"))
                {
                    _ = array.Add(rand.Next(0, 10000));
                    _ = Task.Delay(1);
                }
                else if (r.data_type.Equals("decimal"))
                {
                    double toAdd = rand.Next(0, 10000) / 100.0;
                    _ = array.Add(toAdd);
                    _ = Task.Delay(1);
                }
                else if (r.data_type.ToLower().Contains("date"))
                {
                    int year = rand.Next(2015, 2027);
                    int month = rand.Next(1, 11);
                    int day = rand.Next(1, 28);
                    DateTime toAdd = new DateTime(year, month, day);
                    _ = array.Add(toAdd);
                    _ = Task.Delay(1);
                }
                else
                {
                    _ = array.Add(null);
                }
                _ = Task.Delay(5);

            }

            //method signature
            result += "public void test" + name + "ParameterizedConstructorSetsAllVariables(){\n";
            //constructor
            result += name + " _" + name.ToLower() + "= new " + name + "(\n";
            //assing a value to each variable
            string comma = "";
            int i = 0;
            foreach (Column r in columns)
            {

                if (r.data_type.toCSharpDataType().Equals("string"))
                {
                    result += comma + "\"" + array[i] + "\"";
                }
                else if (r.data_type.toCSharpDataType().Equals("bool"))
                {
                    result += comma + array[i];
                }
                else if (r.data_type.toCSharpDataType().Equals("int"))
                {
                    result += comma + array[i];
                }
                else if (r.data_type.Equals("decimal"))
                {
                    result += comma + array[i];
                }
                else if (r.data_type.ToLower().Contains("decimal"))
                {
                    result += comma + array[i];
                }
                else
                {
                    result += comma + "new " + r.data_type + "()\n";
                }
                comma = ",\n ";

                i++;
            }
            result += "\n);\n";
            i = 0;
            //test each variable
            foreach (Column r in columns)
            {
                if (r.data_type.toCSharpDataType().Equals("string"))
                {
                    result += "Assertions.assertEquals(\"" + array[i] + "\",_" + name.ToLower() + ".get" + r.column_name + "());\n";
                }
                else if (r.data_type.toCSharpDataType().Equals("bool"))
                {
                    result += "Assertions.assertTrue(_" + name.ToLower() + ".get" + r.column_name + "());\n";
                }
                else if (r.data_type.toCSharpDataType().Equals("int"))
                {
                    result += "Assertions.assertEquals(" + array[i] + ",_" + name.ToLower() + ".get" + r.column_name + "());\n";
                }
                else if (r.data_type.Equals("decimal"))
                {
                    result += "Assertions.assertEquals(" + array[i] + ",_" + name.ToLower() + ".get" + r.column_name + "());\n";
                }
                else if (r.data_type.ToLower().Contains("decimal"))
                {
                    result += "Assertions.assertEquals(" + array[i] + ",_" + name.ToLower() + ".get" + r.column_name + "());\n";
                }

                else
                {
                    result += "Assertions.assertEquals(new " + r.data_type + "(),_" + name.ToLower() + ".get" + r.column_name + "());\n";
                }
                i++;

            }
            result += "}\n";
            return result;

        }

        private string testJavaDefaultConstructor()
        {
            string result = commentBox.genJavaTestJavaDoc(JavaTestType.DefaultConstructor, this);
             result += "@Test\n";
            result += "public void test" + name + "DefaultConstructorSetsNoVariables(){\n";
            result += name + " _" + name.ToLower() + "= new " + name + "();\n";
            foreach (Column r in columns)
            {
                if (r.data_type.toCSharpDataType().Equals("string"))
                {
                    result += "Assertions.assertNull(_" + name.ToLower() + ".get" + r.column_name + "());\n";
                }
                else if (r.data_type.toCSharpDataType().Equals("bool"))
                {
                    result += "Assertions.assertFalse(_" + name.ToLower() + ".get" + r.column_name + "());\n";
                }
                else if (r.data_type.toCSharpDataType().Equals("int"))
                {
                    result += "Assertions.assertNull(_" + name.ToLower() + ".get" + r.column_name + "());\n";
                }
                else
                {
                    result += "Assertions.assertNull(_" + name.ToLower() + ".get" + r.column_name + "());\n";
                }
            }
            result += "}\n";
            return result;

        }
        private string testJavaVMDefaultConstructor()
        {
            string result = commentBox.genJavaTestJavaDoc(JavaTestType.ParamartizedConstructor, this);
            result += "@Test\n"; ;
            result += "public void test" + name + "DefaultConstructorSetsNoVariables(){\n";
            result += name + "_VM _" + name.ToLower() + "VM= new " + name + "_VM();\n";
            foreach (Column r in columns)
            {
                if (r.data_type.toCSharpDataType().Equals("string"))
                {
                    result += "Assertions.assertNull(_" + name.ToLower() + "VM.get" + r.column_name + "());\n";
                }
                else if (r.data_type.toCSharpDataType().Equals("bool"))
                {
                    result += "Assertions.assertFalse(_" + name.ToLower() + "VM.get" + r.column_name + "());\n";
                }
                else if (r.data_type.toCSharpDataType().Equals("int"))
                {
                    result += "Assertions.assertNull(_" + name.ToLower() + "VM.get" + r.column_name + "());\n";
                }
                else
                {
                    result += "Assertions.assertNull(_" + name.ToLower() + "VM.get" + r.column_name + "());\n";
                }
            }
            foreach (Column r in columns)
            {

                if (r.references != null && r.references != "")
                {
                    string[] parts = r.references.Split('.');
                    string fk_table = parts[0];
                    string fk_name = parts[1];
                    result += "Assertions.assertNull(_" + name.ToLower() + "VM.get" + fk_table + "());\n";
                }
            }
            foreach (foreignKey key in data_tables.all_foreignKey)
            {
                if (key.referenceTable.ToLower().Equals(name.ToLower()))
                {
                    string child_table = key.mainTable;

                    result += "Assertions.assertNull(_" + name.ToLower() + "VM.get" + child_table + "s());\n";
                }
            }
            result += "}\n";
            return result;

        }

        private string testJavaKeyedParameterizedConstructor()
        {
            string result = commentBox.genJavaTestJavaDoc(JavaTestType.ParamartizedConstructor, this);
            result += "@Test\n";
            //generate random values for each
            ArrayList array = new ArrayList(columns.Count + 1);

            foreach (Column r in columns)
            {
                if (r.primary_key == 'y' || r.primary_key == 'Y' || r.unique == 'y' || r.unique == 'Y')
                {

                    if (r.data_type.toCSharpDataType().Equals("string"))
                    {
                        if (r.default_value.ToLower().Contains("uuid"))
                        {
                            _ = array.Add(generateRandomString(r, 0));

                        }
                        else {
                            _ = array.Add(generateRandomString(r, -2));
                        }
                        
                        _ = Task.Delay(1);
                    }
                    else if (r.data_type.toCSharpDataType().Equals("bool"))
                    {
                        _ = array.Add(true);
                    }
                    else if (r.data_type.toCSharpDataType().Equals("int"))
                    {
                        _ = array.Add(rand.Next(0, 10000));
                        _ = Task.Delay(1);
                    }
                    else
                    {
                        _ = array.Add(null);
                    }
                    _ = Task.Delay(5);
                }
            }

            //method signature
            result += "public void test" + name + "KeyedParameterizedConstructorSetsKeyedVariables(){\n";
            //constructor
            result += name + " _" + name.ToLower() + "= new " + name + "(\n";
            //assing a value to each variable
            string comma = "";
            int i = 0;
            foreach (Column r in columns)
            {
                if (r.primary_key == 'y' || r.primary_key == 'Y' || r.unique == 'y' || r.unique == 'Y')
                {

                    if (r.data_type.toCSharpDataType().Equals("string"))
                    {
                        result += comma + "\"" + array[i] + "\"";
                    }
                    else if (r.data_type.toCSharpDataType().Equals("bool"))
                    {
                        result += comma + array[i];
                    }
                    else if (r.data_type.toCSharpDataType().Equals("int"))
                    {
                        result += comma + array[i];
                    }
                    else
                    {
                        result += comma + "new " + r.data_type + "()\n";
                    }
                    comma = ",\n ";

                    i++;
                }
            }
            result += "\n);\n";
            i = 0;
            //test each variable
            foreach (Column r in columns)
            {
                if (r.primary_key == 'y' || r.primary_key == 'Y' || r.unique == 'y' || r.unique == 'Y')
                {

                    if (r.data_type.toCSharpDataType().Equals("string"))
                    {
                        result += "Assertions.assertEquals(\"" + array[i] + "\",_" + name.ToLower() + ".get" + r.column_name + "());\n";
                    }
                    else if (r.data_type.toCSharpDataType().Equals("bool"))
                    {
                        result += "Assertions.assertTrue(_" + name.ToLower() + ".get" + r.column_name + "());\n";
                    }
                    else if (r.data_type.toCSharpDataType().Equals("int"))
                    {
                        result += "Assertions.assertEquals(" + array[i] + ",_" + name.ToLower() + ".get" + r.column_name + "());\n";
                    }
                    else
                    {
                        result += "Assertions.assertEquals(new " + r.data_type + "(),_" + name.ToLower() + ".get" + r.column_name + "());\n";
                    }
                    i++;
                }
                else
                {
                    if (r.data_type.toCSharpDataType().Equals("string"))
                    {
                        result += "Assertions.assertNull(_" + name.ToLower() + ".get" + r.column_name + "());\n";
                    }
                    else if (r.data_type.toCSharpDataType().Equals("bool"))
                    {
                        result += "Assertions.assertFalse(_" + name.ToLower() + ".get" + r.column_name + "());\n";
                    }
                    else if (r.data_type.toCSharpDataType().Equals("int"))
                    {
                        result += "Assertions.assertNull(_" + name.ToLower() + ".get" + r.column_name + "());\n";
                    }
                    else
                    {
                        result += "Assertions.assertNull(_" + name.ToLower() + ".get" + r.column_name + "());\n";
                    }
                }
            }
            result += "}\n";
            return result;

        }
        private string testJavaVMParameterizedConstructors()
        {
            string result = "";
            bool hasParent = false;
            bool hasChild = false;
            foreach (Column r in columns)
            {
                if (r.references != "")
                {
                    hasParent = true;

                }
            }
            foreach (foreignKey key in data_tables.all_foreignKey)
            {
                if (key.referenceTable.ToLower().Equals(name.ToLower()))
                {
                    hasChild = true;
                }
            }

            for (int j = 0; j < 3; j++)
            {

                //testing can set as super

                //generate random values for each
                ArrayList array = new ArrayList(columns.Count + 1);
                bool hasdate = false;

                foreach (Column r in columns)
                {
                    if (r.column_name.ToLower().Contains("date"))
                    {
                        hasdate = true;
                        break;
                    }
                }

                foreach (Column r in columns)
                {
                    if (r.data_type.toCSharpDataType().Equals("string"))
                    {
                        if (r.default_value.ToLower().Contains("uuid")) {
                             array.Add(generateRandomString(r, 0));
                        }
                        else
                        {
                             array.Add(generateRandomString(r, -2));

                        }
                       
                        _ = Task.Delay(1);
                    }
                    else if (r.data_type.toCSharpDataType().Equals("bool"))
                    {
                        _ = array.Add(true);
                    }
                    else if (r.data_type.toCSharpDataType().Equals("int"))
                    {
                        _ = array.Add(rand.Next(0, 10000));
                        _ = Task.Delay(1);
                    }
                    else if (r.data_type.Equals("decimal"))
                    {
                        double toAdd = rand.Next(0, 10000) / 100.0;
                        _ = array.Add(toAdd);
                        _ = Task.Delay(1);
                    }
                    else if (r.data_type.ToLower().Contains("date"))
                    {
                        int year = rand.Next(2015, 2027);
                        int month = rand.Next(1, 11);
                        int day = rand.Next(1, 28);
                        int hour = rand.Next(1, 23);
                        int minute = rand.Next(1, 55);
                        int second = rand.Next(2, 45);
                        DateTime toAdd = new DateTime(year, month, day, hour, minute, second);
                        _ = array.Add(toAdd);
                        _ = Task.Delay(1);
                    }
                    else
                    {
                        _ = array.Add(null);
                    }
                    _ = Task.Delay(5);
                }
                //method signature
                result += "\n";
                result += commentBox.genJavaTestJavaDoc(JavaTestType.ParamartizedConstructor, this);
                result += "@Test\n";
                switch (j)
                {

                    case 0:
                        result += "public void testSuper" + name + "ParameterizedVMConstructorSetsAllVariables()";
                        break;
                    case 1:
                        if (hasParent)
                        {
                            result += "public void testSuper" + name + "ParameterizedVMConstructorSetsAllVariablesAndParent()";
                        }
                        break;
                    case 2:
                        if (hasChild)
                        {
                            result += "public void testSuper" + name + "ParameterizedVMConstructorSetsAllVariablesAndChildren()";
                        }
                        break;

                }
                if (hasdate)
                {
                    result += " throws ParseException ";
                }
                result += "{\n";
                if (hasdate)
                {
                    DateTime today = DateTime.Today;
                    result += "String strDate = \"" + today.Day + "/" + today.Month + "/" + today.Year + "\";\n";
                    result += "DateFormat df = new SimpleDateFormat(\"dd/MM/yyyy\");\n";
                    result += "Date date = df.parse(strDate);\n";
                }
                //constructor
                result += name + " _" + name.ToLower() + "= new " + name + "(\n";
                //assing a value to each variable
                string comma = "";
                int i = 0;
                foreach (Column r in columns)
                {

                    if (r.data_type.toCSharpDataType().Equals("string"))
                    {
                        result += comma + "\"" + array[i] + "\"";
                    }
                    else if (r.data_type.toCSharpDataType().Equals("bool"))
                    {
                        result += comma + array[i];
                    }
                    else if (r.data_type.toCSharpDataType().Equals("int"))
                    {
                        result += comma + array[i];
                    }
                    else if (r.data_type.Equals("decimal"))
                    {
                        result += comma + array[i];
                    }
                    else if (r.data_type.ToLower().Contains("date"))
                    {
                        result += comma + "df.parse(\"" + array[i] + "\")";
                    }
                    else
                    {
                        result += comma + "new " + r.data_type + "()";
                    }
                    comma = ",\n ";

                    i++;
                }
                result += "\n);\n";
                switch (j)
                {
                    case 0:
                        result += "_" + name.ToLower() + "VM = new " + name + "_VM(_" + name.ToLower() + ");\n";
                        break;
                    case 1:
                        foreach (Column r in columns)
                        {
                            if (r.references != "")
                            {
                                string[] parts = r.references.Split('.');
                                string fk_table = parts[0];
                                string fk_name = parts[1];
                                result += fk_table + " _" + fk_table.ToLower() + " = new " + fk_table + "();\n";
                            }
                        }
                        result += "_" + name.ToLower() + "VM = new " + name + "_VM(_" + name.ToLower();
                        foreach (Column r in columns)
                        {
                            if (r.references != "")
                            {
                                string[] parts = r.references.Split('.');
                                string fk_table = parts[0];
                                string fk_name = parts[1];
                                result += ", _" + fk_table.ToLower();
                            }
                        }
                        result += ");\n";
                        break;
                    case 2:
                        foreach (foreignKey key in data_tables.all_foreignKey)
                        {

                            if (key.referenceTable.ToLower().Equals(name.ToLower()))
                            {

                                string fk_table = key.mainTable;

                                result += "List<" + fk_table + "> _" + fk_table.ToLower() + "s = new ArrayList<>();\n";
                            }
                        }
                        result += "_" + name.ToLower() + "VM = new " + name + "_VM(_" + name.ToLower();
                        foreach (foreignKey key in data_tables.all_foreignKey)
                        {

                            if (key.referenceTable.ToLower().Equals(name.ToLower()))
                            {

                                string fk_table = key.mainTable;

                                result += ", " + fk_table.ToLower() + "s";
                            }
                        }
                        result += ");\n";
                        break;
                }
                i = 0;
                //test each variable
                foreach (Column r in columns)
                {
                    if (r.data_type.toCSharpDataType().Equals("string"))
                    {
                        result += "Assertions.assertEquals(\"" + array[i] + "\",_" + name.ToLower() + "VM.get" + r.column_name + "());\n";
                    }
                    else if (r.data_type.toCSharpDataType().Equals("bool"))
                    {
                        result += "Assertions.assertTrue(_" + name.ToLower() + "VM.get" + r.column_name + "());\n";
                    }
                    else if (r.data_type.toCSharpDataType().Equals("int"))
                    {
                        result += "Assertions.assertEquals(" + array[i] + ",_" + name.ToLower() + "VM.get" + r.column_name + "());\n";
                    }
                    else if (r.data_type.Equals("decimal"))
                    {
                        result += "Assertions.assertEquals(" + array[i] + ",_" + name.ToLower() + "VM.get" + r.column_name + "());\n";
                    }
                    else if (r.data_type.ToLower().Contains("date"))
                    {
                        result += "Assertions.assertEquals(df.parse(\"" + array[i] + "\"),_" + name.ToLower() + "VM.get" + r.column_name + "());\n";
                    }

                    else
                    {
                        result += "Assertions.assertEquals(new " + r.data_type + "(),_" + name.ToLower() + "VM.get" + r.column_name + "());\n";
                    }
                    i++;
                }
                switch (j)
                {
                    case 0: break;
                    case 1:
                        foreach (Column k in columns)
                        {
                            if (k.references != "")
                            {
                                string[] parts = k.references.Split('.');
                                string fk_table = parts[0];
                                string fk_name = parts[1];
                                result += "Assertions.assertEquals(_" + fk_table.ToLower() + " ,_" + name.ToLower() + "VM.get" + fk_table + "());\n";

                            }
                        }
                        break;
                    case 2:
                        foreach (foreignKey key in data_tables.all_foreignKey)
                        {
                            if (key.referenceTable.ToLower().Equals(name.ToLower()))
                            {

                                string fk_table = key.mainTable;

                                result += "Assertions.assertEquals(_" + fk_table.ToLower() + "s,_" + name.ToLower() + "VM.get" + fk_table + "s());\n";

                            }
                        }
                        break;
                }

                result += "}\n";

            }

            return result;

        }
        private string testJavaTooShort(Column r)
        {
            string result = commentBox.genJavaTestJavaDoc(JavaTestType.SetterThrowsException, this,r);
            result += "@Test\n";
            result += "public void  test" + name + "ThrowsIllegalArgumentExceptionIf" + r.column_name + "TooShort(){\n";
            result += "String " + r.column_name + " = \"";
            String dummy = generateRandomString(r, 2 - r.length);
            result += dummy; ;
            result += "\";\n";
            result += "Assertions.assertThrows(IllegalArgumentException.class, () -> {_" + name.ToLower() + ".set" + r.column_name + "(" + r.column_name + ");});\n";
            result += "}\n";
            return result;

        }
        private string testJavaTooLong(Column r)
        {

            string result = commentBox.genJavaTestJavaDoc(JavaTestType.SetterThrowsException, this,r);
            result += "@Test\n";
            result += "public void  test" + name + "ThrowsIllegalArgumentExceptionIf" + r.column_name + "TooLong(){\n";
            result += "String " + r.column_name + " = \"";
            String dummy = generateRandomString(r, +2);
            result += dummy;
            result += "\";\n";
            result += "Assertions.assertThrows(IllegalArgumentException.class, () -> {_" + name.ToLower() + ".set" + r.column_name + "(" + r.column_name + ");});\n";
            result += "}\n";
            return result;

        }
        private string testJavaIntTooBig(Column r)
        {
            string result = commentBox.genJavaTestJavaDoc(JavaTestType.SetterThrowsException, this, r);
            result += "@Test\n";
            result += "public void test" + name + "ThrowsIllegalArgumentExceptionIf" + r.column_name + "TooBig(){\n";
            result += "int " + r.column_name + " = 10001;\n";
            result += "Assertions.assertThrows(IllegalArgumentException.class, () -> {_" + name.ToLower() + ".set" + r.column_name + "(" + r.column_name + ");});\n";
            result += "}\n";
            return result;

        }
        private string testJavaIntTooSmall(Column r)
        {
            string result = commentBox.genJavaTestJavaDoc(JavaTestType.SetterThrowsException, this, r);
            result += "@Test\n";
            result += "public void test" + name + "ThrowsIllegalArgumentExceptionIf" + r.column_name + "TooSmall(){\n";
            result += "int " + r.column_name + " = -1;\n";
            result += "Assertions.assertThrows(IllegalArgumentException.class, () -> {_" + name.ToLower() + ".set" + r.column_name + "(" + r.column_name + ");});\n";
            result += "}\n";
            return result;
        }
        private string testJavaIntSet(Column r)
        {

            int numberToTest = rand.Next(1, 10000);
            string result = commentBox.genJavaTestJavaDoc(JavaTestType.SetterWorks, this,r);
            result += "@Test\n";
            result += "public void test" + name + "Sets" + r.column_name + "(){\n";
            result += "int " + r.column_name + " = " + numberToTest + ";\n";
            result += "_" + name.ToLower() + ".set" + r.column_name + "(" + r.column_name + ");\n";
            result += "Assertions.assertEquals(" + r.column_name + ", _" + name.ToLower() + ".get" + r.column_name + "());\n";
            result += "}\n";

            return result;

        }
        private string testJavaDecimalTooBig(Column r)
        {
            string result = commentBox.genJavaTestJavaDoc(JavaTestType.SetterThrowsException, this, r);
            result += "@Test\n";
            result += "public void test" + name + "ThrowsIllegalArgumentExceptionIf" + r.column_name + "TooBig(){\n";
            result += "double " + r.column_name + " = 10001;\n";
            result += "Assertions.assertThrows(IllegalArgumentException.class, () -> {_" + name.ToLower() + ".set" + r.column_name + "(" + r.column_name + ");});\n";
            result += "}\n";
            return result;

        }
        private string testJavaDecimalTooSmall(Column r)
        {
            string result = commentBox.genJavaTestJavaDoc(JavaTestType.SetterThrowsException, this,r);
            result += "@Test\n";
            result += "public void test" + name + "ThrowsIllegalArgumentExceptionIf" + r.column_name + "TooSmall(){\n";
            result += "double " + r.column_name + " = -1;\n";
            result += "Assertions.assertThrows(IllegalArgumentException.class, () -> {_" + name.ToLower() + ".set" + r.column_name + "(" + r.column_name + ");});\n";
            result += "}\n";
            return result;
        }
        private string testJavaDecimalSet(Column r)
        {

            int numberToTest = rand.Next(1, 10000);
            string result = commentBox.genJavaTestJavaDoc(JavaTestType.SetterWorks, this, r);
            result += "@Test\n";
            result += "public void test" + name + "Sets" + r.column_name + "(){\n";
            result += "double " + r.column_name + " = " + numberToTest + ";\n";
            result += "_" + name.ToLower() + ".set" + r.column_name + "(" + r.column_name + ");\n";
            result += "Assertions.assertEquals(" + r.column_name + ", _" + name.ToLower() + ".get" + r.column_name + "());\n";
            result += "}\n";

            return result;

        }
        private string testJavaDatetimeTooBig(Column r)
        {
            string result = commentBox.genJavaTestJavaDoc(JavaTestType.SetterThrowsException, this,r);
            result += "@Test\n";
            result += "public void test" + name + "ThrowsIllegalArgumentExceptionIf" + r.column_name + "TooBig() throws ParseException{\n";
            result += "String strDate = \"01/01/2190\";\n";
            result += "DateFormat df = new SimpleDateFormat(\"dd/MM/yyyy\");\n";
            result += "Date date = df.parse(strDate);\n";
            result += "Assertions.assertThrows(IllegalArgumentException.class, () -> {_" + name.ToLower() + ".set" + r.column_name + "(date);});\n";
            result += "}\n";
            return result;

        }
        private string testJavaDatetimeTooSmall(Column r)
        {
            string result = commentBox.genJavaTestJavaDoc(JavaTestType.SetterThrowsException, this,r);
            result += "@Test\n";
            result += "public void test" + name + "ThrowsIllegalArgumentExceptionIf" + r.column_name + "TooSmall() throws ParseException{\n";
            result += "String strDate = \"03/03/1990\";\n";
            result += "DateFormat df = new SimpleDateFormat(\"dd/MM/yyyy\");\n";
            result += "Date date = df.parse(strDate);\n";

            result += "Assertions.assertThrows(IllegalArgumentException.class, () -> {_" + name.ToLower() + ".set" + r.column_name + "(date);});\n";
            result += "}\n";
            return result;
        }
        private string testJavaDatetimeSet(Column r)
        {
            _ = rand.Next(1, 10000);
            string result = commentBox.genJavaTestJavaDoc(JavaTestType.SetterWorks, this,r);
            result += "@Test\n";
            result += "public void test" + name + "Sets" + r.column_name + "() throws ParseException{\n";
            DateTime today = DateTime.Today;
            result += "String strDate = \"" + today.Day + "/" + today.Month + "/" + today.Year + "\";\n";
            result += "DateFormat df = new SimpleDateFormat(\"dd/MM/yyyy\");\n";
            result += "Date date = df.parse(strDate);\n";
            result += "_" + name.ToLower() + ".set" + r.column_name + "(date);\n";
            result += "Assertions.assertEquals(date, _" + name.ToLower() + ".get" + r.column_name + "());\n";
            result += "}\n";

            return result;

        }
        private string testJavaStringSet(Column r)
        {

            String dummy = "";

            if (r.default_value.ToLower().Contains("uuid"))
            {
                dummy = generateRandomString(r, 0);
            }
            else {
                dummy = generateRandomString(r, -2);
            }

            string result = commentBox.genJavaTestJavaDoc(JavaTestType.SetterWorks, this,r);
            result += "@Test\n";
            result += "public void testSet" + r.column_name + "Sets" + r.column_name + "(){\n";
            result += "String " + r.column_name + " = \"" + dummy + "\";\n";
            result += "_" + name.ToLower() + ".set" + r.column_name + "(" + r.column_name + ");\n";
            result += "Assertions.assertEquals(" + r.column_name + ",_" + name.ToLower() + ".get" + r.column_name + "());\n";
            result += "}\n";
            return result;

        }
        private string testJavaBoolSetFalse(Column r)
        {
            string result = commentBox.genJavaTestJavaDoc(JavaTestType.SetterWorks, this,r);
            result += "@Test\n";
            result += "public void test" + name + "Sets" + r.column_name + "asFalse(){\n";
            result += "boolean status = false;\n";
            result += "_" + name.ToLower() + ".set" + r.column_name + "(status);\n";
            result += "Assertions.assertEquals(status, _" + name.ToLower() + ".get" + r.column_name + "());\n";
            result += "}\n";
            return result;

        }
        private string testJavaBoolSetTrue(Column r)
        {
            string result = commentBox.genJavaTestJavaDoc(JavaTestType.SetterWorks, this, r);
            result += "@Test\n";
            result += "public void test" + name + "Sets" + r.column_name + "asTrue(){\n";
            result += "boolean status = true;\n";
            result += "_" + name.ToLower() + ".set" + r.column_name + "(status);\n";
            result += "Assertions.assertEquals(status, _" + name.ToLower() + ".get" + r.column_name + "());\n";
            result += "}\n";
            return result;

        }

        private string testJavaObjectSet(string objectname)
        {

            string result = "@Test\n";
            result += "public void testSet" + objectname + "Sets" + objectname + "(){\n";
            result += objectname + " _" + objectname.ToLower() + " = new " + objectname + "();\n";
            result += "_" + name.ToLower() + "VM.set" + objectname + "(_" + objectname.ToLower() + ");\n";
            result += "Assertions.assertEquals(_" + objectname.ToLower() + ",_" + name.ToLower() + "VM.get" + objectname + "());\n";
            result += "}\n";
            return result;

        }
        private string testJavaListObjectSet(string objectname)
        {

            string result = "@Test\n";
            result += "public void testSet" + objectname + "sSets" + objectname + "s(){\n";
            result += "List<" + objectname + "> _" + objectname.ToLower() + "s = new ArrayList<" + objectname + ">();\n";
            result += "_" + name.ToLower() + "VM.set" + objectname + "s(_" + objectname.ToLower() + "s);\n";
            result += "Assertions.assertEquals(_" + objectname.ToLower() + "s,_" + name.ToLower() + "VM.get" + objectname + "s());\n";
            result += "}\n";
            return result;

        }
        private string testJavaCompareTo()
        {
            string result = "";
            bool hasDateTime = false;
            foreach (Column r in columns)
            {
                if (r.data_type.ToLower().Contains("date"))
                {
                    hasDateTime = true;
                    break;
                }
            }
            result += "\n";
            result += commentBox.genJavaTestJavaDoc(JavaTestType.CompareTo, this);
            result += "@Test\n";
            result += "public void testCompareToCanCompareForEachDateField()";
            if (hasDateTime)
            {
                result += "throws ParseException";
            }
            result += " {\n";

            if (hasDateTime)
            {
                result += "DateFormat df = new SimpleDateFormat(\"dd/MM/yyyy\");\n";
            }
            result += name + " smaller = new " + name + "();\n";
            result += name + " bigger = new " + name + "();\n";
            String smaller = "";
            String bigger = "";
            foreach (Column r in columns)
            {

                if (r.data_type.toCSharpDataType().Equals("string"))
                {
                    smaller = "\"aaaa\"";
                    bigger = "\"bbbb\"";
                }
                if (r.data_type.toCSharpDataType().Equals("int"))
                {
                    smaller = "10";
                    bigger = "20";
                }
                if (r.data_type.Equals("decimal"))
                {
                    smaller = "10.23d";
                    bigger = "14.12d";
                }
                if (r.data_type.toCSharpDataType().Equals("bool"))
                {
                    smaller = "false";
                    bigger = "true";
                }
                if (r.data_type.ToLower().Contains("date"))
                {
                    smaller = "df.parse(\"01/01/2023\")";
                    bigger = "df.parse(\"01/01/2024\")";
                }
                result += "//to compare a smaller and larger " + r.column_name + "\n";
                result += "smaller.set" + r.column_name + "(" + smaller + ");\n";
                result += "bigger.set" + r.column_name + "(" + bigger + ");\n";
                result += "Assertions.assertTrue(smaller.compareTo(bigger)<0);\n";
                result += "Assertions.assertTrue(bigger.compareTo(smaller)>0);\n";
                result += "//to set the " + r.column_name + " as equal.\n";
                result += "smaller.set" + r.column_name + "(" + bigger + ");\n";
            }
            result += "Assertions.assertTrue(bigger.compareTo(smaller)==0);\n";
            result += "}\n";

            return result;

        }
        private string generateRandomString(Column r, int reletive_length)
        {

            char letter;
            String dummy = "";
            for (int i = 0; i < r.length + reletive_length; i++)
            {
                int randValue;
                while (true)
                {
                    randValue = rand.Next(65, 122);
                    if (randValue < 91 || randValue > 96)
                    {
                        break;
                    }
                }

                // Generating random character by converting 
                // the random number into character. 
                letter = Convert.ToChar(randValue);
                dummy += letter;

            }
            dummy += "";
            return dummy;
        }

        public string genJavaDataAccessFakes()
        {
            string result = "";
            result += genJavaDAOFakeHeader(settings.database_name);   //done, needs javadoccomment
            result += genJavaDAOFakeCreate();      //done, needs javadoccomment
            result += genJavaDAOFakeAddBatch();
            result += genJavaDAOFakeretrieveByKey(); // done, needs javadoccomment
            result += genJavaDAOFakeretrieveAll();  //done, needs javadoccomment
            result += genJavaDAOFakeRetriveActive(); //done, needs javadoccomment
            result += genJavaDAOFakeretrieveDistinct(); //done, needs javadoccomment
            result += genJavaDAOFakeRetriveByFK();//not done, needs javadoccomment
            result += genJavaDAOFakeUpdate(); // done, needs javadoccomment
            result += genJavaDAOFakeDelete(); // done, needs javadoccomment
            result += genJavaDAOFakeUnDelete();// done, needs javadoccomment
            result += genJavaDAOFakeDeactivate(); // done, needs javadoccomment
            result += genJavaDAOFakeCount(); // done, needs javadoccomment
            result += genJavaDAOFakeFileWrite();
            result += genJavaDAOFakeFileRead();
            result += genJavaDAODuplicateKey(); // not done
            result += genJavaDAOExceptionKey();
            result += genJavaDAOFooter(); // done, needs javadoccomment          
            return result;

        }

        public string genCSharpDataAccessFakes()
        {
            string result = "";
            result += genCSharpAccessorFakeHeader(settings.database_name);  //done
            result += genCSharpAccessorFakeCreate();      //done
            result += genCSharpAccessorFakeAddBatch();// done
            result += genCSharpAccessorFakeretrieveByKey();// done
            result += genCSharpAccessorFakeRetriveActive(); // done
            result += genCSharpAccessorFakeretrieveAll();// done
            result += genCSharpAccessorFakeretrieveDistinct(); //done
            result += genCSharpAccessorFakeRetriveByFK();// done
            result += genCSharpAccessorFakeUpdate(); // done
            result += genCSharpAccessorFakeDelete(); // done
            result += genCSharpAccessorFakeUnDelete();// done
            result += genCSharpAccessorFakeDeactivate(); // done
            result += genCSharpAccessorFakeCount(); // done
            result += genCSharpAccessorFakeFileWrite();// done
            result += genCSharpAccessorFakeFileRead();// done
            result += genCSharpAccessorDuplicateKey(); // done
            result += genCSharpAccessorExceptionKey();// done
            result += genCSharpAccessorFooter();          // done
            return result;

        }

        private string genCSharpAccessorFakeHeader(string databasename) {
            int x = 0;
            string result = "";
            int numberOfFakes = rand.Next(4, 6);
            hasVM = false;
            foreach (Column r in columns)
            {
                if (!r.references.Equals(""))
                {
                    hasVM = true;
                    break;
                }
            }

            result += usingStatements(name, databasename);
            result += "\npublic class " + name + "AccessorFake : i" + name + "Accessor\n{\n";
            if (hasVM)
            {
                result += "private  List<" + name + "_VM> " + name.ToLower() + "VMs;\n";
            }
            else
            {
                result += "private  List<" + name + "> " + name.ToLower() + "s;\n";
            }
            result += "public " + name + "AccessorFake(){\n";
            if (hasVM)
            {
                result += name.ToLower() + "VMs = new List<"+name+">();\n";
            }
            else
            {
                result += name.ToLower() + "s = new List<"+name+">();\n";
            }

            //generate non keyed data
            if (!hasVM)
            {
                for (int i = 0; i < numberOfFakes; i++)
                {
                    result += name + " " + name.ToLower() + x.ToString() + " = new " + name + "(";
                    string comma = "";
                    foreach (Column r in columns)
                    {
                        if (r.data_type.toCSharpDataType().Equals("string"))
                        {
                            int reletiveLength = 0;
                            if (r.default_value.ToLower().Contains("uuid"))
                            {
                                reletiveLength = 0;
                            }
                            else
                            {
                                reletiveLength = 8 - r.length;
                            }
                            string randomtext = "\"" + generateRandomString(r, reletiveLength) + "\"";
                            result += comma + randomtext;
                        }
                        else if (r.data_type.toCSharpDataType().Equals("bool"))
                        {
                            int flip = rand.Next(0, 2);
                            if (flip == 0)
                            {
                                result += comma + "true";
                            }
                            else
                            {
                                result += comma + "false";
                            }
                        }
                        else if (r.data_type.toCSharpDataType().Equals("int"))
                        {
                            result += comma + rand.Next(10, 70);
                        }
                        else if (r.data_type.Equals("decimal"))
                        {
                            double toAdd = rand.Next(1000, 7000) / 100d;
                            result += comma + toAdd.ToString();
                        }
                        else
                        {
                            result += comma + "new " + r.data_type + "()";
                        }
                        comma = ", ";
                    }
                    result += ");\n";
                    x++;
                }
            }
            else
                for (int l = 0; l < 3; l++)
                {
                    {
                        for (int j = 0; j < columns.Count; j++)
                        {
                            if (columns[j].references != "")
                            {
                                if (columns[j].data_type.toCSharpDataType().Equals("string"))
                                {

                                    string randomtext = "\"" + generateRandomString(columns[j], 8 - columns[j].length) + "\"";
                                    if (columns[j].default_value.ToLower().Contains("uuid)"))
                                    {
                                        randomtext = "\"" + generateRandomString(columns[j], 0) + "\"";
                                    }
                                    for (int i = 0; i < numberOfFakes; i++)
                                    {
                                        result += name + " " + name.ToLower() + x.ToString() + " = new " + name + "(";
                                        string comma = "";
                                        for (int k = 0; k < columns.Count; k++)
                                        {
                                            if (columns[k].data_type.toCSharpDataType().Equals("string"))
                                            {
                                                if (j == k)
                                                {
                                                    result += comma + randomtext;
                                                }
                                                else
                                                {
                                                    string newrandomtext = "\"" + generateRandomString(columns[k], 8 - columns[k].length) + "\"";
                                                    if (columns[k].default_value.ToLower().Contains("uuid"))
                                                    {
                                                        newrandomtext = "\"" + generateRandomString(columns[k], 0) + "\"";

                                                    }

                                                    result += comma + newrandomtext;

                                                }
                                            }
                                            else if (columns[k].data_type.toCSharpDataType().Equals("bool"))
                                            {
                                                int flip = rand.Next(0, 2);
                                                if (flip == 0)
                                                {
                                                    result += comma + "true";
                                                }
                                                else
                                                {
                                                    result += comma + "false";
                                                }
                                            }
                                            else if (columns[k].data_type.toCSharpDataType().Equals("int"))
                                            {
                                                result += comma + rand.Next(10, 70);
                                            }
                                            else if (columns[k].data_type.Equals("decimal"))
                                            {
                                                double toAdd = rand.Next(1000, 7000) / 100d;
                                                result += comma + toAdd.ToString();
                                            }
                                            else
                                            {
                                                result += comma + "new " + columns[k].data_type + "()";
                                            }
                                            comma = ", ";
                                        }
                                        result += ");\n";
                                        x++;
                                    }
                                }

                                else if (columns[j].data_type.toCSharpDataType().Equals("int"))
                                {
                                    int randInt = rand.Next(10, 70);
                                    for (int i = 0; i < numberOfFakes; i++)
                                    {
                                        result += name + " " + name.ToLower() + x.ToString() + " = new " + name + "(";
                                        string comma = "";
                                        for (int k = 0; k < columns.Count; k++)
                                        {
                                            if (columns[k].data_type.toCSharpDataType().Equals("string"))
                                            {

                                                string randomtext = "\"" + generateRandomString(columns[k], 8 - columns[k].length) + "\"";
                                                if (columns[k].default_value.ToLower().Contains("uuid"))
                                                {
                                                    randomtext = "\"" + generateRandomString(columns[k], 0) + "\"";

                                                }
                                                result += comma + randomtext;
                                            }
                                            else if (columns[k].data_type.toCSharpDataType().Equals("bool"))
                                            {
                                                int flip = rand.Next(0, 2);
                                                if (flip == 0)
                                                {
                                                    result += comma + "true";
                                                }
                                                else
                                                {
                                                    result += comma + "false";
                                                }
                                            }
                                            else if (columns[k].data_type.toCSharpDataType().Equals("int"))
                                            {
                                                if (k == j)
                                                {
                                                    result += comma + randInt;
                                                }
                                                else { result += comma + rand.Next(10, 70); }
                                            }
                                            else if (columns[k].data_type.Equals("decimal"))
                                            {
                                                double toAdd = rand.Next(1000, 7000) / 100d;
                                                result += comma + toAdd.ToString();
                                            }
                                            else
                                            {
                                                result += comma + "new " + columns[k].data_type + "()";
                                            }
                                            comma = ", ";
                                        }
                                        result += ");\n";
                                        x++;
                                    }
                                }
                            }
                        }
                    }
                }
            numberOfFakes = x;
            //logic for vm goes here
            if (hasVM)
            {
                for (int i = 0; i < numberOfFakes; i++)
                {
                    result += name + "_VM " + name.ToLower() + "_VM" + i.ToString() + "= new " + name + "_VM(" + name.ToLower() + i.ToString() + ");\n";
                }
                for (int i = 0; i < numberOfFakes; i++)
                {
                    result += name.ToLower() + "VMs.Add(" + name.ToLower() + "_VM" + i.ToString() + ");\n";
                }
            }

            else
            {
                for (int i = 0; i < numberOfFakes; i++)
                {
                    result += name.ToLower() + "s.Add(" + name.ToLower() + i.ToString() + ");\n";
                }
            }
            String VM = "";
            if (hasVM)
            {
                VM = "VM";
            }
            result += name.ToLower() + VM + "s.Sort((" + name.ToLower() + "1, " + name.ToLower() + "2) => " + name.ToLower() + "1." + columns[0].column_name + ".CompareTo(" + name.ToLower() + "2." + columns[0].column_name + "));\n";
            result += "}\n";

            return result;

        }
        private string genCSharpAccessorFakeCreate()
        {
            string result = "";
           
            result += "public int add(" + name + " _" + name.ToLower() + ")  {\n";
            result += "if (duplicateKey(_" + name.ToLower() + ")){\n";
            result += "return 0;\n";
            result += "}\n";
            result += "if (exceptionKey(_" + name.ToLower() + ")){\n";
            result += "throw new Exception(\"error\");\n";
            result += "}\n";

            if (hasVM)
            {
                result += "int size = " + name.ToLower() + "VMs.Count;\n";
            }
            else
            {
                result += "int size = " + name.ToLower() + "s.Count;\n";
            }
            if (hasVM)
            {
                result += name + "_VM " + name.ToLower() + "_VM = new " + name + "_VM(_" + name.ToLower() + ");\n";
                result += name.ToLower() + "s.Add(" + name.ToLower() + "_VM);\n";
            }
            else
            {
                result += name.ToLower() + "s.Add(_" + name.ToLower() + ");\n";
            }
            result += "int newsize = " + name.ToLower() + "s.Count;\n";
            result += "return newsize-size;\n";
            result += "}\n";

            return result;

        }
        private string genCSharpAccessorFakeAddBatch()
        {
            string result = "public int addBatchOf" + name + "s(List<" + name + "> _" + name.ToLower() + "s){\n";
            result += "int result = 0;\n";
            result += "foreach (" + name + " " + name.ToLower() + " in _" + name.ToLower() + "s) {\n";
            result += name.ToLower() + "VMs.Add(" + name.ToLower() + ");\n";
            result += "result ++;\n";
            result += "}\n";
            result += "return result;\n";
            result += "}\n";

            return result;

        }
        private string genCSharpAccessorFakeretrieveByKey()
        {
            string result = "";
            string type;
            if (hasVM)
            {
                type = name + "_VM";
            }
            else
            {
                type = name;
            }
            

            result += "public " + type + " get" + name + "ByPrimaryKey(" + type + " _" + name.ToLower() + ") {\n";
            result += type + " result = null;\n";
            result += "foreach (" + type + " " + name.ToLower() + " in " + name.ToLower() + "s) {\n";
            result += "if (";
            string andand = "";
            foreach (Column r in columns)
            {
                if (r.primary_key.Equals('y') || r.primary_key.Equals('Y'))
                {
                    result += andand + name.ToLower() + "." + r.column_name + ".Equals(_" + name.ToLower() + "." + r.column_name + ")";
                    andand = "&&";
                }
            }
            result += "){\n";
            result += "result = " + name.ToLower() + ";\n";
            result += "break;\n}\n";
            result += "}\n";
            result += "if (result == null){\n";
            result += "throw new Exception(\"" + name + " not found\");\n";
            result += "}\n";
            result += "return result;\n";
            result += "}\n";
            return result;

        }
        private string genCSharpAccessorFakeRetriveActive()
        {
            string vmTag = "";
            if (hasVM) { vmTag = "_VM"; }
            string result = "\n";

            result += "public List<" + name + vmTag + "> getActive" + name + "() {\n";
            result += "List<" + name + vmTag + "> results = new List<"+name+">();\n";
            result += "foreach (" + name + vmTag + " " + name.ToLower() + " in " + name.ToLower() + vmTag + "s){\n";
            result += "if (" + name.ToLower() + ".Is_Active){\n";
            result += "results.Add(" + name.ToLower() + ");\n";
            result += "}\n}\n";
            result += "return results;\n}\n";
            return result;

        }

        private string genCSharpAccessorFakeretrieveAll() {
            string result = "";
            
            if (hasVM)
            {
                result += "public List <" + name + "_VM> getAll" + name + "(int limit, int offset, string search_term";
                foreach (Column r in columns)
                {
                    if (r.references != "")
                    {
                        result += ", " + r.data_type.toCSharpDataType() + " " + r.column_name;
                    }
                }

                result += ")  {\n";
                result += "List<" + name + "_VM> results = new List<"+name+">();\n";
                result += "foreach (" + name + "_VM " + name.ToLower() + " in " + name.ToLower() + "VMs){\n";
                result += "if (";
                string andand = "";
                foreach (Column r in columns)
                {
                    if (r.references != "")
                    {
                        result += andand + "(" + name.ToLower() + "." + r.column_name + "!=null||" + name.ToLower() + "." + r.column_name + ".Equals(" + r.column_name + "))\n";
                        andand = "&&";
                    }
                }

                result += "){\n";
                result += "if (search_term.Equals(\"\") ";
                foreach (Column r in columns)
                {
                    result += "|| " + name.ToLower() + "." + r.column_name + ".Contains(search_term)";
                }
                result += "){\n";
                result += "results.Add(" + name.ToLower() + ");\n";
                result += "}\n}\n}\n";
                result += "return results;\n}\n";

            }
            else
            {
                result += "public List <" + name + "> getAll" + name + "(int limit, int offset, String search_term)  {\n";
                result += "List<" + name + "> results = new List<"+name+">();\n";
                result += "foreach (" + name + " " + name.ToLower() + " in " + name.ToLower() + "s){\n";
                result += "if (search_term.Equals(\"\") ";
                foreach (Column r in columns)
                {
                    result += "|| " + name.ToLower() + "." + r.column_name + ".Contains(search_term)";
                }
                result += "){\n";
                result += "results.Add(" + name.ToLower() + ");\n";
                result += "}\n}\n";

                result += "return results;\n";
                result += "}\n";
            }

            return result;

        }
        private string genCSharpAccessorFakeretrieveDistinct()
        {
            string vmTag = "";
            if (hasVM)
            {
                vmTag = "_VM";
            }
            string result = "\n";
            bool stringKey = false;
            foreach (Column r in columns)
            {
                if (r.primary_key.Equals('y') || r.primary_key.Equals('Y'))
                {
                    if (r.data_type.toCSharpDataType().ToLower().Equals("string"))
                    {
                        stringKey = true;
                    }
                }
            }
            if (stringKey)
            {
                result += "public List<string> getDistinct" + name + "ForDropdown() {\n";
                result += "List<string> results = new List<string>();\n";
                result += "foreach (" + name + vmTag + " " + name.ToLower() + " in " + name.ToLower() + vmTag.Replace("_", "") + "s){\n";
                result += "results.Add(" + name.ToLower() + "." + columns[0].column_name + ");\n";
                result += "}\n";
                result += "return results;\n}\n";
            }
            else
            {
                result += "public List<" + name + "> getDistinct" + name + "ForDropdown() {\n";
                result += "List<" + name + "> results = new List<"+name+">();\n";
                result += "foreach (" + name + vmTag + " " + name.ToLower() + " in " + name.ToLower() + vmTag.Replace("_", "") + "s){\n";
                result += name + " _" + name.ToLower() + " = new " + name + "();\n";
                result += "_" + name.ToLower() + "." + columns[0].column_name + "=" + name.ToLower() + "." + columns[0].column_name + ";\n";
                result += "_" + name.ToLower() + "." + columns[1].column_name + "=" + name.ToLower() + "." + columns[1].column_name + ";\n";
                result += "results.Add(_" + name.ToLower() + ");\n";
                result += "}\n";
                result += "return results;\n}\n";
            }
            return result;

        }
        private string genCSharpAccessorFakeRetriveByFK()
        {
            string result = "";
            foreach (Column r in columns)
            {
                if (r.references != "")
                {
                    string vmTag = "_VM";
                    string[] parts = r.references.Split('.');
                    string fk_table = parts[0];
                    string fk_name = parts[1];
                    result += "\n";
                    result += "public List<" + name + "_VM> get" + name + "by" + fk_table + "(" + r.data_type.toCSharpDataType() + " " + fk_name + "){\n";
                    result += "List<" + name + vmTag + "> results = new List<"+name+">();\n";
                    result += "foreach (" + name + vmTag + " " + name.ToLower() + " in " + name.ToLower() + vmTag.Replace("_", "") + "s){\n";
                    result += "if (" + name.ToLower() + "." + r.column_name + ".Equals(" + fk_name + ")){\n";
                    result += "results.Add(" + name.ToLower() + ");\n";
                    result += "}\n}\n";
                    result += "return results;\n}\n";
                }
            }

            return result;

        }
        private string genCSharpAccessorFakeUpdate()
        {
            string VMTag = "";
            if (hasVM)
            {
                VMTag = "_VM";
            }
            string results = "\n";
            results += "public int update" + "(" + name + " old" + name + ", " + name + " new" + name + ") {\n";

            results += "int location =-1;\n";
            results += "if (duplicateKey(old" + name + ")){\n";
            results += "return 0;\n";
            results += "}\n";
            results += "if (exceptionKey(old" + name + ")){\n";
            results += "throw new Exception(\"error\");\n";
            results += "}\n";
            results += "for (int i=0;i<" + name.ToLower() + VMTag.Replace("_", "") + "s.Count;i++){\n";
            results += "if (";
            string andand = "";
            foreach (Column r in columns)
            {
                if (r.primary_key.Equals('Y') || r.primary_key.Equals('y'))
                {
                    results += andand + name.ToLower() + VMTag.Replace("_", "") + "s[i]." + r.column_name + ".Equals(old" + name + "." + r.column_name + "" + ")){\n";
                    andand = "&&";
                }
            }

            results += "location =i;\n";
            results += "break;\n";
            results += "}\n";
            results += "}\n";
            results += "if (location==-1){\n";
            results += "throw new Exception();\n";
            results += "}\n";
            if (hasVM)
            {
                results += name + "_VM updated = new " + name + "_VM(new" + name + ");\n";
                results += name.ToLower() + VMTag.Replace("_", "") + "s[location]=updated;\n";
            }
            else
            {
                results += name.ToLower() + VMTag.Replace("_", "") + "s[location] = new" + name + ";\n";
            }
            results += "return 1;\n}\n";
            return results;

        }
        private string genCSharpAccessorFakeDelete()
        {
            string results = "\n";
            results += "public int delete" + name + "(";
            string comma = "";
            foreach (Column r in columns)
            {
                if (r.primary_key.Equals('Y') || r.primary_key.Equals('y'))
                {
                    results += comma + r.data_type.toCSharpDataType() + " " + r.column_name;
                }
            }
            results += ") {\n";
            if (hasVM)
            {
                results += "int size = " + name.ToLower() + "VMs.Count;\n";
            }
            else
            {
                results += "int size = " + name.ToLower() + "s.Count;\n";
            }
            results += "int location =-1;\n";
            results += "for (int i=0;i<" + name.ToLower() + "VMs.Count;i++){\n";
            results += "if (";
            string andand = "";
            foreach (Column r in columns)
            {
                if (r.primary_key.Equals('Y') || r.primary_key.Equals('y'))
                {
                    results += andand + name.ToLower() + "VMs[i]." + r.column_name + ".Equals(" + r.column_name + ")";
                    andand = "&&";
                }
            }
            results += "){\n";
            results += "location =i;\n";
            results += "break;\n";
            results += "}\n";
            results += "}\n";
            results += "if (location==-1){\n";
            results += "throw new Exception();\n";
            results += "}\n";
            results += name.ToLower() + "VMs.RemoveAt(location);\n";
            if (hasVM)
            {
                results += "int newsize = " + name.ToLower() + "VMs.Count;\n";
            }
            else
            {
                results += "int newsize = " + name.ToLower() + "s.Count;\n";
            }
            results += "return size-newsize;\n}\n";
            return results;

        }
        private string genCSharpAccessorFakeUnDelete()
        {
            string results = "\n";
            results += "public int undelete" + name + "(";
            string comma = "";
            foreach (Column r in columns)
            {
                if (r.primary_key.Equals('Y') || r.primary_key.Equals('y'))
                {
                    results += comma + r.data_type.toCSharpDataType() + " " + r.column_name;
                }
            }
            results += ") {\n";

            results += "int location =-1;\n";
            results += "for (int i=0;i<" + name.ToLower() + "VMs.Count;i++){\n";
            results += "if (";
            string andand = "";
            foreach (Column r in columns)
            {
                if (r.primary_key.Equals('Y') || r.primary_key.Equals('y'))
                {
                    results += andand + name.ToLower() + "VMs[i]." + r.column_name + ".Equals(" + r.column_name + ")";
                    andand = "&&";
                }
            }
            results += "){\n";
            results += "location =i;\n";
            results += "break;\n";
            results += "}\n";
            results += "}\n";
            results += "if (location==-1){\n";
            results += "throw new Exception(\"Unable To Find " + name + ".\");\n";
            results += "}\n";
            results += "if(!" + name.ToLower() + "VMs[location].Is_Active){\n";
            results += name.ToLower() + "VMs[location].Is_Active=true;\n";
            results += "return 1;\n";
            results += "}\n";
            results += "else {\n";
            results += "return 0;\n";
            results += "}\n";
            results += "}\n";
            return results;

        }
        private string genCSharpAccessorFakeDeactivate()
        {
            string results = "\n";
            results += "public int deactivate" + name + "(";
            string comma = "";
            foreach (Column r in columns)
            {
                if (r.primary_key.Equals('Y') || r.primary_key.Equals('y'))
                {
                    results += comma + r.data_type.toCSharpDataType() + " " + r.column_name;
                }
            }
            results += ") {\n";

            results += "int location =-1;\n";
            results += "for (int i=0;i<" + name.ToLower() + "VMs.Count;i++){\n";
            results += "if (";
            string andand = "";
            foreach (Column r in columns)
            {
                if (r.primary_key.Equals('Y') || r.primary_key.Equals('y'))
                {
                    results += andand + name.ToLower() + "VMs[i]." + r.column_name + ".Equals(" + r.column_name + ")";
                    andand = "&&";
                }
            }
            results += "){\n";
            results += "location =i;\n";
            results += "break;\n";
            results += "}\n";
            results += "}\n";
            results += "if (location==-1){\n";
            results += "throw new Exception(\"Unable To Find " + name + ".\");\n";
            results += "}\n";
            results += "if(" + name.ToLower() + "VMs[location].Is_Active){\n";
            results += name.ToLower() + "VMs[location].Is_Active=false;\n";
            results += "return 1;\n";
            results += "}\n";
            results += "else {\n";
            results += "return 0;\n";
            results += "}\n";
            results += "}\n";
            return results;

        }
        private string genCSharpAccessorFakeCount()
        {
            string result = "\n";
            result += "public int get" + name + "Count(string Search_term";
            foreach (Column r in columns)
            {
                if (r.references != "")
                {
                    result += ", " + r.data_type.toCSharpDataType() + " " + r.column_name;
                }
            }

            result += ") {\n";
            result += "List<" + name + "_VM> results = new List<"+name+">();\n";
            result += "foreach (" + name + "_VM " + name.ToLower() + " in " + name.ToLower() + "VMs){\n";
            result += "if (";
            string andand = "";
            foreach (Column r in columns)
            {
                if (r.references != "")
                {
                    result += andand + "(" + name.ToLower() + "." + r.column_name + "!=null||" + name.ToLower() + "." + r.column_name + ".Equals(" + r.column_name + "))\n";
                    andand = "&&";
                }
            }

            result += "){\n";
            result += "if (Search_term.Equals(\"\") ";
            foreach (Column r in columns)
            {
                result += "|| " + name.ToLower() + "." + r.column_name + ".Contains(Search_term)";
            }
            result += "){\n";
            result += "results.Add(" + name.ToLower() + ");\n";
            result += "}\n}\n}\n";
            result += "return results.Count;\n}\n";
            return result;

        }
        private string genCSharpAccessorFakeFileWrite()
        {
            string result = "public int write" + name + "ToFile(List<" + name + "> _" + name + "s, string path) {\n";
            result += "return _" + name + "s.Count;\n";
            result += "}\n";
            return result;

        }
        private string genCSharpAccessorFakeFileRead()
        {
            string result = "public List<" + name + "> read" + name + "sFromFile(FileStream uploadedFile) {\n";
            result += "return " + name.ToLower() + "VMs;\n";
            result += "}\n";

            return result;

        }
        private string genCSharpAccessorDuplicateKey()
        {
            string result = "";
            result += "private bool duplicateKey(" + name + " _" + name.ToLower() + "){\n";
            foreach (Column r in columns)
            {
                if (r.increment == 0 && r.data_type.toCSharpDataType().Equals("string"))
                {
                    result += "return _" + name.ToLower() + "." + r.column_name + ".Equals(\"DUPLICATE\");\n";
                    result += "}\n";
                    break;
                }
                else
                {
                    continue;
                }
            }
            return result;

        }
        private string genCSharpAccessorExceptionKey()
        {
            string result = "";
            result += "private bool exceptionKey(" + name + " _" + name.ToLower() + "){\n";
            foreach (Column r in columns)
            {
                if (r.increment == 0 && r.data_type.toCSharpDataType().Equals("string"))
                {
                    result += "return _" + name.ToLower() + "." + r.column_name + ".Equals(\"EXCEPTION\");\n";
                    result += "}\n";
                    break;
                }
                else
                {
                    continue;
                }
            }
            return result;

        }
        private string genCSharpAccessorFooter()
        {
            string result = "\n}\n}\n";
            return result;

        }


        private string genJavaDAOFakeHeader(string databasename)
        {
            int x = 0;
            string result = "";
            int numberOfFakes = rand.Next(4, 6);
            hasVM = false;
            foreach (Column r in columns)
            {
                if (!r.references.Equals(""))
                {
                    hasVM = true;
                    break;
                }
            }

            result += importStatements(name, databasename);
            result += "\npublic class " + name + "_DAO_Fake implements i" + name + "DAO{\n";
            if (hasVM)
            {
                result += "private  List<" + name + "_VM> " + name.ToLower() + "VMs;\n";
            }
            else
            {
                result += "private  List<" + name + "> " + name.ToLower() + "s;\n";
            }
            result += "public " + name + "_DAO_Fake(){\n";
            if (hasVM)
            {
                result += name.ToLower() + "VMs = new ArrayList<>();\n";
            }
            else
            {
                result += name.ToLower() + "s = new ArrayList<>();\n";
            }

            //generate non keyed data
            if (!hasVM)
            {
                for (int i = 0; i < numberOfFakes; i++)
                {
                    result += name + " " + name.ToLower() + x.ToString() + " = new " + name + "(";
                    string comma = "";
                    foreach (Column r in columns)
                    {
                        if (r.data_type.toCSharpDataType().Equals("string"))
                        {
                            int reletiveLength = 0;
                            if (r.default_value.ToLower().Contains("uuid"))
                            {
                                reletiveLength = 0;
                            }
                            else {
                                reletiveLength = 8 - r.length;
                            }
                            string randomtext = "\"" + generateRandomString(r, reletiveLength) + "\"";
                            result += comma + randomtext;
                        }
                        else if (r.data_type.toCSharpDataType().Equals("bool"))
                        {
                            int flip = rand.Next(0, 2);
                            if (flip == 0)
                            {
                                result += comma + "true";
                            }
                            else
                            {
                                result += comma + "false";
                            }
                        }
                        else if (r.data_type.toCSharpDataType().Equals("int"))
                        {
                            result += comma + rand.Next(10, 70);
                        }
                        else if (r.data_type.Equals("decimal"))
                        {
                            double toAdd = rand.Next(1000, 7000) / 100d;
                            result += comma + toAdd.ToString();
                        }
                        else
                        {
                            result += comma + "new " + r.data_type + "()";
                        }
                        comma = ", ";
                    }
                    result += ");\n";
                    x++;
                }
            }
            else
                for (int l = 0; l < 3; l++)
                {
                    {
                        for (int j = 0; j < columns.Count; j++)
                        {
                            if (columns[j].references != "")
                            {
                                if (columns[j].data_type.toCSharpDataType().Equals("string"))
                                {
                                    
                                    string randomtext = "\"" + generateRandomString(columns[j], 8 - columns[j].length) + "\"";
                                    if (columns[j].default_value.ToLower().Contains("uuid)")){
                                        randomtext = "\"" + generateRandomString(columns[j], 0) + "\"";
                                    }
                                    for (int i = 0; i < numberOfFakes; i++)
                                    {
                                        result += name + " " + name.ToLower() + x.ToString() + " = new " + name + "(";
                                        string comma = "";
                                        for (int k = 0; k < columns.Count; k++)
                                        {
                                            if (columns[k].data_type.toCSharpDataType().Equals("string"))
                                            {
                                                if (j == k)
                                                {
                                                    result += comma + randomtext;
                                                }
                                                else
                                                {
                                                    string newrandomtext = "\"" + generateRandomString(columns[k], 8 - columns[k].length) + "\"";
                                                    if (columns[k].default_value.ToLower().Contains("uuid"))
                                                    {
                                                         newrandomtext = "\"" + generateRandomString(columns[k], 0) + "\"";

                                                    }

                                                    result += comma + newrandomtext;

                                                }
                                            }
                                            else if (columns[k].data_type.toCSharpDataType().Equals("bool"))
                                            {
                                                int flip = rand.Next(0, 2);
                                                if (flip == 0)
                                                {
                                                    result += comma + "true";
                                                }
                                                else
                                                {
                                                    result += comma + "false";
                                                }
                                            }
                                            else if (columns[k].data_type.toCSharpDataType().Equals("int"))
                                            {
                                                result += comma + rand.Next(10, 70);
                                            }
                                            else if (columns[k].data_type.Equals("decimal"))
                                            {
                                                double toAdd = rand.Next(1000, 7000) / 100d;
                                                result += comma + toAdd.ToString();
                                            }
                                            else
                                            {
                                                result += comma + "new " + columns[k].data_type + "()";
                                            }
                                            comma = ", ";
                                        }
                                        result += ");\n";
                                        x++;
                                    }
                                }

                                else if (columns[j].data_type.toCSharpDataType().Equals("int"))
                                {
                                    int randInt = rand.Next(10, 70);
                                    for (int i = 0; i < numberOfFakes; i++)
                                    {
                                        result += name + " " + name.ToLower() + x.ToString() + " = new " + name + "(";
                                        string comma = "";
                                        for (int k = 0; k < columns.Count; k++)
                                        {
                                            if (columns[k].data_type.toCSharpDataType().Equals("string"))
                                            {

                                                string randomtext = "\"" + generateRandomString(columns[k], 8 - columns[k].length) + "\"";
                                                if (columns[k].default_value.ToLower().Contains("uuid")) {
                                                     randomtext = "\"" + generateRandomString(columns[k], 0) + "\"";

                                                }
                                                result += comma + randomtext;
                                            }
                                            else if (columns[k].data_type.toCSharpDataType().Equals("bool"))
                                            {
                                                int flip = rand.Next(0, 2);
                                                if (flip == 0)
                                                {
                                                    result += comma + "true";
                                                }
                                                else
                                                {
                                                    result += comma + "false";
                                                }
                                            }
                                            else if (columns[k].data_type.toCSharpDataType().Equals("int"))
                                            {
                                                if (k == j)
                                                {
                                                    result += comma + randInt;
                                                }
                                                else { result += comma + rand.Next(10, 70); }
                                            }
                                            else if (columns[k].data_type.Equals("decimal"))
                                            {
                                                double toAdd = rand.Next(1000, 7000) / 100d;
                                                result += comma + toAdd.ToString();
                                            }
                                            else
                                            {
                                                result += comma + "new " + columns[k].data_type + "()";
                                            }
                                            comma = ", ";
                                        }
                                        result += ");\n";
                                        x++;
                                    }
                                }
                            }
                        }
                    }
                }
            numberOfFakes = x;
            //logic for vm goes here
            if (hasVM)
            {
                for (int i = 0; i < numberOfFakes; i++)
                {
                    result += name + "_VM " + name.ToLower() + "_VM" + i.ToString() + "= new " + name + "_VM(" + name.ToLower() + i.ToString() + ");\n";
                }
                for (int i = 0; i < numberOfFakes; i++)
                {
                    result += name.ToLower() + "VMs.add(" + name.ToLower() + "_VM" + i.ToString() + ");\n";
                }
            }

            else
            {
                for (int i = 0; i < numberOfFakes; i++)
                {
                    result += name.ToLower() + "s.add(" + name.ToLower() + i.ToString() + ");\n";
                }
            }
            if (hasVM)
            {
                result += "Collections.sort(" + name.ToLower() + "VMs);\n";
            }
            else
            {
                result += "Collections.sort(" + name.ToLower() + "s);\n";
            }
            result += "}\n";

            return result;
        }

        private string genJavaDAOFakeCreate()
        {
            string result = "";
            result += "@Override\n";
            result += "public int add(" + name + " _" + name.ToLower() + ") throws SQLException {\n";
            result += "if (duplicateKey(_" + name.ToLower() + ")){\n";
            result += "return 0;\n";
            result += "}\n";
            result += "if (exceptionKey(_" + name.ToLower() + ")){\n";
            result += "throw new SQLException(\"error\");\n";
            result += "}\n";

            if (hasVM)
            {
                result += "int size = " + name.ToLower() + "VMs.size();\n";
            }
            else
            {
                result += "int size = " + name.ToLower() + "s.size();\n";
            }
            if (hasVM)
            {
                result += name + "_VM " + name.ToLower() + "_VM = new " + name + "_VM(_" + name.ToLower() + ");\n";
                result += name.ToLower() + "s.add(" + name.ToLower() + "_VM);\n";
            }
            else
            {
                result += name.ToLower() + "s.add(_" + name.ToLower() + ");\n";
            }
            result += "int newsize = " + name.ToLower() + "s.size();\n";
            result += "return newsize-size;\n";
            result += "}\n";

            return result;
        }

        private string genJavaDAOFakeAddBatch() {

            string result = "int addBatchOf" + name + "s(List<" + name + "> _" + name.ToLower() + "s) throws SQLException{\n";
            result += "int result = 0;\n";
            result += "for ("+name+" "+name.ToLower()+" : _" +name.ToLower()+"s) {\n";
            result += name.ToLower()+"VMs.add("+name.ToLower()+");\n";
            result += "result ++;\n";
            result += "}\n";
            result += "return result;\n";
            result += "}\n";

            return result;
        
        }
        private string genJavaDAOFakeretrieveByKey()
        {
            string result = "";
            string type;
            if (hasVM)
            {
                type = name + "_VM";
            }
            else
            {
                type = name;
            }
            result += "@Override\n";

            result += "public " + type + " get" + name + "ByPrimaryKey(" + type + " _" + name.ToLower() + ") throws SQLException{\n";
            result += type + " result = null;\n";
            result += "for (" + type + " " + name.ToLower() + " : " + name.ToLower() + "s) {\n";
            result += "if (";
            string andand = "";
            foreach (Column r in columns)
            {
                if (r.primary_key.Equals('y') || r.primary_key.Equals('Y'))
                {
                    result += andand + name.ToLower() + ".get" + r.column_name + "().equals(_" + name.ToLower() + ".get" + r.column_name + "())";
                    andand = "&&";
                }
            }
            result += "){\n";
            result += "result = " + name.ToLower() + ";\n";
            result += "break;\n}\n";
            result += "}\n";
            result += "if (result == null){\n";
            result += "throw new SQLException(\"" + name + " not found\");\n";
            result += "}\n";
            result += "return result;\n";
            result += "}\n";
            return result;
        }
        private string genJavaDAOFakeretrieveAll()
        {
            string result = "";
            result += "@Override\n";
            if (hasVM)
            {
                result += "public List <" + name + "_VM> getAll" + name + "(int limit, int offset, String search_term";
                foreach (Column r in columns)
                {
                    if (r.references != "")
                    {
                        result += ", " + r.data_type.toJavaDataType() + " " + r.column_name;
                    }
                }

                result += ") throws SQLException {\n";
                result += "List<" + name + "_VM> results = new ArrayList<>();\n";
                result += "for (" + name + "_VM " + name.ToLower() + " : " + name.ToLower() + "VMs){\n";
                result += "if (";
                string andand = "";
                foreach (Column r in columns)
                {
                    if (r.references != "")
                    {
                        result += andand + "(" + name.ToLower() + ".get" + r.column_name + "()!=null||" + name.ToLower() + ".get" + r.column_name + "().equals(" + r.column_name + "))\n";
                        andand = "&&";
                    }
                }

                result += "){\n";
                result += "if (search_term.isEmpty() ";
                foreach (Column r in columns) {
                    result += "|| " + name.ToLower() + ".get" + r.column_name + "().contains(search_term)";
                }
                result += "){\n";
                result += "results.add(" + name.ToLower() + ");\n";
                result += "}\n}\n}\n";
                result += "return results;\n}\n";

            }
            else
            {
                result += "public List <" + name + "> getAll" + name + "(int limit, int offset, String search_term) throws SQLException {\n";
                result += "List<" + name + "> results = new ArrayList<>();\n";
                result += "for (" + name + " " + name.ToLower() + " : " + name.ToLower() + "s){\n";
                result += "if (search_term.isEmpty() ";
                foreach (Column r in columns)
                {
                    result += "|| " + name.ToLower() + ".get" + r.column_name + "().contains(search_term)";
                }
                result += "){\n";
                result += "results.add(" + name.ToLower() + ");\n";
                result += "}\n}\n";

                result += "return results;\n";
                result += "}\n";
            }

            return result;
        }
        private string genJavaDAOFakeRetriveActive()
        {
            string vmTag = "";
            if (hasVM) { vmTag = "_VM"; }
            string result = "@Override\n";

            result += "public List<" + name + vmTag + "> getActive" + name + "() throws SQLException{\n";
            result += "List<" + name + vmTag + "> results = new ArrayList<>();\n";
            result += "for (" + name + vmTag + " " + name.ToLower() + " : " + name.ToLower() + vmTag + "s){\n";
            result += "if (" + name.ToLower() + ".getIs_Active()){\n";
            result += "results.add(" + name.ToLower() + ");\n";
            result += "}\n}\n";
            result += "return results;\n}\n";
            return result;
        }
        private string genJavaDAOFakeretrieveDistinct()
        {
            string vmTag = "";
            if (hasVM)
            {
                vmTag = "_VM";
            }
            string result = "@Override\n";
            bool stringKey = false;
            foreach (Column r in columns)
            {
                if (r.primary_key.Equals('y') || r.primary_key.Equals('Y'))
                {
                    if (r.data_type.toJavaDataType().Equals("String"))
                    {
                        stringKey = true;
                    }
                }
            }
            if (stringKey)
            {
                result += "public List<String> getDistinct" + name + "ForDropdown() throws SQLException{\n";
                result += "List<String> results = new ArrayList<>();\n";
                result += "for (" + name + vmTag + " " + name.ToLower() + " : " + name.ToLower() + vmTag.Replace("_", "") + "s){\n";
                result += "results.add(" + name.ToLower() + ".get" + columns[0].column_name + "());\n";
                result += "}\n";
                result += "return results;\n}\n";
            }
            else
            {
                result += "public List<" + name + "> getDistinct" + name + "ForDropdown() throws SQLException{\n";
                result += "List<" + name + "> results = new ArrayList<>();\n";
                result += "for (" + name + vmTag + " " + name.ToLower() + " : " + name.ToLower() + vmTag.Replace("_", "") + "s){\n";
                result += name + " _" + name.ToLower() + " = new " + name + "();\n";
                result += "_" + name.ToLower() + ".set" + columns[0].column_name + "(" + name.ToLower() + ".get" + columns[0].column_name + "());\n";
                result += "_" + name.ToLower() + ".set" + columns[1].column_name + "(" + name.ToLower() + ".get" + columns[1].column_name + "());\n";
                result += "results.add(_" + name.ToLower() + ");\n";
                result += "}\n";
                result += "return results;\n}\n";
            }
            return result;
        }
        private string genJavaDAOFakeRetriveByFK()
        {
            string result = "";
            foreach (Column r in columns)
            {
                if (r.references != "")
                {
                    string vmTag = "_VM";
                    string[] parts = r.references.Split('.');
                    string fk_table = parts[0];
                    string fk_name = parts[1];
                    result += "@Override\n";
                    result += "public List<" + name + "_VM> get" + name + "by" + fk_table + "(" + r.data_type.toJavaDataType() + " " + fk_name + "){\n";
                    result += "List<" + name + vmTag + "> results = new ArrayList<>();\n";
                    result += "for (" + name + vmTag + " " + name.ToLower() + " : " + name.ToLower() + vmTag.Replace("_", "") + "s){\n";
                    result += "if (" + name.ToLower() + ".get" + r.column_name + "().equals(" + fk_name + ")){\n";
                    result += "results.add(" + name.ToLower() + ");\n";
                    result += "}\n}\n";
                    result += "return results;\n}\n";
                }
            }

            return result;
        }
        private string genJavaDAOFakeUpdate()
        {
            string VMTag = "";
            if (hasVM)
            {
                VMTag = "_VM";
            }
            string results = "@Override\n";
            results += "public int update" + "(" + name + " old" + name + ", " + name + " new" + name + ") throws SQLException{\n";

            results += "int location =-1;\n";
            results += "if (duplicateKey(old" + name + ")){\n";
            results += "return 0;\n";
            results += "}\n";
            results += "if (exceptionKey(old" + name + ")){\n";
            results += "throw new SQLException(\"error\");\n";
            results += "}\n";
            results += "for (int i=0;i<" + name.ToLower() + VMTag.Replace("_", "") + "s.size();i++){\n";
            results += "if (";
            string andand = "";
            foreach (Column r in columns)
            {
                if (r.primary_key.Equals('Y') || r.primary_key.Equals('y'))
                {
                    results += andand + name.ToLower() + VMTag.Replace("_", "") + "s.get(i).get" + r.column_name + "().equals(old" + name + ".get" + r.column_name + "()" + ")){\n";
                    andand = "&&";
                }
            }

            results += "location =i;\n";
            results += "break;\n";
            results += "}\n";
            results += "}\n";
            results += "if (location==-1){\n";
            results += "throw new SQLException();\n";
            results += "}\n";
            if (hasVM)
            {
                results += name + "_VM updated = new " + name + "_VM(new" + name + ");\n";
                results += name.ToLower() + VMTag.Replace("_", "") + "s.set(location,updated);\n";
            }
            else
            {
                results += name.ToLower() + VMTag.Replace("_", "") + "s.set(location,new" + name + ");\n";
            }
            results += "return 1;\n}\n";
            return results;
        }
        private string genJavaDAOFakeDelete()
        {
            string results = "@Override\n";
            results += "public int delete" + name + "(";
            string comma = "";
            foreach (Column r in columns)
            {
                if (r.primary_key.Equals('Y') || r.primary_key.Equals('y'))
                {
                    results += comma + r.data_type.toJavaDataType() + " " + r.column_name;
                }
            }
            results += ") throws SQLException{\n";
            if (hasVM)
            {
                results += "int size = " + name.ToLower() + "VMs.size();\n";
            }
            else
            {
                results += "int size = " + name.ToLower() + "s.size();\n";
            }
            results += "int location =-1;\n";
            results += "for (int i=0;i<" + name.ToLower() + "VMs.size();i++){\n";
            results += "if (";
            string andand = "";
            foreach (Column r in columns)
            {
                if (r.primary_key.Equals('Y') || r.primary_key.Equals('y'))
                {
                    results += andand + name.ToLower() + "VMs.get(i).get" + r.column_name + "().equals(" + r.column_name + ")";
                    andand = "&&";
                }
            }
            results += "){\n";
            results += "location =i;\n";
            results += "break;\n";
            results += "}\n";
            results += "}\n";
            results += "if (location==-1){\n";
            results += "throw new SQLException(\"Unable To Find "+name+".\");\n";
            results += "}\n";
            results += name.ToLower() + "VMs.remove(location);\n";
            if (hasVM)
            {
                results += "int newsize = " + name.ToLower() + "VMs.size();\n";
            }
            else
            {
                results += "int newsize = " + name.ToLower() + "s.size();\n";
            }
            results += "return size-newsize;\n}\n";
            return results;

        }
        private string genJavaDAOFakeUnDelete()
        {
            string results = "@Override\n";
            results += "public int undelete" + name + "(";
            string comma = "";
            foreach (Column r in columns)
            {
                if (r.primary_key.Equals('Y') || r.primary_key.Equals('y'))
                {
                    results += comma + r.data_type.toJavaDataType() + " " + r.column_name;
                }
            }
            results += ") throws SQLException{\n";

            results += "int location =-1;\n";
            results += "for (int i=0;i<" + name.ToLower() + "VMs.size();i++){\n";
            results += "if (";
            string andand = "";
            foreach (Column r in columns)
            {
                if (r.primary_key.Equals('Y') || r.primary_key.Equals('y'))
                {
                    results += andand + name.ToLower() + "VMs.get(i).get" + r.column_name + "().equals(" + r.column_name + ")";
                    andand = "&&";
                }
            }
            results += "){\n";
            results += "location =i;\n";
            results += "break;\n";
            results += "}\n";
            results += "}\n";
            results += "if (location==-1){\n";
            results += "throw new SQLException(\"Unable To Find " + name + ".\");\n";
            results += "}\n";
            results += "if(!" + name.ToLower() + "VMs.get(location).getIs_Active()){\n";
            results += name.ToLower() + "VMs.get(location).setIs_Active(true);\n";
            results += "return 1;\n";
            results += "}\n";
            results += "else {\n";
            results += "return 0;\n";
            results += "}\n";
            results += "}\n";
            return results;
        }

        private string genJavaDAOFakeDeactivate()
        {
            string results = "@Override\n";
            results += "public int deactivate" + name + "(";
            string comma = "";
            foreach (Column r in columns)
            {
                if (r.primary_key.Equals('Y') || r.primary_key.Equals('y'))
                {
                    results += comma + r.data_type.toJavaDataType() + " " + r.column_name;
                }
            }
            results += ") throws SQLException{\n";

            results += "int location =-1;\n";
            results += "for (int i=0;i<" + name.ToLower() + "VMs.size();i++){\n";
            results += "if (";
            string andand = "";
            foreach (Column r in columns)
            {
                if (r.primary_key.Equals('Y') || r.primary_key.Equals('y'))
                {
                    results += andand + name.ToLower() + "VMs.get(i).get" + r.column_name + "().equals(" + r.column_name + ")";
                    andand = "&&";
                }
            }
            results += "){\n";
            results += "location =i;\n";
            results += "break;\n";
            results += "}\n";
            results += "}\n";
            results += "if (location==-1){\n";
            results += "throw new SQLException(\"Unable To Find " + name + ".\");\n";
            results += "}\n";
            results += "if(" + name.ToLower() + "VMs.get(location).getIs_Active()){\n";
            results += name.ToLower() + "VMs.get(location).setIs_Active(false);\n";
            results += "return 1;\n";
            results += "}\n";
            results += "else {\n";
            results += "return 0;\n";
            results += "}\n";
            results += "}\n";
            return results;

        }

        private string genJavaDAOFakeFileWrite() {
            
            string result = "int write" + name + "ToFile(List<" + name + "> _" + name + "s, String path) throws IOException{\n";
            result += "return _" + name + "s.size();\n";
            result += "}\n";
            return result;
        
        }
        private string genJavaDAOFakeFileRead()
        {
            
            string result = "List<" + name + "> read" + name + "sFromFile(File uploadedFile) throws Exception{\n";
            result += "return " + name.ToLower() + "VMs;\n";
            result += "}\n";

            return result;


        }
        private string genJavaDAODuplicateKey()
        {
            string result = "";
            result += "private boolean duplicateKey(" + name + " _" + name.ToLower() + "){\n";
            foreach (Column r in columns)
            {
                if (r.increment == 0 && r.data_type.toCSharpDataType().Equals("string"))
                {
                    result += "return _" + name.ToLower() + ".get" + r.column_name + "().equals(\"DUPLICATE\");\n";
                    result += "}\n";
                    break;
                }
                else
                {
                    continue;
                }
            }
            return result;
        }

        private string genJavaDAOExceptionKey()
        {
            string result = "";
            result += "private boolean exceptionKey(" + name + " _" + name.ToLower() + "){\n";
            foreach (Column r in columns)
            {
                if (r.increment == 0 && r.data_type.toCSharpDataType().Equals("string"))
                {
                    result += "return _" + name.ToLower() + ".get" + r.column_name + "().equals(\"EXCEPTION\");\n";
                    result += "}\n";
                    break;
                }
                else
                {
                    continue;
                }
            }
            return result;
        }

        private string genJavaDAOFakeCount()
        {
            string result = "@Override\n";
            result += "public int get" + name + "Count(String Search_term";
            foreach (Column r in columns)
            {
                if (r.references != "")
                {
                    result += ", " + r.data_type.toJavaDataType() + " " + r.column_name;
                }
            }

            result += ") throws SQLException {\n";
            result += "List<" + name + "_VM> results = new ArrayList<>();\n";
            result += "for (" + name + "_VM " + name.ToLower() + " : " + name.ToLower() + "VMs){\n";
            result += "if (";
            string andand = "";
            foreach (Column r in columns)
            {
                if (r.references != "")
                {
                    result += andand + "(" + name.ToLower() + ".get" + r.column_name + "()!=null||" + name.ToLower() + ".get" + r.column_name + "().equals(" + r.column_name + "))\n";
                    andand = "&&";
                }
            }

            result += "){\n";
            result += "if (Search_term.isEmpty() ";
            foreach (Column r in columns)
            {
                result += "|| " + name.ToLower() + ".get" + r.column_name + "().contains(Search_term)";
            }
            result += "){\n";
            result += "results.add(" + name.ToLower() + ");\n";
            result += "}\n}\n}\n";
            result += "return results.count();\n}\n";
            return result;

        }
        //done
        public string genJavaGetAllServletTests()
        {
            string result = "";
            result += packageStatementForTests();
            result += importStatementForTests();     //done
            result += classNameAndStaticVariables(1); //done
            result += initTests(1); //done
            result += tearDownTests(); //done
            result += testLoggedInGets200OnDoGet();//done
            //result += testLoggedInGets302OnDoPostWithNoObjectSet();//done
            result += TestLoggedOutGets302onDoGet();//done
            //result += TestLoggedOutGets302onDoPost();//done
            result += TestWrongRoleGets302onDoGet(); // done
            //result += TestWrongRoleGets302onDoPost(); //  done
            result += TestGetAllGetsAll();//done
            result += TestGetAllCanFilter(); //done
            result += TestGetAllCanSearch();
            result += TestGetAllCanSearchAndReturnZero();
            result += TestInitWithNoParamsDoesNotCrash();
            
            result += "\n}\n";
            return result;
        }

        //done
        public string genJavaCreateServletTests()
        {
            string result = "";
            result += importStatementForTests(); //done
            result += classNameAndStaticVariables(2); //done
            result += initTests(2); //done
            result += tearDownTests(); //done
            result += testLoggedInGets200OnDoGet(); //done
            result += testLoggedInGets302OnDoPostWithNoObjectSet(); //done
            result += TestLoggedOutGets302onDoGet();//done 
            result += TestLoggedOutGets302onDoPost();//done
            result += TestWrongRoleGets302onDoGet(); // done
            result += TestWrongRoleGets302onDoPost(); //  done
            result += TestAddHasErrorsForEachFieldAndKeepsOnSamePage(); //done
            result += TestAddCanAddWithNoErrorsAndRedirects();
            result += testExceptionKeyThrowsException();  //exceptionkey
            result += testDuplicateKeyAddsZero(); //duplicateKey
            result += TestInitWithNoParamsDoesNotCrash();
            result += "\n}\n";//done
            return result;
        }
        //done
        public string genJavaDeleteServletTests()
        {
            string result = "";
            result += importStatementForTests(); //done
            result += classNameAndStaticVariables(3);//done 
            result += initTests(3);//done 
            result += tearDownTests();//done
            result += testLoggedInGets200OnDoGet();//done
            //result += testLoggedInGets302OnDoPostWithNoObjectSet();//done
            result += TestLoggedOutGets302onDoGet();//done
            //result += TestLoggedOutGets302onDoPost();//done
            result += TestDeactivateCanDeactivate(); //done
            result += TestWrongRoleGets302onDoGet();
            result += TestDeactivateFailIfAlreadyFalse(); //done
            result += TestDeactivateCanFailIfKeyNotFound(); // not done
            result += TestActivateCanFailIfKeyNotFound(); // not done
            result += TestactivateCanActivate(); // done
            result += TestActivateFailIfAlreadyTrue(); //done
            result += TestInitWithNoParamsDoesNotCrash();
            result += "\n}\n";
            return result;
        }
        //done
        public string genJavaEditServletTests()
        {
            string result = "";
            result += importStatementForTests();//done 
            result += classNameAndStaticVariables(4);//done 
            result += initTests(4);//done
            result += tearDownTests();//done
            result += testLoggedInGets200OnDoGet();//done
            result += testLoggedInGets302OnDoPostWithNoObjectSet();//done
            result += TestLoggedOutGets302onDoGet();//done
            result += TestLoggedOutGets302onDoPost();//done
            result += TestWrongRoleGets302onDoGet(); // done
            result += TestWrongRoleGets302onDoPost(); //  done
            result += TestGetOneGetsOne(); //done
            result += TestGetOneCanFail(); //done
            result += TestUpdateCanAddWithNoErrorsAndRedirects();
            result += TestUpdateHasErrorsForEachFiledAndKeepsOnSamePage();
            result += TestInitWithNoParamsDoesNotCrash();
            result += testUpdateCanReturnZero();
            result += testUpdateCanThrowSQLException();
            result += "\n}\n";
            return result;
        }
        //done
        private string initTests(int mode)
        {

            if (mode == 1)
            {
                servletName = "All_" + name;
            }

            if (mode == 2)
            {
                servletName = "Add_" + name;
            }
            if (mode == 3)
            {
                servletName = "Delete_" + name;
            }
            if (mode == 4)
            {
                servletName = "Edit_" + name;
            }
            string result = commentBox.genJavaTestJavaDoc(JavaTestType.setupTests, this);
            result += "@BeforeEach\n";
            
            result += "public void setup() throws ServletException{\n\n";
            result += "servlet = new " + servletName + "();\n";
            result += "servlet.init(";
            result += "new " + name + "_DAO_Fake()";
            foreach (Column r in columns)
            {

                if (r.foreign_key != "")
                {
                    string[] parts = r.references.Split('.');
                    string fk_table = parts[0];
                    string fk_name = parts[1];
                    result += ",new " + fk_table + "_DAO_Fake()";

                }
            }
            result += ");\n";
            result += "request =  new MockHttpServletRequest();\n";
            result += "response = new MockHttpServletResponse();\n";
            result += "session = new MockHttpSession();\n";
            result += "rd = new MockRequestDispatcher(PAGE);\n";
            result += "}\n\n";
            return result;
        }
        //done
        private string classNameAndStaticVariables(int mode)
        {
            string result = "";
            string servletName = "";
            if (mode == 1)
            {
                servletName = "All_" + name;
            }

            if (mode == 2)
            {
                servletName = "Add_" + name;
            }
            if (mode == 3)
            {
                servletName = "Delete_" + name;
            }
            if (mode == 4)
            {
                servletName = "Edit_" + name;
            }
            result += "\npublic class " + servletName + "Test {\n";

            result += "private static final String PAGE=\"WEB-INF/" + settings.database_name + "/" + servletName + ".jsp\";\n";
            result += servletName + " servlet;\n";
            result += "MockHttpServletRequest request;\n";
            result += "MockHttpServletResponse response;\n";
            result += "HttpSession session;\n";
            result += "RequestDispatcher rd;\n";

            return result;
        }
        private string packageStatementForTests()
        {
            return "package com." + settings.owner_name + "." + settings.database_name + ".controllers;\n";

        }
        //done
        private string importStatementForTests()
        {
            string result = "";
            result += "import java.io.IOException;\n";
            result += "import java.util.*;\n";
            result += "import com." + settings.owner_name + "." + settings.database_name + ".data_fakes." + name + "_DAO_Fake;\n";
            result += "import com." + settings.owner_name + "." + settings.database_name + ".models." + name + ";\n";
            result += "import com." + settings.owner_name + "." + settings.database_name + ".models." + name + "_VM;\n";
            result += "import com." + settings.owner_name + "." + settings.database_name + ".models.User;\n";
            result += "import jakarta.servlet.RequestDispatcher;\n";
            result += "import jakarta.servlet.ServletException;\n";
            result += "import jakarta.servlet.http.*;\n";
            result += "import org.junit.jupiter.api.AfterEach;\n";
            result += "import org.junit.jupiter.api.BeforeEach;\n";
            result += "import org.junit.jupiter.api.Test;\n";
            result += "import org.springframework.mock.web.*;\n";
            result += "import static org.junit.jupiter.api.Assertions.*;\n";

            return result;
        }
        //done
        private string tearDownTests()
        {
            string result = commentBox.genJavaTestJavaDoc(JavaTestType.teardownTests, this);
            result += "@AfterEach\n";
            
            result += "public void teardown(){\n";
            result += "servlet=null;\n";
            result += "request=null;\n";
            result += "response=null;\n";
            result += "session=null;\n";
            result += "rd=null;\n";
            result += "}\n";

            return result;
        }
        //done
        private string testLoggedInGets200OnDoGet()
        {
            string result = commentBox.genJavaTestJavaDoc(JavaTestType.TwoHundredIfLoggedIn, this);
            result += "@Test\n";
            result += "public void TestLoggedInUserGets200OnDoGet() throws ServletException, IOException{\n";
            result += SetUserOnTest("Jonathan");
            result += "request.setSession(session);\n";
            result += "servlet.doGet(request,response);\n";
            result += "int status = response.getStatus();\n";
            result += "assertEquals(200,status);\n";
            result += "}\n";
            return result;
        }
        //done
        private string TestLoggedOutGets302onDoGet()
        {
            string result = commentBox.genJavaTestJavaDoc(JavaTestType.ThreeOhTwoOnGet, this);
            result += "@Test\n";
            result += "public void TestLoggedOutUserGets302OnDoGet() throws ServletException, IOException{\n";

            result += "request.setSession(session);\n";
            result += "servlet.doGet(request,response);\n";
            result += "int status = response.getStatus();\n";
            result += "assertEquals(302,status);\n";
            result += "String redirect_link = response.getRedirectedUrl();\n";
            result += "String desired_redirect = \""+settings.database_name+"_in\";\n";
            result += "assertEquals(desired_redirect,redirect_link);\n";
            result += "session = request.getSession(false);\n";
            result += "assertNull(session);\n";
            result += "}\n";
            return result;
        }
        //done
        private string testLoggedInGets302OnDoPostWithNoObjectSet()
        {
            string result = commentBox.genJavaTestJavaDoc(JavaTestType.TwoHundredOnPost, this);
            result += "@Test\n";
            result += "public void TestLoggedInUserGets302nDoPostWithNo"+name+"Set() throws ServletException, IOException{\n";
            result += SetUserOnTest("Jonathan");
            result += "request.setSession(session);\n";
            result += "servlet.doPost(request,response);\n";
            result += "int status = response.getStatus();\n";
            result += "assertEquals(302,status);\n";
            result += "}\n";
            return result;
        }
        //done
        private string TestLoggedOutGets302onDoPost()
        {
            string result = commentBox.genJavaTestJavaDoc(JavaTestType.ThreeOhTwoOnPost, this);
            result += "@Test\n";
            result += "public void TestLoggedOutUserGets302OnDoPost() throws ServletException, IOException{\n";

            result += "request.setSession(session);\n";
            result += "servlet.doPost(request,response);\n";
            result += "int status = response.getStatus();\n";
            result += "assertEquals(302,status);\n";
            result += "String redirect_link = response.getRedirectedUrl();\n";
            result += "String desired_redirect = \"" + settings.database_name + "_in\";\n";
            result += "assertEquals(desired_redirect,redirect_link);\n";
            result += "session = request.getSession(false);\n";
            result += "assertNull(session);\n";
            result += "}\n";
            return result;
        }
        private string TestWrongRoleGets302onDoGet()
        {
            string result = commentBox.genJavaTestJavaDoc(JavaTestType.WrongRoleThreeOhTwoGet, this);
            result += "@Test\n";
            result += "public void TestWrongRoleGets302onDoGet() throws ServletException, IOException{\n";
            result += SetUserOnTest("WrongRole");
            result += "request.setSession(session);\n";
            result += "servlet.doGet(request,response);\n";
            result += "int status = response.getStatus();\n";
            result += "assertEquals(302,status);\n";
            result += "String redirect_link = response.getRedirectedUrl();\n";
            result += "String desired_redirect = \"/" + settings.database_name + "_in\";\n";
            result += "assertEquals(desired_redirect,redirect_link);\n";
            result += "}\n";
            return result;
        }

        private string TestWrongRoleGets302onDoPost()
        {
            string result = commentBox.genJavaTestJavaDoc(JavaTestType.WrongRoleThreeOhTwoPost, this);
            result += "@Test\n";
            result += "public void TestWrongRoleGets302onDoPost() throws ServletException, IOException{\n";
            result += SetUserOnTest("WrongRole");
            result += "request.setSession(session);\n";
            result += "servlet.doPost(request,response);\n";
            result += "int status = response.getStatus();\n";
            result += "assertEquals(302,status);\n";
            result += "String redirect_link = response.getRedirectedUrl();\n";
            result += "String desired_redirect = \"/" + settings.database_name + "_in\";\n";
            result += "assertEquals(desired_redirect,redirect_link);\n";
            result += "}\n";
            return result;
        }

        //done

        private string TestGetAllGetsAll()
        {
            string result = commentBox.genJavaTestJavaDoc(JavaTestType.LoggedInGetAllGetsAll, this);
            result += "@Test\n";
            result += "public void testLoggedInUserGetsAll" + name + "s() throws ServletException, IOException{\n";
            result += SetUserOnTest("Jonathan");
            result += "request.setSession(session);\n";
            result += "servlet.doGet(request,response);\n";
            result += "List<" + name + "_VM> " + name.ToLower() + "s = (List<" + name + "_VM>) request.getAttribute(\"" + name + "s\");\n";
            result += "assertNotNull(" + name.ToLower() + "s);\n";
            result += "assertEquals(20," + name.ToLower() + "s.size());\n";
            result += "}\n";

            return result;
        }

        private string TestGetOneGetsOne()
        {
            string result = "";
            foreach (Column r in columns)
                if (r.primary_key.Equals('y') || r.primary_key.Equals('Y'))
                {
                    {
                        result += commentBox.genJavaTestJavaDoc(JavaTestType.GetOneGetsOne, this);
                        result += "@Test\n";
                        result += "public void testGetOne" + name + "GetsOne" + r.column_name + "() throws ServletException, IOException{\n";
                        result += SetUserOnTest("Jonathan");
                        result += r.data_type.toJavaDataType() + " " + r.column_name + "= null;\n";
                        result += "request.setParameter(\"" + r.column_name.ToLower().Replace("_", "") + "\"," + r.column_name + ");\n";
                        result += "request.setSession(session);\n";
                        result += "servlet.doGet(request,response);\n";
                        result += name + "_VM " + name.ToLower() + " = (" + name + "_VM) session.getAttribute(\"" + name.ToLower() + "\");\n";
                        result += "assertNotNull(" + name.ToLower() + ");\n";
                        result += "assertEquals(" + r.column_name + "," + name.ToLower() + ".get" + r.column_name + "());\n";
                        result += "}\n";
                    }
                }
            return result;
        }

        private string TestGetOneCanFail()
        {
            string result = commentBox.genJavaTestJavaDoc(JavaTestType.GetOneCanFail, this);
            result += "@Test\n";
            result += "public void testGetOne" + name + "CanFail" + "() throws ServletException, IOException{\n";
            result += SetUserOnTest("Jonathan");
            foreach (Column r in columns)
            {
                int columnToNull = 1;
                if (columns.Count == 1)
                {

                    columnToNull = 0;
                }
                if (r.primary_key.Equals('y') || r.primary_key.Equals('Y'))
                {
                    {

                        result += r.data_type.toJavaDataType() + " " + r.column_name + "= null;\n";
                        result += "request.setParameter(\"" + r.column_name.ToLower().Replace("_", "") + "\"," + r.column_name + ");\n";

                        result += "assertNull(" + name.ToLower() + ".get" + columns[columnToNull].column_name + "());\n";

                    }
                }
            }
            result += "request.setSession(session);\n";
            result += "servlet.doGet(request,response);\n";
            result += name + "_VM " + name.ToLower() + " = (" + name + "_VM) session.getAttribute(\"" + name.ToLower() + "\");\n";
            foreach (Column r in columns)
            {
                if (r.data_type.toCSharpDataType().Equals("bool"))
                {
                    result += "assertFalse(" + name.ToLower() + ".get" + r.column_name + "());\n";
                }
                else
                {
                    result += "assertNull(" + name.ToLower() + ".get" + r.column_name + "());\n";
                }
            }
            result += "}\n";
            return result;
        }

        private string TestGetAllCanFilter()
        {
            string result = "";
            foreach (Column r in columns)
                if (r.references != "")
                {
                    {
                        result += commentBox.genJavaTestJavaDoc(JavaTestType.GetAllCanFilter, this);
                        result += "@Test\n";
                        result += "public void testLoggedInUserCanFilter" + name + "sBy" + r.column_name + "() throws ServletException, IOException{\n";
                        result += SetUserOnTest("Jonathan");
                        result += r.data_type.toJavaDataType() + " " + r.column_name + "= null;\n";
                        result += "request.setParameter(\"" + r.column_name + "\"," + r.column_name + ");\n";
                        result += "request.setSession(session);\n";
                        result += "servlet.doGet(request,response);\n";
                        result += "List<" + name + "_VM> " + name.ToLower() + "s = (List<" + name + "_VM>) request.getAttribute(\"" + name + "s\");\n";
                        result += "assertNotNull(" + name.ToLower() + "s);\n";
                        result += "assertEquals(20," + name.ToLower() + "s.size());\n";
                        result += "}\n";
                    }
                }
            return result;
        }

        private string TestGetAllCanSearch()
        {
            string result = "";       
            result += commentBox.genJavaTestJavaDoc(JavaTestType.GetAllCanFilter, this);
            result += "@Test\n";
            result += "public void testLoggedInUserCanSearch() throws ServletException, IOException{\n";
            result += SetUserOnTest("Jonathan");
            result += "String searchTerm = \"\";\n";
            result += "request.setParameter(\"search\",searchTerm);\n";
            result += "request.setSession(session);\n";
            result += "servlet.doGet(request,response);\n";
            result += "List<" + name + "_VM> " + name.ToLower() + "s = (List<" + name + "_VM>) request.getAttribute(\"" + name + "s\");\n";
            result += "assertNotNull(" + name.ToLower() + "s);\n";
            result += "assertEquals(20," + name.ToLower() + "s.size());\n";
            result += "}\n";         
            return result;
        }
        private string TestGetAllCanSearchAndReturnZero()
        {
            string result = "";
            result += commentBox.genJavaTestJavaDoc(JavaTestType.GetAllCanFilter, this);
            result += "@Test\n";
            result += "public void testLoggedInUserCanSearchAndReturnZero() throws ServletException, IOException{\n";
            result += SetUserOnTest("Jonathan");
            result += "String searchTerm = \"\";\n";
            result += "request.setParameter(\"search\",searchTerm);\n";
            result += "request.setSession(session);\n";
            result += "servlet.doGet(request,response);\n";
            result += "List<" + name + "_VM> " + name.ToLower() + "s = (List<" + name + "_VM>) request.getAttribute(\"" + name + "s\");\n";
            result += "assertNotNull(" + name.ToLower() + "s);\n";
            result += "assertEquals(0," + name.ToLower() + "s.size());\n";
            result += "}\n";
            return result;
        }

        private string TestAddHasErrorsForEachFieldAndKeepsOnSamePage()
        {
            string result  = commentBox.genJavaTestJavaDoc(JavaTestType.ErrorMessagesforEachField, this);
            result += "@Test\n";
            result += "public void TestAddHasErrorsForEachFieldAndKeepsOnSamePage() throws ServletException, IOException{\n";
            result += SetUserOnTest("Jonathan");
            result += "request.setSession(session);\n";
            result += "servlet.doPost(request,response);\n";
            result += "int responseStatus = response.getStatus();\n";
            result += "Map<String, String> results = (Map<String, String>) request.getAttribute(\"results\");\n";
            foreach (Column r in columns)
            {
                if (r.increment == 0 && !r.data_type.toCSharpDataType().Equals("bool"))
                {
                    result += "String " + r.column_name + "Error = results.get(\"" + name.ToLower() + r.column_name + "error\");\n";
                }
            }
            foreach (Column r in columns)
            {
                if (r.increment == 0 && !r.data_type.toCSharpDataType().Equals("bool"))
                {
                    result += "assertNotEquals(\"\"," + r.column_name + "Error);\n";
                    result += "assertNotNull(" + r.column_name + "Error);\n";
                }
            }

            result += "assertEquals(200,responseStatus);\n";
            result += "}\n";

            return result;
        }
        private string TestAddCanAddWithNoErrorsAndRedirects()
        {
            string result = commentBox.genJavaTestJavaDoc(JavaTestType.CanAddWithNoErrors, this);
            result += "@Test\n";
            result += "public void TestAddCanAddWithNoErrorsAndRedirects() throws ServletException, IOException{\n";
            result += SetUserOnTest("Jonathan");
            result += "request.setSession(session);\n";
            foreach (Column r in columns)
            {
                if (r.increment == 0)
                {
                    if (r.data_type.toCSharpDataType().Equals("string"))
                    {
                        result += "request.setParameter(\"input" + name.ToLower() + r.column_name + "\",\"TestValue\");\n";

                    }
                    if (r.data_type.toCSharpDataType().Equals("int"))
                    {
                        result += "request.setParameter(\"input" + name.ToLower() + r.column_name + "\",\"406\");\n";

                    }
                    if (r.data_type.toCSharpDataType().Equals("bool"))
                    {
                        result += "request.setParameter(\"input" + name.ToLower() + r.column_name + "\",\"true\");\n";

                    }
                    if (r.data_type.toCSharpDataType().ToLower().Contains("date")|| r.data_type.toCSharpDataType().Contains("time"))
                    {
                        result += "request.setParameter(\"input" + name.ToLower() + r.column_name + "\",\"12-31-06 06:21:22 PM\");\n";

                    }

                    if (r.data_type.toCSharpDataType().ToLower().Contains("decimal"))
                    {
                        result += "request.setParameter(\"input" + name.ToLower() + r.column_name + "\",\"243.6\");\n";

                    }
                }
            }
            result += "servlet.doPost(request,response);\n";
            result += "int responseStatus = response.getStatus();\n";
            result += "Map<String, String> results = (Map<String, String>) request.getAttribute(\"results\");\n";
            result += "String " + name + "_Added = results.get(\"dbStatus\");\n";
            result += "assertEquals(302,responseStatus);\n";
            result += "assertNotNull(" + name + "_Added);\n";
            result += "assertEquals(\"" + name + " Added\"," + name + "_Added);\n";
            result += "assertNotEquals(\"\"," + name + "_Added);\n";
            result += "}\n";
            return result;
        }

        private string testExceptionKeyThrowsException()
        {  //fix
            string result = commentBox.genJavaTestJavaDoc(JavaTestType.CanThrowException, this);
            result += "@Test\n";
            result += "public void testExceptionKeyThrowsException() throws ServletException, IOException{\n";
            result += SetUserOnTest("Jonathan");
            result += "request.setSession(session);\n";
            foreach (Column r in columns)
            {
                if (r.increment == 0)
                {
                    if (r.data_type.toCSharpDataType().Equals("string"))
                    {
                        result += "request.setParameter(\"input" + name.ToLower() + r.column_name + "\",\"EXCEPTION\");\n";

                    }
                    if (r.data_type.toCSharpDataType().Equals("int"))
                    {
                        result += "request.setParameter(\"input" + name.ToLower() + r.column_name + "\",\"406\");\n";

                    }
                    if (r.data_type.toCSharpDataType().Equals("bool"))
                    {
                        result += "request.setParameter(\"input" + name.ToLower() + r.column_name + "\",\"true\");\n";

                    }
                    if (r.data_type.toCSharpDataType().ToLower().Contains("date") || r.data_type.toCSharpDataType().Contains("time"))
                    {
                        result += "request.setParameter(\"input" + name.ToLower() + r.column_name + "\",\"12-31-06 06:21:22 PM\");\n";

                    }

                    if (r.data_type.toCSharpDataType().ToLower().Contains("decimal"))
                    {
                        result += "request.setParameter(\"input" + name.ToLower() + r.column_name + "\",\"243.6\");\n";

                    }
                }
            }
            result += "servlet.doPost(request,response);\n";
            result += "int responseStatus = response.getStatus();\n";
            result += "Map<String, String> results = (Map<String, String>) request.getAttribute(\"results\");\n";
            result += "String " + name + "_Added = results.get(\"dbStatus\");\n";
            result += "String dbError = results.get(\"dbError\");\n";
            result += "assertEquals(200,responseStatus);\n";
            result += "assertNotNull(" + name + "_Added);\n";
            result += "assertEquals(\"" + name + " Not Added\"," + name + "_Added);\n";
            result += "assertNotEquals(\"\"," + name + "_Added);\n";
            result += "assertNotNull(dbError);\n";
            result += "assertNotEquals(\"\",dbError);\n";
            result += "assertEquals(\"Database Error\",dbError);\n";
            result += "}\n";
            return result;
        }

        private string testDuplicateKeyAddsZero()
        {
            string result = commentBox.genJavaTestJavaDoc(JavaTestType.DuplicateDoesNotGetAdded, this);
            result += "@Test\n";
            result += "public void testDuplicateKeyReturnsZero() throws ServletException, IOException{\n";
            result += SetUserOnTest("Jonathan");
            result += "request.setSession(session);\n";
            foreach (Column r in columns)
            {
                if (r.increment == 0)
                {
                    if (r.data_type.toCSharpDataType().Equals("string"))
                    {
                        result += "request.setParameter(\"input" + name.ToLower() + r.column_name + "\",\"DUPLICATE\");\n";

                    }
                    if (r.data_type.toCSharpDataType().Equals("int"))
                    {
                        result += "request.setParameter(\"input" + name.ToLower() + r.column_name + "\",\"406\");\n";

                    }
                    if (r.data_type.toCSharpDataType().Equals("bool"))
                    {
                        result += "request.setParameter(\"input" + name.ToLower() + r.column_name + "\",\"true\");\n";

                    }
                    if (r.data_type.toCSharpDataType().ToLower().Contains("date") || r.data_type.toCSharpDataType().Contains("time"))
                    {
                        result += "request.setParameter(\"input" + name.ToLower() + r.column_name + "\",\"12-31-06 06:21:22 PM\");\n";

                    }

                    if (r.data_type.toCSharpDataType().ToLower().Contains("decimal"))
                    {
                        result += "request.setParameter(\"input" + name.ToLower() + r.column_name + "\",\"243.6\");\n";

                    }
                }
            }
            result += "servlet.doPost(request,response);\n";
            result += "int responseStatus = response.getStatus();\n";
            result += "Map<String, String> results = (Map<String, String>) request.getAttribute(\"results\");\n";
            result += "String " + name + "_Added = results.get(\"dbStatus\");\n";
            result += "assertEquals(200,responseStatus);\n";
            result += "assertNotNull(" + name + "_Added);\n";
            result += "assertEquals(\"" + name + " Not Added\"," + name + "_Added);\n";
            result += "assertNotEquals(\"\"," + name + "_Added);\n";
            result += "}\n";
            return result;

        }

        private string TestUpdateHasErrorsForEachFiledAndKeepsOnSamePage()
        {
            string result = commentBox.genJavaTestJavaDoc(JavaTestType.ErrorMessagesforEachField, this);
            result += "@Test\n";
            result += "public void TestUpdateHasErrorsForEachFiledAndKeepsOnSamePage() throws ServletException, IOException{\n";
            result += SetUserOnTest("Jonathan");
            result += name + " " + name.ToLower() + " = new " + name + "();\n";
            result += "session.setAttribute(\"" + name.ToLower() + "\"," + name.ToLower() + ");\n";
            result += "request.setSession(session);\n";
            result += "servlet.doPost(request,response);\n";
            result += "int responseStatus = response.getStatus();\n";
            result += "Map<String, String> results = (Map<String, String>) request.getAttribute(\"results\");\n";
            foreach (Column r in columns)
            {
                if (r.increment == 0 && !r.data_type.toCSharpDataType().Equals("bool"))
                {
                    result += "String " + r.column_name + "Error = results.get(\"" + name.ToLower() + r.column_name + "error\");\n";
                }
            }
            foreach (Column r in columns)
            {
                if (r.increment == 0 && !r.data_type.toCSharpDataType().Equals("bool"))
                {
                    result += "assertNotEquals(\"\"," + r.column_name + "Error);\n";
                    result += "assertNotNull(" + r.column_name + "Error);\n";
                }
            }

            result += "assertEquals(200,responseStatus);\n";
            result += "}\n";

            return result;
        }
        private string TestUpdateCanAddWithNoErrorsAndRedirects()
        {
            string result = commentBox.genJavaTestJavaDoc(JavaTestType.CanUpdateWithNoErrors, this);
            result += "@Test\n";
            result += "public void TestUpdateCanAddWithNoErrorsAndRedirects() throws ServletException, IOException{\n";
            result += SetUserOnTest("Jonathan");
            result += "request.setSession(session);\n";
            result += "//to set the old " + name + "\n";
            result += name + " " + name.ToLower() + " = new " + name + "();\n";
            foreach (Column r in columns)
            {
                if (r.data_type.toCSharpDataType().Equals("string"))
                {
                    result += name.ToLower() + ".set" + r.column_name + "(\"test" + name + "\");\n";

                }
                else if (r.data_type.toCSharpDataType().Equals("int"))
                {
                    result += name.ToLower() + ".set" + r.column_name + "(43);\n";

                }
                else if (r.data_type.toCSharpDataType().Equals("bool"))
                {
                    result += name.ToLower() + ".set" + r.column_name + "(true);\n";

                }
                else if(r.data_type.ToLower().Contains("date") || r.data_type.toCSharpDataType().Contains("time"))
                {
                    result += name.ToLower() + ".set" + r.column_name + "(new Date());\n";
                }

                else if(r.data_type.ToLower().Contains("decimal"))
                {
                    result += name.ToLower() + ".set" + r.column_name + "(23.2d);\n";
                }

                else
                {
                    result += name.ToLower() + ".set" + r.column_name + "(new " + r.data_type + ".toString());\n";
                }
            }

            result += "session.setAttribute(\"" + name.ToLower() + "\"," + name.ToLower() + ");\n";
            result += "//create a new albums parameters\n";
            foreach (Column r in columns)
            {
                if (r.increment == 0)
                {
                    if (r.data_type.toCSharpDataType().Equals("string"))
                    {
                        result += "request.setParameter(\"input" + name.ToLower() + r.column_name + "\",\"TestValue\");\n";

                    }
                    if (r.data_type.toCSharpDataType().Equals("int"))
                    {
                        result += "request.setParameter(\"input" + name.ToLower() + r.column_name + "\",\"406\");\n";

                    }
                    if (r.data_type.toCSharpDataType().Equals("bool"))
                    {
                        result += "request.setParameter(\"input" + name.ToLower() + r.column_name + "\",\"true\");\n";

                    }

                    if (r.data_type.ToLower().Contains("date") || r.data_type.toCSharpDataType().Contains("time"))
                    {
                        result += "request.setParameter(\"input" + name.ToLower() + r.column_name + "\",\"12-31-06 06:21:22 PM\");\n";

                    }

                    if (r.data_type.ToLower().Contains("decimal"))
                    {
                        result += "request.setParameter(\"input" + name.ToLower() + r.column_name + "\",\"243.6\");\n";

                    }
                }
            }
            result += "servlet.doPost(request,response);\n";
            result += "int responseStatus = response.getStatus();\n";
            result += "Map<String, String> results = (Map<String, String>) request.getAttribute(\"results\");\n";
            result += "String " + name + "_Updated = results.get(\"dbStatus\");\n";
            result += "assertEquals(302,responseStatus);\n";
            result += "assertNotNull(" + name + "_Updated);\n";
            result += "assertEquals(\"" + name + " updated\"," + name + "_Updated);\n";
            result += "assertNotEquals(\"\"," + name + "_Updated);\n";
            result += "}\n";
            return result;
        }

        private string testUpdateCanThrowSQLException()
        {
            string result = commentBox.genJavaTestJavaDoc(JavaTestType.CanThrowException, this);
            result += "@Test\n";
            result += "public void testUpdateCanThrowSQLException() throws ServletException, IOException{\n";
            result += SetUserOnTest("Jonathan");
            result += "request.setSession(session);\n";
            result += "//to set the old " + name + "\n";
            result += name + " " + name.ToLower() + " = new " + name + "();\n";
            foreach (Column r in columns)
            {
                if (r.data_type.toCSharpDataType().Equals("string"))
                {
                    result += name.ToLower() + ".set" + r.column_name + "(\"EXCEPTION\");\n";

                }
                if (r.data_type.toCSharpDataType().Equals("int"))
                {
                    result += name.ToLower() + ".set" + r.column_name + "(43);\n";

                }
                if (r.data_type.toCSharpDataType().Equals("bool"))
                {
                    result += name.ToLower() + ".set" + r.column_name + "(true);\n";

                }
            }

            result += "session.setAttribute(\"" + name.ToLower() + "\"," + name.ToLower() + ");\n";
            result += "//create a new albums parameters\n";
            foreach (Column r in columns)
            {
                if (r.increment == 0)
                {
                    if (r.data_type.toCSharpDataType().Equals("string"))
                    {
                        result += "request.setParameter(\"input" + name.ToLower() + r.column_name + "\",\"EXCEPTION\");\n";

                    }
                    if (r.data_type.toCSharpDataType().Equals("int"))
                    {
                        result += "request.setParameter(\"input" + name.ToLower() + r.column_name + "\",\"406\");\n";

                    }
                    if (r.data_type.toCSharpDataType().Equals("bool"))
                    {
                        result += "request.setParameter(\"input" + name.ToLower() + r.column_name + "\",\"true\");\n";

                    }
                    if (r.data_type.ToLower().Contains("date") || r.data_type.toCSharpDataType().Contains("time"))
                    {
                        result += "request.setParameter(\"input" + name.ToLower() + r.column_name + "\",\"12-31-06 06:21:22 PM\");\n";

                    }

                    if (r.data_type.ToLower().Contains("decimal"))
                    {
                        result += "request.setParameter(\"input" + name.ToLower() + r.column_name + "\",\"243.6\");\n";

                    }
                }
            }
            result += "servlet.doPost(request,response);\n";
            result += "int responseStatus = response.getStatus();\n";
            result += "Map<String, String> results = (Map<String, String>) request.getAttribute(\"results\");\n";
            result += "String " + name + "_Updated = results.get(\"dbStatus\");\n";
            result += "String dbError = results.get(\"dbError\");\n";
            result += "assertEquals(200,responseStatus);\n";
            result += "assertNotNull(" + name + "_Updated);\n";
            result += "assertNotEquals(\"\"," + name + "_Updated);\n";
            result += "assertEquals(\"" + name + " Not Updated\"," + name + "_Updated);\n";
            result += "assertNotNull(dbError);\n";
            result += "assertNotEquals(\"\",dbError);\n";
            result += "assertEquals(\"Database Error\",dbError);\n";
            result += "}\n";
            return result;

        }

        private string testUpdateCanReturnZero()
        {
            string result = commentBox.genJavaTestJavaDoc(JavaTestType.DuplicateDoesNotGetUpdated, this);
            result += "@Test\n";
            result += "public void testUpdateCanReturnZero() throws ServletException, IOException{\n";
            result += SetUserOnTest("Jonathan");
            result += "request.setSession(session);\n";
            result += "//to set the old " + name + "\n";
            result += name + " " + name.ToLower() + " = new " + name + "();\n";
            foreach (Column r in columns)
            {
                if (r.data_type.toCSharpDataType().Equals("string"))
                {
                    result += name.ToLower() + ".set" + r.column_name + "(\"DUPLICATE\");\n";

                }
                else if (r.data_type.toCSharpDataType().Equals("int"))
                {
                    result += name.ToLower() + ".set" + r.column_name + "(43);\n";

                }
                else if (r.data_type.toCSharpDataType().Equals("bool"))
                {
                    result += name.ToLower() + ".set" + r.column_name + "(true);\n";

                }
                else
                {
                    result += name.ToLower() + ".set" + r.column_name + "(new " + r.data_type + ".toString());\n";
                }
            }

            result += "session.setAttribute(\"" + name.ToLower() + "\"," + name.ToLower() + ");\n";
            result += "//create a new albums parameters\n";
            foreach (Column r in columns)
            {
                if (r.increment == 0)
                {
                    if (r.data_type.toCSharpDataType().Equals("string"))
                    {
                        result += "request.setParameter(\"input" + name.ToLower() + r.column_name + "\",\"DUPLICATE\");\n";

                    }
                    if (r.data_type.toCSharpDataType().Equals("int"))
                    {
                        result += "request.setParameter(\"input" + name.ToLower() + r.column_name + "\",\"406\");\n";

                    }
                    if (r.data_type.toCSharpDataType().Equals("bool"))
                    {
                        result += "request.setParameter(\"input" + name.ToLower() + r.column_name + "\",\"true\");\n";

                    }
                    if (r.data_type.toCSharpDataType().ToLower().Contains("date") || r.data_type.toCSharpDataType().Contains("time"))
                    {
                        result += "request.setParameter(\"input" + name.ToLower() + r.column_name + "\",\"12-31-06 06:21:22 PM\");\n";

                    }

                    if (r.data_type.toCSharpDataType().ToLower().Contains("decimal"))
                    {
                        result += "request.setParameter(\"input" + name.ToLower() + r.column_name + "\",\"243.6\");\n";

                    }
                }
            }
            result += "servlet.doPost(request,response);\n";
            result += "int responseStatus = response.getStatus();\n";
            result += "Map<String, String> results = (Map<String, String>) request.getAttribute(\"results\");\n";
            result += "String " + name + "_Updated = results.get(\"dbStatus\");\n";
            result += "assertEquals(200,responseStatus);\n";
            result += "assertNotNull(" + name + "_Updated);\n";
            result += "assertEquals(\"" + name + " Not Updated\"," + name + "_Updated);\n";
            result += "assertNotEquals(\"\"," + name + "_Updated);\n";
            result += "}\n";
            return result;
        }

        private string TestDeactivateCanDeactivate()
        {
            string result = commentBox.genJavaTestJavaDoc(JavaTestType.DeactivateCanDeactivate, this);
            result += "@Test\n";
            result += "public void TestDeactivateCanDeactivate() throws ServletException, IOException {\n";
            result += SetUserOnTest("Jonathan");
            result += "request.setSession(session);\n";
            result += "request.setParameter(\"" + name + "id\",null);\n";
            result += "request.setParameter(\"mode\",\"0\");\n";
            result += "servlet.doPost(request,response);\n";
            result += "int status = (int) request.getAttribute(\"result\");\n";
            result += "assertEquals(1,status);\n";
            result += "}\n";

            return result;
        }

        private string TestDeactivateFailIfAlreadyFalse()
        {
            string result = commentBox.genJavaTestJavaDoc(JavaTestType.DeactivateCanFailIfalreadyInactive, this);
            result += "@Test\n";
            result += "public void TestDeactivateFailIfAlreadyFalse() throws ServletException, IOException {\n";
            result += SetUserOnTest("Jonathan");
            result += "request.setSession(session);\n";
            result += "request.setParameter(\"" + name + "id\",null);\n";
            result += "request.setParameter(\"mode\",\"0\");\n";
            result += "servlet.doPost(request,response);\n";
            result += "int status = (int) request.getAttribute(\"result\");\n";
            result += "assertEquals(0,status);\n";
            result += "}\n";

            return result;
        }

        private string TestDeactivateCanFailIfKeyNotFound()
        {
            string result = commentBox.genJavaTestJavaDoc(JavaTestType.DeactiveCanFailWithKeyNotfound, this);
            result += "@Test\n";
            result += "public void TestDeActivateCanFailIfKeyDoesNotExist() throws ServletException, IOException {\n";
            result += SetUserOnTest("Jonathan");
            result += "request.setSession(session);\n";
            result += "request.setParameter(\"" + name + "id\",\"xxxxxxxxxxxxx\");\n";
            result += "request.setParameter(\"mode\",\"0\");\n";
            result += "servlet.doPost(request,response);\n";
            result += "int status = (int) request.getAttribute(\"result\");\n";
            result += "Map<String, String> results = (Map<String, String>) request.getAttribute(\"results\");\n";
            result += "String dbStatus = results.get(\"dbStatus\");\n";
            result += "assertEquals(\"Unable To Find " + name + ".\",dbStatus);\n";
            result += "assertEquals(0,status);\n";
            result += "}\n";

            return result;

        }
        private string TestactivateCanActivate()
        {
            string result = commentBox.genJavaTestJavaDoc(JavaTestType.ActivateCanActivate, this);
            result += "@Test\n";
            result += "public void TestactivateCanActivate() throws ServletException, IOException {\n";
            result += SetUserOnTest("Jonathan");
            result += "request.setSession(session);\n";
            result += "request.setParameter(\"" + name + "id\",null);\n";
            result += "request.setParameter(\"mode\",\"1\");\n";
            result += "servlet.doPost(request,response);\n";
            result += "int status = (int) request.getAttribute(\"result\");\n";
            result += "assertEquals(1,status);\n";
            result += "}\n";

            return result;
        }

        private string TestActivateFailIfAlreadyTrue()
        {
            string result = commentBox.genJavaTestJavaDoc(JavaTestType.ActivateCanFailIfAlreadayActive, this);
            result += "@Test\n";
            result += "public void TestActivateFailIfAlreadyTrue() throws ServletException, IOException {\n";
            result += SetUserOnTest("Jonathan");
            result += "request.setSession(session);\n";
            result += "request.setParameter(\"" + name + "id\",null);\n";
            result += "request.setParameter(\"mode\",\"1\");\n";
            result += "servlet.doPost(request,response);\n";
            result += "int status = (int) request.getAttribute(\"result\");\n";
            result += "assertEquals(0,status);\n";
            result += "}\n";

            return result;
        }

        private string TestActivateCanFailIfKeyNotFound()
        {
            string result = commentBox.genJavaTestJavaDoc(JavaTestType.ActivateCanFailWithKeyNotFound, this);
            result += "@Test\n";
            result += "public void TestActivateCanFailIfKeyDoesNotExist() throws ServletException, IOException {\n";
            result += SetUserOnTest("Jonathan");
            result += "request.setSession(session);\n";
            result += "request.setParameter(\"" + name + "id\",\"xxxxxxxxxxxxx\");\n";
            result += "request.setParameter(\"mode\",\"1\");\n";
            result += "servlet.doPost(request,response);\n";
            result += "int status = (int) request.getAttribute(\"result\");\n";
            result += "Map<String, String> results = (Map<String, String>) request.getAttribute(\"results\");\n";
            result += "String dbStatus = results.get(\"dbStatus\");\n";
            result += "assertEquals(\"Unable To Find " + name + ".\",dbStatus);\n";
            result += "assertEquals(0,status);\n";
            result += "}\n";
            return result;
        }
        private string TestInitWithNoParamsDoesNotCrash()
        {
            string result = commentBox.genJavaTestJavaDoc(JavaTestType.initTest, this);
            result += "@Test\n";
            result += "public void testInitWithNoParametersDoesNotThrowException() throws ServletException {\n";
            result += "servlet = null;\n";
            result += "servlet = new " + servletName + "();\n";
            result += "servlet.init();\n";
            result += "}\n";

            return result;

        }

        private string SetUserOnTest(string role)
        {
            string result = "User user = new User();\n";
            result += "user.setRole_ID(\"" + role + "\");\n";
            result += "session.setAttribute(\"User\",user);\n";

            return result;

        }

        public string genJavascriptObject()
        {
            string result = "";
            result += genJavascriptConstructor();
            result += genJavascriptsetterAndGetter();
            result += "\n}";
            result += "\n";
            return result;
        }

        private string genJavascriptConstructor()
        {
            string result = "";
            result += "function " + name + "(";
            string comma = "";
            foreach (Column r in columns)
            {
                result += comma + "_" + r.column_name;
                comma = ",";
            }
            result += ") {\n";
            comma = "";
            foreach (Column r in columns)
            {
                result += comma + "this." + r.column_name + " = _" + r.column_name + ";";
                comma = "\n";
            }
            result += "\n";
            return result;
        }

        private string genJavascriptsetterAndGetter()
        {
            string result = "";
            foreach (Column r in columns)
            {
                //setter
                result += "this.set" + r.column_name + "= function (_" + r.column_name + "){\n";
                result += "this." + r.column_name + "= _" + r.column_name + ";\n";
                result += "}\n";
                //getter
                result += "this.get" + r.column_name + "= function() {\n";
                result += "return this." + r.column_name + ";\n";
                result += "}\n";

            }
            return result;
        }
        public string genPythonObject()
        {
            string result = "";
            result += genPythonModel();
            result += genPythonInstanceConstructor();
            result += genPythonsetterAndGetter();
            result += "\n";
            result += "\n";
            return result;
        }

        public string genPythonCommmands()
        {
            string result = "";

            result += genPythonCreate();
            result += genPythonCreateFK();
            result += genPythonGetAll();
            result += genPythonGetByPK();
            result += genPythonGetByFK();
            result += genPythonDelete();
            result += genPythonUpdate();
            return result;
        }
        private string genPythonInstanceConstructor()
        {
            string result = "";
            result += name.ToLower() + " = " + name + "(";
            string comma = "";
            foreach (Column r in columns)
            {
                result += comma + r.column_name + "='" + genFakeData(r) + "'";
                comma = ",";

            }
            result += ")\n";
            return result;
        }

        private string genPythonModel()
        {
            string result = "class " + name + "(models.Model):\n";
            foreach (Column r in columns)
            {
                string commentText = "\"holds " + r.column_name + " related to the " + name + " object\"";

                if (r.increment != 0)
                {
                    result += fourSpaces + r.column_name + " =  models.AutoField(" + commentText + ",primary_key=True)\n";
                    continue;
                }
                if (r.references != "")
                {
                    string[] parts = r.references.Split('.');
                    string fk_table = parts[0];
                    string fk_name = parts[1];
                    result += fourSpaces + r.column_name + " =  models.ForeignKey(" + fk_table + ", on_delete=models.CASCADE)\n";
                    continue;
                }
                if (r.column_name.ToLower().Contains("email"))
                {
                    result += fourSpaces + r.column_name + " =  models.EmailField(" + commentText + ")\n";
                    continue;
                }
                string primary_key = "";
                string default_value = "";
                string unique = "";
                string nullable = "";

                string comma = ",";
                if (r.primary_key.Equals('y') || r.primary_key.Equals('Y'))
                {

                    primary_key = comma + "primary_key=True";
                    comma = ",";
                }
                if (r.default_value != "")
                {
                    default_value = comma + "default=" + r.default_value;
                    comma = ",";
                }
                if (r.unique == 'y' || r.unique == 'Y')
                {
                    unique = comma + "unique=True";
                    comma = ",";
                }
                if (r.nullable == 'y' || r.nullable == 'Y')
                {
                    nullable = comma + "null=True";
                    comma = ",";
                }

                result += fourSpaces + r.column_name + "=models." + r.data_type.toDjangoDataType(r.length, commentText) + primary_key + default_value + unique + nullable + ")\n";

            }
            result += genPython__init__();
            result += genPythonToString();
            return result;
        }
        /// <summary>
        /// Generates a custom python init method for an object based on this <see cref="table"/>. 
        /// This init method takes a paramater for each <see cref="Column"/> in <see cref="columns"/>
        /// </summary>
        /// <returns>a <see cref="String"/> that represents formatted python code for an __init__ function</returns>
        private string genPython__init__()
        {
            string result = fourSpaces + "def __init__ (self";
            foreach (Column r in columns)
            {
                result += ", _" + r.column_name.ToLower();

            }
            result += ") :\n";
            foreach (Column r in columns)
            {
                result += fourSpaces + fourSpaces + "self." + r.column_name + " = _" + r.column_name.ToLower() + "\n";

            }

            return result;

        }
        /// <summary>
        /// Generates a custom python __str__ method for an object based on this <see cref="table"/>. 
        /// This __str__ method prints the current value for each <see cref="Column"/> in <see cref="columns"/>
        /// </summary>
        /// <returns>a <see cref="String"/> that represents formatted python code for an __str__ function</returns>
        private string genPythonToString()
        {
            string result = fourSpaces + "# to define the to string method for the " + name + " model\n";
            result += fourSpaces + "def __str__(self):\n";
            result += fourSpaces + fourSpaces + "stringRep = \"\\n\"\n";
            foreach (Column r in columns)
            {
                result += fourSpaces + fourSpaces + "stringRep += \"" + r.column_name + " = \" + self." + r.column_name + ".__str__()+\"\\n\"\n";

            }
            result += fourSpaces + fourSpaces + "stringRep += \"\\n\"\n";
            result += fourSpaces + fourSpaces + "return stringRep\n";
            return result;
        }
        /// <summary>
        /// Generates a custom python getter and setter method for an object based on this <see cref="table"/>. 
        /// The getter methods take no paramaters and return the current value of the variable.
        /// the setter methods take a paramater and set the associated value
        /// One setter and one getter is created for each <see cref="Column"/> in <see cref="columns"/>
        /// </summary>
        /// <returns>a <see cref="String"/> that represents formatted python code for an setter and getter functions</returns>
        private string genPythonsetterAndGetter()
        {
            string result = "";
            foreach (Column r in columns)
            {
                //setter
                result += "";
                //getter
                result += "";

            }
            return result;
        }

        private string genPythonCreate()
        {
            string result = genPythonInstanceConstructor();
            result += name.ToLower() + ".save()\n";
            return result;

        }

        private string genPythonCreateFK()
        {
            string result = "";
            foreach (foreignKey key in data_tables.all_foreignKey)
                if (key.referenceTable.ToLower().Equals(name.ToLower()))
                {
                    foreach (table t in data_tables.all_tables)
                    {
                        if (t.name.Equals(key.mainTable))
                        {
                            {
                                result += name.ToLower() + "." + key.mainTable.ToLower() + "_set.create(";
                                string comma = "";

                                foreach (Column r in t.columns)
                                {
                                    if (r.increment == 0 && r.references == "")
                                    {
                                        result += comma + r.column_name + "=" + genFakeData(r) + "";
                                        comma = ",";
                                    }
                                }

                                result += ")\n";
                            }
                        }
                    }
                }
            return result;

        }
        private string genPythonGetAll()
        {
            string result = name + ".objects.values()\n";
            return result;

        }
        private string genPythonGetByPK()
        {
            string result = "";
            string comma = "";
            result += name + ".objects.filter(";
            foreach (Column r in columns)
            {
                if (r.primary_key == 'Y' || r.primary_key == 'y')
                {
                    result += comma + r.column_name + "='xxx'";
                    comma = ",";
                }
            }
            result += ").values()\n";
            return result;

        }

        private string genPythonGetByFK()
        {
            string result = "";
            foreach (foreignKey key in data_tables.all_foreignKey)
            {
                if (key.referenceTable.ToLower().Equals(name.ToLower()))
                {
                    result += name.ToLower() + "." + key.mainTable.ToLower() + "_set.all()\n";
                    result += name.ToLower() + "." + key.mainTable.ToLower() + "_set.count()\n";
                }
            }
            return result;

        }
        private string genPythonDelete()
        {
            string result = "";
            string comma = "";
            result += name + ".objects.filter(";
            foreach (Column r in columns)
            {
                if (r.primary_key == 'Y' || r.primary_key == 'y')
                {
                    result += comma + r.column_name + "='xxx'";
                    comma = ",";
                }
            }
            result += ").delete()\n";
            return result;

        }
        private string genPythonUpdate()
        {
            string result = "";
            string comma = "";
            result += name + ".objects.filter(";
            foreach (Column r in columns)
            {
                if (r.primary_key == 'Y' || r.primary_key == 'y')
                {
                    result += comma + r.column_name + "='xxx'";
                    comma = ",";
                }
            }
            result += ").update(";
            comma = "";
            foreach (Column r in columns)
            {
                if (r.primary_key != 'Y' && r.primary_key != 'y')
                {
                    result += comma + r.column_name + "='xxx'";
                    comma = ",";
                }
            }
            result += ")\n";
            return result;

        }
        private string genFakeData(Column r)
        {
            string result = "";

            if (r.data_type.toCSharpDataType().Equals("string"))
            {
                result =  "\"" + generateRandomString(r, 8 - r.length) + "\"";

                if (r.default_value.ToLower().Contains("uuid"))
                {
                    result = "\"" + generateRandomString(r, 0) + "\"";

                }

                

            }
            else if (r.data_type.toCSharpDataType().Equals("bool"))
            {
                int flip = rand.Next(0, 2);
                if (flip == 0)
                {
                    result = "True";
                }
                else
                {
                    result = "False";
                }
            }
            else if (r.data_type.toCSharpDataType().Equals("int"))
            {
                result = rand.Next(10, 70).ToString();
            }
            else if (r.data_type.Equals("decimal"))
            {
                double toAdd = rand.Next(1000, 7000) / 100d;
                result = toAdd.ToString();
            }
            else
            {
                result += "";
            }
            return result;
        }

        private string genServletJavaDoc(ServletType type) {
            string result = "";
            switch (type)
            {
                case ServletType.UploadServlet:
                    result = uploadServletJavaDoc();
                    break;
                case ServletType.DeleteServlet:
                    result = deleteServletJavaDoc();
                    break;
                case ServletType.ExportServlet:
                    result = exportServletJavaDoc();
                    break;
                case ServletType.CreateServlet:
                    result = createServletJavaDoc();
                    break;
                case ServletType.ViewAllSErvlet:
                    result = viewAllServletJavaDoc();
                    break;
                case ServletType.ViewEditSErvlet:
                    result = viewEditServletJavaDoc();
                    break;
            }

            return result;

        }
        private string uploadServletJavaDoc() {
            string result = "/**\n";
            string header = " * Servlet implementation class Up;oad"+name+"Servlet" +"\n*\n";
            header += "* <p>This servlet handles the addition of new {@link "+name+"} entries in the application.\n";
            header += " * It is mapped to the URL pattern <code>/upload" + name + "</code>.</p>\n*\n";
            string doGet = "* <p><strong>HTTP GET</strong>: Renders the form for adding a new "+name.ToLower()+". Access is restricted\n";
            doGet += " * to users with the \"User\" role. If unauthorized, the user is redirected to the login page.</p>\n*\n";
            string doPost = "* <p><strong>HTTP POST</strong>: Processes form submissions for adding a new "+name.ToLower()+". Validates input,\n";
            doPost += " * attempts insertion into the database via {@link "+name+"DAO}, and forwards back to the form view with\r\n";
            doPost += " * success or error messages as necessary. Successful insertions redirect to the list of all " + name.ToLower() + "s.</p>\r\n*\n";
            string access = " * <p>Access to this servlet requires that the user session contain a {@link User} object that is logged in\n";
            access += " * appropriate roles set (specifically the \"User\" role).</p>\n*\n";
            string created = " * <p>Created by Jonathan Beck on "+DateTime.Now.ToShortDateString()+"</p>\n*\n";
            string see = " * @author Jonathan Beck\n";
            see += " * @version 1.0\n";
            see += " * @see com."+settings.owner_name+"."+settings.database_name+".models."+name+"\n";
            see += " * @see com."+settings.owner_name+"."+settings.database_name+".data."+name+"DAO\n";
            see += " * @see jakarta.servlet.http.HttpServlet\n";
            result += header + doGet + doPost + access + created + see;
            result += "*/\n";
            return result; 
        }
        private string deleteServletJavaDoc()
        {
            string result = "/**\n";
            string header = " * Servlet implementation class Add" + name + "Servlet" + "\n*\n";
            header += "* <p>This servlet handles the deletion of  {@link " + name + "} entries in the application.\n";
            header += " * It is mapped to the URL pattern <code>/delete" + name + "</code>.</p>\n*\n";
            string doGet = " * <p><strong>HTTP GET</strong>: handles the request for deleting a"+name+ " via {@link "+name+"DAO}. Access is restricted\r\n " +
                "* to users with the \"User\" role. If unauthorized, the user is redirected to the login page.</p>";
            string doPost = "";
            string access = " * <p>Access to this servlet requires that the user session contain a {@link User} object that is logged in\n";
            access += " * appropriate roles set (specifically the \"User\" role).</p>\n*\n";
            string created = " * <p>Created by Jonathan Beck on " + DateTime.Now.ToShortDateString() + "</p>\n*\n";
            string see = " * @author Jonathan Beck\n";
            see += " * @version 1.0\n";
            see += " * @see com." + settings.owner_name + "." + settings.database_name + ".models." + name + "\n";
            see += " * @see com." + settings.owner_name + "." + settings.database_name + ".data." + name + "DAO\n";
            see += " * @see jakarta.servlet.http.HttpServlet\n";
            result += header + doGet + doPost + access + created + see;
            result += "*/\n";
            return result;
        }
        private string exportServletJavaDoc()
        {
            string result = "/**\n";
            string header = " * Servlet implementation class Add" + name + "Servlet" + "\n*\n";
            header += "* <p>This servlet handles the exporting of {@link " + name + "} entries in the application to csv.\n";
            header += " * It is mapped to the URL pattern <code>/export" + name + "</code>.</p>\n*\n";
            string doGet = "* <p><strong>HTTP GET</strong>: Creates a txt file of all  " + name.ToLower() + "s via {@link "+name+"DAO} and allows the users to download. Access is restricted\n";
            doGet += " * to users with the \"User\" role. If unauthorized, the user is redirected to the login page.</p>\n*\n";
            string doPost = "*\n";
            string access = " * <p>Access to this servlet requires that the user session contain a {@link User} object that is logged in\n";
            access += " * appropriate roles set (specifically the \"User\" role).</p>\n*\n";
            string created = " * <p>Created by Jonathan Beck on " + DateTime.Now.ToShortDateString() + "</p>\n*\n";
            string see = " * @author Jonathan Beck\n";
            see += " * @version 1.0\n";
            see += " * @see com." + settings.owner_name + "." + settings.database_name + ".models." + name + "\n";
            see += " * @see com." + settings.owner_name + "." + settings.database_name + ".data." + name + "DAO\n";
            see += " * @see jakarta.servlet.http.HttpServlet\n";
            result += header + doGet + doPost + access + created + see;
            result += "*/\n";
            return result;
        }
        private string createServletJavaDoc()
        {
            string result = "/**\n";
            string header = " * Servlet implementation class Add" + name + "Servlet" + "\n*\n";
            header += "* <p>This servlet handles the addition of new {@link " + name + "} entries in the application.\n";
            header += " * It is mapped to the URL pattern <code>/add" + name + "</code>.</p>\n*\n";
            string doGet = " * <p><strong>HTTP GET</strong>: Renders the form for adding a new "+name+". Access is restricted\r\n" +
                " * to users with the \"User\" role. If unauthorized, the user is redirected to the login page.</p>\n";
            string doPost = " * <p><strong>HTTP POST</strong>: Processes form submissions for adding a new "+name+". Validates input,\r\n" +
                " * attempts insertion into the database via {@link "+name+"DAO}, and forwards back to the form view with\r\n " +
                "* success or error messages as necessary. Successful insertions redirect to the list of all "+name+".</p\n>";
            string access = " * <p>Access to this servlet requires that the user session contain a {@link User} object that is logged in\n";
            access += " * appropriate roles set (specifically the \"User\" role).</p>\n*\n";
            string created = " * <p>Created by Jonathan Beck on " + DateTime.Now.ToShortDateString() + "</p>\n*\n";
            string see = " * @author Jonathan Beck\n";
            see += " * @version 1.0\n";
            see += " * @see com." + settings.owner_name + "." + settings.database_name + ".models." + name + "\n";
            see += " * @see com." + settings.owner_name + "." + settings.database_name + ".data." + name + "DAO\n";
            see += " * @see jakarta.servlet.http.HttpServlet\n";
            result += header + doGet + doPost + access + created + see;
            result += "*/\n";
            return result;
        }
        private string viewAllServletJavaDoc()
        {
            string result = "/**\n";
            string header = " * Servlet implementation class All" + name + "Servlet" + "\n*\n";
            header += "* <p>This servlet handles the viewing of all {@link " + name + "} entries in the application.\n";
            header += " * It is mapped to the URL pattern <code>/all-" + name + "s</code>.</p>\n*\n";
            string doGet = " * <p><strong>HTTP GET</strong>: Renders the table for viewing all " + name + "s from the database via {@link "+name+"DAO}. Allows earching and filtering by foreign keys. Access is restricted\r\n" +
                " * to users with the \"User\" role. If unauthorized, the user is redirected to the login page.</p>";
            string doPost = "*\n";
            string access = " * <p>Access to this servlet requires that the user session contain a {@link User} object that is logged in\n";
            access += " * appropriate roles set (specifically the \"User\" role).</p>\n*\n";
            string created = " * <p>Created by Jonathan Beck on " + DateTime.Now.ToShortDateString() + "</p>\n*\n";
            string see = " * @author Jonathan Beck\n";
            see += " * @version 1.0\n";
            see += " * @see com." + settings.owner_name + "." + settings.database_name + ".models." + name + "\n";
            see += " * @see com." + settings.owner_name + "." + settings.database_name + ".data." + name + "DAO\n";
            see += " * @see jakarta.servlet.http.HttpServlet\n";
            result += header + doGet + doPost + access + created + see;
            result += "*/\n";
            return result;
        }
        private string viewEditServletJavaDoc()
        {
            int x = 0;
            string result = "/**\n";
            string header = " * Servlet implementation class Edit" + name + "Servlet" + "\n*\n";
            header += "* <p>This servlet handles the editing of {@link " + name + "} entries in the application.\n";
            header += " * It is mapped to the URL pattern <code>/edit" + name + "</code>.</p>\n*\n";
            string doGet = " * <p><strong>HTTP GET</strong>: Renders the table for viewing a single "+name+"s. Allows editing of appropriate fields. Access is restricted\r\n" +
                " * to users with the \"User\" role. If unauthorized, the user is redirected to the login page.</p>\n";
            string doPost = " * <p><strong>HTTP POST</strong>: Processes form submissions for editing a " + name + ". Validates input,\r\n" +
                " * attempts update of the the database via {@link " + name + "DAO}, and forwards back to the form view with\r\n " +
                "* success or error messages as necessary. Successful updates redirect to the list of all " + name + ".</p\n>";
            string access = " * <p>Access to this servlet requires that the user session contain a {@link User} object that is logged in\n";
            access += " * appropriate roles set (specifically the \"User\" role).</p>\n*\n";
            string created = " * <p>Created by Jonathan Beck on " + DateTime.Now.ToShortDateString() + "</p>\n*\n";
            string see = " * @author Jonathan Beck\n";
            see += " * @version 1.0\n";
            see += " * @see com." + settings.owner_name + "." + settings.database_name + ".models." + name + "\n";
            see += " * @see com." + settings.owner_name + "." + settings.database_name + ".data." + name + "DAO\n";
            see += " * @see jakarta.servlet.http.HttpServlet\n";
            result += header + doGet + doPost + access + created + see;
            result += "*/\n";
            return result;
        }
        public string genUseCases(int outerLoop)
        {

            string result = "";
            int innerLoop = 1;
            foreach (UseCaseType type in Enum.GetValues(typeof(UseCaseType)))
            {
                result += genUsseCase(type, outerLoop + 1, innerLoop);
                innerLoop++;
            }
            return result;
        }
        public string genUsseCase(UseCaseType type, int outerLoop, int innerLoop)
        {
            string result = "";
            string header = "<h2>UC" + outerLoop + "-" + innerLoop + " :" + type + " " + name + "</h2><br/>\n";
            string description = "<p><b>Description</b> A user will be able to " + type + " " + name + " objects. <p><br/>\n";
            string actors = "<p><b>Actors </b> The primary User </p><br/>";
            string preConditions = "<b>Preconditions</b> <ul>\n";
            preConditions += "<li>the user is logged in</li>\n";
            switch (type)
            {
                case UseCaseType.ActivateThing:
                    preConditions += "<li>Some " + name + "s have been added to the database</li>\n";
                    preConditions += "<li>Some " + name + " have already been deactiavted</li>\n";
                    break;
                case UseCaseType.DeactivateThing:
                    preConditions += "<li>Some " + name + "s have been added to the database</li>\n";
                    preConditions += "<li>Some " + name + " are currently active</li>\n";
                    break;
                case UseCaseType.createThing:
                    preConditions += "";
                    break;
                case UseCaseType.UpdateThing:
                    preConditions += "<li>Some " + name + "s have been added to the database</li>\n";
                    preConditions += "<li>Some " + name + " are currently active</li>\n";
                    break;
                case UseCaseType.SearchThing:
                    preConditions += "<li>Some " + name + "s have been added to the database</li>\n";
                    preConditions += "<li>Some " + name + " are currently active</li>\n";
                    break;
                case UseCaseType.RetrieveOneThing:
                    preConditions += "<li>Some " + name + "s have been added to the database</li>\n";
                    preConditions += "<li>Some " + name + " are currently active</li>\n";
                    break;
                case UseCaseType.RetrieveAllThing:
                    preConditions += "<li>Some " + name + "s have been added to the database</li>\n";
                    preConditions += "<li>Some " + name + " are currently active</li>\n";
                    break;
                case UseCaseType.FilterThing:
                    preConditions += "<li>Some " + name + "s have been added to the database</li>\n";
                    preConditions += "<li>Some " + name + " are currently active</li>\n";
                    break;
                case UseCaseType.DeleteThing:
                    preConditions += "<li>Some " + name + "s have been added to the database</li>\n";
                    preConditions += "<li>Some " + name + " are currently active</li>\n";
                    break;
            }
            preConditions += "</ul>\n";
            string postConditions = "<b>Postconditions</b> <ul>\n";
            postConditions += "";
            switch (type)
            {
                case UseCaseType.ActivateThing:
                    postConditions += "<li>The selected " + name + " will be activated</li>\n";

                    break;
                case UseCaseType.DeactivateThing:
                    postConditions += "<li>The selected \"+name+ \" will be deactivated</li>\n";
                    break;
                case UseCaseType.createThing:
                    postConditions += "<li>A " + name + " object will be added to the database</li>\n";
                    break;
                case UseCaseType.UpdateThing:
                    postConditions += "<li>A record of a " + name + " object will be updated</li>\n";
                    break;
                case UseCaseType.SearchThing:
                    postConditions += "<li>A list of " + name + " objects will be displayed that match the criteria.</li>\n";
                    break;
                case UseCaseType.RetrieveOneThing:
                    postConditions += "<li>The details of a single " + name + " object will be displayed</li>\n";
                    break;
                case UseCaseType.RetrieveAllThing:
                    postConditions += "<li>A list of " + name + " objects will be displayed.</li>\n";
                    break;
                case UseCaseType.FilterThing:
                    postConditions += "<li>A list of " + name + " objects will be displayed that match the criteria</li>\n";
                    break;
                case UseCaseType.DeleteThing:
                    postConditions += "<li>The selected "+name+ " will be deleted</li>\n";
                    break;
            }
            postConditions += "</ul>";
            string steps = "<b>Steps:</b><ul>\n";
            steps += "<li>The User will click the " + name + " Management icon on the top of the page</li>\n";
            switch (type)
            {
                case UseCaseType.ActivateThing:
                    steps += "<li>The user will click a button to view a list of deactived " + name + "s</li>\n";
                    steps += "<li>The user will click \"activate\" on the appropriate record</li>\n";
                    break;
                case UseCaseType.DeactivateThing:
                    steps += "<li>The user will click \"deactivate\" on the appropriate record</li>\n";
                    break;
                case UseCaseType.createThing:
                    steps += "<li>The user will click the Add New " + name + " button</li>\n";
                    steps += "<li>The user will insert values into fields as needed</li>\n";
                    steps += "<li>The user will click save.</li>\n";
                    break;
                case UseCaseType.UpdateThing:
                    steps += "<li>The user will click on the \"view\" icon next to the appropriate record</li>\n";
                    steps += "<li>A detailed view of the record will appear</li>\n";
                    steps += "<li>The user will update fields as needed</li>\n";
                    steps += "<li>The user will click save.</li>\n";
                    break;
                case UseCaseType.SearchThing:
                    steps += "<li>The user will type in the search box on the page, then press enter</li>\n";
                    steps += "<li>A List of records matching the search criteria will display</li>\n";
                    break;
                case UseCaseType.RetrieveOneThing:
                    steps += "<li>The user will click on the \"view\" icon next to the appropriate record</li>\n";
                    steps += "<li>A detailed view of the record will appear</li>\n";
                    break;
                case UseCaseType.RetrieveAllThing:

                    break;
                case UseCaseType.FilterThing:
                    steps += "<li>The user will select from the drop downs on the top of the page, then press enter</li>\n";
                    steps += "<li>A List of records matching the filter criteria will display</li>\n";
                    break;
                case UseCaseType.DeleteThing:
                    steps += "<li>The user will click \"deactivate\" on the appropriate record</li>\n";
                    steps += "<li>A warning box will appear, and the user will click the appropriate decision</li>\n";
                    break;
            }
            steps += "</ul>\n";

            string alternateFlow = "<b>Alternate Flows</b><ul>\n";
            alternateFlow += "<li><b>Database Unavailable:</b> If the database is unavailable, an error message will be displayed  " +
                "the user will be prompted to try again later.</li>\n";
            switch (type)
            {
                case UseCaseType.createThing:
                    alternateFlow += "<li><b>Invalid Data: </b> If invalid data is supplied, the form will be reloaded with error messages</li>\n";
                    break;
                case UseCaseType.RetrieveOneThing:
                    alternateFlow += "<li><b>Invalid Record: </b> If an invalid record is invoked, the list will be reloaded with error messages</li>\n";
                    break;
                case UseCaseType.RetrieveAllThing:

                    break;
                case UseCaseType.FilterThing:
                    alternateFlow += "<li><b>Zero Results:</b>If zero records match the filter, no records will be displayed, alongside an error message</li>\n";
                    break;
                case UseCaseType.SearchThing:
                    alternateFlow += "<li><b>Zero Results:</b>If zero records match the filter, no records will be displayed, alongside an error message</li>\n";
                    break;
                case UseCaseType.UpdateThing:
                    alternateFlow += "<li><b>Invalid Record: </b> If an invalid record is invoked, the list will be reloaded with error messages</li>\n";
                    alternateFlow += "<li><b>Invalid Data: </b> If invalid data is supplied, the form will be reloaded with error messages</li>\n";

                    break;
                case UseCaseType.DeleteThing:
                    alternateFlow += "<li><b>Invalid Record: </b> If an invalid record is invoked, the list will be reloaded with error messages</li>\n";

                    break;
                case UseCaseType.DeactivateThing:
                    alternateFlow += "<li><b>Invalid Record: </b> If an invalid record is invoked, the list will be reloaded with error messages</li>\n";

                    break;
                case UseCaseType.ActivateThing:
                    alternateFlow += "<li><b>Invalid Record: </b> If an invalid record is invoked, the list will be reloaded with error messages</li>\n";

                    break;
            }
            alternateFlow += "</ul>\n";

            result += header + description + actors + preConditions + postConditions + steps + alternateFlow;
            return result;
        }
    }
}

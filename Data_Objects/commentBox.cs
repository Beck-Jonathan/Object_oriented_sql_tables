﻿using appData2;
using System;

namespace Data_Objects
{
    public class commentBox
    {
        public static String GenXMLComment(XMLType type, table table, XML_Method_Type method)
        {
            string result = "";
            if (type.Equals(XMLType.Class))
            {

            }
            else if (type.Equals(XMLType.Method))
            {
                String summary = "";
                String name = "Created By Jonathan Beck " + DateTime.Now.ToString();
                String Params = "";
                String returns = "";
                    
                switch (method)
                {
                    case XML_Method_Type.CSharp_Manager_Add:
                        summary = "Logic layer add method for" + table.name + " objects";
                        Params = "";
                        returns = "bool. True if added, False otherwise.";
                        break;
                    case XML_Method_Type.CSharp_Manager_Delete:
                        summary = "Logic layer delete method for" + table.name + " objects";
                        Params = "";
                        returns = "Int, number of records deleted"; 
                        break; 
                    case XML_Method_Type.CSharp_Manager_Undelete:
                        summary = "Logic layer undelete method for" + table.name + " objects";
                        Params = "";
                        returns = "Int, number of records restored";
                        break;
                    case XML_Method_Type.CSharp_Manager_Retreive_By_PK:
                        summary = "Logic layer retreive by pk method for" + table.name + " objects";
                        Params = "";
                        returns = "<see cref=\""+table.name+"\"";
                        break;
                    case XML_Method_Type.CSharp_Manager_Retreive_All_No_Param:
                        summary = "Logic layer retreive all method for" + table.name + " objects";
                        Params = "";
                        returns = "List of <see cref=\"" + table.name + "\"";
                        break;
                    case XML_Method_Type.CSharp_Manager_Retreive_All_One_Param:
                        summary = "Logic layer retreive all method for" + table.name + " objects";
                        Params = "";
                        returns = "List of <see cref=\"" + table.name + "\"";
                        break;
                    case XML_Method_Type.CSharp_Manager_Retreive_All_Two_Param:
                        summary = "Logic layer retreive all method for" + table.name + " objects";
                        Params = "";
                        returns = "List of <see cref=\"" + table.name + "\"";
                        break;
                    case XML_Method_Type.CSharp_Manager_Update:
                        summary = "Logic layer update method for" + table.name + " objects";
                        Params = "";
                        returns = "Int, number of records updated";
                        break;
                    case XML_Method_Type.CSharp_Manager_Retreive_By_FK_No_Param:
                        summary = "Logic layer retrevei by fk method for" + table.name + " objects";
                        Params = "";
                        returns = "List of <see cref=\"" + table.name + "\"";
                        break;
                    case XML_Method_Type.CSharp_Manager_Retreive_By_FK_One_Param:
                        summary = "Logic layer retrevei by fk method for" + table.name + " objects";
                        Params = "";
                        returns = "List of <see cref=\"" + table.name + "\"";
                        break; 
                    case XML_Method_Type.CSharp_Manager_Retreive_By_FK_Two_Param:
                        summary = "Logic layer retrevei by fk method for" + table.name + " objects";
                        Params = "";
                        returns = "List of <see cref=\"" + table.name + "\"";
                        break; 
                    case XML_Method_Type.CSharp_Accessor_Add:
                        summary = "Data Access layer Add method for " + table.name + " objects";
                        Params = "";
                        returns = "Int, number of records added";
                        break; 
                    case XML_Method_Type.CSharp_Accessor_Delete:
                        summary = "Data Access layer Delete method for " + table.name + " objects";
                        Params = "";
                        returns = "Int, number of records deleted";
                        break;
                    case XML_Method_Type.CSharp_Accessor_Undelete:
                        summary = "Data Access layer undelete method for " + table.name + " objects";
                        Params = "";
                        returns = "Int, number of records restored";
                        break;
                    case XML_Method_Type.CSharp_Accessor_Retreive_By_PK:
                        summary = "Data Access layer retreive by PK method for " + table.name + " objects";
                        Params = "";
                        returns = "<see cref=\"" + table.name + "\"";
                        break;
                    case XML_Method_Type.CSharp_Accessor_Retreive_All_Two_Param:
                        summary = "Data Access layer Retreive All method for " + table.name + " objects";
                        Params = "";
                        returns = "List of <see cref=\"" + table.name + "\"";
                        break;
                        
                    case XML_Method_Type.CSharp_Accessor_Update:
                        summary = "Data Access layer Update method for " + table.name + " objects";
                        Params = "";
                        returns = "Int, number of records updated";
                        break;
                    case XML_Method_Type.CSharp_Accessor_Retreive_By_FK_Two_Param:
                        summary = "Data Access layer Retreive by FK method for " + table.name + " objects";
                        Params = "";
                        returns = "List of <see cref=\"" + table.name + "\"";
                        break;
                }
                summary = "<summary>\n" + summary +"\n"+name +"\n</summary>";
                returns = "<returns>" + returns + "</returns";
                result = result + summary + Params + returns;
            }
            else
            {
                return result;
            }
            return result;
        }







        //creates various commeent boxes based on
        //table name and function type.
        public static String genCommentBox(String table, Component_Enum type)
        {
            String full_comment_box = " ";
            String start_stars = "/******************\n";
            String end_stars = "\n***************/\n";
            String middle = " ";
            String PrintStatement = "";
            if (type.Equals(Component_Enum.SQL_table))
            {
                middle = "Create the " + table + " table";

            }
            if (type.Equals(Component_Enum.SQL_Audit_Table))
            {
                middle = "Create the " + table + " Audit table";
            }
            if (type.Equals(Component_Enum.SQL_Update))
            {
                middle = "Create the update script for the " + table + " table";
            }
            if (type.Equals(Component_Enum.SQL_Delete))
            {
                middle = "Create the delete script for the " + table + " table";
            }
            if (type.Equals(Component_Enum.SQL_Retreive_By_PK))
            {
                middle = "Create the retreive by key script for the " + table + " table";
            }

            if (type.Equals(Component_Enum.SQL_Retreive_By_All))
            {
                middle = "Create the retreive by all script for the " + table + " table";
            }
            if (type.Equals(Component_Enum.SQL_Insert))
            {
                middle = "Create the insert script for the " + table + " table";
            }
            if (type.Equals(Component_Enum.SQL_Insert_Trigger))
            {
                middle = "Create the insert trigger script for the " + table + " table";
            }
            if (type.Equals(Component_Enum.SQL_Update_Trigger))
            {
                middle = "Create the update trigger script for the " + table + " table";
            }

            if (type.Equals(Component_Enum.SQL_Delete_Trigger))
            {
                middle = "Create the delete trigger script for the " + table + " table";
            }

            if (type.Equals(Component_Enum.CSharp_IAccessor))
            {
                middle = "Create the I Accessor for the " + table + " table";
            }

            if (type.Equals(Component_Enum.CSharp_IManager))
            {
                middle = "create the I manager for the " + table + " table;";

            }

            if  (type.Equals(Component_Enum.CSharp_Accessor)) { 
                middle = "create the Accessor for the " + table + " table"; 
            }

            if (type.Equals(Component_Enum.CSharp_Manager)) {
                middle = "create the manager for the " + table + " table"; 
            }
            if (type.Equals(Component_Enum.CSharp_DataObject)) {
                middle = "create the data object for the " + table + " table";
            }
            if (type.Equals(Component_Enum.CSharp_XAML_Window)) { 
                middle = "create the XAML Window for the " + table + " table";
            }
            if (type.Equals(Component_Enum.CSharp_Window_Control)) {
                middle = "create the c# Window for the " + table + " table";
            }
            if (type.Equals(Component_Enum.SQL_Sample_Data)) {
                middle = "Insert Sample Data For The  " + table + " table"; 
            }
            if (type.Equals(Component_Enum.Java_JSP_Add)) {
                start_stars = "<%--************\n";
                middle = "Create the JSP  For adding to The  " + table + " table";
                end_stars = "\n**********--%>\n";
            }
            if (type.Equals(Component_Enum.Java_JSP_ViewAll)) {
                start_stars = "<%--************\n";
                middle = "Create the JSP  For Viewing All of The  " + table + " table";
                end_stars = "\n**********--%>\n";
            }
            if (type.Equals(Component_Enum.Java_Servlet_Add)) {
                middle = "Create the Servlet  For adding to The  " + table + " table";
            }
            if (type.Equals(Component_Enum.Java_Servlet_ViewAll)) { 
                middle = "Create the Servlet  For Viewing all of the  " + table + " table"; 
            }
            if (type.Equals(Component_Enum.SQL_Undelete))
            {
                middle = "Create the undelete script for the " + table + " table";
            }
            if (type.Equals(Component_Enum.SQL_Retreive_Active)) {
                middle = "Create The Retreive_By_Active script for the " + table + " table";
            }
            if (type.Equals(Component_Enum.Java_Servlet_Delete))
            {
                middle = "Create the Servlet For Deleteing from the " + table + " table";
            }
            if (type.Equals(Component_Enum.Java_JSP_ViewEdit))
            {
                
                start_stars = "<%--************\n";
                middle = "Create the JSP For Viuw/Edit from the " + table + " table";
                end_stars = "\n**********--%>\n";
            }
            if (type.Equals(Component_Enum.Java_Servlet_ViewEdit))
            {
                middle = "Create the Servlet Viuw/Edit from the " + table + " table";
            }
            if (type.Equals(Component_Enum.SQL_Select_Distinct))
            {
                middle = "Create the Select Disctint for Drop downs from the " + table + " table";
            }
            if (type.Equals(Component_Enum.CSharp_Manager))
            {
                middle = "Create the Logic Layer Manager for the " + table + " table";
            }
            if (type.Equals(Component_Enum.CSharp_Manager_Add))
            {
                middle = "Create the Logic Layer Add method for the  " + table + " table";
            }
            if (type.Equals(Component_Enum.CSharp_Manager_Delete))
            {
                middle = "Create the Logic Layer Delete method for the " + table + " table";
            }
            if (type.Equals(Component_Enum.CSharp_Manager_Undelete))
            {
                middle = "Create the Logic Layer Undelete method for the " + table + " table";
            }
            if (type.Equals(Component_Enum.CSharp_Manager_Retreive_By_PK))
            {
                middle = "Create the Logic Layer Retreive by Primary Key method for the" + table + " table";
            }
            if (type.Equals(Component_Enum.CSharp_Manager_Retreive_All_No_Param))
            {
                middle = "Create the Logic Layer Retreive all method for the the " + table + " table";
                middle += "\n with no paramater supplied.";
            }
            if (type.Equals(Component_Enum.CSharp_Manager_Retreive_All_One_Param))
            {
                middle = "Create the Logic Layer Retreive all method for the the " + table + " table";
                middle += "\n with one paramater supplied.";
            }
            if (type.Equals(Component_Enum.CSharp_Manager_Retreive_All_Two_Param))
            {
                middle = "Create the Logic Layer Retreive all method for the the " + table + " table";
                middle += "\n with two paramater supplied.";
            }
            if (type.Equals(Component_Enum.CSharp_Manager_Update))
            {
                middle = "Create the Logic Layer Update method for the " + table + " table";
            }

            if (type.Equals(Component_Enum.CSharp_Manager_Retreive_By_FK_No_Param))
            {
                middle = "Create the Logic Layer retreive by FK method for the " + table + " table";
                middle += "\n with no paramater supplied.";
            }
            if (type.Equals(Component_Enum.CSharp_Manager_Retreive_By_FK_One_Param))
            {
                middle = "Create the Logic Layer retreive by FK method for the " + table + " table";
                middle += "\n with one paramater supplied.";
            }
            if (type.Equals(Component_Enum.CSharp_Manager_Retreive_By_FK_Two_Param))
            {
                middle = "Create the Logic Layer retreive by FK method for the " + table + " table";
                middle += "\n with two paramater supplied.";
            }


            middle += "\n Created By Jonathan Beck " + DateTime.Now.ToShortDateString() ;
            PrintStatement = "print '' Print '***" + middle + "***' \n go \n";


            full_comment_box = start_stars + middle + end_stars;
            if (settings.TSQLMode) { full_comment_box = full_comment_box + PrintStatement; }
            return full_comment_box;
        }
    }
}


using appData2;
using System;

namespace Data_Objects
{
    public class commentBox
    {
        public static String GenJavaDocComment(int type, string tableName)
        {
            string result = "";
            string layerText = "";

            String methodComment = "/// <summary>" +
             "///A method that ________________\n" +
             "///_____________.\n" +
             "///</ summary >\n" +
             "///< param name = \"a\" > \n" +
             "///____________\n" +
             "///</ param >\n" +
             "///< returns >\n" +
             "///< see cref = \"int\" > int </ see >: ____________.\n" +
             "///</ returns >\n" +
             "///< remarks >\n" +
             "///Parameters:\n" +
             "///< br />\n" +
             "///< see cref = \"int\" > int </ see > a: _______________" +
             "///< br />< br />\n" +
             "///Exceptions:\n" +
             "///< br /> \n" +
             "///< see cref = \"ArgumentOutOfRangeException\" > ArgumentOutOfRangeException </ see >:____________.\n" +
             "///< br />< br /> \n" +
             "///CONTRIBUTOR: Jonathan Beck \n" +
             "///< br /> \n" +
             "///CREATED: " + DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year + "\n" +
             "///< br />< br />\n";

            String classComment = " /// <summary>\n" +
     "///AUTHOR: Jonathan Beck\n" +
     "///<br />\n" +
     "///CREATED: " + DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year + "\n" +
     "///< br />\n" +
      "///An example class to show how code is expected to be written and documented.\n" +
      "///This is where a description of what your file is supposed to contain goes.\n" +
       "///e.g., \"Class with helper methods for input validation.\",\n" +
     "///Class that defines " + tableName + " Objects.\n" +
     "///</summary>\n" +

    "///< remarks>\n" +
    "///UPDATER: updater_name\n" +
     "///< br />\n" +
     "/// UPDATED: yyyy-MM-dd \n" +
     "/// < br />\n" +
     "/// Update comments go here, include method or methods were changed or added\n" +
       " /// A new remark should be added for each update.\n" +
     "///</remarks>\n";

            switch (type)
            {
                case 0:; return methodComment;
                case 1:; return classComment;

                default: return "";

            }




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


using appData2;
using System;

namespace Data_Objects
{
    class comment_box_gen
    {
        public static String JavaDocComment(int type, string tableName)
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
        public static String comment_box(String table, int type)
        {
            String full_comment_box = " ";
            String start_stars = "/******************\n";
            String end_stars = "\n***************/\n";
            String middle = " ";
            String PrintStatement = "";
            if (type == 1)
            {
                middle = "Create the " + table + " table";

            }
            if (type == 2)
            {
                middle = "Create the " + table + " Audit table";
            }
            if (type == 3)
            {
                middle = "Create the update script for the " + table + " table";
            }
            if (type == 4)
            {
                middle = "Create the delete script for the " + table + " table";
            }
            if (type == 5)
            {
                middle = "Create the retreive by key script for the " + table + " table";
            }

            if (type == 6)
            {
                middle = "Create the retreive by all script for the " + table + " table";
            }
            if (type == 7)
            {
                middle = "Create the insert script for the " + table + " table";
            }
            if (type == 8)
            {
                middle = "Create the insert trigger script for the " + table + " table";
            }
            if (type == 9)
            {
                middle = "Create the update trigger script for the " + table + " table";
            }

            if (type == 10)
            {
                middle = "Create the delete trigger script for the " + table + " table";
            }

            if (type == 11)
            {
                middle = "Create the I Accessor for the " + table + " table";
            }

            if (type == 12)
            {
                middle = "create the I manager for the " + table + " table;";

            }

            if (type == 13) { middle = "create the Accessor for the " + table + " table"; }

            if (type == 14) { middle = "create the manager for the " + table + " table"; }
            if (type == 15) { middle = "create the data object for the " + table + " table"; }
            if (type == 16) { middle = "create the XAML Window for the " + table + " table"; }
            if (type == 17) { middle = "create the c# Window for the " + table + " table"; }
            if (type == 18) { middle = "Insert Sample Data For The  " + table + " table"; }
            if (type == 19) {
                start_stars = "<%--************\n";
                middle = "Create the JSP  For adding to The  " + table + " table";
                end_stars = "\n**********--%>\n";
            }
            if (type == 20) {
                start_stars = "<%--************\n";
                middle = "Create the JSP  For Viewing All of The  " + table + " table";
                end_stars = "\n**********--%>\n";
            }
            if (type == 21) { middle = "Create the Servlet  For adding to The  " + table + " table"; }
            if (type == 22) { middle = "Create the Servlet  For Viewing all of the  " + table + " table"; }
            if (type == 23)
            {
                middle = "Create the undelete script for the " + table + " table";
            }
            if (type == 24) {
                middle = "Create The Retreive_By_Active script for the " + table + " table";
            }
            if (type == 25)
            {
                middle = "Create the Servlet For Deleteing from the " + table + " table";
            }
            if (type == 26)
            {
                
                start_stars = "<%--************\n";
                middle = "Create the JSP For Viuw/Edit from the " + table + " table";
                end_stars = "\n**********--%>\n";
            }
            if (type == 27)
            {
                middle = "Create the Servlet Viuw/Edit from the " + table + " table";
            }
            if (type == 28)
            {
                middle = "Create the Select Disctint for Drop downs from the " + table + " table";
            }
            if (type == 29)
            {
                middle = "Create the Logic Layer Manager for the " + table + " table";
            }
            if (type == 30)
            {
                middle = "Create the Logic Layer Add method for the  " + table + " table";
            }
            if (type == 31)
            {
                middle = "Create the Logic Layer Delete method for the " + table + " table";
            }
            if (type == 32)
            {
                middle = "Create the Logic Layer Undelete method for the " + table + " table";
            }
            if (type == 33)
            {
                middle = "Create the Logic Layer Retreive by Primary Key method for the" + table + " table";
            }
            if (type == 34)
            {
                middle = "Create the Logic Layer Retreive all method for the the " + table + " table";
            }
            if (type == 35)
            {
                middle = "Create the Logic Layer Update method for the " + table + " table";
            }

            if (type == 36)
            {
                middle = "Create the Logic Layer retreive by FK method for the " + table + " table";
            }





            middle += "\n Created By Jonathan Beck " + DateTime.Now.ToShortDateString() ;
            PrintStatement = "print '' Print '***" + middle + "***' \n go \n";


            full_comment_box = start_stars + middle + end_stars;
            //if (settings.TSQLMode) { full_comment_box = full_comment_box + PrintStatement; }
            return full_comment_box;
        }
    }
}


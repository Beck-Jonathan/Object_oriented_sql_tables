using appData2;
using System.Collections.Generic;
using System.Linq;
namespace Data_Objects
{
    public class data_tables
    {
        //holds all data tables that were created
        public static List<table> all_tables = new List<table>();
        public static List<foreignKey> all_foreignKey = new List<foreignKey>();
        public static string analyzeRelationships()
        {
            string result = "";
            Dictionary<string, int> ForeignKeyCount = new Dictionary<string, int>();
            Dictionary<string, int> KeyedToCount = new Dictionary<string, int>();
            Dictionary<string, int> TypoKeys = new Dictionary<string, int>();
            int tableNameOffset = 0;
            if (settings.TSQLMode) { tableNameOffset = 4; }
            foreach (table t in all_tables)
            {
                ForeignKeyCount[t.name.bracketStrip().Substring(tableNameOffset).ToLower()] = 0;
                KeyedToCount[t.name.bracketStrip().Substring(tableNameOffset).ToLower()] = 0;
            }
            foreach (foreignKey t in all_foreignKey)
            {
                string main = t.mainTable.ToLower();
                string foreign = t.referenceTable.ToLower();
                if (ForeignKeyCount.Keys.Contains(main))
                {
                    ForeignKeyCount[main] = ForeignKeyCount[main] + 1;
                }
                else
                {
                    TypoKeys[main] = 0;
                }
                if (KeyedToCount.Keys.Contains(foreign))
                {
                    KeyedToCount[foreign] = KeyedToCount[foreign] + 1;
                }
                else { TypoKeys[foreign] = 0; }
            }
            int foreignmax = 0;
            int keyedtomax = 0;
            foreach (string key in ForeignKeyCount.Keys)
            {
                if (ForeignKeyCount[key] > foreignmax)
                {
                    foreignmax = ForeignKeyCount[key];
                }
            }
            foreach (string key in KeyedToCount.Keys)
            {
                if (KeyedToCount[key] > keyedtomax)
                {
                    keyedtomax = KeyedToCount[key];
                }
            }
            for (int i = 0; i <= foreignmax; i++)
            {
                foreach (string key in ForeignKeyCount.Keys)
                {
                    if (ForeignKeyCount[key] == i)
                    {
                        result = result + "The table " + key + " references " + i + " other tables\n";
                    }
                }
            }
            for (int i = 0; i <= keyedtomax; i++)
            {
                foreach (string key in KeyedToCount.Keys)
                {
                    if (KeyedToCount[key] == i)
                    {
                        result = result + "The table " + key + " is keyed to by  " + i + " other tables\n";
                    }
                }
            }
            result += "The following tables are not keyed either way\n";
            {
                foreach (string key in KeyedToCount.Keys)
                {
                    if (KeyedToCount[key] == 0 && ForeignKeyCount[key] == 0)
                    {
                        result = result + key + "\n";
                    }
                }
            }
            result += "Possible typos in the following keys: \n";
            foreach (string key in TypoKeys.Keys)
            {
                result = result + "The key of " + key + " is a possible typo";
            }
            return result;
        }
    
        public static string genHeaderJSP()
        {
            string result = "";
            result += "<!DOCTYPE html>\n";
            result += "<html lang=\"en\">\n";
            result += "<head>\n";
            result += "<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">\n";
            result += "<meta charset=\"UTF-8\">\n";
            result += "<title>${pageTitle}</title>\n";
            result += "<link rel=\"stylesheet\" href=\"https://cdnjs.cloudflare.com/ajax/libs/jquery-modal/0.9.1/jquery.modal.min.css\" />\n";
            result += "<link href=\"https://cdn.jsdelivr.net/npm/bootstrap@5.0.2/dist/css/bootstrap.min.css\" rel=\"stylesheet\">\n";
            result += "<link href=\"css/"+settings.database_name+"/jquery-ui.css\" rel=\"stylesheet\">\n";
            result += "<link href=\"css/"+settings.database_name+"/site.css\" rel=\"stylesheet\">";
            foreach (table t in all_tables)
            {
                result += "<c:if test=\"${pageTitle eq 'Add " + t.name + "'|| pageTitle eq 'Edit " + t.name + "'}\">\n";
                result += "<script src=\"css/"+settings.database_name+"/add" + t.name + ".css\"></script>\n";
                result += "</c:if>\n";
            }
            foreach (table t in all_tables)
            {
                result += "<c:if test=\"${pageTitle eq 'All " + t.name + "'}\">\n";
                result += "<script src=\"css/" + settings.database_name + "/all" + t.name + ".css\"></script>\n";
                result += "</c:if>\n";
            }
            result += "<body onload=\"\">\n";
            result += "<header id=\"xxxHeader\" onload=\"\">\n";
            result += "<div class=\"row\">\n";
            result += "<div class=\"col-md-2\"></div>\n";
            result += "<div class=\"col-md-2\"></div>\n";
            result += "<div class=\"col-md-2\"></div>\n";
            result += "<div class=\"col-md-2\"></div>\n";
            result += "<div class=\"col-md-2\"></div>\n";
            result += "<div class=\"col-md-2\"></div>\n";
            result += "</div>\n";
            result += "<main<\n";
            result += "<c:if test=\"${not empty User}\">\n";
            result += "<%@include file=\"/WEB-INF/"+settings.database_name+"/user_dash_buttons.jsp\"%>\n";
            result += "</c:if>\n";
            result += "";
            return result;
        }

        public static string genFooterJSP()
        {
            string result = "<footer>\n";
            result += "<div class=\"row\">\n";
            result += "<div class=\"col-md-2\"></div>\n";
            result += "<div class=\"col-md-2\"></div>\n";
            result += "<div class=\"col-md-2\"></div>\n";
            result += "<div class=\"col-md-2\"></div>\n";
            result += "<div class=\"col-md-2\"></div>\n";
            result += "<div class=\"col-md-2\"></div>\n";
            result += "</div>\n";
            result += "</footer>\n";
            result += "<script src=\"https://ajax.googleapis.com/ajax/libs/jquery/3.7.1/jquery.min.js\"></script>\n";
            result += "<script src=\"js/jquery.validate.js\"></script>";
            result += "<script src=\"https://ajax.googleapis.com/ajax/libs/jqueryui/1.13.2/jquery-ui.min.js\"></script>\n";
            result += "<script src=\"https://cdnjs.cloudflare.com/ajax/libs/jquery-modal/0.9.1/jquery.modal.min.js\"></script>\n";
            result += "<script src=\"js/"+settings.database_name+"/site.js\"></script>\n";
            foreach (table t in all_tables) {
                result += "<c:if test=\"${pageTitle eq 'Add "+t.name+ "'|| pageTitle eq 'Edit "+t.name+"'}\">\n";
                result += "<script src=\"js/"+settings.database_name+"/add"+t.name+".js\"></script>\n";
                result += "</c:if>\n";
            }
            foreach (table t in all_tables)
            {
                result += "<c:if test=\"${pageTitle eq 'All " + t.name + "'}\">\n";
                result += "<script src=\"js/" + settings.database_name + "/all" + t.name + ".js\"></script>\n";
                result += "</c:if>\n";
            }
            result += "</body>\n";            
            result += "</html>\n";
            
            return result;
        }
    }
}

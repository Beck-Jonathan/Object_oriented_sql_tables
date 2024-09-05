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
    }
}

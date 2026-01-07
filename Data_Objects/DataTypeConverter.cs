using System;
using System.ComponentModel;
namespace Data_Objects
{
    public static class DataTypeConverter
    {
        public static string toSqlReaderDataType(this string cSharpDataType)
        {
            string result = cSharpDataType.Replace("[", "").Replace("]", "");
            if (result == "nvarchar") { result = "String"; }
            if (result == "bit") { result = "bool"; }
            if (result == "Int" || cSharpDataType == "Integer") { result = "Int32"; }
            if (String.Compare(result, "datetime", true) == 0) { result = "DateTime"; }
            return result;
        }
        public static string toCSharpDataType(this string sqlDataType)
        {
            string result = sqlDataType.Replace("[", "").Replace("]", "").ToLower();
            if (result.Contains("varchar")) { result = "string"; }
            if (result.Contains( "bit")) { result = "bool"; }
            if (result.Contains( "tinyint")) { result = "bool"; }
            if (result.Contains( "Int") || sqlDataType == "Integer") { result = "int"; }
            if (result.Contains("decimal")) { result = "decimal"; }
            if (String.Compare(result, "datetime", true) == 0) { result = "DateTime"; }
            return result;
        }
        public static string toJavaDataType(this string sqlDataType)
        {
            string result = sqlDataType.Replace("[", "").Replace("]", "");
            if (result.ToLower().Contains("varchar")) { result = "String"; }
            if (result.ToLower().Equals("bit")) { result = "boolean"; }
            if (result.ToLower().Equals("bool")) { result = "boolean"; }
            if (result.ToLower().Equals("int") || sqlDataType.ToLower().Equals("Integer")) { result = "Integer"; }
            if (result.ToLower().Equals("datetime")) { result = "Date"; }
            if (result.ToLower().Equals("decimal")) { result = "Double"; }
            return result;
        }
        public static string toJavaDAODataType(this string sqlDataType)
        {
            string result = sqlDataType.Replace("[", "").Replace("]", "");
            if (result.ToLower().Contains("varchar")) { result = "String"; }
            if (result.ToLower().Equals("bit")) { result = "Boolean"; }
            if (result.ToLower().Contains("bool")) { result = "Boolean"; }
            if (result.ToLower().Contains("int") || sqlDataType.ToLower().Equals("integer")) { result = "Int"; }
            if (result.ToLower().Contains("date")) { result = "LocalDate"; }
            if (result.ToLower().Equals("decimal")) { result = "Double"; }
            return result;
        }
        public static string bracketStrip(this string fieldName)
        {
            string result = fieldName;
            result = result.Replace("[", "").Replace("]", "");
            return result;
        }
        public static string toSQLDBType(this string dataType, int length)
        {
            string result = dataType;
            if (String.Compare(result, "nvarchar", true) == 0) { result = "NVarChar"; }
            if (length > 0)
            {
                result = result + "," + length;
            }
            return result;
        }
        public static string firstCharLower(this string value)
        {
            return Char.ToLower(value[0]) + value.Substring(1);
        }

        public static string toDjangoDataType(this string value, int length, string comment)
        {
            value = value.ToLower();
            string result;
            switch (value)
            {
                case "int":
                    result = "IntegerField(" + comment;
                    break;
                case "date":
                    result = "DateField(" + comment;
                    break;
                case "dateTime":
                    result = "DatetimeField(" + comment;
                    break;
                case "bit":
                    result = "BooleanField(" + comment;
                    break;
                case "nvarchar":
                    result = "CharField(" + comment + ",max_length=" + length;
                    break;
                case "varchar":
                    result = "CharField(" + comment + ",max_length=" + length;
                    break;
                case "decimal":
                    result = "DecimalField(" + comment;
                    break;
                case "time":
                    result = "TimeField(" + comment;
                    break;
                default:
                    result = value + "Field(" + comment;
                    break;
            }

            return result;

        }
        public static bool isEmail(this string value) {            
            return  value.ToLower().Contains("email") || value.ToLower().Contains("e-mail");           
        }
        public static bool isWebsite(this string value)
        {
            return value.ToLower().Contains("website") || value.ToLower().Contains("url");
        }
    }
}

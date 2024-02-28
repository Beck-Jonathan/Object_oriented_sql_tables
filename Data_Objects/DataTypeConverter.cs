﻿using System;

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
            string result = sqlDataType.Replace("[", "").Replace("]", "");
            if (result == "nvarchar") { result = "string"; }
            if (result == "bit") { result = "bool"; }
            if (result == "Int" || sqlDataType == "Integer") { result = "int"; }
            if (String.Compare(result, "datetime", true) == 0) { result = "DateTime"; }

            return result;


        }

        public static string toJavaDataType(this string sqlDataType)
        {
            string result = sqlDataType.Replace("[", "").Replace("]", "");
            if (result.ToLower().Equals("nvarchar")) { result = "String"; }
            if (result.ToLower().Equals("bit")) { result = "boolean"; }
            if (result.ToLower().Equals("bool")) { result = "boolean"; }
            if (result.ToLower().Equals("int") || sqlDataType.ToLower().Equals("Integer")) { result = "Integer"; }
            if (result.ToLower().Equals("datetime")) { result = "LocalDate"; }
            return result;

        }
        public static string toJavaDAODataType(this string sqlDataType)
        {
            string result = sqlDataType.Replace("[", "").Replace("]", "");
            if (result.ToLower().Equals("nvarchar")) { result = "String"; }
            if (result.ToLower().Equals("bit")) { result = "boolean"; }
            if (result.ToLower().Equals("bool")) { result = "boolean"; }
            if (result.ToLower().Equals("int") || sqlDataType.ToLower().Equals("integer")) { result = "Int"; }
            if (result.ToLower().Equals("datetime")) { result = "LocalDate"; }
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
    }
}

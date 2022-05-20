using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using System.Text.RegularExpressions;
using System.Text;

public partial class UserDefinedFunctions
{
    [Microsoft.SqlServer.Server.SqlFunction]
    public static SqlString FixRuToEn(SqlString input)
    {
        string str = input.ToString();
        Regex regex = new Regex("[БГДЁЖЗИЙЛПУФЦЧШЩЪЫЬЭЮЯ]", RegexOptions.IgnoreCase);
        Match match = regex.Match(str);
        StringBuilder sb = new StringBuilder(str);
        if (match.Success)
        {
            //Замена английских на русские        
            sb.Replace("A", "А");
            sb.Replace("B", "В");
            sb.Replace("C", "С");
            sb.Replace("E", "Е");
            sb.Replace("H", "Н");
            sb.Replace("K", "К");
            sb.Replace("M", "М");
            sb.Replace("O", "О");
            sb.Replace("P", "Р");
            sb.Replace("T", "Т");
            sb.Replace("X", "Х");
            return new SqlString(sb.ToString());
        }
        else
        {
            //Замена русских на английские        
            sb.Replace("А", "A");
            sb.Replace("В", "B");
            sb.Replace("С", "C");
            sb.Replace("Е", "E");
            sb.Replace("Н", "H");
            sb.Replace("К", "K");
            sb.Replace("М", "M");
            sb.Replace("О", "O");
            sb.Replace("Р", "P");
            sb.Replace("Т", "T");
            sb.Replace("Х", "X");
            return new SqlString(sb.ToString());
        }        
    }
}

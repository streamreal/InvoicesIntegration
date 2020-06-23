using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using System.Text.RegularExpressions;

public partial class UserDefinedFunctions
{
    [Microsoft.SqlServer.Server.SqlFunction]
    public static SqlString GetDeliveryNumber(SqlString input)
    {
        string comment = input.ToString();
        Regex regex = new Regex("D[0-9]{6}", RegexOptions.IgnoreCase);
        Match match = regex.Match(comment);
        if (match.Success)
        {
            return new SqlString(match.Value);
        }
        else
        {
            return new SqlString(string.Empty);
        }
        
    }
}

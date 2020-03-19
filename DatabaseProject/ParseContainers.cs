using System;
using System.Data.SqlTypes;
using System.Text.RegularExpressions;

public partial class UserDefinedFunctions
{
    [Microsoft.SqlServer.Server.SqlFunction]
    public static SqlString ParseContainers(SqlString sql_item_cont, SqlString sql_else_cont)
    {
        if (sql_item_cont.IsNull)
        {
            return new SqlString(string.Empty);
        }
        else
        {
            string item_cont = sql_item_cont.ToString().ToUpper().Trim();
            string else_cont = sql_else_cont.ToString().ToUpper().Trim();
            string output = String.Empty;

            MatchCollection m = Regex.Matches(item_cont, "[A-Z]{3}U[0-9]{7}");

            foreach (Match match in m)
            {
                if (!output.Contains(match.Value))
                {
                    if (else_cont.Contains(match.Value))
                    {
                        output = output + match.Value + " ЧАСТЬ, ";
                    }
                    else
                    {
                        output = output + match.Value + ", ";
                    }
                }
            }
            if (output.EndsWith(", "))
            {
                output = output.Remove(output.Length - 2);
            }
            return new SqlString(output);
        }
    }
}


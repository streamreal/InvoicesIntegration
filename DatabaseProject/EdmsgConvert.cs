using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using System.Text;

public partial class UserDefinedFunctions
{
    [Microsoft.SqlServer.Server.SqlFunction]
    public static SqlString EdmsgConvert(SqlBinary msg)
    {
        if (msg.IsNull)
        {
            return new SqlString(string.Empty);
        }
        else
        {
            byte[] message = (byte[])msg;
            UTF8Encoding utf8 = new UTF8Encoding();
            string decodedMessage = utf8.GetString(message);
            return new SqlString(decodedMessage);
        }
    }
}

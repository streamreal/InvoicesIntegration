using System;
using System.Data.SqlTypes;
using System.Text;
using Microsoft.SqlServer.Server;

[Serializable]
[Microsoft.SqlServer.Server.SqlUserDefinedAggregate(
    Format.UserDefined, /// Binary Serialization because of StringBuilder
    IsInvariantToOrder = false, /// order changes the result
    IsInvariantToNulls = true,  /// nulls don't change the result
    IsInvariantToDuplicates = false, /// duplicates change the result
    MaxByteSize = -1)]
public struct RowsConcat : IBinarySerialize
{
    private StringBuilder _accumulator;
    private string _delimiter;
    public Boolean IsNull { get; private set; }

    public void Init()
    {
        _accumulator = new StringBuilder();
        _delimiter = " ";
        this.IsNull = true;
    }

    public void Accumulate(SqlString Value, SqlString Delimiter, SqlInt32 SkipDuplicates)
    {
        if (Value.IsNull || (SkipDuplicates.Value == 1 && _accumulator.ToString().Contains(Value.ToString())))
        {
            return;
        }

        if (!Delimiter.IsNull
            & Delimiter.Value.Length > 0)
        {
            _delimiter = Delimiter.Value; /// save for Merge
            if (_accumulator.Length > 0)
            {
                _accumulator.Append(Delimiter.Value);
            }

        }

        if (Value.IsNull)
        {
            _accumulator.Append(string.Empty);
        }
        else
        {
            _accumulator.Append(Value.Value);
        }
            
        
        if (Value.IsNull == false)
        {
            this.IsNull = false;
        }
    }

    /// Merge onto the end 
    public void Merge(RowsConcat Group)
    {
        /// add the delimiter between strings
        if (_accumulator.Length > 0
            & Group._accumulator.Length > 0) _accumulator.Append(_delimiter);

        ///_accumulator += Group._accumulator;
        _accumulator.Append(Group._accumulator.ToString());

    }

    public SqlString Terminate()
    {
        return new SqlString(_accumulator.ToString());
    }

    /// deserialize from the reader to recreate the struct
    void IBinarySerialize.Read(System.IO.BinaryReader r)
    {
        _delimiter = r.ReadString();
        _accumulator = new StringBuilder(r.ReadString());

        if (_accumulator.Length != 0)
        {
            this.IsNull = false;
        }
    }

    void IBinarySerialize.Write(System.IO.BinaryWriter w)
    {
        w.Write(_delimiter);
        w.Write(_accumulator.ToString());
    }
}

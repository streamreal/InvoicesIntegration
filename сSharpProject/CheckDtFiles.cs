using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace сSharpProject
{
    class CheckDtFiles
    {
        public static void LoadNewDeclarations()
        {
            DirectoryInfo di = new DirectoryInfo(@"\\10.0.0.33\CD_Scans");

			SqlConnectionStringBuilder connBuilder = new SqlConnectionStringBuilder
			{
				DataSource = "10.10.0.28",
				UserID = "phpuser",
				Password = "gnQCUElU"
			};

			IDbConnection connection;
			connection = new SqlConnection(connBuilder.ConnectionString);
			connection.Open();

			DataContext db = new DataContext(connection);
			Table<notices_dt_files> table = db.GetTable<notices_dt_files>();			

			var savedRegNums = table.Select(d => d.regnum);
			
			foreach (FileInfo fi in di.GetFiles("GTD_????????_??????_???????.pdf", SearchOption.AllDirectories))
            {
                string regNum = fi.Name.Replace(".pdf", "").Replace("GTD_", "").Replace("_", "/").ToLower();
				string year = regNum.Substring(13, 2);
                int yearNum;
                int.TryParse(year, out yearNum);

                if (yearNum >= 20)
                {
					int count = savedRegNums.Where(d => d == regNum).Select(d => d).Count();

					if (count > 0)
					{
						continue;
					}
					else
					{
						notices_dt_files toInsert = new notices_dt_files
						{
							regnum = regNum,
							fullpath = fi.FullName,
							load_datetime = DateTime.Now,
							was_processed = 0
						};

						table.InsertOnSubmit(toInsert);

						Console.WriteLine(regNum);

					}					
                }                
            }

			db.SubmitChanges();
			connection.Close();
			Console.WriteLine("END");
			Console.ReadLine();
        }
    }

	[global::System.Data.Linq.Mapping.TableAttribute(Name = "webproject.dbo.notices_dt_files")]
	public partial class notices_dt_files
	{

		private int _id;

		private string _regnum;

		private string _vat;

		private string _fullpath;

		private System.Nullable<System.DateTime> _load_datetime;

		private System.Nullable<int> _was_processed;

		private System.Nullable<System.DateTime> _sent_datetime;

		public notices_dt_files()
		{
		}

		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_id", AutoSync = AutoSync.Default, DbType = "Int NOT NULL IDENTITY", IsDbGenerated = true, IsPrimaryKey = true)]
		public int id
		{
			get
			{
				return this._id;
			}
			set
			{
				if ((this._id != value))
				{
					this._id = value;
				}
			}
		}

		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_regnum", DbType = "VarChar(50)")]
		public string regnum
		{
			get
			{
				return this._regnum;
			}
			set
			{
				if ((this._regnum != value))
				{
					this._regnum = value;
				}
			}
		}

		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_vat", DbType = "VarChar(50)")]
		public string vat
		{
			get
			{
				return this._vat;
			}
			set
			{
				if ((this._vat != value))
				{
					this._vat = value;
				}
			}
		}

		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_fullpath", DbType = "VarChar(300)")]
		public string fullpath
		{
			get
			{
				return this._fullpath;
			}
			set
			{
				if ((this._fullpath != value))
				{
					this._fullpath = value;
				}
			}
		}

		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_load_datetime", DbType = "DateTime")]
		public System.Nullable<System.DateTime> load_datetime
		{
			get
			{
				return this._load_datetime;
			}
			set
			{
				if ((this._load_datetime != value))
				{
					this._load_datetime = value;
				}
			}
		}

		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_was_processed", DbType = "Int")]
		public System.Nullable<int> was_processed
		{
			get
			{
				return this._was_processed;
			}
			set
			{
				if ((this._was_processed != value))
				{
					this._was_processed = value;
				}
			}
		}

		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_sent_datetime", DbType = "DateTime")]
		public System.Nullable<System.DateTime> sent_datetime
		{
			get
			{
				return this._sent_datetime;
			}
			set
			{
				if ((this._sent_datetime != value))
				{
					this._sent_datetime = value;
				}
			}
		}
	}
}

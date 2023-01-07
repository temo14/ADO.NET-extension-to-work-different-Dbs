using System.Data.SqlClient;
using DatabaseHelper.Common;

namespace DatabaseHelper.MsSql;

public sealed class MsSqlDatabase : Database<SqlConnection, SqlCommand, SqlDataReader>
{
	public MsSqlDatabase(string connectionString) : base(connectionString) { }
}
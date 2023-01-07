using DatabaseHelper.Common;
using Oracle.ManagedDataAccess.Client;

namespace DatabaseHelper.Oracle;

public class OracleDatabase : Database<OracleConnection, OracleCommand, OracleDataReader>
{
	public OracleDatabase(string connectionString) : base(connectionString) { }
}
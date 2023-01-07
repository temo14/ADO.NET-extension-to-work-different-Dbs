using DatabaseHelper.Common;
using MySqlConnector;

namespace DatabaseHelper.MySql;

public class MySqlDatabase : Database<MySqlConnection, MySqlCommand, MySqlDataReader>
{
	public MySqlDatabase(string connectionString) : base(connectionString) { }
}
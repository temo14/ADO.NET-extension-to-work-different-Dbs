using System.Data.SqlClient;
using DatabaseHelper.MsSql;
	
using (MsSqlDatabase database = new(@"server=DESKTOP-H6TAP36; database=egadf; integrated security = true"))
{
	string[] testCommands = new string[] { "insert into Employee(PersonName, Phone) values('Test1', 12345)"
                                           ,"insert into Employee(PersonName, Email) values('Test2', 'TestEmail')"
										   //,"insert into Employee(PersonName) values()"
										 };
	
	try
	{
		database.BeginTransaction();
		for (int i = 0; i < testCommands.Length; i++)
		{
			database.ExecuteNonQuery(testCommands[i]);
		}
		database.CommitTransaction();
	}
	catch (Exception exception)
	{
		database.RollbackTransaction();
		Console.WriteLine(exception);
		throw;
	}

}
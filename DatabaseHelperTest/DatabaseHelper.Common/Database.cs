using System.Data;

namespace DatabaseHelper.Common;

public abstract class Database<TConnection, TCommand, TReader> : IDisposable
    where TConnection : IDbConnection, new()
    where TCommand : IDbCommand
    where TReader : IDataRecord
{
    protected TConnection? _connection;
    protected IDbTransaction? _transaction;

    protected bool IsTransactionActive => _transaction != null;

    public Database(string connectionString)
    {
        ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public string ConnectionString { get; }

    public virtual TConnection GetConnection() => _connection ??= new TConnection { ConnectionString = this.ConnectionString };

    public virtual TConnection OpenConnection()
    {
        GetConnection().Open();
        return GetConnection();
    }

    public virtual void CloseConnection()
    {
        GetConnection().Close();
    }

    public virtual void BeginTransaction()
    {
        if (IsTransactionActive) throw new InvalidOperationException("Transaction is already active");
        _transaction = OpenConnection().BeginTransaction();
    }

    public virtual void CommitTransaction()
    {
        if (!IsTransactionActive) throw new InvalidOperationException("Transaction is not active");
        _transaction?.Commit();
        _transaction?.Dispose();
        _transaction = null;
    }

    public virtual void RollbackTransaction()
    {
        if (!IsTransactionActive) throw new InvalidOperationException("Transaction is not active");
        _transaction?.Rollback();
        _transaction?.Dispose();
        _transaction = null;
    }

    public virtual TCommand GetCommand(string commandText, CommandType commandType, params IDataParameter[] parameters)
    {
        if (commandText == null) throw new ArgumentNullException(nameof(commandText));
        TCommand command = (TCommand) GetConnection().CreateCommand();
        command.CommandText = commandText;
        command.CommandType = commandType;
        command.Transaction = _transaction;
        foreach (var parameter in parameters)
        {
            command.Parameters.Add(parameter);
        }

        return command;
    }

    public virtual TCommand GetCommand(string commandText, params IDataParameter[] parameters) =>
        GetCommand(commandText, CommandType.Text, parameters);

    public virtual int ExecuteNonQuery(string commandText, CommandType commandType, params IDbDataParameter[] parameters)
    {
        IDbCommand command = GetCommand(commandText, commandType, parameters);

        try
        {
            if (GetConnection().State == ConnectionState.Closed)
            {
                command.Connection?.Open();
            }
            return command.ExecuteNonQuery();
        }
        finally
        {
            if (!IsTransactionActive) CloseConnection();
        }
    }

    public virtual int ExecuteNonQuery(string commandText, params IDbDataParameter[] parameters) =>
        ExecuteNonQuery(commandText, CommandType.Text, parameters);

    public virtual T ExecuteScalar<T>(string commandText, CommandType commandType, params IDbDataParameter[] parameters)
    {
        IDbCommand command = GetCommand(commandText, commandType, parameters);

        try
        {
            if (GetConnection().State == ConnectionState.Closed)
            {
                command.Connection?.Open();
            }
            //TODO: We need to make test of this conversion.
            return (T) Convert.ChangeType(command.ExecuteScalar(), typeof(T));
        }
        finally
        {
            if (!IsTransactionActive) CloseConnection();
        }
    }

    public virtual T ExecuteScalar<T>(string commandText, params IDbDataParameter[] parameters) =>
        ExecuteScalar<T>(commandText, CommandType.Text, parameters);

    public virtual TReader ExecuteReader(string commandText, CommandType commandType, params IDbDataParameter[] parameters)
    {
        TCommand command = GetCommand(commandText, commandType, parameters);

        if (GetConnection().State == ConnectionState.Closed)
        {
            command.Connection?.Open();
        }
        return (TReader) command.ExecuteReader();
    }

    public virtual TReader ExecuteReader(string commandText, params IDbDataParameter[] parameters) =>
        ExecuteReader(commandText, CommandType.Text, parameters);

    public virtual void Dispose()
    {
        CloseConnection();
        _connection?.Dispose();
    }
}
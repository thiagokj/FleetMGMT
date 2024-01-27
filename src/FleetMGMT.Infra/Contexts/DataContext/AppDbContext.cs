using System.Data;
using Microsoft.Data.SqlClient;

namespace FleetMGMT.Infra.Contexts.DataContext;
public class AppDbContext(IDbConnection connection) : IDisposable
{
    private readonly IDbConnection _connection = connection ?? throw new ArgumentNullException(nameof(connection));

    public SqlConnection Connection
    {
        get
        {
            if (_connection.State == ConnectionState.Closed)
                _connection.Open();

            return _connection as SqlConnection ??
                throw new InvalidOperationException(@"Falha ao converter 
                IDbConnection para SqlConnection.");
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_connection != null && _connection.State != ConnectionState.Closed)
                _connection.Close();
        }
    }
}

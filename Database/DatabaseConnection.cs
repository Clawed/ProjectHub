using System;
using System.Data;
using MySql.Data.MySqlClient;
using ProjectHub.Database.Interfaces;
using ProjectHub.Database.Adapter;
using ProjectHub.Core;

namespace ProjectHub.Database
{
    public class DatabaseConnection : IDatabaseClient, IDisposable
    {
        private readonly IQueryAdapter _adapter;
        private readonly MySqlConnection _con;

        public DatabaseConnection(string ConnectionStr)
        {
            _con = new MySqlConnection(ConnectionStr);
            _adapter = new NormalQueryReactor(this);
        }

        public void Dispose()
        {
            if (_con.State == ConnectionState.Open)
            {
                _con.Close();
            }

            _con.Dispose();
            GC.SuppressFinalize(this);
        }

        public void connect()
        {
            Open();
        }

        public void disconnect()
        {
            Close();
        }

        public IQueryAdapter GetQueryReactor()
        {
            return _adapter;
        }

        public void prepare()
        {
            // nothing here
        }

        public void reportDone()
        {
            Dispose();
        }

        public MySqlCommand createNewCommand()
        {
            return _con.CreateCommand();
        }

        public void Open()
        {
            if (_con.State == ConnectionState.Closed)
            {
                try
                {
                    _con.Open();
                }
                catch (Exception Error)
                {
                    Logging.WriteLine(Error.ToString());
                }
            }
        }

        public void Close()
        {
            if (_con.State == ConnectionState.Open)
            {
                _con.Close();
            }
        }
    }
}
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Data.Common;


namespace Sefaz.Infra.DbContexto
{
     public class Database 
    {

        public Database(OracleConnection conn)
        {
            Db = conn;
        }

        protected static OracleConnection Db { get; set; }
               

        public  void AddInParameter(IDbCommand comando, string NomeParametro, OracleDbType Tipo, Object Valor)
        {
            OracleParameter param = new OracleParameter(NomeParametro, Tipo, ParameterDirection.Input);
            param.Value = Valor;
            comando.Parameters.Add(param);

        }

        public  void AddOutParameter(IDbCommand comando, string NomeParametro, OracleDbType Tipo, Object Valor)
        {
            OracleParameter param = new OracleParameter(NomeParametro, Tipo,1,null, ParameterDirection.Output);
            comando.Parameters.Add(param);

        }

        public void AddCursorOutParameter(IDbCommand comando, string NomeParametro)
        {
            OracleParameter param = new OracleParameter(NomeParametro,OracleDbType.RefCursor, ParameterDirection.Output);
            comando.Parameters.Add(param);

        }

        public  DbCommand GetStoredProcCommand(string NomeProcedure)
        {
            DbCommand comand = Db.CreateCommand();
            comand.CommandType = CommandType.StoredProcedure;
            comand.CommandText = NomeProcedure;
            return comand;

        }

        public void ExecuteNonQuery(DbCommand command)
        {
            command.Connection.Open();
            command.ExecuteNonQuery();
        }

        public IDataReader ExecuteReader(DbCommand command)
        {
            command.Connection.Open();
            return command.ExecuteReader();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sefaz.Infra.DbContexto
{
    internal class GeradorDeParametros
    {
        #region Criacao de Parametros para Procedures

        public static DbParameter ParamInLong(string Nome, long? Valor, DbCommand cmd)
        {
            DbParameter ParametroIn = cmd.CreateParameter();
            ParametroIn.ParameterName = Nome;
            ParametroIn.DbType = DbType.Int64;
            ParametroIn.Value = Valor.HasValue ? Valor.Value : (long?)null;
            return ParametroIn;
        }

        public static DbParameter ParamInDouble(string Nome, double? Valor, DbCommand cmd)
        {
            DbParameter ParametroIn = cmd.CreateParameter();
            ParametroIn.ParameterName = Nome;
            ParametroIn.DbType = DbType.Double;
            ParametroIn.Value = Valor.HasValue ? Valor.Value : (double?)null;
            return ParametroIn;
        }

        public static DbParameter ParamInDecimal(string Nome, decimal? Valor, DbCommand cmd)
        {
            DbParameter ParametroIn = cmd.CreateParameter();
            ParametroIn.ParameterName = Nome;
            ParametroIn.DbType = DbType.Decimal;
            ParametroIn.Value = Valor;
            return ParametroIn;
        }

        public static DbParameter ParamInString(string Nome, string Valor, DbCommand cmd, int Tamanho = 2500)
        {
            DbParameter ParametroIn = cmd.CreateParameter();
            ParametroIn.ParameterName = Nome;
            ParametroIn.DbType = DbType.String;
            ParametroIn.Size = Tamanho;
            ParametroIn.Value = Valor;
            return ParametroIn;
        }

        public static DbParameter ParamInDate(string Nome, DateTime? Valor, DbCommand cmd)
        {
            DbParameter ParametroIn = cmd.CreateParameter();
            ParametroIn.ParameterName = Nome;
            ParametroIn.DbType = DbType.DateTime;
            ParametroIn.Value = Valor.HasValue ? Valor.Value : (DateTime?)null;
            return ParametroIn;
        }


        public static DbParameter ParamOut(string NomeParam, DbType Tipo, DbCommand cmd)
        {
            DbParameter ParametroOut = cmd.CreateParameter();
            ParametroOut.ParameterName = NomeParam;
            ParametroOut.DbType = Tipo;
            ParametroOut.Direction = ParameterDirection.Output;
            if (Tipo == DbType.String) { ParametroOut.Size = 2550; }
            if (Tipo == DbType.Object) { ParametroOut.GetType().GetProperty("OracleDbType").GetSetMethod().Invoke(ParametroOut, new object[] { 121 }); } // OracleDbType.RefCursor

            return ParametroOut;
        }

        #endregion
    }
}

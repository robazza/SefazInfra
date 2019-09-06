using Oracle.ManagedDataAccess.Client;
using Sefaz.Infra.Configuration;
using Sefaz.Infra.DbContexto.Model;
using Sefaz.Infra.Geral.Seguranca;
using System;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using System.Web.Routing;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.EntityClient;
using System.Data.Common;
using System.Collections.Generic;
using Sefaz.Infra.DbContexto.Model;

namespace Sefaz.Infra.DbContexto
{
    public static class OracleContexto
    {
        public static System.Data.Common.DbConnection Database { get; set; }

        public static System.Data.Common.DbConnection CriarConexao(string siglasistema, string usuario, string senha)
        {
            ObjectCache cache = MemoryCache.Default;
            string conexao;

            if (cache["stringconexao" + siglasistema] == null)
            {
                conexao = ObterStringConexao(siglasistema, usuario, senha);
                cache["stringconexao" + siglasistema] = conexao;
            }
            else
            {
                conexao = (string)cache["stringconexao" + siglasistema];
            }

            return new OracleConnection(conexao);

        }

        public static System.Data.Common.DbConnection CriarConexao(string siglasistema)
        {
            ObjectCache cache = MemoryCache.Default;
            string conexao;

            if (cache["stringconexao" + siglasistema] == null)
            {
                conexao = ObterStringConexao(siglasistema);
                cache["stringconexao" + siglasistema] = conexao;
            }
            else
            {
                conexao = (string)cache["stringconexao" + siglasistema];
            }


            return new OracleConnection(conexao);


        }

        public static Database ObterConexao()
        {
            OracleConnection db = (OracleConnection)CriarConexao();
            return new Database(db);
        }

        public static System.Data.Common.DbConnection CriarConexao()
        {
            ObjectCache cache = MemoryCache.Default;
            string conexao;

            if (cache["stringconexao"] == null)
            {
                conexao = ObterStringConexao(CodigoSistemaCorrente);
                cache["stringconexao"] = conexao;
            }
            else
            {
                conexao = (string)cache["stringconexao"];
            }

            var connection = new OracleConnection(conexao);

            Logar(connection, "Sefaz.Infra.Dados.OracleContexto.CriarConexao", string.Format("Conexão criada para o sistema: {0}", CodigoSistemaCorrente));
            RegistrarVariaveisAmbiente(connection);

            return new OracleConnection(conexao);
        }

        public static ObjectContext CriarContexto()
        {
            ObjectCache cache = MemoryCache.Default;
            ObjectContext contexto = null;
            if (cache["conexao"] == null)
            {
                var entityBuilder = new EntityConnectionStringBuilder();

                entityBuilder.Provider = "Oracle.ManagedDataAccess.Client";
                entityBuilder.ProviderConnectionString = ObterStringConexao(CodigoSistemaCorrente);

                entityBuilder.Metadata = ConfigurationManager.AppSettings["MetaData"];
                cache["conexao"] = entityBuilder.ToString();
                contexto = new ObjectContext(entityBuilder.ToString());
            }
            else
            {
                contexto = new ObjectContext((string)cache["conexao"]);
            }
            RegistrarVariaveisAmbiente(contexto);
            return contexto;
        }

        public static System.Data.Common.DbConnection CriarConexaoSeg()
        {
            ObjectCache cache = MemoryCache.Default;
            string conexao;

            if (cache["stringconexaoseg"] == null)
            {
                conexao = ObterStringConexaoSegura();
                cache["stringconexaoseg"] = conexao;
            }
            else
            {
                conexao = (string)cache["stringconexao"];
            }


            return new OracleConnection(conexao);
        }

        internal static ObjectContext CriarContextoSeg()
        {
            ObjectCache cache = MemoryCache.Default;
            ObjectContext contexto = null;

            if (cache["conexaoSeg"] == null)
            {
                var entityBuilder = new EntityConnectionStringBuilder();

                entityBuilder.Metadata = ConfigurationManager.AppSettings["MetaDataSeg"];
                entityBuilder.Provider = "Oracle.ManagedDataAccess.Client";

                entityBuilder.ProviderConnectionString = ObterStringConexaoSegura();

                cache["conexaoSeg"] = entityBuilder.ToString();
                contexto = new ObjectContext(entityBuilder.ToString());
            }
            else
            {
                contexto = new ObjectContext((string)cache["conexaoSeg"]);
            }

            return contexto;
        }

        public static DbCommand GetStoredProcCommand(string NomeProcedure, DbConnection conexao)
        {
            DbCommand comand = conexao.CreateCommand();
            comand.CommandType = CommandType.StoredProcedure;
            comand.CommandText = NomeProcedure;
            return comand;
        }

        private static void RegistrarVariaveisAmbiente(ObjectContext contexto)
        {
            string controllerName = "";
            string actionName = "";
            string id = "";
            if (HttpContext.Current != null)
            {
                RouteData route = RouteTable.Routes.GetRouteData(new HttpContextWrapper(HttpContext.Current));
                if (route != null)
                {
                    controllerName = (string)route.Values["controller"];
                    actionName = (string)route.Values["action"];
                    id = route.Values["id"].ToString();
                }
            }

            var parametros = new List<OracleParameter>();

            parametros.Add(new OracleParameter("pcddominio", OracleDbType.Varchar2, CodigoDominioCorrente, ParameterDirection.Input));
            parametros.Add(new OracleParameter("pcdsistema", OracleDbType.Varchar2, CodigoSistemaCorrente, ParameterDirection.Input));
            parametros.Add(new OracleParameter("pdsurl", OracleDbType.Varchar2, controllerName + @"/" + actionName + @"/" + id, ParameterDirection.Input));
            parametros.Add(new OracleParameter("pdslogin", OracleDbType.Varchar2, LoginUsuario, ParameterDirection.Input));
            parametros.Add(new OracleParameter("pdstipologinapp", OracleDbType.Varchar2, TipoDocumentoUsuarioLogado, ParameterDirection.Input));
            parametros.Add(new OracleParameter("pdsloginapp", OracleDbType.Varchar2, DocumentoUsuarioLogado.ToString(), ParameterDirection.Input));
            parametros.Add(new OracleParameter("pdstipoidrepresentado", OracleDbType.Varchar2, TipoDocumentoRepresentado, ParameterDirection.Input));
            parametros.Add(new OracleParameter("pdsrepresentado", OracleDbType.Varchar2, DocumentoRepresentado.ToString(), ParameterDirection.Input));
            parametros.Add(new OracleParameter("pdsperfil", OracleDbType.Varchar2, Perfil, ParameterDirection.Input));

            try
            {
                contexto.ExecuteStoreCommand("begin DBAINFRA.PCKAUDIT.registrarvariaveis(:pCdDominio,:pCdSistema,:pDsUrl,:pDsLogin,:pdstipologinapp,:pdsloginapp,:pdstipoidrepresentado,:pdsrepresentado,:pdsperfil ); end;", parametros.ToArray());
            }
            catch (Exception ex)
            {
                throw new Exception("Houve um erro ao registrar as variáveis de auditoria. " + ex.Message);
            }
        }

        private static void RegistrarVariaveisAmbiente(OracleConnection conexao)
        {
            try
            {
                string controllerName = "";
                string actionName = "";
                string id = "";
                string url = string.Empty;
                if (HttpContext.Current != null)
                {
                    RouteData route = RouteTable.Routes.GetRouteData(new HttpContextWrapper(HttpContext.Current));
                    if (route != null)
                    {
                        controllerName = (string)route.Values["controller"];
                        actionName = (string)route.Values["action"];
                        id = route.Values["id"].ToString();
                    }
                }

                url = controllerName + @"/" + actionName + @"/" + id;

                string variavies = string.Format("{0}: {1}; ", "pcddominio", CodigoDominioCorrente);
                variavies += string.Format("{0}: {1}; ", "pcdsistema", CodigoSistemaCorrente);
                variavies += string.Format("{0}: {1}; ", "pdsurl", url);
                variavies += string.Format("{0}: {1}; ", "pdslogin", LoginUsuario);
                variavies += string.Format("{0}: {1}; ", "pdstipologinapp", TipoDocumentoUsuarioLogado);
                variavies += string.Format("{0}: {1}; ", "pdsloginapp", DocumentoUsuarioLogado.ToString());
                variavies += string.Format("{0}: {1}; ", "pdstipoidrepresentado", TipoDocumentoRepresentado);
                variavies += string.Format("{0}: {1}; ", "pdsrepresentado", DocumentoRepresentado.ToString());
                variavies += string.Format("{0}: {1}; ", "pdsperfil", Perfil);

                Logar(conexao, "Sefaz.Infra.Dados.OracleContexto.RegistrarVariaveisAmbiente", string.Format("Registrro variáveis do sistema: {0}. Variáveis=> {1}", CodigoSistemaCorrente, variavies));

                if (conexao.State == ConnectionState.Closed)
                {
                    conexao.Open();
                }

                using (var cmd = (Oracle.ManagedDataAccess.Client.OracleCommand)conexao.CreateCommand())
                {
                    cmd.CommandText = "dbainfra.pckaudit.registrarvariaveis";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(GeradorDeParametros.ParamInString("pcddominio", CodigoDominioCorrente, cmd));
                    cmd.Parameters.Add(GeradorDeParametros.ParamInString("pcdsistema", CodigoSistemaCorrente, cmd));
                    cmd.Parameters.Add(GeradorDeParametros.ParamInString("pdsurl", url, cmd));
                    cmd.Parameters.Add(GeradorDeParametros.ParamInString("pdslogin", LoginUsuario, cmd));
                    cmd.Parameters.Add(GeradorDeParametros.ParamInString("pdstipologinapp", TipoDocumentoUsuarioLogado, cmd));
                    cmd.Parameters.Add(GeradorDeParametros.ParamInString("pdsloginapp", DocumentoUsuarioLogado.ToString(), cmd));
                    cmd.Parameters.Add(GeradorDeParametros.ParamInString("pdstipoidrepresentado", TipoDocumentoRepresentado, cmd));
                    cmd.Parameters.Add(GeradorDeParametros.ParamInString("pdsrepresentado", DocumentoRepresentado.ToString(), cmd));
                    cmd.Parameters.Add(GeradorDeParametros.ParamInString("pdsperfil", Perfil, cmd));

                    cmd.ExecuteNonQuery();

                    Logar(conexao, "Sefaz.Infra.Dados.OracleContexto.RegistrarVariaveisAmbiente", string.Format("Registrro variáveis do sistema: {0}. Variáveis registradas com sucesso.", CodigoSistemaCorrente));

                    if (conexao.State == ConnectionState.Open)
                    {
                        conexao.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Logar(conexao, "Sefaz.Infra.Dados.OracleContexto.RegistrarVariaveisAmbiente", string.Format("Erro ao registrar variáveis do sistema: {0}", CodigoSistemaCorrente));
                throw new Exception("Houve um erro ao registrar as variáveis de auditoria. " + ex.Message);
            }
        }

        private static void Logar(OracleConnection conexao, string processo, string mensagem)
        {
            try
            {
                if (conexao.State == ConnectionState.Closed)
                {
                    conexao.Open();
                }

                using (var cmd = (Oracle.ManagedDataAccess.Client.OracleCommand)conexao.CreateCommand())
                {
                    cmd.CommandText = string.Format("dba{0}.pcklog.inserirlog", CodigoSistemaCorrente);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(GeradorDeParametros.ParamInLong("psqtipomensagemlog", 40, cmd));
                    cmd.Parameters.Add(GeradorDeParametros.ParamInString("pdsprocesso", processo, cmd));
                    cmd.Parameters.Add(GeradorDeParametros.ParamInLong("pnucodigoerro", 0, cmd));
                    cmd.Parameters.Add(GeradorDeParametros.ParamInString("pdsmensagem", mensagem, cmd));

                    cmd.ExecuteNonQuery();

                    if (conexao.State == ConnectionState.Open)
                    {
                        conexao.Close();
                    }
                }
            }
            catch (Exception)
            {
            }
        }


        private static string ObterStringConexao(string siglasistema)
        {
            try
            {
                string login = LoginUsuario;

                OracleElement oracleElement = SefazInfraSection.ObterConfiguracao().Dados.Oracle;

                string minPoolsize = oracleElement.MinPoolSize.ToString();
                string maxPoolsize = oracleElement.MaxPoolSize.ToString();
                string lifetime = oracleElement.ConnectionLifeTime.ToString();
                string timeout = oracleElement.ConnectionTimeout.ToString();
                string incPoolsize = oracleElement.IncrPoolSize.ToString();
                string decPoolsize = oracleElement.DecrPoolSize.ToString();
                string pooling = oracleElement.Pooling.ToString();
                string validateconection = oracleElement.ValidateConnection.ToString();


                if (!siglasistema.Equals(CodigoSistemaCorrente))
                {
                    login = Sefaz.Infra.Configuration.SefazInfraSection.ObterConfiguracao().Geral.UsuarioSistema;
                }

                Entities db = new Entities();

                db.Configuration.ProxyCreationEnabled = true;
                db.Configuration.LazyLoadingEnabled = true;

                GRUPO grupo = db.GRUPO.Where(g => g.SISTEMA.CDSISTEMA.Equals(siglasistema) & g.USUARIO.Where(u => u.DSLOGIN.Equals(login)).Any() & g.SISTEMA.DOMINIO.Where(d => d.CDDOMINIO.Equals(CodigoDominioCorrente)).Any()).ToList().FirstOrDefault();


                if (grupo == null)
                {
                    throw new Exception(
                               "Não foi possível obter as credenciais de conexao para o banco de dados da aplicação para o sistema: " +
                               siglasistema + " com o login: " + LoginUsuario + " no dominio: " + CodigoDominioCorrente +
                               " e no ambiente: " + AmbienteSistemaSeguranca +
                               ". Favor verificar as configurações de segurança.");
                }

                string usuariosenha = "password=" + Sefaz.Infra.Criptografia.CriptografiaFactory.Simetrica().Descriptografar(grupo.DSSENHABANCODADOS) + ";user id=" + grupo.CDGRUPO;

                short IdInstancia = grupo.SISTEMA.INSTANCIAESQUEMABANCO.Where(i => i.INSTANCIABANCO.AMBIENTE.CDAMBIENTE.Equals(AmbienteSistemaSeguranca)).First().SQINSTANCIABANCO;

                INSTANCIABANCO instancia = db.INSTANCIABANCO.Where(i => i.SQINSTANCIABANCO == IdInstancia).ToList().FirstOrDefault();

                string providerconexao =
                    ";data source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + instancia.HOST + ")(PORT=" + instancia.PORT + ")))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=" + instancia.SERVICENAME + ")));" + usuariosenha;// + ";MultipleActiveResultSets=True";

                if (!string.IsNullOrEmpty(minPoolsize) & !string.IsNullOrEmpty(maxPoolsize) & !string.IsNullOrEmpty(lifetime) & !string.IsNullOrEmpty(timeout) & !string.IsNullOrEmpty(incPoolsize) & !string.IsNullOrEmpty(decPoolsize) & !string.IsNullOrEmpty(pooling) & !string.IsNullOrEmpty(validateconection))
                {
                    providerconexao += ";Min Pool Size=" + minPoolsize + ";";
                    providerconexao += "Connection Lifetime=" + lifetime + ";";
                    providerconexao += "Connection Timeout=" + timeout + ";";
                    providerconexao += "Incr Pool Size=" + incPoolsize + ";";
                    providerconexao += "Decr Pool Size=" + decPoolsize + ";";
                    providerconexao += "Pooling=" + pooling + ";";
                    providerconexao += "Validate Connection=" + validateconection + ";";

                }
                return providerconexao;
            }
            catch (Exception ex)
            {

                throw ex;
            }


        }

        private static string ObterStringConexao(string siglasistema, string usuario, string senha)
        {
            try
            {
                string login = LoginUsuario;

                OracleElement oracleElement = SefazInfraSection.ObterConfiguracao().Dados.Oracle;

                string minPoolsize = oracleElement.MinPoolSize.ToString();
                string maxPoolsize = oracleElement.MaxPoolSize.ToString();
                string lifetime = oracleElement.ConnectionLifeTime.ToString();
                string timeout = oracleElement.ConnectionTimeout.ToString();
                string incPoolsize = oracleElement.IncrPoolSize.ToString();
                string decPoolsize = oracleElement.DecrPoolSize.ToString();
                string pooling = oracleElement.Pooling.ToString();
                string validateconection = oracleElement.ValidateConnection.ToString();


                if (!siglasistema.Equals(CodigoSistemaCorrente))
                {
                    login = Sefaz.Infra.Configuration.SefazInfraSection.ObterConfiguracao().Geral.UsuarioSistema;
                }

                Entities db = new Entities();

                GRUPO grupo = db.GRUPO.Where(g => g.SISTEMA.CDSISTEMA.Equals(siglasistema) & g.USUARIO.Where(u => u.DSLOGIN.Equals(login)).Any() & g.SISTEMA.DOMINIO.Where(d => d.CDDOMINIO.Equals(CodigoDominioCorrente)).Any()).ToList().FirstOrDefault();

                if (grupo == null)
                {
                    throw new Exception(
                               "Não foi possível obter as credenciais de conexao para o banco de dados da aplicação para o sistema: " +
                               siglasistema + " com o login: " + LoginUsuario + " no dominio: " + CodigoDominioCorrente +
                               " e no ambiente: " + AmbienteSistemaSeguranca +
                               ". Favor verificar as configurações de segurança.");
                }

                string usuariosenha = "password=" + Sefaz.Infra.Criptografia.CriptografiaFactory.Simetrica().Descriptografar(senha) + ";user id=" + usuario;

                short IdInstancia = grupo.SISTEMA.INSTANCIAESQUEMABANCO.First().SQINSTANCIABANCO;

                INSTANCIABANCO instancia = db.INSTANCIABANCO.Where(i => i.SQINSTANCIABANCO == IdInstancia).ToList().FirstOrDefault();

                string providerconexao =
                    ";data source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + instancia.HOST + ")(PORT=" + instancia.PORT + ")))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=" + instancia.SERVICENAME + ")));" + usuariosenha;

                if (!string.IsNullOrEmpty(minPoolsize) & !string.IsNullOrEmpty(maxPoolsize) & !string.IsNullOrEmpty(lifetime) & !string.IsNullOrEmpty(timeout) & !string.IsNullOrEmpty(incPoolsize) & !string.IsNullOrEmpty(decPoolsize) & !string.IsNullOrEmpty(pooling) & !string.IsNullOrEmpty(validateconection))
                {
                    providerconexao += ";Min Pool Size=" + minPoolsize + ";";
                    providerconexao += "Connection Lifetime=" + lifetime + ";";
                    providerconexao += "Connection Timeout=" + timeout + ";";
                    providerconexao += "Incr Pool Size=" + incPoolsize + ";";
                    providerconexao += "Decr Pool Size=" + decPoolsize + ";";
                    providerconexao += "Pooling=" + pooling + ";";
                    providerconexao += "Validate Connection=" + validateconection + ";";

                }
                return providerconexao;
            }
            catch (Exception ex)
            {

                throw ex;
            }


        }

        internal static string ObterStringConexaoSegura()
        {
            string HostSeg = ConfigurationManager.AppSettings["HostSeg"];
            string PortSeg = ConfigurationManager.AppSettings["PortSeg"];

            string ServiceNameSeg = InstanciaBancoSeg;

            string usuarioBancoSeguranca = Configuration.SefazInfraSection.ObterConfiguracao().Dados.OracleBaseSeguranca.Usuario;
            string senhaBancoSeguranca = Configuration.SefazInfraSection.ObterConfiguracao().Dados.OracleBaseSeguranca.Senha;

            string usuariosenha = "password=" + Sefaz.Infra.Criptografia.CriptografiaFactory.Simetrica().Descriptografar(senhaBancoSeguranca) + ";user id=" + usuarioBancoSeguranca;

            string providerconexao =
                ";data source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + HostSeg + ")(PORT=" + PortSeg + ")))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=" + ServiceNameSeg + ")));" + usuariosenha;

            OracleElement oracleElement = SefazInfraSection.ObterConfiguracao().Dados.Oracle;

            string minPoolsize = oracleElement.MinPoolSize.ToString();
            string maxPoolsize = oracleElement.MaxPoolSize.ToString();
            string lifetime = oracleElement.ConnectionLifeTime.ToString();
            string timeout = oracleElement.ConnectionTimeout.ToString();
            string incPoolsize = oracleElement.IncrPoolSize.ToString();
            string decPoolsize = oracleElement.DecrPoolSize.ToString();
            string pooling = oracleElement.Pooling.ToString();
            string validateconection = oracleElement.ValidateConnection.ToString();


            if (!string.IsNullOrEmpty(minPoolsize) & !string.IsNullOrEmpty(maxPoolsize) & !string.IsNullOrEmpty(lifetime) & !string.IsNullOrEmpty(timeout) & !string.IsNullOrEmpty(incPoolsize) & !string.IsNullOrEmpty(decPoolsize) & !string.IsNullOrEmpty(pooling) & !string.IsNullOrEmpty(validateconection))
            {
                providerconexao += ";Min Pool Size=" + minPoolsize + ";";
                providerconexao += "Connection Lifetime=" + lifetime + ";";
                providerconexao += "Connection Timeout=" + timeout + ";";
                providerconexao += "Incr Pool Size=" + incPoolsize + ";";
                providerconexao += "Decr Pool Size=" + decPoolsize + ";";
                providerconexao += "Pooling=" + pooling + ";";
                providerconexao += "Validate Connection=" + validateconection + ";";

            }

            return providerconexao;
        }

        private static string LoginUsuario
        {

            get
            {
                ControleAcesso controleAcesso = SegurancaFactory.ObterControleAcesso();

                if (controleAcesso.TokenCorrente != null && !String.IsNullOrWhiteSpace(controleAcesso.TokenCorrente.Login))
                {
                    return controleAcesso.TokenCorrente.Login;
                }

                SefazInfraSection configuracao = Sefaz.Infra.Configuration.SefazInfraSection.ObterConfiguracao();

                if (configuracao.Geral != null && !String.IsNullOrWhiteSpace(configuracao.Geral.UsuarioSistema))
                {
                    return configuracao.Geral.UsuarioSistema;
                }

                return string.Empty;
            }
        }

        private static long? DocumentoUsuarioLogado
        {
            get
            {
                ControleAcesso controleAcesso = SegurancaFactory.ObterControleAcesso();
                return controleAcesso?.TokenCorrente?.Cpf ?? (controleAcesso?.TokenCorrente?.Cnpj ?? 0);
            }
        }

        private static string TipoDocumentoUsuarioLogado
        {
            get
            {
                ControleAcesso controleAcesso = SegurancaFactory.ObterControleAcesso();
                return controleAcesso?.TokenCorrente?.Cpf.HasValue == true ? "1" : (controleAcesso?.TokenCorrente?.Cnpj.HasValue == true ? "2" : string.Empty);
            }
        }

        private static long? DocumentoRepresentado
        {
            get
            {
                ControleAcesso controleAcesso = SegurancaFactory.ObterControleAcesso();
                return controleAcesso?.TokenCorrente?.DocRepresentado ?? 0;
            }
        }

        private static string TipoDocumentoRepresentado
        {
            get
            {
                ControleAcesso controleAcesso = SegurancaFactory.ObterControleAcesso();
                return controleAcesso?.TokenCorrente?.TipoDoc ?? string.Empty;
            }
        }

        private static string Perfil
        {
            get
            {
                ControleAcesso controleAcesso = SegurancaFactory.ObterControleAcesso();
                return controleAcesso?.TokenCorrente?.Perfil ?? string.Empty;
            }
        }

        private static string InstanciaBancoSeg
        {
            get { return Configuration.SefazInfraSection.ObterConfiguracao().Dados.OracleBaseSeguranca.Instancia; }
        }

        private static string CodigoDominioCorrente
        {
            get { return Sefaz.Infra.Configuration.SefazInfraSection.ObterConfiguracao().Geral.CodigoDominio; }
        }

        /// <summary>
        /// Recupera o código de sistema da aplicação corrente.
        /// </summary>
        private static string CodigoSistemaCorrente
        {
            get { return Sefaz.Infra.Configuration.SefazInfraSection.ObterConfiguracao().Geral.CodigoSistema; }
        }

        private static string AmbienteSistemaSeguranca
        {
            get { return Sefaz.Infra.Util.Ambiente.CodigoAmbienteLogado; }
        }

        private static string ObterUsuarioSenhaBanco(string login, string sistema, string dominio, string ambiente)
        {
            Entities db = new Entities();

            GRUPO grupo = db.GRUPO.Where(g => g.SISTEMA.CDSISTEMA.Equals(sistema) & g.USUARIO.Where(u => u.DSLOGIN.Equals(login)).Any() & g.SISTEMA.DOMINIO.Where(d => d.CDDOMINIO.Equals(dominio)).Any() & g.SISTEMA.SISTEMAAMBIENTE.Where(sa => sa.AMBIENTE.CDAMBIENTE.Equals(ambiente)).Any()).ToList().FirstOrDefault();

            string usuario = "password=" + Sefaz.Infra.Criptografia.CriptografiaFactory.Simetrica().Descriptografar(grupo.DSSENHABANCODADOS) + ";user id=" + grupo.CDGRUPO;

            return usuario;
        }
    }

    public enum Tipoconexao
    {
        Managed,
        UnManaged
    }
}

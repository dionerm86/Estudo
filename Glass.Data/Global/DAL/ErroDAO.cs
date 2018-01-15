using System;
using System.Collections.Generic;
using Glass.Data.Model;
using Glass.Data.Helper;
using System.Web;
using System.Reflection;
using System.Linq;
using Glass.Configuracoes;

namespace Glass.Data.DAL
{
    public sealed class ErroDAO : BaseDAO<Erro, ErroDAO>
    {
        //private ErroDAO() { }

        #region Métodos de suporte

        private Exception GetExceptionFromErro(Erro erro, Exception innerException)
        {
            Exception ex = null;

            if (!String.IsNullOrEmpty(erro.TipoErro))
            {
                try
                {
                    Type tipo = Type.GetType(erro.TipoErro);
                    ex = Activator.CreateInstance(tipo, erro.Mensagem, innerException) as Exception;
                }
                catch { }
            }

            if (ex == null)
                ex = new Exception(erro.Mensagem, innerException);

            return ex;
        }

        private Erro GetFromParent(uint idParent)
        {
            var erro = objPersistence.LoadData("select * from erro where idParent=" + idParent).ToList().ToArray();
            return erro.Length > 0 ? erro[0] : null;
        }

        private Exception GetUsableException(Exception erro)
        {
            if (erro is HttpUnhandledException || erro is TargetInvocationException)
            {
                if (erro.InnerException != null)
                    return GetUsableException(erro.InnerException);
                else
                    return null;
            }
            else
                return erro;
        }

        private string FormatStackTrace(Exception erro)
        {
            return String.IsNullOrEmpty(erro.StackTrace) ? null :
                "<ul style='margin: 0px; padding-left: 25px'><li style='margin-top: 3px'>" +
                erro.StackTrace.Replace("\r\n", "</li><li style='margin-top: 3px'>") + "</li></ul>";
        }

        #endregion

        /// <summary>
        /// Retorna um erro com todos os seus erros internos.
        /// </summary>
        /// <param name="idErro"></param>
        /// <returns></returns>
        public Exception GetError(uint idErro)
        {
            List<Erro> erro = new List<Erro>();
            erro.Add(GetElementByPrimaryKey(idErro));

            Erro inner = erro[0];
            while ((inner = GetFromParent(inner.IdErro)) != null)
                erro.Add(inner);

            Exception retorno = null;
            for (int i = erro.Count - 1; i >= 0; i--)
                retorno = GetExceptionFromErro(erro[i], retorno);

            return retorno;
        }

        public void InserirFromException(string url, Exception erro)
        {
            InserirFromException(url, erro, null);
        }

        public void InserirFromException(string url, Exception erro, uint? idParent)
        {
            // O try catch é essencial nesta parte do código, devido ao erro recorrente do mysql de NullReferenceException ao inserir dados no banco,
            // como esta função geralmente é chamada no catch dos métodos, não pode lançar erro de forma alguma
            try
            {
                erro = GetUsableException(erro);
                if (erro == null)
                    return;

                Erro novo = new Erro();
                novo.IdParent = idParent;
                novo.UrlErro = url != null && url.Length > 300 ? "..." + url.Substring(url.Length - 297) : url;
                novo.DataErro = DateTime.Now;
                novo.IdFuncErro = UserInfo.GetUserInfo != null ? UserInfo.GetUserInfo.CodUser : 0;
                novo.Mensagem = erro.Message;
                novo.Mensagem = novo.Mensagem.Length > 400 ? novo.Mensagem.Substring(0, 397) + "..." : novo.Mensagem;
                novo.TipoErro = erro.GetType().AssemblyQualifiedName;
                novo.NecessitaEnvio = NecessitaEnvio(novo.Mensagem);

                Exception trace = erro;
                while (String.IsNullOrEmpty(trace.StackTrace) && trace.InnerException != null)
                    trace = trace.InnerException;

                novo.Trace = trace.StackTrace;
                //novo.Trace = novo.Trace != null && novo.Trace.Length > 1500 ? novo.Trace.Substring(0, 1497) + "..." : novo.Trace;

                uint idErro = ErroDAO.Instance.Insert(novo);

                if (erro.InnerException != null)
                    InserirFromException(null, erro.InnerException, idErro);
            }
            catch
            {

            }
        }

        /// <summary>
        /// Verifica se o erro necessita ser enviado
        /// </summary>
        /// <param name="mensagem"></param>
        /// <returns></returns>
        public bool NecessitaEnvio(string mensagem)
        {
            if (string.IsNullOrEmpty(mensagem))
                return false;

            mensagem = mensagem.ToLower();

            var catalogo = new List<string>()
            {
                "referência de objeto não definida para uma instância de um objeto.",
                "object reference not set to an instance of an object.",
                "a cadeia de caracteres de entrada não estava em um formato incorreto.",
                "input string was not in a correct format.",
                "deadlock found when trying to get lock; try restarting transaction",
                "you have an error in your sql syntax; check the manual that corresponds to your mysql server version for the right syntax",
                "o índice estava fora do intervalo. ele deve ser não-negativo e menor que o tamanho da coleção.",
                "index was out of range. must be non-negative and less than the size of the collection.",
                "lock wait timeout exceeded; try restarting transaction",
                "unknown column '",
                "startindex não pode ser menor que zero.",
                "data too long for column '"
            };

            foreach (var item in catalogo)
                if (mensagem.Contains(item))
                    return true;

            return false;
        }

        public string GetErrorMessage(Exception erro)
        {
            erro = GetUsableException(erro);
            if (erro == null)
                return "";

            string retorno = UserInfo.GetUserInfo.IsAdminSync ? "[" + erro.GetType().FullName + "] " : "";
            retorno += erro.Message;

            if (UserInfo.GetUserInfo.IsAdminSync)
                retorno += FormatStackTrace(erro);

            if (erro.InnerException != null)
                retorno += "<div style='padding-left: 12px'><br /><div style='margin-bottom: 3px; font-weight: bold'>Erro interno:</div>" + GetErrorMessage(erro.InnerException) + "</div>";

            return retorno;
        }

        internal void LimpaLogErro()
        {
            using (var session = new GDA.GDATransaction())
            {
                try
                {
                    session.BeginTransaction();

                    if (ExecuteScalar<int>(session, "SELECT COUNT(*) FROM erro") < 50000)
                    {
                        session.Close();
                        return;
                    }

                    do
                    {
                        objPersistence.ExecuteCommand(session, "DELETE FROM erro LIMIT 50000");

                    } while (ExecuteScalar<int>(session, "SELECT COUNT(*) FROM erro") > 0);

                    

                    session.Commit();
                    session.Close();

                }
                catch
                {
                    session.Rollback();
                    session.Close();
                }
            }
        }

        /// <summary>
        /// Recupera um erro marcado como "necessita envio"
        /// </summary>
        /// <returns></returns>
        public List<string[]> ObtemParaEnvio()
        {
            var lstErro = objPersistence.LoadData("Select * From erro Where necessitaEnvio=1 And Coalesce(enviado, 0)=0 limit 10").ToList();

            if (lstErro == null)
                return null;

            var lista = new List<string[]>();

            foreach (var item in lstErro)
            {
                lista.Add(new string[] {
                    FuncionarioDAO.Instance.GetNome(item.IdFuncErro),
                    item.TipoErro,
                    item.Mensagem,
                    item.Trace,
                    item.DataErro.ToString(),
                    item.UrlErro,
                    item.IdErro.ToString() });
            }

            return lista;
        }

        /// <summary>
        /// Marca que o erro foi enviado
        /// </summary>
        /// <param name="idErro"></param>
        public void MarcaEnviado(List<string[]> lstErro)
        {
            var idErro = lstErro.Select(f => f[6]).FirstOrDefault();

            if (string.IsNullOrEmpty(idErro))
                idErro = "0";

            objPersistence.ExecuteCommand(string.Format("Update erro Set enviado=true Where idErro In ({0})", idErro));
        }
    }
}

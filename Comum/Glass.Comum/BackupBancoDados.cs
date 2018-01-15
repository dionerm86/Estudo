using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace Glass
{
    public class BackupBancoDados
    {
        #region Campos Privados

        private string token = null;
        private CookieContainer cookies = new CookieContainer();
        private readonly string baseUrl = "http://" + DBUtils.GetDBServer + "/";

        #endregion

        #region Métodos de Suporte

        /// <summary>
        /// Cria um objeto para fazer requisição a uma página do phpMyAdmin do banco de dados.
        /// </summary>
        /// <param name="pagina">A página que será requisitada.</param>
        /// <param name="dataToSend">Os dados que serão enviados.</param>
        /// <returns></returns>
        private HttpWebResponse RequestToDBWithPost(string pagina, string dadosEnviar)
        {
            // String que salva a página de requisição
            string page = baseUrl + pagina;

            // Cria o objeto de requisição
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(page);
            request.Method = "post";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = dadosEnviar.Length;
            request.CookieContainer = cookies;

            // Escreve os dados do POST no objeto de requisição
            using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                writer.Write(dadosEnviar);

            // Verifica se já existem os cookies de autenticação
            if (cookies.Count == 0)
            {
                // Faz a requisição uma vez para obter o cookie de autenticação
                request.GetResponse().Close();

                // Recria o objeto de requisição
                request = (HttpWebRequest)HttpWebRequest.Create(page);
                request.CookieContainer = cookies;
            }

            // Retorna a resposta do objeto de requisição
            return (HttpWebResponse)request.GetResponse();
        }

        #endregion

        #region Login

        /// <summary>
        /// Faz o login com o sistema do phpMyAdmin.
        /// </summary>
        private void Login()
        {
            // Cria uma string com os dados de envio do formulário
            string dados = "pma_username=" + DBUtils.GetDBUser +
                "&pma_password=" + DBUtils.GetDBPassword +
                "&lang=ptbr-utf-8" +
                "&convcharset=iso-8859-1" +
                "&db=" + DBUtils.GetDBName;

            // Cria o objeto de requisição, invoca-o e salva alguns dados da página
            HttpWebResponse resposta = RequestToDBWithPost("index.php", dados);

            CookieCollection getToken = cookies.GetCookies(new Uri(baseUrl));
            token = getToken["phpMyAdmin"].Value;
        }

        #endregion

        #region Exportação

        /// <summary>
        /// Invoca a página de exportação do banco de dados.
        /// </summary>
        /// <returns>Um Stream com a resposta da página.</returns>
        private Stream Export()
        {
            // Cria uma string com os dados de envio do formulário
            string dados = "phpMyAdmin=" +
                "&token=" + token +
                "&export_type=server" +
                "&db_select[]=" + DBUtils.GetDBName +
                "&what=sql" +
                "&sql_header_comment=" +
                "&sql_use_transaction=" +
                "&sql_disable_fk=" +
                "&sql_compatibility=NONE" +
                "&sql_drop_database=" +
                "&sql_structure=something" +
                "&sql_drop_table=" +
                "&sql_if_not_exists=something" +
                "&sql_auto_increment=something" +
                "&sql_backquotes=something" +
                "&sql_procedure_function=" +
                "&sql_dates=" +
                "&sql_data=something" +
                "&sql_columns=something" +
                "&sql_extended=something" +
                "&sql_max_query_size=50000" +
                "&sql_delayed=" +
                "&sql_ignore=" +
                "&sql_hex_for_blob=something" +
                "&sql_type=INSERT" +
                "&asfile=sendit" +
                "&saveit=" +
                "&onserverover=" +
                "&filename_template=Backup" +
                "&remember_template=on" +
                "&compression=zip";

            // Cria o objeto de requisição, invoca-o e retorna a resposta
            return RequestToDBWithPost("export.php", dados).GetResponseStream();
        }

        #endregion

        /// <summary>
        /// Retorna um Stream com o backup do banco de dados.
        /// </summary>
        /// <returns></returns>
        public Stream BackupMySQLDatabase()
        {
            Login();
            return Export();
        }
    }
}

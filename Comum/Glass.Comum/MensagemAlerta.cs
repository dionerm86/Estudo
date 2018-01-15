using System;
using Colosoft;

namespace Glass
{
    public static class MensagemAlerta
    {
        #region Registra uma mensagem na página

        /// <summary>
        /// Formata uma mensagem para ser usada no JavaScript.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="removerQuebraLinha"></param>
        /// <returns></returns>
        private static string FormatMessage(string message, bool removerQuebraLinha)
        {
            return message.Replace("\"", String.Empty).Replace("'", String.Empty).Replace("\n ", "\n").
                Replace("\n", removerQuebraLinha ? " " : "\\n").Replace("\r", String.Empty);
        }

        /// <summary>
        /// Registra uma mensagem de erro na página
        /// </summary>
        /// <param name="message">Mensagem identificada</param>
        /// <param name="ex"></param>
        /// <param name="page"></param>
        public static void ErrorMsg(string message, Exception ex, System.Web.UI.Page page)
        {
            ShowMsg(FormatErrorMsg(message, ex), page);
        }

        /// <summary>
        /// Registra uma mensagem de erro.
        /// </summary>
        /// <param name="operacao">Descrição da operação realizada.</param>
        /// <param name="resultado">Resultado contendo a mensagem do erro.</param>
        /// <param name="page">Página.</param>
        public static void ErrorMsg(string operacao, Colosoft.Business.SaveResult resultado)
        {
            resultado.Require("resultado").NotNull();
            var page = System.Web.HttpContext.Current.Handler as System.Web.UI.Page;
            if (resultado.Message != null)
                ShowMsg(string.Format("{0}\r\b{1}", operacao, resultado.Message.Format()), page);
            else
                ShowMsg(operacao, page);
        }

        /// <summary>
        /// Registra uma mensagem de erro.
        /// </summary>
        /// <param name="operacao">Descrição da operação realizada.</param>
        /// <param name="resultado">Resultado contendo a mensagem do erro.</param>
        public static void ErrorMsg(string operacao, Colosoft.Business.DeleteResult resultado)
        {
            resultado.Require("resultado").NotNull();
            var page = System.Web.HttpContext.Current.Handler as System.Web.UI.Page;
            if (resultado.Message != null)
                ShowMsg(string.Format("{0}\r\b{1}", operacao, resultado.Message.Format()), page);
            else
                ShowMsg(operacao, page);
        }

        /// <summary>
        /// Formata uma mensagem de erro, verificando se o erro ocorrido foi por ferir integridade referencial,
        /// retirando aspas simples, verificando se a exception possui uma innerException e concatenando a 
        /// variável "message" com a mensagem da exception
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string FormatErrorMsg(string message, Exception ex)
        {
            return FormatErrorMsg(message, ex, false);
        }

        /// <summary>
        /// Formata uma mensagem de erro, verificando se o erro ocorrido foi por ferir integridade referencial,
        /// retirando aspas simples, verificando se a exception possui uma innerException e concatenando a 
        /// variável "message" com a mensagem da exception
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string FormatErrorMsg(string message, Exception ex, bool removerQuebraLinha)
        {
            string msg = String.Empty;

            if (ex != null)
                msg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;

            msg = msg.ToLower().Contains("cannot delete or update a parent row") ?
                "Este registro não pôde ser removido pois existem outros registros relacionados à ele." : msg;

            if (msg.ToLower().Contains("data too long for column "))
                msg = string.Format("O campo {0} excedeu o limite de caracteres.", msg.Replace("Data too long for column ", ""));

            message = String.IsNullOrEmpty(message) ? msg : message + " Erro: " + msg;

            if (msg.ToLower().Contains("object reference not set") && ex != null)
            {
                // Get stack trace for the exception with source file information
                var st = new System.Diagnostics.StackTrace(ex, true);
                
                // Get the top stack frame
                var frame = st.GetFrame(0);

                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();

                msg += string.Format(" fl: {0} {1}", frame.GetFileName(), line);
            }

            return FormatMessage(message, removerQuebraLinha);
        }

        /// <summary>
        /// Mostra a mensagem de erro na tela
        /// </summary>
        /// <param name="message"></param>
        /// <param name="page"></param>
        public static void ShowMsg(string message, System.Web.UI.Page page)
        {
            page.ClientScript.RegisterStartupScript(typeof(string), Guid.NewGuid().ToString(),
                "alert('" + FormatMessage(message, false) + "');", true);
        }

        #endregion
    }
}

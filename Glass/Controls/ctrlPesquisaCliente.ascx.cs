using Glass.Data.DAL;
using System;

namespace Glass.UI.Web.Controls
{
    public partial class ctrlPesquisaCliente : BaseUserControl
    {
        #region Propriedades

        /// <summary>
        /// Identificador do cliente
        /// </summary>
        public int IdCliente
        {
            get { return txtNumCliente.Text.StrParaInt(); }
        }

        /// <summary>
        /// Nome do cliente
        /// </summary>
        public string NomeCliente
        {
            get { return txtNomeCliente.Text; }
        }

        #endregion

        #region Métodos Públicos / Protegidos

        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(ctrlPesquisaCliente));
        }

        #endregion

        #region Métodos AJAX

        /// <summary>
        /// Busca o cliente em tempo real
        /// </summary>
        /// <param name="idCli"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string ObterNomeCliente(string idCli)
        {
            var idCliente = idCli.StrParaUint();

            if (!ClienteDAO.Instance.Exists(idCliente))
                throw new Exception("Cliente não encontrado.");

            return ClienteDAO.Instance.GetNome(idCliente);
        }

        #endregion
    }
}
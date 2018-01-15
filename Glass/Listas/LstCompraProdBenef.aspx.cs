using System;
using System.Web.UI;
using Glass.Data.DAL;

namespace Glass.UI.Web.Listas
{
    public partial class LstCompraProdBenef : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(LstCompraProdBenef));

            if (!IsPostBack)
            {
                ctrlDataIniFin.Data = DateTime.Now.AddDays(-15);
                ctrlDataFimFin.Data = DateTime.Now;
            }
        }

        #region Métodos Ajax

        /// <summary>
        /// Busca o cliente em tempo real
        /// </summary>
        /// <param name="idCli"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string GetCli(string idCli)
        {
            if (String.IsNullOrEmpty(idCli) || !ClienteDAO.Instance.Exists(Glass.Conversoes.StrParaUint(idCli)))
                return "Erro;Cliente não encontrado.";
            else
                return "Ok;" + ClienteDAO.Instance.GetNome(Glass.Conversoes.StrParaUint(idCli));
        }

        #endregion

        #region Métodos da pagina

        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdPedidoEspelho.PageIndex = 0;
        }

        private bool corAlternada = true;

        protected string GetAlternateClass()
        {
            corAlternada = !corAlternada;
            return corAlternada ? "alt" : "";
        }

        #endregion
    }
}
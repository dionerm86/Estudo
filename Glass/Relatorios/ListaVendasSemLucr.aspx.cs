using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using Glass.Data.DAL;

namespace Glass.UI.Web.Relatorios
{
    public partial class ListaVendasSemLucr : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Relatorios.ListaVendasSemLucr));

            if (!IsPostBack)
            {
                ((TextBox)ctrlDataIni.FindControl("txtData")).Text = DateTime.Now.ToString("01/MM/yyyy");
                ((TextBox)ctrlDataFim.FindControl("txtData")).Text = DateTime.Now.ToString("dd/MM/yyyy");
    
                LoginUsuario login = UserInfo.GetUserInfo;
    
                if (login.TipoUsuario != (int)Data.Helper.Utils.TipoFuncionario.Administrador && login.TipoUsuario != (int)Data.Helper.Utils.TipoFuncionario.Gerente)
                {
                    drpLoja.DataBind();
                    drpLoja.SelectedValue = login.IdLoja.ToString();
                    drpLoja.Enabled = false;
    
                    drpVendedor.DataBind();
                    drpVendedor.SelectedValue = login.CodUser.ToString();
                    drpVendedor.Enabled = false;
                }
            }
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdVendasLucr.PageIndex = 0;
        }

        #region Métodos AJAX

        /// <summary>
        /// Busca o cliente em tempo real.
        /// </summary>
        [Ajax.AjaxMethod()]
        public string GetCli(string idCli)
        {
            if (string.IsNullOrEmpty(idCli) || !ClienteDAO.Instance.Exists(idCli.StrParaUint()))
                return "Erro;Cliente não encontrado.";
            else
                return "Ok;" + ClienteDAO.Instance.GetNome(idCli.StrParaUint());
        }

        #endregion
    }
}

using System;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.UI.Web.Listas
{
    public partial class LstCliente : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(LstCliente));
        }

        protected void drpSituacao_DataBound(object sender, EventArgs e)
        {
            //if (!IsPostBack && ClienteConfig.ListarAtivosPadrao)
            //    drpSituacao.SelectedValue = ((int)SituacaoCliente.Ativo).ToString();
        }
    }
}

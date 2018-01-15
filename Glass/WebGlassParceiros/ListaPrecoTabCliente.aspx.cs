using System;
using System.Web.UI;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.UI.Web.WebGlassParceiros
{
    public partial class ListaPrecoTabCliente : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));

            var idCli = UserInfo.GetUserInfo.IdCliente;

            if (idCli.GetValueOrDefault(0) <= 0)
                Response.Redirect("Main.aspx");

            hdfIdCli.Value = idCli.Value.ToString();
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            lnkPesq_Click(sender, e);
        }
    
        protected void lnkPesq_Click(object sender, EventArgs e)
        {
            grdProduto.PageIndex = 0;
        }
    
        protected void cbdSubgrupo_DataBound(object sender, EventArgs e)
        {
            var mudar = ProdutoConfig.TelaPrecoTabelaClienteRelatorio.AlterarSubgruposSelecionados;

            var idCliente = UserInfo.GetUserInfo.IdCliente.GetValueOrDefault();
            var idsSubgrupoCliente = idCliente > 0 ? ClienteDAO.Instance.ObtemIdsSubgrupo(idCliente) : string.Empty;

            /* Chamado 52406. */
            if (!string.IsNullOrEmpty(idsSubgrupoCliente))
                cbdSubgrupo.SelectedValue = idsSubgrupoCliente;
            else if (!string.IsNullOrEmpty(mudar.Key))
                cbdSubgrupo.SelectedValue = SubgrupoProdDAO.Instance.GetSubgruposTemperados().Replace(mudar.Key, mudar.Value);
            else
                cbdSubgrupo.SelectedValue = SubgrupoProdDAO.Instance.ObtemSubgruposMarcadosFiltro(hdfIdCli.Value.StrParaInt());
        }
    }
}

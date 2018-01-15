using System;
using System.Web.UI;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.UI.Web.Relatorios
{
    public partial class ListaPrecoTabCliente : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            Ajax.Utility.RegisterTypeForAjax(typeof(ListaPrecoTabCliente));

            grdProduto.Columns[3].Visible = !PedidoConfig.LiberarPedido;

            // Define se a opção de exibir percentual de desconto/acréscimo vai vir ficar marcada por padrão
            if (!IsPostBack)
            {
                chkExibirPerc.Checked = true;                

                var idCli = Conversoes.StrParaUint(Request["idCli"]);
                if (ClienteDAO.Instance.Exists(idCli))
                {
                    txtNumCli.Text = idCli.ToString();
                    txtNomeCliente.Text = ClienteDAO.Instance.GetNome(idCli);
                    grdProduto.DataBind();
                }
            }
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            lnkPesq_Click(sender, e);
        }
    
        protected void lnkPesq_Click(object sender, EventArgs e)
        {
            grdProduto.PageIndex = 0;
        }
    
        protected void drpGrupo_DataBound(object sender, EventArgs e)
        {
            if (!IsPostBack)
                drpGrupo.SelectedValue = ((int)Glass.Data.Model.NomeGrupoProd.Vidro).ToString();
        }
    
        protected void cbdSubgrupo_DataBound(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ProdutoConfig.TelaPrecoTabelaClienteRelatorio.SubgruposPadraoFiltro))
                cbdSubgrupo.SelectedValue = ProdutoConfig.TelaPrecoTabelaClienteRelatorio.SubgruposPadraoFiltro;
            else
                cbdSubgrupo.SelectedValue = SubgrupoProdDAO.Instance.ObtemSubgruposMarcadosFiltro(Conversoes.StrParaInt(txtNumCli.Text));

            var mudar = ProdutoConfig.TelaPrecoTabelaClienteRelatorio.AlterarSubgruposSelecionados;

            if (!string.IsNullOrEmpty(mudar.Key))
                cbdSubgrupo.SelectedValue = cbdSubgrupo.SelectedValue.Replace(mudar.Key, mudar.Value);
        }

        [Ajax.AjaxMethod]
        public string ObtemIdsSubGrupoCliente(string idCli)
        {
            try
            {
                return SubgrupoProdDAO.Instance.ObtemSubgruposMarcadosFiltro(Conversoes.StrParaInt(idCli));                
            }
            catch
            {
                return "";
            }
        }

        protected void grdProduto_DataBound(object sender, EventArgs e)
        {
            if (chkExibirValorOriginal.Checked)
                grdProduto.Columns[4].Visible = false;
            else
                grdProduto.Columns[4].Visible = true;
        }     
    }
}

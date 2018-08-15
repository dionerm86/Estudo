using System;
using System.Web.UI;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.UI.Web.Relatorios
{
    public partial class ListaPrecoTab : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));

            // Define se a opção de exibir percentual de desconto/acréscimo vai vir ficar marcada por padrão
            if (!IsPostBack)
            {
                chkExibirPerc.Checked = true;                
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

            /* Chamado 52406. */
            if (!string.IsNullOrEmpty(ProdutoConfig.TelaPrecoTabelaClienteRelatorio.SubgruposPadraoFiltro))
                cbdSubgrupo.SelectedValue = ProdutoConfig.TelaPrecoTabelaClienteRelatorio.SubgruposPadraoFiltro;

            var mudar = ProdutoConfig.TelaPrecoTabelaClienteRelatorio.AlterarSubgruposSelecionados;

            if (!string.IsNullOrEmpty(mudar.Key))
                cbdSubgrupo.SelectedValue = cbdSubgrupo.SelectedValue.Replace(mudar.Key, mudar.Value);
        }

        protected void grdProduto_DataBound(object sender, EventArgs e)
        {
            if (chkExibirValorOriginal.Checked)
                grdProduto.Columns[3].Visible = false;
            else
                grdProduto.Columns[3].Visible = true;
        }     
    }
}

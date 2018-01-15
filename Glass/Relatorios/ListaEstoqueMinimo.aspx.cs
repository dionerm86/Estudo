using System;
using System.Web.UI;
using Glass.Data.Helper;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.UI.Web.Relatorios
{
    public partial class ListaEstoqueMinimo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
    
            if (!IsPostBack)
                drpLoja.SelectedValue = UserInfo.GetUserInfo.IdLoja.ToString();
    
            grdProdutos.Columns[7].Visible = PedidoConfig.LiberarPedido;
            lnkSugestaoProducao.Visible = PCPConfig.ControlarProducao;
            lnkLancarEstoqueMin.Visible = UserInfo.GetUserInfo.TipoUsuario == (uint)Data.Helper.Utils.TipoFuncionario.Administrador &&
                drpGrupo.SelectedValue != "" && drpGrupo.SelectedValue != "0";
    
            // Este controle não possui esta propriedade.
            //ctrlSelCorProd1.IdGrupoProd = Glass.Conversoes.StrParaUintNullable(drpGrupo.SelectedValue);
    
            drpTipoBox.Visible = drpGrupo.SelectedValue == ((int)Glass.Data.Model.NomeGrupoProd.Vidro).ToString() &&
                drpSubgrupo.SelectedItem.Text.ToLower().Contains("box padrao");
    
            if (!drpTipoBox.Visible)
                drpTipoBox.SelectedIndex = 0;
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdProdutos.PageIndex = 0;
        }
    
        protected void lnkPesq_Click(object sender, EventArgs e)
        {
            grdProdutos.PageIndex = 0;
        }
    
        protected void lnkSugestaoCompra_PreRender(object sender, EventArgs e)
        {
            // Atualiza o link para as telas de sugestão de compra/produção
            lnkSugestaoCompra.NavigateUrl = "~/Cadastros/CadSugestaoCompra.aspx?idLoja=" + drpLoja.SelectedValue;
            lnkSugestaoProducao.NavigateUrl = lnkSugestaoCompra.NavigateUrl.Replace("?", "?producao=1&") + "&idGrupoProd=" + (int)Glass.Data.Model.NomeGrupoProd.Vidro;
    
            if (!String.IsNullOrEmpty(drpGrupo.SelectedValue) && Glass.Conversoes.StrParaInt(drpGrupo.SelectedValue) > 0)
                lnkSugestaoCompra.NavigateUrl += "&idGrupoProd=" + drpGrupo.SelectedValue;
    
            if (!String.IsNullOrEmpty(drpSubgrupo.SelectedValue) && Glass.Conversoes.StrParaInt(drpSubgrupo.SelectedValue) > 0)
            {
                lnkSugestaoCompra.NavigateUrl += "&idSubgrupoProd=" + drpSubgrupo.SelectedValue;
                if (drpGrupo.SelectedValue == ((int)Glass.Data.Model.NomeGrupoProd.Vidro).ToString())
                    lnkSugestaoProducao.NavigateUrl += "&idSubgrupoProd=" + drpSubgrupo.SelectedValue;
            }
    
            if (!String.IsNullOrEmpty(txtCodProd.Text))
            {
                lnkSugestaoCompra.NavigateUrl += "&codInterno=" + txtCodProd.Text;
                Produto prod = ProdutoDAO.Instance.GetByCodInterno(txtCodProd.Text);
                if (prod.IdGrupoProd == (int)Glass.Data.Model.NomeGrupoProd.Vidro)
                    lnkSugestaoProducao.NavigateUrl += "&codInterno=" + txtCodProd.Text;
            }
    
            if (!String.IsNullOrEmpty(txtDescr.Text))
            {
                lnkSugestaoCompra.NavigateUrl += "&descricao=" + txtDescr.Text;
                lnkSugestaoProducao.NavigateUrl += "&descricao=" + txtDescr.Text;
            }
        }
    }
}

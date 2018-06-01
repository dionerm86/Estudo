using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Microsoft.Practices.ServiceLocation;
using Sync.Controls;
using System.Linq;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadSubgrupoProduto : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            grdSubgrupoProd.Register(true, true);
            odsSubgrupoProd.Register();

            grdSubgrupoProd.Columns[6].Visible = Glass.Data.DAL.GrupoProdDAO.Instance.IsVidro(Glass.Conversoes.StrParaInt(Request["idGrupoProd"]));
            grdSubgrupoProd.Columns[7].Visible = Glass.Data.DAL.GrupoProdDAO.Instance.IsVidro(Glass.Conversoes.StrParaInt(Request["idGrupoProd"]));
            grdSubgrupoProd.Columns[12].Visible = Configuracoes.OrdemCargaConfig.UsarControleOrdemCarga;
            grdSubgrupoProd.Columns[14].Visible = Glass.Conversoes.StrParaInt(Request["IdGrupoProd"]) != (int)Glass.Data.Model.NomeGrupoProd.Vidro && 
                Configuracoes.PedidoConfig.DadosPedido.BloquearItensTipoPedido;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.Title.Contains(":"))
                Page.Title += ": " + Request["descrGrupo"];

            if (!IsPostBack)
                lblDadosPerdidos.Visible = Glass.Conversoes.StrParaInt(Request["IdGrupoProd"]) == (int)Glass.Data.Model.NomeGrupoProd.Vidro;
        }
    
        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            try
            {
                var subGrupo = new Glass.Global.Negocios.Entidades.SubgrupoProd();
                subGrupo.Descricao = ((TextBox)grdSubgrupoProd.FooterRow.FindControl("txtDescricaoIns")).Text;
    
                if (String.IsNullOrEmpty(subGrupo.Descricao))
                {
                    Glass.MensagemAlerta.ShowMsg("Informe o nome do subgrupo.", Page);
                    return;
                }

                var tipoCalculo = Data.Model.TipoCalculoGrupoProd.Qtd;

                if (Enum.TryParse<Data.Model.TipoCalculoGrupoProd>(
                    ((DropDownList)grdSubgrupoProd.FooterRow.FindControl("drpCalculoIns")).SelectedValue, out tipoCalculo))
                    subGrupo.TipoCalculo = tipoCalculo;

                var tipoCalculoNf = Data.Model.TipoCalculoGrupoProd.Qtd;

                if (Enum.TryParse<Data.Model.TipoCalculoGrupoProd>(
                    ((DropDownList)grdSubgrupoProd.FooterRow.FindControl("drpCalculoNfIns")).SelectedValue, out tipoCalculoNf))
                    subGrupo.TipoCalculoNf = tipoCalculoNf;

                subGrupo.IdGrupoProd = Glass.Conversoes.StrParaInt(Request["idGrupoProd"]);
                subGrupo.BloquearEstoque = ((CheckBox)grdSubgrupoProd.FooterRow.FindControl("chkBloquearEstoque")).Checked;
                subGrupo.AlterarEstoque = ((CheckBox)grdSubgrupoProd.FooterRow.FindControl("chkAlterarEstoque")).Checked;
                subGrupo.AlterarEstoqueFiscal = ((CheckBox)grdSubgrupoProd.FooterRow.FindControl("chkAlterarEstoqueFiscal")).Checked;
                subGrupo.ExibirMensagemEstoque = ((CheckBox)grdSubgrupoProd.FooterRow.FindControl("chkExibirMensagemEstoque")).Checked;
                subGrupo.IsVidroTemperado = ((CheckBox)grdSubgrupoProd.FooterRow.FindControl("chkVidroTemperado")).Checked;
                subGrupo.NumeroDiasMinimoEntrega = Glass.Conversoes.StrParaIntNullable(((TextBox)grdSubgrupoProd.FooterRow.FindControl("txtNumDiasMinEntrega")).Text);
                subGrupo.DiaSemanaEntrega = Glass.Conversoes.StrParaIntNullable(((DropDownList)grdSubgrupoProd.FooterRow.FindControl("drpDiaSemana")).SelectedValue);
                subGrupo.GeraVolume = ((CheckBox)grdSubgrupoProd.FooterRow.FindControl("chkGeraVolume")).Checked;
                subGrupo.LiberarPendenteProducao = ((CheckBox)grdSubgrupoProd.FooterRow.FindControl("chkLibPendenteProducao")).Checked;
                subGrupo.PermitirItemRevendaNaVenda = ((CheckBox)grdSubgrupoProd.FooterRow.FindControl("chkPermitirItemRevendaNaVenda")).Checked;
                subGrupo.IdsLojaAssociacao = ((CheckBoxListDropDown)grdSubgrupoProd.FooterRow.FindControl("cblLoja")).SelectedValues;

                subGrupo.TipoSubgrupo = (Data.Model.TipoSubgrupoProd)Enum.Parse(typeof(Data.Model.TipoSubgrupoProd), 
                    ((DropDownList)grdSubgrupoProd.FooterRow.FindControl("drpTipoSubgrupo")).SelectedValue);

                var fluxo = ServiceLocator.Current.GetInstance<Glass.Global.Negocios.IGrupoProdutoFluxo>();

                var resultado = fluxo.SalvarSubgrupoProduto(subGrupo);

                if (!resultado)
                    Glass.MensagemAlerta.ErrorMsg("Falha ao inserir Subgrupo de Produto.", resultado);
                else
                    grdSubgrupoProd.DataBind();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir Subgrupo de Produto.", ex, Page);
            }
        }
    
        protected void grdSubgrupoProd_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GridView grid = (GridView)e.Row.FindControl("grdLog");
            if (grid != null)
                grid.DataBind();
        }

        protected void grdSubgrupoProd_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "AtivarProdutos":
                    ProdutoDAO.Instance.AlterarSituacaoProduto(Situacao.Ativo, null, e.CommandArgument.ToString().StrParaInt());
                    MensagemAlerta.ShowMsg("Produtos ativados.", this.Page);
                    break;
                case "InativarProdutos":
                    ProdutoDAO.Instance.AlterarSituacaoProduto(Situacao.Inativo, null, e.CommandArgument.ToString().StrParaInt());
                    MensagemAlerta.ShowMsg("Produtos inativados.", this.Page);
                    break;
            }
        }

    }
}

using System;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Data.Model;
using Microsoft.Practices.ServiceLocation;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadGrupoProduto : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            grdGrupoProd.Register(true, true);
            odsGrupoProd.Register();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            grdGrupoProd.Columns[10].Visible = Configuracoes.OrdemCargaConfig.UsarControleOrdemCarga;
        }
    
        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            try
            {
                var grupo = new Glass.Global.Negocios.Entidades.GrupoProd();
                grupo.Descricao = ((TextBox)grdGrupoProd.FooterRow.FindControl("txtDescricaoIns")).Text;

                var tipoCalculo = TipoCalculoGrupoProd.Qtd;

                if (Enum.TryParse<TipoCalculoGrupoProd>(
                    ((DropDownList)grdGrupoProd.FooterRow.FindControl("drpCalculoIns")).SelectedValue, out tipoCalculo))
                    grupo.TipoCalculo = tipoCalculo;

                var tipoCalculoNf = TipoCalculoGrupoProd.Qtd;

                if (Enum.TryParse<TipoCalculoGrupoProd>(
                    ((DropDownList)grdGrupoProd.FooterRow.FindControl("drpCalculoNfIns")).SelectedValue, out tipoCalculoNf))
                    grupo.TipoCalculoNf = tipoCalculoNf;

                grupo.BloquearEstoque = ((CheckBox)grdGrupoProd.FooterRow.FindControl("chkBloquearEstoque")).Checked;
                grupo.AlterarEstoque = ((CheckBox)grdGrupoProd.FooterRow.FindControl("chkAlterarEstoque")).Checked;
                grupo.AlterarEstoqueFiscal = ((CheckBox)grdGrupoProd.FooterRow.FindControl("chkAlterarEstoqueFiscal")).Checked;

                grupo.TipoGrupo = (TipoGrupoProd)Enum.Parse(typeof(TipoGrupoProd), ((DropDownList)grdGrupoProd.FooterRow.FindControl("drpTipoGrupo")).SelectedValue);
                grupo.ExibirMensagemEstoque = ((CheckBox)grdGrupoProd.FooterRow.FindControl("chkExibirMensagemEstoque")).Checked;
                grupo.GeraVolume = ((CheckBox)grdGrupoProd.FooterRow.FindControl("chkGeraVolume")).Checked;
    
                if (String.IsNullOrEmpty(grupo.Descricao))
                {
                    Glass.MensagemAlerta.ShowMsg("Informe o nome do grupo.", Page);
                    return;
                }

                var fluxo = ServiceLocator.Current.GetInstance<Glass.Global.Negocios.IGrupoProdutoFluxo>();

                var resultado = fluxo.SalvarGrupoProduto(grupo);

                if (!resultado)
                    Glass.MensagemAlerta.ErrorMsg("Falha ao inserir Grupo de Produto.", resultado);
                else
                    grdGrupoProd.DataBind();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir Grupo de Produto.", ex, Page);
            }
        }

        protected void grdGrupoProd_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "AtivarProdutos":
                    ProdutoDAO.Instance.AlterarSituacaoProduto(Situacao.Ativo, e.CommandArgument.ToString().StrParaInt(), null);
                    MensagemAlerta.ShowMsg("Produtos ativados.", this.Page);
                    break;
                case "InativarProdutos":
                    ProdutoDAO.Instance.AlterarSituacaoProduto(Situacao.Inativo, e.CommandArgument.ToString().StrParaInt(), null);
                    MensagemAlerta.ShowMsg("Produtos inativados.", this.Page);
                    break;
            }
        }
    }
}

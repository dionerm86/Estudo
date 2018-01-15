using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadDescontoAcrescimoCliente : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
    
            if (!IsPostBack)
            {
                if (Glass.Conversoes.StrParaUint(Request["idCliente"]) > 0)
                    lblCliente.Text = Request["idCliente"] + " - " + ClienteDAO.Instance.GetNome(Glass.Conversoes.StrParaUint(Request["idCliente"]));
    
                else if (Glass.Conversoes.StrParaUint(Request["idTabelaDesconto"]) > 0)
                {
                    lblTituloCliente.Text = "Tabela:";
                    lblCliente.Text = TabelaDescontoAcrescimoClienteDAO.Instance.GetDescricao(Glass.Conversoes.StrParaUint(Request["idTabelaDesconto"]));
                    imgIniciarCopia.Visible = false;
                }
    
                dadosGrupo.Visible = Request["idGrupo"] != null || Request["idSubgrupo"] != null;
                grdDesconto.Columns[0].Visible = !dadosGrupo.Visible;
                grdDesconto.Columns[1].Visible = dadosGrupo.Visible;
                grdDesconto.Columns[6].Visible = dadosGrupo.Visible;
                grdDesconto.Columns[7].Visible = dadosGrupo.Visible;
                grdDesconto.Columns[4].Visible = Configuracoes.PedidoConfig.UsarTabelaDescontoAcrescimoPedidoAVista;
                //btnAtualizar.Visible = !dadosGrupo.Visible;

                if (!String.IsNullOrEmpty(Request["idGrupo"]))
                    lblGrupo.Text = GrupoProdDAO.Instance.GetDescricao(Glass.Conversoes.StrParaInt(Request["idGrupo"]));
    
                if (!String.IsNullOrEmpty(Request["idSubgrupo"]))
                    lblSubgrupo.Text = SubgrupoProdDAO.Instance.GetDescricao(Glass.Conversoes.StrParaInt(Request["idSubgrupo"]));
                else
                {
                    Label4.Visible = false;
                    lblSubgrupo.Visible = false;
                }
            }
        }
    
        protected void btnAtualizar_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (GridViewRow r in grdDesconto.Rows)
                    if (!AtualizarLinha(r))
                        return;
                
                Glass.MensagemAlerta.ShowMsg("Descontos/acréscimos atribuídos.", Page);
    
                grdDesconto.DataBind();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao gerar desconto/acréscimo para cliente.", ex, Page);
            }
        }
    
        protected void imbProdutosGrupo_Click(object sender, ImageClickEventArgs e)
        {
            var parent = ((ImageButton)sender).Parent as TableCell;
            var idGrupo = ((HiddenField)parent.FindControl("hdfIdGrupo")).Value;
            var idSubgrupo = ((HiddenField)parent.FindControl("hdfIdSubgrupo")).Value;
    
            var filtro = Request["idCliente"] != null ? "idCliente=" + Request["idCliente"] :
                "idTabelaDesconto=" + Request["idTabelaDesconto"];
    
            Response.Redirect("~/Cadastros/CadDescontoAcrescimoCliente.aspx?" + filtro + "&idGrupo=" + idGrupo + "&idSubgrupo=" + idSubgrupo + 
                ((Page.Master as Layout).IsPopupControle() ? "&popup=true&controlePopup=true" : 
                (Page.Master as Layout).IsPopup() ? "&popup=true" : String.Empty));
        }
    
        protected void lnkVoltar_Load(object sender, EventArgs e)
        {
            var filtro = Request["idCliente"] != null ? "idCliente=" + Request["idCliente"] :
                "idTabelaDesconto=" + Request["idTabelaDesconto"];
    
            lnkVoltar.NavigateUrl = "~/Cadastros/CadDescontoAcrescimoCliente.aspx?" + filtro;
        }
    
        protected void grdDesconto_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Atualizar")
            {
                ImageButton imbAtualizar = e.CommandSource as ImageButton;
                if (AtualizarLinha(imbAtualizar.Parent.Parent as GridViewRow))
                    Glass.MensagemAlerta.ShowMsg("Desconto/acréscimo atribuído para o produto.", Page);
            }
        }
    
        private bool AtualizarLinha(GridViewRow r)
        {
            grdDesconto.UpdateRow(r.RowIndex, false);
            return true;
        }

        protected void odsDesconto_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            var resultado = (Colosoft.Business.SaveResult)e.ReturnValue;

            if (!resultado)
                MensagemAlerta.ShowMsg(resultado.Message.ToString(), Page);
        }

        protected void imgCopiar_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                uint idCliente = Glass.Conversoes.StrParaUint(Request["idCliente"]);
                uint idClienteNovo = Glass.Conversoes.StrParaUint(txtNumCli.Text);

                if (idCliente == 0 || idClienteNovo == 0)
                    throw new Exception("O cliente atual e o cliente novo devem ser informados.");
    
                DescontoAcrescimoClienteDAO.Instance.Copiar(idCliente, idClienteNovo);
                Glass.MensagemAlerta.ShowMsg("Desconto/acréscimo copiado com sucesso!", Page);
                txtNumCli.Text = "";
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao copiar o desconto/acréscimo entre clientes.", ex, Page);
            }
        }

        protected void drpSituacao_DataBound(object sender, EventArgs e)
        {
            if (!IsPostBack)
                // Define como filtro padrão pedidos de Venda/Revenda e Mão de Obra
                foreach (ListItem li in ((DropDownList)sender).Items)
                {
                    switch (li.Value)
                    {
                        case "Ativo":
                            li.Selected = true;
                            break;

                        default:
                            li.Selected = false;
                            break;
                    }
                }
        }
    }
}

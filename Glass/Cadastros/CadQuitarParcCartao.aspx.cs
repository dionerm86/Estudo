using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Data.Model;
using Glass.Configuracoes;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadQuitarParcCartao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                chkAgrupar_CheckedChanged(null, EventArgs.Empty);
    
                // Esconde a coluna da conta bancária se for permitido selecioná-la
                if (FinanceiroConfig.FinanceiroRec.SelecionarContaBancoQuitarParcCartao)
                    grdConta.Columns[3].Visible = false;
            }
    
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdConta.PageIndex = 0;
            grdConta.DataBind();
        }
    
        protected void grdConta_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Quitar")
            {
                try
                {
                    GridViewRow linha = (GridViewRow)((Button)e.CommandSource).Parent.Parent;
                    string data = ((TextBox)linha.FindControl("txtDataQuitar")).Text;
                    uint idContaBanco = Glass.Conversoes.StrParaUint(((DropDownList)linha.FindControl("drpContaBanco")).SelectedValue);
                    var isCaixaDiario = Request["cxDiario"] == "1";

                    if (ContaBancoDAO.Instance.ObtemSituacao(idContaBanco) == (int)Glass.Situacao.Inativo)
                    {
                        Glass.MensagemAlerta.ShowMsg("Esta conta bancária está inativa.", this);
                        return;
                    }

                    string mensagem;
                    if (!chkAgrupar.Checked)
                    {
                        ContasReceberDAO.Instance.QuitarParcCartao(Glass.Conversoes.StrParaUint(e.CommandArgument.ToString()), idContaBanco, data, isCaixaDiario);
                        mensagem = "Parcela quitada com sucesso!";
                    }
                    else
                    {
                        ContasReceberDAO.Instance.QuitarVariasParcCartao(e.CommandArgument.ToString(), idContaBanco, data, isCaixaDiario);
                        mensagem = "Parcelas quitadas com sucesso!";
                    }
    
                    grdConta.DataBind();
                    Glass.MensagemAlerta.ShowMsg(mensagem, this);
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ShowMsg("Erro ao quitar parcela. " + ex.Message, this);
                }
            }
        }
    
        protected void txtDataQuitar_Load(object sender, EventArgs e)
        {
            if (!IsPostBack || ((TextBox)sender).Text == "")
                ((TextBox)sender).Text = DateTime.Now.ToString("dd/MM/yyyy");
        }
    
        protected void imgDataQuitar_Load(object sender, EventArgs e)
        {
            ((ImageButton)sender).OnClientClick = "return SelecionaData('" + ((ImageButton)sender).ClientID.Replace("img", "txt") + "', this)";
        }
    
        protected void chkAgrupar_CheckedChanged(object sender, EventArgs e)
        {
            pedidoTitulo.Visible = !chkAgrupar.Checked;
            pedido.Visible = !chkAgrupar.Checked;
            acertoTitulo.Visible = !chkAgrupar.Checked;
            acerto.Visible = !chkAgrupar.Checked;
            liberarPedidoTitulo.Visible = !chkAgrupar.Checked && PedidoConfig.LiberarPedido;
            liberarPedido.Visible = !chkAgrupar.Checked && PedidoConfig.LiberarPedido;
            vencDataFim.Visible = !chkAgrupar.Checked;
            clienteTitulo.Visible = !chkAgrupar.Checked;
            cliente.Visible = !chkAgrupar.Checked;
            tipoEntregaTitulo.Visible = !chkAgrupar.Checked;
            tipoEntrega.Visible = !chkAgrupar.Checked;
    
            for (int i = 2; i <= 6; i++)
                grdConta.Columns[i].Visible = !chkAgrupar.Checked;
    
            for (int i = 7; i <= 9; i++)
                grdConta.Columns[i].Visible = chkAgrupar.Checked;
        }
    
        protected void btnQuitar_DataBinding(object sender, EventArgs e)
        {
            Button btnQuitar = (Button)sender;
            GridViewRow linha = btnQuitar.Parent.Parent as GridViewRow;
            ContasReceber item = linha.DataItem as ContasReceber;
    
            btnQuitar.CommandArgument = DataBinder.Eval(item, !chkAgrupar.Checked ? "IdContaR" : "IdsContas").ToString();
        }
    
        protected void drpContaBanco_Load(object sender, EventArgs e)
        {
            if (!FinanceiroConfig.FinanceiroRec.SelecionarContaBancoQuitarParcCartao)
                ((DropDownList)sender).Visible = false;
        }
     
        protected void drpTipoCartao_TextChanged(object sender, EventArgs e)
        {
            imgPesq_Click(null, null);
        }
    }
}

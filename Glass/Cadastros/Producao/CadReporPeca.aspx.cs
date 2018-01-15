using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Data.Helper;

namespace Glass.UI.Web.Cadastros.Producao
{
    public partial class CadReporPeca : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtNumPedido.Focus();
                drpSetor.DataBind();
                chkPerdaDefinitiva.Style.Add("display", "none");
    
                if (String.IsNullOrEmpty(ctrlDataPerda.DataString))
                    ctrlDataPerda.Data = DateTime.Now;
            }
        }
    
        protected void imbPesq_Click(object sender, ImageClickEventArgs e)
        {
            uint idPedido = Glass.Conversoes.StrParaUint(txtNumPedido.Text);
    
            if (idPedido > 0 && !PedidoDAO.Instance.PedidoExists(idPedido))
                Glass.MensagemAlerta.ShowMsg("N�o existe nenhum pedido com o n�mero passado.", Page);
        }
    
        protected void grdProdutos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "Marcar")
                {
                    if (drpSetor.SelectedValue == String.Empty)
                    {
                        Glass.MensagemAlerta.ShowMsg("Informe o setor que houve a perda.", Page);
                        return;
                    }
    
                    if (ctrlTipoPerda1.IdTipoPerda == null)
                    {
                        Glass.MensagemAlerta.ShowMsg("Informe o tipo da perda.", Page);
                        return;
                    }
    
                    if (drpFuncionario.SelectedValue == String.Empty)
                    {
                        Glass.MensagemAlerta.ShowMsg("Informe o funcion�rio da perda.", Page);
                        return;
                    }
    
                    if (String.IsNullOrEmpty(ctrlDataPerda.DataString))
                    {
                        Glass.MensagemAlerta.ShowMsg("Informe a data da perda.", Page);
                        return;
                    }
    
                    uint idFuncPerda = Glass.Conversoes.StrParaUint(drpFuncionario.SelectedValue);
    
                    if (FuncionarioDAO.Instance.IsAdminSync(idFuncPerda) && !UserInfo.GetUserInfo.IsAdminSync)
                    {
                        Glass.MensagemAlerta.ShowMsg("Voc� n�o pode marcar este funcion�rio como respons�vel pela perda", Page);
                        return;
                    }
    
                    List<RetalhoProducaoAuxiliar> dadosRetalho = ctrlRetalhoProducao1.Dados;
    
                    if (!RetalhoProducaoDAO.Instance.ValidaRetalhos(dadosRetalho, e.CommandArgument.ToString().Split(';')[0]))
                    {
                        Glass.MensagemAlerta.ShowMsg("Aten��o: As dimens�es do retalho n�o podem ser maiores que as da pe�a.", Page);
                        return;
                    }

                    if (!Config.PossuiPermissao(Config.FuncaoMenuPedido.ReposicaoDePeca))
                    {
                        Glass.MensagemAlerta.ShowMsg("Voc� n�o tem permiss�o para repor pe�as.", Page);
                        return;
                    }
    
                    ProdutoPedidoProducaoDAO.Instance.MarcarPecaReposta(null, e.CommandArgument.ToString().Split(';')[0], Glass.Conversoes.StrParaUint(drpSetor.SelectedValue), 
                        idFuncPerda, ctrlDataPerda.Data, ctrlTipoPerda1.IdTipoPerda.Value, ctrlTipoPerda1.IdSubtipoPerda, txtObs.Text, 
                        chkPerdaDefinitiva.Checked);
                    
                    if (dadosRetalho.Count > 0)
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Imprimir", string.Format("imprimirRetalhos('{0}');", e.CommandArgument.ToString().Split(';')[0]), true);
    
                    /*if (dadosRetalho.Count > 0)
                    {
                        foreach (RetalhoProducaoDAO.RetalhoProducaoAux r in dadosRetalho)
                            RetalhoProducaoDAO.Instance.CriarRetalho(r.Altura.ToString(), r.Largura.ToString(), r.Quantidade.ToString(), e.CommandArgument.ToString());
                    }*/
    
                    grdProdutos.PageIndex = 0;
                    grdProdutos.DataBind();
    
                    Glass.MensagemAlerta.ShowMsg("Pe�a reposta.", Page);
                }
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao marcar reposi��o de pe�a.", ex, Page);
            }
        }
    
        protected void drpFuncionario_DataBound(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(((DropDownList)sender).SelectedValue))
                ((DropDownList)sender).SelectedValue = UserInfo.GetUserInfo.CodUser.ToString();
        }
    
        protected void drpSetor_SelectedIndexChanged(object sender, EventArgs e)
        {
            grdProdutos.DataBind();
        }
    }
}

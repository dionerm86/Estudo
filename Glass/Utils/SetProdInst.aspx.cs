using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Data.Model;

namespace Glass.UI.Web.Utils
{
    public partial class SetProdInst : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            LoginUsuario login = UserInfo.GetUserInfo;
    
            if (!Config.PossuiPermissao(Data.Helper.Config.FuncaoMenuInstalacao.ControleInstalacaoComum) &&
                !Config.PossuiPermissao(Data.Helper.Config.FuncaoMenuInstalacao.ControleInstalacaoTemperado))
            {
                Glass.MensagemAlerta.ShowMsg("Apenas Supervisores de Colocação podem marcar produtos instalados.", Page);
                return;
            }
    
            if (grdProduto.Rows.Count > 0)
            {
                uint idInstalacao = Glass.Conversoes.StrParaUint(Request["idInstalacao"]);
                uint idPedido = Glass.Conversoes.StrParaUint(Request["idPedido"]);
                List<ProdutosInstalacao> lstProdInst = new List<ProdutosInstalacao>();
                ProdutosInstalacao prodInst;
    
                bool finalizar = true;
    
                try
                {
                    // Para cada linha da grid
                    foreach (GridViewRow g in grdProduto.Rows)
                    {
                        // Qtd instalada agora
                        string qtdInstString = ((TextBox)g.FindControl("txtQtdInst")).Text;
                        int qtdInst = !String.IsNullOrEmpty(qtdInstString) ? Glass.Conversoes.StrParaInt(qtdInstString) : 0;
    
                        // Qtd instalada anteriormente
                        int qtdJaInstalada = Glass.Conversoes.StrParaInt(((HiddenField)g.FindControl("hdfQtdeInst")).Value);
    
                        // Qtd total
                        int qtdTotal = Glass.Conversoes.StrParaInt(((HiddenField)g.FindControl("hdfQtdeTotal")).Value);
    
                        // Id do produto do pedido que está sendo marcado como instalado
                        uint idProdPed = Glass.Conversoes.StrParaUint(((HiddenField)g.FindControl("hdfIdProdPed")).Value);
    
                        string descrProd = ((HiddenField)g.FindControl("hdfDescrProduto")).Value;
                        descrProd = descrProd == null ? String.Empty : descrProd;
    
                        // Verifica se qtd instalada é maior que a qtd restante a ser instalada
                        if (qtdInst > qtdTotal - qtdJaInstalada)
                            throw new Exception("A quantidade marcada como instalada do item " +
                                descrProd + " ultrapassa a quantidade restante.");
                        else
                            finalizar = finalizar && (qtdInst == qtdTotal - qtdJaInstalada);
    
                        // Adiciona produto à uma lista temporária para ser inserido depois
                        prodInst = new ProdutosInstalacao();
                        prodInst.IdInstalacao = idInstalacao;
                        prodInst.IdPedido = idPedido;
                        prodInst.IdProdPed = idProdPed;
                        prodInst.QtdeInstalada = qtdInst;
                        lstProdInst.Add(prodInst);
                    }
                    
                    InstalacaoDAO.Instance.ContinuarFinalizar((int)idInstalacao, lstProdInst, txtObs.Text);
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao marcar produtos instalados.", ex, Page);
                    return;
                }
    
                ClientScript.RegisterClientScriptBlock(typeof(string), "ok", "alert('Operação concuída. Instalação " + (finalizar ? "finalizada" : "continuada") + 
                    ".');window.opener.location.href='../Cadastros/CadFinalizarInstalacao.aspx?rand=" + DateTime.Now.Millisecond + "'; closeWindow();", true);
            }
        }
    }
}

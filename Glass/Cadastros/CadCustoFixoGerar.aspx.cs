using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Data.Helper;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadCustoFixoGerar : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            hdfData.Value = txtData.Text;
        }
    
        protected void drpLoja_SelectedIndexChanged(object sender, EventArgs e)
        {
            hdfData.Value = txtData.Text;
        }
    
        protected void btnGerar_Click(object sender, EventArgs e)
        {
            LoginUsuario login = UserInfo.GetUserInfo;
    
            if (!Config.PossuiPermissao(Config.FuncaoMenuFinanceiroPagto.ControleFinanceiroPagamento))
            {
                Glass.MensagemAlerta.ShowMsg("Apenas funcionários Financeiro Pagto/Geral podem gerar custos fixos.", Page);
                return;
            }
    
            if (String.IsNullOrEmpty(hdfData.Value))
            {
                imgPesq_Click(null, new ImageClickEventArgs(0, 0));
                return;
            }
    
            if (grdCustoFixo.Rows.Count > 0)
            {
                string listPKS = String.Empty; // Guarda os ids de todos os custos fixo que foram gerados;
    
                try
                {
                    string custosFixosGerados = String.Empty;
                    string custosFixosInvalidos = String.Empty;
    
                    // Para cada linha da grid
                    foreach (GridViewRow g in grdCustoFixo.Rows)
                    {
                        // Se o custo fixo estiver marcado para ser gerado
                        // Para cada custo fixo selecionado, cria uma conta a pagar com os dados do mesmo
                        // e atualiza a data gerado para hoje
                        if (((CheckBox)g.FindControl("chkSel")).Checked)
                        {
                            uint idCustoFixo = Glass.Conversoes.StrParaUint(((HiddenField)g.FindControl("hdfIdCustoFixo")).Value);
                            decimal valor = Glass.Conversoes.StrParaDecimal(((TextBox)g.FindControl("txtValor")).Text);
                            int diaVenc = Glass.Conversoes.StrParaInt(((TextBox)g.FindControl("txtDiaVenc")).Text);
    
                            CustoFixoDAO.RetornoGerar retorno = CustoFixoDAO.Instance.Gerar(idCustoFixo, hdfData.Value, diaVenc, valor, ((TextBox)g.FindControl("txtObs")).Text);
                            if (retorno.Tipo == CustoFixoDAO.RetornoGerar.TipoRetorno.JaExiste)
                                custosFixosGerados += retorno.Descricao + ", ";
                            else if (retorno.Tipo == CustoFixoDAO.RetornoGerar.TipoRetorno.DataInvalida)
                                custosFixosInvalidos += retorno.Descricao + ", ";
                        }
                    }
    
                    string script = String.Empty;
    
                    if (!String.IsNullOrEmpty(custosFixosInvalidos) || !String.IsNullOrEmpty(custosFixosGerados))
                    {
                        string gerados = String.IsNullOrEmpty(custosFixosGerados) ? String.Empty :
                            "Os custos fixos " + custosFixosGerados.Trim().TrimEnd(',') + @" já tinham sido gerados. ";
    
                        string invalidos = String.IsNullOrEmpty(custosFixosInvalidos) ? String.Empty :
                            "Os custos fixos " + custosFixosInvalidos.Trim().TrimEnd(',') + @" não puderam ser gerados pois o dia de vencimento é inválido. ";
    
                        script += "alert('Custos fixos gerados com sucesso. " + (gerados + invalidos).Trim() + "');";
                    }
                    else
                        script += "alert('Custos fixos gerados com sucesso.');";
    
                    script += "redirectUrl('CadCustoFixoGerar.aspx?rand=" + DateTime.Now.Millisecond + "');";
    
                    ClientScript.RegisterClientScriptBlock(typeof(string), "ok", script, true);
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao gerar custos fixos.", ex, Page);
                    return;
                }
            }
        }
    }
}

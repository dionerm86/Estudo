using System;
using System.Web.UI;
using Glass.Data.DAL;

namespace Glass.UI.Web.Utils
{
    public partial class LancarEstoqueMinimo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblLoja.Text = LojaDAO.Instance.GetNome(Glass.Conversoes.StrParaUint(Request["idLoja"]));
            lblGrupo.Text = GrupoProdDAO.Instance.GetDescricao(Glass.Conversoes.StrParaInt(Request["idGrupoProd"]));
            lblSubgrupo.Text = String.IsNullOrEmpty(Request["idSubgrupoProd"]) ? "(Todos)" :
                SubgrupoProdDAO.Instance.GetDescricao(Glass.Conversoes.StrParaInt(Request["idSubgrupoProd"]));
        }
    
        protected void btnAtualizar_Click(object sender, EventArgs e)
        {
            try
            {
                float qtdeMin = float.Parse(txtEstoqueMinimo.Text);
                uint idLoja = Glass.Conversoes.StrParaUint(Request["idLoja"]);
                uint idGrupo = Glass.Conversoes.StrParaUint(Request["idGrupoProd"]);
                uint? idSubgrupo = Glass.Conversoes.StrParaUintNullable(Request["idSubgrupoProd"]);
    
                ProdutoLojaDAO.Instance.AtualizaEstoqueMinimo(qtdeMin, idLoja, idGrupo, idSubgrupo);
    
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "updateOpener", @"
                    window.opener.FindControl('txtDescr', 'input').value = 
                        window.opener.FindControl('txtDescr', 'input').value == '' ? ' ' : ''; 
                    window.opener.atualizarPagina(); 
                    alert('Estoque atualizado com sucesso!');
                    closeWindow();", true);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar o estoque mínimo.", ex, Page);
            }
        }
    }
}

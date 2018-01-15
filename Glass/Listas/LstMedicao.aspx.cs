using System;
using System.Web.UI;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.UI.Web.Listas
{
    public partial class LstMedicao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Listas.LstMedicao));
            
            // Esconde as opções de inserir, editar, cancelar, visualizar fotos e finalizar da grid, se for relatório
            lnkInserir.Visible = String.IsNullOrEmpty(Request["rel"]);
            lnkImprimir.Visible = Config.PossuiPermissao(Config.FuncaoMenuMedicao.EfetuarMedicao) || !lnkInserir.Visible;
            grdMedicao.Columns[0].Visible = lnkInserir.Visible;
            grdMedicao.Columns[12].Visible = lnkInserir.Visible;
    
            if (Request["idMedicao"] != null)
                txtNumMedicao.Text = Request["idMedicao"];
    
            grdMedicao.DataBind();
        }
    
        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Cadastros/CadMedicao.aspx");
        }
    
        protected void odsMedicao_Deleted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg(null, e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }
    
        [Ajax.AjaxMethod()]
        public string Finalizar(string idMedicao)
        {
            try
            {
                // Finaliza a medição
                uint idOrca = MedicaoDAO.Instance.FinalizarMedicao(Glass.Conversoes.StrParaUint(idMedicao));
    
                return "ok\t" + idOrca.ToString();
            }
            catch (Exception ex)
            {
                return "Erro\t" + Glass.MensagemAlerta.FormatErrorMsg("Falha ao finalizar Medição.", ex);
            }
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdMedicao.PageIndex = 0;
        }

        protected void lnkPesquisar_Click(object sender, EventArgs e)
        {
            grdMedicao.PageIndex = 0;
        }
    }
}

using System;
using System.Web.UI;
using Glass.Data.DAL;

namespace Glass.UI.Web.Listas
{
    public partial class LstCustoFixo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Se o usuário estiver entrando nesta página para gerar relatório apenas, 
            // esconde controle de editar, excluir e inserir
            if (!IsPostBack && Request["rel"] != null)
            {
                grdCustoFixo.Columns[0].Visible = false;
                lnkInserir.Visible = false;
            }
    
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Listas.LstCustoFixo));
        }
    
        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Cadastros/CadCustoFixo.aspx");
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdCustoFixo.PageIndex = 0;
        }
    
        protected void drpSituacao_SelectedIndexChanged(object sender, EventArgs e)
        {
            grdCustoFixo.PageIndex = 0;
        }
    
        [Ajax.AjaxMethod()]
        public static string SetPontoEquilibrio(string idCustoFixo, string valor)
        {
            string retorno = "";
    
            try
            {
                retorno = CustoFixoDAO.Instance.SetPontoEquilibrio(Glass.Conversoes.StrParaUint(idCustoFixo), bool.Parse(valor)).ToString();
            }
            catch (Exception ex)
            {
                return "Ocorreu um erro: " + ex.Message;
            }
    
            return retorno;
        }
    }
}

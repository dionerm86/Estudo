using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;

namespace Glass.UI.Web.Listas
{
    public partial class LstAgendamentoInstalacao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Listas.LstAgendamentoInstalacao));
    
            if (!IsPostBack)
            {
                ctrlDataInicial.Data = DateTime.Now;
                ctrlDataFinal.Data = DateTime.Now.AddMonths(1).AddDays(-DateTime.Now.Day);
            }
    
            tdCliente.Visible = ckbCliente.Checked;
            tdEquipe.Visible = ckbEquipe.Checked;
            tdGridCliente.Visible = ckbCliente.Checked;
            tdGridEquipe.Visible = ckbEquipe.Checked;
    
            grdAgendamento.DataBind();
            grdCliente.DataBind();
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdAgendamento.PageIndex = 0;
            grdCliente.PageIndex = 0;
        }
    
        protected void dataValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            bool valid = false;
    
            DateTime ini = ctrlDataInicial.Data;
            DateTime fim = ctrlDataFinal.Data;
    
            TimeSpan time = fim.Subtract(ini);
    
            if (time.Days > 30)
                valid = false;
            else
                valid = true;
    
            if (valid)
            {
                args.IsValid = true;
            }
            else
            {
                args.IsValid = false;
            }
    
        }
    
        #region Métodos AJAX
    
        /// <summary>
        /// Busca o cliente em tempo real
        /// </summary>
        /// <param name="idCli"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string GetCli(string idCli)
        {
            if (!ClienteDAO.Instance.Exists(Glass.Conversoes.StrParaUint(idCli)))
                return "Erro;Cliente não encontrado.";
            else
                return "Ok;" + ClienteDAO.Instance.GetNome(Glass.Conversoes.StrParaUint(idCli));
        }
    
        #endregion
    
    }
}

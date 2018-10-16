using System;
using Glass.Data.RelModel;
using Glass.Data.RelDAL;
using Glass.Configuracoes;

namespace Glass.UI.Web.Listas
{
    public partial class LstInfoPedidos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            if (!IsPostBack)
                txtData.Text = String.IsNullOrEmpty(Request["data"]) ? DateTime.Now.ToString("dd/MM/yyyy") : Request["data"];
    
            voltar.Visible = Request["voltar"] == "1";
    
            InfoPedidos info = InfoPedidosDAO.Instance.GetInfoPedidos(txtData.Text);
            lblDataConsulta.Text = txtData.Text;
            lblM2FastDelivery.Text = info.TotalFastDelivery.ToString();
        }
    }
}

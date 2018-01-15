using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Relatorios.NFe
{
    public partial class LivroRegistro : System.Web.UI.Page
    {
        string tipoLivro;
    
        protected void Page_Load(object sender, EventArgs e)
        {
            PopulaAno();
            PopulaMes();
            tipoLivro = Request["tipo"];
    
            hdfTipo.Value = tipoLivro.ToString();
    
            switch (Glass.Conversoes.StrParaInt(tipoLivro))
            {
                case 1: Page.Title += " Entrada"; break;
                case 2: Page.Title += " Saída"; break;
                case 3: Page.Title += " Apuração de ICMS"; break;
                case 4: Page.Title += " Apuração de IPI"; break;
            }
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
        }
    
        private void PopulaMes()
        {
            ddlMes.Items.Add(new ListItem("Janeiro", "1"));
            ddlMes.Items.Add(new ListItem("Fevereiro", "2"));
            ddlMes.Items.Add(new ListItem("Março", "3"));
            ddlMes.Items.Add(new ListItem("Abril", "4"));
            ddlMes.Items.Add(new ListItem("Maio", "5"));
            ddlMes.Items.Add(new ListItem("Junho", "6"));
            ddlMes.Items.Add(new ListItem("Julho", "7"));
            ddlMes.Items.Add(new ListItem("Agosto", "8"));
            ddlMes.Items.Add(new ListItem("Setembro", "9"));
            ddlMes.Items.Add(new ListItem("Outubro", "10"));
            ddlMes.Items.Add(new ListItem("Novembro", "11"));
            ddlMes.Items.Add(new ListItem("Dezembro", "12"));
    
            ddlMes.SelectedValue = DateTime.Now.Month.ToString();
        }
    
        private void PopulaAno()
        {
            int inicio = DateTime.Now.Year - 10;
            for (int i = 0; i < 20; i++)
            {
                ddlAno.Items.Add(new ListItem((inicio + i).ToString(), (inicio + i).ToString()));
            }
    
            ddlAno.SelectedValue = DateTime.Now.Year.ToString();
        }
    }
}

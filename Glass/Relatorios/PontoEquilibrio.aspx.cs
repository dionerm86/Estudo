using System;
using System.Web.UI;
using Glass.Data.RelDAL;
using Glass.Data.Helper;

namespace Glass.UI.Web.Relatorios
{
    public partial class PontoEquilibrio : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtDataIni.Data = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                txtDataFim.Data = DateTime.Now;
            }
    
            CarregaPontoEquilibrio();
        }
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            CarregaPontoEquilibrio();
        }
    
        private void CarregaPontoEquilibrio()
        {
           var lista = PontoEquilibrioDAO.Instance.GetPontoEquilibrio(txtDataIni.DataString, txtDataFim.DataString, UserInfo.GetUserInfo);
    
            string html = "<table class='gridStyle' cellpadding='0' cellspacing='0' id='tabelaPE' style='border-collapse: collapse'>";
            html += "<tbody><tr><th align='left'></th><th align='left'>Item</th><th align='right'>Valor</th><th align='right'>%</th></tr></tbody>";
    
            int index = 0;
            foreach (var item in lista)
            {
                if ((index % 2) == 0)
                    html += "<tr style='display: table-row;border-color: inherit;'>";
                else
                    html += "<tr class='alt' style='display: table-row;border-color: inherit;'>";
    
                html += "<td>" + item.Indice + "</td>" + "<td>" + item.Item + "</td>" + "<td align='right'>" + item.ValorString + "</td>" + "<td align='right'>" + item.Percentual + "</td></tr>";
    
                if (item.subItens != null)
                {
                    if (item.subItens.Count > 0)
                    {
                        int indice = 1;
                        foreach (var subItem in item.subItens)
                        {
                            html += "<tr style='display: table-row;border-color: inherit;'>";
                            html += "<td style='font-size:8px;padding-left:10px;'>" + (item.Indice + "." + indice) + "</td>" + "<td style='font-size:8px;padding-left:10px;'>" + subItem.Item + "</td>" + "<td style='font-size:8px;padding-left:5px;' align='right'>" + subItem.ValorString + "</td>" + "<td style='font-size:8px;padding-left:5px;' align='right'>" + subItem.Percentual + "</td></tr>";
                            indice++;
                        }
                    }
                }
    
                index++;
            }
    
            html += "</table>";
    
            pontoEquilibrio.InnerHtml = html;
        }
    }
}

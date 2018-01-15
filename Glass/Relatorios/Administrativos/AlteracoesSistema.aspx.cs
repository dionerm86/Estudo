using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Relatorios.Administrativos
{
    public partial class AlteracoesSistema : System.Web.UI.Page
    {
        private Dictionary<string, object> _valores = new Dictionary<string, object>();
    
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
    
        }
    
        protected void ItemFiltro_DataBinding(object sender, EventArgs e)
        {
            string chave = (sender as Control).ID;
            if (!_valores.ContainsKey(chave))
                _valores.Add(chave, null);
    
            if (sender is DropDownList)
                _valores[chave] = (sender as DropDownList).SelectedValue;
            else if (sender is Glass.UI.Web.Controls.ctrlSelPopup)
                _valores[chave] = (sender as Glass.UI.Web.Controls.ctrlSelPopup).Valor;
        }
    
        protected void ItemFiltro_DataBound(object sender, EventArgs e)
        {
            string chave = (sender as Control).ID;
            if (!_valores.ContainsKey(chave))
                return;
    
            if (sender is DropDownList)
                (sender as DropDownList).SelectedValue = _valores[chave].ToString();
            else if (sender is Glass.UI.Web.Controls.ctrlSelPopup)
                (sender as Glass.UI.Web.Controls.ctrlSelPopup).Valor = _valores[chave].ToString();
        }
    }
}

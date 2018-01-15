using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Model;
using Glass.Configuracoes;

namespace Glass.UI.Web.Relatorios
{
    public partial class ListaProdutos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
    
            // Carrega o DropDownList de colunas
            if (!IsPostBack)
            {
                if (ProdutoConfig.TelaListagem.UsarRelatorioProdutosDiferente)
                {
                    cbdColunas.Items.Add(new ListItem("Custo Forn.", "1"));
                    cbdColunas.Items.Add(new ListItem("Custo Imp.", "2"));
                    cbdColunas.Items.Add(new ListItem("Balcão", "3"));
                    cbdColunas.Items.Add(new ListItem("Obra", "4"));
                    cbdColunas.Items.Add(new ListItem(new Produto().DescrAtacadoRepos, "5"));
                    cbdColunas.Items.Add(new ListItem("Disp. Estoque", "6"));
                    cbdColunas.Items.Add(new ListItem("Reserva", "7"));
                    cbdColunas.Items.Add(new ListItem("Estoque", "8"));
                }
                else
                {
                    cbdColunas.Items.Add(new ListItem("Custo Forn.", "1"));
                    cbdColunas.Items.Add(new ListItem("Custo Imp.", "2"));
                    cbdColunas.Items.Add(new ListItem("Balcão", "3"));
                    cbdColunas.Items.Add(new ListItem("Obra", "4"));
                    cbdColunas.Items.Add(new ListItem(new Produto().DescrAtacadoRepos, "5"));
                    cbdColunas.Items.Add(new ListItem("Disp. Estoque", "6"));
                }
            }
        }
    
        protected void lnkPesq_Click(object sender, EventArgs e)
        {
            grdProdutos.PageIndex = 0;
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdProdutos.PageIndex = 0;
        }
    }
}

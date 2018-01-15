using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros.Projeto
{
    public partial class LstFabricanteFerragem : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            grdFabricanteFerragem.Register(true, true);
            odsFabricanteFerragem.Register();
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdFabricanteFerragem.PageIndex = 0;
        }

        protected void imbInserir_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                var fabricanteFerragem = new Glass.Projeto.Negocios.Entidades.FabricanteFerragem();
                fabricanteFerragem.Nome = ((TextBox)grdFabricanteFerragem.FooterRow.FindControl("txtNome")).Text;
                fabricanteFerragem.Sitio = ((TextBox)grdFabricanteFerragem.FooterRow.FindControl("txtSitio")).Text;

                var ferragemFluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<Glass.Projeto.Negocios.IFerragemFluxo>();

                var resultado = ferragemFluxo.SalvarFabricanteFerragem(fabricanteFerragem);

                if (!resultado)
                {
                    Glass.MensagemAlerta.ErrorMsg("Não foi possível inserir o Fabricante de Ferragem.", resultado);
                    return;
                }

                grdFabricanteFerragem.DataBind();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir Fabricante de Ferragem.", ex, Page);
            }
        }
    }
}
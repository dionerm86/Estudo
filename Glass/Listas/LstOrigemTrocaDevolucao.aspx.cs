using System;
using System.Web.UI.WebControls;
using Microsoft.Practices.ServiceLocation;

namespace Glass.UI.Web.Listas
{
    public partial class LstOrigemTrocaDevolucao : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            grdOrigem.Register(true, true);
            odsOrigem.Register();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }
    
        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            try
            {
                var fluxo = ServiceLocator.Current.GetInstance<Glass.Global.Negocios.IOrigemTrocaDescontoFluxo>();

                var resultado = fluxo.SalvarOrigemTrocaDesconto(
                    new Glass.Global.Negocios.Entidades.OrigemTrocaDesconto
                    {
                        Descricao = ((TextBox)grdOrigem.FooterRow.FindControl("txtDescricaoIns")).Text,
                        Situacao = Situacao.Ativo
                    });

                if (!resultado)
                    Glass.MensagemAlerta.ErrorMsg("Falha ao inserir Origem Troca/Desconto.", resultado);
                else
                    grdOrigem.DataBind();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir Origem Troca/Desconto.", ex, Page);
            }
        }
    }
}

using System;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadTipoFuncionario : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            odsTipoFunc.Register();
            grdTipoFunc.Register(true, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            var tipoFunc = new Glass.Global.Negocios.Entidades.TipoFuncionario();
            tipoFunc.Descricao = ((TextBox)grdTipoFunc.FooterRow.FindControl("txtDescricao")).Text;

            var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator.Current
                .GetInstance<Glass.Global.Negocios.IFuncionarioFluxo>();

            // Salva os dados do tipo de funcionario
            var resultado = fluxo.SalvarTipoFuncionario(tipoFunc);

            if (!resultado)
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir tipo de funcionário.", resultado);
            else
                grdTipoFunc.DataBind();
        }
    }
}

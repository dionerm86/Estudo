using Glass.Global.Negocios.Entidades;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Listas
{
    public partial class LstDepartamento : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            grdDepartamento.Register(true, true);
            odsDepartamento.Register();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdDepartamento.PageIndex = 0;
        }
    
        protected void imgAdd_Click(object sender, ImageClickEventArgs e)
        {
            string nome = ((TextBox)grdDepartamento.FooterRow.FindControl("txtNome")).Text;
            string descricao = ((TextBox)grdDepartamento.FooterRow.FindControl("txtDescricao")).Text;
    
            var novo = new Departamento();
            novo.Nome = nome;
            novo.Descricao = descricao;

            var funcionarioFluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<Glass.Global.Negocios.IFuncionarioFluxo>();

            var resultado = funcionarioFluxo.SalvarDepartamento(novo);

            if (!resultado)
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir o departamento.", resultado);
            else
                grdDepartamento.DataBind();
        }
    }
}

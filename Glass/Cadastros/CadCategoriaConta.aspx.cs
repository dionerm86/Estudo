using System;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadCategoriaConta : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            odsCategoriaConta.Register();
            grdCategoriaConta.Register(true, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            var descricao = ((TextBox)grdCategoriaConta.FooterRow.FindControl("txtDescricao")).Text;

            var tipo1 = Glass.Data.Model.TipoCategoriaConta.Subtotal;
            Glass.Data.Model.TipoCategoriaConta? tipo = null;

            if (Enum.TryParse(((DropDownList)grdCategoriaConta.FooterRow.FindControl("drpTipo")).SelectedValue, out tipo1))
                tipo = tipo1;

            var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<Glass.Financeiro.Negocios.IPlanoContasFluxo>();

            var categoria = new Glass.Financeiro.Negocios.Entidades.CategoriaConta
            {
                Descricao = descricao,
                Tipo = tipo
            };

            Glass.Situacao situacao = Situacao.Ativo;

            if (Enum.TryParse<Glass.Situacao>(((DropDownList)grdCategoriaConta.FooterRow.FindControl("drpSituacao")).SelectedValue, out situacao))
                categoria.Situacao = situacao;

            var resultado = fluxo.SalvarCategoriaConta(categoria);

            if (resultado)
            {
                grdCategoriaConta.DataBind();
                Glass.MensagemAlerta.ShowMsg("Categoria inserida.", Page);
            }
            else
                // Mostra o erro ocorrido
                this.MostrarErro(resultado, "Falha ao inserir Categoria.");
        }
    
        protected void grdCategoriaConta_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Up" || e.CommandName == "Down")
            {
                var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<Glass.Financeiro.Negocios.IPlanoContasFluxo>();

                var idCategoriaConta = Convert.ToInt32(e.CommandArgument);

                var resultado = fluxo.AlterarPosicaoCategoriaConta(idCategoriaConta, e.CommandName == "Up");

                if (resultado)
                    grdCategoriaConta.DataBind();
                else
                    this.MostrarErro(resultado, "Falha ao mudar posição da categoria.");
            }
        }
    }
}

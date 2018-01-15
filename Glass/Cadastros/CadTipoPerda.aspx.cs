using Glass.PCP.Negocios.Entidades;
using System;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadTipoPerda : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            grdTipoPerda.Register(true, true);
            odsTipoPerda.Register();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }
    
        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            try
            {
                var tipoPerda = new TipoPerda();
                tipoPerda.Descricao = ((TextBox)grdTipoPerda.FooterRow.FindControl("txtDescricao")).Text;
                tipoPerda.Situacao = Data.Model.SituacaoTipoPerda.Ativo;
                tipoPerda.ExibirPainelProducao = ((CheckBox)grdTipoPerda.FooterRow.FindControl("cbxExibirPainelProducao")).Checked;

                var perdaFluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<Glass.PCP.Negocios.IPerdaFluxo>();

                var resultado = perdaFluxo.SalvarTipoPerda(tipoPerda);
                if (!resultado)
                    Glass.MensagemAlerta.ErrorMsg("Falha ao inserir tipo de perda.", resultado);
    
                else
                    grdTipoPerda.DataBind();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir tipo de perda.", ex, Page);
            }
        }

        protected void grdTipoPerda_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "AtivarInativar")
            {
                try
                {
                    var perdaFluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Glass.PCP.Negocios.IPerdaFluxo>();

                    var idTipoPerda = int.Parse(e.CommandArgument.ToString());

                    var tipoPerda = perdaFluxo.ObtemTipoPerda(idTipoPerda);
                    if (tipoPerda != null)
                    {
                        // Altera a situa��o do tipo perda
                        tipoPerda.Situacao = tipoPerda.Situacao == Data.Model.SituacaoTipoPerda.Ativo ?
                            Data.Model.SituacaoTipoPerda.Inativo : Data.Model.SituacaoTipoPerda.Ativo;
                        var resultado = perdaFluxo.SalvarTipoPerda(tipoPerda);

                        if (!resultado.Success)
                            Glass.MensagemAlerta.ErrorMsg("N�o foi poss�vel alterar a situa��o do tipo perda", resultado);
                        else
                            grdTipoPerda.DataBind();
                    }
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao alterar situa��o do tipo perda", ex, Page);
                }
            }
        }
    }
}

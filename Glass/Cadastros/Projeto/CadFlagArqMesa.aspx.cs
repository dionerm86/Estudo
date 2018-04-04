using Sync.Controls;
using System;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros.Projeto
{
    public partial class CadFlagArqMesa : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            grdFlag.Register(true, true);
            odsFlag.Register();
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            try
            {
                var flag = new Glass.Projeto.Negocios.Entidades.FlagArqMesa();
                flag.Descricao = ((TextBox)grdFlag.FooterRow.FindControl("txtDescricao")).Text;
                flag.Padrao = ((CheckBox)grdFlag.FooterRow.FindControl("chkPadrao")).Checked;
                flag.TipoArquivoArr = ((CheckBoxListDropDown)grdFlag.FooterRow.FindControl("drpTipoArquivo")).SelectedValues;
                flag.Situacao = Situacao.Ativo;

                var flagFluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<Glass.Projeto.Negocios.IFlagArqMesaFluxo>();

                var resultado = flagFluxo.SalvarFlagArqMesa(flag);
                if (!resultado)
                    Glass.MensagemAlerta.ErrorMsg("Falha ao inserir flag.", resultado);

                else
                    grdFlag.DataBind();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir flag.", ex, Page);
            }
        }
    }
}
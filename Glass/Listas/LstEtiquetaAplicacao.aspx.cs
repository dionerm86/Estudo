using System;
using System.Web.UI.WebControls;
using Microsoft.Practices.ServiceLocation;

namespace Glass.UI.Web.Listas
{
    public partial class LstEtiquetaAplicacao : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            grdAplicacao.Register(true, true);
            odsAplicacao.Register();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }
    
        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            try
            {
                var aplicacao = new Glass.Global.Negocios.Entidades.EtiquetaAplicacao();
                aplicacao.Descricao = ((TextBox)grdAplicacao.FooterRow.FindControl("txtDescricaoIns")).Text;
                aplicacao.CodInterno = ((TextBox)grdAplicacao.FooterRow.FindControl("txtCodInternoIns")).Text;
                aplicacao.DestacarEtiqueta = ((CheckBox)grdAplicacao.FooterRow.FindControl("chkDestacar")).Checked;
                aplicacao.GerarFormaInexistente = ((CheckBox)grdAplicacao.FooterRow.FindControl("chkGerarForma")).Checked;
                aplicacao.DiasMinimos = ((TextBox)grdAplicacao.FooterRow.FindControl("txtDiasMinimos")).Text.StrParaInt();
                aplicacao.TipoPedido = ((Sync.Controls.CheckBoxListDropDown)grdAplicacao.FooterRow.FindControl("drpTipoPedido")).SelectedValue;

                Glass.Situacao situacao = Situacao.Ativo;
                Enum.TryParse<Glass.Situacao>
                    (((DropDownList)grdAplicacao.FooterRow.FindControl("drpSituacao")).SelectedValue, out situacao);

                aplicacao.Situacao = situacao;

                var fluxo = ServiceLocator.Current.GetInstance<Glass.Global.Negocios.IEtiquetaFluxo>();

                var resultado = fluxo.SalvarEtiquetaAplicacao(aplicacao);
                if (!resultado)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao inserir Aplicação.", resultado);
                }
                else
                    grdAplicacao.DataBind();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir Aplicação.", ex, Page);
            }
        }

        protected void drpTipoPedido_Load(object sender, EventArgs e)
        {
            if (Configuracoes.EstoqueConfig.ControlarEstoqueVidrosClientes)
                ((Sync.Controls.CheckBoxListDropDown)sender).Items.Insert(2, new ListItem("Mão-de-obra Especial", "5"));
        }
    }
}

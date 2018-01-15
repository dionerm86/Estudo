using System;
using System.Web.UI.WebControls;
using Glass.Data.Model;
using Microsoft.Practices.ServiceLocation;

namespace Glass.UI.Web.Listas
{
    public partial class LstEtiquetaProcesso : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            grdProcesso.Register(true, true);
            odsProcesso.Register();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
        }
    
        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            try
            {
                string idAplicacao = ((HiddenField)grdProcesso.FooterRow.FindControl("hdfIdAplicacao")).Value;
    
                var processo = new Glass.Global.Negocios.Entidades.EtiquetaProcesso();
                processo.Descricao = ((TextBox)grdProcesso.FooterRow.FindControl("txtDescricaoIns")).Text;
                processo.CodInterno = ((TextBox)grdProcesso.FooterRow.FindControl("txtCodInternoIns")).Text;
                processo.IdAplicacao = !String.IsNullOrEmpty(idAplicacao) ? (int?)Glass.Conversoes.StrParaInt(idAplicacao) : null;
                processo.DestacarEtiqueta = ((CheckBox)grdProcesso.FooterRow.FindControl("chkDestacar")).Checked;
                processo.GerarFormaInexistente = ((CheckBox)grdProcesso.FooterRow.FindControl("chkGerarForma")).Checked;
                processo.GerarArquivoDeMesa = ((CheckBox)grdProcesso.FooterRow.FindControl("chkGerarArquivoDeMesa")).Checked;
                processo.TipoPedido = ((Sync.Controls.CheckBoxListDropDown)grdProcesso.FooterRow.FindControl("drpTipoPedido")).SelectedValue;
                processo.NumeroDiasUteisDataEntrega = ((TextBox)grdProcesso.FooterRow.FindControl("txtDiasEntrega")).Text.StrParaInt();

                Glass.Data.Model.EtiquetaTipoProcesso tipoProcesso = EtiquetaTipoProcesso.Instalacao;
                if (Enum.TryParse<Glass.Data.Model.EtiquetaTipoProcesso>
                    (((DropDownList)grdProcesso.FooterRow.FindControl("drpTipoProcesso")).SelectedValue, out tipoProcesso))
                    processo.TipoProcesso = tipoProcesso;

                Glass.Situacao situacao = Situacao.Ativo;
                Enum.TryParse<Glass.Situacao>
                    (((DropDownList)grdProcesso.FooterRow.FindControl("drpSituacao")).SelectedValue, out situacao);

                processo.Situacao = situacao;

                var fluxo = ServiceLocator.Current.GetInstance<Glass.Global.Negocios.IEtiquetaFluxo>();

                var resultado = fluxo.SalvarEtiquetaProcesso(processo);

                if (!resultado)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao inserir Processo.", resultado);
                }
                else
                    grdProcesso.DataBind();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir Processo.", ex, Page);
            }
        }

        protected void drpTipoPedido_Load(object sender, EventArgs e)
        {
            if (Configuracoes.EstoqueConfig.ControlarEstoqueVidrosClientes)
                ((Sync.Controls.CheckBoxListDropDown)sender).Items.Insert(2, new ListItem("Mão-de-obra Especial", "5"));
        }
    }
}

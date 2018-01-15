using System;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadDescontoParcelaPag : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadDescontoParcelaPag));

            drpTipo.Items[2].Enabled = Geral.ControleInstalacao &&
                PedidoConfig.Instalacao.ComissaoInstalacao;
        }

        protected void btnBuscarCompra_Click(object sender, EventArgs e)
        {
            if (txtNumCompra.Text == String.Empty)
            {
                tbDesconto.Visible = false;
                return;
            }

            uint idCompra = Glass.Conversoes.StrParaUint(txtNumCompra.Text);

            if (!CompraDAO.Instance.CompraExists(idCompra))
            {
                Glass.MensagemAlerta.ShowMsg("Não existe nenhuma compra com o número passado.", Page);
                tbDesconto.Visible = false;
            }
            else
            {
                int situacao = CompraDAO.Instance.ObtemSituacao(null, idCompra);

                if (situacao == (int)Compra.SituacaoEnum.Cancelada)
                {
                    Glass.MensagemAlerta.ShowMsg("Esta compra foi cancelada.", Page);
                    tbDesconto.Visible = false;
                }
                else if (situacao != (int)Compra.SituacaoEnum.Finalizada)
                {
                    Glass.MensagemAlerta.ShowMsg("Esta compra ainda não foi finalizada.", Page);
                    tbDesconto.Visible = false;
                }
                else
                    tbDesconto.Visible = true;
            }

            grdConta.DataBind();
        }

        protected void btnBuscarNf_Click(object sender, EventArgs e)
        {
            if (txtNumeroNf.Text == String.Empty)
            {
                tbDesconto.Visible = false;
                return;
            }

            uint numeroNf = Glass.Conversoes.StrParaUint(txtNumeroNf.Text);

            if (NotaFiscalDAO.Instance.GetByNumeroNFe(numeroNf, (int)NotaFiscal.TipoDoc.EntradaTerceiros).Length == 0)
            {
                Glass.MensagemAlerta.ShowMsg("Não existe nenhuma nota fiscal de entrada (terceiros) com o número passado.", Page);
                tbDesconto.Visible = false;
            }
            else
            {
                tbDesconto.Visible = true;
            }

            grdConta.DataBind();
        }
        
        protected void btnBuscarComissao_Click(object sender, EventArgs e)
        {
            tbDesconto.Visible = true;
            grdConta.DataBind();
        }

        protected void btnBuscarCustoFixo_Click(object sender, EventArgs e)
        {
            tbDesconto.Visible = true;
            grdConta.DataBind();
        }

        protected void btnBuscarCte_Click(object sender, EventArgs e)
        {
            tbDesconto.Visible = true;
            grdConta.DataBind();
        }

        protected void btnBuscarImpostoServ_Click(object sender, EventArgs e)
        {
            if (txtNumImpostoServ.Text == String.Empty)
            {
                tbDesconto.Visible = false;
                return;
            }

            uint idImpostoServ = Glass.Conversoes.StrParaUint(txtNumImpostoServ.Text);

            if (!ImpostoServDAO.Instance.Exists(idImpostoServ))
            {
                Glass.MensagemAlerta.ShowMsg("Não existe nenhum lançamento de imposto/serviço avulso com o número passado.", Page);
                tbDesconto.Visible = false;
            }
            else
            {
                int situacao = ImpostoServDAO.Instance.ObtemSituacao(idImpostoServ);

                if (situacao == (int)ImpostoServ.SituacaoEnum.Cancelado)
                {
                    Glass.MensagemAlerta.ShowMsg("Este lançamento de imposto/serviço avulso foi cancelado.", Page);
                    tbDesconto.Visible = false;
                }
                else if (situacao != (int)ImpostoServ.SituacaoEnum.Finalizado)
                {
                    Glass.MensagemAlerta.ShowMsg("Este lançamento de imposto/serviço avulso ainda não foi finalizado.", Page);
                    tbDesconto.Visible = false;
                }
                else
                    tbDesconto.Visible = true;
            }

            grdConta.DataBind();
        }

        protected void drpTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (drpTipo.SelectedIndex == 0)
            {
                drpNome.DataSourceID = "odsFuncionario";
                drpNome.DataTextField = "Nome";
                drpNome.DataValueField = "IdFunc";
            }
            else if (drpTipo.SelectedIndex == 1)
            {
                drpNome.DataSourceID = "odsComissionado";
                drpNome.DataTextField = "Nome";
                drpNome.DataValueField = "IdComissionado";
            }
            else
            {
                drpNome.DataSourceID = "odsInstalador";
                drpNome.DataTextField = "Nome";
                drpNome.DataValueField = "IdFunc";
            }

            hdfNome.Value = "";
            drpNome.DataBind();
        }

        protected void drpNome_SelectedIndexChanged(object sender, EventArgs e)
        {
            hdfNome.Value = drpNome.SelectedValue;
        }

        protected void drpNome_DataBound(object sender, EventArgs e)
        {
            if (hdfNome.Value != "")
            {
                drpNome.SelectedIndex = drpNome.Items.IndexOf(drpNome.Items.FindByValue(hdfNome.Value));
                if (drpNome.SelectedValue != hdfNome.Value)
                    hdfNome.Value = "";
            }
        }

        [Ajax.AjaxMethod()]
        public string GetByCompra(string idCompra)
        {
            return WebGlass.Business.ContasPagar.Fluxo.BuscarEValidar.Ajax.GetContasByCompra(idCompra);
        }

        [Ajax.AjaxMethod()]
        public string AplicarDescontoAcrescimo(string idContaPg, string valorString, string descontoString, string acrescimoString, string motivo)
        {
            return WebGlass.Business.ContasPagar.Fluxo.DescontoAcrescimo.Ajax.AplicarDescontoAcrescimo(idContaPg,
                valorString, descontoString, acrescimoString, motivo);
        }
    }
}

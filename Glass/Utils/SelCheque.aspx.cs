using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.UI.Web.Utils
{
    public partial class SelCheque : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(Request["adicionarTodosVisible"]))
                lnkAddAll.Visible = Request["adicionarTodosVisible"] == "true";

            if (!string.IsNullOrEmpty(Request["cliente"]))
            {
                string cliente = Request["cliente"];
                txtNumCli.Text = cliente;
                txtNome.Text = ClienteDAO.Instance.GetNome(Glass.Conversoes.StrParaUint(cliente));
            }

            Ajax.Utility.RegisterTypeForAjax(typeof(Utils.SelCheque));
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            
            bool receberCheque = Request["tipo"] == "3" || Request["tipo"] == "5" || Request["exibirSituacao"] == "1";
            Label15.Visible = receberCheque && !IsBuscarReapresentados();
            drpSituacao.Visible = receberCheque && !IsBuscarReapresentados();
            grdCheque.Columns[10].Visible = receberCheque && !IsBuscarReapresentados();

            if (Request["tipo"] == "5")
            {
                Label20.Visible = false;
                drpTipo.Visible = false;
                drpTipo.SelectedValue = IsFinanceiroPagto() ? "1" : "2";
                chkIncluirReapresentados.Checked = IsBuscarReapresentados();
                chkIncluirReapresentados.Visible = false;
            }
            else
            {
                chkIncluirReapresentados.Visible = (drpSituacao.SelectedValue == "3" || drpSituacao.SelectedValue == "10");

                if (Request["tipo"] == "2")
                {
                    Label20.Visible = false;
                    drpTipo.Visible = false;
                }
            }

            if (receberCheque)
            {
                foreach (ListItem i in drpSituacao.Items)
                    i.Enabled = IsBuscarReapresentados() ? i.Value == "11" :
                        i.Value == "1" || i.Value == "3" || i.Value == "6" || i.Value == "10" || (Request["tipo"] == "5" ? i.Value == "7" : false);
            }

            if (!PedidoConfig.LiberarPedido)
            {
                Label21.Visible = false;
                txtNumLiberarPedido.Visible = false;
            }

            // Salva a situação/control pagto passados por parâmetro em um hidden field
            hdfSituacao.Value = Request["situacao"];
            hdfControlPagto.Value = Request["controlPagto"];

            if (!IsPostBack)
            {
                int index = drpSituacao.Items.IndexOf(drpSituacao.Items.FindByValue(!IsBuscarReapresentados() ? Request["situacao"] : "11"));
                drpSituacao.SelectedIndex = index > -1 ? index : (receberCheque ? 1 : 0);
                grdCheque.DataBind();
            }
        }

        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdCheque.PageIndex = 0;
            grdCheque.DataBind();
        }

        protected void drpSituacao_SelectedIndexChanged(object sender, EventArgs e)
        {
            grdCheque.PageIndex = 0;
            grdCheque.DataBind();
        }

        protected bool IsBuscarReapresentados()
        {
            return Request["reapresentados"] == "1";
        }

        protected bool IsFinanceiroPagto()
        {
            return Request["pagto"] == "1";
        }

        [Ajax.AjaxMethod]
        public string ObterLista(string idPedido, string idLiberarPedido, string idAcerto, string numeroNfe, string tipo, string numCheque, string situacao,
            string reapresentado, string titular, string agencia, string conta, string dataIni, string dataFim, string idCli, string nomeCli,
            string idFornec, string nomeFornec, string valorInicial, string valorFinal, string ordenacao)
        {
            try
            {
                bool? result = Conversoes.ParaNullable<bool>(reapresentado);

                var cheques = ChequesDAO.Instance.GetForSel(Glass.Conversoes.StrParaUint(idPedido), Glass.Conversoes.StrParaUint(idLiberarPedido), Glass.Conversoes.StrParaUint(idAcerto),
                    Glass.Conversoes.StrParaUint(numeroNfe), Glass.Conversoes.StrParaInt(tipo), Glass.Conversoes.StrParaInt(numCheque), Glass.Conversoes.StrParaInt(situacao), result, titular, agencia, conta,
                    dataIni, dataFim, Glass.Conversoes.StrParaUint(idCli), nomeCli, Glass.Conversoes.StrParaUint(idFornec), nomeFornec, float.Parse(valorInicial),
                    float.Parse(valorFinal), Glass.Conversoes.StrParaInt(ordenacao));

                string ret = "";

                foreach (Cheques c in cheques)
                {
                    ret += c.IdCheque + "," + c.IdCliente + "," + c.Num + "," + c.Titular.Replace(",", "") + "," + c.Banco + "," + c.Agencia + "," + c.Conta + "," +
                        c.Valor.ToString().Replace(',', '.') + "," + c.ValorRestante.ToString().Replace(',', '.') + "," +
                        (c.DataVenc != null ? c.DataVenc.Value.ToString("dd/MM/yyyy") : String.Empty) + "," + c.Obs.Replace(",","") + "|";
                }

                return ret;
            }

            catch (Exception ex)
            {
                return "Erro;" + Glass.MensagemAlerta.FormatErrorMsg("Falha ao recuperar dados do pedido.", ex);
            }
        }
    }
}

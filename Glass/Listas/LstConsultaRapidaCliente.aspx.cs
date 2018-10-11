using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Model;
using Glass.Data.Helper;
using Glass.Data.DAL;
using System.Drawing;
using Glass.Configuracoes;
using Colosoft;
using System.Data;
using System.Linq;

namespace Glass.UI.Web.Listas
{
    public partial class LstConsultaRapidaCliente : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Listas.LstConsultaRapidaCliente));
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));

            if (string.IsNullOrEmpty(txtNumCli.Text))
            {
                tblInfo.Visible = false;
                tblTabelas.Visible = false;
            }

            lblPedidosBloqueio.Text = "Pedidos prontos porém não liberados a " + PedidoConfig.NumeroDiasPedidoProntoAtrasado + " dias";
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            var idCliente = Glass.Conversoes.StrParaUint(txtNumCli.Text);

            if (idCliente == 0)
                 return;

            lblTotalDebitos.Text = ContasReceberDAO.Instance.ObtemTotalDebitosCliente(idCliente).ToString("C");

            BuscaInfoCliente(idCliente);

            tblInfo.Visible = true;
            tblTabelas.Visible = true;
        }

        protected void BuscaInfoCliente(uint idCli)
        {
            Cliente cli = ClienteDAO.Instance.GetElement(idCli);

            #region Dados Cadastrais

            lblInfoCodCli.Text = cli.IdCli.ToString();
            lblInfoNomeCli.Text = cli.Nome;
            lblInfoContato.Text = ((!string.IsNullOrEmpty(cli.Contato) ? cli.Contato : null) +
            (!string.IsNullOrEmpty(cli.Contato1) ? ", " + cli.Contato1 : null) +
            (!string.IsNullOrEmpty(cli.Contato2) ? ", " + cli.Contato2 : null)).TrimStart(',').TrimEnd(',');
            lblInfoTelefoneContato.Text = ((!string.IsNullOrEmpty(cli.TelCont) ? cli.TelCont : null) +
                (!string.IsNullOrEmpty(cli.TelRes) ? ", " + cli.TelRes : null) +
                (!string.IsNullOrEmpty(cli.TelCel) ? ", " + cli.TelCel : null)).TrimStart(',').TrimEnd(',');
            lblInfoCPF.Text = Formatacoes.FormataCpfCnpj(cli.CpfCnpj);
            lblInfoTipoFiscal.Text = cli.TipoFiscalString;
            lblInfoTipoContribuinte.Text = cli.IndicadorIEDestinatario.Translate().Format();
            lblInfoInscEst.Text = cli.RgEscinst;
            lblInfoCrt.Text = cli.DescrCrt;
            lblInfoCnae.Text = cli.Cnae;
            lblEmail.Text = cli.Email;

            lblObs.Text = cli.Obs;
            lblObsLib.Text = cli.ObsLiberacao;
            lblObsNFe.Text = cli.ObsNfe;

            lblObs.ForeColor = Liberacao.TelaLiberacao.CorExibirObservacaoCliente;

            if (cli.IdFunc > 0)
                lblInfoVendedor.Text = FuncionarioDAO.Instance.GetNome((uint)cli.IdFunc.Value);

            lblInfoSituacao.Text = cli.DescrSituacao;

            if (cli.Situacao > 1)
            {
                lblInfoSituacao.ForeColor = Color.Red;
                lblInfoSituacao.Font.Bold = true;
            }

            #endregion

            #region Dados Financeiros

            lblFinancLimPadrao.Text = cli.Limite.ToString("C");
            lblPagarAntesProducao.Text = cli.PagamentoAntesProducao ? "Sim" : "Não";
            lblTabelaDescontoAcrescimo.Text = cli.TabelaDescontoAcrescimo;

            if (cli.Limite > 0)
                lblFinancLimDisp.Text = (cli.Limite - ContasReceberDAO.Instance.GetDebitos((uint)cli.IdCli, null)).ToString("C");

            lblFinancPagtoPadrao.Text = FormaPagtoDAO.Instance.GetDescricao((uint)cli.IdFormaPagto.GetValueOrDefault(0));

            var formasPagtoDisp = FormaPagtoDAO.Instance.GetByCliente((uint)cli.IdCli);
            if (formasPagtoDisp.Count > 0)
            {
                lblFinancPagtoDisp.Text = "<ul>";

                foreach (FormaPagto fp in formasPagtoDisp)
                {
                    lblFinancPagtoDisp.Text += "<li>" + fp.Descricao + "</li>";
                }

                lblFinancPagtoDisp.Text += "</ul>";
            }

            var parc = ParcelasDAO.Instance.GetPadraoCliente((uint)cli.IdCli);
            lblFinancParcPadrao.Text = parc != null ? parc.DescrCompleta : String.Empty;

            var parcelasDisp = ParcelasDAO.Instance.GetByClienteFornecedor((uint)cli.IdCli, 0, false, ParcelasDAO.TipoConsulta.Todos);
            if (parcelasDisp.Length > 0)
            {
                lblFinancParcDisp.Text = "<ul>";

                foreach (Parcelas p in parcelasDisp)
                {
                    if(!p.NaoUsar && p.Situacao == Situacao.Ativo)
                        lblFinancParcDisp.Text += "<li>" + p.DescrCompleta + "</li>";
                }

                lblFinancParcDisp.Text += "</ul>";
            }

            lblFinancPercSinalMin.Text = cli.PercSinalMinimo == null ? "" : cli.PercSinalMinimo.ToString() + "%";
            lblFinancCredito.Text = cli.Credito.ToString("C");

            lblContasVencidas.Text = ContasReceberDAO.Instance.ClientePossuiContasVencidas((uint)cli.IdCli) ? "Cliente Possui Contas Vencidas" : "";

            CarregarGrdVendas(idCli);

            #endregion
        }

        protected void imgPesq_click(object sender, ImageClickEventArgs e)
        {
            grdSugestao.PageIndex = 0;
        }

        [Ajax.AjaxMethod()]
        public static string GetCli(string idCli)
        {
            if (!ClienteDAO.Instance.Exists(Glass.Conversoes.StrParaUint(idCli)))
                return "Erro;Cliente não encontrado.";
            else
                return "Ok;" + ClienteDAO.Instance.GetNome(Glass.Conversoes.StrParaUint(idCli));
        }

        protected void grdLimiteCliente_DataBound(object sender, EventArgs e)
        {
        }

        protected void grdDescontoAcrescimo_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;

            e.Row.FindControl("grdLimiteCliente").DataBind();
        }

        protected void grdSugestao_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Cancelar")
            {
                SugestaoClienteDAO.Instance.Cancelar(Convert.ToInt32(e.CommandArgument));
                grdSugestao.DataBind();
            }
        }

        protected void grdSugestao_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var excluir = (LinkButton)e.Row.FindControl("lnkExcluir");

                if (UserInfo.GetUserInfo.IsAdministrador)
                {
                    excluir.Visible = true;
                }
                else
                {
                    excluir.Visible = false;
                }
            }
        }

        /// <summary>
        /// Recupera os dados para a grid de compras do cliente.
        /// </summary>
        /// <param name="idCliente"></param>
        protected void CarregarGrdVendas(uint idCliente)
        {
            var dataAtual = DateTime.Now;

            int mesInicio = dataAtual.AddMonths(-5).Month;
            int mesFim = dataAtual.Month;
            int anoInicio = dataAtual.AddMonths(-5).Year;
            int anoFim = dataAtual.Year;

            var vendasCliente = Data.RelDAL.VendasDAO.Instance.GetList(idCliente, null, null, false, 0, null,
                mesInicio, anoInicio, mesFim, anoFim, 0, null, 0, null, null, null, 0, 0, 0, 0, 0, false, null, 0, null)
                .FirstOrDefault();

            if (vendasCliente == null)
            {
                return;
            }

            var tabela = new DataTable();

            foreach (string mesAno in vendasCliente.MesVenda)
            {
                tabela.Columns.Add(new DataColumn(mesAno)
                {
                    DataType = typeof(string),
                });
            }

            var linha = tabela.NewRow();

            for (int i = 0; i < tabela.Columns.Count; i++)
            {
                linha[i] = string.Format("{0:C}", vendasCliente.ValorVenda[i]);
            }

            tabela.Columns.Add(new DataColumn("Total"));

            linha[tabela.Columns.Count - 1] = string.Format("{0:C}", vendasCliente.Total);

            tabela.Rows.Add(linha);

            grdVendas.DataSource = tabela;

            grdVendas.DataBind();
        }

    }
}

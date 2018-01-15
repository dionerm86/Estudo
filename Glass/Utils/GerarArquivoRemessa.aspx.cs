using Glass.Configuracoes;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Sync.Utils.Boleto.Models;
using System;
using System.Text;
using System.Web.UI.WebControls;
using System.Linq;

namespace Glass.UI.Web.Utils
{
    public partial class GerarArquivoRemessa : System.Web.UI.Page
    {
        #region Métodos Protegidos

        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(GerarArquivoRemessa));
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));

            if (FinanceiroConfig.SepararValoresFiscaisEReaisContasReceber)
            {
                lblTipoContaSemSeparacao.Style.Add("display", "none");
                drpTipoContaSemSeparacao.Style.Add("display", "none");
            }
        }

        protected void cbdFormaPagto_DataBound(object sender, EventArgs e)
        {
            ((Sync.Controls.CheckBoxListDropDown)sender).Items.RemoveAt(0);
        }

        protected void cbdTipo_DataBound(object sender, EventArgs e)
        {
            if (FinanceiroConfig.ImpedirGeracaoCnabContaCF)
            {
                var itemCf = new ListItem("CF", "8");

                if (((Sync.Controls.CheckBoxListDropDown)sender).Items.Contains(itemCf))
                    ((Sync.Controls.CheckBoxListDropDown)sender).Items.Remove(itemCf);
            }
            /* Chamado 42928. */
            else if (!FinanceiroConfig.SepararValoresFiscaisEReaisContasReceber)
                for (var i = 0; i < ((Sync.Controls.CheckBoxListDropDown)sender).Items.Count; i++)
                    ((Sync.Controls.CheckBoxListDropDown)sender).Items[i].Selected = true;
        }

        protected void btnGerarCnab_Click(object sender, EventArgs e)
        {
            try
            {
                var dados = ctrlDadosCnab.DadosCnab;
                Boletos boletos = dados;

                boletos.NumRemessa = txtNumRemessa.Text.StrParaInt();
                boletos.IdLoja = drpLojaGerar.SelectedValue.StrParaUint();
                boletos.IdsContaRec = hdfIdsContas.Value;
                boletos.IdContaBanco = drpContaBanco.SelectedValue.StrParaUint();

                /* Chamado 33737. */
                if (FinanceiroConfig.ContasReceber.ForcarInstrucaoPadrao &&
                    (Sync.Utils.CodigoBanco)dados.CodBanco == Sync.Utils.CodigoBanco.BancoBrasil)
                {
                    if (dados.Instrucao1 > 0)
                        boletos.Instrucoes.Add(
                            new Sync.Utils.Boleto.Instrucoes.InstrucaoBancoBrasil((Sync.Utils.Boleto.TipoArquivo)dados.TipoCnab,
                                dados.Instrucao1, 0, 0, 0, 0, 0, 0, 0));

                    if (dados.Instrucao2 > 0)
                        boletos.Instrucoes.Add(
                            new Sync.Utils.Boleto.Instrucoes.InstrucaoBancoBrasil((Sync.Utils.Boleto.TipoArquivo)dados.TipoCnab,
                                dados.Instrucao2, 0, 0, 0, 0, 0, 0, 0));
                }

                /* Chamado 33737. */
                if (FinanceiroConfig.ContasReceber.ForcarInstrucaoPadrao &&
                    (Sync.Utils.CodigoBanco)dados.CodBanco == Sync.Utils.CodigoBanco.Banrisul)
                {
                    if (dados.Instrucao1 > 0)
                        boletos.Instrucoes.Add(
                            new Sync.Utils.Boleto.Instrucoes.InstrucaoBanrisul(dados.Instrucao1, 0, 0, 0, 0, 0, 0, 0));

                    if (dados.Instrucao2 > 0)
                        boletos.Instrucoes.Add(
                            new Sync.Utils.Boleto.Instrucoes.InstrucaoBanrisul(dados.Instrucao2, 0, 0, 0, 0, 0, 0, 0));
                }

                var idArquivoRemessa = ArquivoRemessaDAO.Instance.GerarEnvio(boletos);

                dados.IdArquivoRemessa = (int)idArquivoRemessa;

                DadosCnabDAO.Instance.Insert(dados);

                Response.Redirect("~/Handlers/ArquivoRemessa.ashx?id=" + idArquivoRemessa + "&codBanco=" + boletos.CodigoBanco);
            }
            catch (Exception ex)
            {
                MensagemAlerta.ErrorMsg("Falha ao gerar arquivo remessa", ex, Page);
            }
        }

        protected void drpContaBanco_TextChanged(object sender, EventArgs e)
        {
            CtrlChanged(0);
        }

        protected void ctrlDadosCnab_CtrlDadosCnabChange(object sender, EventArgs e)
        {
            CtrlChanged(((DropDownList)sender).SelectedValue.StrParaInt());
        }

        private void CtrlChanged(int tipoCnab)
        {
            var numRemessa = ArquivoRemessaDAO.Instance.GetNextNumRemessa(drpContaBanco.SelectedValue.StrParaUint());
            txtNumRemessa.Text = numRemessa.ToString();

            var codBanco = ContaBancoDAO.Instance.ObtemCodigoBanco(drpContaBanco.SelectedValue.StrParaUint());

            ((HiddenField)ctrlDadosCnab.FindControl("hdfCodBanco")).Value = codBanco.GetValueOrDefault().ToString();

            ctrlDadosCnab.CtrlCarregado = true;
            ctrlDadosCnab.LimpaControles();

            if (tipoCnab > 0)
            {
                ctrlDadosCnab.AlterarTipoCnab(tipoCnab);

                if (DadosCnabDAO.Instance.ObterTipoCnabPadrao(null, codBanco.GetValueOrDefault()) == tipoCnab)
                    ctrlDadosCnab.DadosCnab = DadosCnabDAO.Instance.ObtemValorPadrao(null, codBanco.GetValueOrDefault(), tipoCnab);
            }
            else
            {
                var tipoCnabPadrao = DadosCnabDAO.Instance.ObterTipoCnabPadrao(null, codBanco.GetValueOrDefault());

                if (tipoCnabPadrao > 0)
                    ctrlDadosCnab.DadosCnab = DadosCnabDAO.Instance.ObtemValorPadrao(null, codBanco.GetValueOrDefault(), tipoCnabPadrao);
            }

            ctrlDadosCnab.DataBind();
        }

        #endregion

        #region Métodos AJAX

        [Ajax.AjaxMethod]
        public string GetContas(string tipoPeriodo, string dataIni, string dataFim, string tiposConta, string tipoContaSemSeparacao, string formasPagto, string idCli, string nomeCli,
            string idLoja, string idContaBancoCliente, string idsContas)
        {
            var sb = new StringBuilder();

            var contas = ContasReceberDAO.Instance.GetForCnab(tipoPeriodo.StrParaInt(), dataIni, dataFim, tiposConta, tipoContaSemSeparacao.StrParaInt(),
                formasPagto, idCli.StrParaUint(), nomeCli, idLoja.StrParaUint(), idContaBancoCliente.StrParaInt(), idsContas);
            
            foreach (var c in contas)
            {
                var formaPagto = c.IdConta.Value == UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.ParcelamentoObra) ?
                    FormaPagtoDAO.Instance.GetElementByPrimaryKey((uint)Data.Model.Pagto.FormaPagto.Prazo) :
                    UtilsPlanoConta.GetFormaPagtoByIdConta(c.IdConta.Value);

                sb.Append(c.IdContaR + "\t");
                sb.Append(c.Referencia.Replace("\t", "").Replace("\n", "") + "\t");
                sb.Append(c.NomeCli.Replace("\t", "").Replace("\n", "") + "\t");
                sb.Append(c.DataCad + "\t");
                sb.Append(c.ValorVec.ToString("C") + "\t");
                sb.Append(c.DataVec + "\t");
                sb.Append(c.DescricaoContaContabil + "\t");
                sb.Append(formaPagto.Descricao + "\t");
                sb.Append(c.NomeLoja + "\t");
                sb.Append(c.Obs + "\t");
                sb.Append(c.NumParc + "\\" + c.NumParcMax + "\t");
                sb.Append("\n");
            }

            return sb.ToString().TrimEnd('\n');
        }

        [Ajax.AjaxMethod]
        public string GetContaBanco(string idLoja)
        {
            var id = idLoja.StrParaUint();

            if (id == 0)
                return null;

            var contasBanco = ContaBancoDAO.Instance.ObterBancoAgrupado(0, 0, id);

            if (contasBanco.Length == 0)
                return "";

            return contasBanco[0].IdContaBanco.ToString();
        }

        [Ajax.AjaxMethod]
        public string GetNumArquivoRemessa(string idContaBanco)
        {
            return ArquivoRemessaDAO.Instance.GetNextNumRemessa(idContaBanco.StrParaUint()).ToString();
        }

        [Ajax.AjaxMethod]
        public string GetIdLoja(string idContaBanco)
        {
            return ContaBancoDAO.Instance.ObtemIdLoja(idContaBanco.StrParaUint()).ToString();
        }

        [Ajax.AjaxMethod]
        public string GetCodBanco(string idContaBanco)
        {
            return ContaBancoDAO.Instance.ObtemCodigoBanco(idContaBanco.StrParaUint()).ToString();
        }

        #endregion
    }
}
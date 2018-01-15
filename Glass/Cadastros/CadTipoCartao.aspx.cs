using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadTipoCartao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(CadTipoCartao));

            if (!IsPostBack)
            {
                var idTipoCartao = Request["idTipoCartao"].StrParaInt();
                if (idTipoCartao == 0)
                    Response.Redirect("~/Listas/LstTipoCartao.aspx");

                drpCartao.SelectedValue = idTipoCartao.ToString();
                CarregaPlanoContas(idTipoCartao);

                if (!UserInfo.GetUserInfo.IsAdminSync)
                {
                    foreach (var item in fsPlanosConta.Controls)
                    {
                        if (item is LinkButton)
                            ((LinkButton)item).Visible = false;
                        else if (item is Button)
                            ((Button)item).Enabled = false;
                    }
                    
                }
            }
        }

        #region Associação do cartão à conta bancária

        protected void drpTipoCartao_DataBound(object sender, EventArgs e)
        {
            if (drpContaBanco.Items.Count == 0)
                drpContaBanco.DataBind();

            drpTipoCartao_SelectedIndexChanged(sender, e);
        }

        protected void drpTipoCartao_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var idTipoCartao = drpCartao.SelectedValue.StrParaUintNullable();
                var idLoja = drpLoja.SelectedValue.StrParaUint();

                if (idTipoCartao.GetValueOrDefault(0) > 0)
                {
                    var dados = AssocContaBancoDAO.Instance.GetContaBancoCartao(idTipoCartao.Value, idLoja);
                    drpContaBanco.SelectedValue = dados.IdContaBanco > 0 ? dados.IdContaBanco.ToString() : "";
                    chkBloquearContaBanco.Checked = dados.BloquearContaBanco;
                }

                var tipoCartao = TipoCartaoCreditoDAO.Instance.GetElementByPrimaryKey(idTipoCartao.GetValueOrDefault(0));
                var numParc = tipoCartao.NumParc;
                txtNumParc.Text = numParc.ToString();
                txtNumParc.Enabled = tipoCartao.Tipo == TipoCartaoEnum.Credito;

                // Atualiza a tabela de parcelas
                txtNumParc_TextChanged(sender, e);
            }
            catch { }
        }

        protected void btnSalvarAssocContaBanco_Click(object sender, EventArgs e)
        {
            try
            {
                var idTipoCartao = drpCartao.SelectedValue.StrParaUint();
                var idContaBanco = drpContaBanco.SelectedValue.StrParaUintNullable();
                var idLoja = drpLoja.SelectedValue.StrParaUint();

               var contaAntiga = AssocContaBancoDAO.Instance.GetContaBancoCartao(idTipoCartao, idLoja);

                AssocContaBancoDAO.Instance.AtualizarTipoCartao(idTipoCartao, idContaBanco, chkBloquearContaBanco.Checked, drpLoja.SelectedValue.StrParaUint());
                LogAlteracaoDAO.Instance.LogTipoCartao(null, (int)idTipoCartao, (int)idLoja, ContaBancoDAO.Instance.GetDescricao(contaAntiga.IdContaBanco), ContaBancoDAO.Instance.GetDescricao(idContaBanco.GetValueOrDefault(0)));

                MensagemAlerta.ShowMsg("Conta bancária associada ao cartão.", Page);
                drpTipoCartao_SelectedIndexChanged(sender, e);
            }
            catch (Exception ex)
            {
                MensagemAlerta.ErrorMsg("Falha ao associar conta bancária ao cartão.", ex, Page);
            }
        }

        #endregion

        #region Juros de parcela cartão de crédito

        protected void txtNumParc_TextChanged(object sender, EventArgs e)
        {
            try
            {
                var idLoja = drpLoja.SelectedValue.StrParaUint();
                var idTipoCartao = drpCartao.SelectedValue.StrParaUint();
                var numParc = txtNumParc.Text.StrParaInt();

                // Atualiza a tabela de parcelas
                tblJurosParc.Rows.Clear();

                for (int i = 1; i <= numParc; i++)
                {
                    TableRow linha = new TableRow();
                    TableCell cabecalho = new TableCell();
                    TableCell dados = new TableCell();

                    cabecalho.Text = "Juros " + i + "ª Parcela";
                    TextBox txtJuros = new TextBox();
                    txtJuros.ID = "txtJuros" + i;
                    txtJuros.Text = JurosParcelaCartaoDAO.Instance.GetByTipoCartaoNumParc(idTipoCartao, idLoja, i).Juros.ToString();
                    txtJuros.Width = new Unit("40px");
                    txtJuros.Attributes.Add("onkeypress", "return soNumeros(event, false, true)");
                    dados.Controls.Add(txtJuros);

                    Label lblPerc = new Label();
                    lblPerc.ID = "lblPerc" + i;
                    lblPerc.Text = "%";
                    dados.Controls.Add(lblPerc);

                    linha.Cells.AddRange(new TableCell[] { cabecalho, dados });
                    tblJurosParc.Rows.Add(linha);
                }
            }
            catch { }
        }

        [Ajax.AjaxMethod]
        public string SalvarJurosParc(string idLojaStr, string idTipoCartaoStr, string numParcStr, string jurosStr)
        {
            try
            {
                // Salva o número de parcelas do cartão
                uint idLoja = Glass.Conversoes.StrParaUint(idLojaStr);
                uint idTipoCartao = Glass.Conversoes.StrParaUint(idTipoCartaoStr);
                int numParc = Glass.Conversoes.StrParaInt(numParcStr);

                var descricaoAnterior = JurosParcelaCartaoDAO.Instance.ObtemDescricaoJurosParcelas(null, (int)idTipoCartao, (int)idLoja);

                TipoCartaoCreditoDAO.Instance.AlteraNumParc(idTipoCartao, numParc);

                // Separa os juros
                string[] dadosJuros = jurosStr.Split(';');

                // Salva os juros de parcelas
                for (int i = 0; i < numParc; i++)
                {
                    float juros = float.Parse(dadosJuros[i].Replace(".", ","));
                    JurosParcelaCartaoDAO.Instance.AlteraJurosParc(idTipoCartao, idLoja, i + 1, juros);
                }

                TipoCartaoCreditoDAO.Instance.AtualizaLog(null, (int)idTipoCartao, descricaoAnterior, (int)idLoja);

                return "Ok;Juros das parcelas salvos com sucesso!";
            }
            catch (Exception ex)
            {
                return "Erro;" + Glass.MensagemAlerta.FormatErrorMsg("Falha ao salvar juros de parcela.", ex);
            }
        }

        #endregion

        protected void btnSalvarPlanoConta_Click(object sender, EventArgs e)
        {
            try
            {
                //Verifica se todos os planos de conta forma informados.
                if (hdfDevolucaoPagto.Value.StrParaUint() == 0 || hdfEntrada.Value.StrParaUint() == 0 || hdfEstorno.Value.StrParaUint() == 0 ||
                    hdfEstornoChequeDev.Value.StrParaUint() == 0 || hdfEstornoDevolucaoPagto.Value.StrParaUint() == 0 || hdfEstornoEntrada.Value.StrParaUint() == 0 ||
                    hdfEstornoRecPrazo.Value.StrParaUint() == 0 || hdfFunc.Value.StrParaUint() == 0 || hdfRecChequeDev.Value.StrParaUint() == 0 ||
                    hdfRecPrazo.Value.StrParaUint() == 0 || hdfVista.Value.StrParaUint() == 0)
                {
                    MensagemAlerta.ShowMsg("Planos de conta associados.", Page);
                    return;
                }

                List<uint> listPlanoContas = new List<uint>();
                listPlanoContas.Add(hdfDevolucaoPagto.Value.StrParaUint());
                listPlanoContas.Add(hdfEntrada.Value.StrParaUint());
                listPlanoContas.Add(hdfEstorno.Value.StrParaUint());
                listPlanoContas.Add(hdfEstornoChequeDev.Value.StrParaUint());
                listPlanoContas.Add(hdfEstornoDevolucaoPagto.Value.StrParaUint());
                listPlanoContas.Add(hdfEstornoEntrada.Value.StrParaUint());
                listPlanoContas.Add(hdfEstornoRecPrazo.Value.StrParaUint());
                listPlanoContas.Add(hdfFunc.Value.StrParaUint());
                listPlanoContas.Add(hdfRecChequeDev.Value.StrParaUint());
                listPlanoContas.Add(hdfRecPrazo.Value.StrParaUint());
                listPlanoContas.Add(hdfVista.Value.StrParaUint());
                //Verifica se existe algum plano de conta duplicado dentro do mesmo tipo de cartão
                var duplicado = listPlanoContas.GroupBy(x => x)
                    .Where(x => x.Count() > 1)
                    .ToList().Count > 0;
                if (duplicado)
                    throw new Exception("Foi selecionado o mesmo plano de conta para mais de uma forma de recebimento.");

                var idTipoCartao = drpCartao.SelectedValue.StrParaUint();

                //Verifica se existe algum plano de conta duplicado entre todos os tipos de cartão.
                var listPlanoContasEmUsoTipoCartao = TipoCartaoCreditoDAO.Instance.PlanosContaEmUsoTipoCartao(idTipoCartao);
                var listPlanoRepetido = listPlanoContas.Intersect(listPlanoContasEmUsoTipoCartao);
                if (listPlanoRepetido.Count<uint>() > 0)
                    throw new Exception("Um ou mais planos de conta informados para esse tipo de cartão já está sendo utilizado por outro tipo de cartão.");

                var tipoCartao = TipoCartaoCreditoDAO.Instance.GetElementByPrimaryKey(idTipoCartao);

                tipoCartao.IdContaDevolucaoPagto = hdfDevolucaoPagto.Value.StrParaInt();
                tipoCartao.IdContaEntrada = hdfEntrada.Value.StrParaInt();
                tipoCartao.IdContaEstorno = hdfEstorno.Value.StrParaInt();
                tipoCartao.IdContaEstornoChequeDev = hdfEstornoChequeDev.Value.StrParaInt();
                tipoCartao.IdContaEstornoDevolucaoPagto = hdfEstornoDevolucaoPagto.Value.StrParaInt();
                tipoCartao.IdContaEstornoEntrada = hdfEstornoEntrada.Value.StrParaInt();
                tipoCartao.IdContaEstornoRecPrazo = hdfEstornoRecPrazo.Value.StrParaInt();
                tipoCartao.IdContaFunc = hdfFunc.Value.StrParaInt();
                tipoCartao.IdContaRecChequeDev = hdfRecChequeDev.Value.StrParaInt();
                tipoCartao.IdContaRecPrazo = hdfRecPrazo.Value.StrParaInt();
                tipoCartao.IdContaVista = hdfVista.Value.StrParaInt();

                TipoCartaoCreditoDAO.Instance.Update(tipoCartao);

                /* Chamado 44948. */
                Data.Helper.UtilsPlanoConta.CarregarContasCartoes();

                MensagemAlerta.ShowMsg("Planos de conta associados.", Page);
                drpTipoCartao_SelectedIndexChanged(sender, e);
                CarregaPlanoContas(Request["idTipoCartao"].StrParaInt());
            }
            catch (Exception ex)
            {
                MensagemAlerta.ErrorMsg("Falha ao associar plano de conta ao cartão.", ex, Page);
            }
        }

        protected void drpLoja_SelectedIndexChanged(object sender, EventArgs e)
        {
            drpTipoCartao_SelectedIndexChanged(sender, e);
            CarregaPlanoContas(Request["idTipoCartao"].StrParaInt());
        }

        public void CarregaPlanoContas(int idTipoCartao)
        {
            try
            {
                var tipoCartao = TipoCartaoCreditoDAO.Instance.GetElementByPrimaryKey(idTipoCartao);

                hdfDevolucaoPagto.Value = tipoCartao.IdContaDevolucaoPagto.ToString();
                hdfEntrada.Value = tipoCartao.IdContaEntrada.ToString();
                hdfEstorno.Value = tipoCartao.IdContaEstorno.ToString();
                hdfEstornoChequeDev.Value = tipoCartao.IdContaEstornoChequeDev.ToString();
                hdfEstornoDevolucaoPagto.Value = tipoCartao.IdContaEstornoDevolucaoPagto.ToString();
                hdfEstornoEntrada.Value = tipoCartao.IdContaEstornoEntrada.ToString();
                hdfEstornoRecPrazo.Value = tipoCartao.IdContaEstornoRecPrazo.ToString();
                hdfFunc.Value = tipoCartao.IdContaFunc.ToString();
                hdfRecChequeDev.Value = tipoCartao.IdContaRecChequeDev.ToString();
                hdfRecPrazo.Value = tipoCartao.IdContaRecPrazo.ToString();
                hdfVista.Value = tipoCartao.IdContaVista.ToString();

                lblDevolucaoPagto.Text = PlanoContasDAO.Instance.GetDescricao((uint)tipoCartao.IdContaDevolucaoPagto, true);
                lblEntrada.Text = PlanoContasDAO.Instance.GetDescricao((uint)tipoCartao.IdContaEntrada, true);
                lblEstorno.Text = PlanoContasDAO.Instance.GetDescricao((uint)tipoCartao.IdContaEstorno, true);
                lblEstornoChequeDev.Text = PlanoContasDAO.Instance.GetDescricao((uint)tipoCartao.IdContaEstornoChequeDev, true);
                lblEstornoDevolucaoPagto.Text = PlanoContasDAO.Instance.GetDescricao((uint)tipoCartao.IdContaEstornoDevolucaoPagto, true);
                lblEstornoEntrada.Text = PlanoContasDAO.Instance.GetDescricao((uint)tipoCartao.IdContaEstornoEntrada, true);
                lblEstornoRecPrazo.Text = PlanoContasDAO.Instance.GetDescricao((uint)tipoCartao.IdContaEstornoRecPrazo, true);
                lblFunc.Text = PlanoContasDAO.Instance.GetDescricao((uint)tipoCartao.IdContaFunc, true);
                lblRecChequeDev.Text = PlanoContasDAO.Instance.GetDescricao((uint)tipoCartao.IdContaRecChequeDev, true);
                lblRecPrazo.Text = PlanoContasDAO.Instance.GetDescricao((uint)tipoCartao.IdContaRecPrazo, true);
                lblVista.Text = PlanoContasDAO.Instance.GetDescricao((uint)tipoCartao.IdContaVista, true);

            }
            catch { }
        }
    }
}
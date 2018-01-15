using System;
using System.Linq;
using System.Text;
using Glass.Data.DAL;
using Glass.Data.Model;

namespace WebGlass.Business.ContasPagar.Ajax
{
    public interface IBuscarEValidar
    {
        string GetContasByCompra(string idCompra, string soAVistaStr);
        string GetContasByCompra(string idCompra);
        string GetContasByCustoFixo(string idCustoFixo, string soAVistaStr);
        string GetContasByImpostoServ(string idImpostoServ, string soAVistaStr);
        string GetDadosPagto(string idPagto, string controleFormaPagto, string controleParcelas, 
            string callbackIncluir, string callbackExcluir);
    }

    internal class BuscarEValidar : IBuscarEValidar
    {
        public string GetContasByCompra(string idCompra, string soAVistaStr)
        {
            try
            {
                if (!CompraDAO.Instance.CompraExists(Glass.Conversoes.StrParaUint(idCompra)))
                    return "Erro#Não existe compra com o número informado.";

                var compra = CompraDAO.Instance.GetCompra(Glass.Conversoes.StrParaUint(idCompra));
                bool soAVista = bool.Parse(soAVistaStr);

                var lstContas = ContasPagarDAO.Instance.GetNaoPagasByCompra(compra.IdCompra, soAVista);
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append("ok#");

                foreach (var c in lstContas)
                {
                    sb.Append(c.IdContaPg + "\t");
                    sb.Append(c.IdFornec + "\t");
                    sb.Append(c.NomeFornec.Replace("'", "") + "\t");
                    sb.Append(c.ValorVenc.ToString("C") + "\t");
                    sb.Append(c.DataVenc.ToString("dd/MM/yyyy") + "\t");
                    sb.Append(c.DescrPlanoConta.Replace("'", "") + "|");
                }

                return sb.ToString().TrimEnd('|') + "#" + compra.IdFornec + "#" + compra.NomeFornec.Replace("'", "") + "#" + compra.Obs;
            }
            catch (Exception ex)
            {
                return "Erro#" + ex.Message;
            }
        }

        public string GetContasByCompra(string idCompra)
        {
            string retorno = "";
            foreach (var c in ContasPagarDAO.Instance.GetByCompra(Glass.Conversoes.StrParaUint(idCompra)))
                retorno += c.IdContaPg + ",";

            return retorno.TrimEnd(',');
        }

        public string GetContasByCustoFixo(string idCustoFixo, string soAVistaStr)
        {
            try
            {
                if (!CustoFixoDAO.Instance.Exists(Glass.Conversoes.StrParaUint(idCustoFixo)))
                    return "Erro#Não existe custo fixo com o número informado.";

                var custoFixo = CustoFixoDAO.Instance.GetElement(Glass.Conversoes.StrParaUint(idCustoFixo));
                bool soAVista = bool.Parse(soAVistaStr);

                var lstContas = ContasPagarDAO.Instance.GetNaoPagasByCustoFixo(custoFixo.IdCustoFixo, soAVista);
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append("ok#");

                foreach (var c in lstContas)
                {
                    sb.Append(c.IdContaPg + "\t");
                    sb.Append(c.IdFornec + "\t");
                    sb.Append(c.NomeFornec.Replace("'", "") + "\t");
                    sb.Append(c.ValorVenc.ToString("C") + "\t");
                    sb.Append(c.DataVenc.ToString("dd/MM/yyyy") + "\t");
                    sb.Append(c.DescrPlanoConta.Replace("'", "") + "|");
                }

                return sb.ToString().TrimEnd('|') + "#" + custoFixo.IdFornec + "#" + custoFixo.NomeFornec.Replace("'", "") + "#";
            }
            catch (Exception ex)
            {
                return "Erro#" + ex.Message;
            }
        }

        public string GetContasByImpostoServ(string idImpostoServ, string soAVistaStr)
        {
            try
            {
                if (!ImpostoServDAO.Instance.Exists(Glass.Conversoes.StrParaUint(idImpostoServ)))
                    return "Erro#Não existe lançamento de imposto/serviço com o número informado.";

                var imp = ImpostoServDAO.Instance.GetElement(Glass.Conversoes.StrParaUint(idImpostoServ));
                bool soAVista = bool.Parse(soAVistaStr);

                var lstContas = ContasPagarDAO.Instance.GetNaoPagasByImpostoServ(imp.IdImpostoServ, soAVista);
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append("ok#");

                foreach (var c in lstContas)
                {
                    sb.Append(c.IdContaPg + "\t");
                    sb.Append(c.IdFornec + "\t");
                    sb.Append(c.NomeFornec.Replace("'", "") + "\t");
                    sb.Append(c.ValorVenc.ToString("C") + "\t");
                    sb.Append(c.DataVenc.ToString("dd/MM/yyyy") + "\t");
                    sb.Append((c.DescrPlanoConta != null ? c.DescrPlanoConta.Replace("'", "") : String.Empty) + "|");
                }

                return sb.ToString().TrimEnd('|') + "#" + imp.IdFornec + "#" + imp.NomeFornec.Replace("'", "") + "#" + imp.Obs;
            }
            catch (Exception ex)
            {
                return "Erro#" + ex.Message;
            }
        }

        public string GetDadosPagto(string idPagto, string controleFormaPagto, string controleParcelas, 
            string callbackIncluir, string callbackExcluir)
        {
            try
            {
                var p = PagtoDAO.Instance.GetPagto(Glass.Conversoes.StrParaUint(idPagto));
                if (p == null)
                    throw new Exception("Pagamento não existe.");

                var cp = ContasPagarDAO.Instance.GetByPagto(p.IdPagto);
                var ch = ChequesDAO.Instance.GetByPagto(p.IdPagto).ToArray();
                var pp = PagtoPagtoDAO.Instance.GetByPagto(p.IdPagto);
                var cr = ContasPagarDAO.Instance.GetRenegociadasPagto(p.IdPagto);

                decimal valorContas = 0;

                StringBuilder contas = new StringBuilder();
                StringBuilder contaCheque = new StringBuilder();
                foreach (var c in cp)
                {
                    valorContas += c.ValorVenc + c.Multa + c.Juros - c.Desconto;
                    contas.Append("setContaPagar(");
                    contas.Append("'" + c.IdContaPg + "',");
                    contas.Append("'" + (c.IdCompra > 0 ? c.IdCompra.ToString() : "") + "',");
                    contas.Append("'" + (c.IdCustoFixo > 0 ? c.IdCustoFixo.ToString() : "") + "',");
                    contas.Append("'" + (c.IdImpostoServ > 0 ? c.IdImpostoServ.ToString() : "") + "',");
                    contas.Append("'" + (c.IdFornec > 0 ? c.IdFornec.ToString() : "") + "',");
                    contas.Append("'" + (c.NomeFornec != null ? c.NomeFornec : "").Replace("\'", "\\'") + "',");
                    contas.Append("'" + c.ValorVenc.ToString("0.00") + "',");
                    contas.Append("'" + c.DataVenc.ToString("dd/MM/yyyy") + "',");
                    contas.Append("'" + (!String.IsNullOrEmpty(c.DescrPlanoConta) ? c.DescrPlanoConta.Replace("\'", "\\'") : String.Empty) + "',");
                    contas.Append("null,");
                    contas.Append("'" + (c.Multa > 0 ? c.Multa.ToString("0.00") : "") + "',");
                    contas.Append("'" + (c.Juros > 0 ? c.Juros.ToString("0.00") : "") + "');\n");

                    if (c.IdChequePagto > 0)
                    {
                        var che = Array.Find(ch, cheque => cheque.IdCheque == c.IdChequePagto.Value);

                        contaCheque.Append(c.IdContaPg + ",");
                        contaCheque.Append(che.Num + "/" + che.Agencia + "/" + che.Conta + ";");
                    }
                }

                int campoChequeTerc = 0;
                int campoChequeProp = 0;
                StringBuilder formasPagto = new StringBuilder();
                StringBuilder parcelas = new StringBuilder();

                if (pp.Count > 0)
                {
                    foreach (var pp1 in pp)
                    {
                        string prefixoCampo = controleFormaPagto + "_tblFormaPagto_Pagto" + pp1.NumFormaPagto + "_";
                        formasPagto.Append("document.getElementById('" + prefixoCampo + "drpFormaPagto').value='" + pp1.IdFormaPagto + "';\n");

                        if (pp1.IdFormaPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeProprio)
                            campoChequeProp = pp1.NumFormaPagto;
                        else if (pp1.IdFormaPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeTerceiro)
                            campoChequeTerc = pp1.NumFormaPagto;
                        else
                            formasPagto.Append("document.getElementById('" + prefixoCampo + "txtValor').value='" + pp1.ValorPagto + "';\n");

                        if (pp1.IdContaBanco > 0)
                            formasPagto.Append("document.getElementById('" + prefixoCampo + "drpContaBanco').value='" + pp1.IdContaBanco + "';\n");

                        if (pp1.IdFormaPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.Boleto)
                            formasPagto.Append("document.getElementById('" + prefixoCampo + "txtNumeroBoleto').value='" + pp1.NumBoleto + "';\n");
                    }
                }
                else
                {
                    int numParcela = 1;
                    foreach (var c in cr)
                    {
                        string prefixoCampo = controleParcelas + "_Parc" + (numParcela++) + "_";
                        parcelas.Append("document.getElementById('" + prefixoCampo + "txtValor').value='" + c.ValorVenc + "';\n");
                        parcelas.Append("document.getElementById('" + prefixoCampo + "txtData').value='" + c.DataVenc.ToString("dd/MM/yyyy") + "';\n");
                    }
                }

                StringBuilder cheques = new StringBuilder();
                foreach (var c in ch)
                {
                    if (c.Situacao == (int)Cheques.SituacaoCheque.Devolvido || c.Situacao == (int)Cheques.SituacaoCheque.Protestado)
                        throw new Exception("O cheque de número " + c.Num + " está " + (c.DescrSituacao) + ", não é possível retificar este pagamento, é necessário quitar a conta a pagar gerada ao devolver/protestar o mesmo.");

                    /*nomeTabelaCheques, idCheque, contaBancoIdCheque, numCheque, digitoNum, titular, valor, dataVenc, banco, agencia, conta,
                    situacao, obs, selChequesWin, tipo, origem, idAcertoCheque, idContaR, idPedido, idSinal, idAcerto, idLiberarPedido, idTrocaDevolucao,
                    cpfCnpj, idLoja, nomeLoja, nomeCampo, linha, callbackIncluir, callbackExcluir, nomeControleFormaPagto, exibirCpfCnpj*/

                    cheques.Append("setCheque(");
                    cheques.Append("'" + controleFormaPagto + "_TabelaCheques',");
                    cheques.Append(c.IdCheque + ",");
                    cheques.Append("'" + (c.IdContaBanco > 0 ? c.IdContaBanco.Value.ToString() : "") + "',");
                    cheques.Append(c.Num + ",");
                    cheques.Append("'" + c.DigitoNum + "',");
                    cheques.Append("'" + (c.Titular != null ? c.Titular : "") + "',");
                    cheques.Append("'" + c.Valor.ToString("0.00") + "',");
                    cheques.Append("'" + (c.DataVenc != null ? c.DataVenc.Value.ToString("dd/MM/yyyy") : "") + "',");
                    cheques.Append("'" + (c.Banco != null ? c.Banco : "") + "',");
                    cheques.Append("'" + (c.Agencia != null ? c.Agencia : "") + "',");
                    cheques.Append("'" + (c.Conta != null ? c.Conta : "") + "',");
                    cheques.Append(c.Situacao + ",");
                    cheques.Append("'" + c.Obs.Replace("\r", "").Replace("\n", "") + "', null,");
                    cheques.Append("'" + (c.Tipo == 1 ? "proprio" : "terceiro") + "',");
                    cheques.Append(c.Origem + ",");
                    cheques.Append("'" + (c.IdAcertoCheque != null ? c.IdAcertoCheque.Value.ToString() : "") + "',");
                    cheques.Append("'" + (c.IdContaR != null ? c.IdContaR.Value.ToString() : "") + "',");
                    cheques.Append("'" + (c.IdPedido != null ? c.IdPedido.Value.ToString() : "") + "',");
                    cheques.Append("'" + (c.IdSinal != null ? c.IdSinal.Value.ToString() : "") + "',");
                    cheques.Append("'" + (c.IdAcerto != null ? c.IdAcerto.Value.ToString() : "") + "',");
                    cheques.Append("'" + (c.IdLiberarPedido != null ? c.IdLiberarPedido.Value.ToString() : "") + "',");
                    cheques.Append("'" + (c.IdTrocaDevolucao != null ? c.IdTrocaDevolucao.Value.ToString() : "") + "',");
                    cheques.Append("'" + (c.CpfCnpj != null ? c.CpfCnpj.ToString() : "") + "',");
                    cheques.Append(c.IdLoja + ",");
                    cheques.Append("'" + LojaDAO.Instance.GetNome(c.IdLoja) + "',");
                    cheques.Append("'" + controleFormaPagto + "_tblFormaPagto_Pagto" + (c.Tipo == 1 ? campoChequeProp : campoChequeTerc) + "_txtValor',");
                    cheques.Append("'',");
                    cheques.Append("'" + callbackIncluir + "',");
                    cheques.Append("'" + callbackExcluir + "');\n");
                }

                StringBuilder dadosPagto = new StringBuilder();
                dadosPagto.Append((p.NomeFornec != null ? p.NomeFornec : "") + "#");
                dadosPagto.Append((p.IdFornec > 0 ? p.IdFornec.Value.ToString() : "") + "#");
                dadosPagto.Append(p.DataPagto.ToString("dd/MM/yyyy") + "#");
                dadosPagto.Append((p.Desconto > 0 ? p.Desconto.ToString("0.00") : "") + "#");
                dadosPagto.Append(p.Obs + "#");

                valorContas = Math.Round(valorContas, 2);
                dadosPagto.Append((valorContas < p.ValorPago).ToString().ToLower() + "#");
                dadosPagto.Append((valorContas > p.ValorPago).ToString().ToLower() + "#");
                dadosPagto.Append((pp.Count == 0 && cr.Length > 0).ToString().ToLower() + "#");
                dadosPagto.Append(cr.Length);

                return "Ok#" + contas.ToString().TrimEnd(';', '\n') + "#" + cheques.ToString().TrimEnd(';', '\n') + "#" +
                    contaCheque.ToString().TrimEnd(';', '\n') + "#" + (pp.Count > 0 ? formasPagto : parcelas).ToString().TrimEnd(';', '\n') + "#" + dadosPagto;
            }
            catch (Exception ex)
            {
                return "Erro#" + ex.Message;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass;
using Glass.Configuracoes;
using System.Linq;

namespace WebGlass.Business.Pedido.Ajax
{
    public interface IBuscarEValidar
    {
        string GetPedidoParaConferencia(string idPedido);
        string IsPedidosAlterados(string idsPedidos, string idsSinais, string idsPagtoAntecip, string dataTela);
        string GetPedidosByCliente(string idCliente, string nomeCliente, string idsPedidosRem, string dataIni, string dataFim, string situacaoProd, string tiposPedidos, string idLoja);
        string ValidaPedido(string idPedidoStr, string tipoVendaStr, string idFormaPagtoStr, string cxDiario, string idsPedidoStr, string idsOc);
        string GetDadosPedido(string idPedidoStr);
        string GetPedidosByCliente(string idCliente, string nomeCliente);
        string PedidoExiste(string idPedido);
        string IsPedidoConfirmadoLiberado(string idPedido);
        string IsPedidoConfirmadoLiberado(string idPedido, string nf);
        string PedidosPossuemSt(string idsPedido);
    }

    internal class BuscarEValidar : IBuscarEValidar
    {
        public string GetPedidoParaConferencia(string idPedido)
        {
            try
            {
                if (!PedidoDAO.Instance.PedidoExists(Glass.Conversoes.StrParaUint(idPedido)))
                    return "Erro\tPedido informado não existe.";

                var pedido = PedidoDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(idPedido));
                var cliente = ClienteDAO.Instance.GetElementByPrimaryKey(pedido.IdCli);

                if (pedido.Situacao != Glass.Data.Model.Pedido.SituacaoPedido.EmConferencia &&
                    pedido.Situacao != Glass.Data.Model.Pedido.SituacaoPedido.AtivoConferencia)
                    return "Erro\tEste pedido não está em conferência.";

                int situacao = PedidoConferenciaDAO.Instance.ObtemSituacao(pedido.IdPedido);

                if (situacao == (int)Glass.Data.Model.PedidoConferencia.SituacaoConferencia.EmAndamento)
                    return "Erro\tA conferência deste pedido já está em andamento.";

                if (situacao == (int)Glass.Data.Model.PedidoConferencia.SituacaoConferencia.Finalizada)
                    return "Erro\tA conferência deste pedido já está finalizada.";

                StringBuilder str = new StringBuilder();
                str.Append("ok\t" + pedido.IdPedido + "\t");
                str.Append(LojaDAO.Instance.GetNome(pedido.IdLoja).Replace("'", "") + "\t");
                str.Append(cliente.Nome + "\t");
                str.Append(cliente.Telefone + "\t");
                str.Append(pedido.LocalizacaoObra + "\t");
                str.Append(pedido.DataEntrega != null ? pedido.DataEntrega.Value.ToString("dd/MM/yyyy") : String.Empty);

                return str.ToString();
            }
            catch (Exception ex)
            {
                return Glass.MensagemAlerta.FormatErrorMsg("Erro\tFalha ao carregar pedidos.", ex);
            }
        }

        public string IsPedidosAlterados(string idsPedidos, string idsSinais, string idsPagtoAntecip, string dataTela)
        {
            idsPedidos = idsPedidos.TrimStart(',').TrimEnd(',');
            idsSinais = idsSinais.TrimStart(',').TrimEnd(',');
            idsPagtoAntecip = idsPagtoAntecip.TrimStart(',').TrimEnd(',');

            var dataUltimaAlteracao = PedidoDAO.Instance.ObterDataUltimaAlteracaoPedido(null, idsPedidos);
            var dataRecebimentoSinalOuPagamentoAntecipado = PedidoDAO.Instance.ObterDataRecebimentoSinalOuPagamentoAntecipado(null, idsPedidos);
            var dataCancelamentoSinalOuPagamentoAntecipado = PedidoDAO.Instance.ObterDataCancelamentoSinalOuPagamentoAntecipado(null, idsSinais, idsPagtoAntecip);

            var mensagemRetorno = "true|Um ou mais pedidos {0}. Clique no botão Recalcular liberação ou atualize a página e insira os pedidos novamente.<br />{1}.{2}";

            // Verifica se a data da última ALTERAÇÃO do pedido é maior do que a data em que ele inserido na tela de liberação de pedido.
            if (dataUltimaAlteracao > Conversoes.ConverteData(dataTela))
            {
                return string.Format(mensagemRetorno, "sofreram alguma alteração após serem inseridos na tela",
                    // Pedidos verificados.
                    string.Format("{0}", !string.IsNullOrWhiteSpace(idsPedidos) ? string.Format("Pedido(s) verificado(s): {0}", idsPedidos) : string.Empty),
                    // Data da alteração do pedido.
                    string.Format("<br />Data da alteração: {0}.", dataUltimaAlteracao.Value.ToString("dd/MM/yyyy HH:mm:ss")));
            }
            // Verifica se a data de RECEBIMENTO do sinal ou pagamento antecipado do pedido é maior do que a data em que ele foi inserido na tela de liberação de pedido.
            else if (dataRecebimentoSinalOuPagamentoAntecipado > Conversoes.ConverteData(dataTela))
            {
                return string.Format(mensagemRetorno, "tiveram o sinal/pagamento antecipado recebido",
                    // Sinais verificados.
                    string.Format("{0} {1}", !string.IsNullOrWhiteSpace(idsSinais) ? string.Format("Sinal(is) verificado(s): {0}", idsSinais) : string.Empty,
                        // Pagamentos antecipados verificados.
                        !string.IsNullOrWhiteSpace(idsPagtoAntecip) ? string.Format("Pagamento(s) antecipado(s) verificado(s): {0}", idsPagtoAntecip) : string.Empty),
                    // Data do recebimento do sinal/pagamento antecipado.
                    string.Format("<br />Data do recebimento: {0}.", dataRecebimentoSinalOuPagamentoAntecipado.Value.ToString("dd/MM/yyyy HH:mm:ss")));
            }
            // Verifica se a data de CANCELAMENTO do sinal ou pagamento antecipado do pedido é maior do que a data em que ele foi inserido na tela de liberação de pedido.
            else if (dataCancelamentoSinalOuPagamentoAntecipado > Conversoes.ConverteData(dataTela))
            {
                return string.Format(mensagemRetorno, "tiveram o sinal/pagamento antecipado cancelado",
                    // Sinais verificados.
                    string.Format("{0} {1}", !string.IsNullOrWhiteSpace(idsSinais) ? string.Format("Sinal(is) verificado(s): {0}", idsSinais) : string.Empty,
                        // Pagamentos antecipados verificados.
                        !string.IsNullOrWhiteSpace(idsPagtoAntecip) ? string.Format("Pagamento(s) antecipado(s) verificado(s): {0}", idsPagtoAntecip) : string.Empty),
                    // Data do cancelamento.
                    string.Format("<br />Data do cancelamento: {0}.", dataCancelamentoSinalOuPagamentoAntecipado.Value.ToString("dd/MM/yyyy HH:mm:ss")));
            }
            else
                return "false";
        }

        public string GetPedidosByCliente(string idCliente, string nomeCliente, string idsPedidosRem, string dataIni,
            string dataFim, string situacaoProd, string tiposPedidos, string idLoja)
        {
            try
            {
                return PedidoDAO.Instance.GetIdsPedidosForLiberacao(Glass.Conversoes.StrParaUint(idCliente), nomeCliente, idsPedidosRem,
                    dataIni, dataFim, Glass.Conversoes.StrParaInt(situacaoProd), tiposPedidos, idLoja.StrParaInt());
            }
            catch
            {
                return "0";
            }
        }

        public string ValidaPedido(string idPedidoStr, string tipoVendaStr, string idFormaPagtoStr, string cxDiario, string idsPedidoStr, string idsOc)
        {
            var idPedido = idPedidoStr.StrParaUintNullable().GetValueOrDefault();
            var tipoVenda = tipoVendaStr.StrParaIntNullable();
            var idFormaPagto = idFormaPagtoStr.StrParaIntNullable();
            var idsPedido = !string.IsNullOrEmpty(idsPedidoStr) && idsPedidoStr.Split(',') != null ?
                idsPedidoStr.Split(',').Select(f => f.StrParaUintNullable().GetValueOrDefault()).ToList() : new List<uint>();

            return PedidoDAO.Instance.ValidaPedidoLiberacao(null, idPedido, tipoVenda, idFormaPagto, cxDiario == "1", idsPedido, idsOc);
        }

        public string GetDadosPedido(string idPedidoStr)
        {
            try
            {
                uint idPedido = Glass.Conversoes.StrParaUint(idPedidoStr);
                uint idCliente = PedidoDAO.Instance.ObtemIdCliente(null, idPedido);

                string enderecoObra = PedidoDAO.Instance.ObtemEnderecoObra(idPedido);
                string bairroObra = PedidoDAO.Instance.ObtemBairroObra(idPedido);
                string cidadeObra = PedidoDAO.Instance.ObtemCidadeObra(idPedido);
                string nomeCliente = ClienteDAO.Instance.GetNome(idCliente);
                string telCont = ClienteDAO.Instance.ObtemTelCont(idCliente);

                return "Ok;" + nomeCliente + ";" + telCont + ";" + enderecoObra + ";" + bairroObra + ";" + cidadeObra;
            }
            catch (Exception ex)
            {
                return "Erro;" + Glass.MensagemAlerta.FormatErrorMsg("Falha ao recuperar dados do pedido.", ex);
            }
        }

        public string GetPedidosByCliente(string idCliente, string nomeCliente)
        {
            try
            {
                return PedidoDAO.Instance.GetIdsForNFe(Glass.Conversoes.StrParaUint(idCliente), nomeCliente);
            }
            catch
            {
                return "0";
            }
        }

        public string PedidoExiste(string idPedido)
        {
            try
            {
                return PedidoDAO.Instance.PedidoExists(null, Glass.Conversoes.StrParaUint(idPedido)).ToString().ToLower();
            }
            catch
            {
                return "false";
            }
        }

        public string IsPedidoConfirmadoLiberado(string idPedido)
        {
            return IsPedidoConfirmadoLiberado(idPedido, "false");
        }

        public string IsPedidoConfirmadoLiberado(string idPedido, string nf)
        {
            string compl = PedidoConfig.LiberarPedido && FiscalConfig.BloquearEmissaoNFeApenasPedidosLiberados ? 
                "liberado" : "confirmado";

            if (FiscalConfig.PermitirGerarNotaPedidoConferido)
                compl = "conferido";

            try
            {
                return PedidoDAO.Instance.IsPedidoConfirmadoLiberado(Glass.Conversoes.StrParaUint(idPedido), 
                    bool.Parse(nf)).ToString().ToLower() + "|" + compl;
            }
            catch
            {
                return "false|" + compl;
            }
        }

        public string PedidosPossuemSt(string idsPedido)
        {
            return PedidoDAO.Instance.PedidosPossuemST(idsPedido).ToString().ToLower();
        }
    }
}

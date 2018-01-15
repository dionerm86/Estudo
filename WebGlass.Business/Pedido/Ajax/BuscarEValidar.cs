using System;
using System.Collections.Generic;
using System.Text;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass;
using Glass.Configuracoes;

namespace WebGlass.Business.Pedido.Ajax
{
    public interface IBuscarEValidar
    {
        string GetPedidoParaConferencia(string idPedido);
        string IsPedidosAlterados(string idsPedidos, string idsSinais, string idsPagtoAntecip, string dataTela);
        string GetPedidosByCliente(string idCliente, string nomeCliente, string idsPedidosRem, string dataIni,
            string dataFim, string situacaoProd, string tiposPedidos, string idloja);
        string ValidaPedido(string idPedidoStr, string tipoVendaStr, string cxDiario, string idsPedido);
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
            DateTime? dataComp = Conversoes.ConverteData(dataTela);
            DateTime? dataUltAlt = PedidoDAO.Instance.GetDataUltimoRecebAlt(idsPedidos.TrimStart(',').TrimEnd(','),
                idsSinais.TrimStart(',').TrimEnd(','), idsPagtoAntecip.TrimStart(',').TrimEnd(','));

            return (dataUltAlt > dataComp).ToString().ToLower();
        }

        public string GetPedidosByCliente(string idCliente, string nomeCliente, string idsPedidosRem, string dataIni,
            string dataFim, string situacaoProd, string tiposPedidos, string idLoja)
        {
            try
            {
                return PedidoDAO.Instance.GetIdsPedidosForLiberacao(Conversoes.StrParaUint(idCliente), nomeCliente, idsPedidosRem,
                    dataIni, dataFim, Conversoes.StrParaInt(situacaoProd), tiposPedidos, idLoja.StrParaInt());
            }
            catch
            {
                return "0";
            }
        }

        public string ValidaPedido(string idPedidoStr, string tipoVendaStr, string cxDiario, string idsPedido)
        {
            return PedidoDAO.Instance.ValidaPedidoLiberacao(idPedidoStr, tipoVendaStr, cxDiario, idsPedido);
        }

        public string GetDadosPedido(string idPedidoStr)
        {
            try
            {
                uint idPedido = Glass.Conversoes.StrParaUint(idPedidoStr);
                uint idCliente = PedidoDAO.Instance.ObtemIdCliente(idPedido);

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
                return PedidoDAO.Instance.PedidoExists(Glass.Conversoes.StrParaUint(idPedido)).ToString().ToLower();
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

using System;
using Glass.Data.DAL;

namespace WebGlass.Business.ContasReceber.Ajax
{
    public interface IBuscarEValidar
    {
        string GetContasRecFromPedido(string idPedido);
        string GetContasRecFromPedido(string idCliente, string idPedido);
    }

    internal class BuscarEValidar : IBuscarEValidar
    {
        public string GetContasRecFromPedido(string idPedido)
        {
            try
            {
                var lstContaRec = ContasReceberDAO.Instance.GetByPedido(null, Glass.Conversoes.StrParaUint(idPedido), true, false);

                if (lstContaRec.Count == 0)
                    return "Erro|Este pedido não possui parcelas em aberto.";

                string retorno = lstContaRec[0].IdCliente + "|";

                foreach (var c in lstContaRec)
                    retorno += c.IdContaR + ";" + idPedido + ";" + c.NomeCli.Replace(";", "") + ";" +
                        c.ValorVec.ToString("C") + ";" + c.DataVec.ToString("d") + ";" + c.ObsScript + "|";

                return retorno.TrimEnd('|');
            }
            catch (Exception ex)
            {
                return "Erro|" + Glass.MensagemAlerta.FormatErrorMsg(null, ex);
            }
        }

        public string GetContasRecFromPedido(string idCliente, string idPedido)
        {
            try
            {
                var lstContaRec = ContasReceberDAO.Instance.GetByPedido(null, Glass.Conversoes.StrParaUint(idPedido), true, false);

                if (lstContaRec.Count == 0)
                    return "Erro|Este pedido não possui parcelas em aberto.";

                if (idCliente != "0" && lstContaRec[0].IdCliente != Glass.Conversoes.StrParaUint(idCliente))
                    return "Erro|Este pedido não pertence ao mesmo cliente das outras parcelas adicionadas.";

                string retorno = lstContaRec[0].IdCliente + "|";

                foreach (var c in lstContaRec)
                    retorno += c.IdContaR + ";" + idPedido + ";" + c.PedidosLiberacao + ";" + c.NomeCli.Replace(";", "") + ";" +
                        c.ValorVec.ToString("C") + ";" + c.DataVec.ToString("d") + ";" + c.Juros + ";" + c.Multa + "|";

                return retorno.TrimEnd('|');
            }
            catch (Exception ex)
            {
                return "Erro|" + Glass.MensagemAlerta.FormatErrorMsg(null, ex);
            }
        }
    }
}

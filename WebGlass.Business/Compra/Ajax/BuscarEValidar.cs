using System;
using Glass.Data.DAL;

namespace WebGlass.Business.Compra.Ajax
{
    public interface IBuscarEValidar
    {
        string GetComprasByFornecedor(string idFornec, string nomeFornec);
        string ValidaCompra(string idCompra);
    }

    internal class BuscarEValidar : IBuscarEValidar
    {
        public string GetComprasByFornecedor(string idFornec, string nomeFornec)
        {
            try
            {
                return CompraDAO.Instance.GetIdsForNFe(Glass.Conversoes.StrParaUint(idFornec), nomeFornec);
            }
            catch
            {
                return "0";
            }
        }

        public string ValidaCompra(string idCompraStr)
        {
            try
            {
                uint idCompra = Glass.Conversoes.StrParaUint(idCompraStr);
                if (!CompraDAO.Instance.Exists(idCompra))
                    throw new Exception("Compra não existe.");

                var situacao = CompraDAO.Instance.ObtemSituacao(null, idCompra);

                if (situacao != (int)Glass.Data.Model.Compra.SituacaoEnum.EmAndamento &&
                    situacao != (int)Glass.Data.Model.Compra.SituacaoEnum.Finalizada)
                    throw new Exception("Compra não está finalizada.");

                string numerosNfe;
                if (!String.IsNullOrEmpty(numerosNfe = CompraNotaFiscalDAO.Instance.ObtemNumerosNFe(idCompra)))
                    throw new Exception("Essa compra já possui notas fiscais geradas (número(s) " + numerosNfe + ").");

                return "Ok";
            }
            catch (Exception ex)
            {
                return "Erro;" + Glass.MensagemAlerta.FormatErrorMsg("", ex);
            }
        }
    }
}

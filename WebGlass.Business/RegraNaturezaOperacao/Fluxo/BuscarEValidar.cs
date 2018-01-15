using Glass.Data.DAL;

namespace WebGlass.Business.RegraNaturezaOperacao.Fluxo
{
    public sealed class BuscarEValidar : BaseFluxo<BuscarEValidar>
    {
        private BuscarEValidar() { }

        public uint? BuscaCodigoNaturezaOperacaoPorRegra(bool gerandoNfSaida, uint? codigoLoja, uint? codigoCliente, int? codigoProduto)
        {
            return RegraNaturezaOperacaoDAO.Instance.BuscaNaturezaOperacao(0, null, codigoLoja, codigoCliente, codigoProduto, gerandoNfSaida);
        }

        public uint? BuscaCodigoNaturezaOperacaoPorRegra(uint codigoNotaFiscal, uint? codigoLoja, uint? codigoCliente, int? codigoProduto)
        {
            return RegraNaturezaOperacaoDAO.Instance.BuscaNaturezaOperacao(codigoNotaFiscal, codigoLoja, codigoCliente, codigoProduto);
        }

        public uint? BuscaCodigoNaturezaOperacaoPorRegra(uint idNf, Glass.Data.Model.NotaFiscal.TipoDoc tipoDocumentoNotaFiscal, uint idLoja, uint? idCliente,
            int? idProd)
        {
            return RegraNaturezaOperacaoDAO.Instance.BuscaNaturezaOperacao(idNf, tipoDocumentoNotaFiscal, idLoja, idCliente, idProd);
        }
    }
}

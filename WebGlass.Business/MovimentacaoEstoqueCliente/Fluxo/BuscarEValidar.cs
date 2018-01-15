using System.Collections.Generic;
using System.Linq;
using Glass.Data.DAL;

namespace WebGlass.Business.MovimentacaoEstoqueCliente.Fluxo
{
    public sealed class BuscarEValidar : BaseFluxo<BuscarEValidar>
    {
        private BuscarEValidar() { }

        public IList<Entidade.MovimentacaoEstoqueCliente> ObtemParaRelatorio(uint codigoLoja, uint codigoCliente,
            string codigoInternoProduto, string descricaoProduto, string dataIni, string dataFim, int tipoMovimentacao,
            int situacaoProduto, uint codigoCfop, uint codigoGrupoProduto, uint codigoSubgrupoProdo,
            uint codigoCorVidro, uint codigoCorFerragem, uint codigoCorAluminio)
        {
            var itens = MovEstoqueClienteDAO.Instance.GetForRpt(codigoLoja, codigoCliente, codigoInternoProduto,
                descricaoProduto, dataIni, dataFim, tipoMovimentacao, situacaoProduto, codigoCfop, codigoGrupoProduto,
                codigoSubgrupoProdo, codigoCorVidro, codigoCorFerragem, codigoCorAluminio);

            return itens.Select(x => new Entidade.MovimentacaoEstoqueCliente(x)).ToList();
        }
    }
}

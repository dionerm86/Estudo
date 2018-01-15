using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.DAL;

namespace WebGlass.Business.MovimentacaoEstoqueCliente.Fluxo
{
    public sealed class CRUD : BaseFluxo<CRUD>
    {
        private CRUD() { }

        #region Create

        public void CreditaEstoqueManual(uint codigoCliente, uint codigoProduto, uint codigoLoja,
            decimal quantidade, decimal? valor, DateTime dataMovimentacao, string observacao)
        {
            MovEstoqueClienteDAO.Instance.CreditaEstoqueManual(codigoCliente, codigoProduto, codigoLoja,
                quantidade, valor, dataMovimentacao, observacao);
        }

        public void BaixaEstoqueManual(uint codigoCliente, uint codigoProduto, uint codigoLoja,
            decimal quantidade, decimal? valor, DateTime dataMovimentacao, string observacao)
        {
            MovEstoqueClienteDAO.Instance.BaixaEstoqueManual(codigoCliente, codigoProduto, codigoLoja,
                quantidade, valor, dataMovimentacao, observacao);
        }

        #endregion

        #region Read

        public IList<Entidade.MovimentacaoEstoqueCliente> ObtemLista(uint codigoLoja, uint codigoCliente, 
            string codigoInternoProduto, string descricaoProduto, string dataIni, string dataFim, int tipoMovimentacao,
            int situacaoProduto, uint codigoCfop, uint codigoGrupoProduto, uint codigoSubgrupoProdo, 
            uint codigoCorVidro, uint codigoCorFerragem, uint codigoCorAluminio)
        {
            var itens = MovEstoqueClienteDAO.Instance.GetList(codigoLoja, codigoCliente, codigoInternoProduto, 
                descricaoProduto, dataIni, dataFim, tipoMovimentacao, situacaoProduto, codigoCfop, codigoGrupoProduto, 
                codigoSubgrupoProdo, codigoCorVidro, codigoCorFerragem, codigoCorAluminio);

            return itens.Select(x => new Entidade.MovimentacaoEstoqueCliente(x)).ToList();
        }

        #endregion

        #region Update
        #endregion

        #region Delete

        public int Remover(Entidade.MovimentacaoEstoqueCliente movimentacao)
        {
            return MovEstoqueClienteDAO.Instance.Delete(movimentacao._movEstoque);
        }

        #endregion
    }
}

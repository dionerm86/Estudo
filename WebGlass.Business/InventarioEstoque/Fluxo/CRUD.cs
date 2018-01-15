using System.Collections.Generic;
using System.Linq;
using Glass.Data.DAL;

namespace WebGlass.Business.InventarioEstoque.Fluxo
{
    public sealed class CRUD : BaseFluxo<CRUD>
    {
        private CRUD() { }

        #region Create

        public uint NovoInventario(Entidade.InventarioEstoque inventario)
        {
            inventario.Situacao = Entidade.InventarioEstoque.SituacaoEnum.Aberto;
            return InventarioEstoqueDAO.Instance.Insert(inventario._inventario);
        }

        #endregion

        #region Read

        public IList<Entidade.InventarioEstoque> ObtemItens(uint codigoLoja, uint codigoGrupoProduto, uint codigoSubgrupoProduto,
            Entidade.InventarioEstoque.SituacaoEnum? situacao, string sortExpression, int startRow, int pageSize)
        {
            var s = situacao != null ? (Glass.Data.Model.InventarioEstoque.SituacaoEnum?)(int)situacao : null;
            var itens = InventarioEstoqueDAO.Instance.ObtemItens(codigoLoja, codigoGrupoProduto, 
                codigoSubgrupoProduto, s, sortExpression, startRow, pageSize);

            return itens.Select(x => new Entidade.InventarioEstoque(x)).ToList();
        }

        public int ObtemNumeroRegistros(uint codigoLoja, uint codigoGrupoProduto, uint codigoSubgrupoProduto,
            Entidade.InventarioEstoque.SituacaoEnum? situacao)
        {
            var s = situacao != null ? (Glass.Data.Model.InventarioEstoque.SituacaoEnum?)(int)situacao : null;
            return InventarioEstoqueDAO.Instance.ObtemNumeroRegistros(codigoLoja, codigoGrupoProduto, 
                codigoSubgrupoProduto, s);
        }

        public Entidade.InventarioEstoque ObtemItem(uint codigoInventario)
        {
            var item = new Entidade.InventarioEstoque()
            {
                Codigo = codigoInventario
            };

            GDA.GDAOperations.RecoverData(item._inventario);
            return item;
        }

        #endregion

        #region Update

        public int Atualizar(Entidade.InventarioEstoque inventario)
        {
            return InventarioEstoqueDAO.Instance.Update(inventario._inventario);
        }

        #endregion

        #region Delete

        #endregion
    }
}

using GDA;
using Glass.Data.DAL;
using System;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(MovInternaEstoqueFiscalDAO))]
    [PersistenceClass("mov_interna_estoque_fiscal")]
    public class MovInternaEstoqueFiscal : ModelBaseCadastro, Sync.Fiscal.EFD.Entidade.IMovimentacaoInternaEstoque
    {
        #region Propiedades

        [PersistenceProperty("IdMovInternaEstoqueFiscal", PersistenceParameterType.IdentityKey)]
        public int IdMovInternaEstoqueFiscal { get; set; }

        [PersistenceForeignKey(typeof(Produto), "IdProd")]
        [PersistenceProperty("IdProdOrigem")]
        public int IdProdOrigem { get; set; }

        [PersistenceForeignKey(typeof(Produto), "IdProd")]
        [PersistenceProperty("IdProdDestino")]
        public int IdProdDestino { get; set; }

        [PersistenceForeignKey(typeof(Loja), "IdLoja")]
        [PersistenceProperty("IdLoja")]
        public int IdLoja { get; set; }

        [PersistenceProperty("QtdeOrigem")]
        public decimal QtdeOrigem { get; set; }

        [PersistenceProperty("QtdeDestino")]
        public decimal QtdeDestino { get; set; }

        #endregion

        #region EFD - IMovimentacaoInternaEstoque

        public DateTime DataMov
        {
            get { return DataCad; }
        }

        public int CodItemOri
        {
            get { return IdProdOrigem; }
        }

        public int CodItemDest
        {
            get { return IdProdDestino; }
        }

        #endregion
    }
}

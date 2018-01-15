using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class PagtoCreditoFornecedorDAO : BaseDAO<PagtoCreditoFornecedor, PagtoCreditoFornecedorDAO>
    {
        //private PagtoCreditoFornecedorDAO() { }

        public PagtoCreditoFornecedor[] GetPagamentos(uint idCreditoFornecedor)
        {
            return GetPagamentos(null, idCreditoFornecedor);
        }

        public PagtoCreditoFornecedor[] GetPagamentos(GDASession session, uint idCreditoFornecedor)
        {
            string sql = @"Select pcf.*, fp.Descricao As FormaPagamento, cb.Nome As ContaBanco
                from pagto_credito_fornecedor pcf
                Left Join formapagto fp On(pcf.IdFormaPagto=fp.IdFormaPagto)
                Left Join conta_banco cb On(pcf.IdContaBanco=cb.IdContaBanco)";
                        
            if(idCreditoFornecedor > 0)
                sql += " Where idCreditoFornecedor=" + idCreditoFornecedor;
            return objPersistence.LoadData(session, sql).ToList().ToArray();
        }
    }
}

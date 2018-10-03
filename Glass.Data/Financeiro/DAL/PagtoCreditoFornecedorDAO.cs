using Glass.Data.Model;
using GDA;
using System.Linq;
using System;

namespace Glass.Data.DAL
{
    public sealed class PagtoCreditoFornecedorDAO : BaseDAO<PagtoCreditoFornecedor, PagtoCreditoFornecedorDAO>
    {
        /// <summary>
        /// Obtem uma lista com os pagamentos de um CreditoFornecedor específico.
        /// Ou recupera a lista.
        /// </summary>
        /// <param name="idCreditoFornecedor">Identificador do Crédito do fornecedor</param>
        /// <returns></returns>
        public PagtoCreditoFornecedor[] GetPagamentos(uint idCreditoFornecedor)
        {
            return GetPagamentos(null, idCreditoFornecedor);
        }

        /// <summary>
        /// Método sobescrito Insert, para fazer a checagem da validade da forma de pagamento antes de inserir no banco de dados
        /// </summary>
        /// <param name="session">Sessão do GDA</param>
        /// <param name="objInsert">PagtoCreditoFornecedor a ser inserido no banco</param>
        /// <returns></returns>
        public override uint Insert(GDASession session, PagtoCreditoFornecedor objInsert)
        {
            var pagtoValido = FormaPagtoDAO.Instance.GetAll(session)
                .ToList()
                .Where(f => f.IdFormaPagto == objInsert.IdFormaPagto)
                .Any();

            if (!pagtoValido)
            {
                throw new Exception("Não foi possível processar alguma das formas de pagamento utilizadas. Tente efetuar o pagamento novamente.");
            }

            return base.Insert(session, objInsert);
        }

        /// <summary>
        /// Obtem uma lista com os pagamentos de um CreditoFornecedor específico
        /// Ou recupera a lista.
        /// </summary>
        /// <param name="session">Sessão do GDA</param>
        /// <param name="idCreditoFornecedor">Identificador do Crédito do fornecedor</param>
        /// <returns></returns>
        public PagtoCreditoFornecedor[] GetPagamentos(GDASession session, uint idCreditoFornecedor)
        {
            string sql = $@"Select pcf.*, fp.Descricao As FormaPagamento, cb.Nome As ContaBanco
                from pagto_credito_fornecedor pcf
                Left Join formapagto fp On(pcf.IdFormaPagto=fp.IdFormaPagto)
                Left Join conta_banco cb On(pcf.IdContaBanco=cb.IdContaBanco)
                Where idCreditoFornecedor={idCreditoFornecedor}";

            return objPersistence.LoadData(session, sql).ToList().ToArray();
        }
    }
}

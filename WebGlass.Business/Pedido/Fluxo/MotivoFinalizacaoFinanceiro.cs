using System;
using System.Linq;
using Glass.Data.DAL;

namespace WebGlass.Business.Pedido.Fluxo
{
    public sealed class MotivoFinalizacaoFinanceiro : BaseFluxo<MotivoFinalizacaoFinanceiro>
    {
        private MotivoFinalizacaoFinanceiro() { }

        public Entidade.MotivoFinalizacaoFinanceiro[] ObtemObservacoesFinanceiro(uint idPedido, uint idFuncCad, 
            string dataCadIni, string dataCadFim, string motivo, string sortExpression, int startRow, int pageSize)
        {
            var itens = ObservacaoFinalizacaoFinanceiroDAO.Instance.ObtemObservacoesFinalizacao(idPedido, 
                idFuncCad, dataCadIni, dataCadFim, motivo, sortExpression, startRow, pageSize).ToArray();

            return Array.ConvertAll(itens, x => new Entidade.MotivoFinalizacaoFinanceiro(x));
        }

        public int ObtemNumeroObservacoesFinanceiro(uint idPedido, uint idFuncCad, string dataCadIni, 
            string dataCadFim, string motivo)
        {
            return ObservacaoFinalizacaoFinanceiroDAO.Instance.ObtemNumeroObservacoesFinalizacao(idPedido,
                idFuncCad, dataCadIni, dataCadFim, motivo);
        }
    }
}

using System;
using System.Linq;
using Glass.Data.DAL;

namespace WebGlass.Business.Pedido.Fluxo
{
    public sealed class MotivoFinalizacaoFinanceiro : BaseFluxo<MotivoFinalizacaoFinanceiro>
    {
        private MotivoFinalizacaoFinanceiro() { }

        public Entidade.MotivoFinalizacaoFinanceiro[] ObtemObservacoesFinanceiro(uint idPedido, uint idCliente, string nomeCliente, uint idFuncCad, 
            string dataCadIni, string dataCadFim, string motivo, string sortExpression, int startRow, int pageSize)
        {
            var itens = ObservacaoFinalizacaoFinanceiroDAO.Instance.ObtemObservacoesFinalizacao(idPedido, idCliente, nomeCliente, 
                idFuncCad, dataCadIni, dataCadFim, motivo, sortExpression, startRow, pageSize).ToArray();

            return Array.ConvertAll(itens, x => new Entidade.MotivoFinalizacaoFinanceiro(x));
        }

        public int ObtemNumeroObservacoesFinanceiro(uint idPedido, uint idCliente, string nomeCliente, uint idFuncCad, string dataCadIni, 
            string dataCadFim, string motivo)
        {
            return ObservacaoFinalizacaoFinanceiroDAO.Instance.ObtemNumeroObservacoesFinalizacao(idPedido, idCliente, nomeCliente,
                idFuncCad, dataCadIni, dataCadFim, motivo);
        }
    }
}

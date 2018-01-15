using System;
using Glass.Data.DAL;
using Glass.Data.Helper;

namespace WebGlass.Business.CaixaGeral.Fluxo
{
    public sealed class Estornar : BaseFluxo<Estornar>
    {
        private Estornar() { }

        public void EstornarRetirada(uint idCxGeral, string obs)
        {
            var mov = CaixaGeralDAO.Instance.GetMovimentacao(idCxGeral);

            if (mov == null)
                throw new Exception("Essa movimentação não existe, ou não foi feita hoje.");

            if (!mov.LancManual ||
                mov.IdConta == UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.TransfDeCxDiarioDinheiro) ||
                mov.IdConta == UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.TransfDeCxDiarioCheque))
                throw new Exception("Essa movimentação foi gerada pelo sistema. Só é possível cancelar movimentações manuais.");

            // Realiza cancelamento
            CaixaGeralDAO.Instance.CancelaMovimentacao(mov, obs);

            if (mov.IdCheque > 0)
                ChequesDAO.Instance.UpdateSituacao(null, mov.IdCheque.Value, Glass.Data.Model.Cheques.SituacaoCheque.EmAberto);
        }
    }
}

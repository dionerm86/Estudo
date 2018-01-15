using System;
using Glass.Data.DAL;
using Glass.Data.Helper;

namespace WebGlass.Business.CaixaGeral.Fluxo
{
    public sealed class BuscarEValidar : BaseFluxo<BuscarEValidar>
    {
        private BuscarEValidar() { }

        public string Buscar(uint idCxGeral)
        {
            var mov = CaixaGeralDAO.Instance.GetMovimentacao(idCxGeral);

            if (mov == null)
                throw new Exception("Essa movimentação não existe, ou não foi feita hoje.");

            if (!mov.LancManual ||
                mov.IdConta == UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.TransfDeCxDiarioDinheiro) ||
                mov.IdConta == UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.TransfDeCxDiarioCheque))
                throw new Exception("Essa movimentação foi gerada pelo sistema. Só é possível cancelar movimentações manuais.");

            return mov.DescrPlanoConta + " Valor: " + mov.ValorMov.ToString("C") + " Forma Saída: " +
                (mov.FormaSaida == 1 ? "Dinheiro" : "Cheque");
        }
    }
}

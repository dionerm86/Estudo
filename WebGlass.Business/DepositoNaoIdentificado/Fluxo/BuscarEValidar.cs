using System;

namespace WebGlass.Business.DepositoNaoIdentificado.Fluxo
{
    public sealed class BuscarEValidar : BaseFluxo<BuscarEValidar>
    {
        private BuscarEValidar() { }

        public void ValidaCancelamento(uint idDepositoNaoIdentificado, string motivo)
        {
            throw new NotImplementedException();
        }
    }
}

namespace WebGlass.Business.DepositoNaoIdentificado.Fluxo
{
    public sealed class DepositoNaoIdentificado : BaseFluxo<DepositoNaoIdentificado>
    {
        private DepositoNaoIdentificado() { }

        /// <summary>
        /// Cancela um depoisto não identificado
        /// </summary>
        /// <param name="idDepositoNaoIdentificado"></param>
        /// <param name="motivo"></param>
        public void Cancelar(uint idDepositoNaoIdentificado, string motivo)
        {
            //Cancela o depósito
            Glass.Data.DAL.DepositoNaoIdentificadoDAO.Instance.Cancelar(idDepositoNaoIdentificado, motivo);
        }
    }
}

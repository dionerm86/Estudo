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

            // Realiza cancelamento
            CaixaGeralDAO.Instance.CancelaMovimentacao(mov, obs);
        }
    }
}

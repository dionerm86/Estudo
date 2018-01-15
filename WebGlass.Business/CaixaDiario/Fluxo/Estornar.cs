using Glass.Data.DAL;

namespace WebGlass.Business.CaixaDiario.Fluxo
{
    public sealed class Estornar : BaseFluxo<Estornar>
    {
        private Estornar() { }

        public void EstornarCreditoRetirada(int idCaixaDiario, string obs)
        {
            CaixaDiarioDAO.Instance.EstornarCreditoOuRetirada(idCaixaDiario, obs);
        }
    }
}
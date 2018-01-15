using Glass.Data.DAL;

namespace WebGlass.Business.CaixaDiario.Fluxo
{
    public sealed class BuscarEValidar : BaseFluxo<BuscarEValidar>
    {
        private BuscarEValidar() { }

        public string BuscarCreditoRetirada(int idCaixaDiario)
        {
            Glass.Data.Model.CaixaDiario movimentacao;

            CaixaDiarioDAO.Instance.VerificarEstornoCreditoOuRetiradaJaEfetuadoComTransacao(idCaixaDiario, out movimentacao);
            CaixaDiarioDAO.Instance.ValidarEstornoCreditoOuRetirada(idCaixaDiario, movimentacao.FormaSaida.GetValueOrDefault());

            return
                string.Format("{0} Valor: {1} Forma Saída: {2}",
                    movimentacao.DescrPlanoConta, movimentacao.Valor.ToString("C"),
                    movimentacao.FormaSaida == 1 ? "Dinheiro" : "Cheque");
        }
    }
}
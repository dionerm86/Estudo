using Glass.Configuracoes;
using Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo;
using Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo.Acrescimo;
using Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo.Comissao;
using Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo.Desconto;
using Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo.Enum;
using Glass.Pool;

namespace Glass.Data.Helper.Calculos.Estrategia
{
    public class DescontoAcrescimoStrategyFactory : PoolableObject<DescontoAcrescimoStrategyFactory>
    {
        private DescontoAcrescimoStrategyFactory() { }

        public IDescontoAcrescimoStrategy RecuperaEstrategia(TipoCalculo tipo, TipoAplicacao aplicacao)
        {
            IDescontoAcrescimoStrategy estrategia = null;

            switch (tipo)
            {
                case TipoCalculo.Acrescimo:
                    estrategia = RecuperaEstrategiaAcrescimo(aplicacao);
                    break;

                case TipoCalculo.Desconto:
                    estrategia = RecuperaEstrategiaDesconto(aplicacao);
                    break;

                case TipoCalculo.Comissao:
                    estrategia = RecuperaEstrategiaComissao(aplicacao);
                    break;
            }

            return estrategia ?? new DescontoAcrescimoSemAlteracaoStrategy();
        }

        private IDescontoAcrescimoStrategy RecuperaEstrategiaAcrescimo(TipoAplicacao aplicacao)
        {
            switch (aplicacao)
            {
                case TipoAplicacao.Geral:
                    return AcrescimoGeralStrategy.Instance;

                case TipoAplicacao.Ambiente:
                    return AcrescimoAmbienteStrategy.Instance;
            }

            return null;
        }

        private IDescontoAcrescimoStrategy RecuperaEstrategiaDesconto(TipoAplicacao aplicacao)
        {
            switch (aplicacao)
            {
                case TipoAplicacao.Geral:
                    return DescontoGeralStrategy.Instance;

                case TipoAplicacao.Ambiente:
                    return DescontoAmbienteStrategy.Instance;

                case TipoAplicacao.Quantidade:
                    return DescontoQuantidadeStrategy.Instance;
            }

            return null;
        }

        private IDescontoAcrescimoStrategy RecuperaEstrategiaComissao(TipoAplicacao aplicacao)
        {
            if (PedidoConfig.Comissao.ComissaoPedido)
            {
                switch (aplicacao)
                {
                    case TipoAplicacao.Geral:
                        return ComissaoGeralStrategy.Instance;
                }
            }

            return null;
        }
    }
}

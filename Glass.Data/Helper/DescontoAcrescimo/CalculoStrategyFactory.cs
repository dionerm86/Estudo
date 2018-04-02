using Glass.Configuracoes;
using Glass.Data.Helper.DescontoAcrescimo.Estrategia;
using Glass.Data.Helper.DescontoAcrescimo.Estrategia.Acrescimo;
using Glass.Data.Helper.DescontoAcrescimo.Estrategia.Comissao;
using Glass.Data.Helper.DescontoAcrescimo.Estrategia.Desconto;
using Glass.Data.Helper.DescontoAcrescimo.Estrategia.Enum;
using Glass.Pool;

namespace Glass.Data.Helper.DescontoAcrescimo
{
    public class CalculoStrategyFactory : PoolableObject<CalculoStrategyFactory>
    {
        private CalculoStrategyFactory() { }

        public ICalculoStrategy RecuperaEstrategia(TipoCalculo tipo, TipoAplicacao aplicacao)
        {
            ICalculoStrategy estrategia = null;

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

            return estrategia ?? new SemAlteracaoStrategy();
        }

        private ICalculoStrategy RecuperaEstrategiaAcrescimo(TipoAplicacao aplicacao)
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

        private ICalculoStrategy RecuperaEstrategiaDesconto(TipoAplicacao aplicacao)
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

        private ICalculoStrategy RecuperaEstrategiaComissao(TipoAplicacao aplicacao)
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

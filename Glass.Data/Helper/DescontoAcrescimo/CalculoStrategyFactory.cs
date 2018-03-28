using Glass.Data.Helper.DescontoAcrescimo.Estrategia;
using Glass.Data.Helper.DescontoAcrescimo.Estrategia.Acrescimo;
using Glass.Data.Helper.DescontoAcrescimo.Estrategia.Desconto;
using Glass.Data.Helper.DescontoAcrescimo.Estrategia.Enum;
using Glass.Pool;

namespace Glass.Data.Helper.DescontoAcrescimo
{
    class CalculoStrategyFactory : PoolableObject<CalculoStrategyFactory>
    {
        private CalculoStrategyFactory() { }

        public ICalculoStrategy RecuperaEstrategia(TipoDirecao direcao, TipoCalculo tipo, TipoAplicacao aplicacao)
        {
            ICalculoStrategy estrategia = null;

            switch (tipo)
            {
                case TipoCalculo.Acrescimo:
                    estrategia = RecuperaEstrategiaAcrescimo(direcao, aplicacao);
                    break;

                case TipoCalculo.Desconto:
                    estrategia = RecuperaEstrategiaDesconto(direcao, aplicacao);
                    break;
            }

            return estrategia ?? new SemAlteracaoStrategy();
        }

        private ICalculoStrategy RecuperaEstrategiaAcrescimo(TipoDirecao direcao, TipoAplicacao aplicacao)
        {
            switch (direcao)
            {
                case TipoDirecao.Aplicar:
                    return RecuperaEstrategiaAplicarAcrescimo(aplicacao);

                case TipoDirecao.Remover:
                    return RecuperaEstrategiaRemoverAcrescimo(aplicacao);
            }

            return null;
        }

        private ICalculoStrategy RecuperaEstrategiaDesconto(TipoDirecao direcao, TipoAplicacao aplicacao)
        {
            switch (direcao)
            {
                case TipoDirecao.Aplicar:
                    return RecuperaEstrategiaAplicarDesconto(aplicacao);

                case TipoDirecao.Remover:
                    return RecuperaEstrategiaRemoverDesconto(aplicacao);
            }

            return null;
        }

        private ICalculoStrategy RecuperaEstrategiaAplicarAcrescimo(TipoAplicacao aplicacao)
        {
            switch (aplicacao)
            {
                case TipoAplicacao.Geral:
                    return new AplicarAcrescimoGeralStrategy();

                case TipoAplicacao.Ambiente:
                    return new AplicarAcrescimoAmbienteStrategy();
            }

            return null;
        }

        private ICalculoStrategy RecuperaEstrategiaRemoverAcrescimo(TipoAplicacao aplicacao)
        {
            switch (aplicacao)
            {
                case TipoAplicacao.Geral:
                    return new RemoverAcrescimoGeralStrategy();

                case TipoAplicacao.Ambiente:
                    return new RemoverAcrescimoAmbienteStrategy();
            }

            return null;
        }

        private ICalculoStrategy RecuperaEstrategiaAplicarDesconto(TipoAplicacao aplicacao)
        {
            switch (aplicacao)
            {
                case TipoAplicacao.Geral:
                    return new AplicarDescontoGeralStrategy();

                case TipoAplicacao.Ambiente:
                    return new AplicarDescontoAmbienteStrategy();
            }

            return null;
        }

        private ICalculoStrategy RecuperaEstrategiaRemoverDesconto(TipoAplicacao aplicacao)
        {
            switch (aplicacao)
            {
                case TipoAplicacao.Geral:
                    return new RemoverDescontoGeralStrategy();

                case TipoAplicacao.Ambiente:
                    return new RemoverDescontoAmbienteStrategy();
            }

            return null;
        }
    }
}

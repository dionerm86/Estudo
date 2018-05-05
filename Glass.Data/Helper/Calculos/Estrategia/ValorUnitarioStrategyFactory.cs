using Glass.Configuracoes;
using Glass.Data.DAL;
using Glass.Data.Helper.Calculos.Estrategia.ValorUnitario;
using Glass.Data.Helper.Calculos.Estrategia.ValorUnitario.M2;
using Glass.Data.Helper.Calculos.Estrategia.ValorUnitario.ML;
using Glass.Data.Helper.Calculos.Estrategia.ValorUnitario.MLAL;
using Glass.Data.Helper.Calculos.Estrategia.ValorUnitario.Perimetro;
using Glass.Data.Helper.Calculos.Estrategia.ValorUnitario.Qtd;
using Glass.Data.Model;
using Glass.Pool;

namespace Glass.Data.Helper.Calculos.Estrategia
{
    public class ValorUnitarioStrategyFactory : Singleton<ValorUnitarioStrategyFactory>
    {
        private ValorUnitarioStrategyFactory() { }

        public IValorUnitarioStrategy RecuperaEstrategia(IProdutoCalculo produto, bool nf, bool compra)
        {
            IValorUnitarioStrategy estrategia = null;

            bool tipoCalculoFiscal = nf || (compra && CompraConfig.UsarTipoCalculoNfParaCompra);
            var tipoCalculo = (TipoCalculoGrupoProd)GrupoProdDAO.Instance.TipoCalculo(
                null,
                (int)produto.IdProduto,
                tipoCalculoFiscal
            );

            switch (tipoCalculo)
            {
                case TipoCalculoGrupoProd.M2:
                    estrategia = M2Strategy.Instance;
                    break;

                case TipoCalculoGrupoProd.M2Direto:
                    estrategia = M2DiretoStrategy.Instance;
                    break;

                case TipoCalculoGrupoProd.Perimetro:
                    estrategia = PerimetroStrategy.Instance;
                    break;

                case TipoCalculoGrupoProd.ML:
                    estrategia = MLStrategy.Instance;
                    break;

                case TipoCalculoGrupoProd.MLAL0:
                    estrategia = MLAL0Strategy.Instance;
                    break;

                case TipoCalculoGrupoProd.MLAL05:
                    estrategia = MLAL05Strategy.Instance;
                    break;

                case TipoCalculoGrupoProd.MLAL1:
                    estrategia = MLAL1Strategy.Instance;
                    break;

                case TipoCalculoGrupoProd.MLAL6:
                    estrategia = MLAL6Strategy.Instance;
                    break;

                case TipoCalculoGrupoProd.QtdDecimal:
                    estrategia = QtdDecimalStrategy.Instance;
                    break;
            }

            return estrategia ?? GenericoStrategy.Instance;
        }
    }
}

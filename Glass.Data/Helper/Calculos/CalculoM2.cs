using GDA;
using Glass.Configuracoes;
using Glass.Data.Model;
using System;

namespace Glass.Data.Helper.Calculos
{
    sealed class CalculoM2 : BaseCalculo<CalculoM2>
    {
        private CalculoM2() { }

        /// <summary>
        /// Cálculo de arredondamento de m².
        /// </summary>
        public float Calcular(GDASession sessao, IContainerCalculo container, IProdutoCalculo produto, bool calcularMultiploDe5)
        {
            AtualizaDadosProdutosCalculo(produto, sessao, container);
            return Calcular(produto, calcularMultiploDe5, (int)produto.Altura, produto.Largura, produto.Qtde);
        }

        public float CalcularM2Calculo(GDASession sessao, IContainerCalculo container, IProdutoCalculo produto, bool usarChapa,
            bool calcularMultiploDe5, int numeroBeneficiamentos, int qtdeAmbiente = 1, int? larguraUsar = null)
        {
            AtualizaDadosProdutosCalculo(produto, sessao, container);

            bool possuiChapaVidro = usarChapa
                && produto.IdProduto > 0
                && produto.DadosProduto.DadosChapaVidro.ProdutoPossuiChapaVidro();

            int altura = (int)produto.Altura;
            int largura = larguraUsar ?? produto.Largura;

            AjustarDadosChapaVidro(produto, ref possuiChapaVidro, ref altura, ref largura);

            var quantidade = produto.Qtde * qtdeAmbiente;

            float m2 = Calcular(produto, calcularMultiploDe5, altura, largura, quantidade);
            m2 = Math.Max(m2, CalcularAreaMinimaProduto(produto, numeroBeneficiamentos, quantidade));

            return AplicarM2MinimoChapaVidro(produto, possuiChapaVidro, m2);
        }

        private float Calcular(IProdutoCalculo produto, bool calcularMultiploDe5, int altura, int largura, float qtde)
        {
            int adicionarValorRedondo = AdicionarValorRedondo(produto);
            altura += adicionarValorRedondo;

            int larguraRedondo = produto.Redondo
                ? altura
                : 1000;

            // Se a largura estiver zerada, deve considerar a altura no cálculo e não 1000 como estava, 
            // para não calcular errado, chamado 7564
            largura = adicionarValorRedondo + (
                largura == 0
                    ? larguraRedondo
                    : largura
            );

            altura = ArredondarAlturaBox(produto, altura);

            if (!calcularMultiploDe5)
            {
                // Se não for para recalcular utilizando múltiplo de 5, deve retornar mais de duas casas decimais
                return largura * altura / 1000000f * qtde;
            }

            var calculo = CalcularMultiploDe5(produto, altura, largura, qtde);
            var m2 = AjustaCalculoM2(calculo);

            // Alteração feita para vidros com m2 menor que 0.01 ficar com esta medida, para que o valor
            // não fique zerado, quando fizer a alteração para calcular vidro com x casas decimais,
            // utilizar esta opção somente se estiver sendo utilizada 2 casas decimais
            return Math.Max((float)m2, 0.01f);
        }

        private int AdicionarValorRedondo(IProdutoCalculo produto)
        {
            if (produto.Redondo)
            {
                return produto.Espessura < 12
                    ? Geral.AdicionalVidroRedondoAte12mm
                    : Geral.AdicionalVidroRedondoAcima12mm;
            }

            return 0;
        }

        private void AjustarDadosChapaVidro(IProdutoCalculo produto, ref bool possuiChapaVidro, ref int altura, ref int largura)
        {
            if (!possuiChapaVidro)
                return;
            
            int alturaReal = altura;

            int alturaMinimaChapa = produto.DadosProduto.DadosChapaVidro.AlturaMinimaChapaVidro();
            int alturaChapa = produto.DadosProduto.DadosChapaVidro.AlturaChapaVidro();

            if (altura > alturaMinimaChapa && alturaMinimaChapa > 0)
            {
                if (altura < alturaChapa)
                {
                    altura = alturaChapa;
                }
                else
                {
                    possuiChapaVidro = false;
                }
            }

            int larguraMinimaChapa = produto.DadosProduto.DadosChapaVidro.LarguraMinimaChapaVidro();
            int larguraChapa = produto.DadosProduto.DadosChapaVidro.LarguraChapaVidro();

            if (possuiChapaVidro && largura > larguraMinimaChapa && larguraMinimaChapa > 0)
            {
                if (largura < larguraChapa)
                {
                    largura = larguraChapa;
                }
                else
                {
                    altura = alturaReal;
                    possuiChapaVidro = false;
                }
            }
        }

        private float AplicarM2MinimoChapaVidro(IProdutoCalculo produto, bool possuiChapaVidro, float m2)
        {
            if (possuiChapaVidro)
            {
                float perc = produto.DadosProduto.DadosChapaVidro.PercentualAcrescimoM2ChapaVidro(m2);
                return (float)Math.Round(m2 * (1 + perc), Geral.NumeroCasasDecimaisTotM);
            }

            return m2;
        }

        private int ArredondarAlturaBox(IProdutoCalculo produto, int altura)
        {
            var alturaBox = altura >= 1840 && altura < 1855;
            var arredondarAlturaBoxPadrao = Geral.ArredondarBoxPara1900SubgrupoBoxPadrao &&
                produto.DadosProduto.DadosGrupoSubgrupo.DescricaoSubgrupo() == "BOX PADRÃO";

            // A União Box pediu para não arrendondar os boxes para 1900
            if (Geral.ArredondarBoxPara1900
                && produto.IdProduto > 0
                && alturaBox
                && (produto.DadosProduto.DadosGrupoSubgrupo.ProdutoDeProducao() || arredondarAlturaBoxPadrao))
            {
                return 1900;
            }

            return altura;
        }

        private decimal CalcularMultiploDe5(IProdutoCalculo produto, int altura, int largura, float quantidade)
        {
            decimal calculoLargura = Math.Round(largura / 50m + 0.49m) * 50;
            decimal calculoAltura = Math.Round(altura / 50m + 0.49m) * 50;

            // Arredonda vidro Aramado com múltiplo de 25 ou 10
            if (produto.IdProduto > 0 && produto.DadosProduto.Descricao().ToLower().Contains("aramado"))
            {
                var multiploAramado = Geral.MultiploParaCalculoDeAramado;
                calculoLargura = Math.Round((decimal)largura / multiploAramado + 0.499m) * multiploAramado;
                calculoAltura = Math.Round((decimal)altura / multiploAramado + 0.499m) * multiploAramado;
            }
            else if (PedidoConfig.CalcularMultiplo10)
            {
                calculoLargura = Math.Round(largura / 100m + 0.499m) * 100;
                calculoAltura = Math.Round(altura / 100m + 0.499m) * 100;
            }

            return calculoLargura * calculoAltura / 1000000m * (decimal)quantidade;
        }

        private decimal AjustaCalculoM2(decimal calculo)
        {
            var numeroCasasDecimaisTotM = Geral.NumeroCasasDecimaisTotM;

            var ajuste = 1 / (decimal)Math.Pow(10, numeroCasasDecimaisTotM + 1);
            var m2Ajustado = Math.Round(calculo + ajuste, numeroCasasDecimaisTotM);

            m2Ajustado -= m2Ajustado % (ajuste * 2) == 0
                ? 0
                : ajuste;

            return Math.Round(m2Ajustado, numeroCasasDecimaisTotM);
        }

        private float CalcularAreaMinimaProduto(IProdutoCalculo produto, int numeroBeneficiamentos, float qtde)
        {
            if (produto.IdProduto > 0)
            {
                float m2Minimo = produto.DadosProduto.CalcularAreaMinima(numeroBeneficiamentos)
                    ? produto.DadosProduto.AreaMinima()
                    : 0;

                return m2Minimo * qtde;
            }

            return 0;
        }
    }
}

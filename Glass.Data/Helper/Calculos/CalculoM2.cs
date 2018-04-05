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
        public float Calcular(GDASession sessao, IProdutoCalculo produto, IContainerCalculo container, bool calcularMultiploDe5)
        {
            return Calcular(
                sessao,
                produto,
                container,
                calcularMultiploDe5,
                (int)produto.Altura,
                produto.Largura,
                produto.Qtde
            );
        }

        public float CalcularM2Calculo(GDASession sessao, IProdutoCalculo produto, IContainerCalculo container, bool usarChapa,
            bool calcMult5, int numeroBeneficiamentos, int qtdeAmbiente = 1, int? larguraUsar = null)
        {
            bool possuiChapaVidro = usarChapa
                && produto.IdProduto > 0
                && container.DadosChapaVidro.ProdutoPossuiChapaVidro(produto);

            int altura = (int)produto.Altura;
            int largura = larguraUsar ?? produto.Largura;

            AjustarDadosChapaVidro(produto, container, ref possuiChapaVidro, ref altura, ref largura);

            var quantidade = produto.Qtde * qtdeAmbiente;

            float m2 = Calcular(sessao, produto, container, calcMult5, altura, largura, quantidade);
            m2 = Math.Max(m2, CalcularAreaMinimaProduto(sessao, produto, container, numeroBeneficiamentos, quantidade));

            return AplicarM2MinimoChapaVidro(produto, container, possuiChapaVidro, m2);
        }

        private float Calcular(GDASession sessao, IProdutoCalculo produto, IContainerCalculo container, bool calcularMultiploDe5,
            int altura, int largura, float qtde)
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

            altura = ArredondarAlturaBox(sessao, produto, container, altura);

            if (!calcularMultiploDe5)
            {
                // Se não for para recalcular utilizando múltiplo de 5, deve retornar mais de duas casas decimais
                return largura * altura / 1000000f * qtde;
            }

            var calculo = CalcularMultiploDe5(sessao, produto, container, altura, largura, qtde);
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

        private void AjustarDadosChapaVidro(IProdutoCalculo produto, IContainerCalculo container,
            ref bool possuiChapaVidro, ref int altura, ref int largura)
        {
            if (!possuiChapaVidro)
                return;
            
            int alturaReal = altura;

            int alturaMinimaChapa = container.DadosChapaVidro.AlturaMinimaChapaVidro(produto);
            int alturaChapa = container.DadosChapaVidro.AlturaChapaVidro(produto);

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

            int larguraMinimaChapa = container.DadosChapaVidro.LarguraMinimaChapaVidro(produto);
            int larguraChapa = container.DadosChapaVidro.LarguraChapaVidro(produto);

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

        private float AplicarM2MinimoChapaVidro(IProdutoCalculo produto, IContainerCalculo container,
            bool possuiChapaVidro, float m2)
        {
            if (possuiChapaVidro)
            {
                float perc = container.DadosChapaVidro.PercentualAcrescimoM2ChapaVidro(produto, m2);
                return (float)Math.Round(m2 * (1 + perc), Geral.NumeroCasasDecimaisTotM);
            }

            return m2;
        }

        private int ArredondarAlturaBox(GDASession sessao, IProdutoCalculo produto, IContainerCalculo container, int altura)
        {
            var alturaBox = altura >= 1840 && altura < 1855;
            var arredondarAlturaBoxPadrao = Geral.ArredondarBoxPara1900SubgrupoBoxPadrao &&
                container.DadosProduto.DescricaoSubgrupo(sessao, produto) == "BOX PADRÃO";

            // A União Box pediu para não arrendondar os boxes para 1900
            if (Geral.ArredondarBoxPara1900
                && produto.IdProduto > 0
                && alturaBox
                && (container.DadosProduto.ProdutoDeProducao(sessao, produto) || arredondarAlturaBoxPadrao))
            {
                return 1900;
            }

            return altura;
        }

        private decimal CalcularMultiploDe5(GDASession sessao, IProdutoCalculo produto, IContainerCalculo container,
            int altura, int largura, float quantidade)
        {
            decimal calculoLargura = Math.Round(largura / 50m + 0.49m) * 50;
            decimal calculoAltura = Math.Round(altura / 50m + 0.49m) * 50;

            // Arredonda vidro Aramado com múltiplo de 25 ou 10
            if (produto.IdProduto > 0 && container.DadosProduto.Descricao(sessao, produto).ToLower().Contains("aramado"))
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

        private float CalcularAreaMinimaProduto(GDASession sessao, IProdutoCalculo produto, IContainerCalculo container,
            int numeroBeneficiamentos, float qtde)
        {
            if (produto.IdProduto > 0)
            {
                float m2Minimo = container.DadosProduto.CalcularAreaMinima(sessao, produto, numeroBeneficiamentos)
                    ? container.DadosProduto.AreaMinima(sessao, produto)
                    : 0;

                return m2Minimo * qtde;
            }

            return 0;
        }
    }
}

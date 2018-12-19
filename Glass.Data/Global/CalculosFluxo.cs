using GDA;
using Glass.Data.DAL;
using Glass.Data.Helper.Calculos;
using Glass.Data.Model;
using Glass.Data.Model.Calculos;
using System;

namespace Glass.Global
{
    public static class CalculosFluxo
    {
        #region Cálculo de arredondamento de M2

        /// <summary>
        /// Cálculo de arredondamento de m2 utilizando 2 casas decimais.
        /// </summary>
        /// <param name="sessao">sessao.</param>
        /// <param name="largura">largura.</param>
        /// <param name="altura">altura.</param>
        /// <param name="qtd">qtd.</param>
        /// <param name="idProd">idProd.</param>
        /// <param name="redondo">redondo.</param>
        /// <param name="espessura">espessura.</param>
        /// <param name="calcMult5">calcMult5.</param>
        /// <returns>Retorna o M2 calculado com arredondamento de 2 casas decimais.</returns>
        public static float ArredondaM2(
            GDASession sessao,
            int largura,
            int altura,
            float qtd,
            int idProd,
            bool redondo,
            float espessura,
            bool calcMult5)
        {
            var container = new ContainerCalculoDTO();

            var produto = new ProdutoCalculoDTO()
            {
                IdProduto = (uint)idProd,
                Largura = largura,
                Altura = altura,
                Qtde = qtd,
                Redondo = redondo,
                Espessura = espessura,
            };

            return CalculoM2.Instance.Calcular(sessao, container, produto, calcMult5);
        }

        /// <summary>
        /// Cálculo de arredondamento de m2 utilizando 3 casas decimais
        /// </summary>
        /// <param name="largura"></param>
        /// <param name="altura"></param>
        /// <param name="qtd"></param>
        /// <returns></returns>
        public static float ArredondaM2Compra(int largura, int altura, int qtd)
        {
            double res1, res2;
            double result;

            res1 = Math.Round(((float)largura / 50) + 0.49) * 50;
            res2 = Math.Round(((float)altura / 50) + 0.49) * 50;

            result = (float)((res1 * res2) / 1000000) * qtd;

            return Single.Parse(Math.Round(result + 0.0001, 3).ToString());
        }

        /// <summary>
        /// Cálculo de arredondamento de largura/altura utilizando 2 casas decimais
        /// </summary>
        /// <param name="largura"></param>
        /// <param name="qtd"></param>
        /// <returns></returns>
        public static int ArredondaLargAlt(int largAlt)
        {
            return (int)Math.Round(((float)largAlt / 50) + 0.49, 2) * 50;
        }

        /// <summary>
        /// Cálculo de M2.
        /// </summary>
        /// <param name="sessao">sessao.</param>
        /// <param name="idCliente">idCliente.</param>
        /// <param name="altura">altura.</param>
        /// <param name="largura">largura.</param>
        /// <param name="qtde">qtde.</param>
        /// <param name="idProduto">idProduto.</param>
        /// <param name="redondo">redondo.</param>
        /// <param name="numBenef">numBenef.</param>
        /// <param name="areaMinima">areaMinima.</param>
        /// <param name="usarChapa">usarChapa.</param>
        /// <param name="espessura">espessura.</param>
        /// <param name="calcMult5">calcMult5.</param>
        /// <returns>Retorna o M2 calculado.</returns>
        public static float CalcM2Calculo(
            GDASession sessao,
            uint idCliente,
            int altura,
            int largura,
            float qtde,
            int idProduto,
            bool redondo,
            int numBenef,
            float areaMinima,
            bool usarChapa,
            float espessura,
            bool calcMult5)
        {
            var container = new ContainerCalculoDTO()
            {
                Cliente = new ClienteDTO(() => idCliente),
            };

            var produto = new ProdutoCalculoDTO()
            {
                IdProduto = (uint)idProduto,
                Altura = altura,
                Largura = largura,
                Qtde = qtde,
                Redondo = redondo,
                Espessura = espessura,
            };

            return CalculoM2.Instance.CalcularM2Calculo(sessao, container, produto, usarChapa, calcMult5, numBenef);
        }

        #endregion

        #region Descrição do tipo de cálculo

        public const string NOME_MLAL = "ML Barra 6m";

        /// <summary>
        /// Retorna a descrição do tipo do cálculo.
        /// </summary>
        /// <param name="idGrupoProd">O id do grupo do produto.</param>
        /// <param name="idSubgrupoProd">O id do subgrupo do produto.</param>
        /// <param name="nf">Verificar o tipo de cálculo para NF?</param>
        /// <param name="unidade">O retorno será da unidade do tipo de cálculo?</param>
        /// <returns></returns>
        public static string GetDescrTipoCalculo(int idGrupoProd, int? idSubgrupoProd, bool nf, bool unidade)
        {
            int tipo = Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(null, idGrupoProd, idSubgrupoProd, nf);
            return GetDescrTipoCalculo(tipo, unidade);
        }

        /// <summary>
        /// Retorna a descrição do tipo do cálculo.
        /// </summary>
        /// <param name="tipo">O tipo do cálculo.</param>
        /// <param name="unidade">O retorno será da unidade do tipo de cálculo?</param>
        /// <returns></returns>
        public static string GetDescrTipoCalculo(int? tipo, bool unidade)
        {
            if (tipo != null)
                return Colosoft.Translator.Translate((TipoCalculoGrupoProd)tipo.Value, unidade ? "unidade" : null).Format();

            return string.Empty;
        }

        /// <summary>
        /// Retorna a descrição do tipo do cálculo.
        /// </summary>
        /// <param name="tipo">O tipo do cálculo.</param>
        /// <param name="unidade">O retorno será da unidade do tipo de cálculo?</param>
        /// <returns></returns>
        public static string GetDescrTipoCalculo(Glass.Data.Model.TipoCalculoGrupoProd? tipo, bool unidade)
        {
            return Colosoft.Translator.Translate(tipo, unidade ? "unidade" : null).Format();
        }

        #endregion

        #region Calcula o tamanho do alumínio

        public static void CalcTamanhoAluminio(GDASession sessao, int idProd, ref float altura, int tipoCalc, int arredondarAluminio, decimal valorUnit,
            float qtde, ref decimal total, ref decimal custo)
        {
            var idGrupoProd = ProdutoDAO.Instance.ObtemIdGrupoProd(sessao, idProd);
            decimal custoCompraProd = ProdutoDAO.Instance.ObtemCustoCompra(sessao, idProd);

            var alturaCalc = altura;

            if (!Glass.Data.DAL.GrupoProdDAO.Instance.IsAluminio(idGrupoProd))
                arredondarAluminio = 0;

            // Se for para arredondar a altura do alumínio
            if (arredondarAluminio > 0)
                ArredondaAluminio(tipoCalc, altura, ref alturaCalc);

            // Se for para arredondar o alumínio e salvar este arredondamento
            if (arredondarAluminio == 1)
                altura = alturaCalc;

            if (tipoCalc != (int)Glass.Data.Model.TipoCalculoGrupoProd.ML)
            {
                // Faz o cálculo normalmente do múltiplo de 6 metros
                if (alturaCalc == 6)
                    total = valorUnit * (decimal)qtde;
                else if (alturaCalc < 6) // Calcula o restante da barra adicionando
                    total = (valorUnit * (decimal)((alturaCalc % 6) / 6)) * (decimal)qtde;
                else
                    total = (valorUnit / 6) * (decimal)(alturaCalc * qtde);

                custo = (custoCompraProd / 6) * (decimal)(alturaCalc * qtde);
            }
            else
            {
                total = valorUnit * (decimal)(alturaCalc * qtde);
                custo = custoCompraProd * (decimal)(alturaCalc * qtde);
            }
        }

        /// <summary>
        /// Arredonda o comprimento do produto que utilize cálculo por metro linear
        /// </summary>
        /// <param name="tipoCalc"></param>
        /// <param name="altura"></param>
        /// <param name="alturaCalc"></param>
        public static void ArredondaAluminio(int tipoCalc, float altura, ref float alturaCalc)
        {
            if (tipoCalc != (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL05 && tipoCalc != (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL1 &&
                tipoCalc != (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL6 && tipoCalc != (int)Glass.Data.Model.TipoCalculoGrupoProd.ML &&
                tipoCalc != (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL0)
                return;

            Single decimosAltura = altura - (int)altura;

            float valorArredondar = tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL05 ? 0.5f :
                tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL1 ? 1f : 0f;

            if (tipoCalc < (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL6)
            {
                if (decimosAltura > 0 && decimosAltura < valorArredondar)
                    alturaCalc = ((int)altura) + (Single)valorArredondar;
                else if (decimosAltura > valorArredondar)
                    alturaCalc = ((int)altura) + (valorArredondar * 2);
            }
            else if (tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL6 && altura < 6)
                alturaCalc = 6;
        }

        #endregion

        #region Calcula totais de um item produto rápido

        /// <summary>
        /// Calcula os totais de acordo com o tipo de cálculo.
        /// </summary>
        /// <param name="tipoCalc"></param>
        /// <param name="altura"></param>
        /// <param name="largura"></param>
        /// <param name="qtde"></param>
        /// <param name="totM2"></param>
        /// <param name="valorUnit"></param>
        /// <returns></returns>
        public static decimal CalcTotaisItemProdFast(int tipoCalc, float altura, int largura, float qtde, float totM2, decimal valorUnit)
        {
            return CalcTotaisItemProdFast(null, tipoCalc, altura, largura, qtde, totM2, valorUnit);
        }

        /// <summary>
        /// Calcula os totais de acordo com o tipo de cálculo.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="tipoCalc"></param>
        /// <param name="altura"></param>
        /// <param name="largura"></param>
        /// <param name="qtde"></param>
        /// <param name="totM2"></param>
        /// <param name="valorUnit"></param>
        /// <returns></returns>
        public static decimal CalcTotaisItemProdFast(GDASession sessao, int tipoCalc, float altura, int largura, float qtde, float totM2, decimal valorUnit)
        {
            return CalcTotaisItemProdFast(sessao, tipoCalc, altura, largura, qtde, totM2, valorUnit, 2, 2);
        }

        /// <summary>
        /// Calcula os totais de acordo com o tipo de cálculo.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="tipoCalc"></param>
        /// <param name="altura"></param>
        /// <param name="largura"></param>
        /// <param name="qtde"></param>
        /// <param name="totM2"></param>
        /// <param name="valorUnit"></param>
        /// <param name="alturaBenef"></param>
        /// <param name="larguraBenef"></param>
        /// <returns></returns>
        public static decimal CalcTotaisItemProdFast(GDASession sessao, int tipoCalc, float altura, int largura, float qtde, float totM2, decimal valorUnit,
            int alturaBenef, int larguraBenef)
        {
            decimal retorno = 0;
            switch ((Glass.Data.Model.TipoCalculoGrupoProd)tipoCalc)
            {
                case Glass.Data.Model.TipoCalculoGrupoProd.M2:
                case Glass.Data.Model.TipoCalculoGrupoProd.M2Direto:
                    retorno = valorUnit * (decimal)totM2;
                    break;

                case Glass.Data.Model.TipoCalculoGrupoProd.ML:
                    retorno = valorUnit * (decimal)(altura * qtde);
                    break;

                case Glass.Data.Model.TipoCalculoGrupoProd.MLAL0:
                case Glass.Data.Model.TipoCalculoGrupoProd.MLAL05:
                case Glass.Data.Model.TipoCalculoGrupoProd.MLAL1:
                case Glass.Data.Model.TipoCalculoGrupoProd.MLAL6:
                    decimal total = 0, custo = 0;
                    CalcTamanhoAluminio(sessao, 0, ref altura, tipoCalc, 0, valorUnit, qtde, ref total, ref custo);
                    retorno = total;
                    break;

                case Glass.Data.Model.TipoCalculoGrupoProd.Perimetro:
                    retorno = valorUnit * (decimal)((altura * alturaBenef / 1000) + (largura * larguraBenef / 1000)) * (decimal)qtde;
                    break;

                default:
                    retorno = valorUnit * (decimal)qtde;
                    break;
            }

            return Math.Round(retorno, 2);
        }

        /// <summary>
        /// Realiza o calculo do valor de custo.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="tipoCalc">Tipo de cálculo que será usado.</param>
        /// <param name="altura">Altura.</param>
        /// <param name="largura">Largura.</param>
        /// <param name="qtde">Quantidade.</param>
        /// <param name="totM2">Área total em m².</param>
        /// <param name="valorUnit">Valor unitário.</param>
        /// <param name="alturaBenef">Altura do beneficiamento.</param>
        /// <param name="larguraBenef">Largura do beneficiamento.</param>
        /// <returns></returns>
        public static decimal CalcularValorCusto(
            GDASession sessao, int tipoCalc, float altura, int largura, float qtde,
            float totM2, decimal valorUnit, int alturaBenef, int larguraBenef)
        {
            decimal retorno = 0;
            switch ((TipoCalculoGrupoProd)tipoCalc)
            {
                case TipoCalculoGrupoProd.M2:
                case TipoCalculoGrupoProd.M2Direto:
                    retorno = valorUnit / (decimal)totM2;
                    break;

                case TipoCalculoGrupoProd.ML:
                    retorno = valorUnit / (decimal)(altura * qtde);
                    break;

                case Glass.Data.Model.TipoCalculoGrupoProd.MLAL0:
                case Glass.Data.Model.TipoCalculoGrupoProd.MLAL05:
                case Glass.Data.Model.TipoCalculoGrupoProd.MLAL1:
                case Glass.Data.Model.TipoCalculoGrupoProd.MLAL6:
                    decimal total = 0, custo = 0;
                    CalcTamanhoAluminio(sessao, 0, ref altura, tipoCalc, 0, valorUnit, qtde, ref total, ref custo);
                    retorno = custo;
                    break;

                case Glass.Data.Model.TipoCalculoGrupoProd.Perimetro:
                    retorno = (valorUnit / (decimal)((altura * alturaBenef / 1000) + (largura * larguraBenef / 1000))) * (decimal)qtde;
                    break;

                default:
                    retorno = valorUnit / (decimal)qtde;
                    break;
            }

            return Math.Round(retorno, 2);
        }

        #endregion

        public static decimal CalcularTotal(GDASession sessao, int idCliente, int idProduto, double altura,
            int largura, double quantidade, int? quantidadeAmbiente, double areaM2, double areaCalculadaM2,
            decimal valorUnitario, double percentualDescontoPorQuantidade, bool calcularMultiploDe5,
            int numeroBeneficiamentos)
        {
            var container = new ContainerCalculoDTO
            {
                Cliente = new ClienteDTO(() => (uint)idCliente),
            };

            var produto = new ProdutoCalculoDTO
            {
                Altura = (float)altura,
                Largura = largura,
                Qtde = (float)quantidade,
                QtdeAmbiente = quantidadeAmbiente ?? 0,
                IdProduto = (uint)idProduto,
                TotM = (float)areaM2,
                TotM2Calc = (float)areaCalculadaM2,
                ValorUnit = valorUnitario,
                PercDescontoQtde = (float)percentualDescontoPorQuantidade,
            };

            ValorTotal.Instance.Calcular(
                sessao,
                container,
                produto,
                Data.Helper.Calculos.Estrategia.ValorTotal.Enum.ArredondarAluminio.ArredondarApenasCalculo,
                calcularMultiploDe5,
                numeroBeneficiamentos);

            return produto.Total;
        }
    }
}

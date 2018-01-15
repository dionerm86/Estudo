using System;
using Glass.Configuracoes;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Data.Model;
using GDA;

namespace Glass.Global
{
    public static class CalculosFluxo
    {
        #region Cálculo de arredondamento de M2

        /// <summary>
        /// Cálculo de arredondamento de m2 utilizando 2 casas decimais
        /// </summary>
        /// <param name="largura"></param>
        /// <param name="altura"></param>
        /// <param name="qtd"></param>
        /// <returns></returns>
        public static Single ArredondaM2(int largura, int altura, int qtd, int idProd, bool redondo)
        {
            return ArredondaM2(largura, altura, qtd, idProd, redondo, 0, true);
        }

        /// <summary>
        /// Cálculo de arredondamento de m2 utilizando 2 casas decimais
        /// </summary>
        /// <param name="largura"></param>
        /// <param name="altura"></param>
        /// <param name="qtd"></param>
        /// <returns></returns>
        public static Single ArredondaM2(int largura, int altura, int qtd, int idProd, bool redondo, float espessura, bool calcMult5)
        {
            return ArredondaM2(largura, altura, (float)qtd, idProd, redondo, espessura, calcMult5);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Cálculo de arredondamento de m2 utilizando 2 casas decimais
        /// </summary>
        /// <param name="largura"></param>
        /// <param name="altura"></param>
        /// <param name="qtd"></param>
        /// <returns></returns>
        public static Single ArredondaM2(int largura, int altura, float qtd, int idProd, bool redondo)
        {
            return ArredondaM2(null, largura, altura, qtd, idProd, redondo);
        }

        /// <summary>
        /// Cálculo de arredondamento de m2 utilizando 2 casas decimais
        /// </summary>
        /// <param name="largura"></param>        
        /// <param name="altura"></param>
        /// <param name="qtd"></param>
        /// <returns></returns>
        public static Single ArredondaM2(GDASession sessao, int largura, int altura, float qtd, int idProd, bool redondo)
        {
            return ArredondaM2(sessao, largura, altura, qtd, idProd, redondo, 0, true);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Cálculo de arredondamento de m2 utilizando 2 casas decimais
        /// </summary>
        /// <param name="largura"></param>
        /// <param name="altura"></param>
        /// <param name="qtd"></param>
        /// <returns></returns>
        public static Single ArredondaM2(int largura, int altura, float qtd, int idProd, bool redondo, float espessura, bool calcMult5)
        {
            return ArredondaM2(null, largura, altura, qtd, idProd, redondo, espessura, calcMult5);
        }

        /// <summary>
        /// Cálculo de arredondamento de m2 utilizando 2 casas decimais
        /// </summary>
        /// <param name="largura"></param>
        /// <param name="altura"></param>
        /// <param name="qtd"></param>
        /// <returns></returns>
        public static Single ArredondaM2(GDASession sessao, int largura, int altura, float qtd, int idProd, bool redondo, float espessura, bool calcMult5)
        {
            double res1, res2;
            double result;

            if (redondo)
            {
                int addValor = espessura < 12 ? Geral.AdicionalVidroRedondoAte12mm : Geral.AdicionalVidroRedondoAcima12mm;

                // Se a largura estiver zerada, deve considerar a altura no cálculo e não 1000 como estava, 
                // para não calcular errado, chamado 7564
                largura = (largura == 0 ? (redondo ? altura : 1000) : largura) + addValor;

                altura += addValor;
            }

            // A União Box pediu para não arrendondar os boxes para 1900
            if (Geral.ArredondarBoxPara1900 &&
                idProd > 0 && (altura >= 1840 && altura < 1855) &&
                (ProdutoDAO.Instance.IsProdutoProducao(sessao, idProd) ||
                (Geral.ArredondarBoxPara1900SubgrupoBoxPadrao &&
                SubgrupoProdDAO.Instance.GetDescricao(sessao, ProdutoDAO.Instance.ObtemIdSubgrupoProd(sessao, idProd).Value) == "BOX PADRÃO")))
                altura = 1900;

            if (calcMult5)
            {
                res1 = Math.Round(((float)largura / 50) + 0.49) * 50;
                res2 = Math.Round(((float)altura / 50) + 0.49) * 50;

                // Arredonda vidro Aramado com múltiplo de 25 ou 10
                if (idProd > 0 && ProdutoDAO.Instance.GetDescrProduto(sessao, idProd).ToLower().Contains("aramado"))
                {
                    var multiploAramado = Geral.MultiploParaCalculoDeAramado;
                    res1 = Math.Round(((float)largura / multiploAramado) + 0.499) * multiploAramado;
                    res2 = Math.Round(((float)altura / multiploAramado) + 0.499) * multiploAramado;
                }
                else if (PedidoConfig.CalcularMultiplo10)
                {
                    res1 = Math.Round(((float)largura / 100) + 0.499) * 100;
                    res2 = Math.Round(((float)altura / 100) + 0.499) * 100;
                }

                result = (float)((res1 * res2) / 1000000) * qtd;
            }
            else // Se não for para recalcular utilizando múltiplo de 5, deve retornar mais de duas casas decimais
                return ((largura * altura) / 1000000f) * qtd;

            var ajuste = 1 / Math.Pow(10, Geral.NumeroCasasDecimaisTotM + 1);

            var m2 = Single.Parse(Math.Round(result + ajuste, Geral.NumeroCasasDecimaisTotM).ToString());
            m2 -= (float)(m2 % (ajuste * 2) == 0 ? 0 : ajuste);

            m2 = Single.Parse(Math.Round(m2, Geral.NumeroCasasDecimaisTotM).ToString());

            // Alteração feita para vidros com m2 menor que 0.01 ficar com esta medida, para que o valor não fique zerado,
            // quando fizer a alteração para calcular vidro com x casas decimais, utilizar esta opção somente se estiver sendo utilizada
            // 2 casas decimais
            m2 = m2 < 0.01f ? 0.01f : m2;

            return m2;
        }

        /// <summary>
        /// Cálculo de arredondamento de m2 utilizando 3 casas decimais
        /// </summary>
        /// <param name="largura"></param>
        /// <param name="altura"></param>
        /// <param name="qtd"></param>
        /// <returns></returns>
        public static Single ArredondaM2Compra(int largura, int altura, int qtd)
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
        /// <param name="idCliente"></param>
        /// <param name="altura"></param>
        /// <param name="largura"></param>
        /// <param name="qtde"></param>
        /// <param name="idProduto"></param>
        /// <param name="redondo"></param>
        /// <param name="numBenef"></param>
        /// <param name="areaMinima"></param>
        /// <param name="usarChapa"></param>
        /// <param name="espessura"></param>
        /// <param name="calcMult5"></param>
        /// <returns></returns>
        public static float CalcM2Calculo(uint idCliente, int altura, int largura, float qtde, int idProduto, bool redondo, int numBenef,
            float areaMinima, bool usarChapa, float espessura, bool calcMult5)
        {
            return CalcM2Calculo(null, idCliente, altura, largura, qtde, idProduto, redondo, numBenef, areaMinima, usarChapa, espessura, calcMult5);
        }

        /// <summary>
        /// Cálculo de M2.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idCliente"></param>
        /// <param name="altura"></param>
        /// <param name="largura"></param>
        /// <param name="qtde"></param>
        /// <param name="idProduto"></param>
        /// <param name="redondo"></param>
        /// <param name="numBenef"></param>
        /// <param name="areaMinima"></param>
        /// <param name="usarChapa"></param>
        /// <param name="espessura"></param>
        /// <param name="calcMult5"></param>
        /// <returns></returns>
        public static float CalcM2Calculo(GDASession sessao, uint idCliente, int altura, int largura, float qtde, int idProduto, bool redondo, int numBenef,
            float areaMinima, bool usarChapa, float espessura, bool calcMult5)
        {
            ChapaVidro chapa = null;
            bool chapaNula = false;

            if (usarChapa && idProduto > 0)
            {
                chapa = ChapaVidroDAO.Instance.GetElement(sessao, (uint)idProduto);
                if (chapa != null)
                {
                    int alturaReal = altura;

                    if (altura > chapa.AlturaMinima && chapa.AlturaMinima > 0)
                    {
                        if (altura < chapa.Altura)
                            altura = chapa.Altura;
                        else
                        {
                            chapa = null;
                            chapaNula = true;
                        }
                    }

                    // Não tirar essa validação: chapa != null
                    if (chapa != null && largura > chapa.LarguraMinima && chapa.LarguraMinima > 0)
                    {
                        if (largura < chapa.Largura)
                            largura = chapa.Largura;
                        else
                        {
                            altura = alturaReal;
                            chapa = null;
                            chapaNula = true;
                        }
                    }
                }
            }

            float m2 = Glass.Global.CalculosFluxo.ArredondaM2(sessao, largura, altura, qtde, idProduto, redondo, espessura, calcMult5);

            if (idProduto > 0)
            {
                Single m2Minimo = ProdutoDAO.Instance.CalcularAreaMinima(sessao, idCliente, idProduto, redondo, numBenef) ? areaMinima : 0;
                if (m2 < m2Minimo * qtde)
                    m2 = m2Minimo * qtde;
            }

            // Se a chapa tiver ficado nula na função acima, carrega novamente para cobrar o percentual mínimo
            if (chapaNula && chapa == null && usarChapa && idProduto > 0)
                chapa = ChapaVidroDAO.Instance.GetElement(sessao, (uint)idProduto);

            // Aplica o percentual de acréscimo do m² da chapa de vidro
            if (chapa != null)
            {
                float perc = 0;
                if (chapa.TotM2Minimo3 > 0 && m2 >= (chapa.TotM2Minimo3 * qtde))
                    perc = chapa.PercAcrescimoTotM23 / 100;
                else if (chapa.TotM2Minimo2 > 0 && m2 >= (chapa.TotM2Minimo2 * qtde))
                    perc = chapa.PercAcrescimoTotM22 / 100;
                else if (chapa.TotM2Minimo1 > 0 && m2 >= (chapa.TotM2Minimo1 * qtde))
                    perc = chapa.PercAcrescimoTotM21 / 100;

                m2 = (float)Math.Round(m2 * (1 + perc), Geral.NumeroCasasDecimaisTotM);
            }

            return m2;
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
            int tipo = Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(idGrupoProd, idSubgrupoProd, nf);
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

        public static void CalcTamanhoAluminio(int idProd, ref float altura, int tipoCalc, int arredondarAluminio, decimal valorUnit,
            float qtde, ref decimal total, ref decimal custo)
        {
            CalcTamanhoAluminio(null, idProd, ref altura, tipoCalc, arredondarAluminio, valorUnit, qtde, ref total, ref custo);
        }

        public static void CalcTamanhoAluminio(GDASession sessao, int idProd, ref float altura, int tipoCalc, int arredondarAluminio, decimal valorUnit,
            float qtde, ref decimal total, ref decimal custo)
        {
            var idGrupoProd = ProdutoDAO.Instance.ObtemIdGrupoProd(sessao, idProd);
            decimal custoCompraProd = ProdutoDAO.Instance.ObtemCustoCompra(sessao, idProd);

            Single decimosAltura = altura - (int)altura;
            Single alturaCalc = altura;

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
        /// <param name="tipoCalc"></param>
        /// <param name="altura"></param>
        /// <param name="largura"></param>
        /// <param name="qtde"></param>
        /// <param name="totM2"></param>
        /// <param name="valorUnit"></param>
        /// <param name="alturaBenef"></param>
        /// <param name="larguraBenef"></param>
        /// <returns></returns>
        public static decimal CalcTotaisItemProdFast(int tipoCalc, float altura, int largura, float qtde, float totM2, decimal valorUnit,
            int alturaBenef, int larguraBenef)
        {
            return CalcTotaisItemProdFast(null, tipoCalc, altura, largura, qtde, totM2, valorUnit, alturaBenef, larguraBenef);
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

        #endregion

        #region Calcula valor unitário a partir de um total

        public static void CalcValorUnitItemProd(uint idCliente, int idProd, int largura, float qtde, float qtdeAmbiente, decimal total, float espessura, bool redondo,
            int arredondarAluminio, bool compra, bool calcMult5, Single altura, Single totM2, ref decimal valorUnitario, int numeroBenef, int alturaBenef, int larguraBenef)
        {
            CalcValorUnitItemProd(null, idCliente, idProd, largura, qtde, qtdeAmbiente, total, espessura, redondo, arredondarAluminio, compra, calcMult5,
                altura, totM2, ref valorUnitario, numeroBenef, alturaBenef, larguraBenef);
        }

        public static void CalcValorUnitItemProd(GDASession sessao, uint idCliente, int idProd, int largura, float qtde, float qtdeAmbiente, decimal total, float espessura, bool redondo,
            int arredondarAluminio, bool compra, bool calcMult5, Single altura, Single totM2, ref decimal valorUnitario, int numeroBenef, int alturaBenef, int larguraBenef)
        {
            CalcValorUnitItemProd(sessao, idCliente, idProd, largura, qtde, qtdeAmbiente, total, espessura, redondo, arredondarAluminio, compra, calcMult5, altura,
                totM2, ref valorUnitario, false, numeroBenef, alturaBenef, larguraBenef);
        }

        public static void CalcValorUnitItemProd(uint idCliente, int idProd, int largura, float qtde, float qtdeAmbiente, decimal total, float espessura, bool redondo,
            int arredondarAluminio, bool compra, bool calcMult5, Single altura, Single totM2, ref decimal valorUnitario, bool nf, int numeroBenef, int alturaBenef, int larguraBenef)
        {
            CalcValorUnitItemProd(null, idCliente, idProd, largura, qtde, qtdeAmbiente, total, espessura, redondo, arredondarAluminio, compra, calcMult5,
                altura, totM2, ref valorUnitario, nf, numeroBenef, alturaBenef, larguraBenef);
        }

        public static void CalcValorUnitItemProd(GDASession sessao, uint idCliente, int idProd, int largura, float qtde, float qtdeAmbiente, decimal total, float espessura, bool redondo,
            int arredondarAluminio, bool compra, bool calcMult5, Single altura, Single totM2, ref decimal valorUnitario, bool nf, int numeroBenef, int alturaBenef, int larguraBenef)
        {
            CalcValorUnitItemProd(sessao, idCliente, idProd, largura, qtde, qtdeAmbiente, total, espessura, redondo,
                arredondarAluminio, compra, calcMult5, altura, totM2, ref valorUnitario, nf, numeroBenef, false, alturaBenef, larguraBenef);
        }

        internal static void CalcValorUnitItemProd(GDASession sessao, uint idCliente, int idProd, int largura, float qtde, float qtdeAmbiente, decimal total, float espessura, bool redondo,
            int arredondarAluminio, bool compra, bool calcMult5, Single altura, Single totM2, ref decimal valorUnitario, bool nf, int numeroBenef, bool calcularAreaMinima,
            int alturaBenef, int larguraBenef)
        {
            // Busca os dados do produto escolhido
            int tipoCalc = Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(sessao, idProd, nf || (compra && CompraConfig.UsarTipoCalculoNfParaCompra));
            var idGrupoProd = ProdutoDAO.Instance.ObtemIdGrupoProd(sessao, idProd);

            qtdeAmbiente = qtdeAmbiente > 0 ? qtdeAmbiente : 1;

            // Verifica se o produto deve ser calculado m², se for, mutiplica o valor pela área quadrada
            if (tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 || tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto)
            {
                float areaMinimaProd = ProdutoDAO.Instance.ObtemAreaMinima(sessao, idProd);
                float totM2Temp = totM2;

                // Se o vidro for redondo, adiciona uma certa quantidade na largura e na altura, dependendo da espessura
                int addValor = redondo ? (espessura < 12 ? Geral.AdicionalVidroRedondoAte12mm : Geral.AdicionalVidroRedondoAcima12mm) : 0;

                // Calcula a área total do produto, se a largura for 0, qr dizer q o vidro é redondo, nesse caso,
                // subtitui a largura por 1000, apenas no cálculo
                totM2 = !nf ? Glass.Global.CalculosFluxo.ArredondaM2(sessao, largura, (int)altura, qtde, idProd, redondo, espessura, calcMult5 && tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2) : totM2Temp;

                // Calcula o m² de cálculo
                float totM2Preco = !nf ? Glass.Global.CalculosFluxo.CalcM2Calculo(sessao, idCliente, (int)altura, largura, qtde * qtdeAmbiente, idProd, redondo, numeroBenef > 0 ? numeroBenef :
                    calcularAreaMinima ? 1 : 0, areaMinimaProd, true, espessura, calcMult5 && tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2) : totM2Temp;

                // Se o m2 deste produto for menor que o valor mínimo estabelecido para esse produto,
                // utiliza o valor mínimo para calcular o produto, se AtivarAreaMinima estiver marcado
                Single m2Minimo = ProdutoDAO.Instance.CalcularAreaMinima(sessao, idCliente, idProd, redondo, numeroBenef) ? areaMinimaProd : 0;
                float totM2Calc = totM2Preco < (m2Minimo * qtde * qtdeAmbiente) ? (m2Minimo * qtde * qtdeAmbiente) : totM2Preco;

                // Calcula o valor unitário do produto
                valorUnitario = total / (totM2Calc > 0 ? (decimal)totM2Calc : 1);
            }
            else if (tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.Perimetro)
            {
                int metroLinear = ((int)altura * alturaBenef) + (largura * larguraBenef);
                float dividir = (metroLinear / 1000F) * (qtde * qtdeAmbiente);
                valorUnitario = total / (dividir > 0 ? (decimal)dividir : 1);
            }
            else if (tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.ML)
            {
                float dividir = altura * qtde;
                valorUnitario = total / (dividir > 0 ? (decimal)dividir : 1);
            }
            else if ((tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL0 || tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL05 || tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL1 ||
                tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL6))
            {
                Single decimosAltura = altura - (int)altura;
                Single alturaCalc = altura;

                // Se a empresa trabalha com alumínio vendido no metro, e se o produto atualizado for alumínio,
                // calcula o restante de barra(6m cada) 

                float valorArredondar = tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL05 ? 0.5f :
                    tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL1 ? 1f : 0f;

                if (!Glass.Data.DAL.GrupoProdDAO.Instance.IsAluminio((int)idGrupoProd))
                    arredondarAluminio = 0;

                // Se for para arredondar a altura do alumínio
                if (arredondarAluminio > 0 && tipoCalc != (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL0)
                {
                    if (tipoCalc != (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL6)
                    {
                        if (decimosAltura > 0 && decimosAltura < valorArredondar)
                            alturaCalc = ((int)altura) + (Single)valorArredondar;
                        else if (decimosAltura > valorArredondar)
                            alturaCalc = ((int)altura) + (valorArredondar * 2);
                    }
                    else if (altura < 6)
                        alturaCalc = 6;

                    // Se for para arredondar o alumínio e salvar este arredondamento
                    if (arredondarAluminio == 1)
                        altura = alturaCalc;
                }

                // Faz o cálculo normalmente do múltiplo de 6 metros
                if (alturaCalc == 6)
                    valorUnitario = total / (decimal)((qtde > 0 ? qtde : 1) * qtdeAmbiente);
                else if (alturaCalc < 6) // Calcula o restante da barra adicionando 
                {
                    float dividir = (float)(alturaCalc % 6);
                    valorUnitario = ((total / (decimal)((qtde > 0 ? qtde : 1) * qtdeAmbiente)) / (decimal)(dividir > 0 ? dividir : 1)) * 6;
                }
                else
                {
                    float dividir = alturaCalc * qtde * qtdeAmbiente;
                    valorUnitario = total / (dividir > 0 ? (decimal)dividir : 1) * 6;
                }
            }
            else
            {
                if (altura > 0 && largura > 0)
                    totM2 = Glass.Global.CalculosFluxo.ArredondaM2(sessao, largura, (int)altura, qtde, idProd, redondo);

                valorUnitario = total / (decimal)((qtde > 0 ? qtde : 1) * qtdeAmbiente);
            }
        }

        #endregion
    }
}

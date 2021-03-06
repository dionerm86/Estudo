﻿using GDA;
using Glass.Data.Helper.Calculos.Estrategia;
using Glass.Data.Helper.Calculos.Estrategia.ValorTotal.Enum;
using Glass.Data.Model;
using Glass.Pool;
using System.Collections.Generic;

namespace Glass.Data.Helper.Calculos
{
    sealed class ValorTotal : Singleton<ValorTotal>
    {
        private ValorTotal() { }

        public void Calcular(GDASession sessao, IContainerCalculo container, IProdutoCalculo produto,
            ArredondarAluminio arredondarAluminio, bool calcularMultiploDe5, int numeroBeneficiamentos,
            bool usarChapaVidro = true, bool valorBruto = false, bool primeiroCalculo = false)
        {
            Calcular(
                sessao,
                container,
                produto,
                arredondarAluminio,
                calcularMultiploDe5,
                false,
                false,
                numeroBeneficiamentos,
                usarChapaVidro,
                valorBruto,
                primeiroCalculo);
        }

        /// <summary>
        /// Método utilizado para calcular o valor total e o total de m² de um produto.
        /// </summary>
        public void Calcular(GDASession sessao, IContainerCalculo container, IProdutoCalculo produto,
            ArredondarAluminio arredondarAluminio, bool calcularMultiploDe5, bool nf, bool compra, int numeroBeneficiamentos,
            bool usarChapaVidro = true, bool valorBruto = false, bool primeiroCalculo = false)
        {
            produto.InicializarParaCalculo(sessao, container);

            var alturaBeneficiamento = this.NormalizarAlturaLarguraBeneficiamento(
                produto.AlturaBenef,
                container,
                (int)produto.DadosProduto.DadosGrupoSubgrupo.TipoCalculo(nf, compra));

            var larguraBeneficiamento = this.NormalizarAlturaLarguraBeneficiamento(
                produto.LarguraBenef,
                container,
                (int)produto.DadosProduto.DadosGrupoSubgrupo.TipoCalculo(nf, compra));

            var estrategia = ValorTotalStrategyFactory.Instance.RecuperaEstrategia(produto, nf, compra);
            var acrescimo = 0.00M;

            try
            {
                DescontoAcrescimo.Instance.RemoverDescontoQtde(sessao, container, produto);
                acrescimo = produto.ValorAcrescimo;
                produto.ValorAcrescimo = 0.00M;

                estrategia.Calcular(
                    sessao,
                    produto,
                    arredondarAluminio,
                    calcularMultiploDe5,
                    compra,
                    nf,
                    numeroBeneficiamentos,
                    alturaBeneficiamento,
                    larguraBeneficiamento,
                    usarChapaVidro,
                    valorBruto
                );
            }
            finally
            {
                produto.ValorAcrescimo = acrescimo;
                DescontoAcrescimo.Instance.AplicarDescontoQtde(sessao, container, produto);
            }

            this.IncluirDescontoPorQuantidade(produto);
        }

        /// <summary>
        /// Prepara o recálculo de produtos com desconto por quantidade.
        /// </summary>
        public void PrepararRecalculoAtualizacao(GDASession sessao, IContainerCalculo container, IProdutoCalculo produto)
        {
            var valorUnitario = produto.ValorUnit;

            try
            {
                DescontoAcrescimo.Instance.RemoverDescontoQtde(sessao, container, produto);
            }
            finally
            {
                produto.ValorUnit = valorUnitario;
            }
        }

        private int NormalizarAlturaLarguraBeneficiamento(int? valor, IContainerCalculo container, int tipoCalc)
        {
            if ((container?.MaoDeObra ?? false) && tipoCalc == (int)TipoCalculoGrupoProd.Perimetro)
                return valor.Value;

            return 2;
        }

        private void IncluirDescontoPorQuantidade(IProdutoCalculo produto)
        {
            if (produto.PercDescontoQtde > 0)
            {
                var fatorMultiplicacao = produto.PercDescontoQtde < 100
                    ? 1 - (produto.PercDescontoQtde / 100)
                    : 0;

                produto.Total *= (decimal)fatorMultiplicacao;
            }
        }
    }
}

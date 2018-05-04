using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GDA;
using Glass.Data;

namespace Glass.Rentabilidade.Negocios.Componentes
{
    /// <summary>
    /// Implementação base da calculadora de rentabilidade.
    /// </summary>
    public abstract class CalculadoraRentabilidade
    {
        #region Propriedades

        /// <summary>
        /// Provedor dos descritores de registros de rentabilidade.
        /// </summary>
        protected IProvedorDescritorRegistroRentabilidade ProvedorDescritoresRegistro { get; }

        /// <summary>
        /// Provedor dos indicadores financeiros.
        /// </summary>
        protected IProvedorIndicadorFinanceiro ProvedorIndicadoresFinanceiro { get; }

        /// <summary>
        /// Provedor da calculadora de rentabilidade.
        /// </summary>
        protected IProvedorCalculadoraRentabilidade ProvedorCalculadoraRentabilidade { get; }

        /// <summary>
        /// Identifica se o calculo da rentabilidade está habilitado.
        /// </summary>
        public bool CalculoHabilitado
        {
            get { return Configuracoes.RentabilidadeConfig.CalcularRentabilidade; }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="provedorDescritoresRegistro"></param>
        protected CalculadoraRentabilidade(
            IProvedorDescritorRegistroRentabilidade provedorDescritoresRegistro,
            IProvedorIndicadorFinanceiro provedorIndicadoresFinanceiro,
            IProvedorCalculadoraRentabilidade provedorCalculadoraRentabilidade)
        {
            ProvedorDescritoresRegistro = provedorDescritoresRegistro;
            ProvedorIndicadoresFinanceiro = provedorIndicadoresFinanceiro;
            ProvedorCalculadoraRentabilidade = provedorCalculadoraRentabilidade;
        }

        #endregion

        #region Métodos Protegidos

        /// <summary>
        /// Cria um resultado não executado.
        /// </summary>
        /// <returns></returns>
        protected Data.ICalculoRentabilidadeResultado CriarResultadoNaoExecutado()
        {
            return new CalculoNaoExecutado();
        }

        /// <summary>
        /// Monta o resultado dao calculo;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="comparadorRegistroRentabilidade"></param>
        /// <param name="aplicarValor">Aplicar o valor do registro para a referência</param>
        /// <returns></returns>
        internal Data.ICalculoRentabilidadeResultado MontarResultado<T>(
            IItemRentabilidadeComReferencias<T> item,
            Func<RegistroRentabilidade, T, bool> comparadorRegistroRentabilidade,
            Action<RegistroRentabilidade, T> aplicarValor) where T : Colosoft.Data.BaseModel, new()
        {
            // Recupera os registros novos
            var novos = item.Referencias.Where(f => !f.ExistsInStorage)
                .Select(f =>
                {
                    var registro = item.RegistrosRentabilidade
                        .OfType<RegistroRentabilidade>()
                        .FirstOrDefault(x => comparadorRegistroRentabilidade(x, f));

                    if (registro != null)
                        aplicarValor(registro, f);

                    return f;
                });

            // Recupera os registros atualizados
            var atualizados = item.Referencias
                .Where(f => f.ExistsInStorage &&
                    item.RegistrosRentabilidade.OfType<RegistroRentabilidade>()
                        .Any(x =>
                        {
                            if (comparadorRegistroRentabilidade(x, f))
                            {
                                aplicarValor(x, f);
                                return true;
                            }
                            else
                                return false;
                        }));

            // Recupera os registros apagados.
            var apagados = item.Referencias.Except(novos.Concat(atualizados));

            return new CalculoRentabilidadeResultado<T>(
                true, item.PercentualRentabilidade, item.RentabilidadeFinanceira,
                novos.Select(f =>
                    new CalculoRentabilidadeResultado<T>
                        .Item(CalculoRentabilidadeSituacaoItem.Novo, f))
                .Concat(atualizados.Select(f =>
                    new CalculoRentabilidadeResultado<T>
                        .Item(CalculoRentabilidadeSituacaoItem.Atualizado, f)))
                .Concat(apagados.Select(f =>
                    new CalculoRentabilidadeResultado<T>
                        .Item(CalculoRentabilidadeSituacaoItem.Apagado, f)))
                .ToList());
        }

        /// <summary>
        /// Cria o resultado do calculo da rentabilidade para o item informado.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="resultadoCalculo"></param>
        /// <param name="subResultados">Sub resultados processados.</param>
        /// <returns></returns>
        protected abstract Data.ICalculoRentabilidadeResultado CriaResultado(
            IItemRentabilidade item, ResultadoRentabilidade resultadoCalculo,
            IEnumerable<Data.ICalculoRentabilidadeResultado> subResultados); 

        /// <summary>
        /// Executa o cálculo para o item informado.
        /// </summary>
        /// <param name="item"></param>
        protected Data.ICalculoRentabilidadeResultado Calcular(IItemRentabilidade item)
        {
            var calculadora = ProvedorCalculadoraRentabilidade.Calculadora;

            var container = item as IItemRentabilidadeContainer;

            var subResultados = new List<Data.ICalculoRentabilidadeResultado>();

            // Caso o itens possua outras itens filhos, 
            // os mesmos deve ser calculados
            if (container != null)
                foreach (var i in container.Itens)
                    subResultados.Add(Calcular(i));

            // Executa o calculo da rentabilidae
            var resultadoCalculo = calculadora.Calcular(item);
            item.LimparRegistros();
            resultadoCalculo.AplicarResultado();

            return CriaResultado(item, resultadoCalculo, subResultados);
        }

        /// <summary>
        /// Calcula o prazo médio para a parcela.
        /// </summary>
        /// <param name="parcela"></param>
        /// <returns></returns>
        protected int CalcularPrazoMedio(Data.Model.Parcelas parcela)
        {
            if (parcela == null) return 0;

            var numeroDias = parcela.NumeroDias;

            if (numeroDias.Length == 0)
                return 0;

            return parcela.NumeroDias.Sum() / parcela.NumeroDias.Length;
        }

        #endregion

        #region Tipos Aninhados

        /// <summary>
        /// Implementação de um resultado do calculo da rentabilidade que não foi executado.
        /// </summary>
        class CalculoNaoExecutado : Data.ICalculoRentabilidadeResultado
        {
            #region Eventos

            public event EventHandler<SalvarCalculoRentabilidadeEventArgs> Salvando;

            #endregion

            #region Propriedades

            public bool Executado => false;

            public decimal PercentualRentabilidade => 0m;

            public decimal RentabilidadeFinanceira => 0m;

            #endregion

            #region Métodos Públicos

            public void Salvar(GDASession sessao)
            {
                // Não faz nada.
            }

            #endregion
        }

        #endregion
    }
}

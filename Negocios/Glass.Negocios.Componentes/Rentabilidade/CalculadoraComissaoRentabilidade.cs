using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Rentabilidade.Negocios.Componentes
{
    /// <summary>
    /// Implementação da calculadora da comissão pelo percentual de rentabilidade.
    /// </summary>
    public class CalculadoraComissaoRentabilidade : ICalculadoraComissaoRentabilidade
    {
        #region Local Variables

        private readonly IDictionary<int, IEnumerable<Faixa>> _faixasFuncionario = new Dictionary<int, IEnumerable<Faixa>>();

        #endregion

        #region Métodos Privados

        /// <summary>
        /// Recuperea as faixas com base no identificador do funcionário, de
        /// forma direta no banco de dados.
        /// </summary>
        /// <param name="idFunc"></param>
        /// <returns></returns>
        private IEnumerable<Faixa> ObterFaixasDireto(int? idFunc)
        {
            var registros = SourceContext.Instance.CreateQuery()
                    .From<Data.Model.FaixaRentabilidadeComissao>()
                    .Where("IdFunc=?id")
                    .Add("?id", idFunc)
                    .Execute()
                    .Select(f => new
                    {
                        Id = f.GetInt32(nameof(Data.Model.FaixaRentabilidadeComissao.IdFaixaRentabilidadeComissao)),
                        PercentualRentabilidade = f.GetDecimal(nameof(Data.Model.FaixaRentabilidadeComissao.PercentualRentabilidade)) / 100m,
                        PercentualReducaoComissao = f.GetDecimal(nameof(Data.Model.FaixaRentabilidadeComissao.PercentualComissao)) / 100m
                    })
                    .OrderBy(f => f.PercentualRentabilidade)
                    .ToList();

            if (!registros.Any()) yield break;

            // Verifica se a faixa começa negativa para trabalhar 
            // com a lógica inicial invertida
            if (registros[0].PercentualRentabilidade < 0m)
            {
                var registro = registros[0];
                registros.RemoveAt(0);

                yield return new Faixa(decimal.MinValue, registro.PercentualRentabilidade, registro.PercentualReducaoComissao);

                if (registros.Any())
                    foreach(var registro2 in registros)
                    {
                        yield return new Faixa(
                            registro.PercentualRentabilidade, 
                            registro2.PercentualRentabilidade, 
                            registro2.PercentualReducaoComissao);

                        registro = registro2;
                    }
            }
            else
            {
                var inicio = 0m;

                foreach(var registro in registros)
                {
                    yield return new Faixa(
                            inicio,
                            registro.PercentualRentabilidade,
                            registro.PercentualReducaoComissao);

                    inicio = registro.PercentualRentabilidade;
                }
            }
        }

        /// <summary>
        /// Recupera as faixas com base no identificador do funcionário.
        /// </summary>
        /// <param name="idFunc"></param>
        /// <returns></returns>
        private IEnumerable<Faixa> ObterFaixas(int? idFunc)
        {
            lock (_faixasFuncionario)
            {
                IEnumerable<Faixa> faixas;
                if (!_faixasFuncionario.TryGetValue(idFunc.GetValueOrDefault(), out faixas))
                {
                    faixas = ObterFaixasDireto(idFunc).ToArray();

                    if (idFunc.HasValue && idFunc.Value != 0 && !faixas.Any())
                        // Carrega as faixas gerais
                        faixas = ObterFaixas(null);
                    
                    _faixasFuncionario.Add(idFunc.GetValueOrDefault(), faixas);
                }

                return faixas;
            }
        }

        #endregion

        #region Métodos Privados

        /// <summary>
        /// Realiza o cálculo dos valores da rentabilidade sobre a comissão.
        /// </summary>
        /// <param name="itemRentabilidade">Item onde será aplicado o cálculo.</param>
        /// <param name="idFunc">Identificador do funcionário que criou o item.</param>
        /// <param name="forcaPercentualComissao">Identifica se é para forçar a atualização do percentual de comissão.</param>
        /// <returns></returns>
        public ResultadoComissaoRentabilidade Calcular(IItemRentabilidade itemRentabilidade, int? idFunc, bool forcaPercentualComissao)
        {
            var faixas = ObterFaixas(idFunc);

            var faixa = faixas.FirstOrDefault(f => 
                itemRentabilidade.PercentualRentabilidade > f.Inicio && 
                itemRentabilidade.PercentualRentabilidade <= f.Fim);

            if (faixa != null && faixa.PercentualComissao < itemRentabilidade.PercentualComissao)
                return new ResultadoComissaoRentabilidade(false, faixa.PercentualComissao);

            return new ResultadoComissaoRentabilidade(true, forcaPercentualComissao && faixa != null ? faixa.PercentualComissao : 0m);
        }

        /// <summary>
        /// Força a atualização dos dados da calculadora.
        /// </summary>
        public void AtualizarDados()
        {
            lock (_faixasFuncionario)
                _faixasFuncionario.Clear();
        }

        #endregion

        #region Tipos Aninhados

        /// <summary>
        /// Representa uma faixa usada pela calculadora.
        /// </summary>
        class Faixa
        {
            /// <summary>
            /// Valor de início da faixa da rentabilidade.
            /// </summary>
            public decimal Inicio { get; }

            /// <summary>
            /// Valor de fim da faixa da rentabilidade.
            /// </summary>
            public decimal Fim { get; }

            /// <summary>
            /// Percentual de redução.
            /// </summary>
            public decimal PercentualComissao { get; }

            #region Constructors

            /// <summary>
            /// Construtor padrão.
            /// </summary>
            /// <param name="inicio"></param>
            /// <param name="fim"></param>
            /// <param name="percentualComissao"></param>
            public Faixa(decimal inicio, decimal fim, decimal percentualComissao)
            {
                Inicio = inicio;
                Fim = fim;
                PercentualComissao = percentualComissao;
            }

            #endregion
        }

        #endregion
    }
}

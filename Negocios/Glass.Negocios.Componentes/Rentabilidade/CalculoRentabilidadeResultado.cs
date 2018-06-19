using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Glass.Data;

namespace Glass.Rentabilidade.Negocios.Componentes
{
    /// <summary>
    /// Possíveis situações do item.
    /// </summary>
    enum CalculoRentabilidadeSituacaoItem
    {
        /// <summary>
        /// Identifica que é um registro novo.
        /// </summary>
        Novo,
        /// <summary>
        /// Identifica que o registor foi apagado.
        /// </summary>
        Apagado,
        /// <summary>
        /// Identificado que o registro foi atualizado.
        /// </summary>
        Atualizado
    }

    /// <summary>
    /// Representa um container de resultados do calculo de rentabilidade.
    /// </summary>
    interface ICalculoRentabilidadeResultadoContainer
    {
        #region Propriedades

        /// <summary>
        /// Relação dos resultados filhos.
        /// </summary>
        IEnumerable<Data.ICalculoRentabilidadeResultado> Resultados { get; }

        #endregion

        #region Métodos

        /// <summary>
        /// Adiciona um resultado filho.
        /// </summary>
        /// <param name="resultado"></param>
        void Adicionar(Data.ICalculoRentabilidadeResultado resultado);

        #endregion
    }

    /// <summary>
    /// Representa o resultado do cálculo da rentabilidade.
    /// </summary>
    /// <typeparam name="TRegistro">Tipo do registro associado.</typeparam>
    class CalculoRentabilidadeResultado<TRegistro> : 
        Data.ICalculoRentabilidadeResultado, ICalculoRentabilidadeResultadoContainer where TRegistro : Colosoft.Data.BaseModel, new()
    {
        #region Variáveis Locais

        private readonly List<Data.ICalculoRentabilidadeResultado> _resultadosFilhos = new List<Data.ICalculoRentabilidadeResultado>();

        #endregion

        #region Events

        /// <summary>
        /// Evento acionado quando o resultado estiver sendo salvo.
        /// </summary>
        public event EventHandler<Data.SalvarCalculoRentabilidadeEventArgs> Salvando;

        #endregion

        #region Properties

        /// <summary>
        /// Item da rentabilidade.
        /// </summary>
        public IItemRentabilidade ItemRentabilidade { get; }

        /// <summary>
        /// Identifica se o cálculo foi executado.
        /// </summary>
        public bool Executado { get; }

        /// <summary>
        /// Percentual da rentabilidade.
        /// </summary>
        public decimal PercentualRentabilidade { get; }

        /// <summary>
        /// Rentabilidade financeira.
        /// </summary>
        public decimal RentabilidadeFinanceira { get; }

        /// <summary>
        /// Itens associados.
        /// </summary>
        public IEnumerable<Item> Itens { get; }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="itemRentabilidade"></param>
        /// <param name="executado">Identifica se o cálculo foi executado.</param>
        /// <param name="percentualRentabilidade"></param>
        /// <param name="rentabilidadeFinanceira"></param>
        /// <param name="itens">Itens associados.</param>
        public CalculoRentabilidadeResultado(
            IItemRentabilidade itemRentabilidade,
            bool executado,
            decimal percentualRentabilidade, decimal rentabilidadeFinanceira,
            IEnumerable<Item> itens)
        {
            ItemRentabilidade = itemRentabilidade;
            Executado = executado;
            PercentualRentabilidade = percentualRentabilidade;
            RentabilidadeFinanceira = rentabilidadeFinanceira;
            Itens = itens ?? new Item[0];
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Salva os itens do resultado.
        /// </summary>
        /// <param name="sessao"></param>
        public virtual void Salvar(GDA.GDASession sessao)
        {
            // Salva os resultados filhos
            foreach (var i in _resultadosFilhos)
                i.Salvar(sessao);

            var dao = GDA.GDAOperations.GetDAO<TRegistro>();

            // Percorre os itens para serem salvos
            foreach (var item in Itens)
            {
                switch (item.Situacao)
                {
                    case CalculoRentabilidadeSituacaoItem.Novo:
                        dao.Insert(sessao, item.Registro);
                        item.Registro.ExistsInStorage = true;
                        break;
                    case CalculoRentabilidadeSituacaoItem.Atualizado:
                        dao.Update(sessao, item.Registro);
                        break;
                    case CalculoRentabilidadeSituacaoItem.Apagado:
                        dao.Delete(sessao, item.Registro);
                        break;
                }
            }

            Salvando?.Invoke(this, new Data.SalvarCalculoRentabilidadeEventArgs(sessao, PercentualRentabilidade, RentabilidadeFinanceira));
        }

        #endregion

        #region Membros de ICalculoRentabilidadeResultadoContainer

        IEnumerable<ICalculoRentabilidadeResultado> ICalculoRentabilidadeResultadoContainer.Resultados => _resultadosFilhos;

        void ICalculoRentabilidadeResultadoContainer.Adicionar(ICalculoRentabilidadeResultado resultado)
        {
            _resultadosFilhos.Add(resultado);
        }

        #endregion

        #region Tipos Aninhados

        /// <summary>
        /// Representa um item do resultado.
        /// </summary>
        public class Item
        {
            #region Variáveis Locais

            private readonly CalculoRentabilidadeSituacaoItem _situacao;

            #endregion

            #region Propriedades

            /// <summary>
            /// Situação do item.
            /// </summary>
            public CalculoRentabilidadeSituacaoItem Situacao
            {
                get
                {
                    if (_situacao == CalculoRentabilidadeSituacaoItem.Novo && Registro.ExistsInStorage)
                        return CalculoRentabilidadeSituacaoItem.Atualizado;

                    return _situacao;
                }
            }

            /// <summary>
            /// Registro associado.
            /// </summary>
            public TRegistro Registro { get; }

            #endregion

            #region Construtores

            /// <summary>
            /// Construtor padrão.
            /// </summary>
            /// <param name="situacao"></param>
            /// <param name="registro"></param>
            public Item(CalculoRentabilidadeSituacaoItem situacao, TRegistro registro)
            {
                _situacao = situacao;
                Registro = registro;
            }

            #endregion

            #region Métodos Públicos

            /// <summary>
            /// Recupera o texto que representa a instancia.
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return $"{Situacao}: {Registro}";
            }

            #endregion
        }

        #endregion
    }
}

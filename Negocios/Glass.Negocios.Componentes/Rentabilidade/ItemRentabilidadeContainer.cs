using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Rentabilidade.Negocios.Componentes
{
    /// <summary>
    /// Implementação do item de rentabilidade com itens e referências.
    /// </summary>
    /// <typeparam name="TReferencia"></typeparam>
    class ItemRentabilidadeContainer<TOwner, TReferencia> :
        ItemRentabilidade, IItemRentabilidadeContainer, 
        IItemRentabilidadeComReferencias<TReferencia>,
        IItemRentabilidade<TOwner>
        where TReferencia : Colosoft.Data.BaseModel
    {
        #region Variáveis Locais

        private readonly Lazy<IList<TReferencia>> _referencias;
        private readonly Func<IItemRentabilidade, bool> _filtroItensParaCalculo;
        private readonly Func<TReferencia, IRegistroRentabilidade> _conversorReferencia;
        private bool _registrosCarregados = false;

        #endregion

        #region Propriedades

        /// <summary>
        /// Proprietário.
        /// </summary>
        public TOwner Proprietario { get; }

        /// <summary>
        /// Relação de todos os itens
        /// </summary>
        public IEnumerable<IItemRentabilidade> TodosItens { get; }

        /// <summary>
        /// Referencias.
        /// </summary>
        public IEnumerable<TReferencia> Referencias => _referencias.Value;

        /// <summary>
        /// Relação dos itens que serão usados nos calculos da instancia.
        /// </summary>
        public IEnumerable<IItemRentabilidade> Itens => TodosItens.Where(_filtroItensParaCalculo);

        /// <summary>
        /// Fator do ICMS de substituição.
        /// </summary>
        public override decimal FatorICMSSubstituicao
        {
            get { return Itens.Any() ? Itens.Average(f => f.FatorICMSSubstituicao) : 0m; }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Percentual do ICMS de compra.
        /// </summary>
        public override decimal PercentualICMSCompra
        {
            get { return Itens.Any() ? Itens.Average(f => f.PercentualICMSCompra) : 0m; }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Percentual do ICMS de venda.
        /// </summary>
        public override decimal PercentualICMSVenda
        {
            get { return Itens.Any() ? Itens.Average(f => f.PercentualICMSVenda) : 0m; }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Percentual do IPI de compra.
        /// </summary>
        public override decimal PercentualIPICompra
        {
            get { return Itens.Any() ? Itens.Average(f => f.PercentualIPICompra) : 0m; }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Percentual do IPI de venda.
        /// </summary>
        public override decimal PercentualIPIVenda
        {
            get { return Itens.Any() ? Itens.Average(f => f.PercentualIPIVenda) : 0m; }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Preço de custo.
        /// </summary>
        public override decimal PrecoCusto
        {
            get { return Itens.Sum(f => f.PrecoCusto); }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Registros da rentabilidade.
        /// </summary>
        public override IEnumerable<IRegistroRentabilidade> RegistrosRentabilidade
        {
            get
            {
                if (!_registrosCarregados)
                    foreach (var i in Referencias.Select(_conversorReferencia))
                        base.AdicionarRegistro(i);

                _registrosCarregados = true;
                return base.RegistrosRentabilidade;
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="provedorIndicadoresFinanceiro"></param>
        /// <param name="criarRegistroRentabilidade"></param>
        /// <param name="proprietario"></param>
        /// <param name="itens"></param>
        /// <param name="filtroItensParaCalculo">Filtro que será usado para recupera os itens para o calculo.</param>
        /// <param name="referencias"></param>
        /// <param name="conversorReferencia">Método usado para converter a referencia em um item de rentabilidade.</param>
        public ItemRentabilidadeContainer(
            IProvedorIndicadorFinanceiro provedorIndicadoresFinanceiro,
            CriadorRegistroRentabilidade criarRegistroRentabilidade,
            TOwner proprietario,
            IEnumerable<IItemRentabilidade> itens,
            Func<IItemRentabilidade, bool> filtroItensParaCalculo,
            Lazy<IList<TReferencia>> referencias,
            Func<TReferencia, IRegistroRentabilidade> conversorReferencia)
            : base(provedorIndicadoresFinanceiro, criarRegistroRentabilidade)
        {
            Proprietario = proprietario;
            TodosItens = itens;
            _filtroItensParaCalculo = filtroItensParaCalculo;
            _referencias = referencias;
            _conversorReferencia = conversorReferencia;
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Limpa os registros da rentabilidade.
        /// </summary>
        public override void LimparRegistros()
        {
            _registrosCarregados = true;
            base.LimparRegistros();
        }

        #endregion
    }
}

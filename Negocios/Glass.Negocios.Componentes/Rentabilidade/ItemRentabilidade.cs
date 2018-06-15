using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Colosoft;

namespace Glass.Rentabilidade.Negocios.Componentes
{
    /// <summary>
    /// Assinatura do método usado para criar o registro de rentabilidade.
    /// </summary>
    /// <param name="tipo">Tipo do registro.</param>
    /// <param name="nome">Nome.</param>
    /// <param name="valor">Valor.</param>
    /// <returns></returns>
    public delegate IRegistroRentabilidade CriadorRegistroRentabilidade(TipoRegistroRentabilidade tipo, string nome, decimal valor);

    /// <summary>
    /// Assinatura de um item de rentabilidade com o proprietário.
    /// </summary>
    /// <typeparam name="TOwner"></typeparam>
    interface IItemRentabilidade<out TOwner> : IItemRentabilidade
    {
        #region Propriedades

        /// <summary>
        /// Proprietário.
        /// </summary>
        TOwner Proprietario { get; }

        #endregion
    }

    /// <summary>
    /// Implementação base de um item da rentabilidade.
    /// </summary>
    abstract class ItemRentabilidade : IItemRentabilidade
    {
        #region Variáveis Locais

        private readonly List<Rentabilidade.IRegistroRentabilidade> _registrosRentabilidade = new List<IRegistroRentabilidade>();
        private readonly CriadorRegistroRentabilidade _criarRegistroRentabilidade;
        private decimal? _percentualICMSCompra;

        #endregion

        #region Propriedades

        /// <summary>
        /// Indicadores financeiros.
        /// </summary>
        protected IProvedorIndicadorFinanceiro IndicadoresFinanceiros { get; }

        /// <summary>
        /// Descrição do item.
        /// </summary>
        public string Descricao { get; set; }

        /// <summary>
        /// Preço de venda sem o IPI.
        /// </summary>
        public decimal PrecoVendaSemIPI { get; set; }

        /// <summary>
        /// Preço de custo do item.
        /// </summary>
        public virtual decimal PrecoCusto { get; set; }

        /// <summary>
        /// Prazo média em dias.
        /// </summary>
        public int PrazoMedio { get; set; }

        /// <summary>
        /// Percentual do ICMS de compra.
        /// </summary>
        public virtual decimal PercentualICMSCompra
        { 
            get
            {
                return _percentualICMSCompra ??
                    (IndicadoresFinanceiros.Contains("AliqICMSCompra") ?
                        IndicadoresFinanceiros["AliqICMSCompra"] : 0m);
            }
            set
            {
                _percentualICMSCompra = value;
            }
        }

        /// <summary>
        /// Percentual do ICMS de venda.
        /// </summary>
        public virtual decimal PercentualICMSVenda { get; set; }

        /// <summary>
        /// Fator do ICMS de substituição.
        /// </summary>
        public decimal FatorICMSSubstituicao { get; set; }

        /// <summary>
        /// Percentual do IPI de compra.
        /// </summary>
        public virtual decimal PercentualIPICompra { get; set; }

        /// <summary>
        /// Percentual do IPI de venda.
        /// </summary>
        public virtual decimal PercentualIPIVenda { get; set; }

        /// <summary>
        /// Percentual de comissão.
        /// </summary>
        public decimal PercentualComissao { get; set; }

        /// <summary>
        /// Custos extras.
        /// </summary>
        public decimal CustosExtras { get; set; }

        /// <summary>
        /// Percentual de rentabilidade.
        /// </summary>
        public decimal PercentualRentabilidade { get; set; }

        /// <summary>
        /// Rentabilidade financeira.
        /// </summary>
        public decimal RentabilidadeFinanceira { get; set; }

        /// <summary>
        /// Registros de rentabilidade associados.
        /// </summary>
        public virtual IEnumerable<Rentabilidade.IRegistroRentabilidade> RegistrosRentabilidade => _registrosRentabilidade;

        #endregion

        #region Constructors

        /// <summary>
        /// Cria a instancia com base no registor vindo do banco de dados.
        /// </summary>
        /// <param name="provedorIndicadoresFinanceiro">Provedor dos indicadores financeiros.</param>
        /// <param name="criarRegistroRentabilidade">Referencia do método para criar o registor de rentabilidade.</param>
        protected ItemRentabilidade(
            IProvedorIndicadorFinanceiro provedorIndicadoresFinanceiro, 
            CriadorRegistroRentabilidade criarRegistroRentabilidade)
        {
            provedorIndicadoresFinanceiro.Require(nameof(provedorIndicadoresFinanceiro)).NotNull();
            criarRegistroRentabilidade.Require(nameof(criarRegistroRentabilidade)).NotNull();

            IndicadoresFinanceiros = provedorIndicadoresFinanceiro;
            _criarRegistroRentabilidade = criarRegistroRentabilidade;
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Limpa os registros da instancia.
        /// </summary>
        public virtual void LimparRegistros()
        {
            _registrosRentabilidade.Clear();
        }

        /// <summary>
        /// Adiciona um registro para o item.
        /// </summary>
        /// <param name="registro"></param>
        public void AdicionarRegistro(IRegistroRentabilidade registro)
        {
            _registrosRentabilidade.Add(registro);
        }

        /// <summary>
        /// Cria uma instancia do registro.
        /// </summary>
        /// <param name="tipo">Tipo do registro.</param>
        /// <param name="nome">Nome do registro.</param>
        /// <param name="valor">Valor do registro</param>
        /// <returns></returns>
        public virtual IRegistroRentabilidade CriarRegistro(TipoRegistroRentabilidade tipo, string nome, decimal valor)
        {
            var registro = RegistrosRentabilidade.FirstOrDefault(f => f.Tipo == tipo && f.Descritor.Nome == nome);
            if (registro != null)
                _registrosRentabilidade.Remove(registro);

            return _criarRegistroRentabilidade(tipo, nome, valor);
        }
        
        /// <summary>
        /// Recupera o texto que representa a instancia.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Descricao} => PrecoVendaSemIPI: {PrecoVendaSemIPI}; PrecoCusto: {PrecoCusto}";
        }

        #endregion
    }

    /// <summary>
    /// Implementação do item da rentabilidade com referências.
    /// </summary>
    /// <typeparam name="TOwner">Tipo do proprietário do item.</typeparam>
    /// <typeparam name="TReferencia"></typeparam>
    class ItemRentabilidade<TOwner, TReferencia> :
        ItemRentabilidade, IItemRentabilidade<TOwner>, IItemRentabilidadeComReferencias<TReferencia>
        where TReferencia : Colosoft.Data.BaseModel
    {
        #region Variáveis Locais

        private readonly Lazy<IList<TReferencia>> _referencias;
        private readonly Func<TReferencia, IRegistroRentabilidade> _conversorReferencia;
        private bool _registrosCarregados = false;

        #endregion

        #region Propriedades

        /// <summary>
        /// Proprietário.
        /// </summary>
        public TOwner Proprietario { get; }

        /// <summary>
        /// Referencias.
        /// </summary>
        public IEnumerable<TReferencia> Referencias => _referencias.Value;

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
        /// <param name="referencias"></param>
        public ItemRentabilidade(
            IProvedorIndicadorFinanceiro provedorIndicadoresFinanceiro,
            CriadorRegistroRentabilidade criarRegistroRentabilidade,
            TOwner proprietario,
            Lazy<IList<TReferencia>> referencias,
            Func<TReferencia, IRegistroRentabilidade> conversorReferencia)
            : base(provedorIndicadoresFinanceiro, criarRegistroRentabilidade)
        {
            Proprietario = proprietario;
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

using Colosoft.Business;
using Colosoft.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Rentabilidade.Negocios.Entidades
{
    /// <summary>
    /// Representa a entidade de negócio de um indicador financeiro.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(IndicadorFinanceiroLoader))]
    [Glass.Negocios.ControleAlteracao(Data.Model.LogAlteracao.TabelaAlteracao.IndicadorFinanceiro)]
    public class IndicadorFinanceiro : Colosoft.Business.Entity<Data.Model.IndicadorFinanceiro>
    {
        #region Tipos Aninhados

        class IndicadorFinanceiroLoader : Colosoft.Business.EntityLoader<IndicadorFinanceiro, Data.Model.IndicadorFinanceiro>
        {
            public IndicadorFinanceiroLoader()
            {
                Configure()
                    .Uid(f => f.IdIndicadorFinanceiro)
                    .FindName(f => f.Nome)
                    .Creator(f => new IndicadorFinanceiro(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador do indicador.
        /// </summary>
        public int IdIndicadorFinanceiro
        {
            get { return DataModel.IdIndicadorFinanceiro; }
            set
            {
                if (DataModel.IdIndicadorFinanceiro != value &&
                    RaisePropertyChanging(nameof(IdIndicadorFinanceiro), value))
                {
                    DataModel.IdIndicadorFinanceiro = value;
                    RaisePropertyChanged(nameof(IdIndicadorFinanceiro));
                }
            }
        }

        /// <summary>
        /// Nome.
        /// </summary>
        public string Nome
        {
            get { return DataModel.Nome; }
            set
            {
                if (DataModel.Nome != value &&
                    RaisePropertyChanging(nameof(Nome), value))
                {
                    DataModel.Nome = value;
                    RaisePropertyChanged(nameof(Nome));
                }
            }
        }

        /// <summary>
        /// Descrição.
        /// </summary>
        public string Descricao
        {
            get { return DataModel.Descricao; }
            set
            {
                if (DataModel.Descricao != value &&
                    RaisePropertyChanging(nameof(Descricao), value))
                {
                    DataModel.Descricao = value;
                    RaisePropertyChanged(nameof(Descricao));
                }
            }
        }

        /// <summary>
        /// Valor.
        /// </summary>
        public decimal Valor
        {
            get { return DataModel.Valor; }
            set
            {
                if (DataModel.Valor != value &&
                    RaisePropertyChanging(nameof(Valor), value))
                {
                    DataModel.Valor = value;
                    RaisePropertyChanged(nameof(Valor));
                }
            }
        }

        /// <summary>
        /// Formatação.
        /// </summary>
        public string Formatacao
        {
            get { return DataModel.Formatacao; }
            set
            {
                if (DataModel.Formatacao != value &&
                    RaisePropertyChanging(nameof(Formatacao), value))
                {
                    DataModel.Formatacao = value;
                    RaisePropertyChanged(nameof(Formatacao));
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
		/// Construtor padrão.
		/// </summary>
		public IndicadorFinanceiro()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected IndicadorFinanceiro(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.IndicadorFinanceiro> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public IndicadorFinanceiro(Data.Model.IndicadorFinanceiro dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Salva os dados da expressão.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public override SaveResult Save(IPersistenceSession session)
        {
            var resultado = base.Save(session);

            if (resultado && !ExistsInStorage)
            {
                var provedorConfig = Microsoft.Practices.ServiceLocation.ServiceLocator.Current
                    .GetInstance<IProvedorConfigRegistroRentabilidade>();

                var posicaoConfig = provedorConfig.ObterUltimaPosicao() + 1;

                var config = new ConfigRegistroRentabilidade
                {
                    Tipo = (byte)Rentabilidade.TipoRegistroRentabilidade.IndicadorFinaceiro,
                    IdRegistro = IdIndicadorFinanceiro,
                    Posicao = posicaoConfig
                };

                var resultado2 = config.Save(session);
                if (!resultado2)
                    return resultado2;
            }

            return resultado;
        }

        /// <summary>
        /// Método acionado quando a expressão for apagada.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public override DeleteResult Delete(IPersistenceSession session)
        {
            var resultado = base.Delete(session);

            if (resultado)
            {
                // Apaga a configuração do registro de rentabilidade associada
                session.Delete<Data.Model.ConfigRegistroRentabilidade>(
                    Colosoft.Query.ConditionalContainer.Parse("Tipo=?tipo AND IdRegistro=?id")
                        .Add("?tipo", (byte)Rentabilidade.TipoRegistroRentabilidade.IndicadorFinaceiro)
                        .Add("?id", IdIndicadorFinanceiro));
            }

            return resultado;
        }

        #endregion
    }
}

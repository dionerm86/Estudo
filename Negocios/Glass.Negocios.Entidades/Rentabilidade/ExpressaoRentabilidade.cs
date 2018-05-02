using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Colosoft.Business;
using Colosoft.Data;

namespace Glass.Rentabilidade.Negocios.Entidades
{
    /// <summary>
    /// Assinatura do provedor da expressões de rentabilidade.
    /// </summary>
    public interface IProvedorExpressaoRentabilidade
    {
        /// <summary>
        /// Recupera a última posição das expressões da rentabilidade.
        /// </summary>
        /// <returns></returns>
        int ObterUltimaPosicao();
    }

    /// <summary>
    /// Representa o calculo de rentabilidade.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(ExpressaoRentabilidadeLoader))]
    [Glass.Negocios.ControleAlteracao(Data.Model.LogAlteracao.TabelaAlteracao.ExpressaoRentabilidade)]
    public class ExpressaoRentabilidade : Colosoft.Business.Entity<Data.Model.ExpressaoRentabilidade>
    {
        #region Tipos Aninhados

        class ExpressaoRentabilidadeLoader : Colosoft.Business.EntityLoader<ExpressaoRentabilidade, Data.Model.ExpressaoRentabilidade>
        {
            public ExpressaoRentabilidadeLoader()
            {
                Configure()
                    .Uid(f => f.IdExpressaoRentabilidade)
                    .FindName(f => f.Nome)
                    .Description(f => f.Descricao)
                    .Creator(f => new ExpressaoRentabilidade(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador do cálculo.
        /// </summary>
        public int IdExpressaoRentabilidade
        {
            get { return DataModel.IdExpressaoRentabilidade; }
            set
            {
                if (DataModel.IdExpressaoRentabilidade != value &&
                    RaisePropertyChanging(nameof(IdExpressaoRentabilidade), value))
                {
                    DataModel.IdExpressaoRentabilidade = value;
                    RaisePropertyChanged(nameof(IdExpressaoRentabilidade));
                }
            }
        }


        /// <summary>
        /// Posição.
        /// </summary>
        public int Posicao
        {
            get { return DataModel.Posicao; }
            set
            {
                if (DataModel.Posicao != value &&
                    RaisePropertyChanging(nameof(Posicao), value))
                {
                    DataModel.Posicao = value;
                    RaisePropertyChanged(nameof(Posicao));
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
        /// Expressão.
        /// </summary>
        public string Expressao
        {
            get { return DataModel.Expressao; }
            set
            {
                if (DataModel.Expressao != value &&
                    RaisePropertyChanging(nameof(Expressao), value))
                {
                    DataModel.Expressao = value;
                    RaisePropertyChanged(nameof(Expressao));
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

        /// <summary>
        /// Identifica se a expressão soma na formula da rentabilidade.
        /// </summary>
        public bool SomaFormulaRentabilidade
        {
            get { return DataModel.SomaFormulaRentabilidade; }
            set
            {
                if (DataModel.SomaFormulaRentabilidade != value &&
                    RaisePropertyChanging(nameof(SomaFormulaRentabilidade), value))
                {
                    DataModel.SomaFormulaRentabilidade = value;
                    RaisePropertyChanged(nameof(SomaFormulaRentabilidade));
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
		/// Construtor padrão.
		/// </summary>
		public ExpressaoRentabilidade()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected ExpressaoRentabilidade(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.ExpressaoRentabilidade> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public ExpressaoRentabilidade(Data.Model.ExpressaoRentabilidade dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
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
            if (!ExistsInStorage && Posicao == 0)
            {
                var provedor = Microsoft.Practices.ServiceLocation.ServiceLocator.Current
                    .GetInstance<IProvedorExpressaoRentabilidade>();

                Posicao = provedor.ObterUltimaPosicao() + 1;
            }

            var resultado = base.Save(session);

            if (resultado && !ExistsInStorage)
            {
                var provedorConfig = Microsoft.Practices.ServiceLocation.ServiceLocator.Current
                    .GetInstance<IProvedorConfigRegistroRentabilidade>();

                var posicaoConfig = provedorConfig.ObterUltimaPosicao() + 1;

                var config = new ConfigRegistroRentabilidade
                {
                    Tipo = (byte)Rentabilidade.TipoRegistroRentabilidade.Expressao,
                    IdRegistro = IdExpressaoRentabilidade,
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
                        .Add("?tipo", (byte)Rentabilidade.TipoRegistroRentabilidade.Expressao)
                        .Add("?id", IdExpressaoRentabilidade));
            }

            return resultado;
        }

        #endregion
    }
}

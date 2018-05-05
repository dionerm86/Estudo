using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Rentabilidade.Negocios.Entidades
{
    /// <summary>
    /// Assinatura do provedor das configurações.
    /// </summary>
    public interface IProvedorConfigRegistroRentabilidade
    {
        /// <summary>
        /// Recupera a última posição da configuração.
        /// </summary>
        /// <returns></returns>
        int ObterUltimaPosicao();
    }

    /// <summary>
    /// Representa a configuração do registro de rentabilidade.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(ConfigRegistroRentabilidadeLoader))]
    public class ConfigRegistroRentabilidade : Colosoft.Business.Entity<Data.Model.ConfigRegistroRentabilidade>
    {
        #region Tipos Aninhados

        class ConfigRegistroRentabilidadeLoader : Colosoft.Business.EntityLoader<ConfigRegistroRentabilidade, Data.Model.ConfigRegistroRentabilidade>
        {
            public ConfigRegistroRentabilidadeLoader()
            {
                Configure()
                    .Keys(f => f.Tipo, f => f.IdRegistro)
                    .Creator(f => new ConfigRegistroRentabilidade(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Tipo.
        /// </summary>
        public int Tipo
        {
            get { return DataModel.Tipo; }
            set
            {
                if (DataModel.Tipo != value &&
                    RaisePropertyChanging(nameof(Tipo), value))
                {
                    DataModel.Tipo = value;
                    RaisePropertyChanged(nameof(Tipo));
                }
            }
        }

        /// <summary>
        /// Identificador do registro.
        /// </summary>
        public int IdRegistro
        {
            get { return DataModel.IdRegistro; }
            set
            {
                if (DataModel.IdRegistro != value &&
                    RaisePropertyChanging(nameof(IdRegistro), value))
                {
                    DataModel.IdRegistro = value;
                    RaisePropertyChanged(nameof(IdRegistro));
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
        /// Identifica se é para exibir no relatório.
        /// </summary>
        public bool ExibirRelatorio
        {
            get { return DataModel.ExibirRelatorio; }
            set
            {
                if (DataModel.ExibirRelatorio != value &&
                    RaisePropertyChanging(nameof(ExibirRelatorio), value))
                {
                    DataModel.ExibirRelatorio = value;
                    RaisePropertyChanged(nameof(ExibirRelatorio));
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
		/// Construtor padrão.
		/// </summary>
		public ConfigRegistroRentabilidade()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected ConfigRegistroRentabilidade(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.ConfigRegistroRentabilidade> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public ConfigRegistroRentabilidade(Data.Model.ConfigRegistroRentabilidade dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion
    }
}

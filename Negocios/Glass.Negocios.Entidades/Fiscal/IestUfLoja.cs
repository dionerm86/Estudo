using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Fiscal.Negocios.Entidades
{
    /// <summary>
    /// Representa a entidade de négocio do tipo de
    /// Inscrição Estadual Substituto Tributário por UF da Loja
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(IestUfLojaLoader))]
    [Glass.Negocios.ControleAlteracao(Data.Model.LogAlteracao.TabelaAlteracao.IestUfLoja)]
    public class IestUfLoja : Colosoft.Business.Entity<Data.Model.IestUfLoja>
    {
        #region Tipos Aninhados

        class IestUfLojaLoader : Colosoft.Business.EntityLoader<IestUfLoja, Data.Model.IestUfLoja>
        {
            public IestUfLojaLoader()
            {
                Configure()
                    .Uid(t => t.IdIestUfLoja)
                    .Creator(f => new IestUfLoja(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador do IEST UF Loja
        /// </summary>
        public int IdIestUfLoja
        {
            get { return DataModel.IdIestUfLoja; }
            set
            {
                if(DataModel.IdIestUfLoja != value &&
                    RaisePropertyChanging("IdIestUfLoja", value))
                {
                    DataModel.IdIestUfLoja = value;
                    RaisePropertyChanged("IdIestUfLoja");
                }
            }
        }

        /// <summary>
        /// Identificador da Loja no IestUfLoja
        /// </summary>
        public uint IdLoja
        {
            get { return DataModel.IdLoja; }
            set
            {
                if(DataModel.IdLoja != value &&
                    RaisePropertyChanging("IdLoja", value))
                {
                    DataModel.IdLoja = value;
                    RaisePropertyChanged("IdLoja");
                }
            }
        }

        /// <summary>
        /// Nome UF no IestUfLoja
        /// </summary>
        public string NomeUf
        {
            get { return DataModel.NomeUf; }
            set
            {
                if(DataModel.NomeUf != value &&
                    RaisePropertyChanging("NomeUf", value))
                {
                    DataModel.NomeUf = value;
                    RaisePropertyChanged("NomeUf");
                }
            }
        }

        /// <summary>
        /// Inscrição Estadual do Substituto Tributário no IestUfLoja
        /// </summary>
        public string InscEstSt
        {
            get { return DataModel.InscEstSt; }
            set
            {
                if(DataModel.InscEstSt != value &&
                    RaisePropertyChanging("InscEstSt", value))
                {
                    DataModel.InscEstSt = value;
                    RaisePropertyChanged("InscEstSt");
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public IestUfLoja()
            : this(null, null, null) { }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected IestUfLoja(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.IestUfLoja> args)
            : base(args.DataModel, args.UIContext, args.TypeManager) { }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public IestUfLoja(Data.Model.IestUfLoja dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager) { }

        #endregion
    }
}

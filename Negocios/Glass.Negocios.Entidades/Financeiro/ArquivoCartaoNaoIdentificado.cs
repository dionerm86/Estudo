using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Financeiro.Negocios.Entidades
{
    /// <summary>
    /// Representa a entidade do arquivo de cartão não identificado
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(ArquivoCartaoNaoIdentificadoLoader))]
    public class ArquivoCartaoNaoIdentificado : Glass.Negocios.Entidades.EntidadeBaseCadastro<Data.Model.ArquivoCartaoNaoIdentificado>
    {
        #region Tipos Aninhados

        class ArquivoCartaoNaoIdentificadoLoader : Colosoft.Business.EntityLoader<ArquivoCartaoNaoIdentificado, Data.Model.ArquivoCartaoNaoIdentificado>
        {
            public ArquivoCartaoNaoIdentificadoLoader()
            {
                Configure()
                    .Uid(f => f.IdArquivoCartaoNaoIdentificado)
                    .Creator(f => new ArquivoCartaoNaoIdentificado(f));
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public ArquivoCartaoNaoIdentificado()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected ArquivoCartaoNaoIdentificado(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.ArquivoCartaoNaoIdentificado> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public ArquivoCartaoNaoIdentificado(Data.Model.ArquivoCartaoNaoIdentificado dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Id do arquivo de cartão não identificado
        /// </summary>
        public int IdArquivoCartaoNaoIdentificado
        {
            get { return DataModel.IdArquivoCartaoNaoIdentificado; }
            set
            {
                if (DataModel.IdArquivoCartaoNaoIdentificado != value &&
                    RaisePropertyChanging("IdArquivoCartaoNaoIdentificado", value))
                {
                    DataModel.IdArquivoCartaoNaoIdentificado = value;
                    RaisePropertyChanged("IdArquivoCartaoNaoIdentificado");
                }
            }
        }

        /// <summary>
        /// Situação do Arquivo.
        /// </summary>
        public Data.Model.SituacaoArquivoCartaoNaoIdentificado Situacao
        {
            get { return DataModel.Situacao; }
            set
            {
                if (DataModel.Situacao != value &&
                    RaisePropertyChanging("Situacao", value))
                {
                    DataModel.Situacao = value;
                    RaisePropertyChanged("Situacao");
                }
            }
        }

        /// <summary>
        /// Data de cadastro do Arquivo de cartão não identificado
        /// </summary>
        public DateTime DataCad
        {
            get { return DataModel.DataCad; }
        }

        /// <summary>
        /// Usuário que efetuou o cadastro
        /// </summary>
        public uint UsuCad
        {
            get { return DataModel.Usucad; }
        }

        public bool PodeCancelar
        {
            get { return PodeCancelarArquivo(IdArquivoCartaoNaoIdentificado); }
        }

        /// <summary>
        /// Indicador Pode cancelar o arquivo
        /// </summary>
        public static bool PodeCancelarArquivo(int idArquivoCartaoNaoIdentificado)
        {
            return Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<IProvedorArquivoCartaoNaoIdentificado>()
                    .PodeCancelarArquivo(idArquivoCartaoNaoIdentificado);
        }

        #endregion
    }
}

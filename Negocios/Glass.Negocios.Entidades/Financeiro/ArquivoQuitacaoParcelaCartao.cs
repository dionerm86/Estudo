using System;

namespace Glass.Financeiro.Negocios.Entidades
{
    /// <summary>
    /// Representa a entidade do arquivo de quitar parcela de cartão
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(ArquivoQuitacaoParcelaCartaoLoader))]
    public class ArquivoQuitacaoParcelaCartao : Glass.Negocios.Entidades.EntidadeBaseCadastro<Data.Model.ArquivoQuitacaoParcelaCartao>
    {
        #region Tipos Aninhados

        class ArquivoQuitacaoParcelaCartaoLoader : Colosoft.Business.EntityLoader<ArquivoQuitacaoParcelaCartao, Data.Model.ArquivoQuitacaoParcelaCartao>
        {
            public ArquivoQuitacaoParcelaCartaoLoader()
            {
                Configure()
                    .Uid(f => f.IdArquivoQuitacaoParcelaCartao)
                    .Creator(f => new ArquivoQuitacaoParcelaCartao(f));
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public ArquivoQuitacaoParcelaCartao()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected ArquivoQuitacaoParcelaCartao(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.ArquivoQuitacaoParcelaCartao> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public ArquivoQuitacaoParcelaCartao(Data.Model.ArquivoQuitacaoParcelaCartao dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Id do arquivo Quitação Parcela de Cartão
        /// </summary>
        public int IdArquivoQuitacaoParcelaCartao
        {
            get { return DataModel.IdArquivoQuitacaoParcelaCartao; }
            set
            {
                if (DataModel.IdArquivoQuitacaoParcelaCartao != value &&
                    RaisePropertyChanging("IdArquivoQuitacaoParcelaCartao", value))
                {
                    DataModel.IdArquivoQuitacaoParcelaCartao = value;
                    RaisePropertyChanged("IdArquivoQuitacaoParcelaCartao");
                }
            }
        }

        /// <summary>
        /// Situação do Arquivo.
        /// </summary>
        public Data.Model.SituacaoArquivoQuitacaoParcelaCartao Situacao
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

        #endregion
    }
}

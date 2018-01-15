using System;
using System.Collections.Generic;

namespace Glass.Financeiro.Negocios.Entidades
{
    [Colosoft.Business.EntityLoader(typeof(QuitacaoParcelaCartaoLoader))]
    public class QuitacaoParcelaCartao : Glass.Negocios.Entidades.EntidadeBaseCadastro<Glass.Data.Model.QuitacaoParcelaCartao>
    {
        #region Tipos Aninhados

        class QuitacaoParcelaCartaoLoader : Colosoft.Business.EntityLoader<QuitacaoParcelaCartao, Glass.Data.Model.QuitacaoParcelaCartao>
        {
            public QuitacaoParcelaCartaoLoader()
            {
                Configure()
                    .Uid(f => f.IdQuitacaoParcelaCartao)
                    .Creator(f => new QuitacaoParcelaCartao(f));
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public QuitacaoParcelaCartao()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected QuitacaoParcelaCartao(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.QuitacaoParcelaCartao> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public QuitacaoParcelaCartao(Data.Model.QuitacaoParcelaCartao dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Id do Quitação Parcela de Cartão
        /// </summary>
        public int IdQuitacaoParcelaCartao
        {
            get { return DataModel.IdQuitacaoParcelaCartao; }
            set
            {
                if(DataModel.IdQuitacaoParcelaCartao != value &&
                    RaisePropertyChanging("IdQuitacaoParcelaCartao", value))
                {
                    DataModel.IdQuitacaoParcelaCartao = value;
                    RaisePropertyChanged("IdQuitacaoParcelaCartao");
                }
            }
        }

        /// <summary>
        /// Id do Arquivo Quitação Parcela de Cartão
        /// </summary>
        public int IdArquivoQuitacaoParcelaCartao
        {
            get { return DataModel.IdArquivoQuitacaoParcelaCartao; }
            set
            {
                if(DataModel.IdArquivoQuitacaoParcelaCartao != value &&
                    RaisePropertyChanging("IdArquivoQuitacaoParcelaCartao", IdArquivoQuitacaoParcelaCartao))
                {
                    DataModel.IdArquivoQuitacaoParcelaCartao = value;
                    RaisePropertyChanged("IdArquivoQuitacaoParcelaCartao");
                }
            }
        }

        /// <summary>
        /// Número de autorização do cartão
        /// </summary>
        public string NumAutCartao
        {
            get { return DataModel.NumAutCartao; }
            set
            {
                if(DataModel.NumAutCartao != value &&
                    RaisePropertyChanging("NumAutCartao", value))
                {
                    DataModel.NumAutCartao = value;
                    RaisePropertyChanged("NumAutCartao");
                }
            }
        }

        /// <summary>
        /// ùltimos Dígitos do Cartão
        /// </summary>
        public string UltimosDigitosCartao
        {
            get { return DataModel.UltimosDigitosCartao; }
            set
            {
                if(DataModel.UltimosDigitosCartao != value &&
                    RaisePropertyChanging("UltimosDigitosCartao", value))
                {
                    DataModel.UltimosDigitosCartao = value;
                    RaisePropertyChanged("UltimosDigitosCartao");
                }
            }
        }

        /// <summary>
        /// Valor da Parcela
        /// </summary>
        public decimal Valor
        {
            get { return DataModel.Valor; }
            set
            {
                if(DataModel.Valor != value &&
                    RaisePropertyChanging("Valor", value))
                {
                    DataModel.Valor = value;
                    RaisePropertyChanged("Valor");
                }
            }
        }

        /// <summary>
        /// Tipo do cartão (Debito/Credito)
        /// </summary>
        public Glass.Data.Model.TipoCartaoEnum Tipo
        {
            get { return DataModel.Tipo; }
            set
            {
                if(DataModel.Tipo != value &&
                    RaisePropertyChanging("Tipo", value))
                {
                    DataModel.Tipo = value;
                    RaisePropertyChanged("Tipo");
                }
            }
        }

        /// <summary>
        /// Bandeira do Cartão
        /// </summary>
        public int Bandeira
        {
            get { return DataModel.Bandeira; }
            set
            {
                if(DataModel.Bandeira != value &&
                    RaisePropertyChanging("Bandeira", value))
                {
                    DataModel.Bandeira = value;
                    RaisePropertyChanged("Bandeira");
                }
            }
        }

        /// <summary>
        /// Número da Parcela
        /// </summary>
        public int NumParcela
        {
            get { return DataModel.NumParcela; }
            set
            {
                if(DataModel.NumParcela != value &&
                    RaisePropertyChanging("NumParcela", value))
                {
                    DataModel.NumParcela = value;
                    RaisePropertyChanged("NumParcela");
                }
            }
        }

        /// <summary>
        /// Total de parcelas
        /// </summary>
        public int NumParcelaMax
        {
            get { return DataModel.NumParcelaMax; }
            set
            {
                if(DataModel.NumParcelaMax != value &&
                    RaisePropertyChanging("NumParcelaMax", value))
                {
                    DataModel.NumParcelaMax = value;
                    RaisePropertyChanged("NumParcelaMax");
                }
            }
        }
        
        /// <summary>
        /// Tarifa 
        /// </summary>
        public decimal Tarifa
        {
            get { return DataModel.Tarifa; }
            set
            {
                if(DataModel.Tarifa != value &&
                    RaisePropertyChanging("Tarifa", value))
                {
                    DataModel.Tarifa = value;
                    RaisePropertyChanged("Tarifa");
                }
            }
        }

        /// <summary>
        /// Identifica se o registro quitou sua parcela
        /// </summary>
        public bool Quitada
        {
            get { return DataModel.Quitada; }
            set
            {
                if (DataModel.Quitada != value &&
                    RaisePropertyChanging("Quitada", value))
                {
                    DataModel.Quitada = value;
                    RaisePropertyChanged("Quitada");
                }
            }
        }

        /// <summary>
        /// Data de vencimento da parcela
        /// </summary>
        public DateTime DataVencimento
        {
            get { return DataModel.DataVenc; }
            set
            {
                if(DataModel.DataVenc != value &&
                    RaisePropertyChanging("DataVenc", value))
                {
                    DataModel.DataVenc = value;
                    RaisePropertyChanged("DataVenc");
                }
            }
        }

        #endregion
    }
}

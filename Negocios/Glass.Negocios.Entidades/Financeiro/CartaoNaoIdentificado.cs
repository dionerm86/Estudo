using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Colosoft;

namespace Glass.Financeiro.Negocios.Entidades
{

    /// <summary>
    /// Representa a entidade de negócio do Cartão não identificado
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(CartaoNaoIdentificadoLoader))]
    public class CartaoNaoIdentificado : Glass.Negocios.Entidades.EntidadeBaseCadastro<Data.Model.CartaoNaoIdentificado>, ICartaoNaoIdentificado
    {
        #region Tipos Aninhados

        class CartaoNaoIdentificadoLoader : Colosoft.Business.EntityLoader<CartaoNaoIdentificado, Data.Model.CartaoNaoIdentificado>
        {
            public CartaoNaoIdentificadoLoader()
            {
                Configure()
                    .Uid(f => f.IdCartaoNaoIdentificado)
                    .Creator(f => new CartaoNaoIdentificado(f));
            }           
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public CartaoNaoIdentificado()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected CartaoNaoIdentificado(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.CartaoNaoIdentificado> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public CartaoNaoIdentificado(Data.Model.CartaoNaoIdentificado dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Id do cartão não identificado.
        /// </summary>
        public int IdCartaoNaoIdentificado
        {
            get { return DataModel.IdCartaoNaoIdentificado; }
            set
            {
                if (DataModel.IdCartaoNaoIdentificado != value &&
                    RaisePropertyChanging("IdCartaoNaoIdentificado", value))
                {
                    DataModel.IdCartaoNaoIdentificado = value;
                    RaisePropertyChanged("IdCartaoNaoIdentificado");
                }
            }
        }

        /// <summary>
        /// Id da conta bancária.
        /// </summary>
        public int IdContaBanco
        {
            get { return DataModel.IdContaBanco; }
            set
            {
                if (DataModel.IdContaBanco != value &&
                    RaisePropertyChanging("IdContaBanco", value))
                {
                    DataModel.IdContaBanco = value;
                    RaisePropertyChanged("IdContaBanco");
                }
            }
        }

        /// <summary>
        /// ValorMov.
        /// </summary>
        public decimal Valor
        {
            get { return DataModel.Valor; }
            set
            {
                if (DataModel.Valor != value &&
                    RaisePropertyChanging("Valor", value))
                {
                    DataModel.Valor = value;
                    RaisePropertyChanged("Valor");
                }
            }
        }

        /// <summary>
        /// Tipo Cartão.
        /// </summary>
        public int TipoCartao
        {
            get { return DataModel.TipoCartao; }
            set
            {
                if (DataModel.TipoCartao != value &&
                    RaisePropertyChanging("TipoCartao", value))
                {
                    DataModel.TipoCartao = value;
                    RaisePropertyChanged("TipoCartao");
                }
            }
        }

        /// <summary>
        /// Id do acerto Associado.
        /// </summary>
        public int? IdAcerto
        {
            get { return DataModel.IdAcerto; }
            set
            {
                if (DataModel.IdAcerto != value &&
                    RaisePropertyChanging("IdAcerto", value))
                {
                    DataModel.IdAcerto = value;
                    RaisePropertyChanged("IdAcerto");
                }
            }
        }

        /// <summary>
        /// Id da conta a receber Associada
        /// </summary>
        public int? IdContaR
        {
            get { return DataModel.IdContaR; }
            set
            {
                if (DataModel.IdContaR != value &&
                    RaisePropertyChanging("PropertyName", value))
                {
                    DataModel.IdContaR = value;
                    RaisePropertyChanged("PropertyName");
                }
            }
        }

        /// <summary>
        /// Id da Obra Associada.
        /// </summary>
        public int? IdObra
        {
            get { return DataModel.IdObra; }
            set
            {
                if (DataModel.IdObra != value &&
                    RaisePropertyChanging("IdObra", value))
                {
                    DataModel.IdObra = value;
                    RaisePropertyChanged("IdObra");
                }
            }
        }

        /// <summary>
        /// Id do sinal associado.
        /// </summary>
        public int? IdSinal
        {
            get { return DataModel.IdSinal; }
            set
            {
                if (DataModel.IdSinal != value &&
                    RaisePropertyChanging("IdSinal", value))
                {
                    DataModel.IdSinal = value;
                    RaisePropertyChanged("IdSinal");
                }
            }
        }

        /// <summary>
        /// Id da troca/devolução associada.
        /// </summary>
        public int? IdTrocaDevolucao
        {
            get { return DataModel.IdTrocaDevolucao; }
            set
            {
                if (DataModel.IdTrocaDevolucao != value &&
                    RaisePropertyChanging("IdTrocaDevolucao", value))
                {
                    DataModel.IdTrocaDevolucao = value;
                    RaisePropertyChanged("IdTrocaDevolucao");
                }
            }
        }

        /// <summary>
        /// Id da devolução de pagamento.
        /// </summary>
        public int? IdDevolucaoPagto
        {
            get { return DataModel.IdDevolucaoPagto; }
            set
            {
                if (DataModel.IdDevolucaoPagto != value &&
                    RaisePropertyChanging("IdDevolucaoPagto", value))
                {
                    DataModel.IdDevolucaoPagto = value;
                    RaisePropertyChanged("IdDevolucaoPagto");
                }
            }
        }

        /// <summary>
        /// Id do acerto de cheque associado.
        /// </summary>
        public int? IdAcertoCheque
        {
            get { return DataModel.IdAcertoCheque; }
            set
            {
                if (DataModel.IdAcertoCheque != value &&
                    RaisePropertyChanging("IdAcertoCheque", value))
                {
                    DataModel.IdAcertoCheque = value;
                    RaisePropertyChanged("IdAcertoCheque");
                }
            }
        }

        /// <summary>
        /// Id do Pedido.
        /// </summary>
        public int? IdPedido
        {
            get { return DataModel.IdPedido; }
            set
            {
                if (DataModel.IdPedido != value &&
                    RaisePropertyChanging("IdPedido", value))
                {
                    DataModel.IdPedido = value;
                    RaisePropertyChanged("IdPedido");
                }
            }
        }

        /// <summary>
        /// Id da liberação.
        /// </summary>
        public int? IdLiberarPedido
        {
            get { return DataModel.IdLiberarPedido; }
            set
            {
                if (DataModel.IdLiberarPedido != value &&
                    RaisePropertyChanging("IdLiberarPedido", value))
                {
                    DataModel.IdLiberarPedido = value;
                    RaisePropertyChanged("IdLiberarPedido");
                }
            }
        }

        /// <summary>
        /// Situação do cartão não identificado.
        /// </summary>  
        public Data.Model.SituacaoCartaoNaoIdentificado Situacao
        {
            get { return DataModel.Situacao; }
            set
            {
                if (DataModel.Situacao != value &&
                    RaisePropertyChanging("PropertyName", value))
                {
                    DataModel.Situacao = value;
                    RaisePropertyChanged("PropertyName");
                }
            }
        }

        /// <summary>
        /// Descrição.
        /// </summary>
        public string Observacao
        {
            get { return DataModel.Observacao; }
            set
            {
                if (DataModel.Observacao != value &&
                    RaisePropertyChanging("Observacao", value))
                {
                    DataModel.Observacao = value;
                    RaisePropertyChanged("Observacao");
                }
            }
        }

        /// <summary>
        /// Numero de autorização do cartão.
        /// </summary>
        public string NumAutCartao
        {
            get { return DataModel.NumAutCartao; }
            set
            {
                if (DataModel.NumAutCartao != value &&
                    RaisePropertyChanging("NumAutCartao", value))
                {
                    DataModel.NumAutCartao = value;
                    RaisePropertyChanged("NumAutCartao");
                }
            }
        }

        /// <summary>
        /// Indica se o CNI foi Importado.
        /// </summary>
        public bool Importado
        {
            get { return DataModel.Importado; }
            set
            {
                if (DataModel.Importado != value &&
                    RaisePropertyChanging("Importado", value))
                {
                    DataModel.Importado = value;
                    RaisePropertyChanged("Importado");
                }
            }
        }
       
        /// <summary>
        /// Data de cadastro do cartão não identificado
        /// </summary>
        public DateTime DataCad
        {
            get { return DataModel.DataCad; }
        }
               
        /// <summary>
        /// Numero de parcelas geradas.
        /// </summary>
        public int NumeroParcelas
        {
            get { return DataModel.NumeroParcelas; }
            set
            {
                if (DataModel.NumeroParcelas != value &&
                    RaisePropertyChanging("NumeroParcelas", value))
                {
                    DataModel.NumeroParcelas = value;
                    RaisePropertyChanged("NumeroParcelas");
                }
            }
        }

        /// <summary>
        /// Data de recebimento do CNI.
        /// </summary>
        public DateTime DataVenda
        {
            get { return DataModel.DataVenda; }
            set
            {
                if (DataModel.DataVenda != value &&
                    RaisePropertyChanging("DataVenda", value))
                {
                    DataModel.DataVenda = value;
                    RaisePropertyChanged("DataVenda");
                }
            }
        }

        /// <summary>
        /// Nº Estabelecimento.
        /// </summary>
        public string NumeroEstabelecimento
        {
            get { return DataModel.NumeroEstabelecimento; }
            set
            {
                if (DataModel.NumeroEstabelecimento != value &&
                    RaisePropertyChanging("NumeroEstabelecimento", value))
                {
                    DataModel.NumeroEstabelecimento = value;
                    RaisePropertyChanged("NumeroEstabelecimento");
                }
            }
        }

        /// <summary>
        /// Ultimos dígitos do cartão.
        /// </summary>
        public string UltimosDigitosCartao
        {
            get { return DataModel.UltimosDigitosCartao; }
            set
            {
                if (DataModel.UltimosDigitosCartao != value &&
                    RaisePropertyChanging("UltimosDigitosCartao", value))
                {
                    DataModel.UltimosDigitosCartao = value;
                    RaisePropertyChanged("UltimosDigitosCartao");
                }
            }
        }

        /// <summary>
        /// Id do arquivo que gerou o CNI
        /// </summary>
        public int? IdArquivoCartaoNaoIdentificado
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
        /// Recupera se o valor do cartão pode ser alterado
        /// </summary>
        public bool PodeEditarValor
        {
            get
            {
                return EditarValor(IdCartaoNaoIdentificado) && !Importado;
            }
        }

        /// <summary>
        /// Referência do cartão não identificado
        /// </summary>
        public string Referencia
        {
            get
            {
                return Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<IProvedorCartaoNaoIdentificado>().ObterReferencia(this);
            }
        }

        /// <summary>
        /// Indica que a operação foi feita pelo caixa diario
        /// </summary>
        public bool CxDiario { get; set; }

        #endregion

        #region Métodos Públicos



        /// <summary>
        /// Recupera informação se o valor pode ser editado
        /// </summary>
        /// <returns></returns>
        public static bool EditarValor(int idCartaoNaoIdentificado)
        {
            return Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<IProvedorCartaoNaoIdentificado>().EditarValor(idCartaoNaoIdentificado);
        }

        /// <summary>
        /// Salva os dados da entidade.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public override Colosoft.Business.SaveResult Save(Colosoft.Data.IPersistenceSession session)
        {
            if (!ExistsInStorage)
            {
                string msgErro;
                var retornoValidacao = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<IValidadorCartaoNaoIdentificado>().VerificarPodeInserir(NumAutCartao, TipoCartao, out msgErro);

                if (!retornoValidacao)
                    return new Colosoft.Business.SaveResult(false, msgErro.GetFormatter()); 

                Situacao = Data.Model.SituacaoCartaoNaoIdentificado.Ativo;                
            }

            return base.Save(session);
        }
        
        #endregion
    }
}

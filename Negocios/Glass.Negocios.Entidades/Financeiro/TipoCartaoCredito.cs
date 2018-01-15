using System;
using Colosoft;
using System.Collections.Generic;

namespace Glass.Financeiro.Negocios.Entidades
{
    /// <summary>
    /// Representa a model TipoCartaoCredito
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(TipoCartaoCreditoLoader))]
    public class TipoCartaoCredito : Colosoft.Business.Entity<Glass.Data.Model.TipoCartaoCredito>
    {
        #region Tipos Aninhados

        class TipoCartaoCreditoLoader : Colosoft.Business.EntityLoader<TipoCartaoCredito, Glass.Data.Model.TipoCartaoCredito>
        {
            public TipoCartaoCreditoLoader()
            {
                Configure()
                    .Uid(f => f.IdTipoCartao)
                    .Description(f=> string.Concat(f.Tipo.Translate().Format()))
                    .Reference<Financeiro.Negocios.Entidades.BandeiraCartao, Data.Model.BandeiraCartao>("BandeiraCartao", f => f.BandeiraCartao, f => f.Bandeira)
                    .Reference<Financeiro.Negocios.Entidades.OperadoraCartao, Data.Model.OperadoraCartao>("OperadoraCartao", f => f.OperadoraCartao, f => f.Operadora)
                    .Child<JurosParcelaCartao, Glass.Data.Model.JurosParcelaCartao>("JurosParcelas", f => f.JurosParcelas, f => f.IdTipoCartao)
                    .Creator(f => new TipoCartaoCredito(f));
            }
        }

        #endregion

        #region Variáveis Locais

        private Colosoft.Business.IEntityChildrenList<JurosParcelaCartao> _jurosParcelas;

        #endregion

        #region Propriedades

        /// <summary>
        /// Juros das parcelas do tipo de cartão.
        /// </summary>
        public Colosoft.Business.IEntityChildrenList<JurosParcelaCartao> JurosParcelas
        {
            get { return _jurosParcelas; }
        }

        /// <summary>
        /// Identificador do cartão.
        /// </summary>       
        public int IdTipoCartao
        {
            get { return DataModel.IdTipoCartao; }
            set
            {
                if (DataModel.IdTipoCartao != value &&
                    RaisePropertyChanging("IdTipoCartao", value))
                {
                    DataModel.IdTipoCartao = value;
                    RaisePropertyChanged("IdTipoCartao");
                }
            }
        }

        /// <summary>
        /// Numero de Parcelas.
        /// </summary>
        public int NumParc
        {
            get { return DataModel.NumParc; }
            set
            {
                if (DataModel.NumParc != value &&
                    RaisePropertyChanging("NumParc", value))
                {
                    DataModel.NumParc = value;
                    RaisePropertyChanged("NumParc");
                }
            }
        }

        /// <summary>
        /// Operadora.
        /// </summary>
        public uint Operadora
        {
            get { return DataModel.Operadora; }
            set
            {
                if (DataModel.Operadora != value &&
                    RaisePropertyChanging("Operadora", value))
                {
                    DataModel.Operadora = value;
                    RaisePropertyChanged("Operadora");
                }
            }
        }

        /// <summary>
        /// Bandeira.
        /// </summary>
        public uint Bandeira
        {
            get { return DataModel.Bandeira; }
            set
            {
                if (DataModel.Bandeira != value &&
                    RaisePropertyChanging("Bandeira", value))
                {
                    DataModel.Bandeira = value;
                    RaisePropertyChanged("Bandeira");
                }
            }
        }

        /// <summary>
        /// Tipo.
        /// </summary>
        public Data.Model.TipoCartaoEnum Tipo
        {
            get { return DataModel.Tipo; }
            set
            {
                if (DataModel.Tipo != value &&
                    RaisePropertyChanging("Tipo", value))
                {
                    DataModel.Tipo = value;
                    RaisePropertyChanged("Tipo");
                }
            }
        }

        /// <summary>
        /// Identificador da conta func.
        /// </summary>
        public int IdContaFunc
        {
            get { return DataModel.IdContaFunc; }
            set
            {
                if (DataModel.IdContaFunc != value &&
                    RaisePropertyChanging("IdContaFunc", value))
                {
                    DataModel.IdContaFunc = value;
                    RaisePropertyChanged("IdContaFunc");
                }
            }
        }

        /// <summary>
        /// Identificador da conta entrada.
        /// </summary>
        public int IdContaEntrada
        {
            get { return DataModel.IdContaEntrada; }
            set
            {
                if (DataModel.IdContaEntrada != value &&
                    RaisePropertyChanging("IdContaEntrada", value))
                {
                    DataModel.IdContaEntrada = value;
                    RaisePropertyChanged("IdContaEntrada");
                }
            }
        }

        /// <summary>
        /// Identificador da conta estorno.
        /// </summary>
        public int IdContaEstorno
        {
            get { return DataModel.IdContaEstorno; }
            set
            {
                if (DataModel.IdContaEstorno != value &&
                    RaisePropertyChanging("IdContaEstorno", value))
                {
                    DataModel.IdContaEstorno = value;
                    RaisePropertyChanged("IdContaEstorno");
                }
            }
        }

        /// <summary>
        /// Identificador da conta estorno recebimento prazo.
        /// </summary>
        public int IdContaEstornoRecPrazo
        {
            get { return DataModel.IdContaEstornoRecPrazo; }
            set
            {
                if (DataModel.IdContaEstornoRecPrazo != value &&
                    RaisePropertyChanging("IdContaEstornoRecPrazo", value))
                {
                    DataModel.IdContaEstornoRecPrazo = value;
                    RaisePropertyChanged("IdContaEstornoRecPrazo");
                }
            }
        }

        /// <summary>
        /// Identificador da conta estorno entrada.
        /// </summary>
        public int IdContaEstornoEntrada
        {
            get { return DataModel.IdContaEstornoEntrada; }
            set
            {
                if (DataModel.IdContaEstornoEntrada != value &&
                    RaisePropertyChanging("IdContaEstornoEntrada", value))
                {
                    DataModel.IdContaEstornoEntrada = value;
                    RaisePropertyChanged("IdContaEstornoEntrada");
                }
            }
        }

        /// <summary>
        /// Identificador da conta estorno cheque devolvido.
        /// </summary>
        public int IdContaEstornoChequeDev
        {
            get { return DataModel.IdContaEstornoChequeDev; }
            set
            {
                if (DataModel.IdContaEstornoChequeDev != value &&
                    RaisePropertyChanging("IdContaEstornoChequeDev", value))
                {
                    DataModel.IdContaEstornoChequeDev = value;
                    RaisePropertyChanged("IdContaEstornoChequeDev");
                }
            }
        }

        /// <summary>
        /// Identificador da conta devolução pagamento.
        /// </summary>
        public int IdContaDevolucaoPagto
        {
            get { return DataModel.IdContaDevolucaoPagto; }
            set
            {
                if (DataModel.IdContaDevolucaoPagto != value &&
                    RaisePropertyChanging("IdContaDevolucaoPagto", value))
                {
                    DataModel.IdContaDevolucaoPagto = value;
                    RaisePropertyChanged("IdContaDevolucaoPagto");
                }
            }
        }

        /// <summary>
        /// identificador da conta estorno devolução pagamento.
        /// </summary>
        public int IdContaEstornoDevolucaoPagto
        {
            get { return DataModel.IdContaEstornoDevolucaoPagto; }
            set
            {
                if (DataModel.IdContaEstornoDevolucaoPagto != value &&
                    RaisePropertyChanging("IdContaEstornoDevolucaoPagto", value))
                {
                    DataModel.IdContaEstornoDevolucaoPagto = value;
                    RaisePropertyChanged("IdContaEstornoDevolucaoPagto");
                }
            }
        }

        /// <summary>
        /// identificador da conta a vista.
        /// </summary>
        public int IdContaVista
        {
            get { return DataModel.IdContaVista; }
            set
            {
                if (DataModel.IdContaVista != value &&
                    RaisePropertyChanging("IdContaVista", value))
                {
                    DataModel.IdContaVista = value;
                    RaisePropertyChanged("IdContaVista");
                }
            }
        }

        /// <summary>
        /// Identificador da conta recebimento a prazo.
        /// </summary>
        public int IdContaRecPrazo
        {
            get { return DataModel.IdContaRecPrazo; }
            set
            {
                if (DataModel.IdContaRecPrazo != value &&
                    RaisePropertyChanging("IdContaRecPrazo", value))
                {
                    DataModel.IdContaRecPrazo = value;
                    RaisePropertyChanged("IdContaRecPrazo");
                }
            }
        }

        /// <summary>
        /// Identificador da conta recebimento cheque devolução
        /// </summary>
        public int IdContaRecChequeDev
        {
            get { return DataModel.IdContaRecChequeDev; }
            set
            {
                if (DataModel.IdContaRecChequeDev != value &&
                    RaisePropertyChanging("IdContaRecChequeDev", value))
                {
                    DataModel.IdContaRecChequeDev = value;
                    RaisePropertyChanged("IdContaRecChequeDev");
                }
            }
        }

        public List<int> IdsContasRecebimento
        {
            get
            {
                 return new List<int>()
                {
                    IdContaEntrada,
                    IdContaRecPrazo,
                    IdContaVista,
                    IdContaRecChequeDev,
                    IdContaDevolucaoPagto
                };
            }
        }

        public List<int> IdsContasEstorno
        {
            get
            {
                return new List<int>()
                {
                    IdContaEstorno,
                    IdContaEstornoRecPrazo,
                    IdContaEstornoEntrada,
                    IdContaEstornoChequeDev,
                    IdContaEstornoDevolucaoPagto
                };
            }
        }

        /// <summary>
        /// Concatena as traduções e exibe como descrição
        /// </summary>
        public string Descricao
        {
            get { return OperadoraCartao.Descricao + " " + BandeiraCartao.Descricao + " " + Tipo.Translate().Format(); }
        }

        public bool VerificarPodeExcluir
        {
            get
            {
                return !Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<IProvedorTipoCartaoCredito>()
                   .VerificarTipoCartaoCreditoUso(IdTipoCartao);
            }
        }

        /// <summary>
        /// Bandeira Cartão asssociado.
        /// </summary>
        public Financeiro.Negocios.Entidades.BandeiraCartao BandeiraCartao
        {
            get { return GetReference<Financeiro.Negocios.Entidades.BandeiraCartao>("BandeiraCartao", true); }
        }

        /// <summary>
        /// Operadora Cartão asssociado.
        /// </summary>
        public Financeiro.Negocios.Entidades.OperadoraCartao OperadoraCartao
        {
            get { return GetReference<Financeiro.Negocios.Entidades.OperadoraCartao>("OperadoraCartao", true); }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public TipoCartaoCredito()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected TipoCartaoCredito(Colosoft.Business.EntityLoaderCreatorArgs<Glass.Data.Model.TipoCartaoCredito> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {
            _jurosParcelas = GetChild<JurosParcelaCartao>(args.Children, "JurosParcelas");
        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public TipoCartaoCredito(Glass.Data.Model.TipoCartaoCredito dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {
            _jurosParcelas = CreateChild<Colosoft.Business.IEntityChildrenList<JurosParcelaCartao>>("JurosParcelas");
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Verifica se o plano informado esta associado ao tipo de cartão
        /// </summary>
        public bool VerificarPossuiPlanoConta(uint idConta)
        {
            return
                IdContaFunc == idConta || IdContaEntrada == idConta || IdContaEstorno == idConta ||
                IdContaEstornoRecPrazo == idConta || IdContaEstornoEntrada == idConta || IdContaEstornoChequeDev == idConta ||
                IdContaDevolucaoPagto == idConta || IdContaEstornoDevolucaoPagto == idConta || IdContaVista == idConta ||
                IdContaRecPrazo == idConta || IdContaRecChequeDev == idConta;
        }

        /// <summary>
        /// Obtem a conta de estorno da conta informada
        /// </summary>
        /// <param name="idConta"></param>
        /// <returns></returns>
        public uint ObterContaEstorno(uint idConta)
        {
            if (IdContaEntrada == idConta)
                return (uint)IdContaEstornoEntrada;
            else if (IdContaRecPrazo == idConta)
                return (uint)IdContaEstornoRecPrazo;
            else if (IdContaVista == idConta)
                return (uint)IdContaEstorno;
            else if (IdContaDevolucaoPagto == idConta)
                return (uint)IdContaEstornoDevolucaoPagto;
            else if (IdContaRecChequeDev == idConta)
                return (uint)IdContaEstornoChequeDev;
            else
                throw new Exception("Plano de conta de estorno não existente.");
        }

        /// <summary>
        /// Calcula o do juros a ser cobrado.
        /// </summary>
        /// <param name="idLoja"></param>
        /// <param name="numeroParcelas"></param>
        /// <param name="valor"></param>
        /// <returns></returns>
        public decimal CalcularValorJuros(int? idLoja, int numeroParcelas, decimal valor)
        {
            decimal retorno = 0;
            decimal taxaJuros = (decimal)CalcularTaxaJuros(idLoja, numeroParcelas);

            if (!Glass.Configuracoes.FinanceiroConfig.Cartao.CobrarJurosCartaoCliente)
                retorno = valor * (taxaJuros / 100);
            else
            {
                decimal valorParcela = Math.Round(valor * (1 / (1 + (taxaJuros / 100))), 2);
                retorno = valor - valorParcela;
            }

            return Math.Round(retorno, 2);
        }

        /// <summary>
        /// Calcula o juros com base na loja e no número de parcelas.
        /// </summary>
        /// <param name="idLoja"></param>
        /// <param name="numeroParcelas"></param>
        /// <returns></returns>
        public decimal CalcularTaxaJuros(int? idLoja, int numeroParcelas)
        {
            return Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<IProvedorJuros>()
                .CalcularTaxaJuros(this, idLoja, numeroParcelas);
        }

        #endregion
    }
}

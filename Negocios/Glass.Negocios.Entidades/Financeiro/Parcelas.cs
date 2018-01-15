using Colosoft;
using System;
using System.Linq;

namespace Glass.Financeiro.Negocios.Entidades
{
    /// <summary>
    /// Assinatura do validador da entidades de parcelas.
    /// </summary>
    public interface IValidadorParcelas
    {
        /// <summary>
        /// Valida a existencia da instancia informada.
        /// </summary>
        /// <param name="parcelas"></param>
        /// <returns></returns>
        IMessageFormattable[] ValidaExistencia(Parcelas parcelas);

        /// <summary>
        /// Valida a situacação da Parcela antes de Inativa-lá
        /// </summary>
        /// <param name="parcelas"></param>
        /// <returns></returns>
        IMessageFormattable[] ValidaSituacao(int idParcela);

    }

    /// <summary>
    /// Represenrta a entidade de negócio das parcelas.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(ParcelasLoader))]
    public class Parcelas : Colosoft.Business.Entity<Glass.Data.Model.Parcelas>
    {
        #region Tipos Aninhados

        class ParcelasLoader : Colosoft.Business.EntityLoader<Parcelas, Data.Model.Parcelas>
        {
            public ParcelasLoader()
            {
                Configure()
                    .Uid(f => f.IdParcela)
                    .FindName(new ParcelasFindNameConverter(), f => f.Descricao, f => f.NumParcelas)
                    .Creator(f => new Parcelas(f));
            }
        }

        /// <summary>
        /// Implementação do conversor do FindName da parcela.
        /// </summary>
        class ParcelasFindNameConverter : IFindNameConverter
        {
            public string Convert(object[] baseInfo)
            {
                var descricao = (string)baseInfo[0];
                var numParcelas = (int)baseInfo[1];

                string descricaoParcelas = null;

                if (numParcelas == 0)
                    descricaoParcelas = "À vista";
                else if (numParcelas == 1)
                    descricaoParcelas = "1 parcela";
                else
                    descricaoParcelas = numParcelas + " parcelas";

                return descricaoParcelas + (numParcelas > 0 || String.Compare(descricao, descricaoParcelas, true) != 0 ? " - " + descricao : "");
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador da parcela.
        /// </summary>
        public int IdParcela
        {
            get { return DataModel.IdParcela; }
            set
            {
                if (DataModel.IdParcela != value &&
                    RaisePropertyChanging("IdParcela", value))
                {
                    DataModel.IdParcela = value;
                    RaisePropertyChanged("IdParcela");
                }
            }
        }

        /// <summary>
        /// Número de parcelas.
        /// </summary>
        public int NumParcelas
        {
            get { return DataModel.NumParcelas; }
            set
            {
                if (DataModel.NumParcelas != value &&
                    RaisePropertyChanging("NumParcelas", value))
                {
                    DataModel.NumParcelas = value;
                    RaisePropertyChanged("NumParcelas");
                }
            }
        }

        /// <summary>
        /// Dias.
        /// </summary>
        public string Dias
        {
            get { return DataModel.Dias; }
            set
            {
                if (DataModel.Dias != value &&
                    RaisePropertyChanging("Dias", value))
                {
                    DataModel.Dias = value;
                    RaisePropertyChanged("Dias");
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
                    RaisePropertyChanging("Descricao", value))
                {
                    DataModel.Descricao = value;
                    RaisePropertyChanged("Descricao");
                }
            }
        }

        /// <summary>
        /// Identifica se é a parcela padrão.
        /// </summary>
        public bool ParcelaPadrao
        {
            get { return DataModel.ParcelaPadrao; }
            set
            {
                if (DataModel.ParcelaPadrao != value &&
                    RaisePropertyChanging("ParcelaPadrao", value))
                {
                    DataModel.ParcelaPadrao = value;
                    RaisePropertyChanged("ParcelaPadrao");
                }
            }
        }

        /// <summary>
        /// Identifica se é a parcela a vista.
        /// </summary>
        public bool ParcelaAVista
        {
            get { return DataModel.ParcelaAVista; }
            set
            {
                if (DataModel.ParcelaAVista != value &&
                    RaisePropertyChanging("ParcelaAVista", value))
                {
                    DataModel.ParcelaAVista = value;
                    RaisePropertyChanged("ParcelaAVista");
                }
            }
        }

        /// <summary>
        /// Desconto.
        /// </summary>
        public Decimal Desconto
        {
            get { return DataModel.Desconto; }
            set
            {
                if (DataModel.Desconto != value &&
                    RaisePropertyChanging("Desconto", value))
                {
                    DataModel.Desconto = value;
                    RaisePropertyChanged("Desconto");
                }
            }
        }

        /// <summary>
        /// Situacao.
        /// </summary>
        public Situacao Situacao
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

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public Parcelas()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected Parcelas(Colosoft.Business.EntityLoaderCreatorArgs<Glass.Data.Model.Parcelas> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public Parcelas(Glass.Data.Model.Parcelas dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Recupera o nome que representa a instancia.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Loader != null && Loader.FindNameConverter != null)
                return Loader.FindNameConverter.Convert(new object[] { Descricao, NumParcelas });

            return base.ToString();
        }

        /// <summary>
        /// Apaga os dados da instancia.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public override Colosoft.Business.DeleteResult Delete(Colosoft.Data.IPersistenceSession session)
        {
            var validador = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<IValidadorParcelas>();

            var resultadoValidacao = validador.ValidaExistencia(this);

            if (resultadoValidacao.Length > 0)
                return new Colosoft.Business.DeleteResult(false, resultadoValidacao.Join(" "));

            // Apaga as parcelas não usar associadas
            session.Delete<Glass.Data.Model.ParcelasNaoUsar>(
                Colosoft.Query.ConditionalContainer
                    .Parse("IdParcela=?idParcela")
                    .Add("?idParcela", IdParcela));

            return base.Delete(session);
        }

        /// <summary>
        /// Salva os dados da instancia.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public override Colosoft.Business.SaveResult Save(Colosoft.Data.IPersistenceSession session)
        {
            var resultado = base.Save(session);

            if (ExistsInStorage && Situacao == Situacao.Inativo)
            {
                var resultadoValidacao = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<IValidadorParcelas>().ValidaSituacao(IdParcela);

                if (resultadoValidacao.Length > 0)
                    return new Colosoft.Business.SaveResult(false, resultadoValidacao.Join(" "));
            }

            if (resultado && !ExistsInStorage)
            {
                // Recuper ao provedor de parcelas não usar
                var provedorParcelasNaoUsar = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<IProvedorParcelasNaoUsar>();

                // Cria a relação de parcelas não usar
                var naoUsar = provedorParcelasNaoUsar.CriarParcelasNaoUsar(this);

                foreach (var i in naoUsar)
                {
                    // Salva a parcela não usar
                    var resultado2 = i.Save(session);
                    if (!resultado2)
                        return resultado2;
                }
            }

            return resultado;
        }

        #endregion
    }
}

using Colosoft;
using System.Linq;

namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Assinatura do validador da rota.
    /// </summary>
    public interface IValidadorRota
    {
        /// <summary>
        /// Valida a atualização da rota.
        /// </summary>
        /// <param name="rota"></param>
        /// <returns></returns>
        IMessageFormattable[] ValidaAtualizacao(Rota rota);

        /// <summary>
        /// Valida a existencia da rota.
        /// </summary>
        /// <param name="rota"></param>
        /// <returns></returns>
        IMessageFormattable[] ValidaExistencia(Rota rota);
    }

    /// <summary>
    /// Representa a entidade de negócio das rotas.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(RotaLoader))]
    [Glass.Negocios.ControleAlteracao(Data.Model.LogAlteracao.TabelaAlteracao.Rota)]
    public class Rota : Glass.Negocios.Entidades.EntidadeBaseCadastro<Data.Model.Rota>
    {
        #region Tipos aninhados

        class RotaLoader : Colosoft.Business.EntityLoader<Rota, Data.Model.Rota>
        {
            public RotaLoader()
            {
                Configure()
                    .Uid(f => f.IdRota)
                    .FindName(f => f.Descricao)
                    .Child<RotaCliente, Data.Model.RotaCliente>("Clientes", f => f.Clientes, f => f.IdRota)
                    .Creator(f => new Rota(f));
            }
        }

        #endregion

        #region Variáveis locais

        private Colosoft.Business.IEntityChildrenList<RotaCliente> _clientes;

        #endregion

        #region Propriedades

        /// <summary>
        /// Código da rota.
        /// </summary>
        public int IdRota
        {
            get { return DataModel.IdRota; }
            set
            {
                if (DataModel.IdRota != value &&
                    RaisePropertyChanging("IdRota", value))
                {
                    DataModel.IdRota = value;
                    RaisePropertyChanged("IdRota");
                }
            }
        }

        /// <summary>
        /// Código interno da rota.
        /// </summary>
        public string CodInterno
        {
            get { return DataModel.CodInterno; }
            set
            {
                if (DataModel.CodInterno != value &&
                    RaisePropertyChanging("CodInterno", value))
                {
                    DataModel.CodInterno = value;
                    RaisePropertyChanged("CodInterno");
                }
            }
        }

        /// <summary>
        /// Descrição da rota.
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
        /// Situação da rota.
        /// </summary>
        public Situacao Situacao
        {
            get { return (Situacao)DataModel.Situacao; }
            set
            {
                if (DataModel.Situacao != (int)value &&
                    RaisePropertyChanging("Situacao", value))
                {
                    DataModel.Situacao = (int)value;
                    RaisePropertyChanged("Situacao");
                }
            }
        }

        /// <summary>
        /// Distância percorrida na rota.
        /// </summary>
        public int Distancia
        {
            get { return DataModel.Distancia; }
            set
            {
                if (DataModel.Distancia != value &&
                    RaisePropertyChanging("Distancia", value))
                {
                    DataModel.Distancia = value;
                    RaisePropertyChanged("Distancia");
                }
            }
        }

        /// <summary>
        /// Observações sobre a rota.
        /// </summary>
        public string Obs
        {
            get { return DataModel.Obs; }
            set
            {
                if (DataModel.Obs != value &&
                    RaisePropertyChanging("Obs", value))
                {
                    DataModel.Obs = value;
                    RaisePropertyChanged("Obs");
                }
            }
        }

        /// <summary>
        /// Dias da semana que a rota ocorre.
        /// </summary>
        public Data.Model.DiasSemana DiasSemana
        {
            get { return DataModel.DiasSemana; }
            set
            {
                if (DataModel.DiasSemana != value &&
                    RaisePropertyChanging("DiasSemana", value))
                {
                    DataModel.DiasSemana = value;
                    RaisePropertyChanged("DiasSemana");
                }
            }
        }

        /// <summary>
        /// Número mínimo de dias para entrega na rota.
        /// </summary>
        public int NumeroMinimoDiasEntrega
        {
            get { return DataModel.NumeroMinimoDiasEntrega; }
            set
            {
                if (DataModel.NumeroMinimoDiasEntrega != value &&
                    RaisePropertyChanging("NumeroMinimoDiasEntrega", value))
                {
                    DataModel.NumeroMinimoDiasEntrega = value;
                    RaisePropertyChanged("NumeroMinimoDiasEntrega");
                }
            }
        }

        /// <summary>
        /// A rota deve ser entregue no balcão?
        /// </summary>
        public bool EntregaBalcao
        {
            get { return DataModel.EntregaBalcao; }
            set
            {
                if (DataModel.EntregaBalcao != value &&
                    RaisePropertyChanging("EntregaBalcao", value))
                {
                    DataModel.EntregaBalcao = value;
                    RaisePropertyChanged("EntregaBalcao");
                }
            }
        }

        #endregion

        #region Propriedades referenciadas/filhos

        /// <summary>
        /// Clientes associados à rota.
        /// </summary>
        public Colosoft.Business.IEntityChildrenList<RotaCliente> Clientes
        {
            get { return _clientes; }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public Rota()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected Rota(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.Rota> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {
            _clientes = GetChild<RotaCliente>(args.Children, "Clientes");
        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public Rota(Data.Model.Rota dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Salva os dados da rota.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public override Colosoft.Business.SaveResult Save(Colosoft.Data.IPersistenceSession session)
        {
            var validador = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<IValidadorRota>();

            var resultadoValidacao = validador.ValidaAtualizacao(this);

            if (resultadoValidacao.Length > 0)
                return new Colosoft.Business.SaveResult(false, resultadoValidacao.Join(" "));

            return base.Save(session);
        }

        /// <summary>
        /// Método acionado para apagar rota.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public override Colosoft.Business.DeleteResult Delete(Colosoft.Data.IPersistenceSession session)
        {
            var validador = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<IValidadorRota>();

            var resultadoValidacao = validador.ValidaExistencia(this);

            if (resultadoValidacao.Length > 0)
                return new Colosoft.Business.DeleteResult(false, resultadoValidacao.Join(" "));

            return base.Delete(session);
        }

        #endregion
    }
}

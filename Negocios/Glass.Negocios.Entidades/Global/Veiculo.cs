using Colosoft;
using System.Linq;

namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Assinatura do validador de veículo.
    /// </summary>
    public interface IValidadorVeiculo
    {
        /// <summary>
        /// Valida a existencia do veículo.
        /// </summary>
        /// <param name="cliente"></param>
        /// <returns></returns>
        IMessageFormattable[] ValidaExistencia(Veiculo veiculo);
    }

    /// <summary>
    /// Representa a entidade de negócio do veículo.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(VeiculoLoader))]
    public class Veiculo : Glass.Negocios.Entidades.EntidadeBaseCadastro<Data.Model.Veiculo>
    {
        #region Tipos Aninhados

        class VeiculoLoader : Colosoft.Business.EntityLoader<Veiculo, Data.Model.Veiculo>
        {
            public VeiculoLoader()
            {
                Configure()
                    .Keys(f => f.Placa)
                    .FindName(f => f.Placa)
                    .Creator(f => new Veiculo(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Número da placa do veículo.
        /// </summary>
        public string Placa
        {
            get { return DataModel.Placa; }
            set
            {
                if (DataModel.Placa != value &&
                    RaisePropertyChanging("Placa", value))
                {
                    DataModel.Placa = value;
                    RaisePropertyChanged("Placa");
                }
            }
        }

        /// <summary>
        /// Modelo.
        /// </summary>
        public string Modelo
        {
            get { return DataModel.Modelo; }
            set
            {
                if (DataModel.Modelo != value &&
                    RaisePropertyChanging("Modelo", value))
                {
                    DataModel.Modelo = value;
                    RaisePropertyChanged("Modelo");
                }
            }
        }

        /// <summary>
        /// Ano de fabricação.
        /// </summary>
        public int Anofab
        {
            get { return DataModel.Anofab; }
            set
            {
                if (DataModel.Anofab != value &&
                    RaisePropertyChanging("Anofab", value))
                {
                    DataModel.Anofab = value;
                    RaisePropertyChanged("Anofab");
                }
            }
        }

        /// <summary>
        /// Cor.
        /// </summary>
        public string Cor
        {
            get { return DataModel.Cor; }
            set
            {
                if (DataModel.Cor != value &&
                    RaisePropertyChanging("Cor", value))
                {
                    DataModel.Cor = value;
                    RaisePropertyChanged("Cor");
                }
            }
        }

        /// <summary>
        /// Km inicial.
        /// </summary>
        public int Kminicial
        {
            get { return DataModel.Kminicial; }
            set
            {
                if (DataModel.Kminicial != value &&
                    RaisePropertyChanging("Kminicial", value))
                {
                    DataModel.Kminicial = value;
                    RaisePropertyChanged("Kminicial");
                }
            }
        }

        /// <summary>
        /// Valor IPVA.
        /// </summary>
        public float Valoripva
        {
            get { return DataModel.Valoripva; }
            set
            {
                if (DataModel.Valoripva != value &&
                    RaisePropertyChanging("Valoripva", value))
                {
                    DataModel.Valoripva = value;
                    RaisePropertyChanged("Valoripva");
                }
            }
        }

        /// <summary>
        /// Chassi.
        /// </summary>
        public string Chassi
        {
            get { return DataModel.Chassi; }
            set
            {
                if (DataModel.Chassi != value &&
                    RaisePropertyChanging("Chassi", value))
                {
                    DataModel.Chassi = value;
                    RaisePropertyChanged("Chassi");
                }
            }
        }

        /// <summary>
        /// Código da cor na montadora.
        /// </summary>
        public string CodCorMontadora
        {
            get { return DataModel.CodCorMontadora; }
            set
            {
                if (DataModel.CodCorMontadora != value &&
                    RaisePropertyChanging("CodCorMontadora", value))
                {
                    DataModel.CodCorMontadora = value;
                    RaisePropertyChanged("CodCorMontadora");
                }
            }
        }

        /// <summary>
        /// Código do modelo do ranavam.
        /// </summary>
        public string CodModeloRenavam
        {
            get { return DataModel.CodModeloRenavam; }
            set
            {
                if (DataModel.CodModeloRenavam != value &&
                    RaisePropertyChanging("CodModeloRenavam", value))
                {
                    DataModel.CodModeloRenavam = value;
                    RaisePropertyChanged("CodModeloRenavam");
                }
            }
        }

        /// <summary>
        /// Valor unitário.
        /// </summary>
        public decimal ValorUnitario
        {
            get { return DataModel.ValorUnitario; }
            set
            {
                if (DataModel.ValorUnitario != value &&
                    RaisePropertyChanging("ValorUnitario", value))
                {
                    DataModel.ValorUnitario = value;
                    RaisePropertyChanged("ValorUnitario");
                }
            }
        }

        /// <summary>
        /// Tara.
        /// </summary>
        public float Tara
        {
            get { return DataModel.Tara; }
            set
            {
                if (DataModel.Tara != value &&
                    RaisePropertyChanging("Tara", value))
                {
                    DataModel.Tara = value;
                    RaisePropertyChanged("Tara");
                }
            }
        }

        /// <summary>
        /// Capacidade KG.
        /// </summary>
        public float CapacidadeKg
        {
            get { return DataModel.CapacidadeKg; }
            set
            {
                if (DataModel.CapacidadeKg != value &&
                    RaisePropertyChanging("CapacidadeKg", value))
                {
                    DataModel.CapacidadeKg = value;
                    RaisePropertyChanged("CapacidadeKg");
                }
            }
        }

        /// <summary>
        /// Capacidade em M³.
        /// </summary>
        public float CapacidadeM3
        {
            get { return DataModel.CapacidadeM3; }
            set
            {
                if (DataModel.CapacidadeM3 != value &&
                    RaisePropertyChanging("CapacidadeM3", value))
                {
                    DataModel.CapacidadeM3 = value;
                    RaisePropertyChanged("CapacidadeM3");
                }
            }
        }

        /// <summary>
        /// Tipo de proprietário.
        /// </summary>
        public int TipoProprietario
        {
            get { return DataModel.TipoProprietario; }
            set
            {
                if (DataModel.TipoProprietario != value &&
                    RaisePropertyChanging("TipoProprietario", value))
                {
                    DataModel.TipoProprietario = value;
                    RaisePropertyChanged("TipoProprietario");
                }
            }
        }

        /// <summary>
        /// Tipo de veículo.
        /// </summary>
        public int TipoVeiculo
        {
            get { return DataModel.TipoVeiculo; }
            set
            {
                if (DataModel.TipoVeiculo != value &&
                    RaisePropertyChanging("TipoVeiculo", value))
                {
                    DataModel.TipoVeiculo = value;
                    RaisePropertyChanged("TipoVeiculo");
                }
            }
        }

        /// <summary>
        /// Tipo rodado.
        /// </summary>
        public int TipoRodado
        {
            get { return DataModel.TipoRodado; }
            set
            {
                if (DataModel.TipoRodado != value &&
                    RaisePropertyChanging("TipoRodado", value))
                {
                    DataModel.TipoRodado = value;
                    RaisePropertyChanged("TipoRodado");
                }
            }
        }

        /// <summary>
        /// Tipo carroceria.
        /// </summary>
        public int TipoCarroceria
        {
            get { return DataModel.TipoCarroceria; }
            set
            {
                if (DataModel.TipoCarroceria != value &&
                    RaisePropertyChanging("TipoCarroceria", value))
                {
                    DataModel.TipoCarroceria = value;
                    RaisePropertyChanged("TipoCarroceria");
                }
            }
        }

        /// <summary>
        /// UF.
        /// </summary>
        public string UfLicenc
        {
            get { return DataModel.UfLicenc; }
            set
            {
                if (DataModel.UfLicenc != value &&
                    RaisePropertyChanging("UfLicenc", value))
                {
                    DataModel.UfLicenc = value;
                    RaisePropertyChanged("UfLicenc");
                }
            }
        }

        /// <summary>
        /// Renavam.
        /// </summary>
        public string Renavam
        {
            get { return DataModel.Renavam; }
            set
            {
                if (DataModel.Renavam != value &&
                    RaisePropertyChanging("Renavam", value))
                {
                    DataModel.Renavam = value;
                    RaisePropertyChanged("Renavam");
                }
            }
        }

        /// <summary>
        /// situação.
        /// </summary>
        public Glass.Situacao Situacao
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
        public Veiculo()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected Veiculo(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.Veiculo> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public Veiculo(Data.Model.Veiculo dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion

        #region Métodos sobrescritos

        /// <summary>
        /// Sobrescreve o método de exclusão de veículos
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public override Colosoft.Business.DeleteResult Delete(Colosoft.Data.IPersistenceSession session)
        {
            var validador = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<IValidadorVeiculo>();

            // Executa a validação
            var resultadoValidacao = validador.ValidaExistencia(this);
            if (resultadoValidacao.Length > 0)
                return new Colosoft.Business.DeleteResult(false, resultadoValidacao.Join(" "));

            return base.Delete(session);
        }

        #endregion
    }
}

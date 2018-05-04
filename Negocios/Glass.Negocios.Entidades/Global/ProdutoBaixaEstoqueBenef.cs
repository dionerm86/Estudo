using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Representa a associação do produto com o beneficiamento.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(ProdutoBaixaEstoqueBenefLoader))]
    public class ProdutoBaixaEstoqueBenef : Colosoft.Business.Entity<Glass.Data.Model.ProdutoBaixaEstoqueBenef>, IBeneficiamento
    {
        #region Tipo Aninhados

        class ProdutoBaixaEstoqueBenefLoader : Colosoft.Business.EntityLoader<ProdutoBaixaEstoqueBenef, Glass.Data.Model.ProdutoBaixaEstoqueBenef>
        {
            public ProdutoBaixaEstoqueBenefLoader()
            {
                Configure()
                    .Uid(f => f.IdProdBaixaEstBenef)
                    .Creator(f => new ProdutoBaixaEstoqueBenef(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador da associação.
        /// </summary>
        public int IdProdBaixaEstBenef
        {
            get { return DataModel.IdProdBaixaEstBenef; }
            set
            {
                if (DataModel.IdProdBaixaEstBenef != value &&
                    RaisePropertyChanging("IdProdBaixaEstBenef", value))
                {
                    DataModel.IdProdBaixaEstBenef = value;
                    RaisePropertyChanged("IdProdBaixaEstBenef");
                }
            }
        }

        /// <summary>
        /// Identificador do produto associado.
        /// </summary>
        public int IdProdBaixaEst
        {
            get { return DataModel.IdProdBaixaEst; }
            set
            {
                if (DataModel.IdProdBaixaEst != value &&
                    RaisePropertyChanging("IdProdBaixaEst", value))
                {
                    DataModel.IdProdBaixaEst = value;
                    RaisePropertyChanged("IdProdBaixaEst");
                }
            }
        }

        /// <summary>
        /// Identificador do produto associado.
        /// </summary>
        public int IdBenefConfig
        {
            get { return DataModel.IdBenefConfig; }
            set
            {
                if (DataModel.IdBenefConfig != value &&
                    RaisePropertyChanging("IdBenefConfig", value))
                {
                    DataModel.IdBenefConfig = value;
                    RaisePropertyChanged("IdBenefConfig");
                }
            }
        }

        /// <summary>
        /// Quantidade.
        /// </summary>
        public int Qtd
        {
            get { return DataModel.Qtd; }
            set
            {
                if (DataModel.Qtd != value &&
                    RaisePropertyChanging("Qtd", value))
                {
                    DataModel.Qtd = value;
                    RaisePropertyChanged("Qtd");
                }
            }
        }

        /// <summary>
        /// LapLarg.
        /// </summary>
        public int LapLarg
        {
            get { return DataModel.LapLarg; }
            set
            {
                if (DataModel.LapLarg != value &&
                    RaisePropertyChanging("LapLarg", value))
                {
                    DataModel.LapLarg = value;
                    RaisePropertyChanged("LapLarg");
                }
            }
        }

        /// <summary>
        /// LapAlt.
        /// </summary>
        public int LapAlt
        {
            get { return DataModel.LapAlt; }
            set
            {
                if (DataModel.LapAlt != value &&
                    RaisePropertyChanging("LapAlt", value))
                {
                    DataModel.LapAlt = value;
                    RaisePropertyChanged("LapAlt");
                }
            }
        }

        /// <summary>
        /// BisLarg.
        /// </summary>
        public int BisLarg
        {
            get { return DataModel.BisLarg; }
            set
            {
                if (DataModel.BisLarg != value &&
                    RaisePropertyChanging("BisLarg", value))
                {
                    DataModel.BisLarg = value;
                    RaisePropertyChanged("BisLarg");
                }
            }
        }

        /// <summary>
        /// BisAlt.
        /// </summary>
        public int BisAlt
        {
            get { return DataModel.BisAlt; }
            set
            {
                if (DataModel.BisAlt != value &&
                    RaisePropertyChanging("BisAlt", value))
                {
                    DataModel.BisAlt = value;
                    RaisePropertyChanged("BisAlt");
                }
            }
        }

        /// <summary>
        /// Espessura do bisote.
        /// </summary>
        public float EspBisote
        {
            get { return DataModel.EspBisote; }
            set
            {
                if (DataModel.EspBisote != value &&
                    RaisePropertyChanging("EspBisote", value))
                {
                    DataModel.EspBisote = value;
                    RaisePropertyChanged("EspBisote");
                }
            }
        }

        /// <summary>
        /// Espessura do furo.
        /// </summary>
        public int EspFuro
        {
            get { return DataModel.EspFuro; }
            set
            {
                if (DataModel.EspFuro != value &&
                    RaisePropertyChanging("EspFuro", value))
                {
                    DataModel.EspFuro = value;
                    RaisePropertyChanged("EspFuro");
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public ProdutoBaixaEstoqueBenef()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected ProdutoBaixaEstoqueBenef(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.ProdutoBaixaEstoqueBenef> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public ProdutoBaixaEstoqueBenef(Data.Model.ProdutoBaixaEstoqueBenef dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion

        #region Membros de IBeneficiamento

        /// <summary>
        /// Tipo do produto do beneficiamento.
        /// </summary>
        Data.TipoProdutoBeneficiamento IBeneficiamento.TipoProdutoBenef
        {
            get { return Data.TipoProdutoBeneficiamento.ProdutoBaixaEst; }
        }

        /// <summary>
        /// Valor unitário.
        /// </summary>
        decimal IBeneficiamento.ValorUnit
        {
            get { return 0; }
            set { }
        }

        /// <summary>
        /// Valor.
        /// </summary>
        decimal IBeneficiamento.Valor
        {
            get { return 0; }
            set { }
        }

        /// <summary>
        /// Custo.
        /// </summary>
        decimal IBeneficiamento.Custo
        {
            get { return 0; }
            set { }
        }

        /// <summary>
        /// Padrão.
        /// </summary>
        bool IBeneficiamento.Padrao
        {
            get { return false; }
            set { }
        }

        /// <summary>
        /// Valor da Comissão.
        /// </summary>
        decimal IBeneficiamento.ValorComissao
        {
            get { return 0; }
            set { }
        }

        /// <summary>
        /// Valor do acrescimo.
        /// </summary>
        decimal IBeneficiamento.ValorAcrescimo
        {
            get { return 0; }
            set { }
        }

        /// <summary>
        /// Valor do acrescimo.
        /// </summary>
        decimal IBeneficiamento.ValorAcrescimoProd
        {
            get { return 0; }
            set { }
        }

        /// <summary>
        /// Valor do desconto.
        /// </summary>
        decimal IBeneficiamento.ValorDesconto
        {
            get { return 0; }
            set { }
        }

        /// <summary>
        /// Valro do desconto.
        /// </summary>
        decimal IBeneficiamento.ValorDescontoProd
        {
            get { return 0; }
            set { }
        }

        #endregion

        #region Membros de IEquality<IBeneficiamento>

        /// <summary>
        /// Verifica se o beneficiamento informado é igual a instancia.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        bool IEquatable<IBeneficiamento>.Equals(IBeneficiamento other)
        {
            if (other == null)
                return false;

            if (other is ProdutoBaixaEstoqueBenef)
            {
                var prodBenf = (ProdutoBaixaEstoqueBenef)other;
                return this.IdBenefConfig == prodBenf.IdBenefConfig;
            }

            return false;
        }

        #endregion
    }
}

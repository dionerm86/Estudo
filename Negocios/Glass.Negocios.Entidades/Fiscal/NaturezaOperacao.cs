using System.Linq;
using Colosoft;
using Microsoft.Practices.ServiceLocation;

namespace Glass.Fiscal.Negocios.Entidades
{
    /// <summary>
    /// Assinatura do validador das naturezas de operação.
    /// </summary>
    public interface IValidadorNaturezaOperacao
    {
        #region Métodos

        /// <summary>
        /// Valida o código interno da natureza de operação.
        /// </summary>
        /// <param name="naturezaOperacao"></param>
        /// <returns></returns>
        IMessageFormattable ValidaCodigoInterno(NaturezaOperacao naturezaOperacao);

        /// <summary>
        /// Valida a existencia da natureza de operação. Verifica se está sendo usada em algum lugar.
        /// </summary>
        /// <param name="naturezaOperacao"></param>
        /// <returns></returns>
        IMessageFormattable[] ValidaExistencia(Entidades.NaturezaOperacao naturezaOperacao);

        #endregion
    }

    /// <summary>
    /// Representa a entidade de negócio da natureza de operação.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(NaturezaOperacaoLoader))]
    [Glass.Negocios.ControleAlteracao(Data.Model.LogAlteracao.TabelaAlteracao.NaturezaOperacao)]
    public class NaturezaOperacao : Colosoft.Business.Entity<Data.Model.NaturezaOperacao>, Sync.Fiscal.EFD.Entidade.INaturezaOperacao
    {
        #region Tipos Aninhados

        class NaturezaOperacaoLoader : Colosoft.Business.EntityLoader<NaturezaOperacao, Data.Model.NaturezaOperacao>
        {
            public NaturezaOperacaoLoader()
            {
                Configure()
                    .Uid(f => f.IdNaturezaOperacao)
                    .FindName(f => f.CodInterno)
                    .Reference<Cfop, Data.Model.Cfop>("Cfop", f => f.Cfop, f => f.IdCfop)
                    .Creator(f => new NaturezaOperacao(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Referência ao CFOP da natureza de operação.
        /// </summary>
        public Cfop Cfop
        {
            get { return GetReference<Cfop>("Cfop", true); }
        }

        /// <summary>
        /// Identificador da natureza de operação.
        /// </summary>
        public int IdNaturezaOperacao
        {
            get { return DataModel.IdNaturezaOperacao; }
            set
            {
                if (DataModel.IdNaturezaOperacao != value &&
                    RaisePropertyChanging("IdNaturezaOperacao", value))
                {
                    DataModel.IdNaturezaOperacao = value;
                    RaisePropertyChanged("IdNaturezaOperacao");
                }
            }
        }

        /// <summary>
        /// Identificador do Cfop associado.
        /// </summary>
        public int IdCfop
        {
            get { return DataModel.IdCfop; }
            set
            {
                if (DataModel.IdCfop != value &&
                    RaisePropertyChanging("IdCfop", value))
                {
                    DataModel.IdCfop = value;
                    RaisePropertyChanged("IdCfop");
                }
            }
        }

        /// <summary>
        /// Código interno.
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
        /// Mensagem.
        /// </summary>
        public string Mensagem
        {
            get { return DataModel.Mensagem; }
            set
            {
                if (DataModel.Mensagem != value &&
                    RaisePropertyChanging("Mensagem", value))
                {
                    DataModel.Mensagem = value;
                    RaisePropertyChanged("Mensagem");
                }
            }
        }

        /// <summary>
        /// Calcular ICMS.
        /// </summary>
        public bool CalcIcms
        {
            get { return DataModel.CalcIcms; }
            set
            {
                if (DataModel.CalcIcms != value &&
                    RaisePropertyChanging("CalcIcms", value))
                {
                    DataModel.CalcIcms = value;
                    RaisePropertyChanged("CalcIcms");
                }
            }
        }

        /// <summary>
        /// Calcular ICMS ST.
        /// </summary>
        public bool CalcIcmsSt
        {
            get { return DataModel.CalcIcmsSt; }
            set
            {
                if (DataModel.CalcIcmsSt != value &&
                    RaisePropertyChanging("CalcIcmsSt", value))
                {
                    DataModel.CalcIcmsSt = value;
                    RaisePropertyChanged("CalcIcmsSt");
                }
            }
        }

        /// <summary>
        /// Calcular IPI.
        /// </summary>
        public bool CalcIpi
        {
            get { return DataModel.CalcIpi; }
            set
            {
                if (DataModel.CalcIpi != value &&
                    RaisePropertyChanging("CalcIpi", value))
                {
                    DataModel.CalcIpi = value;
                    RaisePropertyChanged("CalcIpi");
                }
            }
        }

        /// <summary>
        /// Calcular PIS.
        /// </summary>
        public bool CalcPis
        {
            get { return DataModel.CalcPis; }
            set
            {
                if (DataModel.CalcPis != value &&
                    RaisePropertyChanging("CalcPis", value))
                {
                    DataModel.CalcPis = value;
                    RaisePropertyChanged("CalcPis");
                }
            }
        }

        /// <summary>
        /// Calcular  Cofins.
        /// </summary>
        public bool CalcCofins
        {
            get { return DataModel.CalcCofins; }
            set
            {
                if (DataModel.CalcCofins != value &&
                    RaisePropertyChanging("CalcCofins", value))
                {
                    DataModel.CalcCofins = value;
                    RaisePropertyChanged("CalcCofins");
                }
            }
        }

        /// <summary>
        /// IPI Integra Base Cálculo ICMS.
        /// </summary>
        public bool IpiIntegraBcIcms
        {
            get { return DataModel.IpiIntegraBcIcms; }
            set
            {
                if (DataModel.IpiIntegraBcIcms != value &&
                    RaisePropertyChanging("IpiIntegraBcIcms", value))
                {
                    DataModel.IpiIntegraBcIcms = value;
                    RaisePropertyChanged("IpiIntegraBcIcms");
                }
            }
        }

        /// <summary>
        /// Alterar estoque fiscal.
        /// </summary>
        public bool AlterarEstoqueFiscal
        {
            get { return DataModel.AlterarEstoqueFiscal; }
            set
            {
                if (DataModel.AlterarEstoqueFiscal != value &&
                    RaisePropertyChanging("AlterarEstoqueFiscal", value))
                {
                    DataModel.AlterarEstoqueFiscal = value;
                    RaisePropertyChanged("AlterarEstoqueFiscal");
                }
            }
        }

        /// <summary>
        /// CST IPI.
        /// </summary>
        public string CstIcms
        {
            get { return DataModel.CstIcms; }
            set
            {
                if (DataModel.CstIcms != value &&
                    RaisePropertyChanging("CstIcms", value))
                {
                    DataModel.CstIcms = value;
                    RaisePropertyChanged("CstIcms");
                }
            }
        }

        /// <summary>
        /// Perc. Redução BC ICMS.
        /// </summary>
        public float PercReducaoBcIcms
        {
            get { return DataModel.PercReducaoBcIcms; }
            set
            {
                if (DataModel.PercReducaoBcIcms != value &&
                    RaisePropertyChanging("PercReducaoBcIcms", value))
                {
                    DataModel.PercReducaoBcIcms = value;
                    RaisePropertyChanged("PercReducaoBcIcms");
                }
            }
        }

        /// <summary>
        /// CST IPI.
        /// </summary>
        public Data.Model.ProdutoCstIpi? CstIpi
        {
            get { return DataModel.CstIpi; }
            set
            {
                if (DataModel.CstIpi != value &&
                    RaisePropertyChanging("CstIpi", value))
                {
                    DataModel.CstIpi = value;
                    RaisePropertyChanged("CstIpi");
                }
            }
        }

        /// <summary>
        /// CST Pis/Cofins.
        /// </summary>
        public int? CstPisCofins
        {
            get { return DataModel.CstPisCofins; }
            set
            {
                if (DataModel.CstPisCofins != value &&
                    RaisePropertyChanging("CstPisCofins", value))
                {
                    DataModel.CstPisCofins = value;
                    RaisePropertyChanged("CstPisCofins");
                }
            }
        }

        /// <summary>
        /// CSOSN.
        /// </summary>
        public string Csosn
        {
            get { return DataModel.Csosn; }
            set
            {
                if (DataModel.Csosn != value &&
                    RaisePropertyChanging("Csosn", value))
                {
                    DataModel.Csosn = value;
                    RaisePropertyChanged("Csosn");
                }
            }
        }

        /// <summary>
        /// Código de enquadramento do IPI.
        /// </summary>
        public string CodEnqIpi
        {
            get { return DataModel.CodEnqIpi; }
            set
            {
                if (DataModel.CodEnqIpi != value &&
                    RaisePropertyChanging("CodEnqIpi", value))
                {
                    DataModel.CodEnqIpi = value;
                    RaisePropertyChanged("CodEnqIpi");
                }
            }
        }
 
        /// <summary>
        /// Calcular diferença de alíquotas.
        /// </summary>
        public bool CalcularDifal
        {
            get { return DataModel.CalcularDifal; }
            set
            {
                if (DataModel.CalcularDifal != value &&
                    RaisePropertyChanging("CalcularDifal", value))
                {
                    DataModel.CalcularDifal = value;
                    RaisePropertyChanged("CalcularDifal");
                }
            }
        }

        /// <summary>
        /// NCM.
        /// </summary>
        public string Ncm
        {
            get { return DataModel.Ncm; }
            set
            {
                if (DataModel.Ncm != value && RaisePropertyChanging("Ncm", value))
                {
                    if (value != null)
                        value = value.Replace("\t", "");

                    DataModel.Ncm = value;
                    RaisePropertyChanged("Ncm");
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public NaturezaOperacao()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected NaturezaOperacao(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.NaturezaOperacao> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public NaturezaOperacao(Data.Model.NaturezaOperacao dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Método usado para apagar a natureza de operação.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public override Colosoft.Business.DeleteResult Delete(Colosoft.Data.IPersistenceSession session)
        {
            var validador = ServiceLocator.Current.GetInstance<IValidadorNaturezaOperacao>();

            // Valida a existencia da natureza de operação
            var validacaoExistencia = validador.ValidaExistencia(this);
            if (validacaoExistencia.Length > 0)
                return new Colosoft.Business.DeleteResult(false,
                    new Colosoft.Text.JoinMessageFormattable(
                        "Não é possível excluir essa natureza de operação.".GetFormatter(), " ",
                        validacaoExistencia.First()));

            return base.Delete(session);
        }

        /// <summary>
        /// Método usado para salvar a entidade.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public override Colosoft.Business.SaveResult Save(Colosoft.Data.IPersistenceSession session)
        {
            var validador = ServiceLocator.Current.GetInstance<IValidadorNaturezaOperacao>();

            // Verifica se o código interno está sendo alterado
            if (this.ExistsInStorage && this.ChangedProperties.Contains("CodInterno"))
            {
                if (!string.IsNullOrEmpty(this.CodInterno))
                {
                    // Valida a existencia da natureza de operação
                    var validacaoExistencia = validador.ValidaExistencia(this);
                    if (validacaoExistencia.Length > 0)
                    {
                        return new Colosoft.Business.SaveResult(false,
                            new Colosoft.Text.JoinMessageFormattable(
                                "Não é possível alterar o código dessa natureza de operação.".GetFormatter(), " ",
                                validacaoExistencia.First()));
                    }
                }
                else
                    return new Colosoft.Business.SaveResult(false,
                        "Não é possível alterar o código da natureza de operação padrão.".GetFormatter());
            }

            // Valida o código interno
            var resultadoValidacao = validador.ValidaCodigoInterno(this);
            if (resultadoValidacao != null)
                return new Colosoft.Business.SaveResult(false, resultadoValidacao);

            return base.Save(session);
        }

        #endregion

        #region Membros INaturezaOperacao

        int Sync.Fiscal.EFD.Entidade.INaturezaOperacao.CodigoCfop
        {
            get { return IdCfop; }
        }

        bool Sync.Fiscal.EFD.Entidade.INaturezaOperacao.CalculaPis
        {
            get { return CalcPis; }
        }

        bool Sync.Fiscal.EFD.Entidade.INaturezaOperacao.CalculaCofins
        {
            get { return CalcCofins; }
        }

        #endregion
    }
}

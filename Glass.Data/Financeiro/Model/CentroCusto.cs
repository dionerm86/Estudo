using System;
using GDA;
using Glass.Data.DAL;
using Glass.Data.EFD;
using Glass.Log;
using System.Globalization;
using System.ComponentModel;

namespace Glass.Data.Model
{
    #region Enumeradores

    /// <summary>
    /// Tipos de Centro de Custo
    /// </summary>
    [Colosoft.EmptyDescription("N/D")]
    public enum TipoCentroCusto
    {
        /// <summary>
        /// Indefinido.
        /// </summary>
        [Description("Indefinido")]
        Indefinido,
        /// <summary>
        /// Estoque
        /// </summary>
        [Description("Estoque")]
        Estoque
    }

    #endregion

    [PersistenceBaseDAO(typeof(CentroCustoDAO))]
    [PersistenceClass("centro_custo")]
    public class CentroCusto : ModelBaseCadastro, Sync.Fiscal.EFD.Entidade.ICentroCusto
    {
        #region Propriedades

        [PersistenceProperty("IDCENTROCUSTO", PersistenceParameterType.IdentityKey)]
        public int IdCentroCusto { get; set; }

        [Log("Loja", true, "NomeFantasia", typeof(LojaDAO))]
        [PersistenceProperty("IDLOJA")]
        [PersistenceForeignKey(typeof(Loja), "IdLoja")]
        public int IdLoja { get; set; }

        private string _descricao;

        [Log("Descrição", true)]
        [PersistenceProperty("DESCRICAO")]
        public string Descricao
        {
            get 
            { 
                if (_forEfd || !String.IsNullOrEmpty(_descricao))
                    return _descricao;
                else if (IdCentroCusto > 0)
                {
                    CentroCusto temp = new CentroCusto();
                    temp.CodigoTipo = (int)IdCentroCusto;
                    return temp.DescrCodigoTipo;
                }
                else
                    return null;
            }
            set { _descricao = value; }
        }

        [PersistenceProperty("CODIGOTIPO")]
        public int CodigoTipo { get; set; }

        [PersistenceProperty("Tipo")]
        public TipoCentroCusto Tipo { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("NOMELOJA", DirectionParameter.InputOptional)]
        public string NomeLoja { get; set; }

        #region Relatório do Centro de Custos

        [PersistenceProperty("Total", DirectionParameter.InputOptional)]
        public decimal Total { get; set; }

        [PersistenceProperty("Mes", DirectionParameter.InputOptional)]
        public int Mes { get; set; }

        [PersistenceProperty("IdPlanoConta", DirectionParameter.InputOptional)]
        public int IdPlanoConta { get; set; }

        [PersistenceProperty("DescrPlanoConta", DirectionParameter.InputOptional)]
        public string DescrPlanoConta { get; set; }

        #endregion

        #endregion

        #region Propriedades de Suporte

        [Log("Área")]
        public string DescrCodigoTipo
        {
            get { return DataSourcesEFD.Instance.GetDescrTipoCentroCusto(CodigoTipo); }
            internal set
            {
                CentroCusto temp = new CentroCusto();
                while (temp.CodigoTipo <= 5)
                {
                    temp.CodigoTipo++;
                    if (temp.DescrCodigoTipo == value)
                    {
                        CodigoTipo = temp.CodigoTipo;
                        break;
                    }
                }
            }
        }

        private bool _forEfd = false;

        internal bool ForEfd
        {
            get { return _forEfd; }
            set { _forEfd = value; }
        }

        #region Relatório do Centro de Custos

        public string Periodo
        {
            get 
            {
                if (Mes == 0 || Mes > 12)
                    return null;

                return Glass.FuncoesGerais.UppercaseFirst(CultureInfo.GetCultureInfo("pt", "br").DateTimeFormat.GetMonthName(Mes));
            }
        }

        public DateTime DataInicial { get; set; }

        public DateTime DataFinal { get; set; }

        #endregion

        #endregion

        #region ICentroCusto Members

        DateTime Sync.Fiscal.EFD.Entidade.ICentroCusto.DataCadastro
        {
            get { return DataCad; }
            set { DataCad = value; }
        }

        #endregion

        #region IBuscarAPartirDoLog Members

        int Sync.Fiscal.EFD.Entidade.IBuscarAPartirDoLog.Codigo
        {
            get { return (int)IdCentroCusto; }
        }

        #endregion
    }
}
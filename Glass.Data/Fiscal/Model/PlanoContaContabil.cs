using System;
using GDA;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(PlanoContaContabilDAO))]
    [PersistenceClass("plano_conta_contabil")]
    public class PlanoContaContabil : ModelBaseCadastro, Sync.Fiscal.EFD.Entidade.IPlanoContaContabil
    {
        #region Enumeradores

        public enum NaturezaEnum
        {
            ContasAtivo = 1,
            ContasPassivo = 2,
            PatrimonioLiquido = 3,
            ContasResultado = 4,
            ContasCompensacao = 5,
            Outras = 9
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("IDCONTACONTABIL", PersistenceParameterType.IdentityKey)]
        public int IdContaContabil { get; set; }

        [Log("Código", true)]
        [PersistenceProperty("CODINTERNO")]
        public string CodInterno { get; set; }

        [Log("Descrição", true)]
        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        [Log("Natureza", true)]
        [PersistenceProperty("NATUREZA")]
        public int Natureza { get; set; }

        [Log("Cod. Conta RFB", true)]
        public string CodContaRfb { get; set; }

        #endregion

        #region Propriedades de Suporte

        [Log("Natureza")]
        public string DescrNatureza
        {
            get
            {
                switch ((NaturezaEnum)Natureza)
                {
                    case NaturezaEnum.ContasAtivo: return "Conta Ativo";
                    case NaturezaEnum.ContasPassivo: return "Conta Passivo";
                    case NaturezaEnum.PatrimonioLiquido: return "Patrimônio Líquido";
                    case NaturezaEnum.ContasResultado: return "Conta de Resultado";
                    case NaturezaEnum.ContasCompensacao: return "Conta de Compensação";
                    case NaturezaEnum.Outras: return "Outras";
                    default: return "";
                }
            }
            internal set
            {
                PlanoContaContabil temp = new PlanoContaContabil();
                while (temp.Natureza <= 9)
                {
                    temp.Natureza++;
                    if (temp.DescrNatureza.Equals(value, StringComparison.InvariantCultureIgnoreCase))
                    {
                        Natureza = temp.Natureza;
                        break;
                    }
                }
            }
        }

        #endregion

        #region IPlanoContaContabil Members

        string Sync.Fiscal.EFD.Entidade.IPlanoContaContabil.CodigoInterno
        {
            get { return CodInterno; }
        }

        string Sync.Fiscal.EFD.Entidade.IPlanoContaContabil.CodigoContaRFB
        {
            get { return CodContaRfb; }
        }

        DateTime Sync.Fiscal.EFD.Entidade.IPlanoContaContabil.DataCadastro
        {
            get { return DataCad; }
            set { DataCad = value; }
        }

        #endregion

        #region IBuscarAPartirDoLog Members

        int Sync.Fiscal.EFD.Entidade.IBuscarAPartirDoLog.Codigo
        {
            get { return (int)IdContaContabil; }
        }

        #endregion
    }
}
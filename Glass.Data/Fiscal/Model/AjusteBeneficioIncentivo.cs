using System;
using GDA;
using Glass.Data.EFD;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(AjusteBeneficioIncentivoDAO))]
    [PersistenceClass("ajuste_beneficio_incentivo")]
    public class AjusteBeneficioIncentivo : Sync.Fiscal.EFD.Entidade.IAjusteBeneficioIncentivo
    {
        #region Propriedades

        [PersistenceProperty("IDAJBENINC", PersistenceParameterType.IdentityKey)]
        public uint IdAjBenInc { get; set; }

        [PersistenceProperty("CODIGO")]
        public string Codigo { get; set; }

        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        [PersistenceProperty("UF")]
        public string Uf { get; set; }

        [PersistenceProperty("DATAINICIO")]
        public DateTime DataInicio { get; set; }

        [PersistenceProperty("DATATERMINO")]
        public DateTime? DataTermino { get; set; }

        [PersistenceProperty("TIPOIMPOSTO")]
        public int TipoImposto { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string CodigoDescricao
        {
            get
            {
                return Codigo + " - " + Descricao;
            }
        }

        public string DescricaoTipoImposto
        {
            get
            {
                switch (TipoImposto)
                {
                    case (int)ConfigEFD.TipoImpostoEnum.ICMS: return "ICMS";
                    case (int)ConfigEFD.TipoImpostoEnum.ICMSST: return "ICMS ST";
                    case (int)ConfigEFD.TipoImpostoEnum.IPI: return "IPI";
                }

                return "";
            }
        }
        #endregion

        #region IAjusteBeneficioIncentivo Members

        int Sync.Fiscal.EFD.Entidade.IAjusteBeneficioIncentivo.Codigo
        {
            get { return (int)IdAjBenInc; }
        }

        string Sync.Fiscal.EFD.Entidade.IAjusteBeneficioIncentivo.CodigoAjuste
        {
            get { return Codigo; }
        }

        #endregion
    }
}
using System;
using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(AjusteApuracaoValorDeclaratorioDAO))]
    [PersistenceClass("sped_ajuste_apuracao_valores_declaratorios")]
    public class AjusteApuracaoValorDeclaratorio : Sync.Fiscal.EFD.Entidade.IAjusteApuracaoValorDeclaratorio
    {
        #region Propriedades

        [PersistenceProperty("ID", PersistenceParameterType.IdentityKey)]
        public uint Id { get; set; }

        [PersistenceProperty("IDAJBENINC")]
        public uint IdAjBenInc { get; set; }

        [PersistenceProperty("VALOR")]
        public decimal Valor { get; set; }

        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        [PersistenceProperty("DATA")]
        public DateTime Data { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("CODIGO", DirectionParameter.InputOptional)]
        public string Codigo { get; set; }

        [PersistenceProperty("CODIGODESCRICAO", DirectionParameter.InputOptional)]
        public string CodigoDescricao { get; set; }

        [PersistenceProperty("UF", DirectionParameter.InputOptional)]
        public string UF { get; set; }

        #endregion
    }
}
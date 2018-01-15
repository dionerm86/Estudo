using System;
using GDA;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(BenefConfigPrecoDAO))]
    [PersistenceClass("benef_config_preco")]
    [Colosoft.Data.Schema.Cache]
    public class BenefConfigPreco : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IDBENEFCONFIGPRECO", PersistenceParameterType.IdentityKey)]
        public int IdBenefConfigPreco { get; set; }

        [PersistenceProperty("IDBENEFCONFIG")]
        [PersistenceForeignKey(typeof(BenefConfig), "IdBenefConfig")]
        public int IdBenefConfig { get; set; }

        [Log("Subgrupo", "Descricao", typeof(SubgrupoProdDAO))]
        [PersistenceProperty("IDSUBGRUPOPROD")]
        [PersistenceForeignKey(typeof(SubgrupoProd), "IdSubgrupoProd")]
        public int? IdSubgrupoProd { get; set; }

        [Log("Cor do vidro", "Descricao", typeof(CorVidroDAO))]
        [PersistenceProperty("IDCORVIDRO")]
        [PersistenceForeignKey(typeof(CorVidro), "IdCorVidro")]
        public int? IdCorVidro { get; set; }

        [Log("Espessura do vidro")]
        [PersistenceProperty("ESPESSURA")]
        [Colosoft.Data.Schema.CacheIndexed]
        public float? Espessura { get; set; }

        [Log("Valor Atacado")]
        [PersistenceProperty("VALORATACADO")]
        public decimal ValorAtacado { get; set; }

        [Log("Valor Balcão")]
        [PersistenceProperty("VALORBALCAO")]
        public decimal ValorBalcao { get; set; }

        [Log("Valor Obra")]
        [PersistenceProperty("VALOROBRA")]
        public decimal ValorObra { get; set; }

        [Log("Custo")]
        [PersistenceProperty("CUSTO")]
        public decimal Custo { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("DESCRICAO", DirectionParameter.InputOptional)]
        public string DescricaoBenef { get; set; }

        [PersistenceProperty("DESCRSUBGRUPOPROD", DirectionParameter.InputOptional)]
        public string DescrSubgrupoProd { get; set; }

        [PersistenceProperty("DESCRCORVIDRO", DirectionParameter.InputOptional)]
        public string DescrCorVidro { get; set; }

        [PersistenceProperty("TIPOCALCULO", DirectionParameter.InputOptional)]
        public int TipoCalculo { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string DescrEspessura
        {
            get { return Espessura > 0 ? Espessura.Value + "mm" : ""; }
        }

        public bool SubgrupoCorVisible
        {
            get { return BenefConfigPrecoDAO.Instance.GetForBenefConfigEditCount((uint)IdBenefConfig, Espessura) > 0; }
        }

        public string DescricaoComTipoCalc
        {
            get
            {
                string descrTipoCalculo = BenefConfig.GetDescrTipoCalculo(TipoCalculo);
                return DescricaoBenef + (!String.IsNullOrEmpty(descrTipoCalculo) ? " (" + descrTipoCalculo + ")" : "");
            }
        }

        public float AjusteAtacado { get; set; }

        public float AjusteBalcao { get; set; }

        public float AjusteObra { get; set; }

        public float AjusteCustoCompra { get; set; }

        #endregion
    }
}

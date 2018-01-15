using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(PagtoAcertoDAO))]
    [PersistenceClass("pagto_acerto")]
    public class PagtoAcerto
    {
        #region Propriedades

        [PersistenceProperty("IDACERTO", PersistenceParameterType.Key)]
        public uint IdAcerto { get; set; }

        [PersistenceProperty("NUMFORMAPAGTO", PersistenceParameterType.Key)]
        public int NumFormaPagto { get; set; }

        [PersistenceProperty("IDFORMAPAGTO")]
        public uint IdFormaPagto { get; set; }

        [PersistenceProperty("VALORPAGTO", PersistenceParameterType.Key)]
        public decimal ValorPagto { get; set; }

        [PersistenceProperty("IDCONTABANCO")]
        public uint? IdContaBanco { get; set; }

        [PersistenceProperty("IDTIPOCARTAO")]
        public uint? IdTipoCartao { get; set; }

        [PersistenceProperty("NumAutCartao")]
        public string NumAutCartao { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("FormaPagto", DirectionParameter.InputOptional)]
        public string FormaPagto { get; set; }

        #endregion

        #region Propiedades de Suporte

        public string DescrFormaPagto
        {
            get
            {
                if (IdFormaPagto == (uint)Pagto.FormaPagto.Obra)
                    return "Obra";

                var ret = FormaPagto;

                if (IdTipoCartao.GetValueOrDefault(0) > 0)
                {
                    var tipoCartao = TipoCartaoCreditoDAO.Instance.GetElementByPrimaryKey(IdTipoCartao.Value);

                    if (tipoCartao != null)
                        ret += " " + tipoCartao.Descricao;
                }

                return ret;
            }
        }

        #endregion
    }
}
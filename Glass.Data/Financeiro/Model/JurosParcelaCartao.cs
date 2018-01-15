using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(JurosParcelaCartaoDAO))]
    [PersistenceClass("juros_parcela_cartao")]
    public class JurosParcelaCartao : Colosoft.Data.BaseModel
    {
        #region Construtores

        public JurosParcelaCartao()
        {
        }

        internal JurosParcelaCartao(uint idTipoCartao, uint? idLoja, int numParc)
        {
            IdTipoCartao = (int)idTipoCartao;
            IdLoja = idLoja;
            NumParc = numParc;
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("IDJUROSPARCELA")]
        public uint IdJurosParcela { get; set; }

        [PersistenceProperty("IDTIPOCARTAO")]
        public int IdTipoCartao { get; set; }

        [PersistenceProperty("IDLOJA")]
        public uint? IdLoja { get; set; }

        [PersistenceProperty("NUMPARC")]
        public int NumParc { get; set; }

        [PersistenceProperty("JUROS")]
        public float Juros { get; set; }

        #endregion
    }
}
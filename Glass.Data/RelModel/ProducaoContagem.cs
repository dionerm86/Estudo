using GDA;
using Glass.Data.Helper;
using Glass.Data.Model;
using Glass.Data.RelDAL;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(ProducaoContagemDAO))]
    [PersistenceClass("producao_contagem")]
    public class ProducaoContagem
    {
        #region Propriedades

        [PersistenceProperty("IDPEDIDO")]
        public uint IdPedido { get; set; }

        [PersistenceProperty("IDPEDIDOEXIBIR")]
        public string IdPedidoExibir { get; set; }

        [PersistenceProperty("IDSETOR")]
        public uint IdSetor { get; set; }

        [PersistenceProperty("TOTM2")]
        public double TotM2 { get; set; }

        [PersistenceProperty("NUMEROPECAS")]
        public long NumeroPecas { get; set; }

        [PersistenceProperty("CRITERIO")]
        public string Criterio { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string NomeSetor
        {
            get { return Utils.ObtemSetor(IdSetor).Descricao; }
        }

        public int NumSeqSetor
        {
            get { return Utils.ObtemSetor(IdSetor).NumeroSequencia; }
        }

        #endregion
    }
}
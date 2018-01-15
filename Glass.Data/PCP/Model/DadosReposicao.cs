using System;
using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(DadosReposicaoDAO))]
    [PersistenceClass("dados_reposicao")]
    public class DadosReposicao
    {
        #region Propriedades

        [PersistenceProperty("IDDADOSREPOSICAO", PersistenceParameterType.IdentityKey)]
        public uint IdDadosReposicao { get; set; }

        [PersistenceProperty("IDPRODPEDPRODUCAO")]
        [PersistenceForeignKey(typeof(ProdutoPedidoProducao), "IdProdPedProducao")]
        public uint IdProdPedProducao { get; set; }

        [PersistenceProperty("NUMSEQ")]
        public int NumSeq { get; set; }

        [PersistenceProperty("IDFUNCREPOS")]
        public uint IdFuncRepos { get; set; }

        [PersistenceProperty("IDSETORREPOS")]
        public uint IdSetorRepos { get; set; }

        [PersistenceProperty("TIPOPERDAREPOS")]
        public int TipoPerdaRepos { get; set; }

        [PersistenceProperty("IDSUBTIPOPERDAREPOS")]
        public uint? IdSubtipoPerdaRepos { get; set; }

        [PersistenceProperty("DATAREPOS")]
        public DateTime DataRepos { get; set; }

        [PersistenceProperty("DADOSREPOSICAOPECA")]
        public string DadosReposicaoPeca { get; set; }

        [PersistenceProperty("OBS")]
        public string Obs { get; set; }

        [PersistenceProperty("SITUACAOPRODUCAO")]
        public int SituacaoProducao { get; set; }

        #endregion
    }
}
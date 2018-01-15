using System;
using System.Linq;
using GDA;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceClass("roteiro_producao")]
    [PersistenceBaseDAO(typeof(RoteiroProducaoDAO))]
    public class RoteiroProducao: Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IDROTEIROPRODUCAO", PersistenceParameterType.IdentityKey)]
        public int IdRoteiroProducao { get; set; }

        [Log("Grupo de Produto", "Descricao", typeof(GrupoProdDAO))]
        [PersistenceProperty("IDGRUPOPROD")]
        public uint? IdGrupoProd { get; set; }

        [Log("Subgrupo de Produto", "Descricao", typeof(SubgrupoProdDAO))]
        [PersistenceProperty("IDSUBGRUPOPROD")]
        public uint? IdSubgrupoProd { get; set; }

        [Log("Processo", "CodInterno", typeof(EtiquetaProcessoDAO))]
        [PersistenceProperty("IDPROCESSO")]
        public uint? IdProcesso { get; set; }

        [PersistenceProperty("IdClassificacaoRoteiroProducao")]
        public int? IdClassificacaoRoteiroProducao { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("CRITERIO", DirectionParameter.InputOptional)]
        public string Criterio { get; set; }

        [PersistenceProperty("CODPROCESSO", DirectionParameter.InputOptional)]
        public string CodProcesso { get; set; }

        #endregion

        #region Propriedade para Log

        private string _setores;

        [Log("Setores")]
        [PersistenceProperty("SETORES", DirectionParameter.InputOptional)]
        public string Setores
        {
            get
            {
                if (_setores == null)
                {
                    var temp = RoteiroProducaoSetorDAO.Instance.ObtemPorRoteiroProducao(IdRoteiroProducao);
                    _setores = SetorDAO.Instance.GetNomeSetores(String.Join(",", temp.Select(x => x.IdSetor.ToString()).ToArray())).Replace(",", ", ");
                }

                return _setores;
            }
            set { _setores = value; }
        }

        #endregion
    }
}

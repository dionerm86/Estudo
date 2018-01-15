using GDA;
using Glass.Data.DAL;
using System.Collections.Generic;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(CidadeDescargaMDFeDAO))]
    [PersistenceClass("cidade_descarga_mdfe")]
    public class CidadeDescargaMDFe
    {
        #region Propriedades

        [PersistenceProperty("IDCIDADEDESCARGA", PersistenceParameterType.IdentityKey)]
        public int IdCidadeDescarga { get; set; }

        [PersistenceProperty("IDMANIFESTOELETRONICO")]
        [PersistenceForeignKey(typeof(ManifestoEletronico), "IdManifestoEletronico")]
        public int IdManifestoEletronico { get; set; }

        [PersistenceProperty("IDCIDADE")]
        [PersistenceForeignKey(typeof(Cidade), "IdCidade")]
        public int IdCidade { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("NOMECIDADE", DirectionParameter.InputOptional)]
        public string NomeCidade { get; set; }

        #endregion

        private List<NFeCidadeDescargaMDFe> _NFesCidadeDescarga = null;
        public List<NFeCidadeDescargaMDFe> NFesCidadeDescarga
        {
            get
            {
                if (_NFesCidadeDescarga == null)
                {
                    _NFesCidadeDescarga = NFeCidadeDescargaMDFeDAO.Instance.ObterNFesCidadeDescargaMDFe(IdCidadeDescarga);
                }
                return _NFesCidadeDescarga;
            }
            set
            {
                _NFesCidadeDescarga = value;
            }
        }

        private List<CTeCidadeDescargaMDFe> _CTesCidadeDescarga = null;
        public List<CTeCidadeDescargaMDFe> CTesCidadeDescarga
        {
            get
            {
                if (_CTesCidadeDescarga == null)
                {
                    _CTesCidadeDescarga = CTeCidadeDescargaMDFeDAO.Instance.ObterCTesCidadeDescargaMDFe(IdCidadeDescarga);
                }
                return _CTesCidadeDescarga;
            }
            set
            {
                _CTesCidadeDescarga = value;
            }
        }
    }
}

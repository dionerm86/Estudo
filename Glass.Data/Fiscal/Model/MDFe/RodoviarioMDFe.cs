using GDA;
using Glass.Data.DAL;
using System;
using System.Collections.Generic;

namespace Glass.Data.Model
{
    [Serializable]
    [PersistenceBaseDAO(typeof(RodoviarioMDFeDAO))]
    [PersistenceClass("rodoviario_mdfe")]
    public class RodoviarioMDFe
    {
        [PersistenceProperty("IDRODOVIARIO", PersistenceParameterType.IdentityKey)]
        public int IdRodoviario { get; set; }

        [PersistenceProperty("IDMANIFESTOELETRONICO")]
        [PersistenceForeignKey(typeof(ManifestoEletronico), "IdManifestoEletronico")]
        public int IdManifestoEletronico { get; set; }

        [PersistenceProperty("PLACATRACAO")]
        [PersistenceForeignKey(typeof(Veiculo), "Placa")]
        public string PlacaTracao { get; set; }

        private List<CiotRodoviarioMDFe> _ciotRodoviario = null;
        public List<CiotRodoviarioMDFe> CiotRodoviario
        {
            get
            {
                if (_ciotRodoviario == null)
                {
                    _ciotRodoviario = CiotRodoviarioMDFeDAO.Instance.ObterCiotRodoviarioMDFe(IdRodoviario);
                }
                return _ciotRodoviario;
            }
            set
            {
                _ciotRodoviario = value;
            }
        }

        private List<PedagioRodoviarioMDFe> _pedagioRodoviario = null;
        public List<PedagioRodoviarioMDFe> PedagioRodoviario
        {
            get
            {
                if (_pedagioRodoviario == null)
                {
                    _pedagioRodoviario = PedagioRodoviarioMDFeDAO.Instance.ObterPedagioRodoviarioMDFe(IdRodoviario);
                }
                return _pedagioRodoviario;
            }
            set
            {
                _pedagioRodoviario = value;
            }
        }

        private List<CondutorVeiculoMDFe> _condutorVeiculo = null;
        public List<CondutorVeiculoMDFe> CondutorVeiculo
        {
            get
            {
                if (_condutorVeiculo == null)
                {
                    _condutorVeiculo = CondutorVeiculoMDFeDAO.Instance.ObterCondutorVeiculoMDFe(IdRodoviario);
                }
                return _condutorVeiculo;
            }
            set
            {
                _condutorVeiculo = value;
            }
        }

        private List<VeiculoRodoviarioMDFe> _veiculoRodoviario = null;
        public List<VeiculoRodoviarioMDFe> VeiculoRodoviario
        {
            get
            {
                if (_veiculoRodoviario == null)
                {
                    _veiculoRodoviario = VeiculoRodoviarioMDFeDAO.Instance.ObterVeiculoRodoviarioMDFe(IdRodoviario);
                }
                return _veiculoRodoviario;
            }
            set
            {
                _veiculoRodoviario = value;
            }
        }

        private List<LacreRodoviarioMDFe> _lacreRodoviario = null;
        public List<LacreRodoviarioMDFe> LacreRodoviario
        {
            get
            {
                if(_lacreRodoviario == null)
                {
                    _lacreRodoviario = LacreRodoviarioMDFeDAO.Instance.ObterLacreRodoviarioMDFe(IdRodoviario);
                }
                return _lacreRodoviario;
            }
            set
            {
                _lacreRodoviario = value;
            }
        }
    }
}

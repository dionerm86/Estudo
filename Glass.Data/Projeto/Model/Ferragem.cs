using GDA;
using System;
using System.ComponentModel;
using Glass.Log;
using Glass.Data.DAL;
using System.Collections.Generic;
using System.Text;

namespace Glass.Data.Model
{
    [Flags]
    public enum EstiloAncoragem
    {
        [Description("Nenhuma")]
        Nenhuma = 0,

        [Description("Topo")]
        Topo = 1,

        [Description("Base")]
        Base = 2,

        [Description("Esquerda")]
        Esquerda = 4,

        [Description("Direita")]
        Direita = 8,

        [Description("Centro")]
        Centro = 16,

        [Description("Topo Esquerda")]
        TopoEsquerda = 32,

        [Description("Topo Centro")]
        TopoCentro = 64,

        [Description("Topo Direita")]
        TopoDireita = 128,

        [Description("Base Esquerda")]
        BaseEsquerda = 256,

        [Description("Base Centro")]
        BaseCentro = 512,

        [Description("Base Direita")]
        BaseDireita = 1024,

        [Description("Centro Esquerda")]
        CentroEsquerda = 2048,

        [Description("Centro Direita")]
        CentroDireita = 4096
    }

    [PersistenceBaseDAO(typeof(FerragemDAO))]
    [PersistenceClass("ferragem")]
    public class Ferragem : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IDFERRAGEM", PersistenceParameterType.IdentityKey)]
        public int IdFerragem { get; set; }

        [Log("Fabricante", "Nome", typeof(BaseDAO<FabricanteFerragemDAO>))]
        [PersistenceProperty("IDFABRICANTEFERRAGEM")]
        [PersistenceForeignKey(typeof(FabricanteFerragem), "IdFabricanteFerragem")]
        public int IdFabricanteFerragem { get; set; }

        [Log("Nome")]
        [PersistenceProperty("NOME")]
        public string Nome { get; set; }

        [Log("Situação")]
        [PersistenceProperty("SITUACAO")]
        public Situacao Situacao { get; set; }

        [Log("Pode Rotacionar")]
        [PersistenceProperty("PODEROTACIONAR")]
        public bool PodeRotacionar { get; set; }

        [Log("Pode Espelhar")]
        [PersistenceProperty("PODEESPELHAR")]
        public bool PodeEspelhar { get; set; }

        [Log("Medidas Estáticas")]
        [PersistenceProperty("MEDIDASESTATICAS")]
        public bool MedidasEstaticas { get; set; }

        [Log("Estilo Ancoragem")]
        [PersistenceProperty("ESTILOANCORAGEM")]
        public EstiloAncoragem EstiloAncoragem { get; set; }

        [Log("Altura")]
        [PersistenceProperty("ALTURA")]
        public double Altura { get; set; }

        [Log("Largura")]
        [PersistenceProperty("LARGURA")]
        public double Largura { get; set; }

        [PersistenceProperty("DATAALTERACAO")]
        public DateTime DataAlteracao { get; set; }

        private Guid? _uUID = null;

        /// <summary>
        /// Identificador único da ferragem.
        /// </summary>
        [PersistenceProperty("UUID")]
        public Guid UUID
        {
            get
            {
                if (_uUID == null)
                    _uUID = Guid.NewGuid();

                return _uUID.Value;
            }
            set
            {
                _uUID = value;
            }
        }

        #endregion

        #region Propriedades de Suporte

        private List<ConstanteFerragem> _constantedaferragem = null;

        [Log("Constante da Ferragem")]
        public string ConstanteDaFerragem
        {
            get
            {
                if (_constantedaferragem == null)
                {
                    _constantedaferragem = ConstanteFerragemDAO.Instance.ObterConstantesFerragens(IdFerragem);
                }

                var retorno = new StringBuilder();
                foreach (var dados in _constantedaferragem)
                {
                    if (!string.IsNullOrWhiteSpace(dados.Nome))
                    {
                        retorno.AppendFormat("{0}:", dados.Nome);
                    }

                    if (!string.IsNullOrWhiteSpace(dados.Valor.ToString()))
                    {
                        retorno.AppendFormat(" {0} ", dados.Valor);
                    }
                }

                return retorno.ToString().TrimEnd(',', ' ');
            }
        }

        private List<CodigoFerragem> _codferragem;

        [Log("Código da Ferragem")]
        public string CodigoDaFeragem
        {
            get
            {
                if (_codferragem == null)
                {
                    _codferragem = CodigoFerragemDAO.Instance.ObterCodigoFerragens(IdFerragem);
                }

                var retorno = new StringBuilder();

                foreach (var dados in _codferragem)
                {
                    if (!string.IsNullOrWhiteSpace(dados.Codigo))
                    {
                        retorno.AppendFormat("{0} ", dados.Codigo);
                    }
                }

                return retorno.ToString().TrimEnd(',', ' ');
            }
        }

        #endregion
    }
}

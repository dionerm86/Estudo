using GDA;
using System;
using System.ComponentModel;
using Glass.Log;

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

    [PersistenceClass("ferragem")]
    public class Ferragem : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IDFERRAGEM", PersistenceParameterType.IdentityKey)]
        public int IdFerragem { get; set; }

        [Log("Fabricante")]
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
    }
}

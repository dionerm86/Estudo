using GDA;
using System.Xml.Serialization;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(GrupoModeloDAO))]
    [XmlRoot("grupoModelo")]
	[PersistenceClass("grupo_modelo")]
	public class GrupoModelo
    {
        #region Enumeradores

        public enum SituacaoGrupoModelo
        {
            Ativo = 1,
            Inativo
        }

        public enum GrupoModeloEnum : uint
        {
            Correr=1,
            PortaPuxSimples,
            PortaPuxDuplo,
            Conj2Portas,
            Fixo,
            Bascula,
            Pivotante,
            Carrinho,
            BoxAbrir,
            CorrerKitInst
        }

        #endregion

        #region Propriedades

        [XmlAttribute("idGrupoModelo")]
        [PersistenceProperty("IDGRUPOMODELO", PersistenceParameterType.IdentityKey)]
        public uint IdGrupoModelo { get; set; }

        [Log("Descrição")]
        [XmlAttribute("descricao")]
        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        [XmlAttribute("situacao")]
        [PersistenceProperty("SITUACAO")]
        public int Situacao { get; set; }

        [Log("Box Padrão")]
        [XmlAttribute("boxPadrao")]
        [PersistenceProperty("BOXPADRAO")]
        public bool BoxPadrao { get; set; }

        [Log("Esquadria")]
        [XmlAttribute("esquadria")]
        [PersistenceProperty("Esquadria")]
        public bool Esquadria { get; set; }

        #endregion

        #region Propriedades de Suporte

        [Log("Situação")]
        [XmlIgnore]
        public string DescrSituacao
        {
            get 
            {
                switch ((SituacaoGrupoModelo)Situacao)
                {
                    case SituacaoGrupoModelo.Ativo: return "Ativo";
                    case SituacaoGrupoModelo.Inativo: return "Inativo";
                    default: return "";
                }
            }
        }

        #endregion
    }
}
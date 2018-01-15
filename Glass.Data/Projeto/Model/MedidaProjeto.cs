using GDA;
using System.Xml.Serialization;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(MedidaProjetoDAO))]
    [XmlRoot("medidaProjeto")]
    [PersistenceClass("medida_projeto")]
    public class MedidaProjeto
    {
        #region Propriedades

        [XmlAttribute("idMedidaProjeto")]
        [PersistenceProperty("IDMEDIDAPROJETO", PersistenceParameterType.IdentityKey)]
        public uint IdMedidaProjeto { get; set; }

        [Log("Grupo de Medida")]
        [XmlElement("IdGrupoMedProj")]
        [PersistenceProperty("IDGRUPOMEDPROJ")]
        public uint? IdGrupoMedProj { get; set; }

        [Log("Descrição")]
        [XmlAttribute("descricao")]
        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        [Log("Valor Padrão")]
        [XmlAttribute("valorPadrao")]
        [PersistenceProperty("VALORPADRAO")]
        public int ValorPadrao { get; set; }

        [Log("Exibir Calc. Medida Exata")]
        [XmlAttribute("exibirMedidaExata")]
        [PersistenceProperty("EXIBIRMEDIDAEXATA")]
        public bool ExibirMedidaExata { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("DESCRGRUPO", DirectionParameter.InputOptional)]
        public string DescrGrupo { get; set; }

        #endregion

        #region Propriedades de Suporte

        [XmlIgnore]
        public bool EditarVisible
        {
            get { return IdMedidaProjeto > 20; }
        }

        [XmlIgnore]
        public string DescricaoTratada
        {
            get { return TrataDescricao(Descricao).ToUpper(); }
        }

        public static string TrataDescricao(string descricao)
        {
            while (descricao.Contains("  "))
                descricao = descricao.Replace("  ", "");

            descricao = descricao.Replace(" ", "").Replace(".", "").Replace("ã", "a").Replace("á", "a").Replace("â", "a")
                .Replace("é", "e").Replace("ê", "e").Replace("í", "i").Replace("ç", "c").Replace("Ã", "A").Replace("Á", "A")
                .Replace("Â", "A").Replace("É", "E").Replace("Ê", "E").Replace("Í", "I").Replace("Ç", "C");

            return descricao;
        }

        #endregion
    }
}
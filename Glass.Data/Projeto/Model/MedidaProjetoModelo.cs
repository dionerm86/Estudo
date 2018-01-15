using GDA;
using System.Xml.Serialization;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(MedidaProjetoModeloDAO))]
    [XmlRoot("medidaProjetoModelo")]
    [PersistenceClass("medida_projeto_modelo")]
    public class MedidaProjetoModelo
    {
        #region Propriedades

        [XmlAttribute("idMedidaProjetoModelo")]
        [PersistenceProperty("IDMEDIDAPROJETOMODELO", PersistenceParameterType.IdentityKey)]
        public uint IdMedidaProjetoModelo { get; set; }

        [XmlAttribute("idProjetoModelo")]
        [PersistenceProperty("IDPROJETOMODELO")]
        public uint IdProjetoModelo { get; set; }

        [XmlAttribute("idMedidaProjeto")]
        [PersistenceProperty("IDMEDIDAPROJETO")]
        public uint IdMedidaProjeto { get; set; }

        #endregion

        #region Propriedades Estendidas

        [XmlIgnore]
        [PersistenceProperty("DESCRMEDIDA", DirectionParameter.InputOptional)]
        public string DescrMedida { get; set; }

        #endregion

        #region Propriedades de Suporte

        [XmlIgnore]
        public string TextBoxId
        {
            get { return "txt" + TrataDescricao(DescrMedida) + "MedInst"; }
        }

        [XmlIgnore]
        public string CalcTipoMedida
        {
            get { return TrataDescricao(DescrMedida).ToUpper(); }
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

        /// <summary>
        /// 1-Qtd
        /// 2-Largura Vão
        /// 3-Altura Vão
        /// 4-Largura Porta
        /// 5-Altura Porta
        /// 6-Largura Báscula
        /// 7-Altura Báscula
        /// 8-Largura Pivotante
        /// 9-Altura Pivotante
        /// 10-Largura Colante
        /// 11-Largura Passante
        /// 12-Altura Inferior
        /// 13-Altura Puxador
        /// 14-Altura Fechadura
        /// 15-Trinco
        /// 16-Espessura Tubo
        /// 17-Dist. Eixo Puxador
        /// 18-Num. Folhas
        /// 19-Largura Vão Esquerda
        /// 20-Largura Vão Direita
        /// </summary>

        #endregion
    }
}
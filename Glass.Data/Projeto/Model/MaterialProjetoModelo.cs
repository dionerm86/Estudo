using System;
using GDA;
using System.Xml.Serialization;
using Glass.Data.DAL;
using Glass.Log;
using System.ComponentModel;

namespace Glass.Data.Model
{
    /// <summary>
    /// Grau utilizado no corte do aluminio
    /// </summary>
    public enum GrauCorteEnum
    {
        /// <summary>
        /// L9090
        /// </summary>
        [Description("L9090")]
        L9090 =1,
        /// <summary>
        /// L9045
        /// </summary>
        [Description("L9045")]
        L9045,
        /// <summary>
        /// H9090
        /// </summary>
        [Description("H9090")]
        H9090,
        /// <summary>
        /// H9045
        /// </summary>
        [Description("H9045")]
        H9045,
        /// <summary>
        /// L4545
        /// </summary>
        [Description("L4545")]
        L4545,
        /// <summary>
        /// L4590
        /// </summary>
        [Description("L4590")]
        L4590,
        /// <summary>
        /// H4545
        /// </summary>
        [Description("H4545")]
        H4545,
        /// <summary>
        /// H4590
        /// </summary>
        [Description("H4590")]
        H4590
    }

    [PersistenceBaseDAO(typeof(MaterialProjetoModeloDAO))]
    [XmlRoot("materialProjetoModelo")]
	[PersistenceClass("material_projeto_modelo")]
	public class MaterialProjetoModelo
    {
        #region Propriedades

        [XmlAttribute("idMaterProjMod")]
        [PersistenceProperty("IDMATERPROJMOD", PersistenceParameterType.IdentityKey)]
        public uint IdMaterProjMod { get; set; }

        [XmlAttribute("idProjetoModelo")]
        [PersistenceProperty("IDPROJETOMODELO")]
        public uint IdProjetoModelo { get; set; }

        [XmlAttribute("idProdProj")]
        [PersistenceProperty("IDPRODPROJ")]
        public uint IdProdProj { get; set; }

        [Log("Quantidade")]
        [XmlIgnore]
        [PersistenceProperty("QTDE")]
        public int Qtde { get; set; }

        [Log("Altura")]
        [XmlIgnore]
        [PersistenceProperty("ALTURA")]
        public Single Altura { get; set; }

        [Log("Largura")]
        [XmlIgnore]
        [PersistenceProperty("LARGURA")]
        public int Largura { get; set; }

        [Log("Espessuras")]
        [XmlAttribute("Espessuras")]
        [PersistenceProperty("ESPESSURAS")]
        public string Espessuras { get; set; }

        [Log("Tot. m²")]
        [XmlIgnore]
        [PersistenceProperty("TOTM")]
        public Single TotM { get; set; }

        [Log("Cálculo Qtde.")]
        [XmlAttribute("calculoQtde")]
        [PersistenceProperty("CALCULOQTDE")]
        public string CalculoQtde { get; set; }

        [Log("Cálculo Altura")]
        [XmlAttribute("calculoAltura")]
        [PersistenceProperty("CALCULOALTURA")]
        public string CalculoAltura { get; set; }
        
        [Log("Grau")]
        [XmlElement("grauCorte")]
        [PersistenceProperty("GRAUCORTE")]
        public GrauCorteEnum? GrauCorte { get; set; }

        #endregion

        #region Propriedades Estendidas

        [XmlIgnore]
        [PersistenceProperty("IDPROD", DirectionParameter.InputOptional)]
        public uint IdProd { get; set; }

        [PersistenceProperty("CODMATERIAL", DirectionParameter.InputOptional)]
        public string CodMaterial { get; set; }

        [PersistenceProperty("DESCRPRODPROJ", DirectionParameter.InputOptional)]
        public string DescrProdProj { get; set; }

        /// <summary>
        /// 1-Aluminio
        /// 2-Ferragem
        /// 3-Outros
        /// 4-Vidro
        /// </summary>
        [XmlIgnore]
        [PersistenceProperty("TIPOPROD", DirectionParameter.InputOptional)]
        public int TipoProd { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string MotivoCancelamento { get; set; }
        
        public string DescricaoEspessura
        {
            get
            {
                var retorno = string.Empty;
                bool primeiro = true;
                if (string.IsNullOrEmpty(Espessuras))
                    return retorno;

                foreach(var espessura in Espessuras.Split(','))
                {
                    if (primeiro)
                    {
                        if (espessura == "3")
                            retorno += "03MM";
                        if (espessura == "4")
                            retorno += "04MM";
                        if (espessura == "5")
                            retorno += "05MM";
                        if (espessura == "6")
                            retorno += "06MM";
                        if (espessura == "8")
                            retorno += "08MM";
                        if (espessura == "10")
                            retorno += "10MM";
                        if (espessura == "12")
                            retorno += "12MM";
                        primeiro = false;
                    }
                    else
                    {
                        if (espessura == "3")
                            retorno += ", 03MM";
                        if (espessura == "4")
                            retorno += ", 04MM";
                        if (espessura == "5")
                            retorno += ", 05MM";
                        if (espessura == "6")
                            retorno += ", 06MM";
                        if (espessura == "8")
                            retorno += ", 08MM";
                        if (espessura == "10")
                            retorno += ", 10MM";
                        if (espessura == "12")
                            retorno += ", 12MM";
                    }
                    
                }

                return retorno;
            }
        }

        #endregion
    }
}
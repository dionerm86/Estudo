using GDA;
using System.Xml.Serialization;
using Glass.Data.DAL;
using Glass.Log;
using Glass.Data.Helper;
using System;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(PosicaoPecaIndividualDAO))]
    [PersistenceClass("posicao_peca_individual")]
    public class PosicaoPecaIndividual : IPosicaoPecaIndividual
    {
        #region Propriedades

        [XmlAttribute("idPosicaoPecaIndividual")]
        [PersistenceProperty("IDPOSPECAIND", PersistenceParameterType.IdentityKey)]
        public uint IdPosPecaInd { get; set; }

        [XmlAttribute("idPecaProjetoModelo")]
        [PersistenceProperty("IDPECAPROJMOD")]
        public uint IdPecaProjMod { get; set; }

        [XmlAttribute("item")]
        [PersistenceProperty("ITEM")]
        public int Item { get; set; }

        [Log("Coord. X")]
        [XmlAttribute("coodX")]
        [PersistenceProperty("COORDX")]
        public int CoordX { get; set; }

        [Log("Coord. Y")]
        [XmlAttribute("coordY")]
        [PersistenceProperty("COORDY")]
        public int CoordY { get; set; }

        [XmlAttribute("orientacao")]
        [PersistenceProperty("ORIENTACAO")]
        public int Orientacao { get; set; }

        [Log("Cálculo")]
        [XmlAttribute("calc")]
        [PersistenceProperty("CALC")]
        public string Calc { get; set; }

        #endregion

        #region Propriedades Estendidas

        [XmlIgnore]
        [PersistenceProperty("NUMINFO", DirectionParameter.InputOptional)]
        public long NumInfo { get; set; }

        #endregion

        #region Propriedades de suporte

        [Log("Orientação")]
        [XmlIgnore]
        public string DescrOrientacao
        {
            get
            {
                switch (Orientacao)
                {
                    case 1: return "Horizontal";
                    case 2: return "Vertical";
                    default: return "";
                }
            }
        }

        #endregion

        #region IPosicaoPecaIndividual members

        public Guid IdPosicaoPecaIndividual { get; set; }

        public Guid IdPecaItemProj { get; set; }

        public float Valor { get; set; }

        #endregion
    }
}
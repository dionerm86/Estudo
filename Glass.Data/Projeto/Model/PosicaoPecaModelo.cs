using GDA;
using System.Xml.Serialization;
using Glass.Data.DAL;
using Glass.Log;
using Glass.Data.Helper;
using System;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(PosicaoPecaModeloDAO))]
    [XmlRoot("posicaoPecaModelo")]
    [PersistenceClass("posicao_peca_modelo")]
    public class PosicaoPecaModelo : IPosicaoPeca
    {
        #region Enumeradores

        public enum OrientacaoEnum : int
        {
            Horizontal = 1,
            Vertical
        }

        #endregion

        #region Propriedades

        [XmlAttribute("idPosicaoPecaModelo")]
        [PersistenceProperty("IDPOSICAOPECAMODELO", PersistenceParameterType.IdentityKey)]
        public uint IdPosicaoPecaModelo { get; set; }

        [XmlAttribute("idProjetoModelo")]
        [PersistenceProperty("IDPROJETOMODELO")]
        public uint IdProjetoModelo { get; set; }

        [Log("Coord. X")]
        [XmlAttribute("coordX")]
        [PersistenceProperty("COORDX")]
        public int CoordX { get; set; }

        [Log("Coord. Y")]
        [XmlAttribute("coordY")]
        [PersistenceProperty("COORDY")]
        public int CoordY { get; set; }

        /// <summary>
        /// 1-Horizontal
        /// 2-Vertical
        /// </summary>
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

        #region Propriedades de Suporte

        [Log("Orientação")]
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

        public Guid IdPosicaoPeca { get; set; }

        public Guid IdItemProjeto { get; set; }

        public float Valor { get; set; }

        #endregion
    }
}
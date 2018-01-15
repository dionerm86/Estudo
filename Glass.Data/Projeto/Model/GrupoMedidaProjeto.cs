using GDA;
using Glass.Data.DAL;
using Glass.Log;
using System.Linq;
using System.Xml.Serialization;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(GrupoMedidaProjetoDAO))]
    [XmlRoot("grupoMedidaProjeto")]
    [PersistenceClass("grupo_medida_projeto")]
    public class GrupoMedidaProjeto
    {
        #region Enumeradores

        public enum TipoMedida
        {
            AlturaPuxador = 1,
            DistBordaFuro,
            EspFuroPux,
            Outros,
            DistEixoFuroPux,
            Altura1329,
            DistBorda1523,
            Altura1335_3539,
            AlturaRecorte,
            LarguraRecorte,             // 10
            AlturaDobradica,
            LarguraFuro,
            AlturaFuro,
            LarguraFuroInferior,
            LarguraFuroCentral,
            LarguraFuroSuperior,
            AlturaFuroInferior,
            AlturaFuroCentral,
            AlturaFuroSuperior,
            EspFuroSuperior,            // 20
            EspFuroInferior,
            Altura1123Bascula,
            DistBordaTrinco,
            AlturaMaoAmiga,
            DistEixoMaoAmiga,
            DistBordaMaoAmiga,
            EspFuroMaoAmiga,
            AlturaFechaduraInferior,
            AlturaFechaduraSuperior,
            AlturaDobradicaInferior,    // 30
            AlturaDobradicaCentral,
            AlturaDobradicaSuperior,
            DistBordaTrincoInf,
            DistBordaTrincoSup,
            AlturaBascula,
            LarguraRecorte1,
            LarguraRecorte2,
            DistBordaDireita,
            DistBordaEsquerda,
            AlturaChanfro,              //40
            LarguraChanfro,
            AltBaseRecorte,
            AltRecorteSuperior,
            AltRecorteInferior,
            LargRecorteEsquerda,
            AltRecorteEsquerda,
            Larg1Recorte,
            Larg2Recorte,
            AlturaTrinco,
            DistBordaRoldana,      //50
            EspRoldana,
            EspTrinco,
            AlturaRoldanaVersatik = 53,
            DistBordaRoldanaVersatik,
            RaioRoldanaVersatik,
            LargRecorteDireita,
            AltRecorteDireita
        }

        #endregion

        #region Propriedades

        [XmlAttribute("IdGrupoMedProj")]
        [PersistenceProperty("IDGRUPOMEDPROJ", PersistenceParameterType.IdentityKey)]
        public uint IdGrupoMedProj { get; set; }

        [Log("Descrição")]
        [XmlAttribute("Descricao")]
        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        #endregion

        #region Propriedades de Suporte

        public bool PodeEditarExcluir
        {
            get
            {
                return !System.Enum.GetNames(typeof(TipoMedida)).Any(f => f == Conversoes.ConverteValor<TipoMedida>(IdGrupoMedProj).ToString());
            }
        }

        #endregion
    }
}
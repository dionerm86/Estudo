using GDA;
using Glass.Data.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(FormulaExpressaoCalculoDAO))]
    [PersistenceClass("formula_expressao_calculo")]
    public class FormulaExpressaoCalculo
    {
        #region Propriedades

        [XmlAttribute("idFormulaExpreCalc")]
        [PersistenceProperty("IDFORMULAEXPRECALC", PersistenceParameterType.IdentityKey)]
        public uint IdFormulaExpreCalc { get; set; }

        [XmlAttribute("descricao")]
        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        [XmlAttribute("expressao")]
        [PersistenceProperty("EXPRESSAO")]
        public string Expressao { get; set; }

        #endregion
    }
}

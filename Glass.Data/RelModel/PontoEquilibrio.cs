using System;
using System.Collections.Generic;
using GDA;
using System.Xml.Serialization;
using System.IO;
using Glass.Data.RelDAL;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(PontoEquilibrioDAO))]
    public class PontoEquilibrio
    {
        #region Propriedades

        [PersistenceProperty("Indice", DirectionParameter.InputOptional)]
        public Int64 Indice { get; set; }

        [PersistenceProperty("Item", DirectionParameter.InputOptional)]
        public string Item { get; set; }

        [PersistenceProperty("Produtos", DirectionParameter.InputOptional)]
        public string Produtos { get; set; }

        [PersistenceProperty("Valor", DirectionParameter.InputOptional)]
        public decimal Valor { get; set; }

        [PersistenceProperty("Parent", DirectionParameter.InputOptional)]
        public Int64 Parent { get; set; }

        #endregion

        #region Propriedades de Suporte

        public List<PontoEquilibrio> subItens { get; set; }

        public string ValorString
        {
            get
            {
                return string.Format("{0:C}", Valor);
            }
        }

        public string Percentual { get; set; }

        public string SubItensSerialize
        {
            get
            {
                string ret = "";
                if (subItens != null)
                {
                    XmlSerializer serializer = new XmlSerializer(subItens.GetType());

                    using (StringWriter writer = new StringWriter())
                    {
                        serializer.Serialize(writer, subItens);

                        ret =  writer.ToString();
                    }
                }

                return ret;
            }
        }

        public PontoEquilibrio()
        {
            subItens = new List<PontoEquilibrio>();
        }

        #endregion
    }
}
using System;
using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(EntradaEstoqueDAO))]
    [PersistenceClass("entrada_estoque")]
    public class EntradaEstoque
    {
        #region Propriedades

        [PersistenceProperty("IDENTRADAESTOQUE", PersistenceParameterType.IdentityKey)]
        public uint IdEntradaEstoque { get; set; }

        [PersistenceProperty("IDLOJA")]
        public uint IdLoja { get; set; }

        [PersistenceProperty("IDFUNC")]
        public uint IdFunc { get; set; }

        [PersistenceProperty("IDCOMPRA")]
        public uint? IdCompra { get; set; }

        [PersistenceProperty("NUMERONFE")]
        public uint? NumeroNFe { get; set; }

        [PersistenceProperty("DATACAD")]
        public DateTime DataCad { get; set; }

        [PersistenceProperty("ESTORNADO")]
        public bool Estornado { get; set; }

        [PersistenceProperty("MANUAL")]
        public bool Manual { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("NOMEFUNC", DirectionParameter.InputOptional)]
        public string NomeFunc { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string Referencia
        {
            get
            {
                string retorno = "";
                if (IdCompra > 0)
                    retorno += "Compra: " + IdCompra + " ";

                if (NumeroNFe > 0)
                    retorno += "NFe: " + NumeroNFe + " ";

                return retorno.Trim();
            }
        }

        public bool PodeCancelar
        {
            get { return Manual && !Estornado; }
        }

        #endregion
    }
}
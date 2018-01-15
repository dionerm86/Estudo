using System;
using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(SaidaEstoqueDAO))]
    [PersistenceClass("saida_estoque")]
    public class SaidaEstoque
    {
        #region Propriedades

        [PersistenceProperty("IDSAIDAESTOQUE", PersistenceParameterType.IdentityKey)]
        public uint IdSaidaEstoque { get; set; }

        [PersistenceProperty("IDLOJA")]
        public uint IdLoja { get; set; }

        [PersistenceProperty("IDFUNC")]
        public uint IdFunc { get; set; }

        [PersistenceProperty("IDPEDIDO")]
        public uint? IdPedido { get; set; }

        [PersistenceProperty("IDLIBERARPEDIDO")]
        public uint? IdLiberarPedido { get; set; }

        [PersistenceProperty("IDVOLUME")]
        public uint? IdVolume { get; set; }

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
                string referencia = "";

                if (IdPedido > 0)
                    referencia += "Pedido: " + IdPedido + " ";

                if (IdLiberarPedido > 0)
                    referencia += "Liberação: " + IdLiberarPedido + " ";

                if (IdVolume > 0)
                    referencia += "Volume: " + IdVolume + " ";

                return referencia;
            }
        }

        public bool PodeCancelar
        {
            get { return Manual && !Estornado; }
        }

        #endregion
    }
}

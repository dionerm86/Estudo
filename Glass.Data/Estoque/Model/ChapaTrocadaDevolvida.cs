using System;
using GDA;
using Glass.Data.Helper;
using System.Drawing;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    public enum SituacaoChapaTrocadaDevolvida
    {
        Disponivel = 1,
        Utilizada = 2,
        Cancelada
    }

    [PersistenceBaseDAO(typeof(ChapaTrocadaDevolvidaDAO))]
    [PersistenceClass("chapa_trocada_devolvida")]
    public class ChapaTrocadaDevolvida
    {
        /// <summary>
        /// Identificador da chapa trocada/ devolvida
        /// </summary>
        [PersistenceProperty("IdChapaTrocadaDevolvida", PersistenceParameterType.IdentityKey)]
        public int IdChapaTrocadaDevolvida { get; set; }

        /// <summary>
        /// Pedido da troca/devolucao
        /// </summary>
        [PersistenceProperty("IDPEDIDO")]
        public int IdPedido { get; set; }

        /// <summary>
        /// Identificador da troca/devolucao
        /// </summary>
        [PersistenceProperty("IDTROCADEVOLUCAO")]
        public int IdTrocaDevolucao { get; set; }

        /// <summary>
        /// IdImpressao da chapa de nota fiscal
        /// </summary>
        [PersistenceProperty("IDPRODIMPRESSAOCHAPA")]
        public int IdProdImpressaoChapa { get; set; }

        /// <summary>
        /// Numero da etiqueta que foi 
        /// </summary>
        [PersistenceProperty("NUMETIQUETA")]
        public string NumEtiqueta { get; set; }

        /// <summary>
        /// Situação da chapa
        /// </summary>
        [PersistenceProperty("SITUACAO")]
        public SituacaoChapaTrocadaDevolvida Situacao { get; set; }
    }
}

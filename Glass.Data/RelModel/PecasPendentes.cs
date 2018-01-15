using System;
using GDA;
using Glass.Data.RelDAL;

namespace Glass.Data.RelModel
{
    /// <summary>
    /// Classe resposável por armazenar os dados do relatório de peças pendentes
    /// </summary>
    [PersistenceBaseDAO(typeof(PecasPendentesDAO))]
    public class PecasPendentes
    {
        #region Propiedades

        /// <summary>
        /// Data de entrega da peça
        /// </summary>
        public DateTime Data { get; set; }

        /// <summary>
        /// Nome do dia da semana da data da entrega da peça.
        /// </summary>
        public string DiaSemana 
        {
            get { return Data.ToString("dddd"); }
        }

        /// <summary>
        /// Quantida de peças
        /// </summary>
        public long Qtde { get; set; }

        /// <summary>
        /// Setor da peça
        /// </summary>
        public string Setor { get; set; }

        /// <summary>
        /// Numero da sequencia Setor da peça
        /// </summary>
        public long NumSeqSetor { get; set; }            
        
        /// <summary>
        /// Armazena o total de metro quadrado da peça
        /// </summary>
        public double TotM { get; set; }

        #endregion
    }
}
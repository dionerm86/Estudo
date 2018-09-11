using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Glass
{
    /// <summary>
    /// Representa a interface de geração de boletos
    /// </summary>
    public interface IGeradorBoleto
    {
        /// <summary>
        /// Gera o boleto
        /// </summary>
        /// <param name="codigoContaReceber"></param>
        /// <param name="codigoNotaFiscal"></param>
        /// <param name="codigoLiberacao"></param>
        /// <param name="codigoContaBanco"></param>
        /// <param name="carteira"></param>
        /// <param name="especieDocumento"></param>
        /// <param name="instrucoes"></param>
        /// <param name="conteudoBoleto"> Boleto que foi gerado</param>
        /// <returns></returns>
        Colosoft.Business.OperationResult<IEnumerable<uint>> GerarBoleto(int codigoContaReceber, int codigoNotaFiscal, int codigoLiberacao, int codigoCte,
            int codigoContaBanco, string carteira, int especieDocumento, string[] instrucoes,
            System.IO.Stream conteudoBoleto);
    }
}

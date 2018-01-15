using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Financeiro.Negocios
{
    public class ImportarArquivoCartaoNaoIdentificadoResultado : Colosoft.Business.SaveResult
    {
        public IList<Entidades.CartaoNaoIdentificado> CartoesNaoIdentificados { get; set; }

        /// <summary>
        /// Construtor Padrão (Falha)
        /// </summary>
        public ImportarArquivoCartaoNaoIdentificadoResultado(Colosoft.IMessageFormattable mensagem)
            :base(false, mensagem)
        {

        }

        /// <summary>
        /// Construtor Padrão (Sucesso)
        /// </summary>
        public ImportarArquivoCartaoNaoIdentificadoResultado(IList<Entidades.CartaoNaoIdentificado> cartoesNaoIdentificados)
            : base(true, null)
        {
            CartoesNaoIdentificados = cartoesNaoIdentificados;
        }       
    }
}

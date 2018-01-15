using System;

namespace Glass.Data.Exceptions
{
    internal class ImportarProjetoException : Exception
    {
        public ImportarProjetoException(string codigoProjeto, Exception erro) :
            base("Erro ao importar projeto." + (!String.IsNullOrEmpty(codigoProjeto) ? " Código: " + codigoProjeto : ""), erro)
        {
        }
    }
}
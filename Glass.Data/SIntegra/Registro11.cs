using Glass.Data.Model;

namespace Glass.Data.SIntegra
{
    internal class Registro11 : Registro
    {
        #region Campos Privados

        private Loja _loja = null;

        #endregion

        #region Construtores

        public Registro11(Loja loja)
        {
            _loja = loja;
        }

        #endregion

        #region Propriedades

        public override int Tipo
        {
            get { return 11; }
        }

        public string Logradouro
        {
            get { return NotNullString(_loja.Endereco); }
        }

        public string Numero
        {
            get { return NotNullString(_loja.Numero); }
        }

        public string Complemento
        {
            get { return NotNullString(_loja.Compl); }
        }

        public string Bairro
        {
            get { return NotNullString(_loja.Bairro); }
        }

        public string CEP
        {
            get { return NotNullString(_loja.Cep).Replace("-", ""); }
        }

        public string NomeContato
        {
            get { return "(sem contato)"; }
        }

        public string Telefone
        {
            get { return FormatTelefone(NotNullString(_loja.Telefone)).Replace("-", ""); }
        }

        #endregion

        #region Propriedades de Suporte

        /// <summary>
        /// Retorna o texto do registro do tipo 11 para uso do SIntegra.
        /// </summary>
        /// <returns>Uma string com os dados formatados para uso do SIntegra.</returns>
        public override string ToString()
        {
            // Formata os campos para o retorno da função
            string n01 = Tipo.ToString().PadLeft(2, '0');
            string n02 = Logradouro.PadRight(34);
            string n03 = Numero.PadLeft(5, '0');
            string n04 = Complemento.PadRight(22);
            string n05 = Bairro.PadRight(15);
            string n06 = CEP.PadLeft(8, '0');
            string n07 = NomeContato.PadRight(28);
            string n08 = Telefone.PadLeft(12, '0');

            // Retorna o texto formatado.
            return n01 + n02 + n03 + n04 + n05 + n06 + n07 + n08;
        }

        #endregion
    }
}
namespace Glass.Financeiro.Negocios.Entidades.Dominio
{
    internal class R000
    {
        #region Variaveis Locais

        private string _cnpj;

        #endregion

        #region Contrutor

        public R000(string cnpj)
        {
            _cnpj = cnpj;
        }

        #endregion

        #region Métodos publicos

        public void Serializar(System.IO.TextWriter writer)
        {
            var ret = "";

            ret += "0000|";
            ret += _cnpj + "|";

            writer.WriteLine(ret);
        }

        #endregion
    }
}

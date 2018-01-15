namespace Glass.Data.Model
{
    public class TabelaSped
    {
        #region Propriedades de Suporte

        public TabelaSped(string codigo, string descricao)
        {
            Codigo = codigo;
            Descricao = descricao;
        }

        public string Codigo { get; set; }

        public string Descricao { get; set; }

        #endregion
    }
}
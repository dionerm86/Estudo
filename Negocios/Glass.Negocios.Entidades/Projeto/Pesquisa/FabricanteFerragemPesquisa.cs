namespace Glass.Projeto.Negocios.Entidades
{
    public class FabricanteFerragemPesquisa
    {
        #region Propriedades

        public int IdFabricanteFerragem { get; set; }

        public string Nome { get; set; }

        public string Sitio { get; set; }

        public bool PodeEditar
        {
            get { return true; }
        }

        #endregion
    }
}

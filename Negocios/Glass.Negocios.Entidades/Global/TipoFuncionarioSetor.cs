namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Classe que armazenas as informações do tipo de funcionário
    /// associado com o seteor.
    /// </summary>
    public class TipoFuncSetor
    {
        #region Propriedades

        /// <summary>
        /// Identificador do tipo de funcionario.
        /// </summary>
        public int IdTipoFunc { get; set; }

        /// <summary>
        /// Descrição.
        /// </summary>
        public string Descricao { get; set; }

        /// <summary>
        /// Identificador do setor de produção associado.
        /// </summary>
        public int? IdSetorProducao { get; set; }

        /// <summary>
        /// Identificador do tipo de funcionário com o setor.
        /// </summary>
        public string TipoFuncComSetor
        {
            get { return IdTipoFunc + (IdSetorProducao > 0 ? "," + IdSetorProducao : ""); }
        }

        #endregion
    }
}

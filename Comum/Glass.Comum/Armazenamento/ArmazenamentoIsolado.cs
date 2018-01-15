namespace Glass.Armazenamento
{
    /// <summary>
    /// Classe que processo as informações para acesso ao 
    /// armazenamento isolado do sistema.
    /// </summary>
    public static class ArmazenamentoIsolado
    {
        #region Local Variables

        private static string _diretorioSistema;

        #endregion

        #region Propriedades

        /// <summary>
        /// Diretório base do sistema.
        /// </summary>
        public static string DiretorioSistema
        {
            get { return _diretorioSistema; }
        }

        /// <summary>
        /// Diretório dos uploads do sistema.
        /// </summary>
        public static string DiretorioUpload
        {
            get { return System.IO.Path.Combine(DiretorioSistema, "Upload"); }
        }

        /// <summary>
        /// Diretório das imagens do sistema (Images).
        /// </summary>
        public static string DiretorioImages
        {
            get { return System.IO.Path.Combine(DiretorioSistema, "Images"); }
        }

        public static string DiretorioRelatorios
        {
            get { return System.IO.Path.Combine(DiretorioSistema, "Relatorios"); }
        }
        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Configura os diretórios.
        /// </summary>
        /// <param name="diretorioSistema"></param>
        public static void Configure(string diretorioSistema)
        {
            _diretorioSistema = diretorioSistema;
        }

        #endregion
    }
}

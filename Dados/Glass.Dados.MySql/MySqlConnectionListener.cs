namespace Glass.Dados.MySql
{
    /// <summary>
    /// Implementação do Listener das conexões do GDA.
    /// </summary>
    public class MySqlConnectionListener : GDA.GDAConnectionListener
    {
        /// <summary>
        /// Método acionado quando uma conexão for criada.
        /// </summary>
        /// <param name="connection">connection.</param>
        public override void NotifyConnectionCreated(System.Data.IDbConnection connection)
        {
        }

        /// <summary>
        /// Método acionado quando uma conexão for aberta.
        /// </summary>
        /// <param name="connection">connection.</param>
        public override void NotifyConnectionOpened(System.Data.IDbConnection connection)
        {
            // Executa o comando para altera o nível de isolamento das consultas.
            var cmd = connection.CreateCommand();
            var nomeParametroTipoTransacao = string.Empty;

            if (System.Configuration.ConfigurationManager.AppSettings["UsarNovaVersaoMySql"]?.ToLower() == "true")
            {
                nomeParametroTipoTransacao = "transaction_isolation";
            }
            else
            {
                nomeParametroTipoTransacao = "tx_isolation";
            }

            // Possibilita consultas sujas e desabilita as verificações de chaves estrangeiras.
            cmd.CommandText = $@"SET SESSION {nomeParametroTipoTransacao} = 'READ-UNCOMMITTED';
                SET SESSION foreign_key_checks = 0;";
            cmd.ExecuteNonQuery();
        }
    }
}

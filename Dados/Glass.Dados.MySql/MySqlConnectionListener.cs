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
            var cmd = connection.CreateCommand();

            var sql = @"SET SESSION {0} = 'READ-UNCOMMITTED';
                SET SESSION foreign_key_checks = 0;";

            try
            {
                cmd.CommandText = string.Format(sql, "tx_isolation");
                cmd.ExecuteNonQuery();
            }
            catch
            {
                cmd.CommandText = string.Format(sql, "transaction_isolation");
                cmd.ExecuteNonQuery();
            }
        }
    }
}

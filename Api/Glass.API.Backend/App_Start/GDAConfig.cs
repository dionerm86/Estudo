// <copyright file="GdaConfig.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System.Web.Http;

namespace Glass.API.Backend
{
    /// <summary>
    /// Classe com a configuração do GDA.
    /// </summary>
    public static class GdaConfig
    {
        /// <summary>
        /// Inicia a configuração do GDA no warm-up da aplicação.
        /// </summary>
        /// <param name="config">Configuração HTTP da aplicação.</param>
        public static void Initialize(HttpConfiguration config)
        {
            GDA.GDAConnectionManager.Listeners.Add(new Dados.MySql.MySqlConnectionListener());

            GDA.GDASettings.LoadConfiguration();
            GDA.GDASession.DefaultCommandTimeout = 60;
            GDA.GDAOperations.DebugTrace += GDAOperations_DebugTrace;
        }

        private static void GDAOperations_DebugTrace(object sender, string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }
    }
}

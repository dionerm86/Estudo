// <copyright file="CustomOutputDriverProvider.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using CalcEngine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Glass.Projeto
{
    /// <summary>
    /// Implementação base do provedor dos drivers de saída.
    /// </summary>
    public class CustomOutputDriverProvider : IOutputDriverProvider
    {
        private CalcEngine.Services.RemoteOutputDriverProvider remoteOutputDriverProvider;
        private string servidor;
        private int porta;

        private void VerificarConfiguracao()
        {
            if (this.servidor != Glass.Configuracoes.PCPConfig.ServidorCalcEngine ||
                this.porta != Glass.Configuracoes.PCPConfig.PortaCalcEngine)
            {
                this.remoteOutputDriverProvider = null;
            }

            if (remoteOutputDriverProvider == null)
            {
                this.servidor = Glass.Configuracoes.PCPConfig.ServidorCalcEngine;
                this.porta = Glass.Configuracoes.PCPConfig.PortaCalcEngine;

                this.remoteOutputDriverProvider = new CalcEngine.Services.RemoteOutputDriverProvider(
                    new Uri($"http://{this.servidor}:{this.porta}/api/outputdriverprovider/"), null);
            }
        }

        /// <inheritdoc />
        public async Task<IOutputDriver> GetDriver(string name)
        {
            this.VerificarConfiguracao();
            return await this.remoteOutputDriverProvider.GetDriver(name);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<OutputDriverInfo>> GetDrivers()
        {
            this.VerificarConfiguracao();
            return await this.remoteOutputDriverProvider.GetDrivers();
        }
    }
}

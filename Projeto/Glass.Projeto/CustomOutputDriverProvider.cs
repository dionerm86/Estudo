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
        private IOutputDriverProvider localOutputDriverProvider;
        private string servidor;
        private int porta;

        private void VerificarConfiguracao()
        {
            if (this.servidor != Configuracoes.PCPConfig.ServidorCalcEngine ||
                this.porta != Configuracoes.PCPConfig.PortaCalcEngine)
            {
                this.remoteOutputDriverProvider = null;
            }

            if (remoteOutputDriverProvider == null && !string.IsNullOrEmpty(Configuracoes.PCPConfig.ServidorCalcEngine))
            {
                this.servidor = Configuracoes.PCPConfig.ServidorCalcEngine;
                this.porta = Configuracoes.PCPConfig.PortaCalcEngine;

                this.remoteOutputDriverProvider = new CalcEngine.Services.RemoteOutputDriverProvider(
                    new Uri($"http://{this.servidor}:{this.porta}/api/outputdriverprovider/"), null);
            }
            else if (localOutputDriverProvider == null)
            {
                localOutputDriverProvider = new OutputDriverProvider(
                    new[]
                    {
                        new CalcEngine.Dxf.DxfOutputDriver()
                    });
            }
        }

        private IOutputDriverProvider GetProvider()
        {
            this.VerificarConfiguracao();
            return this.remoteOutputDriverProvider ?? this.localOutputDriverProvider;
        }

        /// <inheritdoc />
        public async Task<IOutputDriver> GetDriver(string name)
        {
            return await this.GetProvider().GetDriver(name);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<OutputDriverInfo>> GetDrivers()
        {
            this.VerificarConfiguracao();
            return await this.GetProvider().GetDrivers();
        }
    }
}

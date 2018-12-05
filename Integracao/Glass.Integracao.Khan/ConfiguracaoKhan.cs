// <copyright file="ConfiguracaoKhan.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Integracao.Khan
{
    /// <summary>
    /// Representa a configuração do integrador.
    /// </summary>
    internal class ConfiguracaoKhan : ConfiguracaoIntegrador
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ConfiguracaoKhan"/>.
        /// </summary>
        public ConfiguracaoKhan()
        {
            this.EnderecoBase = System.Configuration.ConfigurationManager.AppSettings["Khan:EnderecoBase"];
            this.Token = System.Configuration.ConfigurationManager.AppSettings["Khan:Token"];
            this.Empresa = System.Configuration.ConfigurationManager.AppSettings["Khan:Empresa"];

            var executarJobs = false;
            if (bool.TryParse(System.Configuration.ConfigurationManager.AppSettings["Khan:ExecutarJobs"], out executarJobs))
            {
                this.ExecutarJobs = executarJobs;
            }
            else
            {
                this.ExecutarJobs = false;
            }
        }

        /// <summary>
        /// Obtém ou define o endereço base dos serviços.
        /// </summary>
        public string EnderecoBase
        {
            get
            {
                return this[nameof(this.EnderecoBase)] as string;
            }

            set
            {
                this[nameof(this.EnderecoBase)] = value;
            }
        }

        /// <summary>
        /// Obtém ou define o token de comunicação.
        /// </summary>
        public string Token
        {
            get
            {
                return this[nameof(this.Token)] as string;
            }

            set
            {
                this[nameof(this.Token)] = value;
            }
        }

        /// <summary>
        /// Obtém ou define no nome da empresa para a integração.
        /// </summary>
        public string Empresa
        {
            get
            {
                return this[nameof(this.Empresa)] as string;
            }

            set
            {
                this[nameof(this.Empresa)] = value;
            }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se é para executar os jobs da integração.
        /// </summary>
        public bool ExecutarJobs
        {
            get
            {
                return (this[nameof(this.ExecutarJobs)] as bool?).GetValueOrDefault();
            }

            set
            {
                this[nameof(this.ExecutarJobs)] = value;
            }
        }

        /// <inheritdoc />
        public override bool VerificarSomenteLeitura(string nome)
        {
            return true;
        }
    }
}

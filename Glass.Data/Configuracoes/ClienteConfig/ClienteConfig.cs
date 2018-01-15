﻿using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public static partial class ClienteConfig
    {
        /// <summary>
        /// Ao cadastrar cliente, se o funcionário não for administrador ou financeiro, será cadastrado como inativo
        /// </summary>
        public static bool CadastrarClienteInativo
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.CadastrarClienteInativo); }
        }

        /// <summary>
        /// Define que ao entrar na tela de listagem de clientes, serão exibidos por padrão apenas os ativos
        /// </summary>
        public static bool ListarAtivosPadrao
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ListarAtivosPadrao); }
        }

        /// <summary>
        /// Define que ao buscar a observação do cliente via AJAX o limite será validado
        /// </summary>
        public static bool ValidarLimiteAoBuscarObs
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ValidarLimiteAoBuscarObs); }
        }

        /// <summary>
        /// Define que será exibida a razão social no Gráfico de Vendas (Curva ABC).
        /// </summary>
        public static bool ExibirRazaoSocialGraficoVendasCurvaABC
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirRazaoSocialGraficoVendasCurvaABC); }
        }
    }
}

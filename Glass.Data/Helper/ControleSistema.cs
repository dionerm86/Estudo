using System;
using System.Collections.Generic;

namespace Glass.Data.Helper
{
    /// <summary>
    /// Classe que indica o cliente do sistema.
    /// </summary>
    public static class ControleSistema
    {
        // Dicionário de controle da configuração
        private static Dictionary<uint, DadosControleSistema> _clienteSistema = 
            new Dictionary<uint, DadosControleSistema>();
        
        #region Classe de suporte

        private class DadosControleSistema
        {
            //public const uint MAX_NUMERO_VEZES = 0;
            public uint NumeroVezes { get; set; }
            public ClienteSistema Cliente { get; set; }
        }

        #endregion

        #region Enumeradores

        /// <summary>
        /// Enumerador com os clientes do sistema.
        /// </summary>
        public enum ClienteSistema
        {
            // Manter nessa ordem
            _NaoIdentificado,

            // Organizar pela primeira letra, para
            // melhor localização posterior

            #region "A"

            AllDayGlass,
            Alternativa,
            AlternativaMOC,
            AlumiTemper,
            AmazonBoxManaus,
            AmazonCuiaba,
            AmazonJuazeiro,
            AmazonLojaCE,
            AmazonMossoro,
            AmazonRecife,
            AmazonTemperCE,
            AmazonTemperMA,
            AmazonTemperMT,
            AmazonTemperMTConf,
            AmazonTemperSC,
            Americana,
            AmvidConf,
            Annis,
            Arcuare,
            ArtVidrosMT,
            ArtVidrosRO,
            AtacadaoDoBox,

            #endregion

            #region "B"
            
            BeBBox,
            BellTemper,
            BlueHouse,
            BendGlass,
            BrasilTemper,
            BrasilTemperDF,
            Brazividros,

            #endregion

            #region "C"

            CapitalVidros,
            CarboneAmericana,
            CarbonyTemper,
            CasaDosEspelhos,
            CenterBox,
            Charneca,
            ClearGlass,
            CliqueDivisorias,
            CMV,
            Colpany,
            Contemper,
            ConquistaVidros,
            CriarteVidros,
            CristalBox,
            CristalForte,
            CristalVidros,

            #endregion

            #region "D"

            Dekor,
            Dividros,
            Diviglass,
            Divipam,
            Divine,
            DuboxVidrosBH,

            #endregion

            #region "E"
            ElohimVidros,
            EmVidros,
            Envidrart,
            EspacoVidraceiro,
            EspacoVidros,
            EstruturalVidros,

            #endregion

            #region "F"

            FastTemper,
            FerreiraVidros,
            Funcional,

            #endregion

            #region "G"

            GlassTem,
            GlobalTemper,
            GloboVidrosDF,
            GmVidros,
            GoldGlass,
            GrupoProjeta,
            GuaporeIndustria,
            GuaporeSinop,
            GuaporeVidros,

            #endregion

            #region "H"

            HomeVidros,

            #endregion

            #region "I"

            ImperioDosVidros,
            IndMegaTemper,
            InfinitoVidros,
            Invitra,

            #endregion

            #region "L"

            LaminaTemper,
            LibVidroService,
            LisboaVidro,
            LiteAeaVidro,
            LiteBaldex,
            LiteBrasilTemper,
            LiteBraziVidros,
            LiteGrtm,
            LiteRogerinhoVidros,
            LiteVidracariaInovaBetim,
            LiteVidracariaPlanalto,
            LiteVidroLider,
            LiteVidroNobre,
            LiteVidroNop,
            LiteVidroVip,
            LojaDosEspelhos,
            LojaDosEspelhosSantarem,
            LuzirTemper,

            #endregion

            #region "M"
            
            MarraVidros,
            MatrixVidros,
            MBTemper,
            MegaSolucoes,
            MegaTemper,
            Mercosul,
            MinasLaminacao,
            Mirandex,
            MirandexAcre,
            MirandexPortoVelho,
            ModeloVidros,
            MsVidros,
            MundialTemper,

            #endregion

            #region "N"
            NoroesteVidros,
            NVidros,
            NRC,

            #endregion

            #region "O"
            OReidoBox,
            OuroPreto,
            OuroVidros,

            #endregion

            #region "P"

            PadraoAlternativa,
            PerfectGlass,
            PlanaltoVidros,
            PontoVidraceiro,
            Projevidros,

            #endregion

            #region "R"

            RealVidros,
            ReflectaEsquadrias,
            RiberVidros,
            RpVidros,

            #endregion

            #region "S"

            SaVidros,
            ScComercio,
            SiaGlass,
            Simonica,
            SulGlass,
            SyncLite,
            SyncProjEsp,

            #endregion

            #region "T"

            Tempera,
            TemperadosEstrela,
            TemperForte,
            TemperGlass,
            TemperGlassBA,
            TemperiVidros,
            TemperMax,
            TemperSete,
            Tempex,
            Termari,
            Terra,
            TesteLite,
            TotalGlass,
            TotalVidros,

            #endregion

            #region "U"

            UniaoBox,
            UniversalBox,
            UniversoVidros,
            UniVidros,
            UseVidros,

            #endregion

            #region "V"

            VarandaLivre,
            Vdram,
            VetroRio,
            Vidrex,
            VidracariaBorbaGato,
            VidracariaDamasceno,
            VidracariaPestana,
            Vidralia,
            VidroCel,
            VidroCenter,
            VidroCenterLoja,
            VidroCenterJoaoPessoa,
            VidroFenix,
            VidroLife,
            VidroMetro,
            VidroMove,
            VidroQualy,
            VidroRapido,
            VidrosDresch,
            VidroSer,
            VidrosEVidros,
            VidroTecdoor,
            VidroTech,
            VidroValle,
            VidroVip,
            Vintage,
            Vipal,
            VisaoVidros,
            VitralManaus,
            VitralVarejo,
            Vitrol,
            Vitrus,
            VmGlobal,

            #endregion
        }

        #endregion

        /// <summary>
        /// Retorna a empresa que está executando o sistema.
        /// </summary>
        /// <returns></returns>
        public static ClienteSistema GetSite()
        {
            return GetSite(0);
        }

        /// <summary>
        /// Retorna a empresa que está executando o sistema.
        /// </summary>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        public static ClienteSistema GetSite(uint idLoja)
        {
            // Usa a mesma configuração para todas as lojas
            idLoja = 0;

            if (!_clienteSistema.ContainsKey(idLoja))
            {
                lock (_clienteSistema)
                    if (!_clienteSistema.ContainsKey(idLoja))
                    {
                        // Recupera a configuração do web.config
                        string site = System.Configuration.ConfigurationManager.AppSettings["sistema"];
                        
                        // Converte para o Enum
                        ClienteSistema cliente;
                        try { cliente = Conversoes.ConverteValor<ClienteSistema>(Enum.Parse(typeof(ClienteSistema), site, true)); }
                        catch { cliente = ClienteSistema._NaoIdentificado; }

                        // Verifica se o cliente foi identificado
                        if (cliente == ClienteSistema._NaoIdentificado)
                            throw new Exception("Empresa não identificada. Verificar web.config.");

                        // Adiciona ao dicionário de controle
                       _clienteSistema.Add(idLoja, new DadosControleSistema
                        {
                            NumeroVezes = 0,
                            Cliente = cliente
                        });
                    }
            }

            // Verifica se o valor deve ser lido novamente do arquivo de configuração
            /* if (DadosControleSistema.MAX_NUMERO_VEZES > 0 && 
                ++_clienteSistema[idLoja].NumeroVezes == DadosControleSistema.MAX_NUMERO_VEZES)
            {
                _clienteSistema.Remove(idLoja);
                return GetSite(idLoja);
            }
            else */
                return _clienteSistema[idLoja].Cliente;
        }

        #region Configurações

        /// <summary>
        /// Retorna se a empresa esta executando o sistema em ambiente de testes.
        /// </summary>
        public static bool AmbienteTeste
        {
            get
            {
                var config = System.Configuration.ConfigurationManager.AppSettings["AmbienteTeste"];
                return config != null && config.ToLower() == "true";
            }
        }

        #region Projeto

        /// <summary>
        /// Retorna se a empresa trabalha com projetos por cor e espessura.
        /// </summary>
        public static bool CalcularProjetosPorCorEspessura
        {
            get
            {
                var config = System.Configuration.ConfigurationManager.AppSettings["CalcularProjetosPorCorEspessura"];
                return config != null && config.ToLower() == "true";
            }
        }

        #endregion

        #region E-mail

        /// <summary>
        /// Retorna se a empresa utiliza SSL no envio de e-mail.
        /// </summary>
        public static bool AtivarSslEnvio
        {
            get
            {
                var config = System.Configuration.ConfigurationManager.AppSettings["AtivarSslEnvio"];
                return config == null || config.ToLower() == "true";
            }
        }

        /// <summary>
        /// Retorna a porta que será utilizada no envio de e-mail.
        /// </summary>
        public static int? PortaEnvioEmail
        {
            get
            {
                var usarPorta587Email = System.Configuration.ConfigurationManager.AppSettings["usarPorta587Email"];

                if (usarPorta587Email != null && usarPorta587Email.ToLower() == "true")
                    return 587;

                var config = System.Configuration.ConfigurationManager.AppSettings["portaEnvioEmail"];
                return config != null && config.StrParaIntNullable() > 0 ? config.StrParaInt() : (int?)null;
            }
        }

        #endregion

        #endregion
    }
}

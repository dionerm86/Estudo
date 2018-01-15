namespace Glass.Data.EFD
{
    internal sealed class RecuperaConfiguracao : Glass.Pool.Singleton<RecuperaConfiguracao>, Sync.Fiscal.EFD.Suporte.IConfiguracao
    {
        private RecuperaConfiguracao() { }

        public string PerfilArquivo
        {
            get { return Glass.Configuracoes.FiscalConfig.PerfilArquivoEfdFiscal.ToString(); }
        }

        public bool ZerarDadosIcmsNotaEntradaSeHouverStRegistroC190
        {
            /* Chamado 36186. */
            get { return false; }
            //get { return ControleSistema.GetSite() != ControleSistema.ClienteSistema.VidroCel && ControleSistema.GetSite() != ControleSistema.ClienteSistema.Simonica; }
        }

        public bool ZerarDadosIpiCstIgual49RegistroC190
        {
            get { return Configuracoes.FiscalConfig.ZerarDadosIpiCstIgual49RegistroC190; }
        }

        public string CodigoAjusteAproveitamentoCreditoIcms
        {
            get { return Glass.Configuracoes.FiscalConfig.CodigoAjusteAproveitamentoCreditoIcms; }
        }

        public Sync.Fiscal.EFD.DataSources.TipoControleSaldoCreditoIcms TipoControleSaldoCreditoIcms
        {
            get { return Glass.Configuracoes.FiscalConfig.TipoControleSaldoCreditoIcms; }
        }

        public float PercAproveitamentoCreditoIcms
        {
            get { return Glass.Configuracoes.FiscalConfig.PercAproveitamentoCreditoIcms; }
        }

        public bool ZerarDadosRegistroC170NotaSaidaSeCst10
        {
            get { return false; }
        }

        public bool ZerarDadosRegistroC170NotaSaidaSeCst60
        {
            get { return Configuracoes.FiscalConfig.ZerarDadosRegistroC170NotaSaidaSeCst60; }
        }
        
        public bool ZerarDadosIcmsRegistroC190NotaEntradaSaidaSeCst60
        {
            get { return Configuracoes.FiscalConfig.ZerarDadosIcmsRegistroC190NotaEntradaSaidaSeCst60; }
        }
    }
}

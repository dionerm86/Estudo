using GDA;
using Glass.Data.DAL;
using System;
using System.ComponentModel;

namespace Glass.Data.Model
{
    /// <summary>
    /// Possíveis situações dos clientes do sistema.
    /// </summary>
    public enum TipoNotificacaoEnum
    {
        [Description("Não Definido")]
        NaoDefinido,

        [Description("Desconto Excedente")]
        DescontoExcedente,

        [Description("Resumo Administrativo")]
        ResumoAdministrativo,

        [Description("Setor Inoperante")]
        SetorInoperante,

        [Description("Comercial Inoperante")]
        ComercialInoperante,

        [Description("Faturamento Inoperante")]
        FaturamentoInoperante,

        [Description("Mensagem")]
        Mensagem
    }

    [PersistenceBaseDAO(typeof(NotificacaoDAO))]
    [PersistenceClass("notificacao")]
    public class Notificacao : Colosoft.Data.BaseModel
    {
        [PersistenceProperty("IdNotificacao", PersistenceParameterType.IdentityKey)]
        public int IdNotificacao { get; set; }

        [PersistenceProperty("IdSetor")]
        public int IdSetor { get; set; }

        [PersistenceProperty("IdMensagem")]
        public int IdMensagem { get; set; }

        [PersistenceProperty("TipoNotificacao")]
        public TipoNotificacaoEnum TipoNotificacao { get; set; }

        [PersistenceProperty("Assunto")]
        public string Assunto { get; set; }

        [PersistenceProperty("Mensagem")]
        public string Mensagem { get; set; }

        [PersistenceProperty("IdDestinatario")]
        public int IdDestinatario { get; set; }

        [PersistenceProperty("Remetente")]
        public string Remetente { get; set; }

        [PersistenceProperty("DataCad")]
        public DateTime DataCad { get; set; }

        [PersistenceProperty("Enviada")]
        public bool Enviada { get; set; }        
    }
}

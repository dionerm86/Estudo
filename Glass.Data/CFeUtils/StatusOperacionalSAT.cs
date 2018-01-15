using System;

namespace Glass.Data.CFeUtils
{
    #region Enumeradores

    public enum StatusOperacional
    {
        Desbloqueado  = 0,
        Bloqueio_Sefaz = 1, 
        Bloqueio_Contribuinte = 2,
        Bloqueio_Autonomo = 3,
        Bloqueio_Para_Desativacao = 4
    }

    #endregion

    public class StatusOperacionalSAT
    {
        #region Propriedades

        /// <summary>
        /// numero de Serie do SAT
        /// </summary>
        public string NumeroSerieSAT { get; set; }
        
        /// <summary>
        /// Tipo de LAN
        /// </summary>
        public string TipoLAN { get; set; }

        /// <summary>
        /// IP do Aparelho
        /// </summary>
        public string IpAparelho{get;set;}
        
        /// <summary>
        /// MAC do Aparelho
        /// </summary>
        public string MacAdress { get; set; }
                
        /// <summary>
        /// Máscara de Sub-rede
        /// </summary>
        public string MascaraSubRede { get; set; }
        
        /// <summary>
        /// Endereço do Gateway da rede
        /// </summary>
        public string Gateway { get; set; }
                
        /// <summary>
        /// Endereço do DNS1
        /// </summary>
        public string DNS1 { get; set; }

        /// <summary>
        /// Endereço do DNS2
        /// </summary>
        public string DNS2 { get; set; }

        /// <summary>
        /// Status da Rede
        /// </summary>
        public string StatusRede { get; set; }
                
        /// <summary>
        /// Nível de Bateria do Aparelho
        /// </summary>
        public string NivelBateria { get; set; }
                
        /// <summary>
        /// Memória Total do Aparelho
        /// </summary>
        public string MemoriaTotal { get; set; }
                
        /// <summary>
        /// Memória em uso
        /// </summary>
        public string MemoriaUsada { get; set; }
                
        /// <summary>
        /// Data Atual do Aparelho
        /// </summary>
        public DateTime DataAtualAparelho { get; set; }

        /// <summary>
        /// Versão do Software Básico (BIOS) do aparelho
        /// </summary>
        public string VersaoSoftwareBasico { get; set; }
                
        /// <summary>
        /// Versão do Layout da tabela de informações
        /// </summary>
        public string VersaoLayoutTabInf { get; set; }
                
        /// <summary>
        /// Número do último CFe emitido
        /// </summary>
        public string UltimoCFeEmitido { get; set; }
        
        /// <summary>
        /// Número sequencial do primeiro CFe Armazenado
        /// </summary>
        public string PrimeiroCFeArmazenado { get; set; }
                
        /// <summary>
        /// Sequencial do ultimo CFe armazenado
        /// </summary>
        public string UltimoCFeArmazenado { get; set; }
                
        /// <summary>
        /// Data e hora da ultima transmissao do SAT ao SEFAZ
        /// </summary>
        public DateTime UltimaTransmissaoSEFAZ { get; set; }
        
        /// <summary>
        /// Data e hora da ultima comunicação com a SEFAZ
        /// </summary>
        public DateTime UltimaComunicacaoSEFAZ { get; set; }

        /// <summary>
        /// Data de emissão do certificado digital assinado no aparelho
        /// </summary>
        public DateTime EmissaoCertDigital { get; set; }
        
        /// <summary>
        /// Data vencimento do certificado digital assinado no aparelho
        /// </summary>
        public DateTime VencimentoCertDigital { get; set; }

        /// <summary>
        /// Status Operacional do SAT
        /// </summary>
        public StatusOperacional StatusOperacional { get; set; }

        #endregion
    }
}
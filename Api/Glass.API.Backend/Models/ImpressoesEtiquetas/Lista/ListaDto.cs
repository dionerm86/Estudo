// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.ImpressoesEtiquetas.V1.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de um fornecedor para a tela de listagem de impressões de etiquetas.
    /// </summary>
    [DataContract(Name = "ImpressaoEtiqueta")]
    public class ListaDto : IdDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="impressaoEtiqueta">A model de impressão de etiqueta.</param>
        internal ListaDto(Data.Model.ImpressaoEtiqueta impressaoEtiqueta)
        {
            this.Id = (int)impressaoEtiqueta.IdImpressao;
            this.Loja = impressaoEtiqueta.NomeLoja;
            this.Funcionario = impressaoEtiqueta.NomeFunc;
            this.DataImpressao = impressaoEtiqueta.Data;
            this.Situacao = impressaoEtiqueta.DescrSituacao;
            this.TipoImpressao = impressaoEtiqueta.DescrTipoImpressao;
            this.CorLinha = impressaoEtiqueta.Situacao == (int)Data.Model.ImpressaoEtiqueta.SituacaoImpressaoEtiqueta.Cancelada
                ? "Red"
                : "Black";

            this.Permissoes = new PermissoesDto()
            {
                Imprimir = impressaoEtiqueta.Situacao != (int)Data.Model.ImpressaoEtiqueta.SituacaoImpressaoEtiqueta.Processando,
                BaixarArquivoOtimizacao = impressaoEtiqueta.Situacao == (int)Data.Model.ImpressaoEtiqueta.SituacaoImpressaoEtiqueta.Ativa
                    && !Configuracoes.PCPConfig.Etiqueta.UsarPlanoCorte,

                AbrirECutter = Configuracoes.EtiquetaConfig.TipoExportacaoEtiqueta == Data.Helper.DataSources.TipoExportacaoEtiquetaEnum.eCutter
                    && impressaoEtiqueta.IdArquivoOtimizacao > 0,

                Cancelar = impressaoEtiqueta.Situacao != (int)Data.Model.ImpressaoEtiqueta.SituacaoImpressaoEtiqueta.Cancelada
                    && ((Config.PossuiPermissao(Config.FuncaoMenuPCP.ImprimirEtiquetasMaoDeObra) && impressaoEtiqueta.IdFunc == UserInfo.GetUserInfo.CodUser)
                        || Config.PossuiPermissao(Config.FuncaoMenuPCP.CancelarImpressaoEtiqueta)),

                LogCancelamento = LogCancelamentoDAO.Instance.TemRegistro(Data.Model.LogCancelamento.TabelaCancelamento.ImpressaoEtiqueta, impressaoEtiqueta.IdImpressao),
            };
        }

        /// <summary>
        /// Obtém ou define a loja da impressão de etiqueta.
        /// </summary>
        [JsonProperty("loja")]
        public string Loja { get; set; }

        /// <summary>
        /// Obtém ou define o funcionário da impressão de etiqueta.
        /// </summary>
        [JsonProperty("funcionario")]
        public string Funcionario { get; set; }

        /// <summary>
        /// Obtém ou define a data da impressão da etiqueta.
        /// </summary>
        [JsonProperty("dataImpressao")]
        public DateTime DataImpressao { get; set; }

        /// <summary>
        /// Obtém ou define a situação da impressão de etiqueta.
        /// </summary>
        [JsonProperty("situacao")]
        public string Situacao { get; set; }

        /// <summary>
        /// Obtém ou define o tipo da impressão de etiqueta.
        /// </summary>
        [JsonProperty("tipoImpressao")]
        public string TipoImpressao { get; set; }

        /// <summary>
        /// Obtém ou define a cor da linha da tabela.
        /// </summary>
        [JsonProperty("corLinha")]
        public string CorLinha { get; set; }

        /// <summary>
        /// Obtém ou define a lista de permissões concedidas na tela.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }
    }
}

// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.NotasFiscais;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.API.Backend.Models.NotasFiscais.V1.TiposParticipantes;
using Glass.Data.DAL;
using Glass.Data.Model;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Estoques.V1.Movimentacoes.Reais
{
    /// <summary>
    /// Classe que encapsula os dados de movimentação de estoque real para a tela de listagem.
    /// </summary>
    [DataContract(Name = "MovimentacoesEstoqueReal")]
    public class ListaDto : IdDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="movimentacaoEstoqueReal">A model de movimentação do estoque real.</param>
        internal ListaDto(MovEstoque movimentacaoEstoqueReal)
        {
            this.Id = (int)movimentacaoEstoqueReal.IdMovEstoque;
            this.Referencia = movimentacaoEstoqueReal.Referencia;
            this.Produto = new IdNomeDto
            {
                Id = (int)movimentacaoEstoqueReal.IdProd,
                Nome = movimentacaoEstoqueReal.DescrProduto,
            };

            this.Fornecedor = movimentacaoEstoqueReal.NomeFornec;
            this.Funcionario = movimentacaoEstoqueReal.NomeFunc;
            this.Datas = new DataCadastroMovimentacaoDto
            {
                Cadastro = movimentacaoEstoqueReal.DataCad,
                Movimentacao = movimentacaoEstoqueReal.DataMov,
            };

            this.DadosEstoque = new DadosEstoqueDto
            {
                TipoMovimentacao = movimentacaoEstoqueReal.DescrTipoMov,
                Unidade = movimentacaoEstoqueReal.CodUnidade,
                Quantidade = movimentacaoEstoqueReal.QtdeMov,
                SaldoQuantidade = movimentacaoEstoqueReal.SaldoQtdeMov,
                Valor = movimentacaoEstoqueReal.ValorMov,
                SaldoValor = movimentacaoEstoqueReal.SaldoValorMov,
            };

            this.Observacao = movimentacaoEstoqueReal.Obs;
            this.Permissoes = new PermissoesDto
            {
                Excluir = movimentacaoEstoqueReal.DeleteVisible,
                LogAlteracoes = LogAlteracaoDAO.Instance.TemRegistro(LogAlteracao.TabelaAlteracao.MovEstoque, movimentacaoEstoqueReal.IdMovEstoque, null),
            };

            this.CorLinha = this.ObterCorLinha();
        }

        /// <summary>
        /// Obtém ou define a referencia da movimentação.
        /// </summary>
        [DataMember]
        [JsonProperty("referencia")]
        public string Referencia { get; set; }

        /// <summary>
        /// Obtém ou define o produto associado a movimentação.
        /// </summary>
        [DataMember]
        [JsonProperty("produto")]
        public IdNomeDto Produto { get; set; }

        /// <summary>
        /// Obtém ou define o fornecedor do produto movimentado.
        /// </summary>
        [DataMember]
        [JsonProperty("fornecedor")]
        public string Fornecedor { get; set; }

        /// <summary>
        /// Obtém ou define o funcionário do produto movimentado.
        /// </summary>
        [DataMember]
        [JsonProperty("funcionario")]
        public string Funcionario { get; set; }

        /// <summary>
        /// Obtém ou define as datas de cadastro e movimentação do produto movimentado.
        /// </summary>
        [DataMember]
        [JsonProperty("datas")]
        public DataCadastroMovimentacaoDto Datas { get; set; }

        /// <summary>
        /// Obtém ou define dados pertinentes ao estoque.
        /// </summary>
        [DataMember]
        [JsonProperty("dadosEstoque")]
        public DadosEstoqueDto DadosEstoque { get; set; }

        /// <summary>
        /// Obtém ou define a observação da movimentação.
        /// </summary>
        [DataMember]
        [JsonProperty("observacao")]
        public string Observacao { get; set; }

        /// <summary>
        /// Obtém ou define as permissões utilizadas na tela.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }

        /// <summary>
        /// Obtém ou define a cor da fonte (baseado no tipo de movimentação).
        /// </summary>
        [DataMember]
        [JsonProperty("corLinha")]
        public string CorLinha { get; set; }

        private string ObterCorLinha()
        {
            return this.DadosEstoque.TipoMovimentacao.ToUpper() == "S" ? "red" : "black";
        }
    }
}

// <copyright file="ConverterCadastroAtualizacaoParaFuncionario.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Funcionarios.CadastroAtualizacao;
using System;

namespace Glass.API.Backend.Helper.Funcionarios
{
    /// <summary>
    /// Classe que realiza a tradução entre o DTO e a entidade para cadastro ou atualização de funcionário.
    /// </summary>
    internal class ConverterCadastroAtualizacaoParaFuncionario
    {
        private readonly CadastroAtualizacaoDto cadastro;
        private readonly Lazy<Global.Negocios.Entidades.Funcionario> funcionario;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ConverterCadastroAtualizacaoParaFuncionario"/>.
        /// </summary>
        /// <param name="cadastro">O DTO de cadastro, enviado para o endpoint.</param>
        /// <param name="atual">O funcionário atual (opcional), para que sejam aproveitados os valores, se necessário.</param>
        public ConverterCadastroAtualizacaoParaFuncionario(CadastroAtualizacaoDto cadastro, Global.Negocios.Entidades.Funcionario atual = null)
        {
            this.cadastro = cadastro;
            this.funcionario = new Lazy<Global.Negocios.Entidades.Funcionario>(() =>
            {
                var destino = atual ?? new Global.Negocios.Entidades.Funcionario();
                this.ConverterDtoParaModelo(destino);

                return destino;
            });
        }

        /// <summary>
        /// Realiza a conversão para a model.
        /// </summary>
        /// <returns>A entidade de funcionario preenchida.</returns>
        public Global.Negocios.Entidades.Funcionario ConverterParaFuncionario()
        {
            return this.funcionario.Value;
        }

        private void ConverterDtoParaModelo(Global.Negocios.Entidades.Funcionario destino)
        {
            destino.Nome = this.cadastro.ObterValorNormalizado(c => c.Nome, destino.Nome);
            destino.IdTipoFunc = this.cadastro.ObterValorNormalizado(c => c.IdTipoFuncionario, destino.IdTipoFunc);
            destino.IdLoja = this.cadastro.ObterValorNormalizado(c => c.IdLoja, destino.IdLoja);
            destino.Situacao = this.cadastro.ObterValorNormalizado(c => c.Situacao, destino.Situacao);
            destino.TipoPedido = this.cadastro.ObterValorNormalizado(c => string.Join(",", c.IdsTiposPedidos), destino.TipoPedido);
            destino.NumDiasAtrasarPedido = this.cadastro.ObterValorNormalizado(c => c.NumeroDiasParaAtrasarPedidos, destino.NumDiasAtrasarPedido);
            destino.NumeroPdv = this.cadastro.ObterValorNormalizado(c => c.NumeroPdv.StrParaInt(), destino.NumeroPdv);
            destino.Obs = this.cadastro.ObterValorNormalizado(c => c.Observacao, destino.Obs);

            this.ConverterDadosSetores(destino);
            this.ConverterDadosEndereco(destino);
            this.ConverterDadosContatos(destino);
            this.ConverterDadosDocumentosEDadosPessoais(destino);
            this.ConverterDadosAcesso(destino);
        }

        private void ConverterDadosSetores(Global.Negocios.Entidades.Funcionario destino)
        {
            foreach (var idSetor in this.cadastro.IdsSetores)
            {
                destino.FuncionarioSetores.Add(new Global.Negocios.Entidades.FuncionarioSetor { IdSetor = idSetor});
            }
        }

        private void ConverterDadosEndereco(Global.Negocios.Entidades.Funcionario destino)
        {
            destino.Endereco = this.cadastro.Endereco.Logradouro;
            destino.Compl = this.cadastro.Endereco.Complemento;
            destino.Bairro = this.cadastro.Endereco.Bairro;
            destino.Cidade = this.cadastro.Endereco.Cidade.Nome;
            destino.Uf = this.cadastro.Endereco.Cidade.Uf;
            destino.Cep = this.cadastro.Endereco.Cep;
        }

        private void ConverterDadosContatos(Global.Negocios.Entidades.Funcionario destino)
        {
            destino.TelRes = this.cadastro.Contatos.TelefoneResidencial;
            destino.TelCel = this.cadastro.Contatos.TelefoneCelular;
            destino.TelCont = this.cadastro.Contatos.TelefoneContato;
            destino.Email = this.cadastro.Contatos.Email;
            destino.Ramal = this.cadastro.Contatos.Ramal;
        }

        private void ConverterDadosDocumentosEDadosPessoais(Global.Negocios.Entidades.Funcionario destino)
        {
            destino.Rg = this.cadastro.DocumentosEDadosPessoais.Rg;
            destino.Cpf = this.cadastro.DocumentosEDadosPessoais.Cpf;
            destino.Funcao = this.cadastro.DocumentosEDadosPessoais.Funcao;
            destino.EstCivil = this.cadastro.DocumentosEDadosPessoais.EstadoCivil;
            destino.DataNasc = this.cadastro.DocumentosEDadosPessoais.DataNascimento;
            destino.DataEnt = this.cadastro.DocumentosEDadosPessoais.DataEntrada;
            destino.DataSaida = this.cadastro.DocumentosEDadosPessoais.DataSaida;
            destino.Salario = this.cadastro.DocumentosEDadosPessoais.Salario;
            destino.Gratificacao = this.cadastro.DocumentosEDadosPessoais.Gratificacao;
            destino.NumCarteiraTrabalho = this.cadastro.DocumentosEDadosPessoais.NumeroCTPS;
            destino.AuxAlimentacao = this.cadastro.DocumentosEDadosPessoais.AuxilioAlimentacao;
            destino.Registrado = this.cadastro.DocumentosEDadosPessoais.Registrado;
        }

        private void ConverterDadosAcesso(Global.Negocios.Entidades.Funcionario destino)
        {
            destino.Login = this.cadastro.Acesso.Login;
            destino.Senha = this.cadastro.Acesso.Senha;
        }
    }
}

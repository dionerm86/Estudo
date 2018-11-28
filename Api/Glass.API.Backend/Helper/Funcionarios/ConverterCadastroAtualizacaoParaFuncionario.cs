// <copyright file="ConverterCadastroAtualizacaoParaFuncionario.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Funcionarios.V1.CadastroAtualizacao;
using System;
using System.Linq;
using Glass.Global.Negocios.Entidades;
using Glass.Data.DAL;
using System.Collections.Generic;

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
        /// /// <param name="funcionarioFluxo">O fluxo responsavel pela.</param>
        public ConverterCadastroAtualizacaoParaFuncionario(
            Global.Negocios.IFuncionarioFluxo funcionarioFluxo,
            CadastroAtualizacaoDto cadastro,
            Global.Negocios.Entidades.Funcionario atual = null)
        {
            this.cadastro = cadastro;
            this.funcionario = new Lazy<Global.Negocios.Entidades.Funcionario>(() =>
            {
                var destino = atual ?? funcionarioFluxo.CriarFuncionario();
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

        private void ConverterDtoParaModelo(Funcionario destino)
        {
            destino.Nome = this.cadastro.ObterValorNormalizado(c => c.Nome, destino.Nome);
            destino.IdTipoFunc = this.cadastro.ObterValorNormalizado(c => c.IdTipoFuncionario, destino.IdTipoFunc);
            destino.IdLoja = this.cadastro.ObterValorNormalizado(c => c.IdLoja, destino.IdLoja);
            destino.Situacao = this.cadastro.ObterValorNormalizado(c => c.Situacao, destino.Situacao);
            destino.NumDiasAtrasarPedido = this.cadastro.ObterValorNormalizado(c => c.NumeroDiasParaAtrasarPedidos, destino.NumDiasAtrasarPedido);
            destino.NumeroPdv = this.cadastro.ObterValorNormalizado(c => c.NumeroPdv, destino.NumeroPdv);
            destino.Obs = this.cadastro.ObterValorNormalizado(c => c.Observacao, destino.Obs);

            this.ConverterDadosTiposPedido(destino);
            this.ConverterDadosSetores(destino);
            this.ConverterDadosEndereco(destino);
            this.ConverterDadosContatos(destino);
            this.ConverterDadosDocumentosEDadosPessoais(destino);
            this.ConverterDadosAcesso(destino);
            this.ConverterDadosPermissao(destino);
        }

        private void ConverterDadosTiposPedido(Funcionario destino)
        {
            if (!this.cadastro.VerificarCampoInformado(c => c.IdsTiposPedidos))
            {
                return;
            }

            var valorDestino = destino.TipoPedido?.Split(',')
                .Select(tipoPedido => tipoPedido.StrParaInt());

            var valorNormalizado = this.cadastro.ObterValorNormalizado(c => c.IdsTiposPedidos, valorDestino);

            destino.TipoPedido = valorNormalizado != null
                ? string.Join(",", valorNormalizado.Select(tipoPedido => tipoPedido))
                : null;
        }

        private void ConverterDadosSetores(Funcionario destino)
        {
            string setores = string.Empty;
            var atualizados = new List<FuncionarioSetor>();
            var setoresAdicionados = string.Empty;
            var setoresRemovidos = string.Empty;

            if (!this.cadastro.VerificarCampoInformado(c => c.IdsSetores))
            {
                return;
            }

            foreach (var idSetor in this.cadastro.IdsSetores)
            {
                var funcionarioSetor = destino.FuncionarioSetores.FirstOrDefault(f => f.IdSetor == idSetor);

                if (funcionarioSetor == null)
                {
                   funcionarioSetor = new FuncionarioSetor
                    {
                        IdSetor = idSetor,
                    };

                   destino.FuncionarioSetores.Add(funcionarioSetor);

                   setoresAdicionados += SetorDAO.Instance.ObtemDescricaoSetor(funcionarioSetor.IdSetor) + "\n";
                }

                atualizados.Add(funcionarioSetor);
            }

            // Essa ordenação segue a ordem dos setores na tela, portanto, caso uma seja alterada a outra também deverá ser.
            // Recupera os setores que devem ser apagados
            foreach (var i in destino.FuncionarioSetores.Where(f => !atualizados.Exists(x => f.Equals(x))).OrderBy(f => SetorDAO.Instance.ObtemNumSeq(null, f.IdSetor)).ToArray())
            {
                setoresRemovidos += SetorDAO.Instance.ObtemDescricaoSetor(i.IdSetor) + "\n";
                destino.FuncionarioSetores.Remove(i);
            }

            if (destino.IdFunc > 0)
            {
                LogAlteracaoDAO.Instance.LogFuncionarioSetor(destino.IdFunc, setoresRemovidos, setoresAdicionados);
            }
        }

        private void ConverterDadosEndereco(Funcionario destino)
        {
            if (!this.cadastro.VerificarCampoInformado(c => c.Endereco))
            {
                return;
            }

            destino.Endereco = this.cadastro.Endereco.ObterValorNormalizado(c => c.Logradouro, destino.Endereco);
            destino.Compl = this.cadastro.Endereco.ObterValorNormalizado(c => c.Complemento, destino.Compl);
            destino.Bairro = this.cadastro.Endereco.ObterValorNormalizado(c => c.Bairro, destino.Bairro);
            destino.Cep = this.cadastro.Endereco.ObterValorNormalizado(c => c.Cep, destino.Cep);

            if (this.cadastro.Endereco.VerificarCampoInformado(c => c.Cidade))
            {
                destino.Cidade = this.cadastro.Endereco.Cidade?.ObterValorNormalizado(c => c.Nome, destino.Cidade);
                destino.Uf = this.cadastro.Endereco.Cidade?.ObterValorNormalizado(c => c.Uf, destino.Uf);
            }
        }

        private void ConverterDadosContatos(Funcionario destino)
        {
            if (!this.cadastro.VerificarCampoInformado(c => c.Contatos))
            {
                return;
            }

            destino.TelRes = this.cadastro.Contatos.ObterValorNormalizado(c => c.TelefoneResidencial, destino.TelRes);
            destino.TelCel = this.cadastro.Contatos.ObterValorNormalizado(c => c.TelefoneCelular, destino.TelCel);
            destino.TelCont = this.cadastro.Contatos.ObterValorNormalizado(c => c.TelefoneContato, destino.TelCont);
            destino.Email = this.cadastro.Contatos.ObterValorNormalizado(c => c.Email, destino.Email);
            destino.Ramal = this.cadastro.Contatos.ObterValorNormalizado(c => c.Ramal, destino.Ramal);
        }

        private void ConverterDadosDocumentosEDadosPessoais(Funcionario destino)
        {
            if (!this.cadastro.VerificarCampoInformado(c => c.DocumentosEDadosPessoais))
            {
                return;
            }

            destino.Rg = this.cadastro.DocumentosEDadosPessoais.ObterValorNormalizado(c => c.Rg, destino.Rg);
            destino.Cpf = this.cadastro.DocumentosEDadosPessoais.ObterValorNormalizado(c => c.Cpf, destino.Cpf);
            destino.Funcao = this.cadastro.DocumentosEDadosPessoais.ObterValorNormalizado(c => c.Funcao, destino.Funcao);
            destino.EstCivil = this.cadastro.DocumentosEDadosPessoais.ObterValorNormalizado(c => c.EstadoCivil, destino.EstCivil);
            destino.DataNasc = this.cadastro.DocumentosEDadosPessoais.ObterValorNormalizado(c => c.DataNascimento, destino.DataNasc);
            destino.DataEnt = this.cadastro.DocumentosEDadosPessoais.ObterValorNormalizado(c => c.DataEntrada, destino.DataEnt);
            destino.DataSaida = this.cadastro.DocumentosEDadosPessoais.ObterValorNormalizado(c => c.DataSaida, destino.DataSaida);
            destino.Salario = this.cadastro.DocumentosEDadosPessoais.ObterValorNormalizado(c => c.Salario, destino.Salario);
            destino.Gratificacao = this.cadastro.DocumentosEDadosPessoais.ObterValorNormalizado(c => c.Gratificacao, destino.Gratificacao);
            destino.NumCarteiraTrabalho = this.cadastro.DocumentosEDadosPessoais.ObterValorNormalizado(c => c.NumeroCTPS, destino.NumCarteiraTrabalho);
            destino.AuxAlimentacao = this.cadastro.DocumentosEDadosPessoais.ObterValorNormalizado(c => c.AuxilioAlimentacao, destino.AuxAlimentacao);
            destino.Registrado = this.cadastro.DocumentosEDadosPessoais.ObterValorNormalizado(c => c.Registrado, destino.Registrado);
            destino.NumPis = this.cadastro.DocumentosEDadosPessoais.ObterValorNormalizado(c => c.NumeroPis, destino.NumPis);
        }

        private void ConverterDadosAcesso(Funcionario destino)
        {
            if (!this.cadastro.VerificarCampoInformado(c => c.Acesso))
            {
                return;
            }

            destino.Login = this.cadastro.Acesso.ObterValorNormalizado(c => c.Login, destino.Login);
            destino.Senha = this.cadastro.Acesso.ObterValorNormalizado(c => c.Senha, destino.Senha);
        }

        private void ConverterDadosPermissao(Funcionario destino)
        {
            if (!this.cadastro.VerificarCampoInformado(c => c.Permissoes))
            {
                return;
            }

            destino.HabilitarChat = this.cadastro.Permissoes.ObterValorNormalizado(c => c.UtilizarChat, destino.HabilitarChat);
            destino.HabilitarControleUsuarios = this.cadastro.Permissoes.ObterValorNormalizado(c => c.HabilitarControleUsuarios, destino.HabilitarControleUsuarios);
        }
    }
}

using System;
using System.Collections.Generic;

namespace Glass.Global.Negocios
{
    /// <summary>
    /// Assinatura do fluxo de negócio de clientes.
    /// </summary>
    public interface IClienteFluxo
    {
        #region Cliente

        /// <summary>
        /// Cria uma nova instancia do cliente.
        /// </summary>
        /// <returns></returns>
        Entidades.Cliente CriarCliente();

        /// <summary>
        /// Pesquisa os clientes do sistema.
        /// </summary>
        /// <returns></returns>
        IList<Entidades.ClientePesquisa> PesquisarClientes(int? idCliente, string nomeOuApelido, string cpfCnpj, int? idLoja, string telefone,
            string logradouro, string bairro, int? idCidade, int[] idsTipoCliente, int[] situacao, string codigoRota, int? idVendedor,
            Data.Model.TipoFiscalCliente[] tiposFiscais, int[] formasPagto, DateTime? dataCadastroIni, DateTime? dataCadastroFim,
            DateTime? dataSemCompraIni, DateTime? dataSemCompraFim, DateTime? dataInativadoIni, DateTime? dataInativadoFim, DateTime? dataNascimentoIni,
            DateTime? dataNascimentoFim, int? idTabelaDescontoAcrescimo, bool apenasSemRota, int limite, string uf, string tipoPessoa, bool comCompra);

        /// <summary>
        /// Altera o vendedor dos clientes que preenchem aos requisitos.
        /// </summary>
        Colosoft.Business.SaveResult AlterarVendedorClientes(int? idCliente, string nomeOuApelido, string cpfCnpj, int? idLoja, string telefone,
            string logradouro, string bairro, int? idCidade, int[] idsTipoCliente, int[] situacao, string codigoRota, int? idVendedor,
            Data.Model.TipoFiscalCliente[] tiposFiscais, int[] formasPagto, DateTime? dataCadastroIni, DateTime? dataCadastroFim,
            DateTime? dataSemCompraIni, DateTime? dataSemCompraFim, DateTime? dataInativadoIni, DateTime? dataInativadoFim, int? idTabelaDescontoAcrescimo,
            bool apenasSemRota, int idVendedorNovo, string uf);

        /// <summary>
        /// Altera a Rota dos clientes que preenchem aos requisitos.
        /// </summary>
        Colosoft.Business.SaveResult AlterarRotaClientes(int? idCliente, string nomeOuApelido, string cpfCnpj, int? idLoja, string telefone,
            string logradouro, string bairro, int? idCidade, int[] idsTipoCliente, int[] situacao, string codigoRota, int? idVendedor,
            Data.Model.TipoFiscalCliente[] tiposFiscais, int[] formasPagto, DateTime? dataCadastroIni, DateTime? dataCadastroFim,
            DateTime? dataSemCompraIni, DateTime? dataSemCompraFim, DateTime? dataInativadoIni, DateTime? dataInativadoFim, int? idTabelaDescontoAcrescimo,
            bool apenasSemRota, int idRotaNova, string uf);

        /// <summary>
        /// Ativa todos os clientes que preenchem aos requisitos e que estejam inativos.
        /// </summary>
        Colosoft.Business.SaveResult AtivarClientesInativos(int? idCliente, string nomeOuApelido, string cpfCnpj, int? idLoja, string telefone,
            string logradouro, string bairro, int? idCidade, int[] idsTipoCliente, string codigoRota, int? idVendedor,
            Data.Model.TipoFiscalCliente[] tiposFiscais, int[] formasPagto, DateTime? dataCadastroIni, DateTime? dataCadastroFim,
            DateTime? dataSemCompraIni, DateTime? dataSemCompraFim, DateTime? dataInativadoIni, DateTime? dataInativadoFim, int? idTabelaDescontoAcrescimo,
            bool apenasSemRota, string uf);

        /// <summary>
        /// Recupera os dados para o relatório de fichas de clientes.
        /// </summary>
        IList<Entidades.FichaCliente> PesquisarFichasClientes(int? idCliente, string nomeOuApelido, string cpfCnpj, int? idLoja, string telefone,
            string logradouro, string bairro, int? idCidade, int[] idsTipoCliente, int[] situacao, string codigoRota, int? idVendedor,
            Data.Model.TipoFiscalCliente[] tiposFiscais, int[] formasPagto, DateTime? dataCadastroIni, DateTime? dataCadastroFim,
            DateTime? dataSemCompraIni, DateTime? dataSemCompraFim, DateTime? dataInativadoIni, DateTime? dataInativadoFim, int? idTabelaDescontoAcrescimo,
            bool apenasSemRota, string uf);

        /// <summary>
        /// Recupera os descritores dos clientes.
        /// </summary>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObtemClientes();

        /// <summary>
        /// Recupera o cliente pelo código informado.
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        Entidades.Cliente ObtemCliente(int IdCli);

        /// <summary>
        /// Verifica existência do cliente pelo nome
        /// </summary>
        bool VerificarClientePeloNome(string nome);

        /// <summary>
        /// Recupera o descritor de um cliente.
        /// </summary>
        /// <returns></returns>
        Colosoft.IEntityDescriptor ObtemDescritorCliente(int idCliente);

        /// <summary>
        /// Salva os dados do cliente retornando o id inserido.
        /// </summary>
        /// <param name="cliente"></param>
        /// <returns></returns>
        Glass.Negocios.Global.SalvarClienteResultado SalvarClienteRetornando(Entidades.Cliente cliente);

        /// <summary>
        /// Salva os dados do cliente.
        /// </summary>
        /// <param name="cliente"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult SalvarCliente(Entidades.Cliente cliente);

        /// <summary>
        /// Apaga os dados do cliente.
        /// </summary>
        /// <param name="cliente"></param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarCliente(Entidades.Cliente cliente);

        #endregion

        #region Tipo de cliente

        /// <summary>
        /// Pesquisa os tipos de cliente do sistema.
        /// </summary>
        /// <returns></returns>
        IList<Entidades.TipoCliente> PesquisaTiposCliente();

        /// <summary>
        /// Recupera os descritores dos tipo de cliente.
        /// </summary>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObtemDescritoresTipoCliente();

        /// <summary>
        /// Recupera os descritores dos tipos de cliente pelos identificadores informados.
        /// </summary>
        /// <param name="idsTipoCliente"></param>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObtemTiposCliente(IEnumerable<int> idsTipoCliente);

        /// <summary>
        /// Recupera os dados do tipo de cliente.
        /// </summary>
        /// <param name="idTipoCliente"></param>
        /// <returns></returns>
        Entidades.TipoCliente ObtemTipoCliente(int idTipoCliente);

        /// <summary>
        /// Salva os dados do tipo de cliente.
        /// </summary>
        /// <param name="tipoCliente"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult SalvarTipoCliente(Entidades.TipoCliente tipoCliente);

        /// <summary>
        /// Apaga o tipo de cliente.
        /// </summary>
        /// <param name="tipoCliente"></param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarTipoCliente(Entidades.TipoCliente tipoCliente);

        #endregion

        #region Grupo cliente

        /// <summary>
        /// Pesquisa os grupos de cliente do sistema.
        /// </summary>
        /// <returns></returns>
        IList<Entidades.GrupoCliente> PesquisarGruposCliente();

        /// <summary>
        /// Recupera os descritores dos grupo de cliente.
        /// </summary>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObterDescritoresGrupoCliente();

        /// <summary>
        /// Recupera os descritores dos grupos de cliente pelos identificadores informados.
        /// </summary>
        /// <param name="idsGrupoCliente"></param>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObterGruposCliente(IEnumerable<int> idsGrupoCliente);

        /// <summary>
        /// Recupera os dados do grupo de cliente.
        /// </summary>
        /// <param name="idGrupoCliente"></param>
        /// <returns></returns>
        Entidades.GrupoCliente ObterGrupoCliente(int idGrupoCliente);

        /// <summary>
        /// Salva os dados do grupo de cliente.
        /// </summary>
        /// <param name="grupoCliente"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult SalvarGrupoCliente(Entidades.GrupoCliente grupoCliente);

        /// <summary>
        /// Apaga o grupo de cliente.
        /// </summary>
        /// <param name="grupoCliente"></param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarGrupoCliente(Entidades.GrupoCliente grupoCliente);

        #endregion

        #region Tabela de desconto/acréscimo

        /// <summary>
        /// Cria uma nova instancia da tabela de desconto/acréscimo.
        /// </summary>
        /// <returns></returns>
        Entidades.TabelaDescontoAcrescimoCliente CriarTabelaDescontoAcrescimoCliente();

        /// <summary>
        /// Recupera a tabela de desconto/acréscimo pelo código informado.
        /// </summary>
        /// <returns></returns>
        Entidades.TabelaDescontoAcrescimoCliente ObtemTabelaDescontoAcrescimoCliente(int IdTabelaDesconto);

        /// <summary>
        /// Recupera as tabelas de desconto/acréscimo cadastradas no sistema.
        /// </summary>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObtemDescritoresTabelaDescontoAcrescimo();

        /// <summary>
        /// Pesquisa as tabelas os descontos/acréscimos do sistema.
        /// </summary>
        /// <returns></returns>
        IList<Entidades.TabelaDescontoAcrescimoCliente> PesquisarTabelasDescontosAcrescimos();

        /// <summary>
        /// Salva os dados da tabela de desconto/acréscimo.
        /// </summary>
        Colosoft.Business.SaveResult SalvarTabelaDescontoAcrescimo(Entidades.TabelaDescontoAcrescimoCliente tabelaDesconto);

        /// <summary>
        /// Apaga os dados da tabela de desconto/acréscimo.
        /// </summary>
        Colosoft.Business.DeleteResult ApagarTabelaDescontoAcrescimo(Entidades.TabelaDescontoAcrescimoCliente tabelaDesconto);

        #endregion

        #region Desconto/acréscimo

        /// <summary>
        /// Pesquisa os descontos/acréscimos do sistema.
        /// </summary>
        /// <returns></returns>
        IList<Entidades.DescontoAcrescimoClientePesquisa> PesquisarDescontosAcrescimos(int? idCliente,
            int? idTabelaDesconto, int? idGrupoProd, int? idSubgrupoProd, string codProduto, string produto, Situacao? situacao);

        /// <summary>
        /// Recupera os dados do desconto/acréscimo.
        /// </summary>
        /// <param name="IdDesconto"></param>
        /// <returns></returns>
        Entidades.DescontoAcrescimoCliente ObtemDescontoAcrescimo(int IdDesconto);

        /// <summary>
        /// Salva os dados do desconto/acréscimo.
        /// </summary>
        /// <param name="descontoAcrescimo"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult SalvarDescontoAcrescimo(Entidades.DescontoAcrescimoCliente descontoAcrescimo);

        #endregion
    }
}

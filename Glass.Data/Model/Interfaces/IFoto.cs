using System;
using System.Collections.Generic;
using Glass.Data.DAL;
using Glass.Data.Helper;

namespace Glass.Data.Model
{
    public abstract class IFoto
    {
        #region Construtores

        public static IFoto Nova(TipoFoto tipo)
        {
            switch (tipo)
            {
                case TipoFoto.Medicao: return new FotosMedicao();
                case TipoFoto.Compra: return new FotosCompra();
                case TipoFoto.Cliente: return new FotosCliente();
                case TipoFoto.Pedido: return new FotosPedido();
                case TipoFoto.Liberacao: return new FotosLiberacao();
                case TipoFoto.Orcamento: return new FotosOrcamento();
                case TipoFoto.DevolucaoPagto: return new FotosDevolucaoPagto();
                case TipoFoto.ImpostoServ: return new FotosImpostoServ();
                case TipoFoto.TrocaDevolucao: return new FotosTrocaDevolucao();
                case TipoFoto.ConciliacaoBancaria: return new FotosConciliacaoBancaria();
                case TipoFoto.Pagto: return new FotosPagto();
                case TipoFoto.Cheque: return new FotosCheques();
                case TipoFoto.Acerto: return new FotosAcerto();
                case TipoFoto.PagtoAntecipado: return new FotosPagtoAntecipado();
                case TipoFoto.Obra: return new FotosObra();
                case TipoFoto.Sugestao: return new FotosSugestao();
                case TipoFoto.PedidoInterno: return new FotosPedidoInterno();
                case TipoFoto.Fornecedor: return new FotosFornecedor();
                default: throw new NotImplementedException(tipo.ToString());
            }
        }

        #endregion

        #region Enumeradores

        public enum TipoFoto
        {
            Medicao,
            Compra,
            Cliente,
            Pedido,
            Liberacao,
            Orcamento,
            DevolucaoPagto,
            ImpostoServ,
            TrocaDevolucao,
            ConciliacaoBancaria,
            Pagto,
            Cheque,
            Acerto,
            PagtoAntecipado,
            Obra,
            Sugestao,
            PedidoInterno,
            Fornecedor
        }

        #endregion

        #region Propriedades Abstratas

        public abstract uint IdFoto { get; set; }
        public abstract uint IdParent { get; set; }
        public abstract string Descricao { get; set; }
        public abstract string Extensao { get; set; }
        public abstract TipoFoto Tipo { get; }
        public abstract string CodInterno { get; set; }
        public abstract float AreaQuadrada { get; set; }
        public abstract float MetroLinear { get; set; }
        public abstract bool PermiteCalcularArea { get; }
        public abstract bool ApenasImagens { get; }

        #endregion

        #region Propriedades de Suporte

        private string _path = string.Empty;

        public string Path
        {
            get
            {
                switch (Tipo)
                {
                    case TipoFoto.Medicao: return Utils.GetFotosMedicaoPath;
                    case TipoFoto.Compra: return Utils.GetFotosCompraPath;
                    case TipoFoto.Cliente: return Utils.GetFotosClientePath;
                    case TipoFoto.Pedido: return !string.IsNullOrEmpty(_path) ? _path : Utils.GetFotosPedidoPath;
                    case TipoFoto.Liberacao: return Utils.GetFotosLiberacaoPath;
                    case TipoFoto.Orcamento: return Utils.GetFotosOrcamentoPath;
                    case TipoFoto.DevolucaoPagto: return Utils.GetFotosDevolucaoPagtoPath;
                    case TipoFoto.ImpostoServ: return Utils.GetFotosImpostoServPath;
                    case TipoFoto.TrocaDevolucao: return Utils.GetFotosTrocaDevolucaoPath;
                    case TipoFoto.ConciliacaoBancaria: return Utils.GetFotosConciliacaoBancariaPath;
                    case TipoFoto.Pagto: return Utils.GetFotosPagtoPath;
                    case TipoFoto.Cheque: return Utils.GetFotosChequesPath;
                    case TipoFoto.Acerto: return Utils.GetFotosAcertoPath;
                    case TipoFoto.PagtoAntecipado: return Utils.GetFotosPagtoAntecipadoPath;
                    case TipoFoto.Obra: return Utils.GetFotosObraPath;
                    case TipoFoto.Sugestao: return Utils.GetFotosSugestaoPath;
                    case TipoFoto.PedidoInterno: return Utils.GetFotosPedidoInternoPath;
                    case TipoFoto.Fornecedor: return Utils.GetFotosFornecedorPath;
                    default: throw new NotImplementedException(Tipo.ToString());
                }
            }
            set
            {
                if (Tipo == TipoFoto.Pedido)
                    _path = value;
                else
                    throw new Exception("Apenas Foto do Tipo Pedido pode ter o valor alterado.");
            }
        }

        public string VirtualPath
        {
            get
            {
                switch (Tipo)
                {
                    case TipoFoto.Medicao: return "../Upload/Medicoes";
                    case TipoFoto.Compra: return "../Upload/Compras";
                    case TipoFoto.Cliente: return "../Upload/Clientes";
                    case TipoFoto.Pedido: return "../Upload/Pedidos";
                    case TipoFoto.Liberacao: return "../Upload/Liberacoes";
                    case TipoFoto.Orcamento: return "../Upload/Orcamentos";
                    case TipoFoto.DevolucaoPagto: return "../Upload/DevolucaoPagto";
                    case TipoFoto.ImpostoServ: return "../Upload/ImpostoServ";
                    case TipoFoto.TrocaDevolucao: return "../Upload/TrocaDevolucao";
                    case TipoFoto.ConciliacaoBancaria: return "../Upload/ConciliacaoBancaria";
                    case TipoFoto.Pagto: return "../Upload/Pagto";
                    case TipoFoto.Cheque: return "../Upload/Cheque";
                    case TipoFoto.Acerto: return "../Upload/Acerto";
                    case TipoFoto.PagtoAntecipado: return "../Upload/PagtoAntecipado";
                    case TipoFoto.Obra: return "../Upload/Obra";
                    case TipoFoto.Sugestao: return "../Upload/Sugestao";
                    case TipoFoto.PedidoInterno: return "../Upload/PedidoInterno";
                    case TipoFoto.Fornecedor: return "../Upload/Fornecedores";
                    default: throw new NotImplementedException(Tipo.ToString());
                }
            }
        }

        public string FileName
        {
            get
            {
                string inicio = "";

                switch (Tipo)
                {
                    case TipoFoto.Medicao:
                        inicio = "Med_";
                        break;
                    case TipoFoto.Compra:
                        inicio = "Comp_";
                        break;
                    case TipoFoto.Cliente:
                        inicio = "Cli_";
                        break;
                    case TipoFoto.Pedido:
                        inicio = "Ped_";
                        break;
                    case TipoFoto.Liberacao:
                        inicio = "Lib_";
                        break;
                    case TipoFoto.Orcamento:
                        inicio = "Orc_";
                        break;
                    case TipoFoto.DevolucaoPagto:
                        inicio = "DevPagto_";
                        break;
                    case TipoFoto.ImpostoServ:
                        inicio = "ImpostoServ_";
                        break;
                    case TipoFoto.TrocaDevolucao:
                        inicio = "TrocaDevolucao_";
                        break;
                    case TipoFoto.ConciliacaoBancaria:
                        inicio = "ConciliacaoBancaria_";
                        break;
                    case TipoFoto.Pagto:
                        inicio = "Pagto_";
                        break;
                    case TipoFoto.Cheque:
                        inicio = "Cheque_";
                        break;
                    case TipoFoto.Acerto:
                        inicio = "Acerto_";
                        break;
                    case TipoFoto.PagtoAntecipado:
                        inicio = "PagtoAntecip_";
                        break;
                    case TipoFoto.Obra:
                        inicio = "Obra_";
                        break;
                    case TipoFoto.Sugestao:
                        inicio = "Sugestao_";
                        break;
                    case TipoFoto.PedidoInterno:
                        inicio = "PedidoInterno_";
                        break;
                    case TipoFoto.Fornecedor:
                        inicio = "Fornec_";
                        break;
                    default:
                        throw new NotImplementedException(Tipo.ToString());
                }

                return inicio + IdParent + "_Foto_" + IdFoto + Extensao;
            }
        }

        public string FilePath
        {
            get { return Path + "\\" + FileName; }
        }

        public string VirtualFilePath
        {
            get { return VirtualPath + "/" + FileName; }
        }

        #endregion

        #region Método de retorno de itens

        public static IList<IFoto> GetByParent(uint idParent, TipoFoto tipo)
        {            
            switch (tipo)
            {
                case TipoFoto.Compra: return FotosCompraDAO.Instance.GetByCompra(idParent);
                case TipoFoto.Cliente: return FotosClienteDAO.Instance.GetByCliente(idParent);
                case TipoFoto.Pedido: return FotosPedidoDAO.Instance.GetByPedido(idParent);
                case TipoFoto.Liberacao: return FotosLiberacaoDAO.Instance.GetByLiberacao(idParent);
                case TipoFoto.Orcamento: return FotosOrcamentoDAO.Instance.GetByOrcamento(idParent);
                case TipoFoto.DevolucaoPagto: return FotosDevolucaoPagtoDAO.Instance.GetByDevolucaoPagto(idParent);
                case TipoFoto.ImpostoServ: return FotosImpostoServDAO.Instance.GetByImpostoServ(idParent);
                case TipoFoto.TrocaDevolucao: return FotosTrocaDevolucaoDAO.Instance.GetByTrocaDevolucao(idParent);
                case TipoFoto.ConciliacaoBancaria: return FotosConciliacaoBancariaDAO.Instance.GetByConciliacaoBancaria(idParent);
                case TipoFoto.Pagto: return FotosPagtoDAO.Instance.GetByPagto((int)idParent);
                case TipoFoto.Cheque: return FotosChequesDAO.Instance.ObterPeloCheque((int)idParent);
                case TipoFoto.Acerto: return FotosAcertoDAO.Instance.ObterPeloAcerto((int)idParent);
                case TipoFoto.PagtoAntecipado: return FotosPagtoAntecipadoDAO.Instance.ObterPeloPagtoAntecipado((int)idParent);
                case TipoFoto.Obra: return FotosObraDAO.Instance.ObterPelaObra((int)idParent);
                case TipoFoto.Sugestao: return FotosSugestaoDAO.Instance.ObterPelaSugestao((int)idParent);
                case TipoFoto.PedidoInterno: return FotosPedidoInternoDAO.Instance.ObterPeloPedidoInterno((int)idParent);
                case TipoFoto.Fornecedor: return FotosFornecedorDAO.Instance.GetByFornecedor(idParent);
                default: throw new NotImplementedException(tipo.ToString());
            }
        }

        public static IList<IFoto> GetByParent(string idsParent, TipoFoto tipo)
        {
            switch (tipo)
            {
                case TipoFoto.Medicao: return FotosMedicaoDAO.Instance.GetByMedicao(idsParent);

                default: throw new NotImplementedException(tipo.ToString());
            }
        }

        #endregion

        #region Métodos de alteração do banco

        public uint Insert()
        {
            switch (Tipo)
            {
                case TipoFoto.Medicao: return FotosMedicaoDAO.Instance.Insert((FotosMedicao)this);
                case TipoFoto.Compra: return FotosCompraDAO.Instance.Insert((FotosCompra)this);
                case TipoFoto.Cliente: return FotosClienteDAO.Instance.Insert((FotosCliente)this);
                case TipoFoto.Pedido: return FotosPedidoDAO.Instance.Insert((FotosPedido)this);
                case TipoFoto.Liberacao: return FotosLiberacaoDAO.Instance.Insert((FotosLiberacao)this);
                case TipoFoto.Orcamento: return FotosOrcamentoDAO.Instance.Insert((FotosOrcamento)this);
                case TipoFoto.DevolucaoPagto: return FotosDevolucaoPagtoDAO.Instance.Insert((FotosDevolucaoPagto)this);
                case TipoFoto.ImpostoServ: return FotosImpostoServDAO.Instance.Insert((FotosImpostoServ)this);
                case TipoFoto.TrocaDevolucao: return FotosTrocaDevolucaoDAO.Instance.Insert((FotosTrocaDevolucao)this);
                case TipoFoto.ConciliacaoBancaria: return FotosConciliacaoBancariaDAO.Instance.Insert((FotosConciliacaoBancaria)this);
                case TipoFoto.Pagto: return FotosPagtoDAO.Instance.Insert((FotosPagto)this);
                case TipoFoto.Cheque: return FotosChequesDAO.Instance.Insert((FotosCheques)this);
                case TipoFoto.Acerto: return FotosAcertoDAO.Instance.Insert((FotosAcerto)this);
                case TipoFoto.PagtoAntecipado: return FotosPagtoAntecipadoDAO.Instance.Insert((FotosPagtoAntecipado)this);
                case TipoFoto.Obra: return FotosObraDAO.Instance.Insert((FotosObra)this);
                case TipoFoto.Sugestao: return FotosSugestaoDAO.Instance.Insert((FotosSugestao)this);
                case TipoFoto.PedidoInterno: return FotosPedidoInternoDAO.Instance.Insert((FotosPedidoInterno)this);
                case TipoFoto.Fornecedor: return FotosFornecedorDAO.Instance.Insert((FotosFornecedor)this);
                default: throw new NotImplementedException(Tipo.ToString());
            }
        }

        public void Delete()
        {
            switch (Tipo)
            {
                case TipoFoto.Medicao:
                    FotosMedicaoDAO.Instance.DeleteByPrimaryKey(IdFoto);
                    break;
                case TipoFoto.Compra:
                    FotosCompraDAO.Instance.DeleteByPrimaryKey(IdFoto);
                    break;
                case TipoFoto.Cliente:
                    FotosClienteDAO.Instance.DeleteByPrimaryKey(IdFoto);
                    break;
                case TipoFoto.Pedido:
                    FotosPedidoDAO.Instance.DeleteByPrimaryKey(IdFoto);
                    break;
                case TipoFoto.Liberacao:
                    FotosLiberacaoDAO.Instance.DeleteByPrimaryKey(IdFoto);
                    break;
                case TipoFoto.Orcamento:
                    FotosOrcamentoDAO.Instance.DeleteByPrimaryKey(IdFoto);
                    break;
                case TipoFoto.DevolucaoPagto:
                    FotosDevolucaoPagtoDAO.Instance.DeleteByPrimaryKey(IdFoto);
                    break;
                case TipoFoto.ImpostoServ:
                    FotosImpostoServDAO.Instance.DeleteInstanceByPrimaryKey(IdFoto);
                    break;
                case TipoFoto.TrocaDevolucao:
                    FotosTrocaDevolucaoDAO.Instance.DeleteByPrimaryKey(IdFoto);
                    break;
                case TipoFoto.ConciliacaoBancaria:
                    FotosConciliacaoBancariaDAO.Instance.DeleteByPrimaryKey(IdFoto);
                    break;
                case TipoFoto.Pagto:
                    FotosPagtoDAO.Instance.DeleteByPrimaryKey(IdFoto);
                    break;
                case TipoFoto.Cheque:
                    FotosChequesDAO.Instance.DeleteByPrimaryKey(IdFoto);
                    break;
                case TipoFoto.Acerto:
                    FotosAcertoDAO.Instance.DeleteByPrimaryKey(IdFoto);
                    break;
                case TipoFoto.PagtoAntecipado:
                    FotosPagtoAntecipadoDAO.Instance.DeleteByPrimaryKey(IdFoto);
                    break;
                case TipoFoto.Obra:
                    FotosObraDAO.Instance.DeleteByPrimaryKey(IdFoto);
                    break;
                case TipoFoto.Sugestao:
                    FotosSugestaoDAO.Instance.DeleteByPrimaryKey(IdFoto);
                    break;
                case TipoFoto.PedidoInterno:
                    FotosPedidoInternoDAO.Instance.DeleteByPrimaryKey(IdFoto);
                    break;
                case TipoFoto.Fornecedor:
                    FotosFornecedorDAO.Instance.DeleteByPrimaryKey(IdFoto);
                    break;
                default:
                    throw new NotImplementedException(Tipo.ToString());
            }
        }

        public void Update()
        {
            switch (Tipo)
            {
                case TipoFoto.Medicao:
                    FotosMedicaoDAO.Instance.Update((FotosMedicao)this);
                    break;
                case TipoFoto.Compra:
                    FotosCompraDAO.Instance.Update((FotosCompra)this);
                    break;
                case TipoFoto.Cliente:
                    FotosClienteDAO.Instance.Update((FotosCliente)this);
                    break;
                case TipoFoto.Pedido:
                    FotosPedidoDAO.Instance.Update((FotosPedido)this);
                    break;
                case TipoFoto.Liberacao:
                    FotosLiberacaoDAO.Instance.Update((FotosLiberacao)this);
                    break;
                case TipoFoto.Orcamento:
                    FotosOrcamentoDAO.Instance.Update((FotosOrcamento)this);
                    break;
                case TipoFoto.DevolucaoPagto:
                    FotosDevolucaoPagtoDAO.Instance.Update((FotosDevolucaoPagto)this);
                    break;
                case TipoFoto.ImpostoServ:
                    FotosImpostoServDAO.Instance.Update((FotosImpostoServ)this);
                    break;
                case TipoFoto.TrocaDevolucao:
                    FotosTrocaDevolucaoDAO.Instance.Update((FotosTrocaDevolucao)this);
                    break;
                case TipoFoto.ConciliacaoBancaria:
                    FotosConciliacaoBancariaDAO.Instance.Update((FotosConciliacaoBancaria)this);
                    break;
                case TipoFoto.Pagto:
                    FotosPagtoDAO.Instance.Update((FotosPagto)this);
                    break;
                case TipoFoto.Cheque:
                    FotosChequesDAO.Instance.Update((FotosCheques)this);
                    break;
                case TipoFoto.Acerto:
                    FotosAcertoDAO.Instance.Update((FotosAcerto)this);
                    break;
                case TipoFoto.PagtoAntecipado:
                    FotosPagtoAntecipadoDAO.Instance.Update((FotosPagtoAntecipado)this);
                    break;
                case TipoFoto.Obra:
                    FotosObraDAO.Instance.Update((FotosObra)this);
                    break;
                case TipoFoto.Sugestao:
                    FotosSugestaoDAO.Instance.Update((FotosSugestao)this);
                    break;
                case TipoFoto.PedidoInterno:
                    FotosPedidoInternoDAO.Instance.Update((FotosPedidoInterno)this);
                    break;
                case TipoFoto.Fornecedor:
                    FotosFornecedorDAO.Instance.Update((FotosFornecedor)this);
                    break;
                default:
                    throw new NotImplementedException(Tipo.ToString());
            }
        }

        #endregion
    }
}
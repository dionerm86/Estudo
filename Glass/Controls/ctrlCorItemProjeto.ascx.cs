using System;
using System.Collections.Generic;
using System.Web.UI;
using Glass.Data.DAL;
using Glass.Data.Model;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Controls
{
    public partial class ctrlCorItemProjeto : BaseUserControl
    {
        #region Propriedades

        public uint? IdItemProjeto
        {
            get { return Glass.Conversoes.StrParaUintNullable(hdfCorIdItemProjeto.Value); }
            set { hdfCorIdItemProjeto.Value = Glass.Conversoes.UintParaStr(value); }
        }

        public uint? IdProjeto
        {
            get { return Glass.Conversoes.StrParaUintNullable(hdfCorIdProjeto.Value); }
            set { hdfCorIdProjeto.Value = Glass.Conversoes.UintParaStr(value); }
        }

        public uint? IdOrcamento
        {
            get { return Glass.Conversoes.StrParaUintNullable(hdfCorIdOrcamento.Value); }
            set { hdfCorIdOrcamento.Value = Glass.Conversoes.UintParaStr(value); }
        }

        public uint? IdPedido
        {
            get { return Glass.Conversoes.StrParaUintNullable(hdfCorIdPedido.Value); }
            set { hdfCorIdPedido.Value = Glass.Conversoes.UintParaStr(value); }
        }

        public uint? IdPedidoEspelho
        {
            get { return Glass.Conversoes.StrParaUintNullable(hdfCorIdPedidoEspelho.Value); }
            set { hdfCorIdPedidoEspelho.Value = Glass.Conversoes.UintParaStr(value); }
        }

        public bool ExibirTooltip
        {
            get { return hdfExibirTooltip.Value.ToLower() == "true"; }
            set { hdfExibirTooltip.Value = value.ToString().ToLower(); }
        }

        public string Titulo
        {
            get { return hdfTitulo.Value; }
            set { hdfTitulo.Value = value; }
        }

        #endregion

        #region Eventos

        public delegate void AlterarCor(object sender, EventArgs e);
        private AlterarCor _corAlterada;

        public event AlterarCor CorAlterada
        {
            add { _corAlterada += value; }
            remove { _corAlterada -= value; }
        }

        #endregion

        #region Métodos de suporte

        /// <summary>
        /// Recupera o tipo de entrega e o cliente de um projeto/orçamento/pedido/pedido espelho.
        /// </summary>
        /// <param name="idProjeto"></param>
        /// <param name="idOrcamento"></param>
        /// <param name="idPedido"></param>
        /// <param name="idPedidoEspelho"></param>
        /// <param name="tipoEntrega"></param>
        /// <param name="idCliente"></param>
        private static void GetTipoEntregaCliente(uint? idProjeto, uint? idOrcamento, uint? idPedido, uint? idPedidoEspelho, out int? tipoEntrega, out uint? idCliente)
        {
            if (idProjeto > 0)
            {
                tipoEntrega = ProjetoDAO.Instance.ObtemTipoEntrega(idProjeto.Value);
                idCliente = ProjetoDAO.Instance.ObtemIdCliente(idProjeto.Value);
            }
            else if (idOrcamento > 0)
            {
                tipoEntrega = OrcamentoDAO.Instance.ObtemTipoEntrega(idOrcamento.Value);
                idCliente = OrcamentoDAO.Instance.ObtemIdCliente(idOrcamento.Value);
            }
            else if (idPedido > 0 || idPedidoEspelho > 0)
            {
                if (idPedido == null || idPedido == 0)
                    idPedido = idPedidoEspelho;

                tipoEntrega = PedidoDAO.Instance.ObtemTipoEntrega(idPedido.Value);
                idCliente = PedidoDAO.Instance.ObtemIdCliente(null, idPedido.Value);
            }
            else
            {
                tipoEntrega = null;
                idCliente = null;
            }
        }

        /// <summary>
        /// Recupera os IDs dos itens de projeto que serão atualizados de um projeto/orçamento/pedido/pedido espelho.
        /// Retorna também o tipo de entrega e o cliente do item pesquisado.
        /// </summary>
        /// <param name="idItemProjeto"></param>
        /// <param name="idProjeto"></param>
        /// <param name="idOrcamento"></param>
        /// <param name="idPedido"></param>
        /// <param name="idPedidoEspelho"></param>
        /// <param name="tipoEntrega"></param>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        private static uint[] GetIdsItensProjeto(uint? idItemProjeto, uint? idProjeto, uint? idOrcamento, uint? idPedido, uint? idPedidoEspelho, out int? tipoEntrega, out uint? idCliente)
        {
            if (idItemProjeto != null && ItemProjetoDAO.Instance.Exists(idItemProjeto.Value))
            {
                ItemProjeto item = ItemProjetoDAO.Instance.GetElementByPrimaryKey(idItemProjeto.Value);
                GetTipoEntregaCliente(item.IdProjeto, item.IdOrcamento, item.IdPedido, item.IdPedidoEspelho, out tipoEntrega, out idCliente);

                return new uint[] { idItemProjeto.Value };
            }
            else if (idProjeto != null)
            {
                GetTipoEntregaCliente(idProjeto, null, null, null, out tipoEntrega, out idCliente);

                var itens = ItemProjetoDAO.Instance.GetByProjeto(idProjeto.Value);
                List<uint> ids = new List<uint>();

                foreach (ItemProjeto i in itens)
                    ids.Add(i.IdItemProjeto);

                return ids.ToArray();
            }
            else if (idOrcamento != null)
            {
                GetTipoEntregaCliente(null, idOrcamento, null, null, out tipoEntrega, out idCliente);

                var itens = ItemProjetoDAO.Instance.GetByOrcamento(idOrcamento.Value);
                List<uint> ids = new List<uint>();

                foreach (ItemProjeto i in itens)
                    ids.Add(i.IdItemProjeto);

                return ids.ToArray();
            }
            else if (idPedido != null)
            {
                GetTipoEntregaCliente(null, null, idPedido, null, out tipoEntrega, out idCliente);

                var itens = ItemProjetoDAO.Instance.GetByPedido(idPedido.Value);
                List<uint> ids = new List<uint>();

                foreach (ItemProjeto i in itens)
                    ids.Add(i.IdItemProjeto);

                return ids.ToArray();
            }
            else if (idPedidoEspelho != null)
            {
                GetTipoEntregaCliente(null, null, null, idPedidoEspelho, out tipoEntrega, out idCliente);

                var itens = ItemProjetoDAO.Instance.GetByPedido(idPedidoEspelho.Value);
                List<uint> ids = new List<uint>();

                foreach (ItemProjeto i in itens)
                    ids.Add(i.IdItemProjeto);

                return ids.ToArray();
            }

            tipoEntrega = null;
            idCliente = null;
            return new uint[0];
        }

        /// <summary>
        /// Atualiza os ambientes de um orçamento/pedido/pedido espelho.
        /// </summary>
        private static void AtualizarOrcaPedido(uint[] idsItensProjetos, uint? idItemProjeto, uint? idOrcamento, uint? idPedido, uint? idPedidoEspelho)
        {
            if (idItemProjeto != null && ItemProjetoDAO.Instance.Exists(idItemProjeto.Value))
            {
                // Deve ser getelement para buscar o texto do orçamento e não apagar o texto no produto/ambiente
                var item = ItemProjetoDAO.Instance.GetElement(idItemProjeto.Value);
                AtualizarOrcaPedido(idsItensProjetos, null, item.IdOrcamento, item.IdPedido, item.IdPedidoEspelho);
            }
            else if (idOrcamento != null)
            {
                foreach (uint id in idsItensProjetos)
                {
                    // Deve ser getelement para buscar o texto do orçamento e não apagar o texto no produto/ambiente
                    var itemProj = ItemProjetoDAO.Instance.GetElement(id);
                    uint? idAmbienteOrca = AmbienteOrcamentoDAO.Instance.GetIdByItemProjeto(id);
                    ProdutosOrcamentoDAO.Instance.InsereAtualizaProdProj(idOrcamento.Value, idAmbienteOrca, itemProj);
                }
            }
            else if (idPedido != null)
            {
                var pedido = PedidoDAO.Instance.GetElementByPrimaryKey(idPedido.Value);

                foreach (uint id in idsItensProjetos)
                {
                    // Deve ser getelement para buscar o texto do orçamento e não apagar o texto no produto/ambiente
                    var itemProj = ItemProjetoDAO.Instance.GetElement(id);
                    uint? idAmbientePedido = AmbientePedidoDAO.Instance.GetIdByItemProjeto(id);
                    ProdutosPedidoDAO.Instance.InsereAtualizaProdProj(null, pedido, idAmbientePedido, itemProj, false);
                }
            }
            else if (idPedidoEspelho != null)
            {
                foreach (uint id in idsItensProjetos)
                {
                    // Deve ser getelement para buscar o texto do orçamento e não apagar o texto no produto/ambiente
                    var itemProj = ItemProjetoDAO.Instance.GetElement(id);
                    uint? idAmbientePedidoEspelho = AmbientePedidoEspelhoDAO.Instance.GetIdByItemProjeto(id);
                    ProdutosPedidoEspelhoDAO.Instance.InsereAtualizaProdProj(null, idPedidoEspelho.Value, idAmbientePedidoEspelho, itemProj, false);
                }
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.ClientScript.IsClientScriptIncludeRegistered(GetType(), "ctrlCorItemProjeto"))
                Page.ClientScript.RegisterClientScriptInclude(GetType(), "ctrlCorItemProjeto", this.ResolveClientUrl("~/Scripts/ctrlCorItemProjeto.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)));

            imgExibir.Visible = ExibirTooltip;
            CorItemProjeto.Style.Value = ExibirTooltip ? "display: none" : "";
            tituloCorItemProjeto.Visible = !ExibirTooltip;
            btnAlterar.OnClientClick = "alterar('" + this.ClientID + "'); return false;";

            Page.PreRender += new EventHandler(Page_PreRender);
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            lblTitulo.Text = Titulo;
            drpCorVidro.Items[0].Enabled = IdItemProjeto == null;
            drpCorAluminio.Items[0].Enabled = IdItemProjeto == null;
            drpCorFerragem.Items[0].Enabled = IdItemProjeto == null;

            drpCorVidro.Items.Clear();
            drpCorVidro.DataSource = null;

            if (IdItemProjeto != null && ItemProjetoDAO.Instance.Exists(IdItemProjeto.Value))
            {
                var idProjetoModelo = ItemProjetoDAO.Instance.ObtemIdProjetoModelo(null, IdItemProjeto.Value);
                drpCorVidro.DataSource = CorVidroDAO.Instance.GetForProjeto(idProjetoModelo);
                drpCorVidro.Items.Add(new ListItem("", "0"));
                drpCorVidro.DataBind();

                ItemProjeto item = ItemProjetoDAO.Instance.GetElement(IdItemProjeto.Value);
                drpCorVidro.SelectedValue = item.IdCorVidro.ToString();
                drpCorAluminio.SelectedValue = item.IdCorAluminio > 0 ? item.IdCorAluminio.ToString() : string.Empty;
                drpCorFerragem.SelectedValue = item.IdCorFerragem > 0 ? item.IdCorFerragem.ToString() : string.Empty;
            }
            else
            {
                drpCorVidro.DataSource = CorVidroDAO.Instance.GetForProjeto(null);
                drpCorVidro.Items.Insert(0, "Não Alterar");
                drpCorVidro.DataBind();
            }
        }

        protected void imgExibir_Load(object sender, EventArgs e)
        {
            imgExibir.OnClientClick = "exibirCorItemProjeto(this, '" + CorItemProjeto.ClientID + "'); return false;";
        }

        protected void btnAplicar_Click(object sender, EventArgs e)
        {
            try
            {
                uint? idCorVidro = hdfCorVidro.Value != "" ? (uint?)Glass.Conversoes.StrParaUint(hdfCorVidro.Value) : null;
                uint? idCorAluminio = hdfCorAluminio.Value != "" ? (uint?)Glass.Conversoes.StrParaUint(hdfCorAluminio.Value) : null;
                uint? idCorFerragem = hdfCorFerragem.Value != "" ? (uint?)Glass.Conversoes.StrParaUint(hdfCorFerragem.Value) : null;

                int? tipoEntrega = null;
                uint? idCliente = null;

                var idsItensProjetos = GetIdsItensProjeto(IdItemProjeto, IdProjeto, IdOrcamento, IdPedido, IdPedidoEspelho, out tipoEntrega, out idCliente);

                OrcamentoDAO.Instance.AlteraCorItens(IdItemProjeto, IdOrcamento, IdPedido, IdPedidoEspelho, idCorVidro, idCorAluminio, idCorFerragem, tipoEntrega,
                    idCliente, idsItensProjetos);

                if (_corAlterada != null && _corAlterada.GetInvocationList().Length > 0)
                    _corAlterada.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MensagemAlerta.ErrorMsg("", ex, Page);
            }
        }
    }
}

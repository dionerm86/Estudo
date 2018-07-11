using System;
using System.Web.UI.WebControls;
using Glass.Data.Model;
using Glass.Data.Helper;
using Glass.Data.DAL;
using System.IO;
using System.Linq;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadFotos : System.Web.UI.Page
    {
        private IFoto.TipoFoto GetTipo()
        {
            return (IFoto.TipoFoto)Enum.Parse(typeof (IFoto.TipoFoto), Request["tipo"], true);
        }

        protected string GetTitle()
        {
            switch (GetTipo())
            {
                case IFoto.TipoFoto.Compra:
                    return "da compra";
                case IFoto.TipoFoto.Medicao:
                    return "da medição";
                case IFoto.TipoFoto.Cliente:
                    return "do cliente";
                case IFoto.TipoFoto.Pedido:
                    return "do pedido";
                case IFoto.TipoFoto.Liberacao:
                    return "da liberação";
                case IFoto.TipoFoto.Orcamento:
                    return "do orçamento";
                case IFoto.TipoFoto.DevolucaoPagto:
                    return "da devolução do pagamento";
                case IFoto.TipoFoto.ImpostoServ:
                    return "do lançamento de imposto/serviços avulsos";
                case IFoto.TipoFoto.TrocaDevolucao:
                    return "da troca/devolução";
                case IFoto.TipoFoto.ConciliacaoBancaria:
                    return "da conciliação bancária";
                case IFoto.TipoFoto.Pagto:
                    return "do pagamento";
                case IFoto.TipoFoto.Cheque:
                    return "do cheque";
                case IFoto.TipoFoto.Acerto:
                    return "do acerto";
                case IFoto.TipoFoto.PagtoAntecipado:
                    return "do pagamento antecipado";
                case IFoto.TipoFoto.Sugestao:
                    return "da sugestão";
                case IFoto.TipoFoto.PedidoInterno:
                    return "do pedido interno";
                case IFoto.TipoFoto.Fornecedor:
                    return "do fornecedor";
                default:
                    throw new NotImplementedException();
            }
        }

        private const string INICIO_SUBTITULO = "Arquivos cadastrados para ";

        protected string GetSubtitle()
        {
            string subtitulo = "";

            if (Glass.Conversoes.StrParaUint(Request["id"]) > 0)
            {
                subtitulo = INICIO_SUBTITULO;

                switch (GetTipo())
                {
                    case IFoto.TipoFoto.Compra:
                        subtitulo += "esta compra";
                        break;
                    case IFoto.TipoFoto.Medicao:
                        subtitulo += "esta medição";
                        break;
                    case IFoto.TipoFoto.Cliente:
                        subtitulo += "este cliente";
                        break;
                    case IFoto.TipoFoto.Pedido:
                        subtitulo += "este pedido";
                        break;
                    case IFoto.TipoFoto.Liberacao:
                        subtitulo += "esta liberação";
                        break;
                    case IFoto.TipoFoto.Orcamento:
                        subtitulo += "este orçamento";
                        break;
                    case IFoto.TipoFoto.DevolucaoPagto:
                        subtitulo += "esta devolução do pagamento";
                        break;
                    case IFoto.TipoFoto.ImpostoServ:
                        subtitulo += "este lançamento de imposto/serviços avulsos";
                        break;
                    case IFoto.TipoFoto.TrocaDevolucao:
                        subtitulo += "esta troca/devolução";
                        break;
                    case IFoto.TipoFoto.ConciliacaoBancaria:
                        subtitulo += "esta conciliação bancária";
                        break;
                    case IFoto.TipoFoto.Pagto:
                        subtitulo += "este pagamento";
                        break;
                    case IFoto.TipoFoto.Cheque:
                        subtitulo += "este cheque";
                        break;
                    case IFoto.TipoFoto.Acerto:
                        subtitulo += "este acerto";
                        break;
                    case IFoto.TipoFoto.PagtoAntecipado:
                        subtitulo += "este pagamento antecipado";
                        break;
                    case IFoto.TipoFoto.Obra:
                        subtitulo += Request["gerarCredito"] == "1" ? "este crédito" : "este pagamento antecipado de obra";
                        break;
                    case IFoto.TipoFoto.Sugestao:
                        subtitulo += "esta sugestão";
                        break;
                    case IFoto.TipoFoto.PedidoInterno:
                        subtitulo += "este pedido interno";
                        break;
                    case IFoto.TipoFoto.Fornecedor:
                        subtitulo += "este fornecedor";
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            else
                subtitulo = "O nome do arquivo deve ser exatamente o número " +
                            (Request["tipo"] == "liberacao" ? "da liberação." : "do pedido.") + @"
                Ex.: 9999.jpg. <br /> Para inserir várias imagens adicione ao nome do arquivo hífen e o indice da imagem. Ex.: 9999-1.jpg.";

            return subtitulo;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            LoginUsuario login = UserInfo.GetUserInfo;
            bool visivelMedicao = Config.PossuiPermissao(Config.FuncaoMenuMedicao.EfetuarMedicao) &&
                                  login.CodUser > 0;

            bool visivelCompra = Config.PossuiPermissao(Config.FuncaoMenuFinanceiroPagto.ControleFinanceiroPagamento) &&
                                 login.CodUser > 0;

            bool visivelCliente = Config.PossuiPermissao(Config.FuncaoMenuCadastro.AnexarArquivosCliente) && login.CodUser > 0;

            bool visivelPedido = (Config.PossuiPermissao(Config.FuncaoMenuPedido.AnexarArquivoPedido) && login.CodUser > 0) || login.IdCliente > 0;

            bool visivelLiberacao = Config.PossuiPermissao(Config.FuncaoMenuPedido.AnexarArquivoLiberacaoListaPedido) &&
                                    login.CodUser > 0;

            bool visivelOrcamento = Config.PossuiPermissao(Config.FuncaoMenuPedido.AnexarArquivoPedido) &&
                                    login.CodUser > 0;

            bool visivelFinanceiroPagto = Config.PossuiPermissao(Config.FuncaoMenuFinanceiroPagto.ControleFinanceiroPagamento);

            bool visivelFinanceiro = Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento);

            bool visivelFornecedor = Config.PossuiPermissao(Config.FuncaoMenuCadastro.AnexarArquivosFornecedor) && login.CodUser > 0;

            bool visivelPedidoInterno = Config.PossuiPermissao(Config.FuncaoMenuEstoque.AnexarArquivoPedidoInterno);

            bool visivelCheque = Request["tipo"] == "cheque";

            IFoto.TipoFoto tipo = GetTipo();

            // Exibe/Esconde opção de cadastrar/editar/excluir fotos
            if (Request["crud"] == "0")
                trTitle1.Visible = false;
            else
                trTitle1.Visible =
                    tipo == IFoto.TipoFoto.Medicao ? visivelMedicao :
                    tipo == IFoto.TipoFoto.Compra ? visivelCompra :
                    tipo == IFoto.TipoFoto.Cliente ? visivelCliente :
                    tipo == IFoto.TipoFoto.PedidoInterno ? visivelPedidoInterno :
                    /* Chamado 44690. */
                    tipo == IFoto.TipoFoto.Sugestao ? visivelCliente :
                    tipo == IFoto.TipoFoto.Cheque ? visivelCheque :
                    tipo == IFoto.TipoFoto.Pedido ? visivelPedido :
                    tipo == IFoto.TipoFoto.ImpostoServ ? visivelFinanceiroPagto :
                    tipo == IFoto.TipoFoto.ConciliacaoBancaria ? visivelFinanceiroPagto :
                    tipo == IFoto.TipoFoto.DevolucaoPagto ? visivelFinanceiro :
                    tipo == IFoto.TipoFoto.Orcamento ? visivelOrcamento :
                    tipo == IFoto.TipoFoto.Obra ? visivelFinanceiro :
                    tipo == IFoto.TipoFoto.Fornecedor ? visivelFornecedor:visivelLiberacao;            

            trTitle2.Visible = trTitle1.Visible;
            trCadastro.Visible = trTitle1.Visible;

            System.Collections.Generic.IList<IFoto> lstFotos;

            if (tipo == IFoto.TipoFoto.Medicao)
                lstFotos = IFoto.GetByParent(Request["id"], tipo);
            else
            lstFotos = IFoto.GetByParent(Glass.Conversoes.StrParaUint(Request["id"]), tipo);

            //Unit larguraFoto = new Unit("160px");
            //Unit alturaFoto = new Unit("");
            var larguraTabela = new Unit("160px");

            var linha = new TableRow();

            if (lstFotos.Count > 0)
            {
                int contador = 0;
                foreach (IFoto f in lstFotos)
                {
                    // Permite inserir no máximo 4 fotos por linha
                    if (contador == 4)
                    {
                        tbFotos.Controls.Add(linha);
                        linha = new TableRow();
                        contador = 0;
                    }

                    contador++;

                    var celula = new TableCell();

                    var foto = (CtrlFoto) LoadControl(ResolveClientUrl("~/Controls/ctrlFoto.ascx"));
                    foto.Foto = f;
                    foto.LarguraTabela = larguraTabela;
                    foto.EditVisible = trTitle1.Visible;

                    celula.Controls.Add(foto);
                    celula.Style.Value = "padding: 4px";

                    linha.Controls.Add(celula);
                }
            }
            else
            {
                var celula = new TableCell();
                celula.Text = Request["id"] != "0"
                    ? "Não há fotos cadastradas para " + GetSubtitle().Substring(INICIO_SUBTITULO.Length) + "."
                    : "";
                linha.Controls.Add(celula);
            }

            tbFotos.Controls.Add(linha);

            tabelaCrud.Visible = Request["id"] != "0";
            tblMultFlu.Visible = Request["id"] == "0";
        }

        protected void btnInserir_Click(object sender, EventArgs e)
        {
            // Verifica se algum arquivo foi selecionado
            if (!fluFoto1.HasFile && !fluFoto2.HasFile && !fluFoto3.HasFile && !fluFoto4.HasFile && !fluFoto5.HasFile &&
                !fluFoto6.HasFile && !fluFoto7.HasFile && !fluFoto8.HasFile && !fluFoto9.HasFile && !fluFoto10.HasFile)
            {
                Glass.MensagemAlerta.ShowMsg("Nenhum arquivo selecionado.", Page);
                return;
            }

            IFoto.TipoFoto tipo = GetTipo();

            if (!Inserir(fluFoto1, txtDescricao1.Text, tipo)) return;
            if (!Inserir(fluFoto2, txtDescricao2.Text, tipo)) return;
            if (!Inserir(fluFoto3, txtDescricao3.Text, tipo)) return;
            if (!Inserir(fluFoto4, txtDescricao4.Text, tipo)) return;
            if (!Inserir(fluFoto5, txtDescricao5.Text, tipo)) return;
            if (!Inserir(fluFoto6, txtDescricao6.Text, tipo)) return;
            if (!Inserir(fluFoto7, txtDescricao7.Text, tipo)) return;
            if (!Inserir(fluFoto8, txtDescricao8.Text, tipo)) return;
            if (!Inserir(fluFoto9, txtDescricao9.Text, tipo)) return;
            if (!Inserir(fluFoto10, txtDescricao10.Text, tipo)) return;

            ClientScript.RegisterClientScriptBlock(typeof (string), "reload",
                "redirectUrl('" + Request.Url.ToString() + "');", true);
        }

        private bool Inserir(FileUpload fluFoto, string descricao, IFoto.TipoFoto tipo)
        {
            if (!fluFoto.HasFile)
                return true;

            try
            {
                var idParent = tipo == IFoto.TipoFoto.Medicao ? Glass.Conversoes.StrParaUint(Request["id"].Split(',').Max(f => f)) : Glass.Conversoes.StrParaUint(Request["id"]);
                Anexo.InserirAnexo(tipo, idParent, fluFoto.FileBytes, fluFoto.FileName, descricao);
            }
            catch (Exception ex)
            {
                MensagemAlerta.ErrorMsg("Falha ao inserir arquivo.", ex, Page);
                ClientScript.RegisterStartupScript(typeof(string), "reload", "redirectUrl('" + Request.Url.ToString() + "');", true);
                return false;
            }

            return true;
        }

        protected void btnInserirMult_Click(object sender, EventArgs e)
        {
            // Verifica se algum arquivo foi selecionado.
            if (!fluFoto.HasFile)
            {
                Glass.MensagemAlerta.ShowMsg("Nenhum arquivo selecionado.", Page);
                return;
            }

            IFoto.TipoFoto tipo = GetTipo();

            string retorno = InserirMult(txtDescricao.Text, tipo);

            ClientScript.RegisterClientScriptBlock(typeof (string), "alerta",
                "alert('" + retorno + "'); redirectUrl('" + Request.Url.ToString() + "');", true);
        }

        private string InserirMult(string descricao, IFoto.TipoFoto tipo)
        {
            var hfc = Request.Files;
            var arquivosNaoAnexados = "";

            for (var i = 0; i < hfc.Count; i++)
            {
                // Cadastra a foto.
                var foto = IFoto.Nova(tipo);

                // Recupera os dados do arquivo.
                var arquivo = hfc[i];

                // Recupera o nome do arquivo, que deve ser exatamente o id da referência.
                var idReferencia = Conversoes.StrParaUint(arquivo.FileName.Split('-', '.', ' ')[0]);

                // Verifica se são arquivos de pedidos ou de liberações.
                switch (Request["tipo"])
                {
                    case "pedido":

                        if (PedidoEspelhoDAO.Instance.IsPedidoImpresso(null, foto.IdParent))
                            return ("Não é possível inserir imagem em pedidos que já possuam etiqueta(s) impressa(s).");

                        // Se o tipo for pedido e o nome do arquivo não for o id de um pedido válido então o arquivo não é anexado.
                        else if (!PedidoDAO.Instance.Exists(idReferencia))
                        {
                            arquivosNaoAnexados += arquivo.FileName + ", ";
                            continue;
                        }

                        break;
                    case "liberacao":
                        // Se o tipo for pedido e o nome do arquivo não for o id de uma liberação válida então o arquivo não é anexado.
                        if (!LiberarPedidoDAO.Instance.Exists(idReferencia))
                        {
                            arquivosNaoAnexados += arquivo.FileName + ", ";
                            continue;
                        }

                        break;
                    default:
                        continue;
                }

                foto.IdParent = idReferencia;
                foto.Extensao = arquivo.FileName.Substring(arquivo.FileName.LastIndexOf('.'));

                if (foto.ApenasImagens && !Arquivos.IsImagem(foto.Extensao))
                {
                    arquivosNaoAnexados += arquivo.FileName + ", ";
                    continue;
                }

                foto.Descricao = descricao;
                foto.IdFoto = foto.Insert();

                if (foto.IdFoto == 0)
                {
                    arquivosNaoAnexados += arquivo.FileName + ", ";
                    foto.Delete();
                    continue;
                }

                try
                {
                    ManipulacaoImagem.SalvarImagem(foto.FilePath, arquivo.InputStream);
                }
                catch
                {
                    foto.Delete();
                    arquivosNaoAnexados += arquivo.FileName + ", ";
                    continue;
                }
            }

            return arquivosNaoAnexados = !String.IsNullOrEmpty(arquivosNaoAnexados)
                ? "Arquivos não anexados: " + arquivosNaoAnexados.Trim(' ').Trim(',') +
                  ".\\n\\nCertifique-se de que os nomes dos arquivos estão corretos."
                : "Arquivos anexados com sucesso.";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Glass.Data.Model;
using Glass.Data.DAL;
using System.Web.UI;
using System.Globalization;
using System.IO;
using System.Web;
using Glass.Configuracoes;
using Glass.Global;
using GDA;
using System.Linq;
using ImageMagick;

namespace Glass.Data.Helper
{
    public static class UtilsProjeto
    {
        private static readonly object _calcMedidasPecasLock = new object();
        private static readonly object _calcularExpressaoLock = new object();
        private static readonly object _validarExpressaoLock = new object();

        #region Enumeradores

        public enum GrupoModelo : uint
        {
            Correr08mm = 1,
            PortaPuxSimples,
            PortaPuxDuplo,
            Conj2Pt,
            Fixo,
            Bascula,
            Pivotante,
            Carrinho,
            BoxAbrir,
            CorrerComKit08mm,
            Correr10mm,
            Outros,
            CorrerComKit10mm,
            BoxPadrao
        }

        public enum TipoProdutoProjeto : int
        {
            Aluminio = 1,
            Ferragem,
            Outros,
            Vidro
        }

        #endregion

        #region Modelo de Projeto do grupo outros

        /// <summary>
        /// Retorna o idProjetoModelo referente ao grupo outros.
        /// É utilizado no PCP, quando gera os materiais que não são de nenhum item_projeto
        /// </summary>
        public static uint GetIdProjetoModeloOutros
        {
            get { return 496; }
        }

        #endregion

        #region Table com Medidas da Área de Instalação

        /// <summary>
        /// Cria controles onde serão informados os dados que o Projeto_Modelo passado requer
        /// </summary>
        public static void CreateTableMedInst(ref Table tbMedInst, ItemProjeto itemProjeto, ProjetoModelo projModelo, bool pcp)
        {
            // Cria o cabeçalho
            TableRow trCabecalho = new TableRow();
            TableRow trDados = new TableRow();

            // Variável responsável por habilitar/desabilitar campos da área de instalação
            bool enable = true;

            int distEixoPux = 0;
            if (projModelo.TipoMedidasInst > 0)
                distEixoPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(itemProjeto.IdItemProjeto, 17, false);
            
            HiddenField hdfMedidasAlteradas = new HiddenField();
            hdfMedidasAlteradas.ID = "hdfMedidasAlteradas";
            hdfMedidasAlteradas.Value = "false";

            trCabecalho.Controls.Add(CreateTableCell(hdfMedidasAlteradas, "", 0, null));
            trDados.Controls.Add(new TableCell());

            #region Preenche medidas de projetos cadastrados manualmente

            // Cria o cabeçalho
            List<MedidaProjetoModelo> lstMedida = MedidaProjetoModeloDAO.Instance.GetByProjetoModelo(projModelo.IdProjetoModelo, false);
            foreach (MedidaProjetoModelo mpm in lstMedida)
                trCabecalho.Controls.Add(CreateTableCell(null, mpm.DescrMedida, 70, null));

            // Cria os campos
            foreach (MedidaProjetoModelo mpm in lstMedida)
            {
                int valorMedida = MedidaItemProjetoDAO.Instance.GetByItemProjeto(itemProjeto.IdItemProjeto, mpm.IdMedidaProjeto, false);

                if (valorMedida == 0)
                    valorMedida = MedidaProjetoDAO.Instance.ObtemValorPadrao(mpm.IdMedidaProjeto);

                // Se os campos de medidas estiverem desabilitados, verifica se a medida deve ser editada
                if (itemProjeto.MedidaExata)
                    enable = MedidaProjetoDAO.Instance.ExibirCalcMedidaExata(mpm.IdMedidaProjeto);

                // Se for box padrão e altura vão, traz 1900 por padrão não editável
                if (ProjetoModeloDAO.Instance.IsBoxPadrao(projModelo.IdProjetoModelo) && mpm.IdMedidaProjeto == 3)
                    trDados.Controls.Add(CreateTableCellWithTxt(mpm.TextBoxId, 70,
                        itemProjeto.EspessuraVidro == 6 ? ProjetoConfig.AlturaPadraoProjetoBox6mm : ProjetoConfig.AlturaPadraoProjetoBoxAcima6mm, false, null, true));
                else
                    trDados.Controls.Add(CreateTableCellWithTxt(mpm.TextBoxId, 70, valorMedida, enable, null, true));
            }

            #endregion

            // Se este projeto requerer o eixo do puxador, adiciona textbox
            if (projModelo.EixoPuxador)
            {
                // Preenche a distância do eixo do puxador com 50, e não tiver sido informado
                if (distEixoPux == 0) distEixoPux = 50;

                trCabecalho.Controls.Add(CreateTableCell(null, "Dist. Eixo Puxador", 70, null));
                trDados.Controls.Add(CreateTableCellWithTxt("txtDistEixoPuxadorInst", 70, distEixoPux, true, null, true));
            }

            tbMedInst.Controls.Clear();
            tbMedInst.Controls.Add(trCabecalho);
            tbMedInst.Controls.Add(trDados);
        }

        #endregion

        #region Calcula e Cria Medidas das peças do projeto

        /// <summary>
        /// Calcula Medidas das peças do projeto e depois insere os cálculos na tabela
        /// </summary>
        public static List<PecaProjetoModelo> CalcMedidasPecasComTransacao(ref Table tbPecaModelo, Table tbMedInst, ItemProjeto itemProj,
            ProjetoModelo projModelo, bool calcPecaAuto, bool pcp, out string retornoValidacao)
        {
            return CalcMedidasPecasComTransacao(ref tbPecaModelo, tbMedInst, itemProj, projModelo, calcPecaAuto, pcp, false,
                out retornoValidacao);
        }

        /// <summary>
        /// Calcula Medidas das peças do projeto e depois insere os cálculos na tabela
        /// </summary>
        public static List<PecaProjetoModelo> CalcMedidasPecasComTransacao(ref Table tbPecaModelo, Table tbMedInst, ItemProjeto itemProj,
            ProjetoModelo projModelo, bool calcPecaAuto, bool pcp, bool medidasAlteradas, out string retornoValidacao)
        {
            lock(_calcMedidasPecasLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        retornoValidacao = string.Empty;

                        var retorno = CalcMedidasPecas(transaction, ref tbPecaModelo, tbMedInst, itemProj, projModelo,
                            calcPecaAuto, pcp, medidasAlteradas, out retornoValidacao);

                        transaction.Commit();
                        transaction.Close();

                        return retorno;
                    }
                    catch
                    {
                        transaction.Rollback();
                        transaction.Close();
                        throw;
                    }
                }
            }
        }
        
        /// <summary>
        /// Calcula Medidas das peças do projeto e depois insere os cálculos na tabela
        /// </summary>
        public static List<PecaProjetoModelo> CalcMedidasPecas(GDA.GDASession sessao, ref Table tbPecaModelo, Table tbMedInst, ItemProjeto itemProj,
            ProjetoModelo projModelo, bool calcPecaAuto, bool pcp, bool medidasAlteradas, out string retornoValidacao)
        {
            retornoValidacao = string.Empty;

            // Verifica se todos os textboxes de tbMedInst foram preenchidos
            if (tbMedInst.Controls.Count >= 2)
                foreach (Control c in tbMedInst.Controls[1].Controls)
                    if (c.Controls.Count > 0 && c.Controls[0] is TextBox && ((TextBox)c.Controls[0]).Text == String.Empty)
                    {
                        if (((TextBox)c.Controls[0]).Enabled)
                            throw new Exception("Informe todos as medidas da área de instalação.");
                        else if (String.IsNullOrEmpty(((TextBox)c.Controls[0]).Text))
                            ((TextBox)c.Controls[0]).Text = "0";
                    }

            // Carrega a lista de medidas do modelo de projeto
            List<MedidaProjetoModelo> lstMedProjMod = MedidaProjetoModeloDAO.Instance.GetByProjetoModelo(sessao, itemProj.IdProjetoModelo, true);

            var isBoxPadrao = ProjetoModeloDAO.Instance.IsBoxPadrao(sessao, projModelo.IdProjetoModelo);

            var qtd = 0;
            var largVao = 0;
            var altVao = 0;
            var largPorta = 0;
            var largVaoEsq = 0;
            var largVaoDir = 0;

            if (!medidasAlteradas)
                SalvarMedidasAreaInstalacao(sessao, projModelo, itemProj, tbMedInst, tbPecaModelo);

            BuscarMedidasVaoItemProjeto(sessao, itemProj, isBoxPadrao, out qtd, out largVao, out altVao,
                out largPorta, out largVaoEsq, out largVaoDir);

            // Recupera a quantidade da tabela.
            var medidaQtd = new MedidaItemProjeto();
            TextBox txtQtd = ((TextBox)tbPecaModelo.FindControl("txtQtdMedInst"));
            medidaQtd.IdItemProjeto = itemProj.IdItemProjeto;
            medidaQtd.IdMedidaProjeto = 1;
            medidaQtd.Valor = txtQtd != null && !string.IsNullOrEmpty(txtQtd.Text) && txtQtd.Text != "0" ? txtQtd.Text.StrParaInt() : 1;

            itemProj.Qtde = medidaQtd.Valor;

            // Busca as peças deste item, que serão utilizadas nas expressões
            List<PecaItemProjeto> lstPecaItemProj = PecaItemProjetoDAO.Instance.GetByItemProjeto(sessao, itemProj.IdItemProjeto, projModelo.IdProjetoModelo);

            // Busca as peças com as medidas e produtos inseridos na tela
            List<PecaProjetoModelo> lstPecas = UtilsProjeto.GetPecasFromTable(tbPecaModelo, pcp);

            // Chamado 15837: Se alguma das peças tiver beneficiamento, não permite que as mesmas sejam trocadas
            // Chamado 16671: Se for projeto de box, uma das peças pode ser diferente da outra (fixo e instalação) para resolver foi criada
            // a validação lstPecaItemProj[i].IdProd != lstPecas[i].IdProd
            for (int i = 0; i < lstPecaItemProj.Count; i++)
                foreach (var pecaNova in lstPecas)
                {
                    if (lstPecaItemProj[i].IdProd != pecaNova.IdProd && lstPecaItemProj[i].IdProd != lstPecas[i].IdProd)
                    {
                        var mater = MaterialItemProjetoDAO.Instance.GetMaterialByPeca(lstPecaItemProj[i].IdPecaItemProj);

                        if (mater != null && mater.Beneficiamentos.Count > 0)
                            throw new Exception("Não é permitido trocar peças que possuam beneficiamentos associados.");
                    }

                    //Verifica se o tipo da peça foi alterado no e-commerce caso tenha sido, exibe mensagem de erro.
                    //Não deve ser possivel alterar o tipo da peça no e-commerce, caso tenha sido alterado é erro, por isso esse bloqueio
                    if (UserInfo.GetUserInfo.IsCliente && lstPecaItemProj[i].IdPecaProjMod == pecaNova.IdPecaProjMod)
                        if (lstPecaItemProj[i].Tipo != pecaNova.Tipo)
                            throw new Exception("Não é permitido alterar o tipo da peca.");
                }

            // Se for para calcular as medidas das peças de vidro automaticamente
            if (calcPecaAuto || medidasAlteradas)
            {
                // Busca as peças do modelo cadastrados por padrão para inserir qtd,altura e largura padrão, 
                // e não as medidas inseridas na tela, isto e feito para manter as peças de vidro selecionadas
                List<PecaProjetoModelo> lstPecasModelo = PecaProjetoModeloDAO.Instance.GetByModelo(sessao, itemProj.IdProjetoModelo);
                
                for (int i = 0; i < lstPecas.Count; i++)
                {
                    var ppm = PecaProjetoModeloDAO.Instance.GetByCliente(sessao, lstPecasModelo[i].IdPecaProjMod, itemProj.IdCliente.GetValueOrDefault());

                    // Verifica se há fórmula para calcular a qtd de peças
                    int qtdPeca = !string.IsNullOrEmpty(ppm.CalculoQtde) ?
                        (int)CalcExpressao(sessao, ppm.CalculoQtde, itemProj, null, lstMedProjMod) : ppm.Qtde;

                    lstPecas[i].Qtde = qtdPeca;

                    lstPecas[i].Altura =
                        ProjetoConfig.SelecionarEspessuraAoCalcularProjeto ?
                            (itemProj.EspessuraVidro == 3 ? ppm.Altura03MM :
                            itemProj.EspessuraVidro == 4 ? ppm.Altura04MM :
                            itemProj.EspessuraVidro == 5 ? ppm.Altura05MM :
                            itemProj.EspessuraVidro == 6 ? ppm.Altura06MM :
                            itemProj.EspessuraVidro == 8 ? ppm.Altura08MM :
                            itemProj.EspessuraVidro == 10 ? ppm.Altura10MM :
                            itemProj.EspessuraVidro == 12 ? ppm.Altura12MM : ppm.Altura) :
                            ppm.Altura;

                    lstPecas[i].Largura =
                        ProjetoConfig.SelecionarEspessuraAoCalcularProjeto ?
                            (itemProj.EspessuraVidro == 3 ? ppm.Largura03MM :
                            itemProj.EspessuraVidro == 4 ? ppm.Largura04MM :
                            itemProj.EspessuraVidro == 5 ? ppm.Largura05MM :
                            itemProj.EspessuraVidro == 6 ? ppm.Largura06MM :
                            itemProj.EspessuraVidro == 8 ? ppm.Largura08MM :
                            itemProj.EspessuraVidro == 10 ? ppm.Largura10MM :
                            itemProj.EspessuraVidro == 12 ? ppm.Largura12MM : ppm.Largura) :
                            ppm.Largura;

                    lstPecas[i].IdAplicacao = ppm.IdAplicacao;
                    lstPecas[i].IdProcesso = ppm.IdProcesso;
                    lstPecas[i].Redondo = ppm.Redondo;
                }

                // Se o projeto possuir espessura de tubo, a altura da última (e penúltima) peça deve ser subtraída deste valor
                if (projModelo.TipoMedidasInst == 8 || projModelo.TipoMedidasInst == 9)
                {
                    int espTuboMedInst = MedidaItemProjetoDAO.Instance.GetByItemProjeto(sessao, itemProj.IdItemProjeto, 16, false);

                    // Se este modelo tiver apenas 3 peças, sempre a última peça deverá ter sua altura subtraída da espessura do tubo, 
                    // mas se este modelo tiver 4 peças, as duas últimas peças terão suas alturas subtraídas da espessura do tubo
                    if (lstPecas.Count >= 3)
                        lstPecas[2].Altura -= espTuboMedInst;

                    if (lstPecas.Count == 4)
                        lstPecas[3].Altura -= espTuboMedInst;
                }

                // Pega a quantidade de peças
                int qtdPecas = 0;
                foreach (PecaProjetoModelo ppm in lstPecas)
                {
                    // Verifica se há fórmula para calcular a qtd de peças
                    int qtdPeca = !String.IsNullOrEmpty(ppm.CalculoQtde) ? (int)UtilsProjeto.CalcExpressao(sessao, ppm.CalculoQtde, itemProj, null, lstMedProjMod) : ppm.Qtde;
                    qtdPecas += qtdPeca;
                }

                // Pega a quantidade de peças por projeto, considerando apenas um projeto
                qtdPecas /= qtd;

                #region Calcula as medidas das peças

                float larguraPecas = 0;
                foreach (PecaProjetoModelo ppm in lstPecas)
                {
                    #region Cálculo de Box Padrão

                    if (isBoxPadrao)
                    {
                        int qtdPecasBP = (largVaoDir > 0 ? qtdPecas / 2 : qtdPecas);

                        float largVaoOriginal = largVao > 0 ? largVao :
                        ppm.CalculoLargura.Contains("LARGVAODIR") ? largVaoDir : largVaoEsq;
                        float largVaoCalc = largVaoOriginal;

                        largVaoCalc = largVaoCalc % 100 == 0 ? largVaoCalc : largVaoCalc % 100 <= 20 ? (largVaoCalc - (largVaoCalc % 100) + 50) : largVaoCalc % 100 > 70 ? (largVaoCalc - (largVaoCalc % 100) + 150) : (largVaoCalc - (largVaoCalc % 100) + 100);

                        if (ppm.Largura == 50 && (largVaoCalc / qtdPecasBP) % 50 == 25)
                            ppm.Largura = 25;
                        else if (ppm.Largura == 0 && (largVaoCalc / qtdPecasBP) % 50 == 25)
                            ppm.Largura = -25;
                        else if (largVaoOriginal % 100 != 0)
                            ppm.Largura = 0;

                        if (qtdPecas % 2 == 1)
                        {
                            if (ppm.Largura > 0)
                                ppm.Largura -= (int)((largVaoCalc / qtdPecasBP) % 50);
                            else
                                ppm.Largura -= (int)((largVaoCalc / qtdPecasBP) % 50);
                        }

                        ppm.Altura += (int)CalcExpressao(sessao, ppm.CalculoAltura, itemProj, lstPecaItemProj, lstMedProjMod);
                        ppm.Largura += (int)(largVaoCalc / qtdPecasBP);

                        // Modificação necessária para que caso seja box de 4 peças e o total do vão dividido por 4
                        // não dê múltiplo de 50, o ajuste abaixo corrija esta situação
                        if (qtdPecas == 4)
                        {
                            if (ppm.Largura % 50 > 0)
                                if (ppm.Tipo == 1)
                                    ppm.Largura -= ppm.Largura % 50;
                                else
                                    ppm.Largura += 50 - (ppm.Largura % 50);
                        }

                        larguraPecas += ppm.Largura * (ppm.Qtde / qtd);
                        continue;
                    }

                    #endregion

                    ppm.Altura += (int)Math.Ceiling(CalcExpressao(sessao, ppm.CalculoAltura, itemProj, lstPecaItemProj, lstMedProjMod));
                    ppm.Largura += (int)Math.Ceiling(CalcExpressao(sessao, ppm.CalculoLargura, itemProj, lstPecaItemProj, lstMedProjMod));
                }

                #region Ajuste box padrão 3 ou 4 peças

                // Ajusta a largura da porta de um box padrão de 3 peças de 50 em 50 até ficar maior que a largura do vão
                if (isBoxPadrao && qtdPecas == 3)
                {
                    if ((larguraPecas - largVao) < 30)
                    {
                        while ((larguraPecas - largVao) < 30)
                            foreach (PecaProjetoModelo ppm in lstPecas)
                                if (ppm.Tipo == 1)
                                {
                                    ppm.Largura += 50;
                                    larguraPecas += 50;
                                }
                    }
                    else if ((larguraPecas - largVao) > 75)
                    {
                        while ((larguraPecas - largVao) > 75)
                            foreach (PecaProjetoModelo ppm in lstPecas)
                                if (ppm.Tipo == 1)
                                {
                                    ppm.Largura -= 50;
                                    larguraPecas -= 50;
                                }
                    }
                }

                // Calcula o transpasse de box 4 folhas
                int transpasse = 0;
                if (larguraPecas != largVao && qtdPecas == 4)
                    transpasse = 50;

                // Modifica a largura do vão a fim de calcular corretamente o transpasse das peças móveis
                if (largVao % 50 != 0)
                    largVao = largVao + (50 - (largVao % 50));
                else
                    largVao += 50;

                // Ajusta a largura da porta de um box padrão de 4 peças de 50 em 50 até ficar maior que a largura do vão
                if (isBoxPadrao && qtdPecas == 4 && ((larguraPecas - transpasse) < largVao || larguraPecas == largVao))
                {
                    do
                    {
                        foreach (PecaProjetoModelo ppm in lstPecas)
                            if (ppm.Tipo == 1)
                            {
                                ppm.Largura += 50;
                                larguraPecas += 50;
                            }
                    }
                    while (larguraPecas + transpasse <= largVao);
                }

                // Se a largura do fixo for maior que a da porta, inverte, para que o transpasse fique na porta
                if (isBoxPadrao && qtdPecas == 4 && lstPecas.Count == 2)
                {
                    if ((lstPecas[0].Tipo == 1 && lstPecas[0].Largura < lstPecas[1].Largura) ||
                        (lstPecas[1].Tipo == 1 && lstPecas[0].Largura > lstPecas[1].Largura))
                    {
                        int larguraTemp = lstPecas[0].Largura;
                        lstPecas[0].Largura = lstPecas[1].Largura;
                        lstPecas[1].Largura = larguraTemp;
                    }
                }

                #endregion

                #endregion
            }
            else if (itemProj.MedidaExata)
            {
                // Busca as peças do modelo cadastrados para atribuir processo e aplicação
                List<PecaProjetoModelo> lstPecasModelo = PecaProjetoModeloDAO.Instance.GetByModelo(sessao, itemProj.IdProjetoModelo);

                for (int i = 0; i < lstPecas.Count; i++)
                {
                    lstPecas[i].IdAplicacao = lstPecasModelo[i].IdAplicacao;
                    lstPecas[i].IdProcesso = lstPecasModelo[i].IdProcesso;
                }
            }

            ValidarMedidasPecas(sessao, itemProj, lstPecas, lstMedProjMod, out retornoValidacao);

            return lstPecas;
        }

        #region Salva as medidas da área de instalação

        public static void SalvarMedidasAreaInstalacao(GDASession session, ProjetoModelo projetoModelo, ItemProjeto itemProjeto,
            Table tbMedidasAreaInstalacao, Table tbPecaModelo)
        {
            // Exclui medidas que possam ter sido inseridas neste projeto.
            MedidaItemProjetoDAO.Instance.DeleteByItemProjeto(session, itemProjeto.IdItemProjeto);

            // Insere a quantidade
            var medidaQtd = new MedidaItemProjeto();
            TextBox txtQtd = ((TextBox)tbPecaModelo.FindControl("txtQtdMedInst"));
            medidaQtd.IdItemProjeto = itemProjeto.IdItemProjeto;
            medidaQtd.IdMedidaProjeto = 1;
            medidaQtd.Valor = txtQtd != null && !string.IsNullOrEmpty(txtQtd.Text) && txtQtd.Text != "0" ? txtQtd.Text.StrParaInt() : 1;
            MedidaItemProjetoDAO.Instance.Insert(session, medidaQtd);

            #region Salva as medidas da área de instalação

            // Pega as medidas da área de instalação
            switch (projetoModelo.TipoMedidasInst)
            {
                case 0: // Projeto cadastrado
                    foreach (MedidaProjetoModelo mpm in MedidaProjetoModeloDAO.Instance.GetByProjetoModelo(session, itemProjeto.IdProjetoModelo, false))
                    {
                        TextBox txt = ((TextBox)tbMedidasAreaInstalacao.FindControl(mpm.TextBoxId));

                        var alturaBox = itemProjeto.EspessuraVidro == 6 ? ProjetoConfig.AlturaPadraoProjetoBox6mm : ProjetoConfig.AlturaPadraoProjetoBoxAcima6mm;

                        // Não insere a medida QTD, pois já foi inserida no código acima
                        if (mpm.IdMedidaProjeto != 1 && txt != null && (txt.Enabled || txt.Text == alturaBox.ToString()))
                            MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, mpm.IdMedidaProjeto,
                                txt.Text.StrParaInt());
                    }
                    break;
                case 1: // Qtd, Largura Vão, Altura Vão
                    if (((TextBox)tbMedidasAreaInstalacao.FindControl("txtLargVaoMedInst")).Text != string.Empty) MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 2, ((TextBox)tbMedidasAreaInstalacao.FindControl("txtLargVaoMedInst")).Text.StrParaInt());
                    if (((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltVaoMedInst")).Text != string.Empty) MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 3, ((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltVaoMedInst")).Text.StrParaInt());
                    break;
                case 2: // Qtd, Largura Vão, Altura Vão, Portas(Largura)
                    if (((TextBox)tbMedidasAreaInstalacao.FindControl("txtLargVaoMedInst")).Text != string.Empty) MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 2, ((TextBox)tbMedidasAreaInstalacao.FindControl("txtLargVaoMedInst")).Text.StrParaInt());
                    if (((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltVaoMedInst")).Text != string.Empty) MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 3, ((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltVaoMedInst")).Text.StrParaInt());
                    if (((TextBox)tbMedidasAreaInstalacao.FindControl("txtPortasMedInst")).Text != string.Empty) MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 4, ((TextBox)tbMedidasAreaInstalacao.FindControl("txtPortasMedInst")).Text.StrParaInt());
                    break;
                case 3: // Qtd, Largura Vão, Altura Vão, Trinco
                    if (((TextBox)tbMedidasAreaInstalacao.FindControl("txtLargVaoMedInst")).Text != string.Empty) MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 2, ((TextBox)tbMedidasAreaInstalacao.FindControl("txtLargVaoMedInst")).Text.StrParaInt());
                    if (((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltVaoMedInst")).Text != string.Empty) MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 3, ((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltVaoMedInst")).Text.StrParaInt());
                    MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 15, ((TextBox)tbMedidasAreaInstalacao.FindControl("txtTrincoMedInst")).Text.StrParaInt());
                    break;
                case 4: // Qtd, Largura Vão, Altura Vão, Altura Puxador
                    if (((TextBox)tbMedidasAreaInstalacao.FindControl("txtLargVaoMedInst")).Text != string.Empty) MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 2, ((TextBox)tbMedidasAreaInstalacao.FindControl("txtLargVaoMedInst")).Text.StrParaInt());
                    if (((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltVaoMedInst")).Text != string.Empty) MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 3, ((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltVaoMedInst")).Text.StrParaInt());
                    MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 13, ((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltPuxadorMedInst")).Text.StrParaInt());
                    break;
                case 5: // Qtd, Largura Vão, Altura Vão, Altura Fechadura
                    if (((TextBox)tbMedidasAreaInstalacao.FindControl("txtLargVaoMedInst")).Text != string.Empty) MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 2, ((TextBox)tbMedidasAreaInstalacao.FindControl("txtLargVaoMedInst")).Text.StrParaInt());
                    if (((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltVaoMedInst")).Text != string.Empty) MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 3, ((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltVaoMedInst")).Text.StrParaInt());
                    MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 14, ((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltFechaduraMedInst")).Text.StrParaInt());
                    break;
                case 6: // Qtd, Largura Vão, Altura Vão, Altura Fechadura, Portas(Largura)
                    if (((TextBox)tbMedidasAreaInstalacao.FindControl("txtLargVaoMedInst")).Text != string.Empty) MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 2, ((TextBox)tbMedidasAreaInstalacao.FindControl("txtLargVaoMedInst")).Text.StrParaInt());
                    if (((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltVaoMedInst")).Text != string.Empty) MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 3, ((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltVaoMedInst")).Text.StrParaInt());
                    if (((TextBox)tbMedidasAreaInstalacao.FindControl("txtPortasMedInst")).Text != string.Empty) MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 4, ((TextBox)tbMedidasAreaInstalacao.FindControl("txtPortasMedInst")).Text.StrParaInt());
                    MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 14, ((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltFechaduraMedInst")).Text.StrParaInt());
                    break;
                case 7: // Qtd, Largura Vão, Altura Vão, Altura Puxador, Portas(Largura)
                    if (((TextBox)tbMedidasAreaInstalacao.FindControl("txtLargVaoMedInst")).Text != string.Empty) MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 2, ((TextBox)tbMedidasAreaInstalacao.FindControl("txtLargVaoMedInst")).Text.StrParaInt());
                    if (((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltVaoMedInst")).Text != string.Empty) MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 3, ((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltVaoMedInst")).Text.StrParaInt());
                    if (((TextBox)tbMedidasAreaInstalacao.FindControl("txtPortasMedInst")).Text != string.Empty) MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 4, ((TextBox)tbMedidasAreaInstalacao.FindControl("txtPortasMedInst")).Text.StrParaInt());
                    MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 13, ((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltPuxadorMedInst")).Text.StrParaInt());
                    break;
                case 8: // Qtd, Largura Vão, Altura Porta, Altura Vão, Altura Fechadura, Esp Tubo
                    if (((TextBox)tbMedidasAreaInstalacao.FindControl("txtLargVaoMedInst")).Text != string.Empty) MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 2, ((TextBox)tbMedidasAreaInstalacao.FindControl("txtLargVaoMedInst")).Text.StrParaInt());
                    if (((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltVaoMedInst")).Text != string.Empty) MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 3, ((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltVaoMedInst")).Text.StrParaInt());
                    if (((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltPortaMedInst")).Text != string.Empty) MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 5, ((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltPortaMedInst")).Text.StrParaInt());
                    MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 14, ((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltFechaduraMedInst")).Text.StrParaInt());
                    MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 16, ((TextBox)tbMedidasAreaInstalacao.FindControl("txtEspTuboMedInst")).Text.StrParaInt());
                    break;
                case 9: // Qtd, Largura Vão, Altura Porta, Altura Vão, Altura Puxador, Esp Tubo
                    if (((TextBox)tbMedidasAreaInstalacao.FindControl("txtLargVaoMedInst")).Text != string.Empty) MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 2, Glass.Conversoes.StrParaInt(((TextBox)tbMedidasAreaInstalacao.FindControl("txtLargVaoMedInst")).Text));
                    if (((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltVaoMedInst")).Text != string.Empty) MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 3, Glass.Conversoes.StrParaInt(((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltVaoMedInst")).Text));
                    if (((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltPortaMedInst")).Text != string.Empty) MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 5, Glass.Conversoes.StrParaInt(((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltPortaMedInst")).Text));
                    MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 13, Glass.Conversoes.StrParaInt(((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltPuxadorMedInst")).Text));
                    MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 16, Glass.Conversoes.StrParaInt(((TextBox)tbMedidasAreaInstalacao.FindControl("txtEspTuboMedInst")).Text));
                    break;
                case 10: // Qtd, Largura Vão, Altura Vão, Altura Puxador
                    if (((TextBox)tbMedidasAreaInstalacao.FindControl("txtLargVaoMedInst")).Text != string.Empty) MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 2, Glass.Conversoes.StrParaInt(((TextBox)tbMedidasAreaInstalacao.FindControl("txtLargVaoMedInst")).Text));
                    if (((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltVaoMedInst")).Text != string.Empty) MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 3, Glass.Conversoes.StrParaInt(((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltVaoMedInst")).Text));
                    MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 13, Glass.Conversoes.StrParaInt(((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltPuxadorMedInst")).Text));
                    break;
                case 11: // Qtd, Largura Porta, Largura Vão, Altura Vão, Altura Puxador
                    if (((TextBox)tbMedidasAreaInstalacao.FindControl("txtLargVaoMedInst")).Text != string.Empty) MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 2, Glass.Conversoes.StrParaInt(((TextBox)tbMedidasAreaInstalacao.FindControl("txtLargVaoMedInst")).Text));
                    if (((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltVaoMedInst")).Text != string.Empty) MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 3, Glass.Conversoes.StrParaInt(((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltVaoMedInst")).Text));
                    if (((TextBox)tbMedidasAreaInstalacao.FindControl("txtLargPortaMedInst")).Text != string.Empty) MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 4, Glass.Conversoes.StrParaInt(((TextBox)tbMedidasAreaInstalacao.FindControl("txtLargPortaMedInst")).Text));
                    MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 13, Glass.Conversoes.StrParaInt(((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltPuxadorMedInst")).Text));
                    break;
                case 12: // Qtd, Largura Vão, Altura porta, Altura Vão, Altura Puxador
                    if (((TextBox)tbMedidasAreaInstalacao.FindControl("txtLargVaoMedInst")).Text != string.Empty) MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 2, Glass.Conversoes.StrParaInt(((TextBox)tbMedidasAreaInstalacao.FindControl("txtLargVaoMedInst")).Text));
                    if (((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltVaoMedInst")).Text != string.Empty) MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 3, Glass.Conversoes.StrParaInt(((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltVaoMedInst")).Text));
                    if (((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltPortaMedInst")).Text != string.Empty) MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 5, Glass.Conversoes.StrParaInt(((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltPortaMedInst")).Text));
                    MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 13, Glass.Conversoes.StrParaInt(((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltPuxadorMedInst")).Text));
                    break;
                case 13: // Qtd, Largura Porta, Largura Vão, Altura Porta, Altura Vão, Altura Puxador
                    if (((TextBox)tbMedidasAreaInstalacao.FindControl("txtLargVaoMedInst")).Text != string.Empty) MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 2, Glass.Conversoes.StrParaInt(((TextBox)tbMedidasAreaInstalacao.FindControl("txtLargVaoMedInst")).Text));
                    if (((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltVaoMedInst")).Text != string.Empty) MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 3, Glass.Conversoes.StrParaInt(((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltVaoMedInst")).Text));
                    if (((TextBox)tbMedidasAreaInstalacao.FindControl("txtLargPortaMedInst")).Text != string.Empty) MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 4, Glass.Conversoes.StrParaInt(((TextBox)tbMedidasAreaInstalacao.FindControl("txtLargPortaMedInst")).Text));
                    if (((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltPortaMedInst")).Text != string.Empty) MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 5, Glass.Conversoes.StrParaInt(((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltPortaMedInst")).Text));
                    MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 13, Glass.Conversoes.StrParaInt(((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltPuxadorMedInst")).Text));
                    break;
                case 14: // Qtd, Largura Vão, Altura Inferior, Altura Vão
                    if (((TextBox)tbMedidasAreaInstalacao.FindControl("txtLargVaoMedInst")).Text != string.Empty) MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 2, Glass.Conversoes.StrParaInt(((TextBox)tbMedidasAreaInstalacao.FindControl("txtLargVaoMedInst")).Text));
                    if (((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltVaoMedInst")).Text != string.Empty) MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 3, Glass.Conversoes.StrParaInt(((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltVaoMedInst")).Text));
                    if (((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltInferiorMedInst")).Text != string.Empty) MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 12, Glass.Conversoes.StrParaInt(((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltInferiorMedInst")).Text));
                    break;
                case 15: // Qtd, Largura Colante, Largura Passante, Altura Vão
                    if (((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltVaoMedInst")).Text != string.Empty) MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 3, Glass.Conversoes.StrParaInt(((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltVaoMedInst")).Text));
                    if (((TextBox)tbMedidasAreaInstalacao.FindControl("txtLargColanteMedInst")).Text != string.Empty) MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 10, Glass.Conversoes.StrParaInt(((TextBox)tbMedidasAreaInstalacao.FindControl("txtLargColanteMedInst")).Text));
                    if (((TextBox)tbMedidasAreaInstalacao.FindControl("txtLargPassanteMedInst")).Text != string.Empty) MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 11, Glass.Conversoes.StrParaInt(((TextBox)tbMedidasAreaInstalacao.FindControl("txtLargPassanteMedInst")).Text));
                    break;
                case 16: // Qtd, Largura Vão, Altura Báscula, Altura Vão
                    if (((TextBox)tbMedidasAreaInstalacao.FindControl("txtLargVaoMedInst")).Text != string.Empty) MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 2, Glass.Conversoes.StrParaInt(((TextBox)tbMedidasAreaInstalacao.FindControl("txtLargVaoMedInst")).Text));
                    if (((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltVaoMedInst")).Text != string.Empty) MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 3, Glass.Conversoes.StrParaInt(((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltVaoMedInst")).Text));
                    if (((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltBasculaMedInst")).Text != string.Empty) MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 7, Glass.Conversoes.StrParaInt(((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltBasculaMedInst")).Text));
                    break;
                case 17: // Qtd, Largura Vão, Largura Báscula, Altura Vão
                    if (((TextBox)tbMedidasAreaInstalacao.FindControl("txtLargVaoMedInst")).Text != string.Empty) MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 2, Glass.Conversoes.StrParaInt(((TextBox)tbMedidasAreaInstalacao.FindControl("txtLargVaoMedInst")).Text));
                    if (((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltVaoMedInst")).Text != string.Empty) MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 3, Glass.Conversoes.StrParaInt(((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltVaoMedInst")).Text));
                    if (((TextBox)tbMedidasAreaInstalacao.FindControl("txtLargBasculaMedInst")).Text != string.Empty) MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 6, Glass.Conversoes.StrParaInt(((TextBox)tbMedidasAreaInstalacao.FindControl("txtLargBasculaMedInst")).Text));
                    break;
                case 18: // Qtd, Largura Báscula, Largura Vão, Altura Báscula, Altura Vão
                    if (((TextBox)tbMedidasAreaInstalacao.FindControl("txtLargVaoMedInst")).Text != string.Empty) MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 2, Glass.Conversoes.StrParaInt(((TextBox)tbMedidasAreaInstalacao.FindControl("txtLargVaoMedInst")).Text));
                    if (((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltVaoMedInst")).Text != string.Empty) MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 3, Glass.Conversoes.StrParaInt(((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltVaoMedInst")).Text));
                    if (((TextBox)tbMedidasAreaInstalacao.FindControl("txtLargBasculaMedInst")).Text != string.Empty) MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 6, Glass.Conversoes.StrParaInt(((TextBox)tbMedidasAreaInstalacao.FindControl("txtLargBasculaMedInst")).Text));
                    if (((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltBasculaMedInst")).Text != string.Empty) MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 7, Glass.Conversoes.StrParaInt(((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltBasculaMedInst")).Text));
                    break;
                case 19: // Qtd, Largura Pivotante, Largura Vão, Altura Vão
                    if (((TextBox)tbMedidasAreaInstalacao.FindControl("txtLargVaoMedInst")).Text != string.Empty) MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 2, Glass.Conversoes.StrParaInt(((TextBox)tbMedidasAreaInstalacao.FindControl("txtLargVaoMedInst")).Text));
                    if (((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltVaoMedInst")).Text != string.Empty) MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 3, Glass.Conversoes.StrParaInt(((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltVaoMedInst")).Text));
                    if (((TextBox)tbMedidasAreaInstalacao.FindControl("txtLargPivotanteMedInst")).Text != string.Empty) MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 8, Glass.Conversoes.StrParaInt(((TextBox)tbMedidasAreaInstalacao.FindControl("txtLargPivotanteMedInst")).Text));
                    break;
                case 20: // Qtd, Largura Vão, Altura Pivotante, Altura Vão
                    if (((TextBox)tbMedidasAreaInstalacao.FindControl("txtLargVaoMedInst")).Text != string.Empty) MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 2, Glass.Conversoes.StrParaInt(((TextBox)tbMedidasAreaInstalacao.FindControl("txtLargVaoMedInst")).Text));
                    if (((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltVaoMedInst")).Text != string.Empty) MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 3, Glass.Conversoes.StrParaInt(((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltVaoMedInst")).Text));
                    if (((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltPivotanteMedInst")).Text != string.Empty) MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 9, Glass.Conversoes.StrParaInt(((TextBox)tbMedidasAreaInstalacao.FindControl("txtAltPivotanteMedInst")).Text));
                    break;
            }

            // Pega a distância do eixo do puxador, se houver
            if (projetoModelo.EixoPuxador && !string.IsNullOrEmpty(((TextBox)tbMedidasAreaInstalacao.FindControl("txtDistEixoPuxadorInst")).Text))
                MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, 17, Glass.Conversoes.StrParaInt(((TextBox)tbMedidasAreaInstalacao.FindControl("txtDistEixoPuxadorInst")).Text));

            #endregion
        }

        #endregion

        #region Busca as medidas da área de instalação

        public static void BuscarMedidasVaoItemProjeto(GDASession session, ItemProjeto itemProjeto, bool boxPadrao,
            out int qtd, out int largVao, out int altVao, out int largPorta, out int largVaoEsq, out int largVaoDir)
        {
            #region Busca medidas da área da instalação para projetos fixos

            qtd = 0;
            largVao = 0;
            altVao = 0;
            largPorta = 0;
            largVaoEsq = 0;
            largVaoDir = 0;

            // Campo usado mesmo nos projetos cadastrados
            qtd = MedidaItemProjetoDAO.Instance.GetByItemProjeto(session, itemProjeto.IdItemProjeto, 1, false);

            if (boxPadrao)
            {
                largVao = MedidaItemProjetoDAO.Instance.GetByItemProjeto(session, itemProjeto.IdItemProjeto, 2, false);
                altVao = MedidaItemProjetoDAO.Instance.GetByItemProjeto(session, itemProjeto.IdItemProjeto, 3, false);
                largPorta = MedidaItemProjetoDAO.Instance.GetByItemProjeto(session, itemProjeto.IdItemProjeto, 4, false);
                largVaoEsq = MedidaItemProjetoDAO.Instance.GetByItemProjeto(session, itemProjeto.IdItemProjeto, 19, false);
                largVaoDir = MedidaItemProjetoDAO.Instance.GetByItemProjeto(session, itemProjeto.IdItemProjeto, 20, false);
            }

            #endregion
        }

        #endregion

        #region Valida as medidas das peças do projeto

        public static void ValidarMedidasPecas(GDASession session, ItemProjeto itemProjeto, List<PecaProjetoModelo> pecasProjetoModelo,
            List<MedidaProjetoModelo> medidasProjetoModelo, out string retornoValidacao)
        {
            retornoValidacao = string.Empty;

            var retornoAlerta = string.Empty;
            var retornoBloqueio = string.Empty;

            for (var i = 0; i < pecasProjetoModelo.Count; i++)
            {
                var validacoesPecaProjetoModelo = ValidacaoPecaModeloDAO.Instance.ObtemValidacoes(session, (int)pecasProjetoModelo[i].IdPecaProjMod);

                if (validacoesPecaProjetoModelo == null ||
                    validacoesPecaProjetoModelo.Count == 0 ||
                    validacoesPecaProjetoModelo[0].IdValidacaoPecaModelo == 0 ||
                    string.IsNullOrEmpty(validacoesPecaProjetoModelo[0].PrimeiraExpressaoValidacao) ||
                    string.IsNullOrEmpty(validacoesPecaProjetoModelo[0].SegundaExpressaoValidacao) ||
                    validacoesPecaProjetoModelo[0].TipoComparador == 0 ||
                    /* Chamado 49118. */
                    pecasProjetoModelo[i].Qtde == 0)
                    continue;

                foreach (var vppm in validacoesPecaProjetoModelo)
                {
                    #region Validação da expressão (necessário porque podem ser inseridas por SQL)

                    if (string.IsNullOrEmpty(vppm.PrimeiraExpressaoValidacao) ||
                        string.IsNullOrEmpty(vppm.SegundaExpressaoValidacao) ||
                        vppm.TipoComparador == 0)
                        continue;

                    #endregion

                    #region Expressões

                    var primeiraExpressaoValidacao = vppm.PrimeiraExpressaoValidacao;
                    var segundaExpressaoValidacao = vppm.SegundaExpressaoValidacao;

                    for (var j = 0; j < pecasProjetoModelo.Count(); j++)
                    {
                        var item = PecaProjetoModeloDAO.Instance.ObtemItem(session, (int)pecasProjetoModelo[j].IdPecaProjMod);

                        primeiraExpressaoValidacao =
                            primeiraExpressaoValidacao
                                .Replace(string.Format("P{0}ALT", item), pecasProjetoModelo[j].Altura.ToString())
                                .Replace(string.Format("P{0}LARG", item), pecasProjetoModelo[j].Largura.ToString())
                                .Replace("QTDE", pecasProjetoModelo[j].Qtde.ToString())
                                .Replace("QTD", pecasProjetoModelo[j].Qtde.ToString())
                                .Replace(string.Format("P{0}ESP", item),
                                    pecasProjetoModelo[j].IdProd > 0 ?
                                        ProdutoDAO.Instance.ObtemEspessura(session, (int)pecasProjetoModelo[j].IdProd).ToString() :
                                        itemProjeto.EspessuraVidro.ToString());

                        segundaExpressaoValidacao =
                            segundaExpressaoValidacao
                                .Replace(string.Format("P{0}ALT", item), pecasProjetoModelo[j].Altura.ToString())
                                .Replace(string.Format("P{0}LARG", item), pecasProjetoModelo[j].Largura.ToString())
                                .Replace("QTDE", pecasProjetoModelo[j].Qtde.ToString())
                                .Replace("QTD", pecasProjetoModelo[j].Qtde.ToString())
                                .Replace(string.Format("P{0}ESP", item),
                                    pecasProjetoModelo[j].IdProd > 0 ?
                                        ProdutoDAO.Instance.ObtemEspessura(session, (int)pecasProjetoModelo[j].IdProd).ToString() :
                                        itemProjeto.EspessuraVidro.ToString());
                    }

                    var resultadoPrimeiraExpressao =
                        (int)CalcExpressao(session, primeiraExpressaoValidacao, itemProjeto, null, medidasProjetoModelo);

                    var resultadoSegundaExpressao =
                        (int)CalcExpressao(session, segundaExpressaoValidacao, itemProjeto, null, medidasProjetoModelo);

                    #endregion

                    #region Validação das expressões

                    var exibirMensagemValidacao = false;

                    switch (vppm.TipoComparador)
                    {
                        case (int)ValidacaoPecaModelo.TipoComparadorExpressaoValidacao.Igual:
                            exibirMensagemValidacao = resultadoPrimeiraExpressao == resultadoSegundaExpressao;
                            break;

                        case (int)ValidacaoPecaModelo.TipoComparadorExpressaoValidacao.Maior:
                            exibirMensagemValidacao = resultadoPrimeiraExpressao > resultadoSegundaExpressao;
                            break;

                        case (int)ValidacaoPecaModelo.TipoComparadorExpressaoValidacao.Menor:
                            exibirMensagemValidacao = resultadoPrimeiraExpressao < resultadoSegundaExpressao;
                            break;

                        case (int)ValidacaoPecaModelo.TipoComparadorExpressaoValidacao.MaiorOuIgual:
                            exibirMensagemValidacao = resultadoPrimeiraExpressao >= resultadoSegundaExpressao;
                            break;

                        case (int)ValidacaoPecaModelo.TipoComparadorExpressaoValidacao.MenorOuIgual:
                            exibirMensagemValidacao = resultadoPrimeiraExpressao <= resultadoSegundaExpressao;
                            break;

                        case (int)ValidacaoPecaModelo.TipoComparadorExpressaoValidacao.Diferente:
                            exibirMensagemValidacao = resultadoPrimeiraExpressao != resultadoSegundaExpressao;
                            break;
                    }

                    if (exibirMensagemValidacao)
                    {
                        var descricaoTipoValidacao = string.Empty;

                        switch (vppm.TipoValidacao)
                        {
                            case (int)ValidacaoPecaModelo.TipoValidacaoPecaModelo.Bloquear:
                                descricaoTipoValidacao = "Bloqueio";
                                break;

                            case (int)ValidacaoPecaModelo.TipoValidacaoPecaModelo.SomenteInformar:
                                descricaoTipoValidacao = "Alerta";
                                break;

                            case (int)ValidacaoPecaModelo.TipoValidacaoPecaModelo.ConsiderarConfiguracao:
                                descricaoTipoValidacao =
                                    ProjetoConfig.ValidacaoProjetoConfiguravel == (int)ValidacaoPecaModelo.TipoValidacaoPecaModelo.Bloquear ?
                                        "Bloqueio" :
                                        ProjetoConfig.ValidacaoProjetoConfiguravel == (int)ValidacaoPecaModelo.TipoValidacaoPecaModelo.SomenteInformar ?
                                            "Alerta" : string.Empty;
                                break;

                        }

                        if (descricaoTipoValidacao == "Alerta")
                            retornoAlerta += string.Format("\nAlerta (Item {0}): {1}", i + 1, vppm.Mensagem);
                        else if (descricaoTipoValidacao == "Bloqueio")
                            retornoBloqueio += string.Format("\nBloqueio (Item {0}): {1}", i + 1, vppm.Mensagem);
                    }

                    #endregion
                }
            }

            if (!string.IsNullOrEmpty(retornoAlerta) || !string.IsNullOrEmpty(retornoBloqueio))
            {
                retornoValidacao += string.Format("{0}\n{1}", retornoBloqueio,
                    string.IsNullOrEmpty(retornoAlerta) ? "\n" : string.Format("{0}\n\n", retornoAlerta));

                if (retornoValidacao.Contains("Bloqueio (Item"))
                    throw new Exception("Cálculo das medidas bloqueado.");
            }
        }

        #endregion

        #region Verifica se as medidas da área de instalação foram alteradas

        public static bool VerificaMedidasAreaInstalacaoAlteradas(GDASession session, ItemProjeto itemProjeto,
            ProjetoModelo projetoModelo, Table tbMedidasAreaInstalacao, Table tbPecaModelo)
        {
            // Carrega a lista de medidas do item de projeto.
            var medidasItemProjeto =
                MedidaItemProjetoDAO.Instance.GetListByItemProjeto(session, itemProjeto.IdItemProjeto)
                    .Select(f =>
                        new
                        {
                            f.IdItemProjeto,
                            f.IdMedidaProjeto,
                            f.NomeMedidaProjeto,
                            f.Valor
                        }).ToList();

            SalvarMedidasAreaInstalacao(session, projetoModelo, itemProjeto, tbMedidasAreaInstalacao, tbPecaModelo);

            // Carrega a lista de medidas atualizadas do item de projeto.
            var medidasItemProjetoNovas =
                MedidaItemProjetoDAO.Instance.GetListByItemProjeto(session, itemProjeto.IdItemProjeto)
                    .Select(f =>
                        new
                        {
                            f.IdItemProjeto,
                            f.IdMedidaProjeto,
                            f.NomeMedidaProjeto,
                            f.Valor
                        }).ToList();

            if (medidasItemProjeto.Count != medidasItemProjetoNovas.Count)
                return true;
            else
            {
                for (var i = 0; i < medidasItemProjeto.Count; i++)
                    if (!medidasItemProjeto[i].Equals(medidasItemProjetoNovas[i]))
                        return true;
            }

            return false;
        }

        #endregion

        #endregion

        #region Monta tabela com peças do modelo

        /// <summary>
        /// Monta a tabela de peças de vidro do item_projeto, com as medidas já cadastradas
        /// </summary>
        /// <param name="tbPecaModelo">Table que será montada as peças do modelo passado</param>
        /// <param name="idItemProjeto"></param>
        /// <param name="itemProjeto"></param>
        /// <param name="pcp"></param>
        /// <param name="visualizar"></param>
        public static void CreateTablePecasItemProjeto(ref Table tbPecaModelo, uint idItemProjeto, ItemProjeto itemProjeto,
            bool pcp, bool visualizar, bool ecommerce)
        {
            CreateTablePecasItemProjeto(null, ref tbPecaModelo, idItemProjeto, itemProjeto, pcp, visualizar, ecommerce);
        }

        /// <summary>
        /// Monta a tabela de peças de vidro do item_projeto, com as medidas já cadastradas
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="tbPecaModelo">Table que será montada as peças do modelo passado</param>
        /// <param name="idItemProjeto"></param>
        /// <param name="itemProjeto"></param>
        /// <param name="pcp"></param>
        /// <param name="visualizar"></param>
        public static void CreateTablePecasItemProjeto(GDA.GDASession sessao, ref Table tbPecaModelo, uint idItemProjeto, ItemProjeto itemProjeto,
            bool pcp, bool visualizar, bool ecommerce)
        {
            // Cria tabela com valores padrões
            CreateTablePecasModelo(sessao, ref tbPecaModelo, PecaProjetoModeloDAO.Instance.GetByModelo(sessao, itemProjeto.IdProjetoModelo), itemProjeto,
                pcp, visualizar, ecommerce);

            // Busca peças cadastradas
            List<PecaItemProjeto> lstPecaItemProjeto = PecaItemProjetoDAO.Instance.GetByItemProjeto(sessao, idItemProjeto, itemProjeto.IdProjetoModelo);

            if (lstPecaItemProjeto != null && lstPecaItemProjeto.Count > 0)
            {
                // Altera o valor dos campos
                for (int i = 0; i < lstPecaItemProjeto.Count; i++)
                {
                    ((HiddenField)tbPecaModelo.FindControl("hdfIdPecaItemProj" + (i + 1))).Value = lstPecaItemProjeto[i].IdPecaItemProj.ToString();
                    ((HiddenField)tbPecaModelo.FindControl("hdfIdProdPeca" + (i + 1))).Value = lstPecaItemProjeto[i].IdProd.Value.ToString();
                    ((TextBox)tbPecaModelo.FindControl("txtDescrProdPeca" + (i + 1))).Text = lstPecaItemProjeto[i].DescrProduto != null ? lstPecaItemProjeto[i].DescrProduto.ToString() : String.Empty;
                    ((TextBox)tbPecaModelo.FindControl("txtQtdPeca" + (i + 1))).Text = lstPecaItemProjeto[i].Qtde.ToString();
                    ((TextBox)tbPecaModelo.FindControl("txtLargPeca" + (i + 1))).Text = lstPecaItemProjeto[i].Largura.ToString();
                    ((TextBox)tbPecaModelo.FindControl("txtAltPeca" + (i + 1))).Text = lstPecaItemProjeto[i].Altura.ToString();

                    if (pcp)
                        ((DropDownList)tbPecaModelo.FindControl("drpTipoPeca" + (i + 1))).SelectedValue = lstPecaItemProjeto[i].Tipo.ToString();
                }
            }
            else
            {
                var contador = 0;

                while (((HiddenField)tbPecaModelo.FindControl("hdfIdPecaItemProj" + ++contador)) != null)
                {
                    ((HiddenField)tbPecaModelo.FindControl("hdfIdPecaItemProj" + contador)).Value = "";
                    ((HiddenField)tbPecaModelo.FindControl("hdfIdProdPeca" + contador)).Value = "";
                    ((TextBox)tbPecaModelo.FindControl("txtDescrProdPeca" + contador)).Text = "";
                }
            }
        }

        /// <summary>
        /// Retorna os itens passado por string em formato de vetor
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string[] GetItensFromPeca(string item)
        {
            if (item == null)
                return null;

            // Formata o item
            string itemTemp = item.Replace(",", " ").Replace("e", " ").Replace("E", " ").Replace("-", "");
            while (itemTemp.Contains("  "))
                itemTemp = itemTemp.Replace("  ", " ");

            return itemTemp.Trim().Split(' ');
        }

        /// <summary>
        /// Retorna o número do item da peça de uma etiqueta.
        /// </summary>
        /// <param name="peca"></param>
        /// <param name="numEtiqueta"></param>
        /// <returns></returns>
        public static int GetItemPecaFromEtiqueta(string itemPeca, string numEtiqueta)
        {
            /* Chamado 16230.
             * Ao buscar as peças do carregamento aparecia uma mensagem de erro, porque o numEtiqueta estava vazio e ao passar pelo
             * numEtiqueta.Substring, acontecia o erro de "referência de objeto..". */
            if (String.IsNullOrEmpty(numEtiqueta))
                return 0;

            string posicaoEtiqueta = numEtiqueta.Substring(numEtiqueta.IndexOf('.') + 1);
            posicaoEtiqueta = posicaoEtiqueta.Substring(0, posicaoEtiqueta.IndexOf('/'));

            string[] vetItem = GetItensFromPeca(itemPeca);

            if (vetItem == null)
                return 0;

            return Glass.Conversoes.StrParaInt(vetItem[(Glass.Conversoes.StrParaInt(posicaoEtiqueta) - 1) % (vetItem.Length)]);
        }

        public static void CreateTablePecasModelo(ref Table tbPecaModelo, List<PecaProjetoModelo> lstPeca, ItemProjeto itemProjeto,
            bool pcp, bool visualizar, bool ecommerce)
        {
            CreateTablePecasModelo(null, ref tbPecaModelo, lstPeca, itemProjeto, pcp, visualizar, ecommerce);
        }

        public static void CreateTablePecasModelo(GDA.GDASession sessao, ref Table tbPecaModelo, List<PecaProjetoModelo> lstPeca, ItemProjeto itemProjeto,
            bool pcp, bool visualizar, bool ecommerce)
        {
            // Se a lista de peças não tiver sido passada, carrega as peças do modelo
            if (lstPeca == null)
                lstPeca = PecaProjetoModeloDAO.Instance.GetByModelo(sessao, itemProjeto.IdProjetoModelo);

            // Copia a tabela de peças em uma tabela temporária
            Table tbPecaTemp = tbPecaModelo;
            tbPecaTemp.Controls.Clear();

            // Pega a quantidade 
            bool medidasInseridas = MedidaItemProjetoDAO.Instance.ExistsMedida(sessao, itemProjeto.IdItemProjeto);

            //Cliente pode ediar com CAD Project
            var clienteCadProject = false;
            if(ecommerce && itemProjeto != null && itemProjeto.IdProjeto.GetValueOrDefault(0) > 0)
            {
                var idCli = ProjetoDAO.Instance.ObtemIdCliente(itemProjeto.IdProjeto.Value);
                clienteCadProject = ClienteDAO.Instance.ObtemHabilitarEditorCad(idCli.GetValueOrDefault(0));
            }
            else if (ecommerce && itemProjeto != null && itemProjeto.IdPedido.GetValueOrDefault(0) > 0)
            {
                var idCli = PedidoDAO.Instance.ObtemIdCliente(sessao, itemProjeto.IdPedido.Value);
                clienteCadProject = ClienteDAO.Instance.ObtemHabilitarEditorCad(idCli);
            }

            // Variável que irá habilitar/desabilitar campos das peças
            bool readOnly =
                visualizar ||
                (!itemProjeto.MedidaExata &&
                /* Chamado 32881. */
                !(pcp && ProjetoConfig.PermitirAlterarMedidasPecasProjetoCalculoVaoPCP));

            bool alteraImagemPeca = false;

            #region Monta o cabeçalho

            HiddenField hdfPecasAlteradas = new HiddenField();
            hdfPecasAlteradas.ID = "hdfPecasAlteradas";
            hdfPecasAlteradas.Value = "false";

            TableRow linhaPeca = new TableRow();
            linhaPeca.Controls.Add(CreateTableCell(hdfPecasAlteradas, "", 0, null));
            linhaPeca.Controls.Add(CreateTableCell(null, "Qtd.", 50, "dtvHeader"));
            linhaPeca.Controls.Add(CreateTableCell(null, "Largura", 50, "dtvHeader"));
            linhaPeca.Controls.Add(CreateTableCell(null, "Altura", 50, "dtvHeader"));
            linhaPeca.Controls.Add(CreateTableCell(null, "Item", 50, "dtvHeader"));
            linhaPeca.Controls.Add(CreateTableCell(null, "Tipo", 50, "dtvHeader"));
            tbPecaTemp.Controls.Add(linhaPeca);

            #endregion

            #region Insere as peças na tela

            int cont = 1;

            // Insere as peças na tela
            foreach (PecaProjetoModelo ppm in lstPeca)
            {
                TableRow linhaItem = new TableRow();

                // Seleciona Produto
                TextBox txtIdProd = new TextBox();
                txtIdProd.ID = "txtIdProdPeca" + cont;
                txtIdProd.Width = 40;
                txtIdProd.Attributes.Add("onkeydown", "if (isEnter(event)) { hdfVidro=FindControl('hdfIdProdPeca" + cont + "', 'input'); txtVidro=FindControl('txtDescrProdPeca" + cont + "', 'input'); setVidro(this.value);}");
                txtIdProd.Attributes.Add("onkeypress", "return !(isEnter(event));");
                txtIdProd.Attributes.Add("onblur", "hdfVidro=FindControl('hdfIdProdPeca" + cont + "', 'input'); txtVidro=FindControl('txtDescrProdPeca" + cont + "', 'input'); setVidro(this.value);");
                if (visualizar) txtIdProd.Style.Value = "display: none";

                HiddenField hdfIdPecaItemProj = new HiddenField();
                hdfIdPecaItemProj.ID = "hdfIdPecaItemProj" + cont;
                if (tbPecaModelo.FindControl("hdfIdPecaItemProj" + cont) != null)
                    hdfIdPecaItemProj.Value = ((TextBox)tbPecaModelo.FindControl("hdfIdPecaItemProj" + cont)).Text;

                HiddenField hdfIdProdPeca = new HiddenField();
                hdfIdProdPeca.ID = "hdfIdProdPeca" + cont;
                if (tbPecaModelo.FindControl("hdfIdProdPeca" + cont) != null)
                    hdfIdProdPeca.Value = ((TextBox)tbPecaModelo.FindControl("hdfIdProdPeca" + cont)).Text;

                HiddenField hdfIdPecaProjMod = new HiddenField();
                hdfIdPecaProjMod.ID = "hdfIdPecaProjMod" + cont;
                hdfIdPecaProjMod.Value = ppm.IdPecaProjMod.ToString();

                HiddenField hdfCalcAltura = new HiddenField();
                hdfCalcAltura.ID = "hdfCalcAltura" + cont;
                hdfCalcAltura.Value = ppm.CalculoAltura;

                HiddenField hdfCalcLargura = new HiddenField();
                hdfCalcLargura.ID = "hdfCalcLargura" + cont;
                hdfCalcLargura.Value = ppm.CalculoLargura;

                HiddenField hdfCalcQtde = new HiddenField();
                hdfCalcQtde.ID = "hdfCalcQtde" + cont;
                hdfCalcQtde.Value = ppm.CalculoQtde;

                HiddenField hdfObs = new HiddenField();
                hdfObs.ID = "hdfObs" + cont;
                hdfObs.Value = ppm.Obs;

                TextBox txtDescrProdPeca = new TextBox();
                txtDescrProdPeca.BorderStyle = BorderStyle.None;
                txtDescrProdPeca.Width = new Unit(150, UnitType.Pixel);
                txtDescrProdPeca.Font.Size = FontUnit.Smaller;
                txtDescrProdPeca.ReadOnly = true;
                txtDescrProdPeca.ID = "txtDescrProdPeca" + cont;
                if (tbPecaModelo.FindControl("hdfIdProdPeca" + cont) != null)
                    txtDescrProdPeca.Text = ProdutoDAO.Instance.GetByCodInterno(sessao, hdfIdProdPeca.Value).Descricao;
                else
                    txtDescrProdPeca.Text = "";

                LinkButton lnkIdProd = new LinkButton();
                lnkIdProd.OnClientClick = "hdfVidro=FindControl('hdfIdProdPeca" + cont + "', 'input'); txtVidro=FindControl('txtDescrProdPeca" + cont +
                    "', 'input'); openWindow(450, 700, '" + lnkIdProd.ResolveClientUrl("~/Utils/SelProd.aspx") + 
                    "?callback=setVidro&proj=1&Parceiro=1&idItemProjeto=' + FindControl('hdfIdItemProjeto', 'input').value + '&idCliente=' + GetQueryString('idCliente')); return false;";

                lnkIdProd.Text = "<img src=\"" + lnkIdProd.ResolveClientUrl("~/Images/Pesquisar.gif") + "\" border=\"0\" />";
                if (visualizar) lnkIdProd.Style.Value = "display: none";

                TableCell cel = new TableCell();
                cel.Style.Add("padding", "2 2 2 2px");
                cel.Attributes.Add("align", "center");
                cel.Controls.Add(hdfIdPecaProjMod);
                cel.Controls.Add(hdfCalcAltura);
                cel.Controls.Add(hdfCalcLargura);
                cel.Controls.Add(hdfCalcQtde);
                cel.Controls.Add(hdfIdPecaItemProj);
                cel.Controls.Add(hdfIdProdPeca);
                cel.Controls.Add(txtDescrProdPeca);
                cel.Controls.Add(txtIdProd);
                cel.Controls.Add(lnkIdProd);
                cel.Controls.Add(hdfObs);
                linhaItem.Controls.Add(cel);

                // Qtd
                TextBox cTxt = new TextBox();
                cTxt.BorderStyle = readOnly ? BorderStyle.None : BorderStyle.NotSet;
                cTxt.Width = 30;
                cTxt.Attributes.Add("onkeypress", "return soNumeros(event, true, true);");
                cTxt.ID = "txtQtdPeca" + cont;
                int qtdPeca = !String.IsNullOrEmpty(ppm.CalculoQtde) && medidasInseridas ? (int)UtilsProjeto.CalcExpressao(sessao, ppm.CalculoQtde, itemProjeto, null) : ppm.Qtde;
                cTxt.Text = qtdPeca.ToString();
                cTxt.ReadOnly = readOnly;
                cTxt.Attributes.Add("onkeyup", "FindControl('hdfPecasAlteradas', 'input').value = 'true';");
                linhaItem.Controls.Add(UtilsProjeto.CreateTableCell(cTxt, null, 40, null));

                // Largura
                cTxt = new TextBox();
                cTxt.BorderStyle = readOnly ? BorderStyle.None : BorderStyle.NotSet;
                cTxt.Width = 40;
                cTxt.Attributes.Add("onkeypress", "return soNumeros(event, true, true);");
                cTxt.ID = "txtLargPeca" + cont;
                cTxt.Attributes.Add("onkeyup", "FindControl('hdfPecasAlteradas', 'input').value = 'true';");

                cTxt.Text =
                    !itemProjeto.MedidaExata ?
                        ppm.Largura.ToString() :
                        0.ToString();

                cTxt.ReadOnly = readOnly;
                linhaItem.Controls.Add(UtilsProjeto.CreateTableCell(cTxt, null, 40, null));

                // Altura
                cTxt = new TextBox();
                cTxt.BorderStyle = readOnly ? BorderStyle.None : BorderStyle.NotSet;
                cTxt.Width = 40;
                cTxt.Attributes.Add("onkeypress", "return soNumeros(event, true, true);");
                cTxt.ID = "txtAltPeca" + cont;
                cTxt.Attributes.Add("onkeyup", "FindControl('hdfPecasAlteradas', 'input').value = 'true';");

                cTxt.Text =
                    !itemProjeto.MedidaExata ?
                        ppm.Altura.ToString() :
                        0.ToString();

                cTxt.ReadOnly = readOnly;
                linhaItem.Controls.Add(UtilsProjeto.CreateTableCell(cTxt, null, 40, null));

                // Item
                cTxt = new TextBox();
                cTxt.BorderStyle = BorderStyle.None;
                cTxt.Width = 40;
                cTxt.ReadOnly = true;
                cTxt.ID = "txtItemPeca" + cont;
                cTxt.Text = ppm.Item;
                cTxt.ReadOnly = true;
                linhaItem.Controls.Add(UtilsProjeto.CreateTableCell(cTxt, null, 40, null));

                PecaItemProjeto peca = PecaItemProjetoDAO.Instance.GetElementExt(sessao, itemProjeto.IdItemProjeto, ppm.IdPecaProjMod);

                /* Chamado 51333. */
                if (peca == null)
                {
                    DropDownList cDrp = new DropDownList();
                    cDrp.Items.Add(new ListItem("I", "1"));
                    cDrp.Items.Add(new ListItem("CX", "2"));
                    cDrp.SelectedValue = ppm.Tipo.ToString();
                    cDrp.ID = "drpTipoPeca" + cont;
                    cDrp.Enabled = !visualizar && !ecommerce;
                    cDrp.Attributes.Add("onchange", "FindControl('hdfPecasAlteradas', 'input').value = 'true'");
                    linhaItem.Controls.Add(UtilsProjeto.CreateTableCell(cDrp, null, 0, null));
                }
                else
                {
                    DropDownList cDrp = new DropDownList();
                    cDrp.Items.Add(new ListItem("I", "1"));
                    cDrp.Items.Add(new ListItem("CX", "2"));
                    cDrp.SelectedValue = peca.Tipo.ToString();
                    cDrp.ID = "drpTipoPeca" + cont;
                    cDrp.Enabled = !visualizar && !ecommerce;
                    cDrp.Attributes.Add("onchange", "FindControl('hdfPecasAlteradas', 'input').value = 'true'");
                    linhaItem.Controls.Add(UtilsProjeto.CreateTableCell(cDrp, null, 0, null));

                    // Imagem (Apenas para peça de instalação)
                    if (peca.Tipo == 1 && (pcp || (ProjetoConfig.InserirImagemProjetoPedido && (UserInfo.GetUserInfo.IsCliente? clienteCadProject : true))))
                    {
                        // Controla a posição da etiqueta (ex: 1/3)
                        int posEtiquetaQtde = 1;

                        // Insere uma imagem de edição para cada item
                        foreach (string item in GetItensFromPeca(ppm.Item))
                        {

                            if (!PodeAlterarImagemPeca(sessao, peca, Glass.Conversoes.StrParaInt(item), posEtiquetaQtde++, true))
                                continue;

                            alteraImagemPeca = true;

                            Table tbl = new Table();
                            tbl.CellPadding = 0;
                            tbl.CellSpacing = 0;
                            tbl.Rows.AddRange(new TableRow[] { new TableRow(), new TableRow(), new TableRow() });

                            tbl.Rows[0].Cells.Add(new TableCell());
                            tbl.Rows[0].Cells[0].Text = "Item " + item;
                            tbl.Rows[0].Cells[0].Wrap = false;
                            tbl.Rows[0].Cells[0].Font.Bold = true;
                            tbl.Rows[0].Cells[0].Style.Value = "text-align: center";

                            tbl.Rows[1].Cells.Add(new TableCell());
                            tbl.Rows[1].Cells[0].Style.Value = "text-align: center";

                            // Só desenha a imagem de edição da peça se for pedido/pedido espelho
                            ImageButton cImb = new ImageButton();
                            cImb.ID = string.Format("cImb_{0}", item);

                            if (ProjetoConfig.UtilizarEditorImagensProjeto)
                            {
                                if (itemProjeto.IdOrcamento > 0)
                                    // Não permite edição da imagem para o orçamento (precisa do IdProdPed)
                                    cImb.OnClientClick = "alert('Não é possível editar imagens durante a confecção do orçamento, apenas no pedido/PCP.'); return false";

                                else
                                    // Só pode editar as peças se o pedido estiver conferido
                                    cImb.OnClientClick = @"
                                    var estaConferido = MetodosAjax.EstaConferido(FindControl('hdfIdItemProjeto', 'input').value);
                                
                                    if (estaConferido != null && estaConferido.value == 'false') {
                                        alert('Confirme o projeto antes de editar as imagens.'); 
                                        return false;
                                    }

                                    var idPecaItemProj = FindControl('hdfIdPecaItemProj" + cont + @"', 'input').value;
                                    var pecaPossuiMaterial = CadProjetoAvulso.PecaPossuiMaterial(idPecaItemProj);

                                    if (pecaPossuiMaterial != null && pecaPossuiMaterial.value == 'false') {
                                        alert('O produto não foi criado. Informe a quantidade, altura e largura da peça, confirme o projeto e tente novamente.');
                                        return false;
                                    }

                                    openWindow(500, 700, " + (tbl.AppRelativeTemplateSourceDirectory.Contains("Parceiros") ?
                                                                    " '../Cadastros/Projeto/EditarImagem.aspx?idProjetoModelo=" :
                                                                    " 'EditarImagem.aspx?idProjetoModelo=") +
                                                                    itemProjeto.IdProjetoModelo + "&idItemProjeto=" + itemProjeto.IdItemProjeto +
                                            "&idPecaItemProj=' + idPecaItemProj + '&item=" + item + "'); return false;";
                            }
                            else
                                cImb.OnClientClick = "openWindow(500, 700, 'DesenhaProjeto.aspx?idItemProjeto=" + itemProjeto.IdItemProjeto +
                                    "&idPecaItemProj=' + FindControl('hdfIdPecaItemProj" + cont + "', 'input').value + '&item=" + item + "'); return false;";

                            cImb.ImageUrl = "~/Images/clipboard.gif";
                            cImb.ToolTip = "Alterar imagem da peça";

                            if (!ProjetoConfig.UtilizarEditorCADImagensProjeto || peca.IdArquivoMesaCorte.GetValueOrDefault() == 0)
                                tbl.Rows[1].Cells[0].Controls.Add(cImb);
                            else
                            {
                                if (peca.IdArquivoMesaCorte > 0)
                                {
                                    var idArquivoCalcEngine = ArquivoMesaCorteDAO.Instance.ObtemIdArquivoCalcEngine(peca.IdArquivoMesaCorte.Value);

                                    if (idArquivoCalcEngine > 0)
                                    {
                                        var cImb2 = new ImageButton();
                                        cImb2.ID = string.Format("cImb2_{0}", item);

                                        if (itemProjeto.IdOrcamento > 0)
                                            // Não permite edição da imagem para o orçamento (precisa do IdProdPed)
                                            cImb2.OnClientClick = "alert('Não é possível editar imagens durante a confecção do orçamento, apenas no pedido/PCP.'); return false";

                                        else
                                        {
                                            // Só pode editar as peças se o pedido estiver conferido
                                            cImb2.OnClientClick = @"return abrirCADProject('" + itemProjeto.CodigoModelo + "'," + peca.IdPecaItemProj + ");";
                                        }

                                        cImb2.ImageUrl = "~/Images/clipboard.gif";
                                        cImb2.ToolTip = "Alterar imagem da peça";

                                        tbl.Rows[2].Cells.Add(new TableCell());
                                        tbl.Rows[2].Cells[0].Style.Value = "text-align: center";
                                        tbl.Rows[2].Cells[0].Controls.Add(cImb2);
                                    }
                                }
                            }

                            #region Cria o controle de log

                            try
                            {
                                object log = ((Page)HttpContext.Current.CurrentHandler).LoadControl("~/Controls/ctrlLogPopup.ascx");

                                if (log != null)
                                {
                                    log.GetType().GetProperty("Tabela").SetValue(log, LogAlteracao.TabelaAlteracao.ImagemProducao, null);
                                    peca.Item = item;
                                    log.GetType().GetProperty("IdRegistro").SetValue(log, peca.IdLog, null);
                                    tbl.Rows[1].Cells[0].Controls.Add(log as TemplateControl);
                                }
                            }
                            catch { }

                            #endregion

                            linhaItem.Cells.Add(UtilsProjeto.CreateTableCell(tbl, null, 20, null));
                        }
                    }
                }

                cont++;

                tbPecaTemp.Rows.Add(linhaItem);
            }

            #endregion

            if (tbPecaModelo.Rows.Count > 0 && tbPecaModelo.Rows[0].Cells.Count > 0)
            {
                HiddenField hdfAlteraImagemPeca = new HiddenField();
                hdfAlteraImagemPeca.ID = "hdfAlteraImagemPeca";
                hdfAlteraImagemPeca.Value = alteraImagemPeca.ToString();

                tbPecaModelo.Rows[0].Cells[0].Controls.Add(hdfAlteraImagemPeca);
            }

            tbPecaModelo = tbPecaTemp;
        }

        /// <summary>
        /// Identifica se a imagem de um item de uma peça pode ser alterada.
        /// </summary>
        public static bool PodeAlterarImagemPeca(PecaItemProjeto peca, int itemPeca, int posEtiquetaQtde,
            bool bloquearSeHouverArquivoSAG)
        {
            var msgErro = string.Empty;
            return PodeAlterarImagemPeca(peca, itemPeca, posEtiquetaQtde, bloquearSeHouverArquivoSAG, ref msgErro);
        }

        /// <summary>
        /// Identifica se a imagem de um item de uma peça pode ser alterada.
        /// </summary>
        public static bool PodeAlterarImagemPeca(PecaItemProjeto peca, int itemPeca, int posEtiquetaQtde,
            bool bloquearSeHouverArquivoSAG, ref string msgErro)
        {
            return PodeAlterarImagemPeca(null, peca, itemPeca, posEtiquetaQtde, bloquearSeHouverArquivoSAG, ref msgErro);
        }

        /// <summary>
        /// Identifica se a imagem de um item de uma peça pode ser alterada.
        /// </summary>
        public static bool PodeAlterarImagemPeca(GDA.GDASession sessao, PecaItemProjeto peca, int itemPeca,
            int posEtiquetaQtde, bool bloquearSeHouverArquivoSAG)
        {
            var msgErro = string.Empty;
            return PodeAlterarImagemPeca(sessao, peca, itemPeca, posEtiquetaQtde, bloquearSeHouverArquivoSAG, ref msgErro);
        }

        /// <summary>
        /// Identifica se a imagem de um item de uma peça pode ser alterada.
        /// </summary>
        public static bool PodeAlterarImagemPeca(GDA.GDASession sessao, PecaItemProjeto peca, int itemPeca, int posEtiquetaQtde,
            bool bloquearSeHouverArquivoSAG, ref string msgErro)
        {
            // Garante que haja uma peça item projeto
            if (peca == null)
                return false;

            bool temArquivoSAG = false;
            string etiqueta = "";

            // Verifica se a situação do pedido é impresso
            if (peca.IdPedido > 0 && PedidoEspelhoDAO.Instance.ObtemSituacao(sessao, peca.IdPedido.Value) == PedidoEspelho.SituacaoPedido.Impresso)
            {
                /* Chamado 23212. */
                if (PecaProjetoModeloDAO.Instance.ObtemTipoArquivoMesaCorte(peca.IdPecaProjMod) == TipoArquivoMesaCorte.FML)
                    return false;

                // Recupera o setor de marcação
                Setor setor = SetorDAO.Instance.ObterSetorPorNome(sessao, "Marcação");

                // Verifica se o usuário possui a permissão de alterar a imagem após impressão de etiquetas e se o setor de marcação existe
                if (!Config.PossuiPermissao(Config.FuncaoMenuPCP.AlterarImagemPecaAposImpressao))
                {
                    msgErro = "Usuário não pode alterar a imagem da peça após a impressão";
                    return false;
                }
                else if (setor.IdSetor == 0)
                {
                    msgErro = "Não foi possível encontrar o setor de marcação.";
                    return false;
                }

                // Busca o número da etiqueta de acordo com o item da peça
                foreach (string e in peca.Etiquetas.Split(','))
                {
                    // Recupera o item da etiqueta
                    string ie = e.Split('-', '.', '/')[2];

                    if (ie == posEtiquetaQtde.ToString())
                    {
                        etiqueta = e.Trim();
                        break;
                    }
                }

                if (String.IsNullOrEmpty(etiqueta))
                    return false;

                // Se tiver passado pelo setor de marcação não exibe
                if (LeituraProducaoDAO.Instance.PassouSetor(sessao, etiqueta, (uint)setor.IdSetor))
                {
                    msgErro = "A peça já foi marcada no setor de marcação, não é possível alterar a imagem.";
                    return false;
                }
            }
            else if (peca.IdPedido > 0 && (PedidoEspelhoDAO.Instance.ObtemSituacao(sessao, peca.IdPedido.Value) == PedidoEspelho.SituacaoPedido.Finalizado ||
                PedidoEspelhoDAO.Instance.ObtemSituacao(sessao, peca.IdPedido.Value) == PedidoEspelho.SituacaoPedido.ImpressoComum))
            {
                /* Chamado 23212. */
                if (PecaProjetoModeloDAO.Instance.ObtemTipoArquivoMesaCorte(peca.IdPecaProjMod) == TipoArquivoMesaCorte.FML)
                    return false;

                int pos = ProdutosPedidoEspelhoDAO.Instance.GetProdPosition(sessao, peca.IdPedido.Value, peca.IdProdPed.Value);
                float qtde = ProdutosPedidoEspelhoDAO.Instance.ObtemQtde(sessao, peca.IdProdPed.Value);

                etiqueta = RelDAL.EtiquetaDAO.Instance.GetNumEtiqueta(peca.IdPedido.Value, pos, posEtiquetaQtde, (int)qtde, ProdutoImpressaoDAO.TipoEtiqueta.Pedido);

                if (String.IsNullOrEmpty(etiqueta))
                    return false;
            }

            // Verifica se a etiqueta tem arquivo de otimização
            temArquivoSAG = EtiquetaArquivoOtimizacaoDAO.Instance.TemArquivoSAG(sessao, etiqueta);

            // Se houver arquivo SAG e o bloqueio for necessário não permite alterar
            if (bloquearSeHouverArquivoSAG && temArquivoSAG)
                return false;

            if (peca.IdProdPed > 0)
            {
                // Verifica se a imagem existe
                ProdutosPedidoEspelho ppe = ProdutosPedidoEspelhoDAO.Instance.GetForImagemPeca(sessao, peca.IdProdPed.Value);
                ppe.Item = itemPeca;

                // Se existir arquivo de mesa não exibe. Ae existir imagem ou se o funcionário tenha permissão para alterar a imagem após a mesma ter sido impressa, então exibe.
                return !temArquivoSAG || Utils.ArquivoExiste(ppe.ImagemUrlSalvarItem) ||
                    Config.PossuiPermissao(Config.FuncaoMenuPCP.AlterarImagemPecaAposImpressao);
            }
            else
                return true;
        }

        #endregion

        #region Extrai List<PecasModeloProjeto> da tabela tbPecasModelo

        /// <summary>
        /// Retorna Lista de PecasModeloProjeto a partir da tbPecasModelo
        /// </summary>
        public static List<PecaProjetoModelo> GetPecasFromTable(Table tbPecaModelo, bool pcp)
        {
            List<PecaProjetoModelo> lstPecas = new List<PecaProjetoModelo>();

            for (int i = 1; i < tbPecaModelo.Rows.Count; i++)
            {
                string idProd = ((HiddenField)tbPecaModelo.FindControl("hdfIdProdPeca" + i)).Value;
                string idPecaItemProj = ((HiddenField)tbPecaModelo.FindControl("hdfIdPecaItemProj" + i)).Value;
                string idPecaProjMod = ((HiddenField)tbPecaModelo.FindControl("hdfIdPecaProjMod" + i)).Value;

                if (String.IsNullOrEmpty(idProd))
                    throw new Exception("Informe todas as peças de vidro.");

                lstPecas.Add(new PecaProjetoModelo
                {
                    IdPecaProjMod = idPecaProjMod.StrParaUint(),
                    IdPecaItemProj = !String.IsNullOrEmpty(idPecaItemProj) ? idPecaItemProj.StrParaUint() : 0,
                    IdProd = idProd.StrParaUint(),
                    Qtde = ((TextBox)tbPecaModelo.FindControl("txtQtdPeca" + i)).Text.StrParaInt(),
                    Obs = ((HiddenField)tbPecaModelo.FindControl("hdfObs" + i)).Value,
                    Largura = ((TextBox)tbPecaModelo.FindControl("txtLargPeca" + i)).Text.StrParaInt(),
                    Altura = ((TextBox)tbPecaModelo.FindControl("txtAltPeca" + i)).Text.StrParaInt(),
                    CalculoAltura = ((HiddenField)tbPecaModelo.FindControl("hdfCalcAltura" + i)).Value,
                    CalculoLargura = ((HiddenField)tbPecaModelo.FindControl("hdfCalcLargura" + i)).Value,
                    CalculoQtde = ((HiddenField)tbPecaModelo.FindControl("hdfCalcQtde" + i)).Value,
                    Tipo = ((DropDownList)tbPecaModelo.FindControl("drpTipoPeca" + i)) != null ? ((DropDownList)tbPecaModelo.FindControl("drpTipoPeca" + i)).SelectedValue.StrParaInt() : 0
                });
            }

            return lstPecas;
        }

        #endregion

        #region Cálculo de qtd de materiais do projeto

        /// <summary>
        /// Calcula a quantidade/comprimento do material passado
        /// </summary>
        public static void CalcMaterial(ref MaterialItemProjeto material, ItemProjeto itemProj, ProjetoModelo modelo,
            uint idProdEscova, uint idProdMaoDeObra)
        {
            CalcMaterial(null, ref material, itemProj, modelo, idProdEscova, idProdMaoDeObra);
        }

        /// <summary>
        /// Calcula a quantidade/comprimento do material passado
        /// </summary>
        public static void CalcMaterial(GDASession sessao, ref MaterialItemProjeto material, ItemProjeto itemProj, ProjetoModelo modelo,
            uint idProdEscova, uint idProdMaoDeObra)
        {
            // Tipo de calculo do modelo passado
            int calcAluminio = modelo.TipoCalcAluminio;

            Dictionary<uint, MedidaItemProjeto> lstMedida = new Dictionary<uint, MedidaItemProjeto>();
            foreach (MedidaItemProjeto m in MedidaItemProjetoDAO.Instance.GetListByItemProjeto(sessao, itemProj.IdItemProjeto))
                if (!lstMedida.ContainsKey(m.IdMedidaProjeto))
                    lstMedida.Add(m.IdMedidaProjeto, m);

            Single qtd = lstMedida.ContainsKey(1) ? lstMedida[1].Valor : 0;
            int largVaoMed = lstMedida.ContainsKey(2) ? lstMedida[2].Valor : 0;
            int altVaoMed = lstMedida.ContainsKey(3) ? lstMedida[3].Valor : 0;
            int largPortaMed = lstMedida.ContainsKey(4) ? lstMedida[4].Valor : 0;
            int altPortaMed = lstMedida.ContainsKey(5) ? lstMedida[5].Valor : 0;
            int largBascMed = lstMedida.ContainsKey(6) ? lstMedida[6].Valor : 0;
            int altBascMed = lstMedida.ContainsKey(7) ? lstMedida[7].Valor : 0;
            int largPivMed = lstMedida.ContainsKey(8) ? lstMedida[8].Valor : 0;
            int altPivMed = lstMedida.ContainsKey(9) ? lstMedida[9].Valor : 0;

            // Arredona medidas
            int largVao = Glass.Global.CalculosFluxo.ArredondaLargAlt(largVaoMed);
            int altVao = Glass.Global.CalculosFluxo.ArredondaLargAlt(altVaoMed);
            int altPorta = Glass.Global.CalculosFluxo.ArredondaLargAlt(altPortaMed);
            int largPorta = Glass.Global.CalculosFluxo.ArredondaLargAlt(largPortaMed);

            List<PecaItemProjeto> lstPeca = null;

            if (Glass.Data.DAL.GrupoProdDAO.Instance.IsAluminio((int)material.IdGrupoProd) || (calcAluminio == 0 && !String.IsNullOrEmpty(material.CalculoAltura))) // Calcula material ALUMÍNIO
            {
                material.Qtde = 1;

                if (lstPeca == null)
                    lstPeca = PecaItemProjetoDAO.Instance.GetByItemProjeto(sessao, itemProj.IdItemProjeto, modelo.IdProjetoModelo);

                #region Realiza o cálculo por tipoCalcAluminio

                // Faz o cálculo da metragem linear que cada alumínio terá que ter com as medidas passadas
                switch (calcAluminio)
                {
                    case 0:
                        if (!String.IsNullOrEmpty(material.CalculoAltura))
                            material.Altura = CalcExpressao(sessao, material.CalculoAltura, itemProj, lstPeca);

                        if (!String.IsNullOrEmpty(material.CalculoQtde))
                            material.Qtde = (int)CalcExpressao(sessao, material.CalculoQtde, itemProj, lstPeca);

                        break;
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                    case 11:
                        switch (material.CodMaterial)
                        {
                            case "AL13":
                            case "AL43":
                            case "AL49":
                            case "AL50":
                            case "AL51":
                            case "AL52":
                                material.Altura = (largVao * qtd) / 1000F; break;
                            case "AL17":
                            case "AL47":
                            case "AL67":
                                material.Altura = (altVao * qtd) / 1000F; break;
                            case "AL05":
                                if (calcAluminio == 2 || calcAluminio == 6)
                                    material.Altura = ((largVao * qtd) / 2) / 1000F;
                                else
                                    material.Altura = (largVao * qtd) / 1000F;
                                break;
                            case "AL10":
                            case "AL10-A":
                            case "AL10-B":
                            case "AL10-C":
                            case "AL10-D":
                            case "AL10-E":
                                if (calcAluminio == 1)
                                    material.Altura = (altVao * qtd * 2) / 1000F;
                                if (calcAluminio == 3 || calcAluminio == 7)
                                    material.Altura = ((altVao * qtd * 2) + (largVao * qtd * 2)) / 1000F;
                                else if (calcAluminio == 6)
                                    material.Altura = (altVao * qtd) / 1000F;
                                else if (calcAluminio == 10)
                                    material.Altura = ((largVao * qtd) + (altVao * qtd * 2) + (altVao - altPorta)) / 1000F;
                                else
                                    material.Altura = (altVao * qtd * 2) / 1000F;
                                break;
                            case "AL15":
                                if (calcAluminio == 1)
                                    material.Altura = (altVao * qtd * 2) / 1000F;
                                else if (calcAluminio == 5 || calcAluminio >= 7)
                                    material.Altura = (altVao * qtd) / 1000F;
                                else
                                    material.Altura = (altVao * qtd * 2) / 1000F;
                                break;
                            case "AL66":
                            case "AL66-A":
                            case "AL66-B":
                                if (calcAluminio == 3 || calcAluminio == 7)
                                    material.Altura = ((altVao * qtd * 2) + (largVao * qtd * 2)) / 1000F;
                                else if (calcAluminio == 4)
                                    material.Altura = ((largVao * qtd) + (altVao * qtd * 2)) / 1000F;
                                else if (calcAluminio == 8)
                                    material.Altura = ((altVao * qtd) + (largVao * qtd * 2)) / 1000F;
                                else if (calcAluminio == 9)
                                    material.Altura = ((largVao * qtd) + ((altVao * qtd) * 1.5F)) / 1000F;
                                else if (calcAluminio == 11)
                                    material.Altura = (altVao * qtd) / 1000F;
                                else
                                    material.Altura = (altVao * qtd * 2) / 1000F;
                                break;
                            case "AL75":
                                if (calcAluminio == 2 || calcAluminio == 6)
                                    material.Altura = ((largVao * qtd) / 2) / 1000F;
                                else
                                    material.Altura = (largVao * qtd) / 1000F;
                                break;
                        }
                        break;
                    case 12:
                        material.Altura = (((largVao - largPorta) * qtd * 2) + (altVao * qtd)) / 1000F; break;
                    case 13:
                        material.Altura = ((largPorta * qtd) + ((altVao - altPorta) * qtd * 2)) / 1000F; break;
                    case 14:
                        material.Altura = ((((largVao * 2) - largPorta) * qtd) + (((altVao * 2) - altPorta) * qtd)) / 1000F; break;
                    case 15:
                        material.Altura = ((((largVao * 2) - largPorta) * qtd) + (altVao * 2 * qtd)) / 1000F; break;
                    case 16:
                        material.Altura = (((largVao * 2) + (altVao * 2) - largPorta) * qtd) / 1000F; break;
                    case 17:
                        material.Altura = ((((largVao - (largPorta * 2)) * 2) * qtd) + (altVao * 2 * qtd)) / 1000F; break;
                    case 18:
                        material.Altura = (((largVao + (largPorta * 2)) * qtd) + (altVao * 2 * qtd)) / 1000F; break;
                    case 19:
                        material.Altura = ((((largVao * 2) - (largPorta * 2)) * qtd) + (altVao * 2 * qtd)) / 1000F; break;
                    case 20:
                        material.Altura = ((largVao * 2 * qtd) + (altVao * 2 * qtd)) / 1000F; break;
                    case 21:
                        material.Altura = (((Glass.Global.CalculosFluxo.ArredondaLargAlt(MedidaItemProjetoDAO.Instance.GetByItemProjeto(sessao, itemProj.IdItemProjeto, 10, false) + MedidaItemProjetoDAO.Instance.GetByItemProjeto(sessao, itemProj.IdItemProjeto, 11, false))) * 2 * qtd) + (altVao * 2 * qtd)) / 1000F; break;
                    case 22:
                        material.Altura = ((largVao * (int)qtd * 2) + (altVao * (int)qtd * 2)) / 1000F; break;
                    case 23:
                        material.Altura = ((largVao * qtd) + ((Glass.Global.CalculosFluxo.ArredondaLargAlt(altVaoMed - altBascMed)) * qtd * 2)) / 1000F; break;
                    case 24:
                        material.Altura = ((largVao * qtd * 2) + (Glass.Global.CalculosFluxo.ArredondaLargAlt(altVaoMed - altBascMed) * 2 * qtd)) / 1000F; break;
                    case 25:
                        material.Altura = ((Glass.Global.CalculosFluxo.ArredondaLargAlt(largVaoMed - largBascMed) * qtd * 2) + (altVao * qtd)) / 1000F; break;
                    case 26:
                        material.Altura = ((Glass.Global.CalculosFluxo.ArredondaLargAlt(largVaoMed - largBascMed) * 2 * qtd) + (altVao * qtd * 2)) / 1000F; break;
                    case 27:
                        material.Altura = ((Glass.Global.CalculosFluxo.ArredondaLargAlt((largVao * 2) - largBascMed) * qtd) + (Glass.Global.CalculosFluxo.ArredondaLargAlt((altVaoMed * 2) - altBascMed) * qtd)) / 1000F; break;
                    case 28:
                        material.Altura = ((Glass.Global.CalculosFluxo.ArredondaLargAlt((largVaoMed * 2) - largBascMed) * qtd) + (altVao * 2 * qtd)) / 1000F; break;
                    case 29:
                        material.Altura = (Glass.Global.CalculosFluxo.ArredondaLargAlt(largVaoMed - (largBascMed * 2)) * qtd) / 1000F; break;
                    case 30:
                        material.Altura = ((Glass.Global.CalculosFluxo.ArredondaLargAlt((largVaoMed * 2) - (largBascMed * 2)) * qtd) + (Glass.Global.CalculosFluxo.ArredondaLargAlt(altVaoMed - altBascMed) * 2 * qtd)) / 1000F; break;
                    case 31:
                        material.Altura = ((Glass.Global.CalculosFluxo.ArredondaLargAlt(largVaoMed - largPivMed) * 2 * qtd) + (altVao * 2 * qtd)) / 1000F; break;
                    case 32:
                        material.Altura = (Glass.Global.CalculosFluxo.ArredondaLargAlt(largVaoMed + ((altVaoMed - altPivMed) * 2)) * qtd) / 1000F; break;
                    case 33:
                        material.Altura = ((largVao * 2 * qtd) + ((Glass.Global.CalculosFluxo.ArredondaLargAlt(altVaoMed - altPivMed) * 2 * qtd))) / 1000F; break;
                    case 34:
                        if (material.CodMaterial == "AL37" || material.CodMaterial == "AL68")
                            material.Altura = (largVao * qtd) / 1000F;
                        else if (material.CodMaterial == "AL06")
                            material.Altura = (largVao * qtd * 2) / 1000F;
                        else if (material.CodMaterial.Contains("AL10") || material.CodMaterial == "AL67")
                            material.Altura = (altVao * qtd * 2) / 1000F;
                        break;
                    case 35:
                        if (material.CodMaterial == "AL68" || material.CodMaterial == "AL06")
                            material.Altura = (largVao * qtd * 2) / 1000F;
                        else if (material.CodMaterial.Contains("AL10"))
                            material.Altura = ((altVao * 2 * qtd) + (((largVao) / 3) * 2)) / 1000F;
                        break;
                    case 36:
                        if (material.CodMaterial == "AL48")
                            material.Altura = (largVao * qtd) / 1000F;
                        else if (material.CodMaterial.Contains("AL66"))
                            material.Altura = (altVao * qtd) / 1000F;
                        break;
                }

                float alturaCalc = material.Altura;
                decimal total = material.Total;
                decimal custo = material.Custo;
                CalculosFluxo.CalcTamanhoAluminio(sessao, (int)material.IdProd, ref alturaCalc, Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo((int)material.IdProd), 1, material.Valor,
                    material.Qtde, ref total, ref custo);

                material.AlturaCalc = alturaCalc;

                #endregion
            }
            else if (!String.IsNullOrEmpty(material.CalculoQtde) && ProjetoModeloDAO.Instance.IsConfiguravel(sessao, itemProj.IdProjetoModelo))
            {
                if (lstPeca == null)
                    lstPeca = PecaItemProjetoDAO.Instance.GetByItemProjeto(sessao, itemProj.IdItemProjeto, modelo.IdProjetoModelo);

                // Se for mão de obra, não utiliza expressão de cálculo (Apenas para projetos iniciais do sistema)
                if (material.IdProd == idProdMaoDeObra && material.QtdModelo > 0)
                    material.Qtde = material.QtdModelo * PecaItemProjetoDAO.Instance.GetQtdPecaItemProjeto(sessao, material.IdItemProjeto);
                else
                    material.Qtde = (int)CalcExpressao(sessao, material.CalculoQtde, itemProj, lstPeca);
            }
            else if (material.IdProd == idProdEscova) // Calcula material ESCOVA
            {
                switch (modelo.IdGrupoModelo)
                {
                    case (uint)GrupoModelo.Correr08mm:
                    case (uint)GrupoModelo.Correr10mm:
                    case (uint)GrupoModelo.CorrerComKit08mm:
                    case (uint)GrupoModelo.CorrerComKit10mm:
                        int codModelo = modelo.Codigo.Replace("CR", "").Replace("K", "").StrParaInt();
                        if (codModelo < 20 || codModelo > 42)
                            material.Qtde = ((largVao * (int)qtd * 2) + (altVao * (int)qtd * 4)) / 1000;
                        else
                            material.Qtde = ((largVao * (int)qtd * 2) + (altVao * (int)qtd * 2)) / 1000;
                        break;
                    case (uint)GrupoModelo.Carrinho:
                        material.Qtde = (largVao * (int)qtd * 4) / 1000;
                        break;
                }
            }
            // Se for mão de obra, pega o total de peças do cálculo
            else if (material.IdProd == idProdMaoDeObra)
                material.Qtde = material.QtdModelo * PecaItemProjetoDAO.Instance.GetQtdPecaItemProjeto(sessao, material.IdItemProjeto);
            else if (Glass.Data.DAL.GrupoProdDAO.Instance.IsFerragem((int)material.IdGrupoProd)) // Calcula material FERRAGEM
                material.Qtde = material.QtdModelo * (int)qtd;
            else
                material.Qtde = material.QtdModelo * (int)qtd;
        }

        #endregion

        #region Interpretador de expressão matemática

        /// <summary>
        /// Retorna o resultado da expressão passada para peças de vidro
        /// </summary>
        /// <param name="expressao"></param>
        /// <param name="itemProj"></param>
        /// <param name="lstPeca"></param>
        /// <returns></returns>
        public static float CalcExpressao(string expressao, ItemProjeto itemProj, List<PecaItemProjeto> lstPeca)
        {
            return CalcExpressao(null, expressao, itemProj, lstPeca);
        }

        /// <summary>
        /// Retorna o resultado da expressão passada para peças de vidro
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="expressao"></param>
        /// <param name="itemProj"></param>
        /// <param name="lstPeca"></param>
        /// <returns></returns>
        public static float CalcExpressao(GDA.GDASession sessao, string expressao, ItemProjeto itemProj, List<PecaItemProjeto> lstPeca)
        {
            return CalcExpressao(sessao, expressao, itemProj, lstPeca, MedidaProjetoModeloDAO.Instance.GetByProjetoModelo(itemProj.IdProjetoModelo, true));
        }

        /// <summary>
        /// Retorna o resultado da expressão passada para peças de vidro
        /// </summary>
        /// <param name="expressao"></param>
        /// <param name="itemProj"></param>
        /// <param name="lstPeca"></param>
        /// <param name="lstMedida"></param>
        /// <returns></returns>
        public static float CalcExpressao(string expressao, ItemProjeto itemProj, List<PecaItemProjeto> lstPeca, List<MedidaProjetoModelo> lstMedida)
        {
            return CalcExpressao(null, expressao, itemProj, lstPeca, lstMedida);
        }

        /// <summary>
        /// Retorna o resultado da expressão passada para peças de vidro
        /// </summary>
        public static float CalcExpressao(GDA.GDASession sessao, string expressao, ItemProjeto itemProj, List<PecaItemProjeto> lstPeca,
            List<MedidaProjetoModelo> lstMedida)
        {
            return CalcExpressao(sessao, expressao, itemProj, lstPeca, lstMedida, null);
        }

        /// <summary>
        /// Retorna o resultado da expressão passada para peças de vidro
        /// </summary>
        /// <param name="expressao"></param>
        /// <param name="itemProj"></param>
        /// <param name="lstPeca"></param>
        /// <param name="lstMedida"></param>
        /// <param name="numEtiqueta"></param>
        /// <returns></returns>
        public static float CalcExpressao(string expressao, ItemProjeto itemProj, List<PecaItemProjeto> lstPeca, List<MedidaProjetoModelo> lstMedida,
            string numEtiqueta)
        {
            return CalcExpressao(null, expressao, itemProj, lstPeca, lstMedida, numEtiqueta);
        }

        /// <summary>
        /// Retorna o resultado da expressão passada para peças de vidro
        /// </summary>
        public static float CalcExpressao(GDA.GDASession sessao, string expressao, ItemProjeto itemProj, List<PecaItemProjeto> lstPeca,
            List<MedidaProjetoModelo> lstMedida, string numEtiqueta)
        {
            lock (_calcularExpressaoLock)
            {
                if (String.IsNullOrEmpty(expressao))
                    throw new Exception("Expressão de cálculo não informada.");

                string exprOrig = expressao;

                try
                {
                    // Substitui a descrição da formula pela sua expressão.
                    var listaFormulas = FormulaExpressaoCalculoDAO.Instance.GetAll();
                    for (int i = 0; i < listaFormulas.Length; i++)
                        if (listaFormulas[i].Expressao != null && listaFormulas[i].Expressao != "")
                            if (expressao.Contains(listaFormulas[i].Descricao))
                            {
                                expressao = expressao.Replace(listaFormulas[i].Descricao, listaFormulas[i].Expressao);
                                i = -1;
                            }

                    // Substitui as variáveis de medidas pelos seus respectivos valores
                    foreach (MedidaProjetoModelo mpm in lstMedida)
                        if (expressao.Contains(mpm.CalcTipoMedida))
                            expressao = expressao.Replace(mpm.CalcTipoMedida, MedidaItemProjetoDAO.Instance.GetByItemProjeto(sessao, itemProj.IdItemProjeto, mpm.IdMedidaProjeto, false).ToString());

                    // Substitui os campos de altura e largura da peça que possam ter sido usados na expressão de cálculo
                    if (lstPeca != null)
                        foreach (PecaItemProjeto pip in lstPeca)
                        {
                            expressao = expressao.Replace("P" + pip.Item.ToUpper().Replace(" ", "") + "ALT", pip.Altura.ToString());
                            expressao = expressao.Replace("P" + pip.Item.ToUpper().Replace(" ", "") + "LARG", pip.Largura.ToString());

                            if (pip.IdProd > 0)
                                expressao = expressao.Replace("P" + pip.Item.ToUpper().Replace(" ", "") + "ESP", ProdutoDAO.Instance.ObtemEspessura(sessao, (int)pip.IdProd.Value).ToString());

                            if (expressao.Contains("FOLGA"))
                            {
                                var pecaProjetoModelo = PecaProjetoModeloDAO.Instance.GetByCliente(sessao, pip.IdPecaProjMod, itemProj.IdCliente.GetValueOrDefault());

                                var folgaAltura =
                                    itemProj.MedidaExata ?
                                        0 : (ProjetoConfig.SelecionarEspessuraAoCalcularProjeto ?
                                            (itemProj.EspessuraVidro == 3 ? pecaProjetoModelo.Altura03MM :
                                            itemProj.EspessuraVidro == 4 ? pecaProjetoModelo.Altura04MM :
                                            itemProj.EspessuraVidro == 5 ? pecaProjetoModelo.Altura05MM :
                                            itemProj.EspessuraVidro == 6 ? pecaProjetoModelo.Altura06MM :
                                            itemProj.EspessuraVidro == 8 ? pecaProjetoModelo.Altura08MM :
                                            itemProj.EspessuraVidro == 10 ? pecaProjetoModelo.Altura10MM :
                                            itemProj.EspessuraVidro == 12 ? pecaProjetoModelo.Altura12MM : 0) : pecaProjetoModelo.Altura);

                                var folgaLargura =
                                    itemProj.MedidaExata ?
                                        0 : (ProjetoConfig.SelecionarEspessuraAoCalcularProjeto ?
                                            (itemProj.EspessuraVidro == 3 ? pecaProjetoModelo.Largura03MM :
                                            itemProj.EspessuraVidro == 4 ? pecaProjetoModelo.Largura04MM :
                                            itemProj.EspessuraVidro == 5 ? pecaProjetoModelo.Largura05MM :
                                            itemProj.EspessuraVidro == 6 ? pecaProjetoModelo.Largura06MM :
                                            itemProj.EspessuraVidro == 8 ? pecaProjetoModelo.Largura08MM :
                                            itemProj.EspessuraVidro == 10 ? pecaProjetoModelo.Largura10MM :
                                            itemProj.EspessuraVidro == 12 ? pecaProjetoModelo.Largura12MM : 0) : pecaProjetoModelo.Largura);

                                expressao = expressao.Replace("FOLGA" + pip.Item.ToUpper().Replace(" ", "") + "ALT", folgaAltura.ToString());
                                expressao = expressao.Replace("FOLGA" + pip.Item.ToUpper().Replace(" ", "") + "LARG", folgaLargura.ToString());
                                expressao = expressao.Replace("--", "+").Replace("+-", "-").Replace("-+", "-");
                            }
                        }

                    //Substitui o campo Item da Etiqueta
                    if (expressao.Contains("IETQ") && !string.IsNullOrEmpty(numEtiqueta))
                    {
                        string itemEtiqueta = numEtiqueta.Split('-', '.')[2].Split('/')[0];
                        expressao = expressao.Replace("IETQ", itemEtiqueta);
                    }

                    // Remove espaços em branco
                    while (expressao.Contains(" "))
                        expressao = expressao.Replace(" ", "");

                    return CalcularExpressao(expressao);
                }
                catch (Exception ex)
                {
                    throw new Exception(MensagemAlerta.FormatErrorMsg("Expressão de cálculo inválida (" + exprOrig + ", " + expressao + "). Projeto Modelo: " + itemProj.IdProjetoModelo +
                        (itemProj.IdOrcamento != null ? "Id do Orçamento: " + itemProj.IdOrcamento :
                         itemProj.IdPedido != null ? "Id do Pedido: " + itemProj.IdPedido :
                         itemProj.IdPedidoEspelho != null ? "Id do Pedido Espelho: " + itemProj.IdPedidoEspelho :
                         "Id do ItemProjeto: " + itemProj.IdItemProjeto), ex));
                }
            }
        }

        /// <summary>
        /// Calcula a expressão informada por parâmetro, retorna o valor final.
        /// </summary>
        private static float CalcularExpressao(string expressao)
        {
            // IMPORTANTE: NÃO REMOVER ESTE CONTADOR PARA IMPEDIR LOOP INFINITO NOS CÁLCULOS DE PROJETO.
            // Contador usado para sair do while (sinal de -)
            var cont = 0;

            // Contador usado para sair do while (parênteses)
            var contWhilePar = 0;

            var resultadoExpressao = 0F;

            // Resolve as expressões dentro dos parênteses primeiro
            while (expressao.Contains("(") || expressao.Contains(")"))
            {
                int innerExprIniPos = expressao.LastIndexOf('(');
                int innerExprFinPos = expressao.IndexOf(')', innerExprIniPos);

                // Pega a expressão dentro dos parênteses mais interno
                string exprTemp = expressao.Substring(innerExprIniPos, innerExprFinPos - innerExprIniPos + 1);

                /* Chamado 51770.
                 * A expressão de cálculo do IF é convertida na region Cálculo IF. */
                if (!exprTemp.Contains("<") && !exprTemp.Contains(">") && !exprTemp.Contains("="))
                {
                    while (exprTemp.Contains("*"))
                        exprTemp = CalcValor(exprTemp, "*");

                    while (exprTemp.Contains("/"))
                        exprTemp = CalcValor(exprTemp, "/");

                    while (exprTemp.Contains("+"))
                        exprTemp = CalcValor(exprTemp, "+");

                    /* Chamado 52842. */
                    while (exprTemp.Contains("-") && cont <= 100)
                    {
                        exprTemp = CalcValor(exprTemp, "-");
                        cont++;
                    }
                }

                #region Cálculo raiz quadrada

                // Verifica se é para calcular raiz
                if (innerExprIniPos >= 4 && expressao.Substring(innerExprIniPos - 4, 4).ToUpper() == "RAIZ")
                {
                    // Calcula raiz quadrada
                    if (!float.TryParse(exprTemp.Replace(".", ",").Replace("(", "").Replace(")", ""), out resultadoExpressao))
                        throw new Exception(string.Format("Falha ao converter a expressão {0} para valor decimal.", exprTemp.Replace(".", ",")));

                    exprTemp = (Math.Sqrt(resultadoExpressao)).ToString();

                    // Remove a palavra RAIZ
                    expressao = expressao.Remove(innerExprIniPos - 4, 4);

                    // Retira as posições que a raiz acrescentou
                    innerExprIniPos -= 4;
                    innerExprFinPos -= 4;
                }

                #endregion

                #region Funções trigonométricas

                // A ordem de cálculo das funções trigonométricas abaixo deve ser exatamente essa para evitar problemas.
                // Por exemplo, caso seno esteja antes de cosseno, o cosseno irá calcular na condição do seno e o valor ficará incorreto.

                #region Cálculo cossecante

                // Verifica se é para calcular cossecante.
                if (innerExprIniPos >= 10 && expressao.Substring(innerExprIniPos - 10, 10).ToUpper() == "COSSECANTE")
                {
                    // Verifica se é um valor válido.
                    if (!float.TryParse(exprTemp.Replace(".", ",").Replace("(", "").Replace(")", ""), out resultadoExpressao))
                        throw new Exception(string.Format("Falha ao converter a expressão {0} para valor decimal.", exprTemp.Replace(".", ",")));

                    // Calcula o cossecante do valor informado.
                    exprTemp = Math.Acos(resultadoExpressao).ToString();

                    // Remove a palavra COSSECANTE.
                    expressao = expressao.Remove(innerExprIniPos - 10, 10);

                    // Retira as posições que a função acrescentou.
                    innerExprIniPos -= 10;
                    innerExprFinPos -= 10;
                }

                #endregion

                #region Cálculo cosseno

                // Verifica se é para calcular cosseno.
                if (innerExprIniPos >= 7 && expressao.Substring(innerExprIniPos - 7, 7).ToUpper() == "COSSENO")
                {
                    // Verifica se é um valor válido.
                    if (!float.TryParse(exprTemp.Replace(".", ",").Replace("(", "").Replace(")", ""), out resultadoExpressao))
                        throw new Exception(string.Format("Falha ao converter a expressão {0} para valor decimal.", exprTemp.Replace(".", ",")));

                    // Calcula o cosseno do valor informado.
                    exprTemp = Math.Cos(resultadoExpressao).ToString();

                    // Remove a palavra COSSENO.
                    expressao = expressao.Remove(innerExprIniPos - 7, 7);

                    // Retira as posições que a função acrescentou.
                    innerExprIniPos -= 7;
                    innerExprFinPos -= 7;
                }

                #endregion

                #region Cálculo secante

                // Verifica se é para calcular secante.
                if (innerExprIniPos >= 7 && expressao.Substring(innerExprIniPos - 7, 7).ToUpper() == "SECANTE")
                {
                    // Verifica se é um valor válido.
                    if (!float.TryParse(exprTemp.Replace(".", ",").Replace("(", "").Replace(")", ""), out resultadoExpressao))
                        throw new Exception(string.Format("Falha ao converter a expressão {0} para valor decimal.", exprTemp.Replace(".", ",")));

                    // Calcula o secante do valor informado.
                    exprTemp = Math.Asin(resultadoExpressao).ToString();

                    // Remove a palavra SECANTE.
                    expressao = expressao.Remove(innerExprIniPos - 7, 7);

                    // Retira as posições que a função acrescentou.
                    innerExprIniPos -= 7;
                    innerExprFinPos -= 7;
                }

                #endregion

                #region Cálculo seno

                // Verifica se é para calcular seno.
                if (innerExprIniPos >= 4 && expressao.Substring(innerExprIniPos - 4, 4).ToUpper() == "SENO")
                {
                    // Verifica se é um valor válido.
                    if (!float.TryParse(exprTemp.Replace(".", ",").Replace("(", "").Replace(")", ""), out resultadoExpressao))
                        throw new Exception(string.Format("Falha ao converter a expressão {0} para valor decimal.", exprTemp.Replace(".", ",")));

                    // Calcula o seno do valor informado.
                    exprTemp = Math.Sin(resultadoExpressao).ToString();

                    // Remove a palavra SENO.
                    expressao = expressao.Remove(innerExprIniPos - 4, 4);

                    // Retira as posições que a função acrescentou.
                    innerExprIniPos -= 4;
                    innerExprFinPos -= 4;
                }

                #endregion

                #region Cálculo cotangente

                // Verifica se é para calcular cotangente.
                if (innerExprIniPos >= 10 && expressao.Substring(innerExprIniPos - 10, 10).ToUpper() == "COTANGENTE")
                {
                    // Verifica se é um valor válido.
                    if (!float.TryParse(exprTemp.Replace(".", ",").Replace("(", "").Replace(")", ""), out resultadoExpressao))
                        throw new Exception(string.Format("Falha ao converter a expressão {0} para valor decimal.", exprTemp.Replace(".", ",")));

                    // Calcula a cotangente do valor informado.
                    exprTemp = Math.Atan(resultadoExpressao).ToString();

                    // Remove a palavra COTANGENTE.
                    expressao = expressao.Remove(innerExprIniPos - 10, 10);

                    // Retira as posições que a função acrescentou.
                    innerExprIniPos -= 10;
                    innerExprFinPos -= 10;
                }

                #endregion

                #region Cálculo tangente

                // Verifica se é para calcular tangente.
                if (innerExprIniPos >= 8 && expressao.Substring(innerExprIniPos - 8, 8).ToUpper() == "TANGENTE")
                {
                    // Verifica se é um valor válido.
                    if (!float.TryParse(exprTemp.Replace(".", ",").Replace("(", "").Replace(")", ""), out resultadoExpressao))
                        throw new Exception(string.Format("Falha ao converter a expressão {0} para valor decimal.", exprTemp.Replace(".", ",")));

                    // Calcula a tangente do valor informado.
                    exprTemp = Math.Tan(resultadoExpressao).ToString();

                    // Remove a palavra TANGENTE.
                    expressao = expressao.Remove(innerExprIniPos - 8, 8);

                    // Retira as posições que a função acrescentou.
                    innerExprIniPos -= 8;
                    innerExprFinPos -= 8;
                }

                #endregion

                #endregion

                #region Cálculo arredondamento para cima

                // Verifica se é para arredondar para cima
                if (innerExprIniPos >= 7 && expressao.Substring(innerExprIniPos - 7, 7).ToUpper() == "CEILING")
                {
                    // Arredonda número para cima
                    if (!float.TryParse(exprTemp.Replace(".", ",").Replace("(", "").Replace(")", ""), out resultadoExpressao))
                        throw new Exception(string.Format("Falha ao converter a expressão {0} para valor decimal.", exprTemp.Replace(".", ",")));

                    exprTemp = ((int)Math.Ceiling(resultadoExpressao)).ToString();

                    // Remove a palavra CEILING
                    expressao = expressao.Remove(innerExprIniPos - 7, 7);

                    // Retira as posições que a função acrescentou
                    innerExprIniPos -= 7;
                    innerExprFinPos -= 7;
                }

                #endregion

                #region Cálculo arredondamento para baixo

                // Verifica se é para arredondar para baixo
                if (innerExprIniPos >= 5 && expressao.Substring(innerExprIniPos - 5, 5).ToUpper() == "FLOOR")
                {
                    // Arredonda número para baixo
                    if (!float.TryParse(exprTemp.Replace(".", ",").Replace("(", "").Replace(")", ""), out resultadoExpressao))
                        throw new Exception(string.Format("Falha ao converter a expressão {0} para valor decimal.", exprTemp.Replace(".", ",")));

                    exprTemp = ((int)Math.Floor(resultadoExpressao)).ToString();

                    // Remove a palavra FLOOR
                    expressao = expressao.Remove(innerExprIniPos - 5, 5);

                    // Retira as posições que a função acrescentou
                    innerExprIniPos -= 5;
                    innerExprFinPos -= 5;
                }

                #endregion
                
                #region Cálculo valor múltiplo

                // Verifica se é para calcular múltiplo
                if (innerExprIniPos >= 4 && expressao.Substring(innerExprIniPos - 4, 4).ToUpper() == "MULT")
                {
                    if (!float.TryParse(exprTemp.Split(';')[0].Replace("(", ""), out resultadoExpressao))
                        throw new Exception(string.Format("Falha ao converter a expressão {0} para valor decimal.", exprTemp.Split(';')[0].Replace("(", "")));

                    int valorBase = (int)Math.Ceiling(resultadoExpressao);
                    int multiplo = exprTemp.Split(';')[1].Replace(")", "").StrParaInt();

                    // Calcula múltiplo
                    while (valorBase % multiplo > 0)
                        valorBase++;

                    exprTemp = valorBase.ToString();

                    // Remove o parâmetro da função MULT
                    int indiceInicioParam = expressao.IndexOf(';', expressao.LastIndexOf('('));
                    expressao = expressao.Remove(indiceInicioParam, innerExprFinPos - indiceInicioParam);

                    // Remove a palavra MULT
                    expressao = expressao.Remove(innerExprIniPos - 4, 4);

                    // Retira as posições que o múltiplo acrescentou
                    innerExprIniPos -= 4;
                    innerExprFinPos -= 4 + (innerExprFinPos - indiceInicioParam);
                }

                #endregion

                #region Cálculo IF

                // Verifica se é para resolver IF.
                if (innerExprIniPos >= 2 && expressao.Substring(innerExprIniPos - 2, 2).ToUpper() == "IF")
                {
                    #region Variáveis

                    // Recupera as condições informadas no IF.
                    var condicoes = exprTemp.Split(';')[0].Replace("(", "");
                    // Variável usada para substituir o resultado do AND ou do OR por uma condição verdadeira,
                    // e prosseguir na interpretação dos demais AND's e OR's.
                    var condicaoVerdadeira = "1=1";
                    // Variável usada para substituir o resultado do AND ou do OR por uma condição falsa.
                    // e prosseguir na interpretação dos demais AND's e OR's.
                    var condicaoFalsa = "0=1";

                    #endregion

                    #region Validação

                    // Esta verificação foi inserida para evitar que a condição do IF possua parêntesis entre as operações.
                    // Para aceitar parêntesis é necessário alterar a lógica de interpretação para ser feita semelhante ao interpretador de expressão geral.
                    if (condicoes.Contains(")"))
                        throw new Exception("Não é possível inserir parêntesis em uma condição da função IF.");

                    #endregion

                    #region Interpretador das condições da função IF

                    do
                    {
                        #region Operador OR

                        // Caso exista o operador OR e ele apareça antes do operador AND ou não exista o operador AND, interpreta a condição.
                        if (condicoes.IndexOf("OR") >= 0 && (condicoes.IndexOf("OR") < condicoes.IndexOf("AND") || condicoes.IndexOf("AND") < 0))
                        {
                            // Recupera o texto à esquerda do OR.
                            var primeiraCondicao = condicoes.Split(new[] { "OR" }, StringSplitOptions.None)[0];
                            // Recupera o texto à direita do OR.
                            var segundaCondicao = condicoes.Split(new[] { "OR" }, StringSplitOptions.None)[1];

                            // Remove o operador AND à direita do operador OR recuperado.
                            if (segundaCondicao.IndexOf("AND") > 0)
                                segundaCondicao = segundaCondicao.Substring(0, segundaCondicao.IndexOf("AND"));

                            // Remove o operador OR à direita do operador OR recuperado.
                            if (segundaCondicao.IndexOf("OR") > 0)
                                segundaCondicao = segundaCondicao.Substring(0, segundaCondicao.IndexOf("OR"));

                            // Substitui o operador OR pelo padrão de condição verdadeira (1=1) ou falsa (0=1) para que a próxima condição possa ser interpretada.
                            // Observe que é utilizado o operador || entre os resultados de cada lado.
                            condicoes = (ResultadoComparacao(primeiraCondicao) || ResultadoComparacao(segundaCondicao) ? condicaoVerdadeira : condicaoFalsa) +
                                // Recupera a condição que ainda não foi interpretada. Obs.: +2 é referente ao texto "OR".
                                condicoes.Substring(primeiraCondicao.Length + 2 + segundaCondicao.Length);
                        }

                        #endregion

                        #region Operador AND

                        // Caso exista o operador AND, ela apareça antes do operador OR ou não exista o operador OR, interpreta a condição.
                        else if (condicoes.IndexOf("AND") >= 0 && (condicoes.IndexOf("AND") < condicoes.IndexOf("OR") || condicoes.IndexOf("OR") < 0))
                        {
                            // Recupera o texto à esquerda do AND.
                            var primeiraCondicao = condicoes.Split(new[] { "AND" }, StringSplitOptions.None)[0];
                            // Recupera o texto à direita do AND.
                            var segundaCondicao = condicoes.Split(new[] { "AND" }, StringSplitOptions.None)[1];

                            // Remove o operador AND à direita do operador AND recuperada.
                            if (segundaCondicao.IndexOf("AND") > 0)
                                segundaCondicao = segundaCondicao.Substring(0, segundaCondicao.IndexOf("AND"));

                            // Remove o operador OR à direita do operador AND recuperada.
                            if (segundaCondicao.IndexOf("OR") > 0)
                                segundaCondicao = segundaCondicao.Substring(0, segundaCondicao.IndexOf("OR"));

                            // Substitui o operador AND pelo padrão de condição verdadeira (1=1) ou falsa (0=1) para que a próxima condição possa ser interpretada.
                            // Observe que é utilizado o operador && entre os resultados de cada lado.
                            condicoes = (ResultadoComparacao(primeiraCondicao) && ResultadoComparacao(segundaCondicao) ? condicaoVerdadeira : condicaoFalsa) +
                                // Recupera o operador que ainda não foi interpretado. Obs.: +3 é referente ao texto "AND".
                                condicoes.Substring(primeiraCondicao.Length + 3 + segundaCondicao.Length);
                        }

                        #endregion

                        #region IF sem operador

                        // Caso o IF não tenha sido criado com os operadores AND ou OR, interpreta a condição normalmente.
                        else if (condicoes != condicaoVerdadeira && condicoes != condicaoFalsa)
                            // Substitui o resultado pelo texto de condição verdadeira ou pelo texto de condição falsa.
                            condicoes = ResultadoComparacao(condicoes) ? condicaoVerdadeira : condicaoFalsa;

                        #endregion

                    // A repetição é feita até que não sobrem AND's ou OR's.
                    } while (condicoes.Contains("AND") || condicoes.Contains("OR"));

                    #endregion

                    #region Recuperação do resultado

                    // Caso a condição seja verdadeira, recupera o segundo campo do IF.
                    var resultado = condicoes == condicaoVerdadeira ? exprTemp.Split(';')[1] :
                        // Caso a condição seja falsa, recupera o terceiro campo do IF.
                        condicoes == condicaoFalsa ? exprTemp.Split(';')[2].Replace(")", "") :
                        // Caso a condição não seja verdadeira ou falsa, ocorreu algum problema ao interpretar o IF.
                        "Erro";

                    // Lança uma exceção informando que o IF não pode ser interpretado.
                    if (resultado == "Erro")
                        throw new Exception("Não foi possível validar a função IF da expressão criada.");

                    // Salva o resultado do IF na expressão temporária.
                    exprTemp = resultado;

                    // Retira as posições que a condição do IF ocupava.
                    innerExprIniPos -= 2;

                    #endregion
                }

                #endregion

                #region Cálculo REIKI

                // Verifica se é para função REIKI
                //REIKI(LARGPORTA;GIRATORIO;FOLGAGIRATORIO;FOLGAPRIPC;FOLGAPC;MAXRASGO;IETQ)
                if (innerExprIniPos >= 5 && expressao.Substring(innerExprIniPos - 5, 5).ToUpper() == "REIKI")
                {
                    string[] valores = exprTemp.Replace("(", "").Replace(")", "").Split(';');

                    int largPorta = valores[0].StrParaInt();
                    int giratorio = valores[1].StrParaInt();
                    int folgaGiratorio = valores[2].StrParaInt();
                    int folgaPriPeca = valores[3].StrParaInt();
                    int folgaPeca = valores[4].StrParaInt();
                    float maxRasgo = valores[5].StrParaFloat();
                    int itemEtq = valores[6].StrParaInt();

                    float dist = largPorta - (giratorio + folgaGiratorio);

                    for (int i = 1; i < itemEtq; i++)
                    {
                        dist -= folgaPeca;

                        if (dist <= maxRasgo)
                            dist = largPorta - folgaPriPeca;
                    }

                    exprTemp = dist.ToString();

                    // Retira as posições que o múltiplo acrescentou
                    innerExprIniPos -= 5;
                }

                #endregion

                // Refaz a variável expressão buscando apenas o resultado da expressão calculada dentro dos parênteses
                expressao =
                    expressao.Substring(0, innerExprIniPos) +
                    exprTemp +
                    expressao.Substring(expressao.Substring(0, innerExprFinPos).Length + 1);

                /* Chamado 41325. */
                //if (contWhilePar >= 8 && !exprTemp.Contains("*") && !exprTemp.Contains("/") && !exprTemp.Contains("+"))
                if (contWhilePar >= 128 && !exprTemp.Contains("*") && !exprTemp.Contains("/") && !exprTemp.Contains("+"))
                {
                    expressao = expressao.Replace("(", "").Replace(")", "");
                    break;
                }

                contWhilePar++;
            }
            
            while (expressao.Contains("*"))
                expressao = CalcValor(expressao, "*");

            while (expressao.Contains("/"))
                expressao = CalcValor(expressao, "/");

            while (expressao.Contains("+"))
                expressao = CalcValor(expressao, "+");
            
            /* Chamado 52842. */
            while (expressao.Contains("-") && cont <= 100)
            {
                expressao = CalcValor(expressao, "-");
                cont++;
            }

            if (!float.TryParse(expressao.Replace(".", ","), out resultadoExpressao))
                throw new Exception(string.Format("Falha ao converter a expressão {0} para valor decimal.", expressao.Replace(".", ",")));

            return resultadoExpressao;
        }

        /// <summary>
        /// Retorna o resultado booleano entre a comparação de um valor com outro. Usado na função IF.
        /// </summary>
        private static bool ResultadoComparacao(string condicao)
        {
            // Recupera o valor à esquerda da comparação.
            var primeiroCampo = Convert.ToInt32(condicao.Split(new[] { ">=", "<=", "==", ">", "<", "=" }, StringSplitOptions.None)[0].StrParaFloat());
            // Recupera o valor à direita da comparação.
            var segundoCampo = Convert.ToInt32(condicao.Split(new[] { ">=", "<=", "==", ">", "<", "=" }, StringSplitOptions.None)[1].StrParaFloat());
            // Variável criada para salvar o resultado da comparação.
            var resultadoComparacao = false;

            #region Comparações
            
            if (condicao.Contains(">="))
                resultadoComparacao = primeiroCampo >= segundoCampo;
            else if (condicao.Contains(">"))
                resultadoComparacao = primeiroCampo > segundoCampo;
            else if (condicao.Contains("<="))
                resultadoComparacao = primeiroCampo <= segundoCampo;
            else if (condicao.Contains("<"))
                resultadoComparacao = primeiroCampo < segundoCampo;
            else if (condicao.Contains("=") || condicao.Contains("=="))
                resultadoComparacao = primeiroCampo == segundoCampo;

            #endregion

            return resultadoComparacao;
        }

        /// <summary>
        /// Pega os dois números próximo ao sinal passado e retorna o resultado
        /// </summary>
        private static string CalcValor(string expr, string sinal)
        {
            // Verificar se o primeiro valor da expressão é negativo.
            var primeiroValorNegativo = expr.Contains("(-") || expr.IndexOf("-") == 0;
            // Verificar se o segundo valor da expressão é negativo.
            var segundoValorNegativo = expr.Contains("*-") || expr.Contains("/-") || expr.Contains("+-") || expr.Contains("--");

            // Caso o primeiro valor seja negativo, remove o sinal de menos.
            if (primeiroValorNegativo)
            {
                expr = expr.Replace("(-", "(");

                if (expr.IndexOf("-") == 0)
                    expr = expr.Substring(1);
            }

            // Caso o segundo valor seja negativo, remove o sinal de menos.
            if (segundoValorNegativo)
                expr = expr.Replace("*-", "*").Replace("/-", "/").Replace("+-", "+").Replace("--", "-");

            var indexOfSign = expr.LastIndexOf(sinal, StringComparison.Ordinal);
            var tamanhoPrimeiroValor = 0;
            var tamanhoSegundoValor = 0;
            var resultadoExpressao = 0F;
            int posTemp;

            // Pega o tamanho do número à esquerda do sinal
            while (true)
            {
                posTemp = indexOfSign - tamanhoPrimeiroValor - 1;

                if (posTemp > -1 && "0123456789,.".Contains(expr[posTemp].ToString()))
                    tamanhoPrimeiroValor++;
                else
                    break;
            }

            // Se não houver número à esquerda, retorna expressao
            if (tamanhoPrimeiroValor == 0 && sinal == "-")
                return expr;

            // Pega o tamanho do número à direita do sinal
            while (true)
            {
                posTemp = indexOfSign + tamanhoSegundoValor + 1;

                if (posTemp < expr.Length && "0123456789,.".Contains(expr[posTemp].ToString()))
                    tamanhoSegundoValor++;
                else
                    break;
            }

            // Verifica se o primeiro valor é um valor válido.
            if (!float.TryParse(expr.Substring(indexOfSign - tamanhoPrimeiroValor, tamanhoPrimeiroValor).Replace(".", ","), out resultadoExpressao))
                throw new Exception(string.Format("Falha ao converter a expressão {0} para valor decimal.", expr.Substring(indexOfSign - tamanhoPrimeiroValor, tamanhoPrimeiroValor).Replace(".", ",")));

            // Caso o primeiro valor seja um valor válido, essa lógica verifica se ele deve ser negativo e o negativa se necessário.
            var primeiroValor = resultadoExpressao * (primeiroValorNegativo ? -1 : 1);

            // Verifica se o segundo valor é um valor válido.
            if (!float.TryParse(expr.Substring(indexOfSign + 1, tamanhoSegundoValor).Replace(".", ","), out resultadoExpressao))
                throw new Exception(string.Format("Falha ao converter a expressão {0} para valor decimal.", expr.Substring(indexOfSign + 1, tamanhoSegundoValor).Replace(".", ",")));

            // Caso o segundo valor seja um valor válido, essa lógica verifica se ele deve ser negativo e o negativa se necessário.
            var segundoValor = resultadoExpressao * (segundoValorNegativo ? -1 : 1);

            double resultado;

            // Faz o cálculo
            switch (sinal)
            {
                case "*":
                    resultado = (float)Math.Round(primeiroValor * segundoValor, 2);
                    break;
                case "/":
                    resultado = (float)Math.Round(primeiroValor / segundoValor, 2);
                    break;
                case "+":
                    resultado = primeiroValor + segundoValor;
                    break;
                case "-":
                    resultado = primeiroValor - segundoValor;
                    break;
                default:
                    throw new Exception("Sinal inexistente.");
            }

            // Pega a posição inicial da expressao calculada
            int posIniExpr = indexOfSign - tamanhoPrimeiroValor;
            expr = expr.Remove(posIniExpr, tamanhoPrimeiroValor + tamanhoSegundoValor + 1);
            expr = expr.Insert(posIniExpr, resultado.ToString());

            return expr.Replace("(", "").Replace(")", "").Replace("+-", "-").Replace("-+", "-").Replace("--", "-");
        }

        #endregion

        #region Validador de expressão matemática
        
        /// <summary>
        /// Valida expressão matemática.
        /// </summary>
        public static bool ValidarExpressao(int idProjetoModelo, string expressao)
        {
            lock (_validarExpressaoLock)
            {
                if (string.IsNullOrEmpty(expressao))
                    throw new Exception("Expressão de cálculo não informada.");

                try
                {
                    // Substitui a descrição da formula pela sua expressão.
                    var listaFormulas = FormulaExpressaoCalculoDAO.Instance.GetAll();
                    for (int i = 0; i < listaFormulas.Length; i++)
                        if (listaFormulas[i].Expressao != null && listaFormulas[i].Expressao != "")
                            if (expressao.Contains(listaFormulas[i].Descricao))
                            {
                                expressao = expressao.Replace(listaFormulas[i].Descricao, listaFormulas[i].Expressao);
                                i = -1;
                            }

                    if (idProjetoModelo > 0)
                    {
                        var medidas = MedidaProjetoModeloDAO.Instance.GetByProjetoModelo((uint)idProjetoModelo, false);
                        var valorRandomico = new Random();

                        // Substitui as variáveis de medidas pelos seus respectivos valores
                        foreach (MedidaProjetoModelo mpm in medidas)
                            if (expressao.Contains(mpm.CalcTipoMedida))
                                expressao = expressao.Replace(mpm.CalcTipoMedida, Math.Round((valorRandomico.NextDouble() * 100), 2).ToString().Replace(",", ".").ToString());

                        // Substitui os campos de altura e largura da peça que possam ter sido usados na expressão de cálculo
                        foreach (var ppm in PecaProjetoModeloDAO.Instance.GetByModelo((uint)idProjetoModelo))
                        {
                            expressao = expressao.Replace("P" + ppm.Item.ToUpper().Replace(" ", "") + "ALT", Math.Round((valorRandomico.NextDouble() * 1000), 2).ToString().Replace(",", "."));
                            expressao = expressao.Replace("P" + ppm.Item.ToUpper().Replace(" ", "") + "LARG", Math.Round((valorRandomico.NextDouble() * 1000), 2).ToString().Replace(",", "."));

                            expressao = expressao.Replace("P" + ppm.Item.ToUpper().Replace(" ", "") + "ESP", valorRandomico.Next(20).ToString());

                            if (expressao.Contains("FOLGA"))
                            {
                                expressao = expressao.Replace("FOLGA" + ppm.Item.ToUpper().Replace(" ", "") + "ALT", valorRandomico.Next(-10, 10).ToString());
                                expressao = expressao.Replace("FOLGA" + ppm.Item.ToUpper().Replace(" ", "") + "LARG", valorRandomico.Next(-10, 10).ToString());
                                expressao = expressao.Replace("--", "+").Replace("+-", "-").Replace("-+", "-");
                            }
                        }
                    }
                    else
                    {
                        var medidas = MedidaProjetoDAO.Instance.GetMedidas();
                        var valorRandomico = new Random();

                        // Substitui as variáveis de medidas pelos seus respectivos valores
                        foreach (MedidaProjeto mp in medidas)
                            if (expressao.Contains(mp.DescricaoTratada.ToUpper()))
                                expressao = expressao.Replace(mp.DescricaoTratada.ToUpper(), Math.Round((valorRandomico.NextDouble() * 100), 2).ToString().Replace(",", ".").ToString());

                        // Substitui os campos de altura e largura da peça que possam ter sido usados na expressão de cálculo
                        foreach (var item in PecaProjetoModeloDAO.Instance.GetDistinctItemPecaProjetoModelo())
                        {
                            expressao = expressao.Replace("P" + item.ToUpper().Replace(" ", "") + "ALT", Math.Round((valorRandomico.NextDouble() * 1000), 2).ToString().Replace(",", "."));
                            expressao = expressao.Replace("P" + item.ToUpper().Replace(" ", "") + "LARG", Math.Round((valorRandomico.NextDouble() * 1000), 2).ToString().Replace(",", "."));

                            expressao = expressao.Replace("P" + item.ToUpper().Replace(" ", "") + "ESP", valorRandomico.Next(20).ToString());

                            if (expressao.Contains("FOLGA"))
                            {
                                expressao = expressao.Replace("FOLGA" + item.ToUpper().Replace(" ", "") + "ALT", valorRandomico.Next(-10, 10).ToString());
                                expressao = expressao.Replace("FOLGA" + item.ToUpper().Replace(" ", "") + "LARG", valorRandomico.Next(-10, 10).ToString());
                                expressao = expressao.Replace("--", "+").Replace("+-", "-").Replace("-+", "-");
                            }
                        }
                    }

                    //Substitui o campo Item da Etiqueta
                    if (expressao.Contains("IETQ"))
                        expressao = expressao.Replace("IETQ", "1");

                    // Remove espaços em branco
                    while (expressao.Contains(" "))
                        expressao = expressao.Replace(" ", "");

                    if (expressao.Contains(";)"))
                        throw new Exception("Todas as condições devem retornar valor.");

                    var valorExpressao = CalcularExpressao(expressao);
                    
                    return true;
                }
                catch (Exception ex)
                {
                    ErroDAO.Instance.InserirFromException(string.Format("ValidarExpressao: {0}", expressao), ex);
                    throw ex;
                }
            }
        }

        #endregion

        #region Retorna URL com o endereço da "figura associada"

        /// <summary>
        /// Retorna os dados a serem passados para o handler, para mostrar a figura associada com a medida correspondente desenhada
        /// </summary>
        /// <param name="idItemProjeto"></param>
        /// <returns></returns>
        public static string GetFiguraAssociadaUrl(uint idItemProjeto)
        {
            return GetFiguraAssociadaUrl(idItemProjeto, ProjetoModeloDAO.Instance.GetElementByPrimaryKey(ItemProjetoDAO.Instance.ObtemIdProjetoModelo(null, idItemProjeto)), null, false);
        }

        /// <summary>
        /// Retorna os dados a serem passados para o handler, para mostrar a figura associada com a medida correspondente desenhada
        /// </summary>
        public static string GetFiguraAssociadaUrl(uint idItemProjeto, uint idProjetoModelo)
        {
            return GetFiguraAssociadaUrl(idItemProjeto, ProjetoModeloDAO.Instance.GetElementByPrimaryKey(idProjetoModelo), null, false);
        }

        public static string GetFiguraAssociadaUrl(uint idItemProjeto, string numEtiqueta, bool pecaCompletaDestacada)
        {
            return GetFiguraAssociadaUrl(idItemProjeto,
                ProjetoModeloDAO.Instance.GetElementByPrimaryKey(ItemProjetoDAO.Instance.ObtemIdProjetoModelo(null, idItemProjeto)),
                numEtiqueta, pecaCompletaDestacada);
        }

        public static string GetFiguraAssociadaUrl(uint idItemProjeto, ProjetoModelo modelo)
        {
            return GetFiguraAssociadaUrl(null, idItemProjeto, modelo);
        }

        public static string GetFiguraAssociadaUrl(GDASession sessao, uint idItemProjeto, ProjetoModelo modelo)
        {
            return GetFiguraAssociadaUrl(sessao, idItemProjeto, modelo, null, false);
        }

        /// <summary>
        /// Retorna os dados a serem passados para o handler, para mostrar a figura associada com a medida correspondente desenhada
        /// </summary>
        public static string GetFiguraAssociadaUrl(uint idItemProjeto, ProjetoModelo modelo, string numEtiqueta, bool pecaCompletaDestacada)
        {
            return GetFiguraAssociadaUrl(null, idItemProjeto, modelo, numEtiqueta, pecaCompletaDestacada);
        }

        /// <summary>
        /// Retorna os dados a serem passados para o handler, para mostrar a figura associada com a medida correspondente desenhada
        /// </summary>
        public static string GetFiguraAssociadaUrl(GDASession sessao, uint idItemProjeto, ProjetoModelo modelo, string numEtiqueta, bool pecaCompletaDestacada)
        {
            uint? idPecaItemProj = null;
            int item = 0;

            if (!String.IsNullOrEmpty(numEtiqueta))
            {
                ProdutosPedidoEspelho prodPed = ProdutosPedidoEspelhoDAO.Instance.GetProdPedByEtiqueta(sessao, null, ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPed(sessao, ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPedProducao(sessao, numEtiqueta).GetValueOrDefault(0)), true);
                if (prodPed.IdMaterItemProj != null)
                {
                    MaterialItemProjeto material = MaterialItemProjetoDAO.Instance.GetElementByPrimaryKey(sessao, prodPed.IdMaterItemProj.Value);
                    idPecaItemProj = material.IdPecaItemProj;
                }

                if (idPecaItemProj > 0)
                    item = GetItemPecaFromEtiqueta(PecaItemProjetoDAO.Instance.ObtemItem(sessao, idPecaItemProj.Value), numEtiqueta);
            }

            return GetFiguraAssociadaUrl(sessao, idItemProjeto, modelo, idPecaItemProj, item, pecaCompletaDestacada);
        }

        /// <summary>
        /// Retorna os dados a serem passados para o handler, para mostrar a figura associada com a medida correspondente desenhada
        /// </summary>
        public static string GetFiguraAssociadaUrl(uint idItemProjeto, uint idProjetoModelo, uint? idPecaItemProj, int numItem)
        {
            return GetFiguraAssociadaUrl(null, idItemProjeto, idProjetoModelo, idPecaItemProj, numItem);
        }

        /// <summary>
        /// Retorna os dados a serem passados para o handler, para mostrar a figura associada com a medida correspondente desenhada
        /// </summary>
        public static string GetFiguraAssociadaUrl(GDASession sessao, uint idItemProjeto, uint idProjetoModelo, uint? idPecaItemProj, int numItem)
        {
            return GetFiguraAssociadaUrl(sessao, idItemProjeto, ProjetoModeloDAO.Instance.GetElementByPrimaryKey(sessao, idProjetoModelo), idPecaItemProj, numItem, false);
        }

        /// <summary>
        /// Retorna os dados a serem passados para o handler, para mostrar a figura associada com a medida correspondente desenhada
        /// </summary>
        private static string GetFiguraAssociadaUrl(uint idItemProjeto, ProjetoModelo modelo, uint? idPecaItemProj, int numItem,
            bool pecaCompletaDestacada)
        {
            return GetFiguraAssociadaUrl(null, idItemProjeto, modelo, idPecaItemProj, numItem, pecaCompletaDestacada);
        }

        /// <summary>
        /// Retorna os dados a serem passados para o handler, para mostrar a figura associada com a medida correspondente desenhada
        /// </summary>
        private static string GetFiguraAssociadaUrl(GDASession sessao, uint idItemProjeto, ProjetoModelo modelo, uint? idPecaItemProj, int numItem,
            bool pecaCompletaDestacada)
        {
            string modeloPath = Utils.ModelosProjetoPath(HttpContext.Current);
            string nomeFigura = modelo.NomeFiguraAssociada;

            if (pecaCompletaDestacada)
            {
                // Se a peça individual não existir, busca a imagem completa do projeto
                nomeFigura = modelo.Codigo + "§" + numItem + "M.jpg";
                if (!File.Exists(modeloPath + nomeFigura))
                    nomeFigura = modelo.IdProjetoModelo.ToString("0##") + "_" + numItem + "M.jpg";
                if (!File.Exists(modeloPath + nomeFigura))
                    nomeFigura = modelo.NomeFiguraAssociada;
            }
            else if (idPecaItemProj != null && !String.IsNullOrEmpty(nomeFigura))
            {
                // Se a peça individual não existir, busca a imagem completa do projeto, desde que seja instalação
                nomeFigura = modelo.Codigo + "§" + numItem + ".jpg";
                if (!File.Exists(modeloPath + nomeFigura))
                    nomeFigura = modelo.IdProjetoModelo.ToString("0##") + "_" + numItem + ".jpg";
                if (!File.Exists(modeloPath + nomeFigura) && PecaItemProjetoDAO.Instance.ObtemTipo(sessao, idPecaItemProj.Value) == 1)
                    nomeFigura = modelo.NomeFiguraAssociada;
            }

            // DEIXAR O INÍCIO COMO ../../ AO INVÉS DE ~/ DÁ ERRO QUANDO ESTÁ NOS CLIENTES, 
            // JÁ EXISTE TRATAMENTO PARA ISTO QUANDO EXECUTA LOCAL
            string imgUrl = "../../Handlers/LoadFiguraAssociada.ashx?tipoDesenho=" + modelo.TipoDesenho +
                "&path=" + modeloPath + nomeFigura + "&idItemProjeto=" + idItemProjeto +
                "&idProjetoModelo=" + modelo.IdProjetoModelo + (numItem > 0 && !pecaCompletaDestacada ? "&item=" + numItem : String.Empty);

            #region Informações para montagem da imagem da peça

            if (MedidaItemProjetoDAO.Instance.ExistsMedida(sessao, idItemProjeto))
            {
                if (modelo.TipoDesenho > 0)
                {
                    int altPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(sessao, idItemProjeto, 13, false);
                    int altFech = MedidaItemProjetoDAO.Instance.GetByItemProjeto(sessao, idItemProjeto, 14, false);
                    int trinco = MedidaItemProjetoDAO.Instance.GetByItemProjeto(sessao, idItemProjeto, 15, false);
                    int distEixoPux = MedidaItemProjetoDAO.Instance.GetByItemProjeto(sessao, idItemProjeto, 17, false);

                    List<PecaItemProjeto> lstPeca;
                    switch (modelo.TipoDesenho)
                    {
                        case 1: // Altura Puxador/Fechadura Lateral Esquerda
                        case 9: // PS01 - PS08
                        case 10: // PS09, PS10, PS14
                        case 11: // PS21, PS22
                        case 12: // PS23 - PS32, PS51, PS52
                        case 13: // PS33, PS34, PS53, 54
                        case 14: // PS35 - PS40, PS43 - PS48, PS55, PS56, PS59, PS60
                        case 15: // PS41, PS42, PS49, PS50, PS57, PS58
                        case 81: // BA02
                        case 82: // BA03
                            imgUrl += "&altPuxFech=" + (altPux > 0 ? altPux : altFech); break;
                        case 2: // Janela 5 peças 8mm
                            lstPeca = PecaItemProjetoDAO.Instance.GetByItemProjeto(sessao, idItemProjeto, modelo.IdProjetoModelo);
                            if (lstPeca.Count == 4)
                                imgUrl += "&altPuxBasc=" + ((lstPeca[3].Altura / 2) - (lstPeca[3].Altura / 10)) + "&altPuxFech=" + (altPux > 0 ? altPux : altFech); break;
                        case 3: // Janela 5 peças 10mm
                            lstPeca = PecaItemProjetoDAO.Instance.GetByItemProjeto(sessao, idItemProjeto, modelo.IdProjetoModelo);
                            if (lstPeca.Count == 4)
                                imgUrl += "&altPuxBasc=" + ((lstPeca[3].Altura / 2) - 50) + "&altPuxFech=" + (altPux > 0 ? altPux : altFech); break;
                        case 4: // CR09, CR51, CRK09, CRK51
                            imgUrl += "&trinco=" + (trinco - (distEixoPux / 2)) + "&distEixoPux=" + distEixoPux; break;
                        case 5: // Porta 4 peças 8mm
                            lstPeca = PecaItemProjetoDAO.Instance.GetByItemProjeto(sessao, idItemProjeto, modelo.IdProjetoModelo);
                            if (lstPeca.Count == 4)
                                imgUrl += "&altPuxBasc=" + ((lstPeca[2].Altura / 2) - (lstPeca[2].Altura / 10)) + "&altPuxFech=" + (altPux > 0 ? altPux : altFech); break;
                        case 6: // Porta 4 peças 10mm
                            lstPeca = PecaItemProjetoDAO.Instance.GetByItemProjeto(sessao, idItemProjeto, modelo.IdProjetoModelo);
                            if (lstPeca.Count == 4)
                                imgUrl += "&altPuxBasc=" + ((lstPeca[2].Altura / 2) - 50) + "&altPuxFech=" + (altPux > 0 ? altPux : altFech); break;
                        case 7: // CR32, CR74, CRK32, CRK74
                            imgUrl += "&altFech=" + (altFech - (distEixoPux / 2)) + "&distEixoPux=" + distEixoPux; break;
                        case 8: // PS01D, PS02D
                            imgUrl += "&altPux=" + altPux; break;
                        case 16: // PS61, PS62
                            lstPeca = PecaItemProjetoDAO.Instance.GetByItemProjeto(sessao, idItemProjeto, modelo.IdProjetoModelo);
                            if (lstPeca.Count == 4)
                                imgUrl += "&altLargBasc=" + (lstPeca[0].Largura / 2) + "&altPuxFech=" + (altPux > 0 ? altPux : altFech); break;
                        case 17: // PD01D, PD02D
                            imgUrl += "&altPuxFech=" + altPux + "&altPux=" + (altPux - (distEixoPux / 2)) + "&distEixoPux=" + distEixoPux; break;
                        case 18: // PD01 - PD08
                        case 19: // PD09, PD10
                        case 20: // PD11 - PD13
                        case 21: // PD21
                        case 22: // PD15 - PD20
                        case 23: // PD22, PD23
                        case 24: // PD23, PD24
                        case 25: // PD25, PD26
                        case 26: // PD27 - PD32, PD51, PD52
                        case 27: // PD33, PD34, PD53, PD54
                        case 28: // PD35 - PD40, PD55, PD56
                        case 29: // PD41, PD42, PD57, PD58
                        case 30: // PD43 - PD48, PD59, PD60
                        case 31: // PD49, PD50
                        case 33: // CPT01, CPT03, CPT05, CPT07, CPT09, CPT11, CPT13, CPT15, CPT17, CPT19, CPT21, CPT25, CPT29, CPT33, CPT37, CPT41, CPT45, CPT49
                        case 34: // CPT02, CPT04, CPT06, CPT08, CPT10, CPT12, CPT22, CPT26, CPT30, CPT34, CPT38, CPT42, CPT46, CPT50
                        case 35: // CPT14, CPT16, CPT18, CPT20, CPT46
                        case 36: // CPT23, CPT27, CPT31, CPT35, CPT39, CPT43, CPT47, CPT51
                        case 37: // CPT24, CPT28, CPT32, CPT36, CPT40, CPT44 
                        case 38: // CPT48, CPT52
                        case 39: // CPT53, CPT55, CPT57, CPT59, CPT61, CPT63, CPT65, CPT67, CPT69, CPT71, CPT73, CPT75, CPT77, CPT79
                        case 40: // CPT54, CPT56, CPT58, CPT60, CPT62, CPT64, CPT66, CPT68, CPT70, CPT72
                        case 41: // CPT74, CPT76, CPT78, CPT80
                        case 42: // CPT81, CPT85, CPT89, CPT93, CPT97, CPT101, CPT105
                        case 43: // CPT82, CPT86, CPT90, CPT94, CPT98, CPT102, CPT106
                        case 44: // CPT83, CPT87, CPT91, CPT95, CPT99, CPT103, CPT107
                        case 45: // CPT84, CPT88, CPT92, CPT96, CPT100, CPT104, CPT108
                            imgUrl += "&altPuxFech=" + (altPux > 0 ? altPux : altFech) + "&altPuxDesc=" + (altPux - (distEixoPux / 2)) + "&distEixoPux=" + distEixoPux; break;
                        case 32: // PD61, PD62
                            lstPeca = PecaItemProjetoDAO.Instance.GetByItemProjeto(sessao, idItemProjeto, modelo.IdProjetoModelo);
                            if (lstPeca.Count == 4)
                                imgUrl += "&altLargBasc=" + (lstPeca[0].Largura / 2) + "&altPuxDesc=" + (altPux - (distEixoPux / 2)) + "&distEixoPux=" + distEixoPux + "&altPuxFech=" + (altPux > 0 ? altPux : altFech); break;
                        case 46: // FX01, FX05, FX07 - FX13
                            lstPeca = PecaItemProjetoDAO.Instance.GetByItemProjeto(sessao, idItemProjeto, modelo.IdProjetoModelo);
                            imgUrl += "&altTrinco=" + (lstPeca[0].Altura / 2); break;
                        case 47: // FX14, FX18, FX19
                        case 48: // FX15
                        case 49: // FX20, FX21
                            lstPeca = PecaItemProjetoDAO.Instance.GetByItemProjeto(sessao, idItemProjeto, modelo.IdProjetoModelo);
                            imgUrl += "&altTrinco=" + (lstPeca[1].Altura / 2) + "&largBasc=" + (lstPeca[0].Largura / 2); break;
                        case 50: // BS01, BS03, BS05
                        case 60: // BS19, BS21
                        case 62: // BS23, BS25
                        case 64: // BS27, BS29 
                        case 66: // BS31, BS33
                            lstPeca = PecaItemProjetoDAO.Instance.GetByItemProjeto(sessao, idItemProjeto, modelo.IdProjetoModelo);
                            imgUrl += "&altBasc=" + ((lstPeca[0].Altura / 2) - (lstPeca[0].Altura / 10)); break;
                        case 51: // BS02, BS04, BS06
                        case 61: // BS20, BS22
                        case 63: // BS24, BS26
                        case 65: // BS28, BS30
                        case 67: // BS32, BS34
                            lstPeca = PecaItemProjetoDAO.Instance.GetByItemProjeto(sessao, idItemProjeto, modelo.IdProjetoModelo);
                            imgUrl += "&altBasc=" + ((lstPeca[0].Altura / 2) - 50); break;
                        case 52: // BS07
                        case 54: // BS09
                        case 56: // BS11, BS13
                        case 58: // BS15, BS17
                            lstPeca = PecaItemProjetoDAO.Instance.GetByItemProjeto(sessao, idItemProjeto, modelo.IdProjetoModelo);
                            imgUrl += "&altBasc=" + ((lstPeca[1].Altura / 2) - (lstPeca[1].Altura / 10)); break;
                        case 53: // BS08
                        case 55: // BS10
                        case 57: // BS12, BS14
                        case 59: // BS16, BS18
                            lstPeca = PecaItemProjetoDAO.Instance.GetByItemProjeto(sessao, idItemProjeto, modelo.IdProjetoModelo);
                            imgUrl += "&altBasc=" + ((lstPeca[1].Altura / 2) - 50); break;
                        case 68: // PV01, PV03
                        case 69: // PV02
                        case 70: // PV04
                        case 71: // PV05
                        case 72: // PV06
                        case 74: // PV08
                        case 76: // PV10
                        case 78: // PV12
                        case 80: // PV14
                            lstPeca = PecaItemProjetoDAO.Instance.GetByItemProjeto(sessao, idItemProjeto, modelo.IdProjetoModelo);
                            imgUrl += "&largPivo=" + (lstPeca[0].Largura / 2); break;
                        case 73: // PV07
                        case 75: // PV09
                        case 77: // PV11
                        case 79: // PV13
                            lstPeca = PecaItemProjetoDAO.Instance.GetByItemProjeto(sessao, idItemProjeto, modelo.IdProjetoModelo);
                            imgUrl += "&largPivo=" + (lstPeca[0].Largura / 2) + "&largPivo2=" + (lstPeca[1].Largura / 2); break;
                    }
                }
            }

            #endregion

            return imgUrl;
        }

        #endregion

        #region TableCell para ser inserida numa Table

        /// <summary>
        /// Cria uma td para ser inserida numa table
        /// </summary>
        /// <param name="controle">Controle a ser inserido dentro da célula (td) criada</param>
        /// <param name="texto">Texto a ser inserido na célula</param>
        /// <param name="largura">Largura da célula</param>
        /// <param name="cssClass"></param>
        /// <returns></returns>
        public static TableCell CreateTableCell(Control controle, string texto, int largura, string cssClass)
        {
            TableCell td = new TableCell();
            td.Style.Value = "padding: 2px; text-align: center";
            td.Attributes.Add("align", "center");

            if (largura > 0)
                td.Width = new Unit(largura, UnitType.Pixel);

            if (!String.IsNullOrEmpty(cssClass))
                td.CssClass = cssClass;

            if (!String.IsNullOrEmpty(texto))
                td.Text = texto;

            if (controle != null)
                td.Controls.Add(controle);

            return td;
        }

        /// <summary>
        /// Cria uma td para ser inserida numa table com uma textbox dentro.
        /// </summary>
        public static TableCell CreateTableCellWithTxt(string nomeTxt, int larguraTxt, int textoTxt, bool enabled, string cssClassTd,
            bool incluirAtributoMedidasAlteradas)
        {
            TextBox txt = new TextBox();
            txt.Attributes.Add("onkeypress", "return soNumeros(event, true, false);");

            if (incluirAtributoMedidasAlteradas)
                txt.Attributes.Add("onkeyup", "if (FindControl('hdfMedidasAlteradas', 'input') != null) { FindControl('hdfMedidasAlteradas', 'input').value = 'true'; } return false;");

            txt.Width = larguraTxt;
            txt.ID = nomeTxt;
            txt.Text = textoTxt != 0 ? textoTxt.ToString() : String.Empty;
            txt.Enabled = enabled;

            return CreateTableCell(txt, null, larguraTxt, null);
        }

        #endregion

        #region Calcula a área do vão

        /// <summary>
        /// Calcula a área do vão de um projeto
        /// </summary>
        public static float CalculaAreaVao(uint idItemProjeto, bool medidaExata)
        {
            return CalculaAreaVao(null, idItemProjeto, medidaExata);
        }

        /// <summary>
        /// Calcula a área do vão de um projeto
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idItemProjeto"></param>
        /// <param name="medidaExata"></param>
        /// <returns></returns>
        public static float CalculaAreaVao(GDA.GDASession sessao, uint idItemProjeto, bool medidaExata)
        {
            int qtdVao = MedidaItemProjetoDAO.Instance.GetByItemProjeto(sessao, idItemProjeto, 1, false);
            float totM2;

            if (medidaExata)
            {
                totM2 = PecaItemProjetoDAO.Instance.ExecuteScalar<float>(sessao,
                    @"Select Sum(
                        ((((50 - If(Mod(altura, 50) > 0, Mod(altura, 50), 50)) + altura) * 
                        ((50 - If(Mod(largura, 50) > 0, Mod(largura, 50), 50)) + largura)) / 1000000) * qtde
                    ) From peca_item_projeto Where idItemProjeto=" + idItemProjeto);
            }
            else
            {
                int larguraVao = MedidaItemProjetoDAO.Instance.GetByItemProjeto(sessao, idItemProjeto, 2, false);
                int alturaVao = MedidaItemProjetoDAO.Instance.GetByItemProjeto(sessao, idItemProjeto, 3, false);

                if (larguraVao > 0 && alturaVao > 0)
                    return Glass.Global.CalculosFluxo.ArredondaM2(sessao, larguraVao, alturaVao, qtdVao, 0, false);
                
                int largVaoEsquerda = MedidaItemProjetoDAO.Instance.GetByItemProjeto(sessao, idItemProjeto, "Larg. Vão Esquerda");
                var largVaoCentral = MedidaItemProjetoDAO.Instance.GetByItemProjeto(sessao, idItemProjeto, "Larg. Vão Central");
                int largVaoDireita = MedidaItemProjetoDAO.Instance.GetByItemProjeto(sessao, idItemProjeto, "Larg. Vão Direita");

                int largVaoDireitaSup = MedidaItemProjetoDAO.Instance.GetByItemProjeto(sessao, idItemProjeto, "Larg.Vão Direiro Superior");
                int altVaoDireitaSup = MedidaItemProjetoDAO.Instance.GetByItemProjeto(sessao, idItemProjeto, "Alt. Vão Superior Direito");

                int largVaoEsquerdaSup = MedidaItemProjetoDAO.Instance.GetByItemProjeto(sessao, idItemProjeto, "Larg.Vão Esquerdo Superior");
                int altVaoEsquerdaSup = MedidaItemProjetoDAO.Instance.GetByItemProjeto(sessao, idItemProjeto, "Alt.Vão Esq Superior");

                totM2 = Glass.Global.CalculosFluxo.ArredondaM2(sessao, largVaoEsquerda, alturaVao, qtdVao, 0, false) +
                    CalculosFluxo.ArredondaM2(sessao, largVaoCentral, alturaVao, qtdVao, 0, false) +
                    Glass.Global.CalculosFluxo.ArredondaM2(sessao, largVaoDireita, alturaVao, qtdVao, 0, false) +
                    Glass.Global.CalculosFluxo.ArredondaM2(sessao, largVaoDireitaSup, altVaoDireitaSup, qtdVao, 0, false) +
                    Glass.Global.CalculosFluxo.ArredondaM2(sessao, largVaoEsquerdaSup, altVaoEsquerdaSup, qtdVao, 0, false);
            }

            return totM2;
        }

        #endregion

        #region Formata texto para orçamento

        /// <summary>
        /// Formata texto para orçamento de projeto
        /// </summary>
        public static string FormataTextoOrcamento(GDA.GDASession sessao, ItemProjeto itemProjeto)
        {
            ProjetoModelo projetoModelo = ProjetoModeloDAO.Instance.GetByItemProjeto(sessao, itemProjeto.IdItemProjeto);

            /* Vários clientes estavam reclamando que quando era selecionado o projeto apenas com vidros
             * estava sendo buscada a descrição do projeto com ferragens, devido à reclamação destes clientes,
             * agora será buscado o texto de apenas vidros independentemente se houver um texto.
            string textoOrcamento = itemProjeto.ApenasVidros && (!String.IsNullOrEmpty(projetoModelo.TextoOrcamentoVidro) || ControleSistema.GetSite() == ControleSistema.ClienteSistema.PlanaltoVidros) ?
                projetoModelo.TextoOrcamentoVidro : projetoModelo.TextoOrcamento;*/

            var textoOrcamento = itemProjeto.ApenasVidros ? projetoModelo.TextoOrcamentoVidro : projetoModelo.TextoOrcamento;

            if (String.IsNullOrEmpty(textoOrcamento))
                return String.Empty;

            // Carrega as peças deste cálculo
            List<PecaItemProjeto> lstPeca = PecaItemProjetoDAO.Instance.GetByItemProjeto(sessao, itemProjeto.IdItemProjeto, itemProjeto.IdProjetoModelo);

            // Conta a quantidade de peças de vidro deste cálculo
            int numPecas = 0;
            float areaTotal = 0;
            foreach (PecaItemProjeto pip in lstPeca)
            {
                numPecas += pip.Qtde;
                areaTotal += Glass.Global.CalculosFluxo.ArredondaM2(sessao, pip.Largura, pip.Altura, pip.Qtde, 0, false);
            }

            // Obtém a espessura das peças
            string espessura = String.Empty;
            foreach (PecaItemProjeto pip in lstPeca)
            {
                MaterialItemProjeto mip = MaterialItemProjetoDAO.Instance.GetMaterialByPeca(sessao, pip.IdPecaItemProj);

                // Tenta pegar a espessura do material, se não der, pega do produto
                if (mip != null && mip.Espessura > 0 && !espessura.Contains(mip.Espessura.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0')))
                    espessura += mip.Espessura.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0') + "/";
                else if (mip != null && mip.Espessura == 0 && mip.IdProd > 0)
                {
                    float espProd = ProdutoDAO.Instance.ObtemEspessura(sessao, (int)mip.IdProd);

                    if (!espessura.Contains(espProd.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0')))
                        espessura += espProd.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0') + "/";
                }
            }

            if (itemProjeto.Qtde == 0) itemProjeto.Qtde = 1;

            float areaVao = (!itemProjeto.MedidaExata ? (itemProjeto.M2Vao / itemProjeto.Qtde) : lstPeca.Count > 0 ?
                Glass.Global.CalculosFluxo.ArredondaM2(sessao, lstPeca[0].Largura, lstPeca[0].Altura, lstPeca[0].Qtde, 0, lstPeca[0].Redondo) : 0);

            if (ProjetoConfig.TelaCadastroAvulso.AreaTotalItemProjetoAreaPecas)
                areaVao = areaTotal;

            string texto = textoOrcamento
                .Replace("#m2", areaVao.ToString("N2") + "m²")
                .Replace("#qtd_pc_fchto", (numPecas / itemProjeto.Qtde).ToString())
                .Replace("#qtd", itemProjeto.Qtde.ToString())
                .Replace("#vao_total", (areaVao * itemProjeto.Qtde).ToString("N2") + "m²")
                .Replace("#cor_v", itemProjeto.DescrCorVidro)
                .Replace("#cor_al", itemProjeto.DescrCorAluminio)
                .Replace("#cor_fr", itemProjeto.DescrCorFerragem)
                .Replace("#num_pc", numPecas.ToString())
                .Replace("#esp_pc", espessura.Trim('/'));

            return texto;
        }

        #endregion

        #region CADProject

        /// <summary>
        /// Cria um projeto no CADProject
        /// </summary>
        public static string CriarProjetoCADProject(string projeto, uint idPecaItemProj, string url, bool pcp)
        {
            var urlServicoCadProject = ProjetoConfig.UrlServicoCadProject;

            if (string.IsNullOrEmpty(urlServicoCadProject))
                throw new Exception("A URL do CADProject não esta cadastrada.");
            
            var urlAcessoCadProject = urlServicoCadProject;

            /* Chamado 63272. */
            if (ProjetoConfig.CadProjectInstaladoNoMesmoLocalSistema)
            {
                // Recupera o endereço de acesso do servidor do sistema com a porta.
                // Se for local retorna LocalHost, se não retorna a url utilizada para acesso.
                var portaCadProject = new Uri(urlAcessoCadProject).Port;
                var urlUsuario = HttpContext.Current.Request.Url;

                urlAcessoCadProject = string.Format("{0}{1}:{2}", (!string.IsNullOrEmpty(urlUsuario.Scheme) ? string.Format("{0}://", urlUsuario.Scheme) : string.Empty), urlUsuario.Host,
                    portaCadProject.ToString());
            }

            var peca = PecaItemProjetoDAO.Instance.GetElementExt(null, idPecaItemProj, pcp);

            if (peca == null)
                throw new Exception("Peça do projeto não encontrada");

            var idArquivoCalcEngine = ArquivoMesaCorteDAO.Instance.ObtemIdArquivoCalcEngine(peca.IdArquivoMesaCorte.Value);

            if (idArquivoCalcEngine <= 0)
                throw new Exception("Peça não possui arquivo CalcEngine");

            var idArquivoMesaCorte = peca.IdArquivoMesaCorte;
            var tipoArquivo = peca.TipoArquivoMesaCorte.GetValueOrDefault(0);
            var caminho = PCPConfig.CaminhoSalvarCadProject(pcp);
            var caimhoCompleto = caminho + peca.IdProdPed.GetValueOrDefault(0) + ".dxf";

            var flags = FlagArqMesaDAO.Instance.ObtemPorPecaProjMod((int)peca.IdPecaProjMod, true);
            var alterouTipoArquivo = false;

            if (peca != null && peca.Tipo == 1)
            {
                int item;

                /* Chamado 58078. */
                if (!int.TryParse(peca.Item, out item))
                    item = peca.Item[0].ToString().StrParaInt();

                var pecaProjMod = PecaProjetoModeloDAO.Instance.GetByItem(null, ItemProjetoDAO.Instance.GetIdProjetoModelo(null, peca.IdItemProjeto), item);

                // Verifica se esta peça possui arquivo de mesa corte, não há associação com o arquivo de mesa de corte vindo do projeto
                if (pecaProjMod.IdArquivoMesaCorte != null)
                {
                    idArquivoMesaCorte = pecaProjMod.IdArquivoMesaCorte;

                    if (tipoArquivo == 0)
                    {
                        tipoArquivo = pecaProjMod.TipoArquivo == null ? 0 : (int)pecaProjMod.TipoArquivo;
                        alterouTipoArquivo = true;
                    }
                }

                flags = FlagArqMesaDAO.Instance.ObtemPorPecaProjMod((int)pecaProjMod.IdPecaProjMod, true);

                if (tipoArquivo == 0 || alterouTipoArquivo)
                    tipoArquivo = tipoArquivo == (int)TipoArquivoMesaCorte.SAG && flags.Where(f => f.Descricao == TipoArquivoMesaCorte.DXF.ToString() || f.Descricao == TipoArquivoMesaCorte.FML.ToString()).Count() > 0 ?
                    flags.Where(f => f.Descricao == TipoArquivoMesaCorte.DXF.ToString() || f.Descricao == TipoArquivoMesaCorte.FML.ToString())
                    .Select(f => (int)((TipoArquivoMesaCorte)Enum.Parse(typeof(TipoArquivoMesaCorte), f.Descricao, true))).FirstOrDefault() :
                    tipoArquivo;
            }

            // Se não houver associação do arquivo de mesa de corte com o projeto, procura associação no produto
            if (idArquivoMesaCorte <= 0 || idArquivoMesaCorte == null)
            {
                idArquivoMesaCorte = ProdutoDAO.Instance.ObtemIdArquivoMesaCorte(null, peca.IdProd.GetValueOrDefault(0));

                if (idArquivoMesaCorte > 0)
                {
                    var tipoArquivoMesaCorteProduto = ProdutoDAO.Instance.ObterTipoArquivoMesaCorte(null, (int)peca.IdProd.GetValueOrDefault(0));

                    tipoArquivo =
                        tipoArquivoMesaCorteProduto > 0 ? (int)tipoArquivoMesaCorteProduto :
                        PCPConfig.TipoArquivoMesaPadrao == "DXF" ? (int)TipoArquivoMesaCorte.DXF :
                        PCPConfig.TipoArquivoMesaPadrao == "FML" ? (int)TipoArquivoMesaCorte.FML :
                        (int)TipoArquivoMesaCorte.SAG;
                }
            }

            tipoArquivo = tipoArquivo == 0 && idArquivoMesaCorte.GetValueOrDefault() > 0 ? ArquivoMesaCorteDAO.Instance.ObtemTipoArquivo(null, idArquivoMesaCorte.Value) : tipoArquivo;

            using (var ms = new MemoryStream())
            {
                string mensagemErro = null;
                bool? retorno = null;
                var espessura = ProdutoDAO.Instance.ObtemEspessura((int)peca.IdProd.GetValueOrDefault(0));

                List<int> idsBenef = null;
                uint idProcesso = 0;
                string descrBenef = null;

                if (pcp)
                {
                    idsBenef = ProdutoPedidoEspelhoBenefDAO.Instance.GetByProdutoPedido(null, peca.IdProdPed.GetValueOrDefault()).Select(f => (int)f.IdBenefConfig).ToList();
                    idProcesso = ProdutosPedidoEspelhoDAO.Instance.ObtemIdProcesso(null, peca.IdProdPed.GetValueOrDefault());
                    descrBenef = ProdutoPedidoEspelhoBenefDAO.Instance.GetDescrBenef(null, peca.IdProdPed.GetValueOrDefault());
                }
                else
                {
                    idsBenef = ProdutoPedidoBenefDAO.Instance.GetByProdutoPedido(peca.IdProdPed.GetValueOrDefault()).Select(f => (int)f.IdBenefConfig).ToList();
                    idProcesso = ProdutosPedidoDAO.Instance.ObtemIdProcesso(null, peca.IdProdPed.GetValueOrDefault());
                    descrBenef = ProdutoPedidoBenefDAO.Instance.GetDescrBenef(null, null, peca.IdProdPed.GetValueOrDefault(), false);
                }

                var descontoLap = ArquivoMesaCorteDAO.Instance.ObterDescontoLapidacao(null, ref peca, idArquivoMesaCorte, (int)peca.IdProd.GetValueOrDefault(0), idsBenef, descrBenef, (int)idProcesso);
                var codigoArquivo = ArquivoCalcEngineDAO.Instance.ObtemValorCampo<string>("nome", "idArquivoCalcEngine=" + idArquivoCalcEngine);

                if (codigoArquivo == null)
                    throw new Exception("Um dos arquivos de mesa está associado à um calc engine inválido.");

                var config = new ArquivoMesaCorteDAO.ConfiguracoesArqMesa(peca.Largura, espessura, descontoLap, codigoArquivo);

                ArquivoMesaCorteDAO.Instance.GerarArquivoCalcEngine(null, idArquivoCalcEngine, descontoLap, tipoArquivo, pcp, peca.IdProdPed.GetValueOrDefault(0), peca, peca.Altura, peca.Largura,
                    ref mensagemErro, codigoArquivo, config, espessura, ms, flags, ref retorno, true, false, false);

                if (!Directory.Exists(caminho))
                    Directory.CreateDirectory(caminho);

                if (File.Exists(caimhoCompleto))
                    File.Delete(caimhoCompleto);

                using (var file = new FileStream(caimhoCompleto, FileMode.Create, FileAccess.Write))
                {
                    ms.WriteTo(file);
                }
            }

            var manager = new CADProject.Remote.Client.ExternalProjectManager(new Uri(urlServicoCadProject + "/Services/ExternalProjectService.svc"));

            using (var content = File.Open(caimhoCompleto, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var guid = manager.CreateProject(projeto, idPecaItemProj.ToString(), url + "&idPecaItemProj=" + idPecaItemProj, content);

                PecaItemProjetoDAO.Instance.AtualizaGUID(idPecaItemProj, guid.ToString());

                return urlAcessoCadProject + "/engineering/project/external/" + guid.ToString();
            }
        }

        /// <summary>
        /// Salva o projeto editado vindo do CADProject
        /// </summary>
        public static void SalvarProjetoCADProject(uint idPecaItemProj, bool pcp, bool cancelado)
        {
            if (cancelado)
                return;

            var urlServicoCadProject = ProjetoConfig.UrlServicoCadProject;

            if (string.IsNullOrEmpty(urlServicoCadProject))
                throw new Exception("A URL do CADProject não esta cadastrada.");

            var manager = new CADProject.Remote.Client.ExternalProjectManager(new Uri(urlServicoCadProject + "/Services/ExternalProjectService.svc"));

            var guid = PecaItemProjetoDAO.Instance.ObterGUID(idPecaItemProj);

            if (guid == null)
                throw new Exception("Não foi possivel importar o arquivo do CADProject. GUID não encontrado.");

            var itemPeca = string.Empty;

            using (var ms = new MemoryStream())
            {
                using (var preview = new MemoryStream())
                {

                    if (!manager.DownloadProject(guid, ms, preview))
                        throw new Exception("Não foi possivel importar o arquivo do CADProject. Falha no download.");

                    var peca = PecaItemProjetoDAO.Instance.GetElementExt(null, idPecaItemProj, pcp);
                    itemPeca = peca.Item;

                    var caminho = PCPConfig.CaminhoSalvarCadProject(pcp) + peca.IdProdPed.GetValueOrDefault(0);
                    var caminhoCompletoDxf = caminho + ".dxf";
                    var caminhoCompletoSvg = caminho + ".svg";
                    var caminhoCompletoJpg = (pcp ? Utils.GetPecaProducaoVirtualPath : Utils.GetPecaComercialVirtualPath) + peca.IdProdPed.GetValueOrDefault(0).ToString().PadLeft(10, '0') + "_" + itemPeca + ".jpg";

                    if (System.Web.HttpContext.Current != null)
                        caminhoCompletoJpg = System.Web.HttpContext.Current.Request.MapPath(caminhoCompletoJpg);

                    if (File.Exists(caminhoCompletoDxf))
                        File.Delete(caminhoCompletoDxf);

                    if (File.Exists(caminhoCompletoSvg))
                        File.Delete(caminhoCompletoSvg);

                    if (File.Exists(caminhoCompletoJpg))
                        File.Delete(caminhoCompletoJpg);

                    using (var file = new FileStream(caminhoCompletoDxf, FileMode.Create, FileAccess.Write))
                    {
                        ms.WriteTo(file);
                    }

                    // Converte os bytes do memory stream preview para texto.
                    preview.Position = 0;

                    var streamReader = new StreamReader(preview);
                    var previewTexto = streamReader.ReadToEnd();

                   
                    using (var previewFormatado = new MemoryStream())
                    {
                        // Escreve o texto alterado no memory stream preview.
                        var streamWriter = new StreamWriter(previewFormatado);
                        streamWriter.Write(previewTexto);
                        streamWriter.Flush();

                        using (var file = new FileStream(caminhoCompletoSvg, FileMode.Create, FileAccess.Write))
                        {
                            previewFormatado.WriteTo(file);
                        }

                        using (var img = new MagickImage(caminhoCompletoSvg))
                        {
                            img.Format = MagickFormat.Png;
                            img.Write(caminhoCompletoJpg);
                        }
                    }
                }
            }

            LogAlteracaoDAO.Instance.LogImagemProducao(idPecaItemProj, itemPeca, string.Format("Edição da imagem do item {0}.", itemPeca));
            PecaItemProjetoDAO.Instance.ImagemEditada(idPecaItemProj, true);
        }

        #endregion
    }
}
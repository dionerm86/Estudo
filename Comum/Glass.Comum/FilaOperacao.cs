using System.Collections.Generic;
using System.Threading;

namespace Glass
{
    /// <summary>
    /// Implementa uma fila de operações do sistema.
    /// </summary>
    public class FilaOperacoes
    {
        #region Classe com a fila

        private class BlockingQueue<T>
        {
            private readonly Queue<T> _queue = new Queue<T>();

            public void Enqueue(T item)
            {
                lock (_queue)
                {
                    _queue.Enqueue(item);
                }
            }

            public T Dequeue()
            {
                lock (_queue)
                {
                    return _queue.Dequeue();
                }
            }

            public T Peek()
            {
                lock (_queue)
                {
                    return _queue.Peek();
                }
            }
        }

        #endregion

        private readonly BlockingQueue<long> _fila;
        private long _numero;

        // Não permite instâncias externas
        private FilaOperacoes()
        {
            _fila = new BlockingQueue<long>();
            _numero = 0;
        }

        #region Instâncias

        static FilaOperacoes()
        {
            #region Financeiro

            #region Depósito não identificado
            
            AtualizarDepositoNaoIdentificado = new FilaOperacoes();
            CancelarDepositoNaoIdentificado = new FilaOperacoes();
            InserirDepositoNaoIdentificado = new FilaOperacoes();

            #endregion
            
            BoletoImpresso = new FilaOperacoes();
            CancelarConta = new FilaOperacoes();
            CancelarDevolucaoPagto = new FilaOperacoes();
            CancelarImpostoServ = new FilaOperacoes();
            CancelarLiberacao = new FilaOperacoes();
            ContasReceber = new FilaOperacoes();
            CorrecaoSaldoMovBanco = new FilaOperacoes();
            FinalizarImpostoServ = new FilaOperacoes();
            LiberarPedido = new FilaOperacoes();
            Pagamento = new FilaOperacoes();
            QuitarChequeDevolvidoEmAberto = new FilaOperacoes();
            QuitarDebitoFuncionario = new FilaOperacoes();
            ReabrirImpostoServ = new FilaOperacoes();
            ReceberAcerto = new FilaOperacoes();
            ReceberContaReceber = new FilaOperacoes();
            ReceberDevolucaoPagto = new FilaOperacoes();
            ReceberObra = new FilaOperacoes();
            ReceberSinal = new FilaOperacoes();
            Recebimento = new FilaOperacoes();
            RecebimentosGerais = new FilaOperacoes();
            RenegociarAcerto = new FilaOperacoes();

            #endregion

            #region Fiscal

            ConhecimentoTransporte = new FilaOperacoes();
            GerarNfCompra = new FilaOperacoes();
            NotaFiscalEmitir = new FilaOperacoes();
            NotaFiscalInserir = new FilaOperacoes();
            NotaFiscalInutilizar = new FilaOperacoes();
            ManifestoEletronico = new FilaOperacoes();

            #endregion

            #region Global

            AjustePrecoProdutoBeneficiamento = new FilaOperacoes();
            AlteraDadosFiscaisProduto = new FilaOperacoes();
            CadastrarCliente = new FilaOperacoes();

            #endregion

            #region Helper

            EnviarEmailAdministradores = new FilaOperacoes();
            LogArquivo = new FilaOperacoes();
            SepararContas = new FilaOperacoes();

            #endregion

            #region Pedido

            AtualizarPedido = new FilaOperacoes();
            AtualizarProdutoPedido = new FilaOperacoes();
            ConfirmarPedido = new FilaOperacoes();
            InserirProdutoPedido = new FilaOperacoes();

            #endregion

            #region PCP

            LeituraPecaProducao = new FilaOperacoes();

            #endregion

            #region Projeto

            ApagarPecaProjetoModelo = new FilaOperacoes();
            AtualizarPecaProjetoModelo = new FilaOperacoes();
            ConfirmarProjeto = new FilaOperacoes();
            GerarPedido = new FilaOperacoes();
            InserirPecaProjetoModelo = new FilaOperacoes();

            #endregion

            #region Orçamento

            AtualizarOrcamento = new FilaOperacoes();

            #endregion
        }

        #region Financeiro

        #region Depósito não identificado

        public static FilaOperacoes AtualizarDepositoNaoIdentificado { get; private set; }
        public static FilaOperacoes CancelarDepositoNaoIdentificado { get; private set; }
        public static FilaOperacoes InserirDepositoNaoIdentificado { get; private set; }

        #endregion

        public static FilaOperacoes BoletoImpresso { get; private set; }
        public static FilaOperacoes CancelarConta { get; private set; }
        public static FilaOperacoes CancelarDevolucaoPagto { get; private set; }
        public static FilaOperacoes CancelarImpostoServ { get; private set; }
        public static FilaOperacoes CancelarLiberacao { get; private set; }
        public static FilaOperacoes ContasReceber { get; private set; }
        public static FilaOperacoes CorrecaoSaldoMovBanco { get; private set; }
        public static FilaOperacoes FinalizarImpostoServ { get; private set; }
        public static FilaOperacoes LiberarPedido { get; private set; }
        public static FilaOperacoes Pagamento { get; private set; }
        public static FilaOperacoes QuitarChequeDevolvidoEmAberto { get; private set; }
        public static FilaOperacoes QuitarDebitoFuncionario { get; private set; }
        public static FilaOperacoes ReabrirImpostoServ { get; private set; }
        public static FilaOperacoes ReceberAcerto { get; private set; }
        public static FilaOperacoes ReceberContaReceber { get; private set; }
        public static FilaOperacoes ReceberDevolucaoPagto { get; private set; }
        public static FilaOperacoes ReceberObra { get; private set; }
        public static FilaOperacoes ReceberSinal { get; private set; }
        public static FilaOperacoes Recebimento { get; private set; }
        public static FilaOperacoes RecebimentosGerais { get; private set; }
        public static FilaOperacoes RenegociarAcerto { get; private set; }

        #endregion

        #region Fiscal

        public static FilaOperacoes ConhecimentoTransporte { get; private set; }
        public static FilaOperacoes GerarNfCompra { get; private set; }
        public static FilaOperacoes NotaFiscalEmitir { get; private set; }
        public static FilaOperacoes NotaFiscalInserir { get; private set; }
        public static FilaOperacoes NotaFiscalInutilizar { get; private set; }
        public static FilaOperacoes ManifestoEletronico { get; private set; }

        #endregion

        #region Global

        public static FilaOperacoes AjustePrecoProdutoBeneficiamento { get; private set; }
        public static FilaOperacoes AlteraDadosFiscaisProduto { get; private set; }
        public static FilaOperacoes CadastrarCliente { get; private set; }

        #endregion

        #region Helper

        public static FilaOperacoes EnviarEmailAdministradores { get; private set; }
        public static FilaOperacoes LogArquivo { get; private set; }
        public static FilaOperacoes SepararContas { get; private set; }

        #endregion

        #region Pedido

        public static FilaOperacoes AtualizarPedido { get; private set; }
        public static FilaOperacoes AtualizarProdutoPedido { get; private set; }
        public static FilaOperacoes ConfirmarPedido { get; private set; }
        public static FilaOperacoes InserirProdutoPedido { get; private set; }

        #endregion

        #region PCP

        public static FilaOperacoes LeituraPecaProducao { get; private set; }

        #endregion

        #region Projeto

        public static FilaOperacoes ApagarPecaProjetoModelo { get; private set; }
        public static FilaOperacoes AtualizarPecaProjetoModelo { get; private set; }
        public static FilaOperacoes ConfirmarProjeto { get; private set; }
        public static FilaOperacoes GerarPedido { get; private set; }
        public static FilaOperacoes InserirPecaProjetoModelo { get; private set; }

        #endregion

        #region Orçamento

        public static FilaOperacoes AtualizarOrcamento { get; private set; }

        #endregion
        
        #endregion

        /// <summary>
        /// Aguarda até que a vez na fila chegue.
        /// </summary>
        /// <returns></returns>
        public void AguardarVez()
        {
            long item = Interlocked.Increment(ref _numero);
            _fila.Enqueue(item);

            while (_fila.Peek() != item)
                Thread.Sleep(300);
        }

        /// <summary>
        /// Termina a execução do item na fila.
        /// </summary>
        public void ProximoFila()
        {
            _fila.Dequeue();
        }
    }
}
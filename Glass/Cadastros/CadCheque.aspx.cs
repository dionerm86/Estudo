using System;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadCheque : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            odsCadCheque.Inserted += odsCadCheque_Inserted;
            if (!IsPostBack)
            {
                dtvCheque.Rows[0].Cells[0].FindControl("drpLoja").Focus();
                grdCheque.Visible = Cadastrar();
                lblTotal.Visible = !Cadastrar();
                GetTotal();
            }
    
            if (!string.IsNullOrEmpty(Request["controlForma"]) && Request["controlForma"] != "9")
            {
                if (!string.IsNullOrEmpty(Request["IdCli"]))
                    ((TextBox)dtvCheque.Rows[0].Cells[0].FindControl("txtTitular")).Text = ClienteDAO.Instance.GetNome(Glass.Conversoes.StrParaUint(Request["IdCli"]));
            }
    
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadCheque));
        }        
    
        protected void odsCadCheque_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao cadastrar Cheque.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
            {
                GetTotal();
                grdCheque.DataBind();            
            }
        }
    
        protected void odsCheques_Deleted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            GetTotal();
        }
    
        protected void GetTotal()
        {
            if (!String.IsNullOrEmpty(Request["idAcerto"]) && Request["idAcerto"] != "null")
            {
                if (Cadastrar())
                    hdfTotal.Value = ChequesDAO.Instance.GetTotalInAcerto(Glass.Conversoes.StrParaUint(Request["idAcerto"])).ToString().Replace(",", ".");
    
                hdfIdCliente.Value = AcertoDAO.Instance.ObtemIdCliente(null, Glass.Conversoes.StrParaUint(Request["idAcerto"])).ToString();
    
                if (!string.IsNullOrEmpty(Request["controlForma"]) && Request["controlForma"] != "9")
                    ((TextBox)dtvCheque.FindControl("txtTitular")).Text = ClienteDAO.Instance.GetNomeByAcerto(Glass.Conversoes.StrParaUint(Request["idAcerto"]));
            }
            else if (!String.IsNullOrEmpty(Request["idPedido"]) && Request["idPedido"] != "null")
            {
                if (Cadastrar())
                    hdfTotal.Value = ChequesDAO.Instance.GetTotalInPedido(Glass.Conversoes.StrParaUint(Request["idPedido"]), Request["IdContaR"] != null ? Glass.Conversoes.StrParaUint(Request["IdContaR"]) : 0, 0, Glass.Conversoes.StrParaInt(Request["origem"])).ToString().Replace(",", ".");
                
                hdfIdCliente.Value = PedidoDAO.Instance.ObtemIdCliente(null, Glass.Conversoes.StrParaUint(Request["idPedido"])).ToString();
    
                if (!string.IsNullOrEmpty(Request["controlForma"]) && Request["controlForma"] != "9")
                    ((TextBox)dtvCheque.FindControl("txtTitular")).Text = ClienteDAO.Instance.GetNomeByPedido(Glass.Conversoes.StrParaUint(Request["idPedido"]));
            }
            else if (!String.IsNullOrEmpty(Request["idChequeDevolvido"]) && Request["idChequeDevolvido"] != "null")
            {
                if (Cadastrar())
                    hdfTotal.Value = ChequesDAO.Instance.GetTotalInChequeDevolvido(Glass.Conversoes.StrParaUint(Request["idChequeDevolvido"])).ToString().Replace(",", ".");
    
                hdfIdCliente.Value = ChequesDAO.Instance.ObtemIdCliente(Glass.Conversoes.StrParaUint(Request["idChequeDevolvido"])).ToString();
            }
            else if (!String.IsNullOrEmpty(Request["idCliente"]) && Request["idCliente"] != "null")
            {
                hdfIdCliente.Value = Glass.Conversoes.StrParaUint(Request["idCliente"]).ToString();
    
                if (!string.IsNullOrEmpty(Request["controlForma"]) && Request["controlForma"] != "9")
                    ((TextBox)dtvCheque.FindControl("txtTitular")).Text = ClienteDAO.Instance.GetNome(Glass.Conversoes.StrParaUint(Request["idCliente"]));
            }
        }
    
        protected void btnInserir_Load(object sender, EventArgs e)
        {
            if (!Cadastrar())
            {
                Button btnInserir = (Button)sender;
                btnInserir.OnClientClick = "try { setOpenerCheque(); } catch (err) { alert(err); return false; } return false;";
            }
        }
    
        /// <summary>
        /// Verifica se o cheque já existe ou se deve ser bloqueado pelo dígito verificador
        /// </summary>
        [Ajax.AjaxMethod()]
        public string ValidaCheque(string idCliente, string banco, string agencia, string conta, string numero, string digitoNum)
        {
            if (ChequesDAO.Instance.ExisteCheque(0, banco, agencia, conta, Glass.Conversoes.StrParaInt(numero)))
                return "false|Já foi cadastrado um cheque com os dados informados.";
    
            if (FinanceiroConfig.FormaPagamento.BloquearChequesDigitoVerificador && !String.IsNullOrEmpty(idCliente) && idCliente != "0" && 
                !String.IsNullOrEmpty(digitoNum) && ChequesDAO.Instance.ExisteChequeDigito(Glass.Conversoes.StrParaUint(idCliente), 0, Glass.Conversoes.StrParaInt(numero), digitoNum))
                return "false|Já foi cadastrado um cheque com os dados informados.";
    
            return "true";
        }
    
        [Ajax.AjaxMethod()]
        public string DadosCliente(string idCliente)
        {
            if (string.IsNullOrEmpty(idCliente))
                return null;
    
            var c = ClienteDAO.Instance.GetElement(Glass.Conversoes.StrParaUint(idCliente));
    
            if (c == null)
                return null;
    
            return c.Nome + ";" + c.TipoPessoa + ";" + c.CpfCnpj;
        }
    
        protected bool Cadastrar()
        {
            try
            {
                return bool.Parse(Request["cadastrar"]);
            }
            catch
            {
                return true;
            }
        }
    
        protected string GetDataVencMaxVista()
        {
            int numeroDiasPermitido = Liberacao.FormaPagamento.NumeroDiasChequeVistaLiberarPedido;
    
            if (numeroDiasPermitido == 0 || !PedidoConfig.LiberarPedido)
                return DateTime.Now.AddYears(1).ToString("dd/MM/yyyy");
    
            DateTime dataMax = DateTime.Now.AddDays(numeroDiasPermitido);
    
            while (!dataMax.DiaUtil())
                dataMax = dataMax.AddDays(1);
    
            return dataMax.ToString("dd/MM/yyyy");
        }
    
        protected string GetDataVencMaxPrazo()
        {
            DateTime dataMax = DateTime.Now;
    
            uint idCliente = Glass.Conversoes.StrParaUint(Request["idCliente"]);
            if (idCliente > 0)
                dataMax = dataMax.AddDays(ParcelasDAO.Instance.GetPrazoMaximoDias(idCliente, 0));
    
            while (!dataMax.DiaUtil())
                dataMax = dataMax.AddDays(-1);
    
            return dataMax.ToString("dd/MM/yyyy");
        }
    
        protected string GetDataVencMin()
        {
            DateTime dataMin = DateTime.Now;

            if (!FinanceiroConfig.FormaPagamento.PermitirChequeDataAtual)
            {
                do
                {
                    dataMin = dataMin.AddDays(1);
                }
                while (!dataMin.DiaUtil());
            }
    
            return dataMin.ToString("dd/MM/yyyy");
        }
    
        protected void txtDigitoNum_Load(object sender, EventArgs e)
        {
            if (!FinanceiroConfig.FormaPagamento.BloquearChequesDigitoVerificador)
                ((TextBox)sender).Style.Add("display", "none");
        }
    
        protected int NumeroDigitosCheque()
        {
            return Glass.Configuracoes.ChequesConfig.TelaCadastro.NumeroDigitosCheque;
        }
    
        protected bool ExibirDadosLimiteCheque()
        {
            return Glass.Configuracoes.FinanceiroConfig.LimitarChequesPorCpfOuCnpj;
        }
    }
}

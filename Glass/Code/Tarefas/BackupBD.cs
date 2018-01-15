using Glass.Data.DAL;
using Glass.Data.Helper;
using Quartz;
using SyncBackup.MySql;
using System;
using System.Linq;

namespace Glass.UI.Web.Code.Tarefas
{
    public class BackupBD : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                // Pega o nome do banco de dados
                var nomeBanco = System.Configuration.ConfigurationManager.AppSettings["NomeBD"];

                if (string.IsNullOrEmpty(nomeBanco))
                    nomeBanco = System.Configuration.ConfigurationManager.AppSettings["sistema"];

                // Cria uma instância do objeto de backup
                var backup = new ExportaBackupMySql();
                backup.TipoBackup = ExportaBackupMySql.TipoBackupEnum.BackupDiario;
                backup.TabelasBD = ConfiguracaoDAO.Instance.ObtemTabelasBD(nomeBanco).ToArray();
                backup.CaminhoPastaBackup = Data.Helper.Utils.GetBackupPath;
                backup.NomeBD = nomeBanco;

                // A opção "Convert Zero Datetime=True" foi criada para resolver um problema na Dekor de conversão de data do MySQL para o WebGlass
                backup.StringDeConexaoBD = ConnectionString.Decrypt(DBUtils.GetConnString + ";Convert Zero Datetime=True");

                int i = 0;
                while (i < 3)
                {
                    try
                    {
                        backup.GeraBackup();
                        break;
                    }
                    catch
                    {
                        i++;
                        Thread.Sleep(10 * 1000);
                    }
                }

                i = 0;
                while (i < 3)
                {
                    try
                    {
                        backup.CompactaArquivo();
                        break;
                    }
                    catch
                    {
                        i++;
                        Thread.Sleep(10 * 1000);
                    }
                }

                i = 0;
                while (i < 3)
                {
                    try
                    {
                        backup.EnviaFTP();
                        break;
                    }
                    catch
                    {
                        i++;
                        Thread.Sleep(10 * 1000);
                    }
                }
            }
            catch (Exception ex)
            {
                ErroDAO.Instance.InserirFromException("Falha ao gerar backup automático", ex);
            }
        }
    }
}
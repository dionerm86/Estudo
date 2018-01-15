<%@ WebHandler Language="C#" Class="TesteArquivoRemoto" %>

using System;
using System.Web;
using Ionic.Utils.Zip;
using Glass.Data.Helper;
using System.IO;
using NPOI.HSSF.UserModel;
using Glass.Data.DAL;
using Glass.Data.Model;
using System.Collections.Generic;

public class TesteArquivoRemoto : IHttpHandler 
{
    public void ProcessRequest(HttpContext context)
    {
        using (new Colosoft.Net.Impersonator("Administrator", "dekorvidros.intra", "P@ssw0rd2014", Colosoft.Net.LogonType.LOGON32_LOGON_NEW_CREDENTIALS, Colosoft.Net.LogonProvider.LOGON32_PROVIDER_DEFAULT))
        {
            var fileName = System.IO.Path.Combine(@"\\sw37408\BLOCCHI", System.IO.Path.GetFileName(System.IO.Path.GetTempFileName()));
            System.IO.File.WriteAllText(fileName, "TESTE");
        }
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
}
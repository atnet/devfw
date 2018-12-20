using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Web;
using JR.DevFw.Framework.Web.Cache;
using JR.DevFw.PluginKernel.Web;
using JR.DevFw.Web.Cache;
using JR.DevFw.Web.Cache.Compoment;
using JR.DevFw.Web.Plugin;

namespace JR.DevFw.Web
{
    /// <summary>
    /// Ӧ�ó���������
    /// </summary>
    public class WebCtx
    {

        /// <summary>
        /// ������־�ļ�
        /// </summary>
        public static string ErrorFilePath;

        private const string KeyCtxDomain = "_ctx_domain";

        public static HttpContext HttpCtx
        {
            get { return HttpContext.Current; }
        }


        /// <summary>
        /// ��������Դ
        /// </summary>
        public object Source { get; set; }

        public HttpResponse Response { get { return HttpCtx.Response; } }
        public HttpRequest Request { get { return HttpCtx.Request; } }


        static WebCtx()
        {
            ErrorFilePath = AppDomain.CurrentDomain.BaseDirectory + "tmp/logs/error.log";
        }

        private PageDataItems _dataItems;
        private string _staticDomain;
        private string _resouceDomain;
        private string _host;
        private string _siteAppPath;
        private static IPluginApp _extends;
        private string _domain;

        /// <summary>
        /// ����ʱ����
        /// </summary>
        public static event FwHandler OnBeginRequest;


        /// <summary>
        /// ��ǰ��Host,�����˿ڣ��磺www.ops.cc:8080
        /// </summary>
        public string Host
        {
            get
            {
                if (this._host == null)
                {
                    // ͨ��Header��ȡHost
                    this._host = HttpCtx.Request.Headers.Get("Host");
                    // ͨ��ASP.NET��ʽ��ȡHost
                    if (String.IsNullOrEmpty(this._host))
                    {
                        this._host = String.Format("{0}{1}", HttpCtx.Request.Url.Host,
                            HttpCtx.Request.Url.Port != 80 ? ":" +
                            HttpCtx.Request.Url.Port.ToString() : "");
                    }
                }
                return this._host;
            }
        }

        public static WebCtx Current
        {
            get
            {
                const string key = "web_ctx_instance";
                WebCtx ctx = null;
                object obj = HttpCtx.Items[key];
                if (obj != null)
                {
                    ctx = obj as WebCtx;
                }

                if (ctx == null)
                {
                    ctx = new WebCtx();
                    HttpCtx.Items[key] = ctx;
                }
                return ctx;
            }
        }

        /// <summary>
        /// ϵͳӦ�ó���Ŀ¼
        /// </summary>
        public string ApplicationPath
        {
            get { return HttpCtx.Request.ApplicationPath; }
        }

        /// <summary>
        /// �����չ
        /// </summary>
        public IPluginApp Plugin
        {
            get
            {
                if (_extends == null)
                {
                    var handler = new WebPluginHandleProxy<System.Web.HttpContext>();
                    _extends = new WebPluginApp(handler);
                }
                return _extends;
            }
        }


        public ICache Cache
        {
            get { return CacheFactory.Sington; }
        }

        //        /// <summary>
        //        /// ����
        //        /// </summary>
        //        public string SiteDomain
        //        {
        //            get
        //            {
        //                if (_siteDomain == null)
        //                {
        //                    string host = String.Format("{0}{1}", context.Request.Url.Host,
        //                        context.Request.Url.Port != 80 ? ":" + context.Request.Url.Port.ToString() : "");
        //
        //                    _siteDomain= String.Format("http://{0}{1}{2}",
        //                        host,
        //                        this.ApplicationPath=="/"?"":this.ApplicationPath,
        //                        this.SiteAppPath=="/"?"/":this.SiteAppPath+"/"
        //                        );
        //
        ////                    this._siteDomain = this.CurrentSite.FullDomain;
        ////
        ////                    if (this._siteDomain.IndexOf("#") != -1)
        ////                    {
        ////                        this._siteDomain = this._siteDomain.Replace(
        ////                             "#", host);
        ////                    }
        //                }
        //                return _siteDomain;
        //            }
        //        }

        //        /// <summary>
        //        /// ��Դ��
        //        /// </summary>
        //        public string ResourceDomain
        //        {
        //            get
        //            {
        //                if (this._resouceDomain == null)
        //                {
        //                    //RES DOMAIN
        //                    if (this.IsVirtualDirectoryRunning)
        //                    {
        //                        this._resouceDomain = String.Empty;
        //                    }
        //                }
        //                return this._resouceDomain ?? (this._resouceDomain = this.SiteDomain);
        //            }
        //        }
        //
        //        /// <summary>
        //        /// ��̬��Դ��
        //        /// </summary>
        //        public string StaticDomain
        //        {
        //            get
        //            {
        //                if (this._staticDomain == null)
        //                {
        //                    if (Settings.SERVER_STATIC_ENABLED && Settings.SERVER_STATIC.Length != 0)
        //                    {
        //                        this._staticDomain = String.Concat("http://", Settings.SERVER_STATIC, "/");
        //                    }
        //                    else
        //                    {
        //                        this._staticDomain = this.ResourceDomain == String.Empty ? "/" : this.ResourceDomain;
        //                    }
        //                }
        //
        //                return this._staticDomain;
        //            }
        //        }
        /// <summary>
        /// ������
        /// </summary>
        public PageDataItems Items
        {
            get
            {
                if (_dataItems == null)
                {
                    _dataItems = new PageDataItems();
                }
                return _dataItems;
            }
        }
        public string Domain
        {
            get
            {

                if (this._domain == null)
                {
                    HttpRequest request = HttpCtx.Request;
                    String appPath = request.ApplicationPath;
                    this._domain = String.Format("{0}//{1}{2}",
                        request.Url.Scheme == "http" ? "" : request.Url.Scheme + ":",
                       this.Host,appPath == "/" ? "" : appPath
                       );
                }

                return this._domain;
            }
            set { this._domain = value; }
        }


        public static void SaveErrorLog(Exception exception)
        {
            lock (ErrorFilePath)
            {
                HttpRequest req = HttpCtx.Request;

                if (!File.Exists(ErrorFilePath))
                {
                    string dir = AppDomain.CurrentDomain.BaseDirectory + "tmp/logs";
                    if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                    File.Create(ErrorFilePath).Close();
                }
                HttpCtx.Response.Write((File.Exists(ErrorFilePath).ToString()));
                using (FileStream fs = new FileStream(ErrorFilePath, FileMode.Append, FileAccess.Write))
                {
                    StreamWriter sw = new StreamWriter(fs);
                    StringBuilder sb = new StringBuilder();

                    sb.Append("---------------------------------------------------------------------\r\n")
                        .Append("[����]��IP:").Append(req.UserHostAddress)
                        .Append("\tʱ�䣺").Append(DateTime.Now.ToString())
                        .Append("\r\n[��Ϣ]��").Append(exception.Message)
                        .Append("\r\n[·��]��").Append(req.Url.PathAndQuery)
                        .Append("  -> ��Դ��").Append(req.Headers["referer"] ?? "��")
                        .Append("\r\n[��ջ]��").Append(exception.StackTrace)
                        .Append("\r\n\r\n");

                    sw.Write(sb.ToString());

                    sw.Flush();
                    sw.Dispose();
                    fs.Dispose();
                }
            }
        }


        //        public bool CheckSiteState()
        //        {
        //            if (this.CurrentSite.State == SiteState.Normal)
        //            {
        //                return true;
        //            }
        //            else if (this.CurrentSite.State == SiteState.Closed)
        //            {
        //                this.RenderNotfound();
        //            }
        //            else if (this.CurrentSite.State == SiteState.Paused)
        //            {
        //                this.Render("<h1 style=\"color:red;text-align:center;font-size:16px;padding:20px\">��վά����,��ͣ���ʣ�</h1>");
        //                //this.RenderNotfound("<h1 style=\"color:red\">��վά����,��ͣ���ʣ�</h1>");
        //            }
        //            return false;
        //        }

        /// <summary>
        /// �������ÿͻ��˻���(��̨���û��沢��ʱ��>0)
        /// </summary>
        /// <returns></returns>
        public bool CheckAndSetClientCache()
        {
            if (WebConf.Opti_ClientCache && WebConf.Opti_ClientCacheSeconds > 0)
            {
                if (CacheUtil.CheckClientCacheExpires(WebConf.Opti_ClientCacheSeconds))
                {
                    CacheUtil.SetClientCache(this.Response, WebConf.Opti_ClientCacheSeconds);
                }
                else
                {
                    return false;
                }
            }
            return true;
        }


        /// <summary>
        /// �������ÿͻ��˻���(�Զ���ʱ��,��λ����)
        /// </summary>
        /// <returns></returns>
        public bool CheckAndSetClientCache(int maxAge)
        {
            if (maxAge > 0)
            {
                if (CacheUtil.CheckClientCacheExpires(maxAge))
                {
                    CacheUtil.SetClientCache(this.Response, maxAge);
                }
                else
                {
                    return false;
                }
            }
            return true;
        }


        /// <summary>
        /// ����
        /// </summary>
        /// <param name="html"></param>
        public void Render(string html)
        {
            HttpResponse response = this.Response;

            response.Write(html);

            //GZipѹ��
            if (WebConf.Opti_SupportGZip)
            {
                response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
                response.AddHeader("Content-Encoding", "gzip");
            }
            /*
            else
            {
            	response.Filter=new DeflateStream(response.Filter,CompressionMode.Compress);
                response.AddHeader("Content-Encoding", "deflate");
            }*/

            response.AddHeader("X-AspNet-Version", String.Format("JR.DevFw - v{0}", FwCtx.Version.GetVersion()));
            response.AddHeader("Support-URL", "github.com/jsix/devfw");
        }

        //
        //        /// <summary>
        //        /// ��ʾ400ҳ��
        //        /// </summary>
        //        /// <returns></returns>
        //        public void RenderNotfound()
        //        {
        //            this.RenderNotfound("File not found!", null);
        //        }
        //
        //        /// <summary>
        //        /// ��ʾ400ҳ��
        //        /// </summary>
        //        /// <returns></returns>
        //        public void RenderNotfound(string message, TemplatePageHandler handler)
        //        {
        //            Response.StatusCode = 404;
        //
        //            string html = null;
        //            try
        //            {
        //                TemplatePage tpl = new TemplatePage(String.Format("/{0}/not_found", this.CurrentSite.Tpl));
        //                if (handler != null)
        //                {
        //                    handler(tpl);
        //                }
        //                tpl.Render();
        //                return;
        //            }
        //            catch
        //            {
        //                html = "File not found!";
        //            }
        //
        //            Response.Write(html);
        //        }
        //
        //        public string ComposeUrl(string url)
        //        {
        //            if (url.StartsWith("/"))
        //                throw new ArgumentException("URL������\"/\"��ͷ!");
        //
        //            return String.Concat(this.SiteDomain, url);
        //        }
    }
}
using System.Web;

namespace SrtsWeb.SrtsWebLab
{
    /// <summary>
    /// Summary description for DownLoadFile
    /// </summary>
    public class DownLoadFile : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            string sGenName = @"E:\Tempdata\Friendly.txt";
            //context.Response.ContentType = "text/plain";
            //context.Response.Write("Hello World");
            // File.Delete(Server.MapPath("TextFiles/" + sFileName + ".txt"));
            context.Response.AddHeader("Content-disposition", "attachment; filename=" + sGenName);
            context.Response.ContentType = "text/plain";
            //context.Response.ContentType = "application/octet-stream";
            context.Response.WriteFile(context.Server.MapPath("TextFiles/Temp.txt"));
            //Response.End();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Reflection;
using System.Web.Optimization;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace HTTPHandlers
{
    public class CacheManifestHandler : IHttpHandler
    {

        private const string SEARCH_PATH = "~/app/";
        public void ProcessRequest(HttpContext context)
        {
            //don't let the browser/proxies cache the manifest using traditional caching methods.
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.Cache.SetNoStore();
            context.Response.Cache.SetExpires(DateTime.MinValue);

            //set the correct MIME type for the manifest
            context.Response.ContentType = "text/cache-manifest";

            //manifest requires this on first line
            context.Response.Write("CACHE MANIFEST" + Environment.NewLine);

            //write out the assets that MUST be cached
            context.Response.Write("NETWORK:" + Environment.NewLine);

            try { 
                //write out the links in the bundles
                
                foreach(var bndlPath in BundleTable.Bundles)
                {
                    WriteBundle(context, bndlPath.Path);
                }
                //WriteBundle(context, "~/bundles/javascript");
                //WriteBundle(context, "~/css/style");

                //add other assets not mentioned in the bundles
                string appPath = HttpContext.Current.Server.MapPath(SEARCH_PATH);
                DirectoryInfo di = new System.IO.DirectoryInfo(appPath);
                foreach (var file in di.GetFiles("*.html", System.IO.SearchOption.AllDirectories)) { 
                    string filePath = file.FullName;
                    filePath = filePath.Replace(appPath, SEARCH_PATH);
                    context.Response.Write(Scripts.Url(filePath) + Environment.NewLine);
                }
            }
            catch (Exception e) { 
                context.Response.Write("//" + e.ToString() + Environment.NewLine);
            }

            //context.Response.Write(Scripts.Url("~/Images/image1.png") + Environment.NewLine);
            //context.Response.Write(Scripts.Url("~/Images/image2.png") + Environment.NewLine);

            //write out the assets that MUST be used online
            //asterisk(*) means everything that is not already referenced to be cached
            context.Response.Write("CACHE:" + Environment.NewLine);
            context.Response.Write("" + Environment.NewLine);

            //if we are debugging then change the manifest file to ensure we download the latest changes
            if (IsDebug) context.Response.Write(Environment.NewLine + "\n#" + DateTime.Now.ToLongTimeString());
        }

        private void WriteBundle(HttpContext context, string virtualPath)
        {

            if (IsDebug)
            {
                var bundleContents = BundleResolver.Current.GetBundleContents(virtualPath);
                if (bundleContents != null)
                {
                    foreach (string contentVirtualPath in bundleContents)
                    {
                        context.Response.Write(Scripts.Url(contentVirtualPath).ToString() + Environment.NewLine);
                    }
                }
            }
            else
            {
                //RELEASE MODE - Url will have cache-busting param added to url
                context.Response.Write(Scripts.Url(virtualPath).ToString() + Environment.NewLine);
            }
        }

        private bool IsDebug
        {
            get
            {
#if DEBUG
                return true;
#else
                return false;
#endif
            }
        }

        public bool IsReusable
        {
            get { return false; }
        }

    }
}

/* Add the following to your Web config
 <system.web>
   <httpHandlers>
      <add name="CacheManifest" verb="GET" path="cache.manifest" type="HTTPHandlers.CacheManifestHandler"/>
  </httpHandlers>
 </system.web>

 <system.webServer>
   <handlers>
      <add name="CacheManifest" verb="GET" path="cache.manifest" type="HTTPHandlers.CacheManifestHandler"/>
  </handlers>
 </system.webServer>
 */
 
 /* Add the following to your layout HTML tag
 <html manifest="@Href("~/cache.manifest")">
  */

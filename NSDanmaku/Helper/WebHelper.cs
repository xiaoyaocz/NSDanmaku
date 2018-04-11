using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

namespace NSDanmaku.Helper
{
    class WebHelper
    {
        public  async Task<string> GetResults(Uri url)
        {
            HttpBaseProtocolFilter fiter = new HttpBaseProtocolFilter();
            fiter.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.Expired);
            using (HttpClient hc = new HttpClient(fiter))
            {
                HttpResponseMessage hr = await hc.GetAsync(url);
                hr.EnsureSuccessStatusCode();
                string results = await hr.Content.ReadAsStringAsync();
                return results;
            }
        }


    }
}

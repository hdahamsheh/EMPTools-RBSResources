using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPMTools.RBSResources.EPMRepository.PSI.Services
{
    public class ResourceService : ServiceBase<SvcResource.ResourceClient>
    {
        public ResourceService(string pwaURL, string username, string password, string domain)
        {
            this.pwaURL = pwaURL;
            this.username = username;
            this.password = password;
            this.domain = domain;

            bool isHttps = this.pwaURL.ToLower().Contains("https");

            client = new SvcResource.ResourceClient(GetBinding(isHttps), GetAddress());

            client.ClientCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
            client.ClientCredentials.Windows.ClientCredential.UserName = username;
            client.ClientCredentials.Windows.ClientCredential.Password = password;
        }

        public SvcResource.ResourceAuthorizationDataSet GetResource(Guid ResourceID)
        {
            var resource = client.ReadResourceAuthorization(ResourceID);
            return resource;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using PSLibrary = Microsoft.Office.Project.Server.Library;

namespace EPMTools.RBSResources.EPMRepository.PSI.Services
{
    public class SecurityService : ServiceBase<SvcSecurity.SecurityClient>
    {   
        public SecurityService(string pwaURL, string username, string password, string domain)
        {
            this.pwaURL = pwaURL;
            this.username = username;
            this.password = password;
            this.domain = domain;

            bool isHttps = pwaURL.ToLower().Contains("https");

            client = new SvcSecurity.SecurityClient(GetBinding(isHttps), GetAddress());

            client.ClientCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
            client.ClientCredentials.Windows.ClientCredential.UserName = username;
            client.ClientCredentials.Windows.ClientCredential.Password = password;
            
        }

        //public void GetGroupsByUser(string username)
        //{
        //    var groups = client.ReadGroupList();
        //}

        public  List<Entities.Group> GetGroups(List<Guid> groupsIDs)
        {
            var groups = client.ReadGroupList();
            bool getAll = groupsIDs.Count < 1;

            List<Entities.Group> groupsList = new List<Entities.Group>();
            foreach(SvcSecurity.SecurityGroupsDataSet.SecurityGroupsRow item in groups.SecurityGroups)
            {
                if (getAll || groupsIDs.Any(g => g == item.WSEC_GRP_UID))
                {
                    groupsList.Add(new Entities.Group()
                    {
                        UniqueID = item.WSEC_GRP_UID,
                        Name = item.WSEC_GRP_NAME
                    });
                }
            }

            return groupsList;
        }
    }
}

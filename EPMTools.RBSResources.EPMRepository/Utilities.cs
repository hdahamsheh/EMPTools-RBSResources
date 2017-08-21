using EPMTools.RBSResources.EPMRepository.Entities;
using Microsoft.ProjectServer.Client;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EPMTools.RBSResources.EPMRepository
{
    public class Utilities
    {
        public Utilities(string pwaUrl, string username, string password, string domain)
        {
            context = new ProjectContext(pwaUrl);
            context.Credentials = new NetworkCredential(username, password, domain);
        }

        private ProjectContext context = null;

        public List<Entities.User> GetResources()
        {
            List<Entities.User> users = new List<Entities.User>();
            try
            {
                context.Load(context.EnterpriseResources,
                    rs => rs.IncludeWithDefaultProperties(r => r.Name, r => r.Email, r => r.IsActive, r => r.User, r => r.User.LoginName, r => r.CustomFields));
                context.ExecuteQuery();

                foreach (var item in context.EnterpriseResources)
                {
                    if (item != null && !item.ServerObjectIsNull.Value)
                    {
                        var user = new Entities.User();

                        user.Email = item.Email != null ? item.Email : "";

                        if (item.User != null && item.User.ServerObjectIsNull.HasValue && !item.User.ServerObjectIsNull.Value)
                            user.LoginName = item.User.LoginName;

                        user.IsActive = item.IsActive;
                        user.Name = item.Name != null ? item.Name : "";
                        user.ID = item.Id;

                        var cf = item.CustomFields.Where(c => c.Name.ToLower() == "rbs").SingleOrDefault();
                        if (cf != null)
                        {
                            string internalName = ((String[])item.FieldValues.Where(f => f.Key == cf.InternalName).Single().Value)[0];

                            user.RBSInternalName = internalName;
                        }

                        users.Add(user);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return users;
        }
     
        public List<Node> GetNodes()
        {
            List<Node> nodes = new List<Node>();
            var ltRBS = context.LoadQuery(context.LookupTables
                .IncludeWithDefaultProperties(r => r.Name, r => r.Entries,
                    r => r.Entries.IncludeWithDefaultProperties(e => ((LookupText)e).Parent, e => ((LookupText)e).Parent.Id))
                .Where(l => l.Name == "RBS"));
            context.ExecuteQuery();

            if (ltRBS.Count() > 0)
            {
                foreach (LookupText item in ltRBS.First().Entries)
                {
                    if (item.Parent.ServerObjectIsNull.HasValue && item.Parent.ServerObjectIsNull.Value)
                    {
                        nodes.Add(new Node()
                        {
                            ID = item.Id,
                            ParentID = Guid.Empty,
                            Discription = item.Description,
                            Text = item.Value,
                            FullText = item.FullValue
                        });
                    }
                    else
                    {
                        nodes.Add(new Node()
                        {
                            ID = item.Id,
                            ParentID = item.Parent.Id,
                            Discription = item.Description,
                            Text = item.Value,
                            FullText = item.FullValue
                        });
                    }

                }
            }            

            return nodes;
        }

        public List<Entities.Group> GetUserSharepointGroupGroups(Guid userID)
        {
            List<Entities.Group> groups = new List<Entities.Group>();
            EnterpriseResource resource = context.EnterpriseResources.GetByGuid(userID);
            context.Load(resource, r=>r.User, r => r.User.Groups, r => r.User.Groups.IncludeWithDefaultProperties(g=>g.LoginName));
            context.ExecuteQuery();

            if(!resource.ServerObjectIsNull.Value)
            {
                foreach(var item in resource.User.Groups)
                {
                    Entities.Group group = new Entities.Group();
                    group.ID = item.Id;
                    group.Name = item.Title;
                    group.Description = item.Description;

                    groups.Add(group);
                }
            }

            return groups;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPMTools.RBSResources.EPMRepository.Entities
{
    public class User
    {
        public string Name { get; set; }
        public string LoginName { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public string RBS { get; set; }
        public string RBSInternalName { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPMTools.RBSResources.EPMRepository.Entities
{
    public class Group
    {
        public int ID { get; internal set; }
        public string Name { get; internal set; }
        public Guid UniqueID { get; internal set; }
    }
}

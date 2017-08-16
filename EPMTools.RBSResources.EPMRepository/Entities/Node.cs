using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPMTools.RBSResources.EPMRepository.Entities
{
    public class Node
    {
        public Node()
        {
            SubNodes = new List<Node>();
            Users = new List<User>();
        }

        public Guid ID { get; set; }
        public Guid ParentID { get; set; }
        public int Level { get; set; }
        public string Text { get; set; }
        public string FullText { get; set; }
        public string Discription { get; set; }
        public IList<Node> SubNodes { get; set; }
        public IList<User> Users { get; set; }
        public string Internalname
        {
            get
            {
                return $"Entry_{ID.ToString().Replace("-", "").ToLower()}";
            }
        }
    }
}

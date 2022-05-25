using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LambdaDynamoDB.Models
{
    public class User
    {
        public string PK { get; set; }
        public string SK { get; set; }

        public string attr1 { get; set; }
        public string attr2 { get; set; }
        public int attr3 { get; set; }


    }
}

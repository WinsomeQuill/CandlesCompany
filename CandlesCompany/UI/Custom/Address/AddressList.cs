using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandlesCompany.UI.Custom.Address
{
    public class AddressList
    {
        public int ID { get; set; } // DataGrid ID адреса
        public string Name { get; set; } // DataGrid название адреса
        public JToken Address { get; set; }
        public AddressList(JToken address)
        {
            Address = address;
            ID = (int)Address["Id"];
            Name = (string)Address["Address"];
        }
    }
}

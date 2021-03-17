using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityNet.Models
{
    public class UserClass
    {
        [JsonProperty("id")]
        public int ID { get; set; }

        [JsonProperty("account")]
        public String Account { get; set; }

        [JsonProperty("name")]
        public String Name { get; set; }

        [JsonProperty("mobile")]
        public String Mobile { get; set; }

        [JsonProperty("usertype")]
        public int UserType { get; set; }

        [JsonProperty("departmentId")]
        public int DepartmentId { get; set; }

        [JsonProperty("proName")]
        public String ProName { get; set; }


        public void trim()
        {
            if (ID == null)
            {
                ID = -1;
            }
            if (Account == null)
            {
                Account = "";
            }
            if (Name == null)
            {
                Name = "";
            }

            if (Mobile == null)
            {
                Mobile = "";
            }

            if (UserType == null)
            {
                UserType = -1;
            }
            if (DepartmentId == null)
            {
                DepartmentId = -1;
            }

            if (ProName == null)
            {
                ProName = "";
            }

        }
    }
}
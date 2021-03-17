using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityNet.Models
{
    public class ShareRole
    {
        [JsonProperty("id")]
        public int ID { get; set; }

        [JsonProperty("name")]
        public String Name { get; set; }

        [JsonProperty("content")]
        public String Content { get; set; }

        [JsonProperty("roletype")]
        public int RoleType { get; set; }

        public void trim()
        {
            if (ID == null)
            {
                ID = -1;
            }
            if (Name == null)
            {
                Name = "";
            }

            if (Content == null)
            {
                Content = "";
            }

            if (RoleType == null)
            {
                RoleType = -1;
            }
        }

    }
}
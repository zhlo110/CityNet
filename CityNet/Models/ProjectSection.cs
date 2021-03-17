using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityNet.Models
{
    public class ProjectSection
    {
        [JsonProperty("id")]
        public int ID { get; set; }

        [JsonProperty("name")]
        public String Name { get; set; }


        [JsonProperty("projectinfoId")]
        public int ProjectinfoId { get; set; }

    }
}
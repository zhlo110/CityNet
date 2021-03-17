using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityNet.Models
{
    public class ProjectArea
    {
        [JsonProperty("id")]
        public int ID { get; set; }

        [JsonProperty("name")]
        public String Name { get; set; }


        [JsonProperty("projectSectionId")]
        public int ProjectSectionId { get; set; }
    }
}
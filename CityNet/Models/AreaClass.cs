using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityNet.Models
{
    //{"id":485,"name":"华中片区","code":null,"priority":5,"useFlag":1}
    public class AreaClass //七大区的解析
    {
        [JsonProperty("id")]
        public int ID { get; set; }

        [JsonProperty("name")]
        public String Name { get; set; }
    }
}
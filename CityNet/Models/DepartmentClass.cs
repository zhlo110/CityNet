using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityNet.Models
{
    public class DepartmentClass
    {
        [JsonProperty("id")]
        public int ID { get; set; }

        [JsonProperty("name")]
        public String Name { get; set; }

        [JsonProperty("parentid")]
        public int ParentID { get; set; }


        [JsonProperty("shortname")]
        public String ShortName { get; set; }

        [JsonProperty("grade")]
        public int Grade { get; set; }

        [JsonProperty("depLevel")]
        public int DepLevel { get; set; }

        [JsonProperty("categoryId")]
        public int CategoryId { get; set; }

        [JsonProperty("projectInfoId")]
        public int ProjectInfoId { get; set; }

        [JsonProperty("projectSectionId")]
        public int ProjectSectionId { get; set; }


        [JsonProperty("projectAreaId")]
        public int ProjectAreaId { get; set; }

        [JsonProperty("createDate")]
        public long CreateDate { get; set; }


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
            if (ParentID == null)
            {
                ParentID = -1;
            }

            if (ShortName == null)
            {
                ShortName = "";
            }

            if (Grade == null)
            {
                Grade = -1;
            }
            if (DepLevel == null)
            {
                DepLevel = -1;
            }

            if (CategoryId == null)
            {
                CategoryId = -1;
            }

            if (ProjectInfoId == null)
            {
                ProjectInfoId = -1;
            }


            if (ProjectAreaId == null)
            {
                ProjectAreaId = -1;
            }

            if (CreateDate == null)
            {
                CreateDate = DateTime.Now.Ticks;
            }
        }

    }
}
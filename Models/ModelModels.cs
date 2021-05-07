using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace tourismAPi.Models
{
    public class infoModels
    {
        [Required]
        public string username { get; set; }
    }

    public class statusModels
    {
        public string license { get; set; }
        public bool showWarn { get; set; }
        public string status { get; set; }
    }

    public class sitemsModels
    {
        public string category { get; set; }
        public bool traffic { get; set; }
        public bool location { get; set; }
        public List<Dictionary<string, object>> items { get; set; }
        public bool showWarn { get; set; }
    }
}
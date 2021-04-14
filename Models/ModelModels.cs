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
        public bool showWarn { get; set; }
        public string status { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YazGelFinal.WebUI.Models
{
    public class UserClick
    {
        public int Id { get; set; }
        public string ConnectionId { get; set; }
        public int ClickCount { get; set; }
    }
}

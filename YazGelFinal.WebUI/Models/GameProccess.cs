using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YazGelFinal.WebUI.Models
{
    public class GameProccess
    {
        public int Id { get; set; }
        public string ImagePath { get; set; }
        public string ImageRandomGuid { get; set; }
        public int CardLocation { get; set; }
        public int CardLevel { get; set; }
        public bool WasShown { get; set; }
        public bool Completed { get; set; }
        public string ConnectionId { get; set; }
    }
}

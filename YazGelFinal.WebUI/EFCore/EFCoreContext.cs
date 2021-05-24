using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YazGelFinal.WebUI.Models;

namespace YazGelFinal.WebUI.EFCore
{
    public class EFCoreContext:DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=sql.poseidon.domainhizmetleri.com;Initial Catalog=metince1_yazgel2;User ID=metince1_webapps;Password=MetinCem*08;");
        }

        public DbSet<Card> Cards { get; set; }
        public DbSet<GameProccess> GameProccesses { get; set; }
        public DbSet<UserClick> UserClicks { get; set; }

    }
}

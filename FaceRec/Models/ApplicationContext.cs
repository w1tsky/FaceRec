using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FaceRec.Models
{
    public class ApplicationContext : DbContext
    {


        public DbSet<Person> Persons { get; set; }
        public DbSet<User> Users { get; set; }
       

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@" Data Source=SQL5097.site4now.net;Initial Catalog=DB_A6C118_facerec;User Id=DB_A6C118_facerec_admin;Password=5027028Pepsi");
        }


    }
}

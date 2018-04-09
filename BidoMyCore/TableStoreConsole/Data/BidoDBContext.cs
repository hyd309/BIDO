using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;

namespace TableStoreConsole.Data
{
    public class BidoDBContext:DbContext
    {
        public BidoDBContext(DbContextOptions<BidoDBContext> options) : base(options)
        {

        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionBuilder)
        //{
        //    optionBuilder.UseSqlServer(TableStoreModel.SqlServerConnection);
        //}

        public DbSet<Device_Location_20170307> Device_Location_20170307s { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Device_Location_20170307>().ToTable("Device_Location_20170307");
        }
    }
}

using FreshFalaye.Pos.Shared.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreshFalaye.Pos.Shared
{
    public class PosDbContextFactory : IDesignTimeDbContextFactory<PosDbContext>
    {
        public PosDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PosDbContext>();

            optionsBuilder.UseSqlServer(
                                "Server=DESKTOP-V3QC17D\\SQLEXPRESS;Database=FreshFalayePosDb;Trusted_Connection=True;TrustServerCertificate=True");           
            return new PosDbContext(optionsBuilder.Options);
        }
    }
}

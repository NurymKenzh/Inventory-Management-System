using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Inventory_Management_System.Models;

namespace Inventory_Management_System.Data
{
    public class Inventory_Management_SystemContext : DbContext
    {
        public Inventory_Management_SystemContext (DbContextOptions<Inventory_Management_SystemContext> options)
            : base(options)
        {
        }

        public DbSet<Inventory_Management_System.Models.Category> Category { get; set; } = default!;
    }
}

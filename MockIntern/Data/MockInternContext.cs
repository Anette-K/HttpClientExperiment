using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace MockIntern.Data
{
    public class MockInternContext : DbContext
    {
        public MockInternContext (DbContextOptions<MockInternContext> options)
            : base(options)
        {
        }

        public DbSet<Shared.Claim> Claim { get; set; } = default!;
    }
}

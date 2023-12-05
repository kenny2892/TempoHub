using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TempoHub.Models;

namespace TempoHub.Data
{
    public class SongContext : DbContext
    {
        public DbSet<SongPath> SongPaths { get; set; }

        public SongContext(DbContextOptions<SongContext> options) : base(options)
        {
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Shared.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.DAL
{
    public class NServiceBusDbContext : DbContext
    {
        public NServiceBusDbContext(DbContextOptions<NServiceBusDbContext> options)
            : base(options)
        {
        }
    }
}
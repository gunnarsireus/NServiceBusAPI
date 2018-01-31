using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace Shared.DAL
{
	public class CarApiContext : DbContext
	{
		public CarApiContext(DbContextOptions<CarApiContext> options)
			: base(options)
		{
		}

		public DbSet<Car> Cars { get; set; }

		public DbSet<Company> Companies { get; set; }
	}
}
using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace Client.DAL
{
	public class CarApiContext : DbContext
	{
		public CarApiContext(DbContextOptions<CarApiContext> options)
			: base(options)
		{
		}

		public DbSet<Car> Cars { get; set; }
		public DbSet<CarOnlineStatus> CarOnlineStatus { get; set; }
		public DbSet<CarCompany> CarCompany { get; set; }
		public DbSet<CarDisabledStatus> CarDisabledStatus { get; set; }
		public DbSet<Company> Companies { get; set; }
	}
}
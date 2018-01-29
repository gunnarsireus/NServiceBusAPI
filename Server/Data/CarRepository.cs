using Shared.Models;
using Shared.DAL;

namespace Server.Data
{
	public class CarRepository : Repository<Car>, ICarRepository
	{
		public CarRepository(CarApiContext context) : base(context)
		{
		}

		public CarApiContext CarApiContext => Context as CarApiContext;

	}
}

using Shared.Models;
using Shared.DAL;
using Microsoft.EntityFrameworkCore;

namespace Server.Data
{
	public class CompanyRepository : Repository<Company>, ICompanyRepository
	{
		public CompanyRepository(DbContext context) : base(context)
		{
		}

		public CarApiContext CarApiContext => Context as CarApiContext;

	}
}

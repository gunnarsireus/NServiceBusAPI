using NServiceBus;
using Shared.Models;
using Shared.Requests;
using Shared.Responses;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Utils
{
  public class Utils
  {
    public static async Task<GetCompaniesResponse> GetCompaniesResponseAsync(IMessageSession messageSession)
    {
      return await messageSession
          .Request<GetCompaniesResponse>(new GetCompaniesRequest()).ConfigureAwait(false);
    }

    public static async Task<GetCompanyResponse> GetCompanyResponseAsync(Guid id, IMessageSession messageSession)
    {
      return await messageSession
          .Request<GetCompanyResponse>(new GetCompanyRequest(id)).ConfigureAwait(false);
    }


    public static async Task<CreateCarResponse> CreateCarResponseAsync(Car car, IMessageSession messageSession)
    {
      return await messageSession
          .Request<CreateCarResponse>(new CreateCarRequest(car)).ConfigureAwait(false);
    }

    public static async Task<GetCarResponse> GetCarResponseAsync(Guid carId, IMessageSession messageSession)
    {
      return await messageSession
          .Request<GetCarResponse>(new GetCarRequest(carId)).ConfigureAwait(false);
    }

    public static async Task<DeleteCarResponse> DeleteCarResponseAsync(Guid carId, IMessageSession messageSession)
    {
      return await messageSession
          .Request<DeleteCarResponse>(new DeleteCarRequest(carId));
    }

    public static async Task<UpdateCarResponse> UpdateCarResponseAsync(Car car, IMessageSession messageSession)
    {
      return await messageSession
          .Request<UpdateCarResponse>(new UpdateCarRequest(car)).ConfigureAwait(false);
    }

    public static async Task<GetCarsResponse> GetCarsResponseAsync(IMessageSession messageSession)
    {
      try
      {
        return await messageSession
            .Request<GetCarsResponse>(new GetCarsRequest()).ConfigureAwait(false);
      }
      catch (Exception e)
      {
        // TODO: add logging?
        throw;
      }
    }
    public async Task<bool> CompanyExistsAsync(Guid id, IMessageSession messageSession)
    {
      var getCompaniesResponse = await GetCompaniesResponseAsync(messageSession).ConfigureAwait(false);
      var companies = getCompaniesResponse.Companies;

      return companies.Any(e => e.Id == id);
    }

    public async static Task<CreateCompanyResponse> CreateCompanyResponseAsync(Company company, IMessageSession messageSession)
    {
      return await messageSession
          .Request<CreateCompanyResponse>(new CreateCompanyRequest(company)).ConfigureAwait(false);
    }

    public async static Task<DeleteCompanyResponse> DeleteCompanyResponseAsync(Guid id, IMessageSession messageSession)
    {
      return await messageSession
          .Request<DeleteCompanyResponse>(new DeleteCompanyRequest(id)).ConfigureAwait(false);
    }

    public static async Task<UpdateCompanyResponse> UpdateCompanyResponseAsync(Company company, IMessageSession messageSession)
    {
      return await messageSession
          .Request<UpdateCompanyResponse>(new UpdateCompanyRequest(company)).ConfigureAwait(false);
    }
  }
}
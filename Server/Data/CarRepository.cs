using Shared.Models;
using Server.DAL;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace Server.Data
{
  public class CarRepository : ICarRepository
  {
    private readonly CarApiContext _context;
    private readonly SemaphoreSlim _asyncLock = new SemaphoreSlim(1, 1);
    public CarRepository(CarApiContext context)
    {
      _context = context;

    }

    public async Task<int> AddCarAsync(Car car)
    {
      await _asyncLock.WaitAsync();
      try
      {
        _context.Cars.Add(car);
        return await _context.SaveChangesAsync();
      }
      finally
      {
        _asyncLock.Release();
      }
    }

    public async Task<int> RemoveCarAsync(Guid Id)
    {
      await _asyncLock.WaitAsync();
      try
      {
        var foundCar = await _context.Cars.Where(c => c.Id == Id).SingleOrDefaultAsync() ?? throw new Exception($"Car with id '{Id}' not found in database");
        _context.Cars.Remove(foundCar);
        return await _context.SaveChangesAsync();
      }
      finally
      {
        _asyncLock.Release();
      }
    }

    public async Task<IEnumerable<Car>> GetAllCarsAsync()
    {
      return await _context.Cars.ToListAsync();
    }

    public async Task<Car> GetCarAsync(Guid Id)
    {
      return await _context.Cars.Where(c => c.Id == Id).SingleOrDefaultAsync();
    }

    public async Task<int> SaveContextChanges()
    {
      await _asyncLock.WaitAsync();
      try
      {
        return await _context.SaveChangesAsync();
      }
      finally
      {
        _asyncLock.Release();
      }
    }


    public async Task<int> UpdateCarAsync(Car car)
    {
      await _asyncLock.WaitAsync();
      try
      {
        // Wait until the database is not locked
        while (IsDatabaseLocked())
        {
          // Wait for a short duration before checking again
          await Task.Delay(100); // You can adjust the delay as needed
        }

        // Proceed with the update operation
        var original = await GetCarAsync(car.Id) ?? throw new Exception($"Car with id '{car.Id}' not found in database");
        _context.Entry(original).CurrentValues.SetValues(car);

        // Save the changes to the database
        return await _context.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
        // Handle concurrency conflicts
        throw;
      }
      finally
      {
        _asyncLock.Release();
      }
    }

    private bool IsDatabaseLocked()
    {
      using (var connection = _context.Database.GetDbConnection())
      {
        connection.Open();
        using (var command = connection.CreateCommand())
        {
          if (command == null)
          {
            // Handle the case where the command object is null
            return false;
          }

          command.CommandText = "PRAGMA lock_status";
          var result = command.ExecuteScalar();
          return result?.ToString() == "1"; // Check for null result as well
        }
      }
    }

  }
}

using NuGet.Protocol;
using RetroCarsWebApp.Models;

namespace RetroCarsWebApp.Services;

public class CarService
{
    private readonly String _filePath;

    public CarService(string filePath)
    {
        _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
    }

    public List<Car> GetCars()
    {
        var jsonString = FileInteractor.ReadAsync(_filePath).Result;
        if (string.IsNullOrEmpty(jsonString))
        {
            FileInteractor.WriteAsync(_filePath, "[]").Wait();
            jsonString = FileInteractor.ReadAsync(_filePath).Result;
        }
        return jsonString.FromJson<List<Car>>();
    }
    
    public Car CreateCar(Car car)
    {
        car.Id = Guid.NewGuid().ToString();
        FileInteractor.WriteAsync(_filePath, GetCars().Append(car).ToJson()).Wait();
        return car;
    }
    
    public IEnumerable<Car> GetUserCars(string userId) =>
        new List<Car>(GetCars().Where(car => car.OwnerId == userId).ToList());
    
    
    public IEnumerable<Car> GetStoreCars(string userId) => 
        GetCars().Where(car => car.OwnerId != userId && car.IsAvailableOnStore).ToList();

    public Car? GetCar(string id) =>
        GetCars().FirstOrDefault(car => car.Id == id);

    public void UpdateCar(Car car) =>
        FileInteractor
            .WriteAsync(_filePath, GetCars().Select(c => c.Id == car.Id ? car : c).ToJson())
            .Wait();

    public void DeleteCar(string id) =>
        FileInteractor.WriteAsync(_filePath, GetCars().Where(car => car.Id != id).ToJson()).Wait();
    
}
using System.Text.Json;
using MyVromVromMachineAPI.models;

namespace MyVromVromMachineAPI.Data;

public static class DataSeeder
{
    public static void SeedCars(DataContext context)
    {
        if (context.Cars.Any()) return;

        var path = Path.Combine(AppContext.BaseDirectory, "Data", "Seed", "cars.json");
        if (!File.Exists(path)) return;

        var json = File.ReadAllText(path);
        var cars = JsonSerializer.Deserialize<List<Car>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (cars == null || cars.Count == 0) return;

        context.Cars.AddRange(cars);
        context.SaveChanges();
    }
}
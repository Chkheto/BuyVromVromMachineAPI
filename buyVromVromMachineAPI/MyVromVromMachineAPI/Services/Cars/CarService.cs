using AutoMapper;
using MyVromVromMachineAPI.Data;
using MyVromVromMachineAPI.DTO.Requests.Cars;
using MyVromVromMachineAPI.models;

namespace MyVromVromMachineAPI.Services.Cars
{
    public class CarService : ICarService
    {
        private readonly DataContext _db;
        private readonly IMapper _mapper;

        public CarService(DataContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<bool> AddCar(AddCar request)
        {
            var car = _mapper.Map<Car>(request);

            if (request.ImageUrl is not null && request.ImageUrl.Length > 0)
            {
                car.ImageUrl = await UploadPicture(request.ImageUrl);
            }

            _db.Cars.Add(car);

            await _db.SaveChangesAsync();

            return true;
        }

        private async Task<string> UploadPicture(IFormFile pic)
        {
            var upload = Path.Combine(
                Directory.GetCurrentDirectory(),
                "uploads",
                "cars"
            );

            Directory.CreateDirectory(upload);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(pic.FileName)}";
            var path = Path.Combine(upload, fileName);

            await using var stream = new FileStream(path, FileMode.Create);
            await pic.CopyToAsync(stream);

            return $"/uploads/cars/{fileName}";
        }
    }
}

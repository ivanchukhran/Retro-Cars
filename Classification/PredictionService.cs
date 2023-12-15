namespace RetroCarsWebApp.Classification;
using Microsoft.ML;


public class PredictionService
{
    private readonly MLContext _context;
    private readonly CarModelTrainingService _carModelTrainingService;
    private readonly string _modelPath;

    public PredictionService(MLContext context, CarModelTrainingService carModelTrainingService, string modelPath)
    {
        _context = context;
        _carModelTrainingService = carModelTrainingService;
        _modelPath = modelPath;
    }

    public string PredictCarClass(Classification.Car car)
    {
        var model = _carModelTrainingService.LoadModel(_modelPath);
        var predictionEngine = _context.Model.CreatePredictionEngine<Classification.Car, Classification.CarPrediction>(model);
        var prediction = predictionEngine.Predict(car);

        return prediction.CarClass;
    }
}
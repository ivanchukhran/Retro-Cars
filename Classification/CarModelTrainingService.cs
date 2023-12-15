using Microsoft.ML;

namespace RetroCarsWebApp.Classification;

public class CarModelTrainingService
{
    private readonly MLContext _context;
    
    public CarModelTrainingService(MLContext context)
    {
        _context = context;
    }

    public ITransformer TrainModel(string dataPath)
    {
        var dataView = _context.Data.LoadFromTextFile<Classification.Car>(dataPath, hasHeader: true, separatorChar: ',');
        var pipeline = _context.Transforms.Conversion.MapValueToKey("CarClass")
            .Append(_context.Transforms.Categorical.OneHotEncoding("Buying"))
            .Append(_context.Transforms.Categorical.OneHotEncoding("Maint"))
            .Append(_context.Transforms.Categorical.OneHotEncoding("Doors"))
            .Append(_context.Transforms.Categorical.OneHotEncoding("Persons"))
            .Append(_context.Transforms.Categorical.OneHotEncoding("LugBoot"))
            .Append(_context.Transforms.Categorical.OneHotEncoding("Safety"))
            .Append(_context.Transforms.Concatenate("Features", "Buying", "Maint", "Doors", "Persons", "LugBoot", "Safety"))
            .Append(_context.Transforms.NormalizeMinMax("Features"))
            .Append(_context.Transforms.CopyColumns("Label", "CarClass"))
            .AppendCacheCheckpoint(_context);
        var trainer = _context.MulticlassClassification.Trainers.SdcaNonCalibrated()
            .Append(_context.Transforms.Conversion.MapKeyToValue("PredictedLabel"));
        var trainingPipeline = pipeline.Append(trainer);
        var model = trainingPipeline.Fit(dataView);
        return model;
    }

    public void SaveModel(ITransformer model, string dataCarModelZip)
    {
        _context.Model.Save(model, null, dataCarModelZip);
    }

    public ITransformer LoadModel(string modelZip)
    {
        return _context.Model.Load(modelZip, out _);
    }
}
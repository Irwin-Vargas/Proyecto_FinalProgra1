using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Trainers;

namespace Proyecto_FinalProgra1.MLModels
{
    public class DemandPredictionModel
{
    private readonly string _modelPath = "MLModels/demand_model.zip";
    private readonly MLContext _context;
    private ITransformer _model;

    public DemandPredictionModel()
    {
        _context = new MLContext();
        using var stream = File.OpenRead(_modelPath);
        _model = _context.Model.Load(stream, out var _);
    }

    public float Predict(SalesData input)
    {
        var predEngine = _context.Model.CreatePredictionEngine<SalesData, SalesPrediction>(_model);
        var prediction = predEngine.Predict(input);
        return prediction.PredictedQuantity;
    }
}
}
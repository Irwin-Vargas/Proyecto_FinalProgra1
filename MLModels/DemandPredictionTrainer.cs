using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML;

namespace Proyecto_FinalProgra1.MLModels
{
    public class DemandPredictionTrainer
{
    private readonly string _modelPath = "MLModels/demand_model.zip";

    public void Train(string dataPath)
    {
        var context = new MLContext();
        var data = context.Data.LoadFromTextFile<SalesData>(dataPath, hasHeader: true, separatorChar: ',');
        var pipeline = context.Transforms.Categorical.OneHotEncoding("ProductId")
            .Append(context.Transforms.Concatenate("Features", "Day", "ProductId"))
            .Append(context.Regression.Trainers.FastTree());
        var model = pipeline.Fit(data);
        context.Model.Save(model, data.Schema, _modelPath);
    }
}
}
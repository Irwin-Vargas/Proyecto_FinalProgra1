using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Trainers;

namespace Proyecto_FinalProgra1.MLModels
{
    public class DemandPredictionTrainer
    {
        private readonly string _modelPath = "MLModels/demand_model.zip";

        public void Train(string dataPath)
{
    Console.WriteLine("Entrenamiento iniciado...");
    if (!File.Exists(dataPath))
    {
        Console.WriteLine("No se encontró el archivo de datos: " + dataPath);
        throw new FileNotFoundException($"No se encontró el archivo de datos: {dataPath}");
    }
    else
    {
        Console.WriteLine("Archivo encontrado: " + dataPath);
    }

    // Asegúrate que la carpeta para guardar el modelo exista
    var modelDir = Path.GetDirectoryName(_modelPath);
    if (!Directory.Exists(modelDir))
    {
        Console.WriteLine("Creando directorio: " + modelDir);
        Directory.CreateDirectory(modelDir);
    }
    else
    {
        Console.WriteLine("Directorio ya existe: " + modelDir);
    }

    var context = new MLContext();
    Console.WriteLine("Cargando datos...");
    var data = context.Data.LoadFromTextFile<SalesData>(dataPath, hasHeader: true, separatorChar: ',');
    Console.WriteLine("Datos cargados. Construyendo pipeline...");

    var pipeline = context.Transforms.Categorical.OneHotEncoding("ProductId")
        .Append(context.Transforms.Concatenate("Features", "Day", "ProductId"))
        .Append(context.Regression.Trainers.FastTree());

    Console.WriteLine("Entrenando modelo...");
    var model = pipeline.Fit(data);

    Console.WriteLine("Guardando modelo...");
    context.Model.Save(model, data.Schema, _modelPath);

    Console.WriteLine($"¡Entrenamiento completado! Modelo guardado en: {_modelPath}");
}

    }
}
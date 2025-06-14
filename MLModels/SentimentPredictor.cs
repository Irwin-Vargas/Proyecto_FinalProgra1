using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML;
using System.IO;

namespace Proyecto_FinalProgra1.MLModels
{
    public class SentimentPredictor
    {
        private static string modelPath = "MLModels/sentimentModel.zip";
        private static MLContext mlContext = new MLContext();
        private static ITransformer model;
        private static PredictionEngine<SentimentData, SentimentPrediction> predEngine;

        static SentimentPredictor()
        {
            LoadModel();
        }

        private static void LoadModel()
        {
            if (File.Exists(modelPath))
            {
                DataViewSchema modelSchema;
                model = mlContext.Model.Load(modelPath, out modelSchema);
                predEngine = mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(model);
            }
        }

        public static SentimentPrediction Predict(string text)
        {
            var input = new SentimentData { Text = text };
            return predEngine.Predict(input);
        }
    }
}
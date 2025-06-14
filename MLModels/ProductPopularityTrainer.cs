using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Proyecto_FinalProgra1.Data;

namespace Proyecto_FinalProgra1.MLModels
{
    public class ProductPopularityTrainer
    {
        private readonly ApplicationDbContext _context;

        public ProductPopularityTrainer(ApplicationDbContext context)
        {
            _context = context;
        }

        public void TrainAndSaveModel()
        {
            var mlContext = new MLContext();

            // Carga datos reales desde la base de datos
            var menuItems = _context.MenuItem
                .Include(mi => mi.Reviews)
                .Include(mi => mi.OrderDetail)
                .ToList();

            var dataList = new List<ProductData>();

            foreach (var item in menuItems)
            {
                var avgRating = item.Reviews.Any() ? (float)item.Reviews.Average(r => r.Rating) : 0f;
                var reviewCount = item.Reviews.Count;
                var totalOrders = item.OrderDetail.Sum(od => od.Quantity);

                // Define si es popular
                bool isPopular = reviewCount >= 3 && totalOrders >= 20;

                dataList.Add(new ProductData
                {
                    ProductId = item.Id,
                    AverageRating = avgRating,
                    ReviewCount = reviewCount,
                    TotalOrders = totalOrders,
                    Label = isPopular
                });
            }

            var data = mlContext.Data.LoadFromEnumerable(dataList);

            var pipeline = mlContext.Transforms.Concatenate("Features",
                                            nameof(ProductData.AverageRating),
                                            nameof(ProductData.ReviewCount),
                                            nameof(ProductData.TotalOrders))
                            .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(
                                        labelColumnName: nameof(ProductData.Label),
                                        featureColumnName: "Features"));

            var model = pipeline.Fit(data);

            if (!Directory.Exists("MLModels"))
                Directory.CreateDirectory("MLModels");

            mlContext.Model.Save(model, data.Schema, "MLModels/product_popularity_model.zip");
        }
    }
}
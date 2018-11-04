using Microsoft.ML.Runtime.Data;
using Microsoft.ML.Runtime.Learners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TakeAFlight.Models.ML
{
    public class DestinationPredictionManager
    {
        private TransformerChain<KeyToValueTransform> Model;
        LocalEnvironment LocalEnvironment;
        public static DestinationPredictionManager Instance = new DestinationPredictionManager();

        private DestinationPredictionManager()
        {
        }

        public void Train()
        {
            LocalEnvironment = new LocalEnvironment();
            string dataPath = "Data//PassengerData.txt";
            var reader = new TextLoader(LocalEnvironment,
                new TextLoader.Arguments()
                {
                    Separator = ",",
                    HasHeader = true,
                    Column = new[]
                    {
                            new TextLoader.Column("Gender", DataKind.R4,0),
                            new TextLoader.Column("Nationality", DataKind.R4, 1),
                            new TextLoader.Column("year",DataKind.R4,2),
                            new TextLoader.Column("Label", DataKind.Text, 3)
                    }
                });

            IDataView trainingDataView = reader.Read(new MultiFileSource(dataPath));
            var pipeline = new TermEstimator(LocalEnvironment, "Label", "Label")
                             .Append(new ConcatEstimator(LocalEnvironment, "Features", "Gender", "Nationality", "year"))
                             .Append(new SdcaMultiClassTrainer(LocalEnvironment, new SdcaMultiClassTrainer.Arguments()))
                             .Append(new KeyToValueEstimator(LocalEnvironment, "PredictedLabel"));
             Model=pipeline.Fit(trainingDataView);

        }

        public Destination Predict(Passenger passenger,List<Destination> destinations)
        {
            var prediction = Model.MakePredictionFunction<PassengerData, DestPrediction>(LocalEnvironment).Predict(
                new PassengerData()
                {
                    Gender = (float)passenger.Gender,
                    Nationality = (float)passenger.Nationality,
                    year = passenger.DateOfBirth.Value.Year

                });
            return destinations.FirstOrDefault(obj => obj.Country.Equals(prediction.PredictedLabels));

        }


    }
}

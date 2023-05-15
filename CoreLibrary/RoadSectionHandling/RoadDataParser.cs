using CoreLibrary.RoadSectionHandling.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreLibrary.RoadSectionHandling
{
    public class RoadDataParser
    {

        private List<RoadPointModel> RawModel { get; set; }

        public RoadDataParser(List<RoadPointModel> rawData)
        {
            this.RawModel = rawData;
        }

        // Key -- location of first and last point in said part of road
        // Value -- road model with points
        private Dictionary<Tuple<LocationPoint, LocationPoint>, RoadPointModel> CreateModelDictionary()
        {

            Dictionary<Tuple<LocationPoint, LocationPoint>, RoadPointModel> modelDictionary = new Dictionary<Tuple<LocationPoint, LocationPoint>, RoadPointModel>();

            foreach(RoadPointModel model in RawModel)
            {
                Tuple<LocationPoint, LocationPoint> key = new Tuple<LocationPoint, LocationPoint>(model.Way.Points.First(), model.Way.Points.Last());

                if (!modelDictionary.ContainsKey(key))
                {
                    modelDictionary.Add(key, model);
                }

            }

            return modelDictionary;

        }

        // Create a list of connected points from sorted list
        private List<AbstractRoadModel> CreateRoadCurvitureModelDictionary(List<RoadPointModel> sortedByRoads)
        {

            Dictionary<LocationPoint, AbstractRoadModel> roadCurvOut = new Dictionary<LocationPoint, AbstractRoadModel>();

            AbstractRoadModel beforeCurvitureModel = null;

            foreach (RoadPointModel roadPart in sortedByRoads)
            {
                foreach(LocationPoint way in roadPart.Way.Points)
                {

                    if (roadCurvOut.ContainsKey(way)){
                        continue;
                    }

                    AbstractRoadModel currentCurvitureModel = (AbstractRoadModel)Activator.CreateInstance(typeof(AbstractRoadModel), new object[] { way });

                    currentCurvitureModel.Previous = beforeCurvitureModel == null ? null : new SegmentCurvitureChain(beforeCurvitureModel);


                    if (beforeCurvitureModel != null)
                    {

                        currentCurvitureModel.Distance = beforeCurvitureModel.Distance + MapParserUtils.CalculateDistanceBetweenPoints(beforeCurvitureModel.CurrentLocation
                            , currentCurvitureModel.CurrentLocation);
                        beforeCurvitureModel.Next = new SegmentCurvitureChain(currentCurvitureModel);

                    }

                    roadCurvOut.Add(way, currentCurvitureModel);

                    beforeCurvitureModel = currentCurvitureModel;

                }
            }

            return roadCurvOut.Values.ToList();
        }

        // Function creates a list of connected points
        // Function needs to determine all separate connected roads
        public List<AbstractRoadModel> GetConnectedWays()
        {
            Dictionary<Tuple<LocationPoint, LocationPoint>, RoadPointModel> modelDict = CreateModelDictionary();

            if (RawModel.Count == 0)
            {
                return null;
            }

            List<RoadPointModel> sortedByRoads = new List<RoadPointModel>
            {
                RawModel[0]
            };
            modelDict.Remove(new Tuple<LocationPoint, LocationPoint>(RawModel.First().Way.Points.First(), RawModel.First().Way.Points.Last()));

            while (true)
            {

                int countBef = modelDict.Count;


                if (modelDict.Count == 0)
                {
                    break;
                }

                // Add to Front

                while (true)
                {
                    if(modelDict.Count == 0)
                    {
                        break;
                    }

                    LocationPoint currentFirst = sortedByRoads.First().Way.Points.First();

                    // Check which record in dictionary has "Road segment end" same as start of current first segment
                    // Found segment is places on front
                    if(modelDict.Keys.Any(m => m.Item2.Equals(currentFirst)))
                    {
                        Tuple<LocationPoint, LocationPoint> currKey = modelDict.Keys.First(m => m.Item2.Equals(currentFirst));
                        sortedByRoads.Insert(0, modelDict[currKey]);
                        modelDict.Remove(currKey);
                    }
                    else
                    {
                        break;
                    }


                }

                // Add to back
                while (true)
                {
                    if (modelDict.Count == 0)
                    {
                        break;
                    }

                    LocationPoint currentLast = sortedByRoads.Last().Way.Points.Last();

                    // Check which record in dictionary has "Road segment start" same as end of current last segment
                    // Found segment is places last
                    if (modelDict.Keys.Any(m => m.Item1.Equals(currentLast)))
                    {
                        Tuple<LocationPoint, LocationPoint> currKey = modelDict.Keys.First(m => m.Item1.Equals(currentLast));
                        sortedByRoads.Add(modelDict[currKey]);
                        modelDict.Remove(currKey);
                    }
                    else
                    {
                        break;
                    }


                }

                if(modelDict.Count == countBef)
                {
                    Console.WriteLine("Unable to connect road segmets because road segments are not connected (check if parts of the road are not cut of)");
                    Console.WriteLine("Returning parts that were able to be connected together (Result might be skewed)");
                    break;
                }

            }
            
            return CreateRoadCurvitureModelDictionary(sortedByRoads);
        }

    }
}

using ConsoleApp2.RoadSectionHandling.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApp2.RoadSectionHandling
{
    public class RoadDataParser
    {

        private List<RoadPointModel> RawModel { get; set; }

        public RoadDataParser(List<RoadPointModel> rawData)
        {
            this.RawModel = rawData;
        }

        private Dictionary<Tuple<LocationPoint, LocationPoint>, RoadPointModel> CreateModelDictionary()
        {

            Dictionary<Tuple<LocationPoint, LocationPoint>, RoadPointModel> modelDictionary = new Dictionary<Tuple<LocationPoint, LocationPoint>, RoadPointModel>();

            foreach(RoadPointModel model in RawModel)
            {
                Tuple<LocationPoint, LocationPoint> key = new Tuple<LocationPoint, LocationPoint>(model.Way.Points[0], model.Way.Points[model.Way.Points.Count - 1]);
                modelDictionary.Add(key, model);
            }

            return modelDictionary;

        }

        private Dictionary<LocationPoint, RoadCurvitureModel> CreateRoadCurvitureModelDictionary(List<RoadPointModel> sortedByRoads)
        {

            Dictionary<LocationPoint, RoadCurvitureModel> roadCurvOut = new Dictionary<LocationPoint, RoadCurvitureModel>();

            RoadCurvitureModel beforeCurvitureModel = null;

            foreach (RoadPointModel roadPart in sortedByRoads)
            {
                foreach(LocationPoint way in roadPart.Way.Points)
                {

                    if (roadCurvOut.ContainsKey(way)){
                        continue;
                    }

                    RoadCurvitureModel currentCurvitureModel = new RoadCurvitureModel(way);
                    currentCurvitureModel.Previous = beforeCurvitureModel == null ? null : new SegmentCurvitureChain(beforeCurvitureModel);

                    if (beforeCurvitureModel != null)
                    {
                        beforeCurvitureModel.Next = new SegmentCurvitureChain(currentCurvitureModel);
                    }

                    roadCurvOut.Add(way, currentCurvitureModel);

                    beforeCurvitureModel = currentCurvitureModel;

                }
            }

            return roadCurvOut;
        }

        public Dictionary<LocationPoint, RoadCurvitureModel> GetConnectedWays()
        {
            Dictionary<Tuple<LocationPoint, LocationPoint>, RoadPointModel> modelDict = CreateModelDictionary();

            List<RoadPointModel> sortedByRoads = new List<RoadPointModel>();
            sortedByRoads.Add(RawModel[0]);
            modelDict.Remove(new Tuple<LocationPoint, LocationPoint>(RawModel[0].Way.Points[0], RawModel[0].Way.Points[RawModel[0].Way.Points.Count - 1]));


            while (true)
            {

                Console.WriteLine(modelDict.Count);

                if(modelDict.Count == 0)
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

                    LocationPoint currentFirst = sortedByRoads[0].Way.Points[0];

                    Console.WriteLine(currentFirst.Longitude + ":" + currentFirst.Latitude);

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

                    LocationPoint currentLast = sortedByRoads[sortedByRoads.Count - 1].Way.Points[sortedByRoads[sortedByRoads.Count - 1].Way.Points.Count - 1];

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

            }

            Dictionary<LocationPoint, RoadCurvitureModel> joinedDict = CreateRoadCurvitureModelDictionary(sortedByRoads);
           
            return joinedDict;
        }

    }
}

using CoreLibrary.RoadSectionHandling.Model;
using CoreLibrary.RoadSectionHandling.CollisionCalculators;
using System;
using System.Collections.Generic;
using System.Text;
using WebSocketLibrary.Models;
using NetTopologySuite.Index.KdTree;

namespace CoreLibrary.RoadSectionHandling.CollisionCalculators
{
    public interface ICollisionCalculatorImplementation
    {
        /**
         * Method determines if collision between cars happens based on currect behaviour
         * If method returns null, there is no collision
         * When CollisionInfo is returned, it should be provided to both vehicles
         */
        public AbstractRoadModel PerformCollisionCalculations(Tuple<VehicleData, AbstractRoadModel> vehicle1Tuple, Tuple<VehicleData, AbstractRoadModel> vehicle2Tuple,
            KdTree<AbstractRoadModel> currectRoadModel);

        public double CalculateTTC();

        public bool CollisionOccured();

        public CollisionSeverity GetCollisionSeverity();

        public enum CollisionSeverity
        {
            MEDIUM,
            SEVERE
        }

        /**
         * We calculate for head-on collisions but we can extend this implementation later
         */
        public enum CollisionType
        {

        }

        /**
         * Create warning message with correct content based on calculation output
         */
        public NotifyMessage CreateNotificationMessage(VehicleData host, VehicleData target, AbstractRoadModel collisionPoint, string roadAttr)
        {
            NotifyMessage msg = new NotifyMessage();

            msg.VehicleId = host.Id;
            msg.Level = GetCollisionSeverity().Equals(CollisionSeverity.MEDIUM) ? NotificationLevel.warning : NotificationLevel.danger;
            msg.Content = new HeadCollisionContent(CalculateTTC(), target.Id);

            if(collisionPoint != null)
            {
                if (IsMaxSpeedExceeded(host, msg, collisionPoint.MaxSpeed))
                {
                    IsVehicleAbleToBrake(host, roadAttr, msg);
                }
            }


            return msg;
        }

        /**
         * Modify warning message based on vehicle speed and max speed at meeting point
         */
        public bool IsMaxSpeedExceeded(VehicleData vehicle, NotifyMessage msg, double maxAllowedSpeed)
        {
            if (vehicle.Speed > maxAllowedSpeed)
            {
                Logger.GetLogger().WriteLine($"Vehicle {vehicle.Id} speed ({vehicle.Speed}) has exceeded the max possible speed ({maxAllowedSpeed}) to traverse curve");
                msg.Level = NotificationLevel.danger;
                msg.Content.MaxSpeedExceededBy = vehicle.Speed - maxAllowedSpeed;

                return true;
            }

            return false;
        }

        /**
         * Determine if braking distance is smaller than distance to meeting point
         */
        public bool IsVehicleAbleToBrake(VehicleData vehicle, string roadRef, NotifyMessage msg)
        {

            double breakingDistance = BrakingDistanceCalculatorUtils.CalculateBrakingDistance(vehicle, roadRef);
            double distanceToColl = vehicle.Speed * CalculateTTC();

            if(distanceToColl < breakingDistance)
            {
                Logger.GetLogger().WriteLine($"Vehicle {vehicle.Id} breaking distance ({breakingDistance}) was higher than distance to collision ({distanceToColl})");
                msg.Content.BrakingDistanceDiff = breakingDistance - distanceToColl;
            }


            return breakingDistance < distanceToColl;

        }
    }
}

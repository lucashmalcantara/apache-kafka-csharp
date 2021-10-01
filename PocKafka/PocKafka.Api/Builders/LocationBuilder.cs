using AutoBogus;
using PocKafka.Infrastructure.Kafka.Models;
using System;

namespace PocKafka.Api.Builders
{
    public class LocationBuilder : AutoFaker<Location>
    {
        private readonly string[] _availableEvents = new string[] { "motionchange", "providerchange", "geofence", "heartbeat" };
        private readonly string[] _availableActivities = new string[] { "motionchange", "providerchange", "geofence", "heartbeat" };

        public LocationBuilder()
        {
            RuleFor(x => x.uuid, Guid.NewGuid().ToString())
               .RuleFor(x => x.capturedAt, DateTimeOffset.Now.ToUnixTimeMilliseconds())
               .RuleFor(x => x.backendArrivalDate, DateTimeOffset.Now.ToUnixTimeMilliseconds())
               .RuleFor(x => x.@event, faker => _availableEvents[faker.Random.Int(0, _availableEvents.Length - 1)])
               .RuleFor(x => x.isMoving, faker => faker.Random.Bool())
               .RuleFor(x => x.coordinates, faker => new Coordinates
               {
                   latitude = faker.Random.Double(-90.0000000d, 90.0000000d),
                   longitude = faker.Random.Double(-180.0000000, 180.0000000d),
                   accuracy = faker.Random.Double(),
                   speed = faker.Random.Double(),
                   heading = faker.Random.Double(),
                   altitude = faker.Random.Double(),
               })
               .RuleFor(x => x.activity, faker => new Activity
               {
                   type = _availableActivities[faker.Random.Int(0, _availableActivities.Length - 1)],
                   confidence = faker.Random.Float()
               })
               .RuleFor(x => x.battery, faker => new Battery
               {
                   isCharging = faker.Random.Bool(),
                   level = faker.Random.Float()
               })
               .RuleFor(x => x.odometer, faker => faker.Random.Double())
               .RuleFor(x => x.mock, faker => faker.Random.Bool() ? (bool?)null : faker.Random.Bool())
               .RuleFor(x => x.extras, faker => new Extras
               {
                   clientId = faker.Random.Number(min: 0, max: 99999999).ToString(),
                   licensePlate = faker.Random.String2(length: 7).ToUpper(),
                   vin = faker.Random.String2(length: 17).ToUpper()
               });
            ;
        }
    }
}
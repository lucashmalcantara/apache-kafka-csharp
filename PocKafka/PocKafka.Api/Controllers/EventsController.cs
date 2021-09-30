using Confluent.Kafka;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PocKafka.Api.Builders;
using PocKafka.Api.Models.Events;
using PocKafka.Infrastructure.Kafka.Interfaces;
using PocKafka.Infrastructure.Kafka.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PocKafka.Api.Controllers
{
    [ApiController]
    [Route("events")]
    public class EventsController : ControllerBase
    {
        private readonly ILogger<EventsController> _logger;
        private readonly IKafkaPublisher<string, Location> _kafkaPublisher;
        private readonly string _kafkaTopic;
        private readonly LocationBuilder _locationBuilder;

        public EventsController(ILogger<EventsController> logger, IKafkaPublisher<string, Location> kafkaPublisher)
        {
            _logger = logger;
            _kafkaPublisher = kafkaPublisher;
            _kafkaTopic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");
            _locationBuilder = new LocationBuilder();
        }

        [HttpGet("healthcheck")]
        public IActionResult Get()
        {
            return Ok($"{DateTime.Now:dd/MM/yyyy HH:mm:ss.fff} - Healthy");
        }

        /// <summary>
        /// Publish a new event in the topic. 
        /// </summary>
        /// <param name="requestModel">Quantity of locations to be generated.</param>
        /// <returns>HTTP Status Code corresponding to the completion of the execution.</returns>
        /// <response code="201">A list of message keys.</response>
        /// <response code="400">Some error predicted in the business rule.</response>
        /// <response code="500">Unhandled error in the business rule.</response>
        [HttpPost("produce-random-locations")]
        [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ProduceRandomLocationsAsync([FromBody] ProduceRandomLocationsRequestModel requestModel)
        {
            try
            {
                var locations = _locationBuilder.Generate(requestModel.Quantity);

                var keys = new List<string>();

                foreach (var location in locations)
                {
                    var key = Guid.NewGuid().ToString();
                    var deliveryResult = await _kafkaPublisher.ProduceAsync(_kafkaTopic, key, location, new CancellationToken());

                    keys.Add(key);

                    _logger.LogInformation($"Entregou um objeto '{deliveryResult.Message.Value}' com a key '{deliveryResult.Message.Key}' em '{deliveryResult.TopicPartition} e offset '{deliveryResult.Offset}' e status '{deliveryResult.Status}'");
                }

                return Created("", keys);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.ToString());
            }
        }

        /// <summary>
        /// Publish a new event in the topic. 
        /// </summary>
        /// <param name="location">Location</param>
        /// <returns>HTTP Status Code corresponding to the completion of the execution.</returns>
        /// <response code="201">Produced message information.</response>
        /// <response code="400">Some error predicted in the business rule.</response>
        /// <response code="500">Unhandled error in the business rule.</response>
        [HttpPost]
        [ProducesResponseType(typeof(DeliveryResult<string, Location>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ProduceAsync([FromBody] Location location)
        {
            try
            {
                var key = Guid.NewGuid();

                var deliveryResult = await _kafkaPublisher.ProduceAsync(_kafkaTopic, key.ToString(), location, new CancellationToken());

                _logger.LogInformation($"Entregou um objeto '{deliveryResult.Message.Value}' com a key '{deliveryResult.Message.Key}' em '{deliveryResult.TopicPartition} e offset '{deliveryResult.Offset}' e status '{deliveryResult.Status}'");

                return Created("", deliveryResult);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.ToString());
            }
        }
    }
}

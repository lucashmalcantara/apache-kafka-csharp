namespace PocKafka.Api.Models.Events
{
    public class ProduceRandomLocationsRequestModel
    {
        /// <summary>
        /// Quantity of random Locations to produce.
        /// </summary>
        /// <example>10</example>
        public int Quantity { get; set; } = 1;
    }
}

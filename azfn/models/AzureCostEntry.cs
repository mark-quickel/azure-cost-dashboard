using Microsoft.Azure.Documents;
using Newtonsoft.Json;

namespace AzureCostDashboardFunction
{
    //[Serializable]
    public record AzureCostEntry {
        [JsonProperty("id")]
        public string Id  { get; set; }
        public string SubscriptionName  { get; set; }
        //[JsonProperty(PropertyName = "SubscriptionGuid")]
        public string SubscriptionGuid  { get; set; }
        public DateTime Date  { get; set; }
        public string ResourceGuid  { get; set; }
        public string ServiceName  { get; set; }
        public string ServiceType  { get; set; }
        public string ServiceRegion  { get; set; }
        public string ServiceResource  { get; set; }
        public decimal Quantity  { get; set; }
        public decimal Cost { get; set; }
    }
}
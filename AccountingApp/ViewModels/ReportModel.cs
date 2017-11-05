using Newtonsoft.Json;
using System.Collections.Generic;

namespace AccountingApp.ViewModels
{
    public class BasicReportModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public decimal Value { get; set; }
    }

    public abstract class BaseReportModel
    {
        [JsonProperty("names")]
        public IEnumerable<string> Names { get; set; }

        [JsonProperty("values")]
        public IEnumerable<decimal> Values { get; set; }
    }

    /// <summary>
    /// 饼图报告Model
    /// </summary>
    public class PieReportModel : BaseReportModel
    {
        [JsonProperty("data")]
        public IEnumerable<BasicReportModel> Data { get; set; }
    }
}
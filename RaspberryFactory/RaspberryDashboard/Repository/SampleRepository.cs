using RaspberryDashboard.Model;
using RaspberryDashboard.Pages;
using System;
using System.Net.Http.Json;
using System.Xml.Linq;

namespace RaspberryDashboard.Repository {
    public class SampleRepository {
        private HttpClient _client;
        private ILogger<SampleRepository> _logger;

        public SampleRepository(HttpClient client, ILogger<SampleRepository> logger) {
            _client = client;
            _logger = logger;
        }

        static List<Sample> _Samples = new List<Sample>() { new Sample() { Name = "A", Active = true }, new Sample() { Name = "B", Active = false } };

        public async Task<List<Sample>> LoadSamplesAsync() {

            //await _client.GetFromJsonAsync<List<Sample>>("/SampleData?test?true");

            return await Task.FromResult(new List<Sample>(_Samples));

        }

        public async Task SaveSamplesAsync(Sample data) {
            _Samples.Add(data);
            await Task.CompletedTask;
        }

    }
}

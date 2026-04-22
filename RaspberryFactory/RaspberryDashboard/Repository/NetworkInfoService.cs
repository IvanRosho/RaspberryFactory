namespace RaspberryDashboard.Repository {
    public class NetworkInfoService {
        private readonly HttpClient _http;

        public string ExternalIp { get; private set; } = "";

        public NetworkInfoService(HttpClient http) {
            _http = http;
        }

        public async Task InitializeAsync() {
            ExternalIp = await _http.GetStringAsync("https://api.ipify.org");
        }
    }
}

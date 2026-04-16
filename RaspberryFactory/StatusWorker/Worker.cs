using CommonFiles.TelemertyDTO;
using Logger;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StatusWorker {

    public class Worker {
        private readonly LogService _logger;
        private readonly MqttConfig _mqttConfig;
        private IMqttClient? _mqttClient;

        public Worker(LogService logger, IOptions<MqttConfig> mqttOptions) {
            _logger = logger;
            _mqttConfig = mqttOptions.Value;
        }

        public async Task RunAsync() {
            while (true) {
                await _logger.LogAsync("Starting metrics collection at {time}");

                // 1. Считываем системные метрики
                var cpu = ReadCpuLoad();
                var ram = ReadRamUsage();
                var sd = ReadSdCardInfo();

                // 2. Считываем процессы
                var topProcesses = ReadTopCpuProcesses();

                // 3. Считываем статусы сервисов
                var services = ReadServiceStatuses();

                // 4. Считываем статусы Docker контейнеров
                var docker = ReadDockerContainers();

                // 5. Считываем список прослушиваемых портов
                var ports = ReadListeningPorts();

                // 6. Собираем JSON
                var json = BuildJson(cpu, ram, sd, topProcesses, services, docker, ports);

                // 7. Публикуем в MQTT
                await PublishToMqttAsync(json);

                await _logger.LogAsync("Metrics collection finished");

                await Task.Delay(TimeSpan.FromMinutes(5));
            }
        }
        private async Task<double> ReadCpuLoad() {
            (long idle, long total) ReadCpuStat() {
                var line = File.ReadLines("/proc/stat").First(l => l.StartsWith("cpu "));
                var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                long user = long.Parse(parts[1]);
                long nice = long.Parse(parts[2]);
                long system = long.Parse(parts[3]);
                long idle = long.Parse(parts[4]);
                long iowait = long.Parse(parts[5]);
                long irq = long.Parse(parts[6]);
                long softirq = long.Parse(parts[7]);
                long steal = long.Parse(parts[8]);
                long total = user + nice + system + idle + iowait + irq + softirq + steal;
                return (idle, total);
            }
            var (idle1, total1) = ReadCpuStat();
            await Task.Delay(200); // небольшая пауза для корректного измерения
            var (idle2, total2) = ReadCpuStat();

            var idleDelta = idle2 - idle1;
            var totalDelta = total2 - total1;

            if (totalDelta == 0)
                return 0;

            double cpuUsage = 1.0 - ((double)idleDelta / totalDelta);
            return Math.Round(cpuUsage * 100, 2);
        }
        private RamInfo ReadRamUsage() {
            var memInfo = File.ReadAllLines("/proc/meminfo");

            long totalKb = 0;
            long availableKb = 0;
            long ParseKb(string line) {
                var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                return long.Parse(parts[1]); // kB
            }
            foreach (var line in memInfo) {
                if (line.StartsWith("MemTotal:")) {
                    totalKb = ParseKb(line);
                }
                else if (line.StartsWith("MemAvailable:")) {
                    availableKb = ParseKb(line);
                }

                if (totalKb > 0 && availableKb > 0)
                    break;
            }
            long usedKb = totalKb - availableKb;
            return new RamInfo {
                TotalMb = (int)(totalKb / 1024),
                FreeMb = (int)(availableKb / 1024),
                UsedMb = (int)(usedKb / 1024)
            };
        }
        private SdCardInfo ReadSdCardInfo() {
            var result = new SdCardInfo();
            // run df -m
            var process = new System.Diagnostics.Process {
                StartInfo = new System.Diagnostics.ProcessStartInfo {
                    FileName = "/bin/bash",
                    Arguments = "-c \"df -m | grep mmcblk\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();
            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines) {
                // /dev/mmcblk0p2 29000 12000 17000 41% /
                var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 6)
                    continue;
                var partition = new SdPartition {
                    Filesystem = parts[0],
                    SizeMb = int.Parse(parts[1]),
                    UsedMb = int.Parse(parts[2]),
                    AvailableMb = int.Parse(parts[3]),
                    UsedPercent = int.Parse(parts[4].TrimEnd('%')),
                    Mount = parts[5]
                };
                result.Partitions.Add(partition);
            }
            return result;
        }
        private List<ProcessInfo> ReadTopCpuProcesses() {
            var result = new List<ProcessInfo>();
            var process = new System.Diagnostics.Process {
                StartInfo = new System.Diagnostics.ProcessStartInfo {
                    FileName = "/bin/bash",
                    Arguments = "-c \"ps -eo pid,comm,%cpu,%mem --sort=-%cpu | head -n 6\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();
            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines.Skip(1)) {
                var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 4)
                    continue;
                var info = new ProcessInfo {
                    Pid = int.Parse(parts[0]),
                    Name = parts[1],
                    Cpu = double.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture),
                    RamMb = double.Parse(parts[3], System.Globalization.CultureInfo.InvariantCulture)
                };
                result.Add(info);
            }
            return result;
        }
        private Dictionary<string, ServiceStatus> ReadServiceStatuses() {
            var result = new Dictionary<string, ServiceStatus>();
            var services = new List<(string Key, string Unit, int Port)>
            {
                ("mosquitto", "mosquitto", 1883),
                ("wireguard", "wg-quick@wg0", 81820),
                ("wg_easy", "", 51821),
                ("postgres", "postgresql", 5432)
            };
            string ReadSystemdStatus(string unit) {
                try {
                    var process = new System.Diagnostics.Process {
                        StartInfo = new System.Diagnostics.ProcessStartInfo {
                            FileName = "/bin/bash",
                            Arguments = $"-c \"systemctl is-active {unit}\"",
                            RedirectStandardOutput = true,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        }
                    };
                    process.Start();
                    var output = process.StandardOutput.ReadToEnd().Trim();
                    process.WaitForExit();
                    return string.IsNullOrWhiteSpace(output) ? "unknown" : output;
                }
                catch {
                    return "unknown";
                }
            }
            bool CheckPortOpen(int port) {
                try {
                    var process = new System.Diagnostics.Process {
                        StartInfo = new System.Diagnostics.ProcessStartInfo {
                            FileName = "/bin/bash",
                            Arguments = $"-c \"ss -tuln | grep :{port} \"",
                            RedirectStandardOutput = true,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        }
                    };
                    process.Start();
                    var output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    return !string.IsNullOrWhiteSpace(output);
                }
                catch {
                    return false;
                }
            }
            foreach (var svc in services) {
                string status = "unknown";
                if (!string.IsNullOrWhiteSpace(svc.Unit)) {
                    status = ReadSystemdStatus(svc.Unit);
                }
                bool portOpen = CheckPortOpen(svc.Port);

                result[svc.Key] = new ServiceStatus {
                    Status = status,
                    Port = svc.Port,
                    PortOpen = portOpen
                };
            }

            return result;
        }
        private List<DockerContainerInfo> ReadDockerContainers() {
            var result = new List<DockerContainerInfo>();
            var process = new System.Diagnostics.Process {
                StartInfo = new System.Diagnostics.ProcessStartInfo {
                    FileName = "/bin/bash",
                    Arguments = "-c \"docker ps --format '{{json .}}'\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();
            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines) {
                try {
                    var raw = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(line);
                    if (raw == null)
                        continue;
                    var info = new DockerContainerInfo {
                        Id = raw.GetValueOrDefault("ID", ""),
                        Name = raw.GetValueOrDefault("Names", ""),
                        Image = raw.GetValueOrDefault("Image", ""),
                        Status = raw.GetValueOrDefault("Status", ""),
                        Ports = raw.GetValueOrDefault("Ports", ""),
                        Created = raw.GetValueOrDefault("CreatedAt", "")
                    };
                    result.Add(info);
                }
                catch {
                    // ignore incorrect
                }
            }
            return result;
        }
        private List<ListeningPort> ReadListeningPorts() {
            var result = new List<ListeningPort>();
            var process = new System.Diagnostics.Process {
                StartInfo = new System.Diagnostics.ProcessStartInfo {
                    FileName = "/bin/bash",
                    Arguments = "-c \"ss -tulnp\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();
            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            int ExtractPort(string addressPort) {
                // "0.0.0.0:22" or "[::]:1883"
                var idx = addressPort.LastIndexOf(':');
                if (idx == -1)
                    return 0;
                var portStr = addressPort[(idx + 1)..];
                return int.TryParse(portStr, out var port) ? port : 0;
            }
            string ExtractProcessName(string line) {
                // search users:(("sshd",pid=612,fd=3))
                var start = line.IndexOf("((");
                if (start == -1)
                    return "";
                var end = line.IndexOf(",", start);
                if (end == -1)
                    return "";

                return line.Substring(start + 2, end - (start + 2)).Trim('"');
            }
            foreach (var line in lines.Skip(1)) {
                try {
                    var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length < 5)
                        continue;
                    string protocol = parts[0];
                    // "0.0.0.0:22"
                    string addressPort = parts[4];
                    int port = ExtractPort(addressPort);
                    // users:(("sshd",pid=612,fd=3))
                    string processName = ExtractProcessName(line);
                    result.Add(new ListeningPort {
                        Protocol = protocol,
                        Port = port,
                        Process = processName
                    });
                }
                catch {
                    // ignore incorrect
                }
            }
            return result;
        }
        private string BuildJson(double cpu, RamInfo ram, SdCardInfo sd, List<ProcessInfo> topProcesses, Dictionary<string, ServiceStatus> services, List<DockerContainerInfo> docker, List<ListeningPort> ports) {
            var telemetry = new RootTelemetry {
                System = new SystemInfo {
                    CpuLoad = cpu,
                    Ram = ram,
                    SdCard = sd
                },
                TopProcesses = topProcesses,
                Services = services,
                DockerContainers = docker,
                ListeningPorts = ports
            };
            var options = new JsonSerializerOptions {
                WriteIndented = false,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            return JsonSerializer.Serialize(telemetry, options);
        }
        private async Task PublishToMqttAsync(string json) {
            try {
                if (_mqttClient == null) {
                    var factory = new MqttClientFactory();
                    _mqttClient = factory.CreateMqttClient();
                    var builder = new MqttClientOptionsBuilder()
                        .WithTcpServer(_mqttConfig.Host, _mqttConfig.Port)
                        .WithCleanSession();
                    if (!string.IsNullOrWhiteSpace(_mqttConfig.Username)) {
                        builder = builder.WithCredentials(_mqttConfig.Username, _mqttConfig.Password);
                    }
                    var options = builder.Build();
                    if (!_mqttClient.IsConnected)
                        await _mqttClient.ConnectAsync(options);
                }
                var message = new MqttApplicationMessageBuilder()
                    .WithTopic(_mqttConfig.Topic)
                    .WithPayload(json)
                    .Build();
                await _mqttClient.PublishAsync(message);
                await _logger.LogAsync($"Published MQTT message ({json.Length} bytes)");
            }
            catch (Exception ex) {
                await _logger.LogAsync($"MQTT publish error: {ex.Message}");
            }
        }
    }
}

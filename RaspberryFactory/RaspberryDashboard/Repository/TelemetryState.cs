using CommonFiles.TelemetryDTO;

namespace RaspberryDashboard.Repository {
    public class TelemetryState {
        public RaspberryTelemetry? RaspberryData { get; private set; }
        public ServicesTelemetry? ServicesData { get; private set; }

        public event Action? OnChange;

        public void UpdateSystem(RaspberryTelemetry dto) {
            RaspberryData = dto;
            OnChange?.Invoke();
        }

        public void UpdateServices(ServicesTelemetry dto) {
            ServicesData = dto;
            OnChange?.Invoke();
        }
    }

}

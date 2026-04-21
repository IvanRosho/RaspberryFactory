using RaspberryDashboard.Model;

namespace RaspberryDashboard.Logic {
    public class SampleLogic {

        public bool Validate(Sample sample) {
            return sample.Name != "Test";
        }
    }
}

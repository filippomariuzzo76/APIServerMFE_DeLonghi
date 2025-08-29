/***************************************************************************************
 * 
 * *************************************************************************************/
namespace APIServerMFE_DeLonghi.Models
{
    public class ArgumentDto
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class PhaseDto
    {
        public string MissionId { get; set; }
        public List<ArgumentDto> Arguments { get; set; }
    }

    public class SerialOrderDto
    {
        public string Id { get; set; }
        public string Priority { get; set; }
        public string EarliestStartTime { get; set; }
        public string RobotId { get; set; }
        public List<PhaseDto> Phases { get; set; }
    }

    public class SerialOrderRequest
    {
        public string TargetUrl { get; set; }   // es: http://192.168.60.170:5050/api/v1/serial-order
        public SerialOrderDto SerialOrder { get; set; }
    }
}
namespace APIServerMFE_DeLonghi.Models
{
    public class AppSettings
    {
        public string MFEUrl { get; set; }
        public string WebHookUrl { get; set; }
        public string WebHookPort { get; set; }
        public string XApiKey { get; set; }
        public bool Subscription_DeleteAllEvents { get; set; }
        public bool Subscription_AlertEvent { get; set; }
        public bool Subscription_RobotRuntimeEvent { get; set; }
        public bool Subscription_SerialOrderStatusEvent { get; set; }
        public bool Subscription_ErrorEvent { get; set; }
        public bool Subscription_RobotIdentityEvent { get; set; }
        public bool Subscription_RobotStateEvent { get; set; }
        public string Mission_WaitTest_id { get; set; }
        public string Mission_WaitParameterTest_id { get; set; }
        public string Mission_Transfer_With_Barcode_To_Warehouse_Ewm_id { get; set; }      
        public string ExportDirectoryName { get; set; }
        public string ExportFileMissionName { get; set; }
        public string ExportFileAutochargingName { get; set; }
        public string ExportFileDistanceName { get; set; }
        public Dictionary<string, string> PLCRegister { get; set; } = new();

        // Metodo helper per accedere con indice 1..10
        public string GetPLCRegister(int index)
        {
            if (index < 1) throw new ArgumentOutOfRangeException(nameof(index));
            var key = $"PLCRegister{index}";
            return PLCRegister.ContainsKey(key) ? PLCRegister[key] : null;
        }
    }
}
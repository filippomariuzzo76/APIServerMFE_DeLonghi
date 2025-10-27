namespace APIServerMFE_DeLonghi
{
    /**********************************************************************************************
     * 
     * *******************************************************************************************/
    public class Enumerators
    {
        public enum MissionState
        {
            Created,
            Waiting, // In attesa
            Pending, // In sospeso
            Outbound,
            Executing,// In esecuzione
            Finished,
            Aborting,
            Aborted, // Annullato
            Paused,
            WaitingForInput
        }

        public enum OrderType
        {
            Ui,  //User Interface
            Api, //Api 
            Evacuation,
            Autocharging,
            Autostaging,
            GoTo
        }

    }//class

    /************************************************************************************************
     * 
     * *********************************************************************************************/
    //public static class RobotEnumerators
    //{
    //    public static readonly Dictionary<string, string> Robots = new Dictionary<string, string>
    //    {
    //      { "MiR_207000102", "763f9d9e-5d67-4dd0-a43a-ee7349f88c4a"},
    //      //{ "MiR_207000036", "f792c417-f62c-42cb-bff7-bb21d24d2g60" },
    //      //{ "MiR_207000037", "a671d218-c73d-43dc-cff8-cc32e35e3h70" }
    //    };

    //    // Metodo per ottenere l'ID dato il nome del robot
    //    public static string GetRobotId(string robotName)
    //    {
    //        return Robots.TryGetValue(robotName, out var id) ? id : null;
    //    }

    //    // Metodo per ottenere il nome dato l'ID del robot
    //    public static string GetRobotName(string robotId)
    //    {
    //        return Robots.FirstOrDefault(x => x.Value == robotId).Key ?? "Robot not found";
    //    }
    //}

    public class RobotEnumerators
    {
        private readonly Dictionary<string, string> _robots;

        public RobotEnumerators(IConfiguration configuration)
        {
            // Legge la sezione "Robots" da appsettings.json e la converte in Dictionary
            _robots = configuration
                .GetSection("Robots")
                .GetChildren()
                .ToDictionary(x => x.Key, x => x.Value);
        }

        public string GetRobotId(string robotName)
        {
            return _robots.TryGetValue(robotName, out var id) ? id : null;
        }

        public string GetRobotName(string robotId)
        {
            return _robots.FirstOrDefault(x => x.Value == robotId).Key ?? "Robot not found";
        }
    }


}//namespace

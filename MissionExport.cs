namespace APIServerMFE_DeLonghi
{
    /********************************************************************************************************************
     * 
     * ******************************************************************************************************************/
    public class MissionMir
    {
        // Type of Mission (Presa o Vuoto)
        public string TypeMission { get; set; }

        // Production Line (ad esempio Linea1, Linea2, Linea10, ecc.)
        public string ProductionLine { get; set; }

        // Position of Line (ad esempio _1, _2, _3)
        public string ProductionSubLine { get; set; }

        // Proprietà per l'ID (UUID)
        public Guid IdMission { get; set; }

        // Costruttore
        public MissionMir(string typeMission, string productionLine, string productionSubLine, Guid idMission)
        {
            TypeMission = typeMission;
            ProductionLine = productionLine;
            ProductionSubLine = productionSubLine;
            IdMission = idMission;
        }
       
        // Metodo per rappresentare l'oggetto come stringa
        public override string ToString()
        {
            return $"{TypeMission} {ProductionLine}{ProductionSubLine} {IdMission}";
        }
    }//class

    /********************************************************************************************************************
     * Mission anager Class
     * 
     * ******************************************************************************************************************/
    public class MissionMirManager
    {
        public List<MissionMir> ListMissions { get; private set; }

        public MissionMirManager()
        {
            // Inizializzazione della lista delle missioni
            ListMissions = new List<MissionMir>
            {
                new MissionMir("Mission_WaitTest_id", "", "", Guid.Parse("905bc823-2e36-4710-9dfd-e8523b6cd985")),
                new MissionMir("Mission_WaitParameterTest_id", "", "", Guid.Parse("a8c8eeb9-efa6-4966-abc9-b48968960454"))
             };
        }

        public MissionMir GetMissionById(string idMission) 
        {
            foreach (var mission in ListMissions)
            {
                if (mission.IdMission == Guid.Parse(idMission.Trim()))
                { 
                  return mission;
                }
            }
            return null;
        }
    }//class
}//namespace




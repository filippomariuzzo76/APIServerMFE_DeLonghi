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
                new MissionMir("Prelievo", "Linea1", "_2", Guid.Parse("8976d1d6-1d93-4e98-a3c1-43a5a179e189")),
                new MissionMir("Prelievo", "Linea1", "_3", Guid.Parse("412f0afc-66fa-4526-a458-cf820cb055e9")),
                new MissionMir("Prelievo", "Linea2", "_1", Guid.Parse("6369f7bf-2d8e-4f6e-b8db-2bdf99c05424")),
                new MissionMir("Prelievo", "Linea2", "_2", Guid.Parse("36279797-6571-4d9a-a33f-efa544333ea3")),
                new MissionMir("Prelievo", "Linea2", "_3", Guid.Parse("f77e4df1-97f5-45cd-90da-76fe85a92283")),
                new MissionMir("Prelievo", "Linea3", "_1", Guid.Parse("d695924c-dfe2-4b58-afcc-fed238869095")),
                new MissionMir("Prelievo", "Linea3", "_2", Guid.Parse("e5dbb35d-244a-4eff-a3f6-d0e93953c9be")),
                new MissionMir("Prelievo", "Linea3", "_3", Guid.Parse("c9dc0330-4802-4925-9e7a-b4f05d39cd6c")),
                new MissionMir("Prelievo", "Linea4", "_1", Guid.Parse("1bb75d91-3260-4578-af0e-7153d6b702bd")),
                new MissionMir("Prelievo", "Linea4", "_2", Guid.Parse("9387ecd3-1cdb-4603-b492-ec40e8821cdb")),
                new MissionMir("Prelievo", "Linea4", "_3", Guid.Parse("1e5103a3-7367-425a-b9ea-7f857a40c8a1")),
                new MissionMir("Prelievo", "Linea5", "_1", Guid.Parse("4587f4e8-fb77-4904-ad91-dca7adb7d138")),
                new MissionMir("Prelievo", "Linea5", "_2", Guid.Parse("1187f3fb-dfd3-41a1-94e4-b3f825febe5a")),
                new MissionMir("Prelievo", "Linea5", "_3", Guid.Parse("e85b4786-9bf7-4229-84d6-5e6f50c2ce09")),
                new MissionMir("Prelievo", "Linea6", "_1", Guid.Parse("9948e098-a89a-472b-b6e5-824cdcf63e40")),
                new MissionMir("Prelievo", "Linea6", "_2", Guid.Parse("2f19284d-bf33-4ddd-a924-4f4601cb4e6e")),
                new MissionMir("Prelievo", "Linea6", "_3", Guid.Parse("7ea29a90-bd6d-47e5-8383-23bc106fb6d3")),
                new MissionMir("Prelievo", "Linea7", "_1", Guid.Parse("db40f244-ce50-4dde-a25d-119fbd3be461")),
                new MissionMir("Prelievo", "Linea7", "_2", Guid.Parse("f73e7244-482e-4c97-a9d9-6674b1ae0729")),
                new MissionMir("Prelievo", "Linea7", "_3", Guid.Parse("4464ff71-358f-44ab-8621-3753721d79bc")),
                new MissionMir("Prelievo", "Linea8", "_1", Guid.Parse("167b75ac-e203-4a42-a3bc-8230d0c885ca")),
                new MissionMir("Prelievo", "Linea8", "_2", Guid.Parse("0b88b73e-6437-4d5c-9182-0c4c97aaf9b1")),
                new MissionMir("Prelievo", "Linea8", "_3", Guid.Parse("5e2249e8-5e2e-4b57-a23d-63090bdd7919")),
                new MissionMir("Prelievo", "Linea9", "_1", Guid.Parse("ff05d429-a135-4746-9684-5e39dc15d4ec")),
                new MissionMir("Prelievo", "Linea9", "_2", Guid.Parse("ff5b3abe-d00d-4187-b925-88b00a425b83")),
                new MissionMir("Prelievo", "Linea9", "_3", Guid.Parse("754bc95e-6d65-442d-8ca3-d13f10c2fe3c")),
                new MissionMir("Prelievo", "Linea10", "_1", Guid.Parse("e5018355-41ab-4154-a092-a35b20a3044b")),
                new MissionMir("Prelievo", "Linea10", "_2", Guid.Parse("f7be9e1f-051b-492a-86c9-95d85a57440a")),
                new MissionMir("Prelievo", "Linea10", "_3", Guid.Parse("910d0e13-6689-4d4e-8867-9a76c51b8089")),
                new MissionMir("Prelievo", "Linea11", "_1", Guid.Parse("b1874a08-d8d7-4697-b3e8-f5993b137fc7")),
                new MissionMir("Prelievo", "Linea11", "_2", Guid.Parse("0c91d531-089a-4453-815b-f0cb3368b471")),
                new MissionMir("Prelievo", "Linea11", "_3", Guid.Parse("a4d94690-6c18-443b-82cf-cf0d30455781")),
                new MissionMir("Prelievo", "Linea12", "_1", Guid.Parse("3efe5a67-1919-4def-bc32-f6a0010e75d8")),
                new MissionMir("Prelievo", "Linea12", "_2", Guid.Parse("16af4497-5c51-4d41-8ca9-0b2fa04b4698")),
                new MissionMir("Prelievo", "Linea12", "_3", Guid.Parse("24a2c501-9c51-465d-bc34-f70827bba127")),
                new MissionMir("Prelievo", "Linea13", "_1", Guid.Parse("75111885-1bad-4394-83f5-b206dafc8571")),
                new MissionMir("Prelievo", "Linea13", "_2", Guid.Parse("cfde1893-cab0-4a75-981f-0a43ce2682c0")),
                new MissionMir("Prelievo", "Linea13", "_3", Guid.Parse("42247bae-ab28-4c8b-bb80-a7d5c1eeb42a")),
                new MissionMir("Prelievo", "Linea14", "_1", Guid.Parse("b3943ea8-15e9-48ac-b129-09b69ea6816e")),
                new MissionMir("Prelievo", "Linea14", "_2", Guid.Parse("427aab3d-f7e8-4098-9b7c-91b31eb0891e")),
                new MissionMir("Prelievo", "Linea15", "_1", Guid.Parse("53623ee3-78ee-489b-99c2-cac778e17b94")),
                new MissionMir("Prelievo", "Linea15", "_2", Guid.Parse("ae69e9c2-bc59-4707-8799-938f9d73546d")),
                new MissionMir("Prelievo", "Linea16", "_1", Guid.Parse("68d36be2-01b5-46e7-a952-73f3507797dd")),
                new MissionMir("Prelievo", "Linea16", "_2", Guid.Parse("fe1641d2-d5cc-4f31-849e-6c12217f6d9f")),
                new MissionMir("Prelievo", "Linea17", "_1", Guid.Parse("701aad66-cbd4-413e-a6e8-d1c9c1ac60e0")),
                new MissionMir("Prelievo", "Linea17", "_2", Guid.Parse("83c8b26d-9a09-4797-83b4-394264aa2d4a")),
                new MissionMir("Prelievo", "Linea18", "_1", Guid.Parse("59f6e6b7-f4bb-4319-ab8d-abf848d912c0")),
                new MissionMir("Prelievo", "Linea18", "_2", Guid.Parse("443f59e8-638d-4ce5-b3be-a5ffc86ff447")),
                new MissionMir("Prelievo", "Linea19", "_1", Guid.Parse("70fdf99f-f110-4798-a393-2c0fc2fdacaa")),
                new MissionMir("Prelievo", "Linea19", "_2", Guid.Parse("bb166a23-abd5-4265-8f6c-269f2fe74b07")),
                new MissionMir("Prelievo", "Linea20", "_1", Guid.Parse("9dfc769a-d8d3-4fdd-a6f1-bd5a5955baf9")),
                new MissionMir("Prelievo", "Linea20", "_2", Guid.Parse("f099cee6-070b-4135-a637-2ba925d22b3e")),
                new MissionMir("Prelievo", "Linea21", "_1", Guid.Parse("af5e4d6d-b6a7-4d5e-a26f-4b38118e4dff")),
                new MissionMir("Prelievo", "Linea21", "_2", Guid.Parse("87312176-5834-4e2f-9f47-66c5f9d0cc8f")),
                new MissionMir("Prelievo", "Linea22", "_1", Guid.Parse("bdec2da5-7ce6-42f7-885a-c3fe17c2a6b9")),
                new MissionMir("Prelievo", "Linea23", "_1", Guid.Parse("d6d09fb3-c926-4bcc-ad24-4d82d6bfc643")),
                new MissionMir("Prelievo", "Linea23", "_2", Guid.Parse("c5235e7f-9833-41ff-91e0-6e9371f1cfe1")),
                new MissionMir("Deposito", "Linea10", "_1", Guid.Parse("ba9f1333-4f7a-4f9b-ac6d-20b11e37da6f")),
                new MissionMir("Deposito", "Linea10", "_2", Guid.Parse("c9a63f6a-8970-4f5b-9391-d5bffc764065")),
                new MissionMir("Deposito", "Linea10", "_3", Guid.Parse("75c68e38-d1de-4126-ac7a-2e7ac613f0bc")),
                new MissionMir("Deposito", "Linea5", "_1",  Guid.Parse("6fcefb8f-957c-4a97-accf-ed7a0fdddcf7")),
                new MissionMir("Deposito", "Linea5", "_2",  Guid.Parse("6c9f0b63-0d81-4491-baa1-e9deffa1f2f0")),
                new MissionMir("Deposito", "Linea5", "_3",  Guid.Parse("d2de18f2-a1e8-4b31-8e6e-e1b8f3c1bb1b")),
                new MissionMir("Deposito", "Linea6", "_1",  Guid.Parse("d57b889c-e7b5-4e4f-9619-53884c992be6")),
                new MissionMir("Deposito", "Linea6", "_2",  Guid.Parse("de7f24fc-ab78-4397-9060-6d910bcaa288")),
                new MissionMir("Deposito", "Linea6", "_3",  Guid.Parse("ab69f6cc-8578-4945-a42a-b6edbaf50c38")),
                new MissionMir("Deposito", "Linea7", "_1",  Guid.Parse("eeeef590-5f55-4e16-a3bb-812d80814b22")),
                new MissionMir("Deposito", "Linea7", "_2",  Guid.Parse("f6682f2a-1bd0-4846-82be-cabb9acab4e4")),
                new MissionMir("Deposito", "Linea7", "_3",  Guid.Parse("67ad9dab-8ec0-4b4f-aec7-f213faee6f7a")),
                new MissionMir("Deposito", "Linea8", "_1",  Guid.Parse("452372f0-385d-48dd-9db8-b959342e2631")),
                new MissionMir("Deposito", "Linea8", "_2",  Guid.Parse("1dfeb31e-7e7e-45de-b0e7-2b97fd7cc5d3")),
                new MissionMir("Deposito", "Linea8", "_3",  Guid.Parse("c8e8165c-2e30-4af2-8733-7767dcfb4ca2")),
                new MissionMir("Deposito", "Linea9", "_1",  Guid.Parse("fac68990-d1bb-422b-b442-6100fa42c424")),
                new MissionMir("Deposito", "Linea9", "_2",  Guid.Parse("77718305-0ef2-4897-9509-66693b855c7c")),
                new MissionMir("Deposito", "Linea9", "_3",  Guid.Parse("ad17ed6e-3482-4929-b1cb-063d2f1f59df")),
                new MissionMir("Deposito", "Linea11", "_1", Guid.Parse("80e5864e-7d4e-44fd-a83b-e1fc0c89c296")),
                new MissionMir("Deposito", "Linea11", "_2", Guid.Parse("88f42064-b67b-4a0b-bfe9-38c55e3797a8")),
                new MissionMir("Deposito", "Linea12", "_1", Guid.Parse("8b289041-d381-4d4d-95ca-fa98cb8403c1")),
                new MissionMir("Deposito", "Linea12", "_2", Guid.Parse("568410d5-7bbc-4a64-aa95-b018f769066d")),
                new MissionMir("Deposito", "Linea12", "_3", Guid.Parse("cacfe453-59c3-4cd2-82ab-8c0c0c9a1764")),
                new MissionMir("Deposito", "Linea13", "_1", Guid.Parse("fc580b6e-091c-4f7a-b643-5408514d79fc")),
                new MissionMir("Deposito", "Linea13", "_2", Guid.Parse("428908b8-ffab-4a2d-a43e-62465823c8c7")),
                new MissionMir("Deposito", "Linea13", "_3", Guid.Parse("7ccc18b7-6a75-4140-95e7-67c741970255")),
                new MissionMir("Deposito", "Linea14", "_1", Guid.Parse("d3df66f2-9165-4bbc-8b3e-8b13687fd137")),
                new MissionMir("Deposito", "Linea14", "_2", Guid.Parse("544c43e7-2aa8-4421-84d7-6d26dfac5695")),
                new MissionMir("Deposito", "Linea15", "_1", Guid.Parse("0923288c-7cc7-4fc3-a0e8-2a153c4bdb0c")),
                new MissionMir("Deposito", "Linea15", "_2", Guid.Parse("e94a5e02-53de-41c5-9fd6-2b4297009b91")),
                new MissionMir("Deposito", "Linea16", "_1", Guid.Parse("4840d8c8-df0a-4f84-86c9-2f9d585cda0c")),
                new MissionMir("Deposito", "Linea16", "_2", Guid.Parse("eec934ee-377e-4902-83f3-a65fa18dc04e")),
                new MissionMir("Deposito", "Linea17", "_1", Guid.Parse("19a168e1-0d38-451d-bce2-fcedf8295659")),
                new MissionMir("Deposito", "Linea17", "_2", Guid.Parse("39615089-4040-491c-8273-8c56feedec72")),
                new MissionMir("Deposito", "Linea18", "_1", Guid.Parse("4eb860e4-71a1-4dc4-9cf9-47a7adb069e4")),
                new MissionMir("Deposito", "Linea18", "_2", Guid.Parse("61449349-7980-4bc7-a990-dbfef633792b")),
                new MissionMir("Deposito", "Linea19", "_1", Guid.Parse("7cf18d17-4cc1-47e4-9f3f-d44851c10013")),
                new MissionMir("Deposito", "Linea19", "_2", Guid.Parse("1db9f55f-84c2-428d-8c27-7e7e9d5593a2")),
                new MissionMir("Deposito", "Linea20", "_1", Guid.Parse("c6eaff45-1684-4540-b2be-4104c2ad6700")),
                new MissionMir("Deposito", "Linea20", "_2", Guid.Parse("64246802-6099-49a5-924c-63d7549ae33c")),
                new MissionMir("Deposito", "Linea21", "_1", Guid.Parse("bc48c8fd-d3b6-4408-b22b-017fa2661702")),
                new MissionMir("Deposito", "Linea21", "_2", Guid.Parse("9e03124e-f2dd-4a7f-b360-e54c0ba9ad29")),
                new MissionMir("Deposito", "Linea22", "_1", Guid.Parse("dd16198d-0672-4dbc-8e28-a2a69fadb033")),
                new MissionMir("Deposito", "Linea23", "_1", Guid.Parse("9c7ca683-47cf-4abe-beba-9821c43d7107")),
                new MissionMir("Deposito", "Linea23", "_2", Guid.Parse("5c0cfa59-2461-4b04-8468-f19d35fdb5ba"))
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




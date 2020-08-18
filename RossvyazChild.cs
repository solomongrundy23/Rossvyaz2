namespace Rossvyaz2
{
    public class RossRecords : Records
    {
        private static RossRecords instance;
        public static RossRecords GetInstance()
        {
            if (instance == null) instance = new RossRecords();
            return instance;
        }
    }
}

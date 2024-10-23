namespace TheFrozenDesert.GamePlayObjects.Equipment
{
    public class CookedFood
    {
        public int AddSaturation { get; set; }
        public bool Ate { get; set; }
    
        public CookedFood(int addSaturaion,bool ate)
        {
            AddSaturation = addSaturaion;
            Ate = ate;
        }
    }

}
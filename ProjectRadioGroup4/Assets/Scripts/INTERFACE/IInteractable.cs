namespace INTERFACE
{
    
    public interface IInteractable
    {
        public void AddToInteractList();

        public void OnScan();

        public void Reveal();

        public bool CanSecondPhase { get; set; }
    }
}
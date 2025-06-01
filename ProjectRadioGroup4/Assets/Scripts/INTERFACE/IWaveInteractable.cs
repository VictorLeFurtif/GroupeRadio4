using INTERACT;

namespace INTERFACE
{
    public interface IWaveInteractable : IInteractable
    {
        WaveSettings GetCurrentWaveSettings();
        void MoveToNextPattern();
        bool HasRemainingPatterns();
        bool CanBeActivated();
        void MarkAsUsed();

        public bool Detected { get; set; }
        void Activate();
    }
}
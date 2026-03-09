public interface IInteractable
{
    void Interact();
    bool CanInteract();
    void ShowInteractionPrompt();
    void HideInteractionPrompt();
}
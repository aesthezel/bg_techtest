using UnityEngine;

namespace Game.Code.Entities
{
    public class NPCInteract : MonoBehaviour
    {
        [SerializeField] 
        private CanvasGroup InteractionCanvasGroup;
        [field: SerializeField] 
        public GameObject InteractionObject { get; private set; }

        public void ToggleInteraction(bool status)
        {
            InteractionCanvasGroup.alpha = status ? 1 : 0;
            InteractionCanvasGroup.interactable = status;
            InteractionCanvasGroup.blocksRaycasts = false;
        }
    }
}
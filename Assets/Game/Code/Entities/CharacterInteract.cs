using System;
using UnityEngine;

namespace Game.Code.Entities
{
    public class CharacterInteract : MonoBehaviour
    {
        [SerializeField] 
        private KeyCode interactKey;
        private Character _character;

        private GameObject _lastCollisionObject;
        
        public void Configure(Character character)
        {
            _character = character;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.TryGetComponent(out NPCInteract npcInteract)) return;
            npcInteract.ToggleInteraction(true);
            _lastCollisionObject = npcInteract.InteractionObject;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.gameObject.TryGetComponent(out NPCInteract npcInteract)) return;
            npcInteract.ToggleInteraction(false);
            _lastCollisionObject = null;
        }

        private void Update()
        {
            if (_character is null) return;
            if (_lastCollisionObject is null) return;
            
            if (Input.GetKeyDown(interactKey))
            {
                _character.TryOpenStore(_lastCollisionObject);
            }
        }
    }
}
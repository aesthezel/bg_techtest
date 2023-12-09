using TMPro;
using UnityEngine;

namespace Game.Code.Entities
{
    public class CharacterCreditsView : MonoBehaviour, ICharacterView
    {
        public Character Character { get; private set; }

        [SerializeField] private TMP_Text creditsText;

        private void UpdateCredits(int credits)
        {
            creditsText.text = $"Money: <color=yellow>{credits}c</color>";
        }
        
        public void Configure(Character character)
        {
            Character = character;
            character.OnCreditsChanged += UpdateCredits;
            
            UpdateCredits(character.Credits);
        }
    }
}
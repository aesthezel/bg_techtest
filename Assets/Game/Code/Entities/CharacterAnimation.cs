using UnityEngine;

namespace Game.Code.Entities
{
    public class CharacterAnimation : MonoBehaviour
    {
        [SerializeField] 
        private Animator animator;
        
        private int idleAnimation = Animator.StringToHash("Idle");
        private int walkAnimation = Animator.StringToHash("Walk");
        
        public void PerformIdle() => animator.Play(idleAnimation);
        public void PerformWalk() => animator.Play(walkAnimation);
    }
}
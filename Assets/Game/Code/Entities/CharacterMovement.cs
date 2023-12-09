using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Code.Entities
{
    public class CharacterMovement : MonoBehaviour
    {
        private const string HorizontalInputTag = "Horizontal";
        
        [SerializeField] 
        private Rigidbody2D rb2D;
        [SerializeField] 
        private float speed;
        [SerializeField] 
        private Transform bodyToRotate;

        public bool IsMoving => MovingHorizontalAxis != 0;
        private float MovingHorizontalAxis => Input.GetAxis(HorizontalInputTag);
        
        private void Start()
        {
            rb2D = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            var movement = new Vector2(MovingHorizontalAxis, Vector2.zero.y);
            rb2D.velocity = movement * (speed * Time.deltaTime);
        }

        private void Update()
        {
            var scale = bodyToRotate.localScale;
            scale.x = MovingHorizontalAxis < 0 ? -1 : 1;
            bodyToRotate.localScale = scale;
        }
    }
}
using UnityEngine;

namespace OmniumLessons
{
    public interface IMovable
    {
        public float Speed { get; set; }
                
        public void Initialize(CharacterData characterData);
        
        public void Move(Vector3 direction);
        
        public void Rotation(Vector3 direction);
    }
}
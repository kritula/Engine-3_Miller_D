using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OmniumLessons
{
    public class CharacterData : MonoBehaviour
    {
        [SerializeField] private int _scoreCost;
        [SerializeField] private float _defaultSpeed;
        [SerializeField] private Transform _characterTransform;
        [SerializeField] private CharacterController _characterController;

        public int ScoreCost => _scoreCost;
        public float DefaultSpeed => _defaultSpeed;
        public Transform CharacterTransform => _characterTransform;
        public CharacterController CharacterController => _characterController;
    }
}
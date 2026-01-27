using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OmniumLessons
{
    public class CharacterData : MonoBehaviour
    {
        // Добавляем информацию о стоимости нашего персонажа за его убийство
        [SerializeField] private int _scoreCost;
        [SerializeField] private float _defaultSpeed;
        [SerializeField] private Transform _characterTransform;
        [SerializeField] private CharacterController _characterController;

        // Ниже представлены свойство. Они похожи на переменные по своей сути.
        // Они управляют доступом к данным через методы get и set.
        // Также могут выполнять дополнительные действия при изменении или запросе данных.

        public int ScoreCost => _scoreCost;
        public float DefaultSpeed => _defaultSpeed;
        public Transform CharacterTransform => _characterTransform;
        public CharacterController CharacterController => _characterController;
    }
}
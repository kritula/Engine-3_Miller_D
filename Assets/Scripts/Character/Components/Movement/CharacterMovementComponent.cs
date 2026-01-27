using System.Xml;
using UnityEngine;

namespace OmniumLessons
{
	public class CharacterMovementComponent : IMovable
	{
		private CharacterData characterData;
		private float _speed;

		public float Speed
		{
			get => _speed;
			set
			{
				if (value < 0)
					value = 0;
				_speed = value;
			}
		}

		// Мы создаем этот метод для инициализации экземпляра класса, чтобы передать ему нужные данные.
		public void Initialize(CharacterData characterData)
		{
			// Ключевое слово this говорит о том, что _characterData принадлежит конкретно этому экземпляру класса.
			// Требуется когда, например, входящая переменная и переменная класса одинаково называются.
			// Например, как в нашем случае
			this.characterData = characterData;
			
			// Присваиваем переменной _speed новое значение
			_speed = characterData.DefaultSpeed;
		}

		// Прописывать используемый namespace можно и напрямую.
		// Данный Vector3 принадлежит пространству имен UnityEngine
		public void Move(UnityEngine.Vector3 direction)
		{
			// Если нет направления движения, мы прерываем выполнение метода Move()
			// Для этого мы используем слово return.
			if (direction == Vector3.zero)
				return;
			
			// Это вычисление угла (в градусах) направления движения объекта на плоскости.
			// Mathf.Atan2 — находит угол в радианах по координатам X и Z
			// Mathf.Rad2Deg — переводит полученный угол из радианов в градусы
			float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
			
			// получаем направление движения по оси Y
			Vector3 move = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
			
			// Обращаемся к CharacterController из переменной _characterData, чтобы передать смещение
			characterData.CharacterController.Move(move * Speed * Time.deltaTime);
		}

		public void Rotation(Vector3 direction)
		{
			if (direction == Vector3.zero)
				return;

			// Можно создавать локальные для метода переменные.
			// Они существуют вплоть до окончания выполнения метода.
			float smooth = 0.1f;
			float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
			float angle = Mathf.SmoothDampAngle(
				characterData.CharacterTransform.eulerAngles.y,
				targetAngle,
				ref smooth,
				smooth);
			
			// передаем направление в компонент Transform
			characterData.CharacterTransform.rotation = Quaternion.Euler(0, angle, 0);
		}
	}
}
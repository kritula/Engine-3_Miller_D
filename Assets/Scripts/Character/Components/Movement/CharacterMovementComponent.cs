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
			this.characterData = characterData;
			
			_speed = characterData.DefaultSpeed;
		}

		public void Move(UnityEngine.Vector3 direction)
		{
			if (direction == Vector3.zero)
				return;
			
			float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
			
			Vector3 move = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
			
			characterData.CharacterController.Move(move * Speed * Time.deltaTime);
		}

		public void Rotation(Vector3 direction)
		{
			if (direction == Vector3.zero)
				return;

			float smooth = 0.1f;
			float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
			float angle = Mathf.SmoothDampAngle(
				characterData.CharacterTransform.eulerAngles.y,
				targetAngle,
				ref smooth,
				smooth);
			
			characterData.CharacterTransform.rotation = Quaternion.Euler(0, angle, 0);
		}
	}
}
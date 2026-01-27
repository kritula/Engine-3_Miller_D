namespace OmniumLessons {
    public interface IAttackComponent {
        public int Damage { get; }

        public void Initialize(CharacterData characterData);

        public void MakeDamage(Character damageTarget);

        public void OnUpdate();
    }
}
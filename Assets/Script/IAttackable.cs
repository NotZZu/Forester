using UnityEngine;

public interface IAttackable
{
    void Attacked(Vector2 attacker, float knockBackPower);
}

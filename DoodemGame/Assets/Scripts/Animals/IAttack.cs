using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttack
{
    public void AttackUpdate();
    public void SetCurrentObjetive(Transform co);
}

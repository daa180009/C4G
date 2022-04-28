using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class DirectEnemyDamage : CardEffectPredicate
{
    public override Card.TargetType TargetType { get { return Card.TargetType.Enemies; } }

    public float damage = 1f;

#if UNITY_EDITOR
    public override void InputGUI()
    {
        damage = EditorGUILayout.FloatField("Damage: ", damage);
    }
#endif

    public override void PerformPredicate(TargetInfo targetInfo, WorldInfo worldInfo, ResolutionInfo resolutionInfo)
    {
        foreach (EnemyController enemy in targetInfo.Enemies)
            enemy.DirectDamage(damage);
    }

    public override string GetDescription(WorldInfo worldInfo)
    {
        return "you deal " + damage + " damage to that enemy.";
    }
}

using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ToB.Entities
{
    public class SecurityArcherPerimeterPattern:EnemyPattern
    {
        Coroutine perimeterCoroutine; 
        SecurityArcherFSM fsm => strategy as SecurityArcherFSM;
        SecurityArcherSO so => enemy.enemySO as SecurityArcherSO;
        public SecurityArcherPerimeterPattern(EnemyStrategy strategy, Action EndCallback = null) : base(strategy, EndCallback)
        {
        }

        public override void Enter()
        {
            base.Enter();
            perimeterCoroutine = enemy.StartCoroutine(Perimeter());
        }

        public override void Exit()
        {
            base.Exit();
            if(perimeterCoroutine != null) enemy.StopCoroutine(perimeterCoroutine);
        }
        IEnumerator Perimeter()
        {
            enemy.LookHorizontal(enemy.LookDirectionHorizontal * -1);
            yield return new WaitForSeconds(Random.Range(so.PerimeterInterval - so.PerimeterTimeRandomRange, so.PerimeterInterval + so.PerimeterTimeRandomRange));
            enemy.LookHorizontal(enemy.LookDirectionHorizontal * -1);
            yield return new WaitForSeconds(Random.Range(so.PerimeterInterval - so.PerimeterTimeRandomRange, so.PerimeterInterval + so.PerimeterTimeRandomRange));
            fsm.ChangePattern(fsm.movePattern);
        }
    }
}
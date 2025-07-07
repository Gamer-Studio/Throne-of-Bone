using UnityEngine;

namespace ToB.Entities
{
    /// <summary>
    /// 피격당하는 부위라는 정체성을 나타내는 인터페이스입니다. <para>
    /// 대부분의 몬스터, 혹은 전체가 그러한 특징을 갖게 될 수도 있지만
    /// 혹시나 마력코어 파괴나 메카닉처럼 부위파괴로 공략하는 적이 있을 경우
    /// 본체가 아닌 다른 곳에 이 인터페이스가 붙게 됩니다. </para>
    /// </summary>
    public interface IEnemyHitPart
    {
        public EnemyStatHandler Stat{ get; }
    }
}

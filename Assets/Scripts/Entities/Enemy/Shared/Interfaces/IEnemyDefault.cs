namespace ToB.Entities
{
    /// <summary>
    /// 몸이 피격당하는 본체인 기본 몬스터 템플릿 제시용 인터페이스입니다
    /// </summary>
    public interface IEnemyDefault
    {
        public EnemyBody EnemyBody { get; }
        public EnemyStatHandler Stat { get; }
        public EnemyRangeBaseSightSensor SightSensor { get; }
    }
}
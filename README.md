## [팀스파르타] 2D 모바일 게임 개발자 과제
### 스크립트 설명   
**1. Monster.cs**   
게임 내 몬스터의 동작 제어.
- Rigidbody2D를 활용.
- 뒤로 밀리지 않는 상태라면 왼쪽으로 일정한 속도로 이동.
- 몬스터가 트럭 또는 다른 몬스터를 만나면 점프 쿨이 돌았을 때 점프 동작.
- 위에 몬스터가 밟고 있다면 뒤로 이동.
- 점프 쿨 타임 적용.
- 몬스터 밀릴 때에는 몬스터 너비만큼 뒤로 이동.
- 뒤에 붙어있는 몬스터도 뒤로 함께 이동.
- HP가 0이하가 되면 비활성화 돼서 풀에 대기.

**2. MonsterFactory.cs**
게임 내 몬스터를 생성하고 관리.
- 젠타임 마다 몬스터를 생성 또는 활성화.
- 게임이 진행중일 때만 몬스터 생성.
- 풀에서 관리중인 비활성화된 몬스터가 있다면 재사용, 없다면 새로 생성하여 풀에 추가.
- 몬스터 활성화 시에 특정 위치, Z-Order, HP 등 몬스터의 상태 초기화.

**3. Hero.cs**
플레이어 캐릭터의 공격 기능 담당.
- 공격속도에 맞추어 자동 공격 또는 마우스 입력으로 수동 공격 가능.
- 자동 공격시에는 가장 가까운 몬스터를 향해 총알 발사.
- 수동 공격시에는 마우스 클릭한 방향을 향해 총알 발사.
- 발사 후에는 발사한 시간을 저장하여 다음 발사 가능 시간 계산 (공격 속도)
- 총알은 풀로 관리하여 비활성화 된 총알 재사용, 없다면 새로 생성하여 풀에 추가.

**4. Bullet.cs**  
총알 관리.
- 몬스터 피격시 몬스터에 데미지를 주고, 비활성화 후 풀에 대기.
- 몬스터를 피격하지 않아도 활성화되면 3초 뒤에 비활성화 후 풀에 대기.
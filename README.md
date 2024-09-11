# 만들려는거
## CCG 계열 게임 로직
* 한개의 게임이 시작하고 끝날때까지의 동작을 구현하는 것을 목표
* (TODO) 게임 내용을 보존하고 저장된 게임 내용에서부터 리플레이 가능
* (TODO) 나중에 웹 서버에 붙여서 웹 게임 형태로 샘플 구현 예정
## 기본사양
1. 플레이어 2인+
2. 각 플레이어는 자신의 덱을 가지고 게임에 입장
3. 턴제로 게임 플레이, 그리고 게임 승리조건 만족시 결말
### 기술적 지향점
1. 게임의 전체 흐름을 시퀀스로 분리하여, 게임 진행을 언제든지 유지하고 불러와서 재개할 수 있도록 함
2. 게임의 현재 진행상황을 메모리 외에 DB 등에 저장하고 이에 따라 stateless하게 다음 동작을 판단할 수 있게 하여 실시간으로 처리하는것 대신에 아예 API 형태로도 동작할 수 있게끔
## 컨셉
### 팩션
* Test
  * 테스트용
* General
  * '공용'
* Politician
  * '질서 선' 캐릭터들의 팩션.
  * 기본적으로 룰에서 보장된 승리조건을 취함
  * 전투 위주의 팩션. 카드의 전체적인 전투 능력치가 높음
* Criminal
  * '질서 악'
  * HQ별로 특수 승리조건으로 '카드의 매수' 등과 관련된 요소를 가질 수 있음
  * 자원 관리에 특화, 커맨드를 통해 카드를 다량으로 뽑거나 할 수 있다. 대신 유닛의 효율이 낮음
* Anarchy
  * '혼돈 악'
  * 카드의 직접적인 파괴 공작에 특화
  * 리스크 있는 플레이를 할 수 있음
* Tricksters
  * '혼돈 선'
  * 필드를 지키고 빌드업 하는데에 특화
  * 필드를 구축하는 그 자체로 승리조건이 되는 HQ가 존재
## 게임 룰
### 준비물
#### 덱
1. HQ 카드 1장, 나머지 30장, 중복 카드는 3장까지. HQ 카드에 맞는 팩션의 카드들만 사용 가능
### 게임 카드 종류
#### HQ
덱의 세력 구성에 관여. 덱에 단 한장 존재한다.
* 가지고 있는 속성치
  * Name : 이름
  * Faction : HQ의 팩션 (1개 이상)
  * SubType : HQ가 가지고 있는 특성 태그 (0개 이상)
  * Level : HQ의 레벨
  * Link : HQ에 연결하여 플레이 할 수 있는 공간의 갯수
  * Power Type : Offensive / Defensive / Wall (유닛과 공통, 유닛에서 설명)
  * Power : 전투가 발생할 때 얼마만큼의 대미지를 줄 수 있는지의 수치 (유닛과 공통, 유닛에서 설명)
  * HP : 게임이 시작할 때, 해당 수치를 가지고 게임을 시작하고, 대미지를 받으면 대미지의 양만큼 수치가 감소한다.
  * 레벨업 능력, 드로우 능력, 파워업 능력 : 많은 HQ의 경우 스스로의 Level 수치를 증가시킬 수 있는 기동 효과를 가진다.
#### 유닛
* 가지고 있는 속성치
  * Name : 이름
  * Faction : 카드의 팩션 (HQ의 Faction의 부분집합이어야 한다)
  * SubType : 유닛이 가지고 있는 특성 태그 (0개 이상)
  * Level : 유닛의 레벨
  * Link : 유닛에 연결하여 플레이 할 수 있는 '스페이스'의 갯수
  * Power Type : 공방 성향을 표현
    * Offensive : 공격 선언할 수 있음. 피격시 대미지로 반격하여 서로가 대미지를 입음
    * Defensive : 공격 선언할 수 없음. 피격시 Offensive처럼 서로 대미지를 줌
    * Wall : 공격 선언할 수 없음. 피격시에도 반격대미지를 주지 않음
  * Power : 공격하거나 전투시 대미지를 얼마나 줄 수 있는지 표현. 유닛의 경우 한 턴에 Power 만큼의 피해를 버틸 수 있다는 기능으로도 통함.
  * 카드의 효과
#### 커맨드
* 가지고 있는 속성치
  * Faction : 카드의 팩션 (HQ의 Faction의 부분집합이어야 한다)
  * SubType : 커맨드가 가지고 있는 특성 태그 (0개 이상)
  * Level : 커맨드의 레벨
  * 카드의 효과
### 카드의 효과
* 기동 효과 : 내 메인 페이즈에 선언함으로서 사용할 수 있는 효과 (사용시 효과처리 대기영역에서 처리)
* 유발 효과 : 특정한 발동 조건이 지정되어 유발시 효과처리 대기영역에서 수행될 효과
* 정적 효과 : 존재함으로서 기한 없이 계속 적용되는 효과
### 게임 영역
#### 1. 덱 영역
* 게임 시작시에 준비한 덱의 카드들이 '덮음' 상태로 존재하는 영역
* 모든 플레이어가 내용을 볼 수 없다.
#### 2. 필드
* 카드(GameCardObject)가 실질적으로 놓이는 장소
* 카드들은 필드에 놓일 때 '액티브'(테이블탑에서, '언탭' 같은 형태), '인액티브'(테이블탑에서, '탭' 같은 형태) 상태를 가진다.
#### 3. 손패
#### 4. 트래시
#### 5. 효과처리 대기영역
* 처리해야 할 효과가 남아있다면 이 곳에 쌓임. 이 효과 해결 전에는 사용자의 다음 행동을 기다리지 않고 자동으로 처리하고 빈 상태에서만 처리한다.
#### 6. 크레딧
* 사용할 수 있는 플레이어의 크레딧
### 게임의 목표
1. (패배조건) 내 HQ의 HP가 0 이하가 됨
2. (패배조건) 내 덱의 장수가 0인 상태에서 드로우
3. 기타 카드에 명시된 게임 조건
### 게임 시작
1. 무작위로 모든 플레이어의 턴 순서를 결정
2. 덱을 셔플하고 3장 드로우
3. 각 플레이어의 HQ 카드가 필드에 놓인 채로 시작
4. 선공 플레이어부터 1턴
#### 턴 흐름
스타트 페이즈 -> 드로우 페이즈 -> 메인 페이즈 -> 엔드 페이즈
* 공통행동 : '상태확인행동'
  * 아래의 상황에서 발생한다.
    * 페이즈 시작시 턴 행동 직후
    * 효과처리 대기영역의 효과 1개가 해결될때마다
    * 플레이어의 액션 선언 및 선언된 액션 해결 직후
  * 발생시 아래의 상태를 확인하고 처리한다.
    * 게임의 패배조건이 충족하였는지 확인하여, 게임의 결과를 결정한다. 결정된 게임은 더 이상 시퀀스를 발생하지 않는다.
      * 패배조건이 동시에 충족되는 경우, 게임을 무승부로 한다.
    * 아래의 과정에서 더이상 트래시에 보낼 것이 없어질때까지 반복한다.
      * 현재 Power 이상의 대미지가 쌓여있는 유닛들을 파기한다.
      * 필드에 남아있는 모든 유닛들 중, 링크에 자신의 레벨보다 더 높은 레벨의 유닛 또는 커맨드가 링크되어 있다면, 해당 객체를 파기한다.
      * 필드에 남아있는 모든 유닛들 중, HQ에서 도달할 수 없는 유닛/커맨드를 파기한다.
    * 효과처리 대기영역이 비어있지 않다면 가장 첫번째 효과 1개의 내용을 수행(해결)한다. (상기의 조건으로 인해 상태확인행동이 다시 발생한다.)

##### 스타트 페이즈
* 턴 행동
  1. 턴 플레이어의 필드에 남아있는 커맨드들을 전부 파기
  2. 턴 플레이어의 필드에 있는 모든 유닛의 대미지가 제거
  3. 턴 플레이어의 필드의 객체들의 인액티브 상태가 액티브로 모두 변경
  4. 1에서 '파기 시'라고 명시된 효과들이 효과처리 대기영역으로 생성하여 이동.
  5. 게임 내의 효과들 중 '스타트 페이즈 시작시에' 라고 명시된 효과들이 효과처리 대기영역으로 생성하여 이동.
  6. 플레이어의 크레딧 수치가 HQ의 레벨 현재 수치와 같아진다.
##### 드로우 페이즈
* 턴 행동
  * 현행 턴의 주인(TurnOwner)은 '카드 1장 드로우'를 수행.
  * '드로우 페이즈 시작시에', '드로우 할때마다' 라고 명시된 효과들이 효과처리 대기영역으로 생성하여 이동.
##### 메인 페이즈
* '메인 페이즈 시작시에' 명시된 효과들이 효과처리 대기영역으로 생성하여 이동.
* 턴 플레이어는 다음의 액션을 선언할 수 있다.
  * 유닛의 플레이
    * HQ 또는 유닛은 자신의 링크 수치만큼의 스페이스를 가진다.
    * 해당 스페이스가 비어있다면 그 HQ 또는 유닛의 레벨 이하의 유닛 카드를, 손에서 내려놓아 플레이할 수 있다.
    * 플레이한 유닛은 인액티브 상태가 된다.
    * 플레이 할 유닛 카드의 레벨만큼의 크레딧을 지불하여야 한다. 지불할 수 없다면 플레이할 수 없다.
    * 해당 유닛의 '필드에 등장할 때' 명시된 효과들이 효과처리 대기영역으로 생성하여 처리.
  * 커맨드의 플레이
    * HQ 또는 유닛은 자신의 링크 수치만큼의 스페이스를 가진다.
    * 해당 스페이스가 비어있다면 그 HQ 또는 유닛의 레벨 이하의 커맨드 카드를, 손에서 내려놓아 플레이할 수 있다.
    * 플레이 할 커맨드 카드의 레벨만큼의 크레딧을 지불하여야 한다. 지불할 수 없다면 플레이할 수 없다.
  * 히든의 플레이
    * 자신이 가지고 있는 크레딧 내에서, 임의의 크레딧을 지불하는 것을 선언하여 수행한다.
    * HQ 또는 유닛은 자신의 링크 수치만큼의 스페이스를 가진다.
    * 패의 카드 한 장을 뒷면인 상태로 비어있는 스페이스에 내려놓는다. 스페이스의 부모 유닛 또는 HQ의 레벨이 지정된 크레딧 수치 이상이어야 한다.
      * 이때 뒷면 상태가 된 카드는 놓인 필드의 주인 플레이어만 정체를 확인할 수 있다.
    * 해당 스페이스에 내려놓은 카드는 아래의 수치를 가지는 유닛으로 간주한다. (선언된 임의의 크레딧 = X)
      * Name : '히든'
      * Faction : General
      * SubType : (None)
      * Level : X
      * Power : X
      * Power Type : Defensive
      * Link : 2
      * 카드 효과 없음
  * 공격 선언
    1. 파워 타입이 Offensive인, 액티브 상태의 유닛 또는 HQ를 인액티브 상태로 하고, 상대의 필드 중 아래의 조건에 해당하는 유닛 또는 HQ를 지정한다.
      * 자신의 스페이스 중 비어있는 스페이스가 있다
      * 자신의 스페이스 전부가 커맨드로 채워져 있다.
    2. 만약 지정한 유닛이 '히든'인 경우, 뒷면 상태의 카드의 정체가 유닛이라면, 아래의 조건을 확인하여 일치시 해당 카드는 앞면 상태가 된다. 앞면 상태로 바뀐다면, 히든으로서 선언된 모든 정보는 무시하고 원래의 유닛의 정보를 사용하게 된다.
      * 선언된 X가 해당 유닛 카드의 레벨 이상의 수치이다.
    2-1. 2에서 앞면으로 된 히든의 '정체가 탄로되었을 때' 명시된 효과들이 효과처리 대기영역에 생성.
    3. 상태확인행동이 발생
    4. 공격한 유닛의 파워만큼, 공격 지정된 유닛 또는 HQ는 대미지를 입는다. 대미지의 수치는, '(파워) - (이 객체가 이번 턴 받은 대미지)'
      4-1. 공격 대상이 된 유닛이 Offensive 또는 Defensive인 경우, 4.에서의 계산처럼 대미지의 수치를 계산하여 공격한 유닛 또는 HQ가 반격 대미지를 입는다.
      4-2. 대미지를 받는 것이 HQ라면, HP를 해당 수치만큼 감소한다.
##### 엔드 페이즈
* 턴 행동
  1. 모든 유닛들의 대미지를 제거한다.
  2. 게임 내의 효과들 중 '엔드 페이즈 시작시에' 라고 명시된 효과들이 효과처리 대기영역으로 생성하여 이동.
  2-1. 상태확인행동이 발생
  4. '턴 종료시까지' 효과들이 기한을 끝마친다.
##### 턴 종료
* 다음 플레이어가 턴을 이어받는다.

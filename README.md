# ToyParty
homework

* 스테이지 21 구현

----------------------------------
하나. 게임시작 - 로그인

  1) Facebook
  
  2) Guest
  
----------------------------------
둘. 게임로직 구현 - 21스테이지

  1) 타일 배치
  
    i) 처음 배치되는 타일은 매칭이 되면 안 됨.
  
  2) 타일 매칭
  
    i) 연속으로 같은 타일이 3개 : 좌우, 상하, 대각선 등
    ii) 같은 타일 4개가 붙어 있는 경우
  
  3) 특수 타일
  
    i) 팽이 : 폭탄 영역에 있거나 매칭되는 타일 옆에 있을 경우가 두번 발생 하면 사라짐
    ii) 폭탄 : 4개이상 매칭이 되면 무조건 폭탄으로 바뀜
              폭탄이 매칭되면 주변 타일이 사라짐
              
  4) 매칭 후 사라진 곳 새로운 타일 생성
  
    i) 새로운 타일에 다시 매칭검사
  
  5) 게임 종료
  
    i) 클리어 : 팽이 10개가 사라짐
    ii) 실패 : 20번 이동후 팽이가 남아 있음
----------------------------------
셋. Remainder

  1) 특수 타일
  
    i) 폭탄 외에 특수 타일
  
  2) UI
  
  3) 스테이지 구성 툴
  
  4) 매칭 점수 계산
  


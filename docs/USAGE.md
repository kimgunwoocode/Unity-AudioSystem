# Unity Audio Manager  

## 문서 (Documentation)  

* [README](README.md)  
* [개발자 가이드](docs/DEV.md)  
* [트러블슈팅](docs/TROUBLESHOOTING.md)  

---

## 기본 설정  

#### AudioManager 생성  
Hierarchy 창에서 우클릭  
Audio > AudioManager 선택  
--> 씬에 AudioManager 프리팹이 생성됩니다.  
  
_**(소리가 재생되는 모든 씬에 존재해야 합니다.)**_  

#### AudioDatabase 생성
Project 창에서 우클릭 (생성위치는 Assets 안에서는 상관 없음)  
Create > Audio > AudioDatabase 선택  
--> 오디오 클립 관리를 위한 ScriptableObject가 생성됩니다. (복수 생성 가능)  
* (생성된 AudioDatabas는 Assets/AudioSystem/Data/AudioSystemConfig.asset에 등록됩니다.)  
* (만약 등록되지 않았다면, AudioSystemConfig의 인스펙터 화면 하단 "Rescan Audio Database"버튼 혹은,
 유니티 메뉴 "Tools/AudioSystem/Rescan Database" 누르기.)  

---  
<br></br>

## AudioClip 등록 (AudioDatabase)  

<img width="446" height="613" alt="image" src="https://github.com/user-attachments/assets/097c7466-689f-4fde-879a-54b1c5d818ba" />
<br><sub> [AudioDatabase ScriptableObject 인스펙터 화면] </sub></br>


#### AudioClip 요소 추가
- 리스트에 요소 추가 ('+' 기호)
- 원하는 Key 이름 입력 (**필수!** 공백 또는 유효하지 않은 문자열은 무시되거나 오류 발생)
- Volume 및 Pitch 설정 (기본 1)

#### Key 등록
- 인스펙터 화면 하단에 "Generate Audio Keys" 버튼 누르기  
--> Assets/AudioSystem/Data/AudioKeys.cs에 키 저장됨.  
(유니티 메뉴 "Tools/AudioSystem/Generate Audio Keys"도 같은 기능.)  
(반드시 버튼을 눌러주어야 합니다. 자동 저장 안됨.)




## API 사용법

using AudioSystem; 선언해야 함!

### Volume
##### 함수 목록 : 
```csharp
void SetMasterVolume(float volume);
void SetBGMVolume(float volume);
void SetSFXVolume(float volume);

float GetMasterVolume();
float GetBGMVolume();
float GetSFXVolume();
```

#### 볼륨 조절 : 
```csharp
AudioManager.SetMasterVolume(1.0f); // Master 볼륨을 volume으로 설정
AudioManager.SetBGMVolume(0.5f);    // BGM 볼륨을 volume으로 설정
AudioManager.SetSFXVolume(0.7f);    // SFX 볼륨을 volume으로 설정
```
 - volume 범위 : 0.0 ~ 1.0  

#### 볼륨값 가져오기 : 
```csharp id="8y1xk3"
float master = AudioManager.GetMasterVolume();  // Master 볼륨값 반환
float bgm = AudioManager.GetBGMVolume();        // BGM 볼륨값 반환
float sfx = AudioManager.GetSFXVolume();        // SFX 볼륨값 반환
```
 - 반환값(float) : 0.0 ~ 1.0



### SFX
##### 함수 목록 : 
```csharp
AudioHandle PlaySFX(string key, bool loop = false);
AudioHandle PlaySFX(string key, Vector3 position, bool loop = false);

void StopSFX(AudioHandle handle);
void StopSFX(string key);
void StopAllSFX();
```

#### SFX 재생
```csharp
AudioHandle handle = AudioManager.PlaySFX("AudioClip_01");          // SFX 재생 (2D, 한 번만 재생)
AudioHandle handle = AudioManager.PlaySFX("AudioClip_02", true);    // SFX 재생 (2D, 멈출 때까지)
AudioHandle handle = AudioManager.PlaySFX("AudioClip_03", transform.position);        // SFX 재생 (3D, 한 번만 재생)
AudioHandle handle = AudioManager.PlaySFX("AudioClip_03", transform.position, true);  // SFX 재생 (3D, 멈출 때까지)
```

#### SFX 정지
```csharp
AudioHandle handle_01 = AudioManager.PlaySFX("AudioClip_01");       // 재생한 SFX의 반환값 저장하기 (참조형)
AudioHandle handle_02 = AudioManager.PlaySFX("AudioClip_02", true); // 루프 재생도 참조 가능

AudioManager.StopSFX(handle_01);  // 재생 중간에 정지 가능
handle_01 = null;
```
 - 루프 재생은 StopSFX로 정지하지 않으면 계속 반복된다.
 - handle 개수는 동시에 INT_MAX를 넘을 수 없다.
 - StopSFX에 의해 정지되지 않았더라도 1회 재생이 끝났다면, AudioHandle은 유효하지 않은 값을 가진다.
 - StopSFX를 호출한 이후에 AudioHandle의 참조는 해제하는 것을 권장한다.

```csharp
AudioHandle handle_02 = AudioManager.PlaySFX("AudioClip_02"); // 루프 재생도 참조 가능

AudioManager.StopSFX("AudioClip_02");                         // 오디오 클립 key 이름으로 정지 가능
```
 - 오디오 클립 key 이름으로 StopSFX를 호출하면, 해당 오디오 클립으로 재생된 모든 AudioSource 재생이 멈춘다.

```csharp
AudioManager.StopAllSFX();  // 모든 SFX 정지
```
 - 오디오 클립 key 또는 StopAllSFX()로 정지하더라도 AudioHandle의 유효성은 만료된다.



### BGM
##### 함수 목록 : 
```csharp
void PlayBGM(string key);
void StopBGM(float duration = 0f);
```
 - BGM은 동시에 하나만 재생 가능

#### BGM 재생
```csharp
AudioManager.PlayBGM("BGM_00");
```

#### BGM 정지
```csharp
AudioManager.StopBGM();      // 즉시 정지
AudioManager.StopBGM(2.0f);  // 페이드아웃 후 정지 (2.0초)
```
 - 페이드아웃 효과 중복 호출시 최초 1회 이후 호출은 효과가 끝날 때까지 무시된다.



### Debug / Info
##### 함수 목록 : 
```csharp
int GetSFXpoolCount();
int GetActiveSFXCount();
```

#### 풀링 개수 가져오기
```csharp
int poolCount = AudioManager.GetSFXpoolCount();      // 대기 중인 비활성 오디오 객체의 총 개수를 반환
int activeCount = AudioManager.GetActiveSFXCount();  // 재생 중인 활성 오디오 객체의 총 개수를 반환
```

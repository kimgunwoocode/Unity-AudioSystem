# Unity Audio Manager

ScriptableObject 기반 키 자동 생성과 Handle 제어를 지원하는 Unity 오디오 시스템  
Handle 기반으로 SFX를 개별 제어하고, 자동 생성된 키로 오타 없는 오디오 사용을 보장합니다.  
(AudioSource Pooling + 2D/3D SFX 지원)

---
  
## 개발 의도 (Why)

기존에 사용하던 오디오 시스템은 string 기반으로 사운드를 관리하는 경우가 많아  
키 오타로 인한 런타임 오류가 발생하기 쉽고, 유지보수에도 불리한 문제가 있었습니다.

또한 동일한 SFX를 여러 개 재생할 경우 특정 인스턴스만 제어하기 어려워  
세밀한 사운드 제어에 한계가 있습니다.

이 시스템은 이러한 문제를 해결하기 위해 다음과 같이 설계되었습니다:

- ScriptableObject에 오디오를 등록하면 **사용자가 정의한 키를 자동으로 코드로 생성**하여  
  타입 안전하게 오디오를 사용할 수 있도록 구성
- SFX 재생 시 고유한 **AudioHandle**클래스를 반환하여  
  특정 사운드 인스턴스를 개별적으로 제어 가능
- 싱글톤 구조를 통해 **어디서든 간단하게 호출 가능하도록 설계**

이를 통해 안정성과 사용성을 모두 확보하면서,  
유연한 오디오 제어를 지원하는 것을 목표로 합니다.

---
  
## 주요 기능 (Features)

* ScriptableObject에 정의한 키로 코드 생성 (AudioDatabase)
* 2D / 3D SFX 재생 지원
* AudioMixer 기반 볼륨 제어
* BGM / SFX / Master 볼륨 분리
* AudioSource Pooling
* Handle 기반 SFX 개별 제어
* 동일 SFX 다중 재생 지원

---
  
  
## 설치 방법 (Installation)

### 1. Unity에 AudioSystem 추가  

#### - 직접 스크립트 추가  
main 브랜치에서 AudioSystem 폴더를 복사  
Unity 프로젝트의 Assets 폴더에 붙여넣기  

#### - Unity Custom Package로 추가  
custom-package 브랜치에서 AudioSystem.unitypackage 다운로드  
Unity에서 다음 메뉴 선택  
&emsp; Assets > Import Package > Custom Package  
다운로드한 AudioSystem.unitypackage 파일 선택 후 Import  

_..._
> _추후 UPM으로 배포 예정_
> 
_..._

### 2. Unity 초기 설정

#### AudioManager 생성  
Hierarchy 창에서 우클릭  
Audio > AudioManager 선택  
--> 씬에 AudioManager 프리팹이 생성됩니다.  

#### AudioDatabase 생성
Project 창에서 우클릭  
Create > Audio > AudioDatabase 선택  
--> 오디오 클립 관리를 위한 ScriptableObject가 생성됩니다. (복수 생성 가능)  

_AudioDatabase 사용 방법은 별도 문서에서 설명_



---
  
  
## API Reference

### Volume

```csharp id="8y1xk3"
SetMasterVolume(float volume);
SetBGMVolume(float volume);
SetSFXVolume(float volume);
```


### SFX

```csharp id="x92mfa"
AudioHandle PlaySFX(string key, bool loop = false);
AudioHandle PlaySFX(string key, Vector3 position, bool loop = false);

void StopSFX(AudioHandle handle);
void StopSFX(string key);
void StopAllSFX();
```


### BGM

```csharp id="u8t7kl"
void PlayBGM(string key);
void StopBGM(float duration = 0f);
```


### Debug / Info

```csharp id="r3k9zp"
int GetSFXpoolCount();
int GetActiveSFXCount();
```

---
## 파일 구조 (Structure)

```plaintext
AudioSystem/
├── AudioClip/                # 샘플 오디오 파일
│   └── Sample_01
│
├── Data/                     # 오디오 데이터 및 설정
│   ├── AudioDatabase.asset
│   ├── AudioKeys.cs          # 자동 생성된 키
│   ├── AudioMixer.mixer
│   └── AudioSystemConfig.asset
│
├── Editor/                   # 에디터 확장 기능
│   ├── AudioDatabaseEditor.cs
│   ├── AudioKeyGenerator.cs
│   ├── AudioDatabaseTracker.cs
│   ├── AudioDatabaseRescan.cs
│   ├── AudioSystemConfigEditor.cs
│   ├── AudioConfigProvider.cs
│   └── CreateAudioManagerObject.cs
│
├── Prefab/                   # 기본 프리팹
│   └── AudioManager.prefab
│
└── Scripts/                  # 런타임 코드
    ├── AudioManager.cs
    ├── AudioDatabase.cs
    └── AudioSystemConfig.cs
```


---
  
## 주의사항 (Notes)

* AudioSystem폴더와 그 하위폴더의 위치 변경 금지
* `AudioHandle`은 재사용되므로 장기간 보관 시 주의 필요
* `StopAllSFX()` 호출 시 모든 handle은 무효화됨
* 잘못된 handle(version mismatch)은 자동으로 무시됨
* loop 사운드는 반드시 handle로 관리하는 것을 권장

---
  
## 개선 사항 (추후 업데이트 목표)

* AudioKeys에서 SFX / BGM 외에 사용자 정의 카테고리를 추가할 수 있는 기능
* AudioDatabase 변경 시 AudioKeys 자동 갱신 (Auto Sync)
* Singleton 동작 방식 설정 기능
&emsp;* 다음 씬까지 유지 여부 (DontDestroyOnLoad)
&emsp;*  씬에 없을 경우 자동 생성 여부
* SFX Fade In / Fade Out 지원
* AudioGroup 기반 관리 시스템
* Addressables 연동
* Async 로딩 지원

---
  
## 문서 (Documentation)

* [사용 가이드](docs/USAGE.md)
* [개발자 가이드](docs/DEV.md)
* [트러블슈팅](docs/TROUBLESHOOTING.md)

---
  
## 라이선스 (License)

MIT License

---
  
## 링크 (Links)

* 메인 저장소: [https://github.com/kimgunwoocode/Unity-AudioSystem.git](https://github.com/kimgunwoocode/Unity-AudioSystem.git)
* unitypackage: [https://github.com/kimgunwoocode/Unity-AudioSystem/tree/Custom-Package](https://github.com/kimgunwoocode/Unity-AudioSystem/tree/Custom-Package)

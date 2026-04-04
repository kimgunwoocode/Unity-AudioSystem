# AudioManager Debug Log 가이드

AudioManager 사용 중 발생할 수 있는 로그들을 정리한 문서입니다.  
디버깅 시 어떤 문제가 발생했는지 빠르게 파악하기 위해 참고하세요.

---

## Error

### 1. AudioSystemConfig 없음
```
[AudioManager] AudioSystemConfig is null
```

- **원인**
  - `audioConfig`가 Inspector에 할당되지 않음

- **영향**
  - 모든 오디오 기능이 동작하지 않음

- **해결 방법**
  - AudioManager 오브젝트에 AudioSystemConfig 연결

---

### 2. 중복 Key
```
[AudioManager] Duplicate key detected: {key} (Skipped)
```

- **원인**
  - 동일한 key를 가진 AudioData가 여러 개 존재

- **영향**
  - 뒤에 등록된 데이터가 무시됨

- **해결 방법**
  - key를 유니크하게 관리

---

## Warning

### 1. 잘못된 AudioData
```
[AudioManager] Invalid AudioData (null or empty key)
```

- **원인**
  - AudioData가 null
  - key가 비어있거나 null

- **영향**
  - 해당 오디오 클립을 사용할 수 없음

- **해결 방법**
  - AudioData에 key 등록 확인

---

### 2. SFX Key 없음
```
[AudioManager] SFX key not found: {key}
```

- **원인**
  - 존재하지 않는 key로 SFX 재생 시도

- **영향**
  - 소리가 재생되지 않음

- **해결 방법**
  - AudioData에 key 등록 확인

---

### 3. BGM Key 없음
```
[AudioManager] BGM key not found: {key}
```

- **원인**
  - 존재하지 않는 key로 BGM 재생 시도

- **영향**
  - 소리가 재생되지 않음

- **해결 방법**
  - AudioData에 key 등록 확인

---

### 4. Audio Clip 없음
```
[AudioManager] Audio clip is null: {key}
```

- **원인**
  - AudioData에 clip이 연결되지 않음

---

### 5. Handle 찾을 수 없음
```
[AudioManager] handleID not found : {handleId}
```

- **원인**
  - 이미 종료된 SFX
  - 잘못된 handle 사용

---

### 6. SFX Pool 부족
```
[AudioManager] SFX pool empty, creating new AudioSource
```

- **원인**
  - 동시에 재생되는 SFX가 pool보다 많음

- **영향**
  - AudioSource가 추가 생성됨 (성능 영향 가능)

---

### 7. BGM 페이드 중복 호출
```
[AudioManager] BGM is already fading out
```

- **원인**
  - 페이드아웃 진행 중에 StopBGM이 다시 호출됨

---




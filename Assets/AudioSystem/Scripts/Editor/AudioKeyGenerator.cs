using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

public class AudioKeyGenerator : Editor
{
    [MenuItem("Tools/Generate Sound Keys")]
    public static void Generate()
    {
        // 1. SoundData 에셋을 찾아옵니다. (경로가 정확해야 합니다)
        // 프로젝트 창에서 해당 에셋을 클릭하고 우클릭 -> Copy Path로 경로를 확인하세요.
        string dataPath = "Assets/AudioSystem/Data/AudioDatabase.asset";
        AudioDatabase data = AssetDatabase.LoadAssetAtPath<AudioDatabase>(dataPath);

        if (data == null)
        {
            Debug.LogError($"<color=red><b>AudioDatabase 에셋을 찾을 수 없습니다! 경로를 확인하세요: {dataPath}</b></color>");
            return;
        }

        string outputPath = "Assets/AudioSystem/Data/AudioKeys.cs";
        string dir = Path.GetDirectoryName(outputPath);
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("public static class AudioKeys");
        sb.AppendLine("{");

        // BGM 클래스 생성
        sb.AppendLine("    public static class BGM");
        sb.AppendLine("    {");
        foreach (var sound in data.bgmList)
        {
            if (string.IsNullOrEmpty(sound.key)) continue;
            if (sound.clip == null) continue;
            string safeName = sound.key.Replace(" ", "_").Replace("-", "_");
            sb.AppendLine($"        public const string {safeName} = \"{sound.key}\";");
        }
        sb.AppendLine("    }");

        sb.AppendLine();

        // SFX 클래스 생성
        sb.AppendLine("    public static class SFX");
        sb.AppendLine("    {");
        foreach (var sound in data.sfxList)
        {
            if (string.IsNullOrEmpty(sound.key)) continue;
            string safeName = sound.key.Replace(" ", "_").Replace("-", "_");
            sb.AppendLine($"        public const string {safeName} = \"{sound.key}\";");
        }
        sb.AppendLine("    }");

        sb.AppendLine("}");

        File.WriteAllText(outputPath, sb.ToString());
        AssetDatabase.Refresh();
        Debug.Log("<color=green><b>AudioKeys 동기화 완료 (AudioDatabase 기반)!</b></color>");
    }
}
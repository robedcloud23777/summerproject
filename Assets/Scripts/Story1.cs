using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Story1 : MonoBehaviour
{
    public TextMeshProUGUI storyText;
    private string[] storyLines = {
        "[적 본부 깊숙한 방. 요원이 마지막 간부를 쓰러뜨리고, 인질로 잡혀있던 동료 알파에게 다가간다. 알파는 지쳐 있지만, 아직 눈에는 생기가 남아있다.]",

"요원(오메가): \"알파, 괜찮아? 이제 끝났어. 나 왔다.\"",

"알파: \"오메가... 정말... 네가 올 줄 알았어. 난... 계속 기다리고 있었어.\"",

"요원: \"미안하다, 조금 늦었지. 하지만 약속했잖아. 널 데리러 온다고.\"",

"알파: \"너무 늦지 않았어... 오메가, 네가 여기 있다는 것만으로도 충분해.\"",

"[요원이 알파의 결박을 풀고 그를 부축한다. 알파는 힘겹게 일어나지만, 몸을 가누기 어렵다.]",

"요원: \"이제 끝났다. 널 집으로 데려갈 거야. 아직 끝까지 버텨야 해.\"",

"알파: \"넌 항상 그렇더라... 마지막 순간에도 날 챙기고. 넌 정말... 멋진 파트너야.\"",

"[요원이 부드럽게 웃으며 알파를 부축해 출구 쪽으로 걸어간다. 두 사람은 여전히 경계하며 건물을 빠져나가기 시작한다.]",

"요원: \"파트너니까. 널 두고 갈 수는 없지. 우리 둘 다 살아서 여기서 나가야 하니까.\"",
"[두 사람은 적 본부의 폐허를 지나고, 저 멀리 헬기가 다가오는 소리가 들린다. 두 사람은 무사히 구출을 기다리며 어둠 속에서 걸음을 재촉한다.]",

"알파: \"이제... 정말 끝난 건가?\"",

"요원: \"그래. 이제 집에 갈 시간이다.\"",

"[요원이 마지막으로 하늘을 올려다본다. 헬기 소리가 점점 가까워지고, 두 사람은 마침내 자유를 향해 나아간다.]",   

"The End..."
    };
    private int currentLine = 0;

    void Start()
    {
        storyText.text = storyLines[currentLine];
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentLine < storyLines.Length - 1)
            {
                currentLine++;
                storyText.text = storyLines[currentLine];
            }
            else
            {
                // 마지막 대사 이후 씬 전환
                SceneManager.LoadScene("Start");  // "NextScene"은 전환할 씬의 이름
            }
        }
    }
}

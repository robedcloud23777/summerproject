using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Story : MonoBehaviour
{
    public TextMeshProUGUI storyText;
    private string[] storyLines = {
        "[어두운 밤, 요원은 적 본부 외곽에 은밀히 도착한다. 무전기에서 지휘관의 목소리가 들려온다.]",

"지휘관: \"오메가, 목표 위치에 도달했나? 확인해.\"",

"요원(오메가): \"대기 중이다. 경비가 생각보다 더 많군. 예상보다 어려울 거 같다.\"",

"지휘관: \"네 동료 알파는 적에게 중요한 인질이다. 시간을 낭비할 순 없다. 진입 지점을 찾았나?\"",

"요원: \"남쪽 방벽에 경비 인원 공백이 있다. 여기서 침투할 생각이다.\"",

"지휘관: \"좋다. 하지만 기억해, 이건 단독 임무다. 지원 없다. 네 동료의 생명은 네 손에 달려있다.\"",

"요원: \"알고 있다. 그들이 무슨 짓을 했는지, 절대 용서 못 해. 끝장을 보겠다.\"",

"지휘관: \"냉정하게 생각해. 감정에 휘둘리면 임무를 그르친다. 네 목표는 알파의 구출이 우선이다. 다른 건 필요 없다.\"",

"요원: \"알파는 내 동료다. 내가 반드시 구출해낸다. 그리고 그들에게 대가를 치르게 할 거다.\"",

"[요원은 조용히 남쪽 방벽을 넘기 시작한다. 무전기에서 마지막으로 지휘관의 목소리가 들려온다.]",

"지휘관: \"좋다. 침투 후 보고해라. 임무 시작이다. 넌 지금 적 본부 한가운데 들어간 거다. 모든 걸 조심해.\"",

"요원: \"오메가, 임무 시작. 이제 끝을 보러 간다.\"",

"[요원은 어둠 속으로 사라지며 적 본부로 침투를 시작한다.]"
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
                SceneManager.LoadScene("Main");  // "NextScene"은 전환할 씬의 이름
            }
        }
    }
}

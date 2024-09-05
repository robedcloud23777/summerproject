using UnityEngine;
using UnityEngine.UI;

public class StageProgressBar : MonoBehaviour
{
    private MapGenerator _generator;
    public Slider stageSlider;  // Slider 컴포넌트
    public Text stageText;      // Text 컴포넌트 (진행률 표시)
    public int totalStages = 3;  // 총 스테이지 수

    void Start()
    {
        // MapGenerator를 씬에서 찾아서 할당
        _generator = FindObjectOfType<MapGenerator>();

        if (_generator != null)
        {
            // Slider의 최소값, 최대값 설정
            stageSlider.minValue = 0;
            stageSlider.maxValue = totalStages;
            stageSlider.value = _generator.stage;  // 현재 스테이지로 Slider 설정

            // 초기 텍스트 업데이트
            UpdateStageText(_generator.stage);
        }
        else
        {
            Debug.LogError("MapGenerator를 찾을 수 없습니다!");
        }
    }

    private void Update()
    {
        if (_generator != null)
        {
            UpdateStageProgress(_generator.stage);
        }
    }

    public void UpdateStageProgress(int stage)
    {
        // Slider의 값을 업데이트
        stageSlider.value = stage;

        // Text UI 업데이트
        UpdateStageText(stage);
    }

    private void UpdateStageText(int stage)
    {
        // 스테이지 진행 상황을 텍스트로 표시
        float progressPercentage = ((float)stage / totalStages) * 100;
        stageText.text = $"Stage: {stage}/{totalStages}";
    }
}

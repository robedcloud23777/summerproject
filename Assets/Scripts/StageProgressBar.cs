using UnityEngine;
using UnityEngine.UI;

public class StageProgressBar : MonoBehaviour
{
    private MapGenerator _generator;
    public Slider stageSlider;  // Slider ������Ʈ
    public Text stageText;      // Text ������Ʈ (����� ǥ��)
    public int totalStages = 3;  // �� �������� ��

    void Start()
    {
        // MapGenerator�� ������ ã�Ƽ� �Ҵ�
        _generator = FindObjectOfType<MapGenerator>();

        if (_generator != null)
        {
            // Slider�� �ּҰ�, �ִ밪 ����
            stageSlider.minValue = 0;
            stageSlider.maxValue = totalStages;
            stageSlider.value = _generator.stage;  // ���� ���������� Slider ����

            // �ʱ� �ؽ�Ʈ ������Ʈ
            UpdateStageText(_generator.stage);
        }
        else
        {
            Debug.LogError("MapGenerator�� ã�� �� �����ϴ�!");
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
        // Slider�� ���� ������Ʈ
        stageSlider.value = stage;

        // Text UI ������Ʈ
        UpdateStageText(stage);
    }

    private void UpdateStageText(int stage)
    {
        // �������� ���� ��Ȳ�� �ؽ�Ʈ�� ǥ��
        float progressPercentage = ((float)stage / totalStages) * 100;
        stageText.text = $"Stage: {stage}/{totalStages}";
    }
}

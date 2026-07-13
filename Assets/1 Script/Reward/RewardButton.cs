using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 레벨업 보상 카드 하나를 관리하는 스크립트
// 예: 공격력 증가 카드, 이동속도 증가 카드
public class RewardButton : MonoBehaviour
{
    [Header("UI")]

    // 카드 전체를 클릭할 수 있게 하는 Button
    // RewardCard 오브젝트 자신에게 붙어 있는 Button을 연결
    [SerializeField] private Button cardButton;

    // 보상 아이콘을 보여주는 Image
    [SerializeField] private Image iconImage;

    // 보상 이름을 보여주는 Text
    [SerializeField] private TMP_Text rewardNameText;

    // 보상 설명을 보여주는 Text
    [SerializeField] private TMP_Text descriptionText;

    // 현재 이 카드에 들어 있는 보상 데이터
    private RewardData rewardData;

    // 카드를 클릭했을 때 실행할 함수
    // RewardManager의 SelectReward 함수를 전달받게 됨
    private Action<RewardData> selectAction;

    private void Awake()
    {
        // Inspector에서 cardButton을 연결하지 않았다면
        // 현재 오브젝트에서 Button 컴포넌트를 자동으로 찾음
        if (cardButton == null)
        {
            cardButton = GetComponent<Button>();
        }
    }

    // RewardManager가 보상 카드를 만들 때 호출하는 함수
    // data에는 표시할 보상 데이터가 들어오고
    // action에는 보상 선택 후 실행할 함수가 들어옴
    public void SetReward(
        RewardData data,
        Action<RewardData> action)
    {
        // 전달받은 보상 데이터를 저장
        rewardData = data;

        // 보상을 선택했을 때 실행할 함수 저장
        selectAction = action;

        // RewardData의 내용을 UI에 표시
        rewardNameText.text = data.rewardName;
        descriptionText.text = data.description;
        iconImage.sprite = data.icon;

        // 이전에 등록된 클릭 이벤트 제거
        // 보상창을 다시 열 때 이벤트가 중복 등록되는 것을 방지
        cardButton.onClick.RemoveAllListeners();

        // 카드 클릭 시 SelectReward 함수 실행
        cardButton.onClick.AddListener(SelectReward);
    }

    // 카드가 클릭되었을 때 실행
    private void SelectReward()
    {
        Debug.Log("보상 카드 클릭됨");

        // 보상 데이터가 없으면 선택할 수 없음
        if (rewardData == null)
        {
            Debug.LogError("RewardData가 없습니다.");
            return;
        }

        if (selectAction == null)
        {
            Debug.LogError("selectAction이 연결되지 않았습니다.");
            return;
        }

        // 저장해 둔 함수가 있다면 실행
        // rewardData를 RewardManager에 전달
        selectAction?.Invoke(rewardData);
    }
}
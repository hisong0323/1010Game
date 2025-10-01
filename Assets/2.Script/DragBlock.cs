using DG.Tweening;
using System.Collections;
using UnityEngine;

public class DragBlock : MonoBehaviour
{
    [field: SerializeField] public Vector2Int BlockCount { private set; get; }
    public Color Color { private set; get; } //블록 색상
    public Vector3[] ChildBlocks { private set; get; } //자식 블록들의 지역좌표

    private BlockArrangeSystem blockArrangeSystem;

    private bool pick = false;

    private void Start()
    {
        StageController.Instance.Pause += OnPause;
    }

    private void OnPause()
    {
        if (pick)
        {
            // 현재 크기에서 0.5 크기로 축소
            transform.DOScale(Vector3.one * 0.5f, 0.1f);
            // 현재 위치에서 부모 오브젝트 위치로 이동
            transform.DOMove(transform.parent.position, 0.1f);
            SoundManager.Instance.PlayDropSound();
        }
    }

    public void Setup(BlockArrangeSystem blockArrangeSystem)
    {
        this.blockArrangeSystem = blockArrangeSystem;

        transform.localScale = Vector3.zero;
        Color = GetComponentInChildren<SpriteRenderer>().color; //자식 블록은 모두 같은 색상이기 때문에 자식 블록 중 누구의 색상을 가져와도 상관없다.

        //블록의 모양에 따라 자식 개수가 다르기 때문에 자식 개수만큼 배열 방을 생성하고, 모든 자식 오브젝트의 지역 좌표를 저장
        ChildBlocks = new Vector3[transform.childCount];
        for (int i = 0; i < ChildBlocks.Length; ++i)
        {
            ChildBlocks[i] = transform.GetChild(i).localPosition;
        }

        transform.DOScale(0.5f, 0.1f);
    }

    // <summary>
    // 해당 오브젝트를 클릭할 때 1회 호출
    //<summary>
    private void OnMouseDown()
    {
        if (StageController.Instance.pause)
            return;
        pick = true;
        transform.DOKill();
        transform.DOScale(Vector3.one, 0.1f);
        SoundManager.Instance.PlayPickSound();
    }

    // <summary>
    // 해당 오브젝트의 클릭을 종료할 때 1회 호출
    // </summary>
    private void OnMouseUp()
    {
        if (StageController.Instance.pause)
            return;
        pick = false;
        // 자식 블록 개수가 홀수, 짝수 일 때 다르게 게산
        // 값을 반올림 하는 Mathf.RoundToInt()를 이용해 블록을 배경블록판이 스냅(Snap)해서 배치
        float x = Mathf.RoundToInt(transform.position.x - BlockCount.x % 2 * 0.5f) + BlockCount.x % 2 * 0.5f;
        float y = Mathf.RoundToInt(transform.position.y - BlockCount.y % 2 * 0.5f) + BlockCount.y % 2 * 0.5f;

        transform.position = new Vector3(x, y, 0);

        // 현재 위치에 블록을 배치할 수 있는지 검사하고 결과를 반환
        bool isSuccess = blockArrangeSystem.TryArrangementBlock(this);

        // 현재 위치에 블록을 배치할 수 없으면 마지막 위치, 크기로 설정
        if (isSuccess == false)
        {
            // 현재 크기에서 0.5 크기로 축소
            transform.DOScale(Vector3.one * 0.5f, 0.1f);
            // 현재 위치에서 부모 오브젝트 위치로 이동
            transform.DOMove(transform.parent.position, 0.1f);
        }

        SoundManager.Instance.PlayDropSound();
    }

    // <summary>
    // 해당 오브젝트를 드래그할 때 매 프레임 호출
    // </summary>
    private void OnMouseDrag()
    {
        if (StageController.Instance.pause)
            return;

        // 현재 모든 블록은 Povot이 블록셋의 정중앙으로 설정되어 있기 때문에 x위치는 그대로 사용하고,

        // y 위치는 y축 블록 개수의 절반(BlockCount.y * 0.5f)에 gap만큼 추가한 위치로 사용

        // Camera.main,ScreenTOWorldPoint()로 Vector3 좌표를 구하면 z 값은 카메라의 위치인 -18이 나오기 때문에
        Vector3 gap = new Vector3(0, BlockCount.y * 0.5f + 1, 10);
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + gap;
    }

    private void OnDestroy()
    {
        StageController.Instance.Pause -= OnPause;
    }
}

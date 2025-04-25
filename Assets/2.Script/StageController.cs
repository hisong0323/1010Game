using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StageController : MonoBehaviour
{
    [SerializeField] private BackgroundBlockSpawner backgroundBlockSpawner; // ��� ��� ����
    [SerializeField] private BackgroundBlockSpawner foregroundBlockSpawner; // ��� ��� ����
    [SerializeField] private DragBlockSpawner dragBlockSpawner; //�巡�� ��� ����
    [SerializeField] private BlockArrengeSystem blockArrangeSystem; // ��� ��ġ
    [SerializeField] private UIController uiController; // ���ӿ��� �Ǿ��� �� UIȰ��ȭ

    public int CurrentScore { private set; get; } // ���� ����
    public int HighScore { private set; get; } // �ְ� ����

    private BackgroundBlock[] backgroundBlocks; // ������ ��� ��� ���� ����
    private int currentDragBlockCount; // ���� �����ִ� �巡�� ��� ����

    private readonly Vector2Int blockCount = new Vector2Int(10, 10); // ��� �ǿ� ��ġ�Ǵ� ��� ����
    private readonly Vector2 blockHalf = new Vector2(0.5f, 0.5f); // ��� �ϳ��� ���� ũ��
    private readonly int maxDragBlockCount = 3; // �� ���� ������ �� �ִ� �巡�� ��� ����

    private List<BackgroundBlock> filledBlockList = new(); // ���� �ϼ��� ��ϵ��� �����ϱ� ���� �ӽ� �����ϴ� ����Ʈ
    private void Awake()
    {
        // ���� �ʱ�ȭ (���� ����, �ְ� ����)
        CurrentScore = 0;
        HighScore = PlayerPrefs.GetInt("HighScore");

        // �� ������� ���Ǵ� ��� ��ϸ� ����
        backgroundBlockSpawner.SpawnBlocks(blockCount, blockHalf);

        // �巡�� ����� ��ġ�� �� ������ ����Ǵ� ��� ����� ����
        backgroundBlocks = new BackgroundBlock[blockCount.x * blockCount.y];
        backgroundBlocks = foregroundBlockSpawner.SpawnBlocks(blockCount, blockHalf);

        // ��� ��ġ �ý���
        blockArrangeSystem.Setup(blockCount, blockHalf, backgroundBlocks, this);
        // �巡�� ��� ����
        StartCoroutine(SpawnDragBlocks());
    }

    private IEnumerator SpawnDragBlocks()
    {
        // ���� �巡�� ����� ������ �ִ�(3)�� ����
        currentDragBlockCount = maxDragBlockCount;
        // �巡�� ��� ����
        dragBlockSpawner.SpawnBlocks();

        // �巡�� ��ϵ��� �̵��� �Ϸ�� ������ ���
        yield return new WaitUntil(() => IsCompleteSpawnBlocks());
    }

    /// <summary>
    /// �巡�� ����� �����ϰ�, ���� �ִϸ��̼��� ����� ��
    /// ��� �׷��� ����� ���� �ִϸ��̼��� ����Ǿ����� �˻�
    /// </summary>
    private bool IsCompleteSpawnBlocks()
    {
        int count = 0;
        for (int i = 0; i < dragBlockSpawner.BlockSpawPoints.Length; ++i)
        {
            if (dragBlockSpawner.BlockSpawPoints[i].childCount != 0 &&
                dragBlockSpawner.BlockSpawPoints[i].GetChild(0).localPosition == Vector3.zero)
            {
                count++;
            }
        }

        return count == dragBlockSpawner.BlockSpawPoints.Length;
    }

    // <summary>
    // ��� ��ġ ��ó��
    // �巡�� ��� ����/����, �� �ϼ�, ���ӿ���, ����
    // </summary>
    public void AfterBlockArrangement(DragBlock block)
    {
        StartCoroutine("OnAfterBlockArrangement", block);
    }

    /// <summary>
    /// ��� ��ġ ��ó��
    /// �巡�� ��� ����, ����� ���� Ȯ��, �� �ϼ�, ���ӿ���, ����
    /// </summary>
    private IEnumerator OnAfterBlockArrangement(DragBlock block)
    {
        // ��ġ�� �Ϸ�� �巡�� ��� ����
        Destroy(block.gameObject);

        // �ϼ��� ���� �ִ��� �˻��ϰ�, �ϼ��� ���� ��ϵ��� ������ ����
        int filledLineCount = CheckFilledLine();

        // �ϼ��� ���� ������ 0��, �ϼ��� ���� ������ 2�� fiiledLineCount�� * 10�� (10, 20, 40, 80...)
        int LineScore = filledLineCount == 0 ? 0 : (int)Mathf.Pow(2, filledLineCount - 1) * 10;

        // ���� ��� (��� ���� + ���� ����)
        CurrentScore += block.ChildBlocks.Length + LineScore;

        // ���� �ϼ��� ��ϵ��� ���� (�������� ��ġ�� ����� �������� ������������ ����)
        yield return StartCoroutine(DestroyFilledBlocks(block));

        // ��� ��ġ�� ���������� ���� �����ִ� �巡�� ����� ������ 1 ����
        currentDragBlockCount--;
        //���� ��ġ ������ �巡�� ����� ������ 0�̸� �巡�� ��� ����
        if (currentDragBlockCount == 0)
        {
            //SpawnDragBlocks();
            yield return StartCoroutine(SpawnDragBlocks());
        }

        // ���� �������� ����� ������ ���
        yield return new WaitForEndOfFrame();

        // ���� ������ �������� �˻�
        if (IsGameOver())
        {
            // Debug.Log("GameOver");

            // ���� ������ �ְ� �������� ������ ���� ������ �ְ� ������ ����
            if (CurrentScore > HighScore)
            {
                PlayerPrefs.SetInt("HighScore", CurrentScore);
            }
            // ���ӿ��� �Ǿ��� �� ����ϴ� Panel UI�� Ȱ��ȭ�ϰ�, ��ũ����, ���� �� ����
            uiController.GameOver();
        }

    }

    private int CheckFilledLine()
    {
        int filledLineCount = 0;
        filledBlockList.Clear();

        // ���� �� �˻�
        for (int y = 0; y < blockCount.y; ++y)
        {
            int fillBlockCount = 0;
            for (int x = 0; x < blockCount.x; ++x)
            {
                // �ش� ����� ä���� ������ fillBlockCount 1 ����
                if (backgroundBlocks[y * blockCount.x + x].BlockState == BlockState.Fill) fillBlockCount++;
            }

            // �ϼ��� ���� ������ �ش� ���� ��� ��� ����� filledBlockList�� ����
            if (fillBlockCount == blockCount.x)
            {
                for (int x = 0; x < blockCount.x; ++x)
                {
                    filledBlockList.Add(backgroundBlocks[y * blockCount.x + x]);
                }
                filledLineCount++;
            }

        }

        // ���� �� �˻�
        for (int x = 0; x < blockCount.x; ++x)
        {
            int fillBlockCount = 0;
            for (int y = 0; y < blockCount.y; ++y)
            {
                // �ش� ����� ä���� ������ fillBlockCount 1 ����
                if (backgroundBlocks[y * blockCount.x + x].BlockState == BlockState.Fill) fillBlockCount++;
            }

            // �ϼ��� ���� ������ �ش� ���� ��� ��� ����� filledBlockList�� ����
            if (fillBlockCount == blockCount.y)
            {
                for (int y = 0; y < blockCount.y; ++y)
                {
                    filledBlockList.Add(backgroundBlocks[y * blockCount.x + x]);
                }
                filledLineCount++;
            }
        }

        return filledLineCount;
    }

    private IEnumerator DestroyFilledBlocks(DragBlock block)
    {
        // �������� ��ġ�� ���(block)�� �Ÿ��� ����� ������ ����
        filledBlockList.Sort((a, b) =>
        (a.transform.position - block.transform.position).sqrMagnitude.CompareTo((b.transform.position - block.transform.position).sqrMagnitude));

        // filledBlockList�� ����Ǿ� �֤��� ��� ����� ������� �ʱ�ȭ
        for (int i = 0; i < filledBlockList.Count; ++i)
        {
            filledBlockList[i].EmptyBlock();

            yield return new WaitForSeconds(0.01f);
        }
        filledBlockList.Clear();
    }

    private bool IsGameOver()
    {
        int dragBlockCount = 0;

        // ��ġ ������ �巡�� ����� �������� ��
        for (int i = 0; i < dragBlockSpawner.BlockSpawPoints.Length; ++i)
        {
            // dragBlockSpawner.BlockSpawnPoints[i]�� �ڽ��� ������ (�ڽ� = �巡�� ���)
            if (dragBlockSpawner.BlockSpawPoints[i].childCount != 0)
            {
                dragBlockCount++;

                // ����� ��ġ�� ���� ���������� �巡�� ����� ������ �������� �ڽ� ��ϵ��� ��ġ�� �� �ִ� ������ �ִ��� �˻�
                // IsPossibleArrangement()�� ��true�� ��ȯ�ϸ� ����� ��ġ�� �� �ִٸ� ������ IsGameOver()�� false ��ȯ�� ���� ����
                if (blockArrangeSystem.IsPossibleArrangement(dragBlockSpawner.BlockSpawPoints[i].GetComponentInChildren<DragBlock>()))
                {
                    return false;
                }
            }
        }

        // dragBlockCount�� ���� �����ִ� �巡�� ����� �����ε� �ʿ� ��ġ�� �� �մ� �巡�� ����� ������
        // if (IsPossibleArrangement())���� true�� �޼ҵ带 ���������� ������
        // �� ��ȯ �ڵ尡 ����ǰ�, dragBlockCount�� 0�� �ƴϸ� ���ӿ���
        return dragBlockCount != 0;
    }
}
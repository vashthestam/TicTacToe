using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Board : MonoBehaviour
{
    [Header("InputSettings:")]
    [SerializeField] private LayerMask boxesLayerMask;
    [SerializeField] private float touchRadius;

    [Header("Mask Sprite:")]
    [SerializeField] private Sprite spriteA;
    [SerializeField] private Sprite spriteB;
    [SerializeField] private Sprite spriteAI;

    [Header("Mask Color:")]
    [SerializeField] private Color colorA;
    [SerializeField] private Color colorB;
    [SerializeField] private Color colorAI;

    public UnityAction<Mark, Color> OnWinAction;
    //public UnityEvent<Mark, Color> OnWinAction2;

    public Mark[] marks;

    private LineRenderer lineRenderer;

    private Camera cam;
    private Mark currentMark;

    public bool isAIMode;
    private bool canPlay;

    private string boxPtah = "Box";

    private int marksCount = 0;

    public List<int> allList = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
    private List<int> playerList = new List<int>();
    private List<int> aiList = new List<int>();

    private void Start()
    {
        cam = Camera.main;
        currentMark = Mark.PlayerA;
        marks = new Mark[9];

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;

        canPlay = true;
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0) && canPlay)
        {
            Vector2 touchPosition = cam.ScreenToWorldPoint(Input.mousePosition);

            Collider2D hit = Physics2D.OverlapCircle(touchPosition, touchRadius, boxesLayerMask);

            if (hit)
            {
                HitBox(hit.GetComponent<Box>());
            }
        }
    }

    private void HitBox(Box box)
    {
        if (!box.isMarked)
        {
            marks[box.index] = currentMark;

            box.SetMark(GetSprite(), currentMark, GetColor());

            allList.Remove(box.index);
            playerList.Add(box.index);

            bool won = CheckIfWin();

            marksCount++;

            if (won)
            {
                canPlay = false;
                if (OnWinAction != null)
                    OnWinAction.Invoke(currentMark, GetColor());
                return;
            }

            if (marksCount == 9)
            {
                canPlay = false;
                OnWinAction.Invoke(Mark.None, Color.white);
            }

            switchPlayer();
        }
    }

    private void AIPlay()
    {
        int[] corner;
        int aiIndex = 0;

        if (marksCount == 1)
        {
            if (marks[4] == Mark.None)
            {
                aiIndex = 4;
            }
            else
            {
                corner = new int[4] { 0, 2, 6, 8 };
                aiIndex = corner[Random.Range(0, 4)];
            }
        }
        else if (marksCount == 3)
        {
            int loseNum = -1;

            foreach (int num in allList)
            {
                if (
                    CheckIfAiWillWin(num, 0, 1, 2) || CheckIfAiWillWin(num, 3, 4, 5) || CheckIfAiWillWin(num, 6, 7, 8) ||
                    CheckIfAiWillWin(num, 0, 3, 6) || CheckIfAiWillWin(num, 1, 4, 7) || CheckIfAiWillWin(num, 2, 5, 8) ||
                    CheckIfAiWillWin(num, 0, 4, 8) || CheckIfAiWillWin(num, 2, 4, 6)
                    )
                {
                    aiIndex = num;
                    break;
                }
                else if (
                    CheckIfPlayerWillWin(num, 0, 1, 2) || CheckIfPlayerWillWin(num, 3, 4, 5) || CheckIfPlayerWillWin(num, 6, 7, 8) ||
                    CheckIfPlayerWillWin(num, 0, 3, 6) || CheckIfPlayerWillWin(num, 1, 4, 7) || CheckIfPlayerWillWin(num, 2, 5, 8) ||
                    CheckIfPlayerWillWin(num, 0, 4, 8) || CheckIfPlayerWillWin(num, 2, 4, 6)
                    )
                {
                    loseNum = num;
                }
                else
                {
                    if ((playerList.Contains(0) && playerList.Contains(8)) ||
                        (playerList.Contains(2) && playerList.Contains(6)))
                    {
                        //{ 1, 3, 5, 7 };
                        aiIndex = Random.Range(1, 4) * 2 - 1;
                    }
                    else if (
                        (playerList.Contains(1) && playerList.Contains(3)) ||
                        (playerList.Contains(1) && playerList.Contains(5)) ||
                        (playerList.Contains(3) && playerList.Contains(7)) ||
                        (playerList.Contains(5) && playerList.Contains(7))
                        )
                    {
                        aiIndex = playerList[0] + playerList[1] - 4;
                    }
                    else if (
                        (playerList.Contains(1) && playerList.Contains(7)) ||
                        (playerList.Contains(3) && playerList.Contains(5))
                        )
                    {
                        corner = new int[4] { 0, 2, 6, 8 };
                        aiIndex = corner[Random.Range(0, 4)];
                    }
                    else
                    {
                        aiIndex = allList[Random.Range(0, allList.Count)];
                    }
                }

                if (loseNum != -1)
                {
                    aiIndex = loseNum;
                }
            }


        }
        else
        {
            int loseNum = -1;

            foreach (int num in allList)
            {
                if (
                    CheckIfAiWillWin(num, 0, 1, 2) || CheckIfAiWillWin(num, 3, 4, 5) || CheckIfAiWillWin(num, 6, 7, 8) ||
                    CheckIfAiWillWin(num, 0, 3, 6) || CheckIfAiWillWin(num, 1, 4, 7) || CheckIfAiWillWin(num, 2, 5, 8) ||
                    CheckIfAiWillWin(num, 0, 4, 8) || CheckIfAiWillWin(num, 2, 4, 6)
                    )
                {
                    aiIndex = num;
                    break;
                }
                else if (
                    CheckIfPlayerWillWin(num, 0, 1, 2) || CheckIfPlayerWillWin(num, 3, 4, 5) || CheckIfPlayerWillWin(num, 6, 7, 8) ||
                    CheckIfPlayerWillWin(num, 0, 3, 6) || CheckIfPlayerWillWin(num, 1, 4, 7) || CheckIfPlayerWillWin(num, 2, 5, 8) ||
                    CheckIfPlayerWillWin(num, 0, 4, 8) || CheckIfPlayerWillWin(num, 2, 4, 6)
                    )
                {
                    loseNum = num;
                }
                else
                {
                    aiIndex = allList[Random.Range(0, allList.Count)];
                }

                if (loseNum != -1)
                {
                    aiIndex = loseNum;
                }
            }
        }

        Box AIBox = transform.Find(boxPtah + aiIndex).gameObject.GetComponent<Box>();

        if (!AIBox.isMarked)
        {
            allList.Remove(aiIndex);
            aiList.Add(aiIndex);

            marks[aiIndex] = Mark.AI;
            AIBox.SetMark(spriteAI, Mark.AI, colorAI);
        }

        bool won = CheckIfWin();

        marksCount++;

        if (won)
        {
            canPlay = false;
            if (OnWinAction != null)
                OnWinAction.Invoke(currentMark, GetColor());
            return;
        }

        if (marksCount == 9)
        {
            canPlay = false;
            OnWinAction.Invoke(Mark.None, Color.white);
        }

        switchPlayer();
    }

    private bool CheckIfWin()
    {
        return 
        BoexesMatched(0, 1, 2) || BoexesMatched(3, 4, 5) || BoexesMatched(6, 7, 8) ||
        BoexesMatched(0, 3, 6) || BoexesMatched(1, 4, 7) || BoexesMatched(2, 5, 8) ||
        BoexesMatched(0, 4, 8) || BoexesMatched(2, 4, 6);
    }

    private bool CheckIfAiWillWin(int num, int i, int j, int k)
    {
        if (num == i)
        {
            if (aiList.Contains(j) && aiList.Contains(k))
                return true;
        }
        else if (num == j)
        {
            if (aiList.Contains(i) && aiList.Contains(k))
                return true;
        }
        else if (num == k)
        {
            if (aiList.Contains(i) && aiList.Contains(j))
                return true;
        }

        return false;
    }

    private bool CheckIfPlayerWillWin(int num, int i, int j, int k)
    {
        if (num == i)
        {
            if (playerList.Contains(j) && playerList.Contains(k))
                return true;
        }
        else if (num == j)
        {
            if (playerList.Contains(i) && playerList.Contains(k))
                return true;
        }
        else if (num == k)
        {
            if (playerList.Contains(i) && playerList.Contains(j))
                return true;
        }

        return false;
    }

    private bool BoexesMatched(int i, int j, int k)
    {
        Mark m = currentMark;

        bool matched = (marks[i] == m && marks[j] == m && marks[k] == m);

        if(matched)
            drawLine(i, k);

        return matched;
    }

    private void drawLine(int i, int j)
    {
        lineRenderer.SetPosition(0, transform.Find(boxPtah + i).position);
        lineRenderer.SetPosition(1, transform.Find(boxPtah + j).position);

        Color color = GetColor();

        lineRenderer.startColor = color;
        lineRenderer.endColor = color;

        lineRenderer.enabled = true;
    }

    private void switchPlayer()
    {
        if (!isAIMode)
            currentMark = (currentMark == Mark.PlayerA) ? Mark.PlayerB : Mark.PlayerA;
        else 
        {
            currentMark = (currentMark == Mark.PlayerA) ? Mark.AI : Mark.PlayerA;
        }

        if (currentMark == Mark.AI)
            AIPlay();
    }

    private Sprite GetSprite()
    {
        return (currentMark == Mark.PlayerA) ? spriteA : spriteB;
    }

    private Color GetColor()
    {
        return (currentMark == Mark.PlayerA) ? colorA : colorB;
    }
}

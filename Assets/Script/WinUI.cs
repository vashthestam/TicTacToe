using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WinUI : MonoBehaviour
{
    [Header("UI References:")]
    [SerializeField] private GameObject uiCanvas;
    [SerializeField] private Text uiWinnerText;
    [SerializeField] private Button uiRestartButton;

    [Header("Board References:")]
    [SerializeField] Board board;

    [Header("Title UI References:")]
    [SerializeField] private GameObject titleCanvas;
    [SerializeField] private Button PvpButton;
    [SerializeField] private Button AIButton;

    private void Start()
    {
        uiRestartButton.onClick.AddListener(()=>SceneManager.LoadScene(0));
        PvpButton.onClick.AddListener(vsPerson);
        AIButton.onClick.AddListener(vsAI);
        board.OnWinAction += OnWinEvent;
        uiCanvas.SetActive(false);
        titleCanvas.SetActive(true);
    }

    public void OnWinEvent(Mark mark, Color color)
    {
        uiWinnerText.text = (mark == Mark.None) ? "Nobady Win": mark.ToString() + " Win!!!";
        uiWinnerText.color = color;

        uiCanvas.SetActive(true);
    }

    private void OnDestroy()
    {
        uiRestartButton.onClick.RemoveAllListeners();
        board.OnWinAction -= OnWinEvent;
    }

    public void vsPerson()
    {
        board.isAIMode = false;
        titleCanvas.SetActive(false);
    }

    public void vsAI()
    {
        board.isAIMode = true;
        titleCanvas.SetActive(false);
    }
}

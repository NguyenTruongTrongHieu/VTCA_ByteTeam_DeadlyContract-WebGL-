using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

enum PlayMode 
{ 
    none,
    playing,
    pause,
    puzzle,
    cooking,
}

public class GamePlayManager : MonoBehaviour
{
    public static GamePlayManager instance;

    [Header("Heart and Dollars")]
    [SerializeField] private Text textHeart;
    [SerializeField] private Text textDollars;
    public PanController panController;

    [Header("Button Play Mode")]
    [SerializeField] private Button btnPuzzle;
    [SerializeField] private Button btnFood;
    [SerializeField] private Button btnIDCard;
    private PlayMode playMode = PlayMode.none;
    private PlayMode prePlayMode = PlayMode.none;

    [Header("Button Puzzle Size")]
    [SerializeField] private GameObject choosingSize;
    [SerializeField] private Button btnCom;
    [SerializeField] private Button btnSize5;
    [SerializeField] private Button btnSize4;
    [SerializeField] private Button btnSize3;

    [Header("Puzzle Component")]
    [SerializeField] private RectTransform itemZone;
    [SerializeField] private List<GameObject> lstItem;
    private List<GameObject> lstCrrItem;

    [SerializeField] private GameObject puzzle5;
    [SerializeField] private GameObject puzzle4;
    [SerializeField] private GameObject puzzle3;

    [Header("Customer Component")]
    [SerializeField] private GameObject customerObject;
    [SerializeField] private GameObject anhicarObject;
    [SerializeField] private List<Sprite> lstImgCustomer;
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private Vector2 customerPointA;
    [SerializeField] private Vector2 customerPointB;

    [Header("ID Card Component")]
    [SerializeField] private GameObject idCardObject;
    [SerializeField] private Text textIdName;
    [SerializeField] private Text textIdAge;
    [SerializeField] private Text textIdAddress;
    [SerializeField] private Button btnSell;
    [SerializeField] private Button btnDontSell;

    private string[] names = { "Nguyễn Văn A", "Trần Thị B", "Lê Minh C", "Phạm Thị D", "Vũ Quốc E",
                               "Đỗ Thiên F", "Bùi Hải G", "Hoàng Đức H", "Dương Thị I", "Mai Thị K" };

    private int[] ages = { 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31 };

    private string[] addresses = { "Hà Nội", "Hồ Chí Minh", "Đà Nẵng", "Hải Phòng", "Cần Thơ",
                                   "Nha Trang", "Hạ Long", "Vũng Tàu", "Huế", "Quảng Ninh" };

    [Header("Conversation Component")]
    [SerializeField] private GameObject conversationObject;
    [SerializeField] private Text textConversation;
    private string customerRequire = "none";
    string[] requiredModes = { "puzzle", "hamburger", "beer" };
    private GameObject crrBorder = null;

    [Header("Time Countdown")]
    [SerializeField] private Text textCountdown;
    [SerializeField] private Button btnPause;

    [Header("Component Cooking")]
    [SerializeField] private GameObject cookingObject;
    [SerializeField] private Button btnStartCooking;
    [SerializeField] private List<GameObject> lstItemCooking;
    [SerializeField] private Transform foodDrop;

    [Header("Component Time Over | Notification")]
    [SerializeField] private GameObject timeOverObject;
    [SerializeField] private Text textTotalDollars;
    [SerializeField] private Text textTotalHeart;
    [SerializeField] private Text textTitle;
    [SerializeField] private Text textButton;

    public Button tieptuc;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        btnPuzzle.onClick.AddListener(GenerateCustomerAndItems);
        btnIDCard.onClick.AddListener(CheckIDCard);
        btnFood.onClick.AddListener(ChooseCookingGame);
        tieptuc.onClick.AddListener(tuiepyc);

        // Puzzle size
        lstCrrItem = new List<GameObject>();
        btnSize5.onClick.AddListener(() => GeneratePuzzleSize(5));
        btnSize4.onClick.AddListener(() => GeneratePuzzleSize(4));
        btnSize3.onClick.AddListener(() => GeneratePuzzleSize(3));
        btnCom.onClick.AddListener(CompletePuzzle);

        // Button for beer required
        btnSell.onClick.AddListener(SellBeer);
        btnDontSell.onClick.AddListener(DontSellBeer);

        // Pause game
        btnPause.onClick.AddListener(PauseGame);

        // Start cooking
        btnStartCooking.onClick.AddListener(StartCooking);

        // Turn off customer
        customerObject.SetActive(false);
        // Turn off id card
        idCardObject.SetActive(false);

        if (playMode == PlayMode.none)
        {
            // Start time
            StartCountdownTime(60);

            // Grenerate customer
            SpawnCustomer();

            // Change play mode
            playMode = PlayMode.playing;
            return;
        }
    }

    private void LoseGame()
    {
        if (int.Parse(textHeart.text) <= 0)
        {
            // Setup timeup component
            timeOverObject.SetActive(true);
            textTotalDollars.gameObject.SetActive(false);
            textTotalHeart.text = "Độ uy tín: " + textHeart.text;
            textTitle.text = "Cửa hàng bị phá sản!";
            textButton.text = "Chơi lại";
            //Dung game
            Time.timeScale = 0;
        }
    }

    public void tuiepyc()
    {
        SceneManager.LoadScene("PlayScene");
        Time.timeScale = 1;
    }
    public void GenerateCustomerAndItems()
    {
        ResetPlayModeAndTurnOffMiniGame();
        choosingSize.SetActive(true);
    }

    private void SpawnCustomer()
    {
        waveManager.SimpleWave(customerObject, customerPointA, customerPointB, 15, 3, 10);

        // Remove items in box
        for (int i = 0; i < lstCrrItem.Count; i++)
        {
            Destroy(lstCrrItem[i]);
        }

        // Reset list current item
        lstCrrItem.Clear();

        // Turn off id card
        idCardObject.SetActive(false);

        // Random customer
        customerObject.SetActive(true);
        customerObject.GetComponent<Image>().sprite = lstImgCustomer[Random.Range(0, lstImgCustomer.Count)];
       
        // Random in4
        string randomName = names[Random.Range(0, names.Length)];
        int randomAge = ages[Random.Range(0, ages.Length)];
        string randomAddress = addresses[Random.Range(0, addresses.Length)];

        textIdName.text = "Tên: " + randomName;
        textIdAge.text = "Tuổi: " + randomAge.ToString();
        textIdAddress.text = "Quê quán: " + randomAddress;

        // Random conversation
        customerRequire = requiredModes[Random.Range(0, requiredModes.Length)];
        conversationObject.SetActive(false);

        // line for testing
       // customerRequire = "puzzle";
        switch (customerRequire)
        {
            case "puzzle":
                textConversation.text = "Gói đồ lại cho tôi";
                SpawnItemInZone(Random.Range(3, 5));
                break;

            case "hamburger":
                textConversation.text = "Bán cho tôi 1 cái hamburger";
                break;

            case "beer":
                textConversation.text = "Bán bia cho tôi";
                break;
        }
    }

    private void SpawnItemInZone(int n)
    {
        Vector2 zoneSize = itemZone.rect.size;
        Vector2 zoneCenter = itemZone.position;

        for (int i = 0; i < n; i++)
        {
            Vector2 randomPosition = new Vector2(
                Random.Range(-zoneSize.x / 2, zoneSize.x / 2),
                Random.Range(-zoneSize.y / 2, zoneSize.y / 2)
            );

            GameObject newPrefab = Instantiate(lstItem[Random.Range(0, lstItem.Count)], itemZone);
            newPrefab.transform.localPosition = randomPosition;

            newPrefab.transform.localScale = Vector3.one;

            lstCrrItem.Add(newPrefab);
        }
    }

    private void GeneratePuzzleSize(int size)
    {
        Debug.Log("GeneratePuzzleSize: " + size);

        // Turn off choosing size and change mode
        choosingSize.SetActive(false);
        playMode = PlayMode.puzzle;

        // Active button complete puzzle
        btnCom.gameObject.SetActive(true);

        switch (size)
        {
            case 3:
                puzzle3.SetActive(true);
                crrBorder = puzzle3;
                textDollars.text = (int.Parse(textDollars.text) - 5).ToString();
                break;

            case 4:
                puzzle4.SetActive(true);
                crrBorder = puzzle4;
                textDollars.text = (int.Parse(textDollars.text) - 10).ToString();
                break;

            case 5:
                puzzle5.SetActive(true);
                crrBorder = puzzle5;
                textDollars.text = (int.Parse(textDollars.text) - 15).ToString();
                break;
        }
    }
    public Border GetCurrentBorder()
    {
        if (crrBorder == null) return null;
        return crrBorder.GetComponent<Border>();
    }

    private void CheckIDCard()
    {
        ResetPlayModeAndTurnOffMiniGame();

        if (playMode == PlayMode.playing)
        {
            idCardObject.SetActive(true);
            anhicarObject.GetComponent<Image>().sprite = customerObject.GetComponent<Image>().sprite;

        }
    }

    private void SellBeer()
    {
        if (customerRequire == "beer" && int.Parse(textIdAge.text.Substring(6)) >= 18)
        {
            AudioManager.Instance.PlayCompleteSound();
            textDollars.text = (int.Parse(textDollars.text) + 50).ToString();
            SpawnCustomer();
        }
        else
        {
            textHeart.text = Mathf.Max(int.Parse(textHeart.text) - 10, 0).ToString();
            SpawnCustomer();
        }
        ResetPlayModeAndTurnOffMiniGame();
    }

    private void DontSellBeer()
    {
        if (customerRequire == "beer" && int.Parse(textIdAge.text.Substring(6)) < 18)
        {
            textHeart.text = Mathf.Min(int.Parse(textHeart.text) + 10, 100).ToString();
            SpawnCustomer();
        }
        else
        {
            textHeart.text = Mathf.Max(int.Parse(textHeart.text) - 10, 0).ToString();
            SpawnCustomer();
        }

        ResetPlayModeAndTurnOffMiniGame();
    }

    private void CompletePuzzle()
    {
        // Checking...
        for (int i = 0; i<lstCrrItem.Count; i++)
        { 
            if (!lstCrrItem[i].GetComponent<Item>().isPlaced)
            {
                return;
            }
        }

        int totalPrice = 0;
        for (int i = 0; i < lstCrrItem.Count; i++)
        {
            totalPrice += lstCrrItem[i].GetComponent<Item>().score;
        }

        if (customerRequire == "puzzle")
        {
            textDollars.text = (int.Parse(textDollars.text) + totalPrice).ToString();

            AudioManager.Instance.PlayCompleteSound();
        }
        else
        {
            textHeart.text = Mathf.Max(int.Parse(textHeart.text) - 10, 0).ToString();
        }

        if (puzzle3.activeSelf)
        {
            Border borderScript = puzzle3.GetComponent<Border>();
            borderScript.ResetBorder();
        }
        else if (puzzle4.activeSelf)
        {
            Border borderScript = puzzle4.GetComponent<Border>();
            borderScript.ResetBorder();
        }
        else if (puzzle5.activeSelf)
        {
            Border borderScript = puzzle5.GetComponent<Border>();
            borderScript.ResetBorder();
        }

        SpawnCustomer();
        ResetPlayModeAndTurnOffMiniGame();
    }

    private void PauseGame()
    {
        if (playMode != PlayMode.pause)
        {
            prePlayMode = playMode;
            playMode = PlayMode.pause;
            Time.timeScale = 0;
        }
        else if (playMode == PlayMode.pause)
        {
            Time.timeScale = 1;
            playMode = prePlayMode;
        }

        Debug.Log("PlayMode: " + playMode);
    }

    private void StartCountdownTime(int seconds)
    {
        StartCoroutine(Countdown(seconds));
    }

    private IEnumerator Countdown(int seconds)
    {
        int timeRemaining = seconds;

        while (timeRemaining > 0)
        {
            int minutes = timeRemaining / 60;
            int secs = timeRemaining % 60;
            textCountdown.text = $"{minutes:0}:{secs:00}";

            yield return new WaitForSeconds(1);
            timeRemaining--;
        }

        textCountdown.text = "0:00";
        Debug.Log("Time up!");

        // Setup timeup component
        timeOverObject.SetActive(true);
        textTotalDollars.gameObject.SetActive(true);
        textTotalDollars.text = "Tổng tiền: " + textDollars.text;
        textTotalHeart.text = "Độ uy tín: " + textHeart.text;
        textTitle.text = "Tổng kết ca";
        textButton.text = "Tiếp tục";
        AudioManager.Instance.PlayCompleteSound();
        //Dung game
        Time.timeScale = 0;
    }

    private void ChooseCookingGame()
    {
        ResetPlayModeAndTurnOffMiniGame();

        if (playMode == PlayMode.playing)
        {
            playMode = PlayMode.cooking;

            cookingObject.SetActive(true);
            btnStartCooking.gameObject.SetActive(true);
        }
    }

    private void StartCooking()
    {
        btnStartCooking.gameObject.SetActive(false);
        StartCoroutine(SpawnItemsSequentially());
    }

    private IEnumerator SpawnItemsSequentially()
    {
        for (int i = 0; i < 10; i++)
        { 
            if (panController.countItem == 10)
            {
                Debug.Log("if");
                yield break;
            }
           else if (i == 10)
            {
                if (panController.countItem <= 10)
                {
                    FinishCooking();
                }
            }

            SpawnRandomItem();
            yield return new WaitForSeconds(1.2f);
        }

        Debug.Log("het vong lap");
        yield return new WaitForSeconds(0.5f);
        FinishCooking();//Xong vòng lặp mà không gom đủ item
    }

    private void SpawnRandomItem()
    {
        int randomIndex = Random.Range(0, lstItemCooking.Count);
        GameObject item = lstItemCooking[randomIndex];

        GameObject spawnedItem = Instantiate(item, foodDrop);

        float randomX = Random.Range(-273f, 273f);
        spawnedItem.transform.localPosition = new Vector3(randomX, 0, 0);
    }

    public void FinishCooking()
    {
        if (customerRequire == "hamburger" && panController.countItem == 10)
        {
            AudioManager.Instance.PlayCompleteSound();
            textDollars.text = (int.Parse(textDollars.text) + 80).ToString();
            SpawnCustomer();
        }
        else if(customerRequire == "hamburger" && panController.countItem < 10)
        {
            textHeart.text = Mathf.Max(int.Parse(textHeart.text) - 10, 0).ToString();
            SpawnCustomer();
        }
        else
        {
            textHeart.text = Mathf.Max(int.Parse(textHeart.text) - 10, 0).ToString();
            SpawnCustomer();
        }

        panController.countItem = 0;
        panController.numberOfFood.text = $"Nguyên liệu: {panController.countItem}/10";

        ResetPlayModeAndTurnOffMiniGame();
    }

    private void ResetPlayModeAndTurnOffMiniGame()
    {
        if (playMode != PlayMode.none)
        {
            playMode = PlayMode.playing;
            idCardObject.SetActive(false);
            cookingObject.SetActive(false);

            // Puzzle
            choosingSize.SetActive(false);
            puzzle3.SetActive(false);
            puzzle4.SetActive(false);
            puzzle5.SetActive(false);
            btnCom.gameObject.SetActive(false);
        }

        LoseGame();
    }
}

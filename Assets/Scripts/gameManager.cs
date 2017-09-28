using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class gameManager : MonoBehaviour {


    public GameObject _new_game_panel;
    public GameObject _game_panel;
    public GameObject _end_panel;
    public GameObject _highscore_panel;
    public GameObject _checkMarks_button;
    public Text difficulty_description;
    private int currentDifficultySelected = 0;
    private string difficultySelectedStr;
    private int board_size_x=0;
    private int board_size_y=0;
    private int mines=10;
    public RectTransform _board;
    
    private field_script[,] game_board;

    public GameObject _field;
    public GameObject _mine;

    public Transform _canvas;
    private int field_size=0;
    public List<int> numbers;


    public Text _time_txt,_mines_txt;
    private float time;
    public int _mines_checked = 0;
    public int _fields_uncovered = 0;
    static public gameManager instance;
    RectTransform createdField;
    void Awake()
    {
        instance = this;
        checkIsHighscoreExists();
        _changeDifficulty(0);


    }

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _backToMenu();
        }
        time += Time.fixedDeltaTime;
        _time_txt.text = time.ToString("F0");
	}

    void createNewBoard(int size_x, int size_y)
    {
        field_size = 30;
        int position_x = field_size/2;
        int position_y = position_x * -1;
        for(int y=0;y<size_y;y++)
        {
            position_x = field_size / 2; 
            position_y = field_size / -2 -field_size * y;
            for (int x = 0; x < size_x; x++)
            {
               position_x = field_size / 2 + field_size * x;
               createdField = Instantiate(_field,_canvas).GetComponent<RectTransform>();
               createdField.sizeDelta = new Vector2(field_size, field_size);
               createdField.anchoredPosition = new Vector2(position_x, position_y);
               game_board[x,y] = createdField.GetComponent<field_script>();
               game_board[x, y]._pos = new Vector2(x, y);
            }
        }
    }

    void createMins(int howMany)
    {
        numbers = new List<int>(board_size_x * board_size_y);
        for (int i = 0; i < board_size_x * board_size_y; i++)
            numbers.Add(i);

        numbers = numbers.OrderBy(x => Random.value).ToList();
        for (int i = 0; i < mines;i++ )
            setMineAtField(numbers[i]);
    }

    void setMineAtField(int field)
    {
        int position_x = field % board_size_x;
        int position_y = field / board_size_x;
        game_board[position_x, position_y]._haveMine = true;
        for (int y = -1; y <=1; y++)
        {
            for (int x = -1; x <= 1; x++)
            {
                if (position_x + x >= 0 && position_x + x < board_size_x && position_y + y >= 0 && position_y + y < board_size_y) 
                {
                    game_board[position_x + x, position_y + y]._minesAround++;
                    game_board[position_x + x, position_y + y]._setDescription();
                }
            }
        }
    }

    void setMinesDescriptions()
    {
        for (int y = 0; y < board_size_y; y++)
        {
            for (int x = 0; x < board_size_x; x++)
            {
                game_board[x, y]._setDescription();
            }
        }
    }

    public void _updateMinesChecked()
    {
        _mines_txt.text = _mines_checked.ToString() + "/" + mines.ToString();
        if (_mines_checked == mines)
        {
            _checkMarks_button.SetActive(true);
        }
        else
        {
            _checkMarks_button.SetActive(false);
        }
        if (_fields_uncovered >= board_size_x * board_size_y - mines)
        {
            _endGame(1);
        }
    }

    public void _checkEmptysAround(Vector2 pos)
    {
        for(int x=-1;x<=1;x+=2)
        {
            if (pos.x + x >= 0 && pos.x + x < board_size_x)
            {
                game_board[(int)(pos.x + x),(int)pos.y]._click();
            }
        }
        for (int y = -1; y <= 1; y += 2)
        {
            if (pos.y + y >= 0 && pos.y + y < board_size_y)
            {
                game_board[(int)(pos.x), (int)(pos.y+y)]._click();
            }
        }
        
    }

    public void _startGame()
    {
        _highscore_panel.SetActive(false);
        _mines_checked = 0;
        _fields_uncovered = 0;
        time = 0;
        if (_canvas.childCount > 0)
        {
            foreach (Transform child in _canvas)
                Destroy(child.gameObject);
        }
        _board.sizeDelta = new Vector2(30 * board_size_x, 30 * board_size_y);
        _new_game_panel.SetActive(false);
        _game_panel.SetActive(true);
        enabled = true;
        game_board = new field_script[board_size_x, board_size_y];
        createNewBoard(board_size_x,board_size_y);
        createMins(mines);
        _updateMinesChecked();
        enabled = true;
    }

    public void _changeDifficulty(int direction)
    {
        currentDifficultySelected += direction;
        if (currentDifficultySelected < 0) currentDifficultySelected = 2;
        if (currentDifficultySelected > 2) currentDifficultySelected = 0;

        switch(currentDifficultySelected)
        {
            case 0:
                  board_size_x=8;
                  board_size_y=8;
                  mines = 10;
                  difficultySelectedStr = "Easy";
                break;
            case 1:
                  board_size_x=16;
                  board_size_y=16;
                  mines = 40;
                  difficultySelectedStr = "Medium";
                break;
            case 2:
                  board_size_x=30;
                  board_size_y=16;
                  mines = 99;
                  difficultySelectedStr = "Hard";
                break;
        }
        _setHighScoreValues(difficultySelectedStr);
        difficulty_description.text = difficultySelectedStr + "\nBoard " + board_size_x + "x" + board_size_y + "\n" + mines + " mines";
    }


    public void _endGame(int ID)
    {
        this.enabled = false;
        _highscore_panel.SetActive(true);

        _highscore_panel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -40);
        switch (ID)
        {
            case 0:
                _end_panel.transform.Find("title").GetComponent<Text>().text = "You lose";
                break;
            case 1:
                _end_panel.transform.Find("title").GetComponent<Text>().text = "You win in " + time.ToString("f0")+" sec";
                if (_checkIfNewHighscore())
                {
                    _showInputForHighscore();
                    
                }
                    

                break;
        }
        _end_panel.gameObject.SetActive(true);
        _game_panel.SetActive(false);
    }

    public void _backToMenu()
    {
        _highscore_panel.SetActive(true);
        _highscore_panel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -220);
        _new_game_panel.SetActive(true);
        _end_panel.SetActive(false);
        _game_panel.SetActive(false);
    }

    public void _restart()
    {
        _end_panel.gameObject.SetActive(false);
        _startGame();
    }

    public void _exit()
    {
        Application.Quit();
    }

    private void checkIsHighscoreExists()
    {
        string[] diff_array = {"Easy","Medium","Hard"};
    for(int diff=0;diff<3;diff++)
        for (int place = 1; place <= 3; place++)
        {
            if(!PlayerPrefs.HasKey(diff_array[diff]+place.ToString()))
                PlayerPrefs.SetString(diff_array[diff]+place.ToString(),"AAA"+(place*100).ToString());
        }
    }

    public string _getHighScore(int place, string difficulty)
    {
        string txt = place.ToString() +". " + PlayerPrefs.GetString(difficulty + place);
        txt = txt.Insert(6, " ");
        txt += " sec";
        return txt;
    }

    public void _setHighScoreValues(string difficulty)
    {
        for (int i = 0; i < 3; i++)
            _highscore_panel.transform.GetChild(i).GetComponent<Text>().text = _getHighScore(i + 1, difficulty);
    }

    private bool _checkIfNewHighscore()
    {
       
        int value = int.Parse(PlayerPrefs.GetString(difficultySelectedStr + 3.ToString()).Substring(3));
        if (time < value)
            return true;
        else
            return false;
    }

    int place = 0;
    private void _showInputForHighscore()
    {
        _end_panel.transform.Find("buttons").gameObject.SetActive(false);
        _highscore_panel.transform.Find("InputField").gameObject.SetActive(true);

        int[] values = new int[3];
        for(int i=0;i<3;i++)
            values[i] = int.Parse(PlayerPrefs.GetString(difficultySelectedStr + (i+1).ToString()).Substring(3));

        if (time < values[0])
            place = 1;
        else if (time < values[1])
            place = 2;
        else
            place = 3;
        switch (place)
        {
            case 1:
                PlayerPrefs.SetString(difficultySelectedStr + 3.ToString(), PlayerPrefs.GetString(difficultySelectedStr + 2.ToString()));
                PlayerPrefs.SetString(difficultySelectedStr + 2.ToString(), PlayerPrefs.GetString(difficultySelectedStr + 1.ToString()));
                break;
            case 2:
                PlayerPrefs.SetString(difficultySelectedStr + 3.ToString(), PlayerPrefs.GetString(difficultySelectedStr + 2.ToString()));
                break;
        }
        _setHighScoreValues(difficultySelectedStr);
        _highscore_panel.transform.Find("InputField").GetComponent<RectTransform>().anchoredPosition = new Vector2(19, 107 - place * 40);
    }

    public void _confirmHighscore()
    {
        string name =  _highscore_panel.transform.Find("InputField").GetComponent<InputField>().text;
        if(name.Length<3)
            name +="   ";
        PlayerPrefs.SetString(difficultySelectedStr + place.ToString(), name.Substring(0, 3) + time.ToString("f0"));
        _setHighScoreValues(difficultySelectedStr);
        _highscore_panel.transform.Find("InputField").gameObject.SetActive(false);
        _end_panel.transform.Find("buttons").gameObject.SetActive(true);
    }


    public void _resetData()
    {
        PlayerPrefs.DeleteAll();
        checkIsHighscoreExists();
        _setHighScoreValues(difficultySelectedStr);
    }

    public void _checkMarks()
    {
        for (int y = 0; y < board_size_y; y++)
        {
            for (int x = 0; x < board_size_x; x++)
            {
                if (game_board[x, y]._haveMine && game_board[x, y]._state != 2)
                {
                    _endGame(0);
                    return;
                }
            }
        }
        _endGame(1);
    }
}

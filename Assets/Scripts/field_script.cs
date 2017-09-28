using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class field_script : MonoBehaviour {

    public int _state;
    public bool _haveMine;
    public int _minesAround;
    private bool wasCheckedAsMine;
    public Vector2 _pos;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (_haveMine)
            gameObject.GetComponent<Image>().enabled=!Input.GetKey(KeyCode.Space);
	}

    public void _click()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (wasCheckedAsMine)
            {
                gameManager.instance._mines_checked--;
                wasCheckedAsMine = false;
            }
            _state++;
            if (_state > 2) 
                _state = 0;
            switch(_state)
            {
                case 0:
                    transform.GetChild(0).GetComponent<Text>().text = "";
                break;
                case 1:
                    transform.GetChild(0).GetComponent<Text>().text = "?";
                break;
                case 2:
                    transform.GetChild(0).GetComponent<Text>().text = "X";
                    gameManager.instance._mines_checked++;
                    wasCheckedAsMine = true;
                break;
            }
            
        }else if(_state==0){
            if (_haveMine)
            {
                gameManager.instance._endGame(0);
            }
            else if (gameObject.GetComponent<UnityEngine.EventSystems.EventTrigger>().enabled)
            {
                gameObject.GetComponent<Image>().enabled = false;
                gameObject.GetComponent<Button>().enabled = false;
                gameObject.GetComponent<UnityEngine.EventSystems.EventTrigger>().enabled = false;
                transform.GetChild(1).gameObject.SetActive(true);
                gameManager.instance._fields_uncovered++;
                if (_minesAround == 0)
                    gameManager.instance._checkEmptysAround(_pos);
            }
        }
        gameManager.instance._updateMinesChecked();
    }

    public void _setDescription()
    {
        if (_minesAround > 0 && !_haveMine)
        {
            transform.GetChild(1).GetComponent<Text>().text = _minesAround.ToString();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.UI;

public class GameController : MonoBehaviour
    {
        [SerializeField] private GameObject ball;
        [SerializeField] private Transform maze, floor;

        public Text _level,_limitedCounter;
        public Button _nextGame;
        public GameObject _pausePage, _gameModePage, _gameOverPage;
        private BallController ballController;
        private float time;
        private MapModel map;
        private int level = 0;
        private int uiLevel = 0;
        private int levelPassCounter = 0;
        private int limitedValue;
        private bool limitedGame;

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }
        private void Start()
        {
            map = GetComponent<MapModel>();
            ballController = ball.GetComponent<BallController>();
            createMap(map.maze,level);
            uiLevel = level + 1;
            _level.text = "Level " + uiLevel;
        }

        private void createMap(int[,,] mapArray, int level)
        {
            if (level > 2)
                level = 0;
            levelPassCounter = 0;
          
            ball.transform.localPosition = 
                    new Vector3(map.ballPosition[level, 0], map.ballPosition[level, 1], map.ballPosition[level, 2]);
            int childCount = 0;
            for (int i = 0; i < 13; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    if (mapArray[level, i, j] == 0)
                    {
                        maze.GetChild(childCount).gameObject.SetActive(false);
                        floor.GetChild(childCount).gameObject.SetActive(true);
                        levelPassCounter++;
                    }
                    else
                    {
                        maze.GetChild(childCount).gameObject.SetActive(true);
                        floor.GetChild(childCount).gameObject.SetActive(false);
                    }
                    floor.GetChild(childCount).gameObject.GetComponent<Renderer>().material.color = Color.black;
                    childCount++;
                }
            }
            ballController.ColoredWallCounter = 0;
    }

        #region Ui Control Functions
        public void gameModeOpenPage()
        {
            _gameModePage.SetActive(true);
        }

        public void gameModeClosePage()
        {
            _gameModePage.SetActive(false);
        }

        public void gameOverOpenPage()
        {
            _gameOverPage.SetActive(true);
            ballController.MoveCounter = 0;
        }

        public void tryAgain()
        {
            _gameOverPage.SetActive(false);
           
            createMap(map.maze, level);
        } 

        public void gamaModeSelect(int mode)
        {
            if (mode == 1)
            {
                // ToDo: PlayerPrefs save game
                level = 2;
                limitedValue = map.limitedValue[level];
                limitedGame = true;
                ballController.MoveCounter = 0;
                uiLevel = level + 1;
                _level.text = "Level" + 1;
                _limitedCounter.gameObject.SetActive(true);
            }
            else
            {
                // ToDo saved game
                level = 0;
                limitedGame = false;
                _limitedCounter.gameObject.SetActive(false);
            }
            levelPassCounter = 0;
            createMap(map.maze, level);
            _gameModePage.SetActive(false);
        }

        public void openPausePage()
        {
            _pausePage.SetActive(true);
        }

        public void closePausePage()
        {
            _pausePage.SetActive(false);
        }

        private void levelPass()
        {
            if (level > 1)
            {
                level = -1;
                limitedGame = false;
                _limitedCounter.gameObject.SetActive(false);
            }
            level++;
            _nextGame.gameObject.SetActive(false);
            uiLevel = level + 1;
            _level.text = "Level " + uiLevel;
            createMap(map.maze, level);
        }
    #endregion

        private void levelFinished()
        {
            // ToDo Add Particular Effect
            // Zemin hareket ettirilecek
            _level.text = level+1 + ". SEVİYE TAMAMLANDI";
            _nextGame.gameObject.SetActive(true);
        }

        private void Update()
        {
            _limitedCounter.text = (limitedValue-ballController.MoveCounter).ToString();
            if (ballController.ColoredWallCounter == levelPassCounter)
            {
                levelFinished();
            }
            if(ballController.MoveCounter == limitedValue && limitedGame)
                gameOverOpenPage();
        }
    }



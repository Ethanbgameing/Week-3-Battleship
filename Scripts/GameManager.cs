using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Battleship
{


    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private int[,] grid = new int[,]
        {
            //TOP LEFT IS 0,0
            {1,1,0,0,1 },
            {0,0,0,0,0 },
            {0,0,1,0,1 },
            {1,0,1,0,0 },
            {1,0,1,0,1 },
            //BOTTOM RIGHT IS 4,4
        };
        // VARIABLES
        private bool[,] hits;

        private int nRows;
        private int nCols;

        private int row;
        private int col;

        private int score;
        private int targetScore;

        private int time;

        //REFERENCES TO GAME OBJECTS
        [SerializeField] Transform gridRoot;
        [SerializeField] GameObject CellPrefab;
        [SerializeField] GameObject winLabel;
        [SerializeField] TextMeshProUGUI timeLabel;
        [SerializeField] TextMeshProUGUI scoreLabel;

        private void Awake()
        {
            nRows = grid.GetLength(0);
            nCols = grid.GetLength(1);

            hits = new bool[nRows,nCols];

            for (int i = 0; i < nRows * nCols; i++)
            {
                Instantiate(CellPrefab, gridRoot);
            }
            BeginGame();
            
        }

        void BeginGame()
        {
            targetScore = 0;
            //LOOKS THROUGH GRID TO FIND ALL SHIPS, THEN SETS THE TARGET SCORE TO GET
            for (int i = 0; i < nRows; i++)
            {
                for (int j = 0; j < nCols; j++)
                {
                    if (grid[i, j] == 1)
                    {
                        targetScore++;
                    }
                }
            }
            Debug.Log(targetScore);
            score = 0;
            time = 0;


            selectCell();
            InvokeRepeating("incrementTime", 1f, 1f);
        }

        Transform GetCurrentCell() 
        {
        int index = (row * nCols) + col;

        return gridRoot.GetChild(index);
        }

        void selectCell()
        {
            Transform cell = GetCurrentCell();

            Transform cursor = cell.Find("Cursor");
            cursor.gameObject.SetActive(true);
        }

        void unselectCell()
        {
            Transform cell = GetCurrentCell();

            Transform cursor = cell.Find("Cursor");
            cursor.gameObject.SetActive(false);
        }

        public void moveHorizontal(int amount)
        {
            unselectCell();

            col += amount;

            col = Mathf.Clamp(col, 0, nCols - 1);

            selectCell();
        }

        public void moveVertical(int amount)
        {
            unselectCell();

            row += amount;

            row = Mathf.Clamp(row, 0, nRows - 1);

            selectCell();
        }

        void showHit()
        {
            Transform target = GetCurrentCell();

            Transform hit = target.Find("Hit");
            hit.gameObject.SetActive(true);
            
        }

        void showMiss()
        {
            Transform target = GetCurrentCell();

            Transform hit = target.Find("Miss");
            hit.gameObject.SetActive(true);
        }

        void resetCell()
        {
            Transform target = GetCurrentCell();

            Transform miss = target.Find("Miss");
            miss.gameObject.SetActive(false);
 
            Transform hit = target.Find("Hit");
            hit.gameObject.SetActive(false);
        }

        void incrementScore()
        {
            score++;

            scoreLabel.text = string.Format($"Score {score}");
        }

        public void Fire()
        {
            //Check if we fired already
            if (hits[row, col]) return;
            //We Fired on this Space
            hits[row, col] = true;
            //Ship Check
            if (grid[row,col] == 1)
            {
                //Hit a Ship
                showHit();
                incrementScore();
            }
            else
            {
                //Hit Water
                showMiss();
            }
        }

        void checkEndGame()
        {
            //for (int i = 0; i < nRows; i++) 
            //{
            //    for(int j = 0; j < nCols; j++) 
            //    {
            //        if (grid[i,j] == 0) continue;

            //        if (hits[row,col] == false) return;
            //    }
            //}

            //IF SCORE IS EQUAL TO TARGET, END GAME
            if(score == targetScore)
            {
                winLabel.SetActive(true);
                CancelInvoke("incrementTime");
            }
            
        }

        void incrementTime()
        {
            time++;
            timeLabel.text = string.Format($"{time / 60}:{(time % 60).ToString("00")}");
            checkEndGame();
        }

        public  void ResetGame()
        {
            //RESET CLOCK
            CancelInvoke("incrementTime");
            time = 0;

            //SETUP RANDOM
            System.Random random = new System.Random();
            int shipChance;

            //TURN OFF WIN LABEL
            winLabel.SetActive(false);

            //UNSELECT CURRENT CELL
            unselectCell();

            //FOR EACH ROW AND COLUMN,
            for (int i = 0; i < nRows; i++)
            {
                for (int j = 0; j < nCols; j++)
                {
                    //SELECT CELL TO RESET
                    row = i; col = j;
                    selectCell();
                    resetCell();
                    unselectCell();

                    //RESET HIT STATUS
                    hits[i,j] = false;

                    //GENERATE A RANDOM NUMBER
                    shipChance = random.Next(2);
                    if ( shipChance > 0 )
                    {
                        //IF CHANCE = 1, THEN SELECTED GRID IS A SHIP GRID
                        grid[i, j] = 1;
                    }
                    else
                    {
                        //OTHERWISE IT'S WATER
                        grid[i, j] = 0;
                    }
                }
            }
            //AFTER ALL VARIABLES HAVE BEEN SET, RESTART VALUES AND BEGIN NEW GAME
            BeginGame();
        }
    }


}


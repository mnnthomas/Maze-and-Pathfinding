using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MazeGenerator
{
    public class Movements
    {
        public MazeCellData _Cell;
        public int _Direction;

        public Movements(MazeCellData cell, int dir)
        {
            _Cell = cell;
            _Direction = dir;
        }
    }

    public class MazeGenerator : MonoBehaviour
    {
        [SerializeField] private int m_MazeWidth;
        [SerializeField] private int m_MazeLength;
        [SerializeField] private GameObject m_MazeCell;

        
        public MazeCellData[,] _MazeCells;

        private void Start()
        {
            _MazeCells = new MazeCellData[m_MazeWidth, m_MazeLength];
            CreateMazeCells();
        }

        private void CreateMazeCells()
        {
            for(int i = 0; i < _MazeCells.GetLength(0); i++)
            {
                for(int j = 0; j < _MazeCells.GetLength(1); j++)
                {
                    GameObject mazeCell = Instantiate(m_MazeCell);
                    mazeCell.transform.position = new Vector3(i, 0, j);
                    mazeCell.name = "Cell " + i + "_" + j;
                    mazeCell.transform.parent = transform;

                    MazeCellData mazeCellData = mazeCell.GetComponent<MazeCellData>();

                    mazeCellData._IsVisited = false;
                    mazeCellData._X = i;
                    mazeCellData._Y = j;

                    _MazeCells[i, j] = mazeCellData;
                }
            }
            Camera.main.transform.position = new Vector3(m_MazeWidth / 2, m_MazeWidth + m_MazeLength, m_MazeLength / 2);
            GenerateMaze();
        }

        private void GenerateMaze()
        {
            RemoveWall(_MazeCells[0, 0], 0);
            RemoveWall(_MazeCells[_MazeCells.GetLength(0) - 1, _MazeCells.GetLength(1) - 1], 3);


            for (int i = 0; i < _MazeCells.GetLength(0); i++)
            {
                for(int j = 0; j < _MazeCells.GetLength(1); j++)
                {
                    if(!_MazeCells[i, j]._IsVisited)
                    {
                        if ((i != 0 && j != 0) || (i != _MazeCells.GetLength(0) - 1 && j != _MazeCells.GetLength(1) - 1))
                        {
                            //Removing a path to already visited cell when starting a new search
                            List<Movements> visitedCells = FindNearestVisitedCells(i, j);
                            if (visitedCells.Count > 0)
                                RemoveWallFromTo(_MazeCells[i, j], visitedCells[0]._Cell, visitedCells[0]._Direction);
                        }

                        FindMazePathFrom(i, j);
                        
                    }
                }
            }
        }

        private void FindMazePathFrom(int x, int y)
        {
            _MazeCells[x , y]._IsVisited = true;
            _MazeCells[x, y]._Floor.GetComponent<Renderer>().material.color = Color.red;

            List<Movements> unvisitedCells = FindNearestUnvisitedCells(x, y);
           
            int randValue = default;
            if (unvisitedCells.Count > 0)
                randValue = Random.Range(0, unvisitedCells.Count);
            else
                return;

            MazeCellData foundCell = unvisitedCells[randValue]._Cell;
            int foundDir = unvisitedCells[randValue]._Direction;

            RemoveWallFromTo(_MazeCells[x, y], foundCell, foundDir);
            foundCell._IsVisited = true;
            foundCell._Floor.GetComponent<Renderer>().material.color = Color.red;

            //Recursion till we have no more possible movements
            FindMazePathFrom(foundCell._X, foundCell._Y);
        }

        public List<Movements> FindNearestUnvisitedCells(int x, int y)
        {
            List<Movements> cells = new List<Movements>();

            if (y - 1 > 0 && !_MazeCells[x, y - 1]._IsVisited) // bottom
                cells.Add(new Movements(_MazeCells[x, y - 1], 0));
            if (x - 1 > 0 && !_MazeCells[x - 1, y]._IsVisited) // left
                cells.Add(new Movements(_MazeCells[x - 1, y], 1));
            if (x + 1 < m_MazeWidth && !_MazeCells[x + 1, y]._IsVisited) // right
                cells.Add(new Movements(_MazeCells[x + 1, y], 2));
            if (y + 1 < m_MazeLength && !_MazeCells[x, y + 1]._IsVisited) // top
                cells.Add(new Movements(_MazeCells[x, y + 1], 3));

            return cells;
        }

        public List<Movements> FindNearestVisitedCells(int x, int y)
        {
            List<Movements> cells = new List<Movements>();

            if (y - 1 > 0 && _MazeCells[x, y - 1]._IsVisited) // bottom
                cells.Add(new Movements(_MazeCells[x, y - 1], 0));
            if (x - 1 > 0 && _MazeCells[x - 1, y]._IsVisited) // left
                cells.Add(new Movements(_MazeCells[x - 1, y], 1));
            if (x + 1 < m_MazeWidth && _MazeCells[x + 1, y]._IsVisited) // right
                cells.Add(new Movements(_MazeCells[x + 1, y], 2));
            if (y + 1 < m_MazeLength && _MazeCells[x, y + 1]._IsVisited) // top
                cells.Add(new Movements(_MazeCells[x, y + 1], 3));

            return cells;
        }

        public List<Movements> FindNearestMovableCells(int x, int y)
        {
            List<Movements> cells = new List<Movements>();

            if (y - 1 > 0 && !_MazeCells[x, y]._BackWall.activeInHierarchy) // bottom
                cells.Add(new Movements(_MazeCells[x, y - 1], 0));
            if (x - 1 > 0 && !_MazeCells[x, y]._LeftWall.activeInHierarchy) // left
                cells.Add(new Movements(_MazeCells[x - 1, y], 1));
            if (x + 1 < m_MazeWidth && !_MazeCells[x, y]._RightWall.activeInHierarchy) // right
                cells.Add(new Movements(_MazeCells[x + 1, y], 2));
            if (y + 1 < m_MazeLength && !_MazeCells[x, y]._FrontWall.activeInHierarchy) // top
                cells.Add(new Movements(_MazeCells[x, y + 1], 3));

            return cells;
        }

        private void RemoveWallFromTo(MazeCellData fromCell, MazeCellData toCell, int dir)
        {
            if (dir == 0)
            {
                fromCell._BackWall.SetActive(false);
                toCell._FrontWall.SetActive(false);
            }
            else if (dir == 1)
            {
                fromCell._LeftWall.SetActive(false);
                toCell._RightWall.SetActive(false);
            }
            else if (dir == 2)
            {
                fromCell._RightWall.SetActive(false);
                toCell._LeftWall.SetActive(false);
            }
            else if (dir == 3)
            {
                fromCell._FrontWall.SetActive(false);
                toCell._BackWall.SetActive(false);
            }
        }

        private void RemoveWall(MazeCellData cell, int dir)
        {
            if (dir == 0)
                cell._BackWall.SetActive(false);
            else if (dir == 1)
                cell._LeftWall.SetActive(false);
            else if (dir == 2)
                cell._RightWall.SetActive(false);
            else if (dir == 3)
                cell._FrontWall.SetActive(false);

        }
    }

   
}

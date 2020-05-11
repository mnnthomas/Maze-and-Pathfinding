using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MazeGenerator
{
    public class MazeCellData : MonoBehaviour
    {
        public GameObject _RightWall;
        public GameObject _LeftWall;
        public GameObject _FrontWall;
        public GameObject _BackWall;
        public GameObject _Floor;

        public int _X, _Y;
        public bool _IsVisited;
    }
}


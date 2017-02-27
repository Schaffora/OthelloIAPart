using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OthelloIA3
{
    /* State enum for a tile */
    public enum state
        {
            black,
            white,
            empty,
            isAbleToPlay
        }
    /* Tile class */
    public class Tile
    {


        public state state;
        public Tile()
        {
            /* Initial state is empty */
            state = state.empty;
        }
    }

    public class Board : IPlayable.IPlayable
    {
        private const int BOARDSIZE = 8;
        public int boardsize;
        public Tile[,] tiles;
        private int[,] myBoard;
        private List<Tuple<int, int>> flips;
        private List<Tuple<int, int>> potentialFlips;
        private List<Tuple<int, int>> ableCases;
        int[,] direction = { { 1, 0 }, { 0, 1 }, { 0, -1 }, { -1, 0 }, { 1, 1 }, { 1, -1 }, { -1, -1 }, { -1, 1 } };
   

        public Board()
        {
            /* List used to flips the right tiles*/
            flips = new List<Tuple<int, int>>();
            potentialFlips = new List<Tuple<int, int>>();
            ableCases = new List<Tuple<int, int>>();

            /* Board size */
            boardsize = BOARDSIZE;

            /* List of the 64 tiles */
            tiles = new Tile[boardsize, boardsize];
            myBoard = new int[BOARDSIZE, BOARDSIZE];
            /*Tiles initialisation*/
            for (int i = 0; i < boardsize; i++)
            {
                for (int j = 0; j < boardsize; j++)
                {
                    tiles[i, j] = new Tile();
                }
            }

            /* Board initialisation */
            tiles[4, 4].state = state.white;
            tiles[3, 3].state = state.white;
            tiles[4, 3].state = state.black;
            tiles[3, 4].state = state.black;

            /* Search playable */
            //updatePlayables(isWhiteTurn);
        }

        public string GetName()
        {
            return "03_Gygi_Schaffo";
        }

        public Tuple<int, int> GetNextMove(int[,] game, int level, bool whiteTurn)
        {
            updatePlayables(whiteTurn);
            char[] colonnes = "ABCDEFGH".ToCharArray();
            if (ableCases.Count == 0)
                return new Tuple<int, int>(-1, -1);
            return ableCases.First();
        }

        /* Our perfekt algorithme to find the playables */
        public bool playable(int c, int l, state s)
        {  // updatePlayables
            bool found = false;
            state otherState;
            if (s == state.white)
                otherState = state.black;
            else
                otherState = state.white;

            for (int i = 0; i < 8; i++)
            {
                if (found == true)
                {
                    break;
                }
                int sl = l + direction[i, 0];
                int sc = c + direction[i, 1];

                bool got = false;
                while (sl >= 0 && sl <= boardsize - 1 && sc >= 0 && sc <= boardsize - 1)
                {
                    if (tiles[sc, sl].state == state.empty || tiles[sc, sl].state == state.isAbleToPlay)
                    {
                        got = false;
                        break;
                    }
                    if (tiles[sc, sl].state == otherState)
                    {
                        got = true;
                    }
                    if (tiles[sc, sl].state == s && got == false)
                    {
                        got = false;
                        break;
                    }
                    if (tiles[sc, sl].state == s && got == true)
                    {
                        found = true;
                        break;
                    }
                    sl = sl + direction[i, 0];
                    sc = sc + direction[i, 1];
                }
            }
            return found;
        }

        /* Interface playmove function */
        public bool playMove(int column, int line, bool isWhite)
        {
            if (tiles[column, line].state == state.isAbleToPlay)
            {
                if (isWhite == true)
                    return verify(column, line, state.white, isWhite);
                else
                    return verify(column, line, state.black, isWhite);
            }
            else
                return false;
        }

        /* Function that play and verify if a player won */
        public bool verify(int column, int line, state s, bool isWhite)
        {
            tiles[column, line].state = s;
            flipPieces(column, line, s);
            if (updatePlayables(!isWhite))
                return true;
            else
                return false;
        }

        /* Clear all playables field, is called after a move */
        public void clearPlayables()
        {
            for (int i = 0; i < boardsize; i++)
            {
                for (int j = 0; j < boardsize; j++)
                {
                    if (tiles[i, j].state == state.isAbleToPlay)
                    {
                        tiles[i, j].state = state.empty;
                    }
                }
            }
        }

        /*Update the playable after a move */
        public bool updatePlayables(bool isWhiteTurn)
        {
            clearPlayables();
            ableCases.Clear();
            int numberOfPlayables = 0;
            for (int i = 0; i < boardsize; i++)
            {
                for (int j = 0; j < boardsize; j++)
                {
                    if (IsPlayable(i, j, isWhiteTurn))
                    {
                        if (tiles[i, j].state == state.empty)
                        {
                            tiles[i, j].state = state.isAbleToPlay;
                            ableCases.Add(new Tuple<int, int>(i, j));
                            numberOfPlayables++;
                        }
                    }
                }
            }
            if (numberOfPlayables == 0)
                return false;
            else
                return true;
        }

        /* Our perfekt flip algorithm */
        public void flipPieces(int c, int l, state s)
        {
            flips.Clear();

            state otherState = state.white;
            if (s == state.white)
                otherState = state.black;

            for (int i = 0; i < 8; i++)
            {
                int sl = l + direction[i, 0];
                int sc = c + direction[i, 1];
                potentialFlips.Clear();
                bool got = false;
                while (sl >= 0 && sl <= boardsize - 1 && sc >= 0 && sc <= boardsize - 1)
                {
                    if (tiles[sc, sl].state == state.empty || tiles[sc, sl].state == state.isAbleToPlay)
                    {
                        got = false;
                        break;
                    }
                    if (tiles[sc, sl].state == otherState)
                    {
                        got = true;
                        potentialFlips.Add(new Tuple<int, int>(sc, sl));
                    }
                    if (tiles[sc, sl].state == s && got == false)
                    {
                        potentialFlips.Clear();
                        got = false;
                        break;
                    }
                    if (tiles[sc, sl].state == s && got == true)
                    {
                        foreach (Tuple<int, int> item in potentialFlips)
                        {
                            int cVal = item.Item1;
                            int lVal = item.Item2;
                            flips.Add(new Tuple<int, int>(cVal, lVal));
                        }
                        potentialFlips.Clear();
                        break;
                    }
                    sl = sl + direction[i, 0];
                    sc = sc + direction[i, 1];
                }
            }
            foreach (Tuple<int, int> item in flips)
            {
                int cVal = item.Item1;
                int lVal = item.Item2;
                tiles[cVal, lVal].state = s;
            }
        }

       
        /* Function that get the score using state */
        public int getScore(state s)
        {
            int score = 0;
            for (int i = 0; i < boardsize; i++)
            {
                for (int j = 0; j < boardsize; j++)
                {
                    if (tiles[i, j].state == s)
                        score++;
                }
            }
            return score;
        }

        /* Function to reset, restart a game */
        public void resetGame()
        {
            for (int i = 0; i < boardsize; i++)
            {
                for (int j = 0; j < boardsize; j++)
                {
                    tiles[i, j].state = state.empty;
                }
            }
            tiles[4, 4].state = state.white;
            tiles[3, 3].state = state.white;
            tiles[4, 3].state = state.black;
            tiles[3, 4].state = state.black;
            updatePlayables(false);
        }

        public bool IsPlayable(int column, int line, bool isWhite)
        {
            bool tilePlayabe = false;
            if (isWhite)
                tilePlayabe = playable(column, line, state.white);
            else
                tilePlayabe = playable(column, line, state.black);
            return tilePlayabe;
        }

        public bool PlayMove(int column, int line, bool isWhite)
        {
            updatePlayables(isWhite);
            if (tiles[column, line].state == state.isAbleToPlay)
            {
                if (isWhite == true)
                    return verify(column, line, state.white, isWhite);
                else
                    return verify(column, line, state.black, isWhite);
            }
            else
                return false;
        }

        public int[,] GetBoard()
        {
            for (int i = 0; i < BOARDSIZE; i++)
            {
                for (int j = 0; j < BOARDSIZE; j++)
                {
                    if (this.tiles[i, j].state == state.black)
                        myBoard[i, j] = 1;
                    else if (this.tiles[i, j].state == state.white)
                        myBoard[i, j] = 0;
                    else
                        myBoard[i, j] = -1;
                }
            }
            return myBoard;
        }

        public int GetWhiteScore()
        {
            return getScore(state.white);
        }

        public int GetBlackScore()
        {
            return getScore(state.black);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuSolver
{
    class Program
    {
        const int PENCIL_SIZE = 27;
        const int NUM_LAYERS = 27;
        //                     "111111111222222222333333333444444444555555555666666666777777777888888888999999999"
        static string puzzle = ".....2.9.....9...4..98..6.7......57..83.2.96..12......1.6..58..4...3.....9.7.....";
        static int[,,] pencilMarks = new int[PENCIL_SIZE, PENCIL_SIZE, 2];
        static int[, ,] board = new int[9, 9, NUM_LAYERS];
        static int[,] candidateCount = new int[9, 9];
        static int currentLayer = 0;
        static int solveCount = 0;
        static int previousSolveCount = 0;
        static int repetitionCount = 0;
        static DateTime startTime, endTime;
        static TimeSpan duration;
        static void Main(string[] args)
        {
            initialize();
            setSln();
            getInput();
            startTime = DateTime.Now;
            solveForSingles();
            if (solvedState())
            {
                printBoard();
                Console.WriteLine("\nValid Solution!!");
            }
            else
            {
                printAll();
                Console.WriteLine("\n!!Epic Fail...");
                bruteForce();
            }
            if (solvedState())
            {
                printBoard();
                Console.WriteLine("\nValid Solution!!");
            }
            else
            {
                Console.WriteLine("\n!!Epic Fail...");
                printAll();
            }
            endTime = DateTime.Now;
            duration = endTime - startTime;
            Console.WriteLine("It took {0} to complete", duration);
            Console.WriteLine("Took {0} iterations(s).", repetitionCount);
            Console.ReadKey();
        }

        static void solveForSingles()
        {
            countSolved();
            previousSolveCount = 0;
            while ((solveCount != previousSolveCount) && (currentLayer < NUM_LAYERS))
            {
                previousSolveCount = solveCount;
                eraseCandidates();
                setSlnLayer(currentLayer);
                currentLayer++;
                eliminateSingles();
                eraseCandidates();
                Console.WriteLine("Before Find Singles");
                setSlnLayer(currentLayer);
                updateCandidateCount();
                findSinglesinMultiple();
                Console.WriteLine("After Find Singles");
                countSolved();
                repetitionCount++;
            }
            setSlnLayer(currentLayer);
        }
        static bool userEnter()
        {
            string[] usrntry = new string[9];
            string input;
            Console.Clear();
            for (int i = 0; i < 9; i++)
            {
                Console.Write("\nEnter line {0} of Puzzle, entering a period for a blank space\n>", i + 1);
                input = Console.ReadLine();
                if (input.Length == 9)
                    usrntry[i] = input;
                else
                {
                    Console.WriteLine("Entry was {0} characters, and it must be 9.  Please try again.", input.Length);
                    i--;
                }
            }
            puzzle = "";
            for (int i = 0; i < 9; i++)
            {
                puzzle += usrntry[i];
            }
            setSln();
            Console.WriteLine();
            printBoard();
            Console.WriteLine("Is this the puzzle you entered? Enter 1 for Yes and 2 for No.");
            if (Console.ReadLine().CompareTo("1") == 0)
                return true;
            else
                return false;
        }
        static void getInput()
        {
            int choice;
            bool accepted = false;
            do
            {
                Console.WriteLine("Would you like to input your own board or run program's puzzle?\n1. Input\n2. Run Program's\nThe programs Puzzle is:");
                printBoard();
                Console.Write("\n>");
                try
                {
                    choice = Convert.ToInt32(Console.ReadLine());
                    switch (choice)
                    {
                        case 1:
                            while (!userEnter()) ;
                            accepted = true;
                            break;
                        case 2:
                            accepted = true;
                            break;
                        default:
                            Console.WriteLine("Not a valid input.");
                            break;
                    }
                }
                catch (FormatException e)
                {
                    Console.WriteLine(e.Message);
                }
            } while (accepted != true);
        }
        static void initialize()
        {
            for (int row = 0; row < PENCIL_SIZE; row++)
            {
                for (int col = 0; col < PENCIL_SIZE; col++)
                {
                    pencilMarks[row, col, 0] = (col % 3) + (3 * (row % 3)) + 1;
                }
            }
            for (int layers = 0; layers < NUM_LAYERS; layers++)
            {
                for (int row = 0; row < 9; row++)
                {
                    for (int col = 0; col < 9; col++)
                    {
                        board[row, col, layers] = col + 1;
                    }
                }
            }
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    candidateCount[row, col] = 0;
                }
            }
        }
        static void setSln()
        {
            char temp;
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    temp = puzzle[((row * 9) + col)];
                    switch (temp)
                    {
                        case '1':
                            board[row, col, 0] = 1;
                            break;
                        case '2':
                            board[row, col, 0] = 2;
                            break;
                        case '3':
                            board[row, col, 0] = 3;
                            break;
                        case '4':
                            board[row, col, 0] = 4;
                            break;
                        case '5':
                            board[row, col, 0] = 5;
                            break;
                        case '6':
                            board[row, col, 0] = 6;
                            break;
                        case '7':
                            board[row, col, 0] = 7;
                            break;
                        case '8':
                            board[row, col, 0] = 8;
                            break;
                        case '9':
                            board[row, col, 0] = 9;
                            break;
                        default:
                            board[row, col, 0] = 0;
                            break;
                    }
                }
            }
        }
        static void printAll()
        {
            printBoard();
            printCandiates();
        }
        static void printBoard()
        {
            /**************************/
            /*      PRINT BOARD       */
            /**************************/
            Console.WriteLine("\n-------------");
            for (int row = 1; row <= 9; row++)
            {
                Console.Write("|");
                for (int col = 1; col <= 9; col++)
                {
                    switch (board[row - 1, col - 1, currentLayer])
                    {
                        case 1:
                            Console.Write("1");
                            break;
                        case 2:
                            Console.Write("2");
                            break;
                        case 3:
                            Console.Write("3");
                            break;
                        case 4:
                            Console.Write("4");
                            break;
                        case 5:
                            Console.Write("5");
                            break;
                        case 6:
                            Console.Write("6");
                            break;
                        case 7:
                            Console.Write("7");
                            break;
                        case 8:
                            Console.Write("8");
                            break;
                        case 9:
                            Console.Write("9");
                            break;
                        case 0:
                            Console.Write(" ");
                            break;
                    }
                    if ((col % 3) == 0)
                    {
                        Console.Write("|");
                    }
                }
                Console.WriteLine();
                if ((row % 3) == 0)
                {
                    Console.WriteLine("-------------");
                }
            }
        }
        static void printCandiates()
        {
            /**************************/
            /*   Print Pencil Marks   */
            /**************************/
            Console.WriteLine("\n---------------------------------------");
            for (int row = 0; row < PENCIL_SIZE; row++)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("|");
                Console.ForegroundColor = ConsoleColor.Gray;
                for (int col = 0; col < PENCIL_SIZE; col++)
                {
                    if (pencilMarks[row, col, 0] != 0)
                        Console.Write(pencilMarks[row, col, 0]);
                    else if (board[((row - (row % 3)) / 3), ((col - (col % 3)) / 3), currentLayer] != 0)
                        Console.Write("*");
                    else
                        Console.Write(" ");
                    if ((col % 3) == 2)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write("|");
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                    if (col % 9 == 8)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write("|");
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                }
                Console.WriteLine();
                if ((row % 3) == 2)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("---------------------------------------");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                if ((row % 9) == 8)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("---------------------------------------");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            }
            /**************************/
            /*    PRINT CANDIDATES    */
            /**************************/
            Console.WriteLine("-------------");
            for (int row = 1; row <= 9; row++)
            {
                Console.Write("|");
                for (int col = 1; col <= 9; col++)
                {
                    Console.Write(candidateCount[row - 1, col - 1]);
                    if ((col % 3) == 0)
                    {
                        Console.Write("|");
                    }
                }
                Console.WriteLine();
                if ((row % 3) == 0)
                {
                    Console.WriteLine("-------------");
                }
            }
        }
        static void setSlnLayer(int startLayer)
        {
            for (int row = 0; row < 9; row++)
                for (int col = 0; col < 9; col++)
                    board[row, col, (startLayer + 1)] = board[row, col, startLayer];
        }
        static void eraseCandidates()
        {
            for (int row = 0; row < 9; row++)
                for (int col = 0; col < 9; col++)
                    if (board[row, col, 0] != 0)
                        eraseCandidates(row, col, true, board[row, col, currentLayer]);
        }
        static void eraseCandidates(int _row, int _col, bool isSolved, int val)
        {
            if (isSolved)
            {
                /**************************/
                /*  ERASE INDIVIDUAL CELL */
                /**************************/
                for (int row = (_row * 3); row < ((_row * 3) + 3); row++)
                {
                    for (int col = (_col * 3); col < ((_col * 3) + 3); col++)
                    {
                        pencilMarks[row, col, 0] = 0;
                    }
                }
                /**************************/
                /*        ERASE BOX       */
                /**************************/
                eraseBox(_row, _col, val);
                /**************************/
                /*        ERASE ROW       */
                /**************************/
                eraseRow(_row, val);
                /**************************/
                /*      ERASE COLUMN      */
                /**************************/
                eraseCol(_col, val);
                /**************************/
                /*      UPDATE COUNT      */
                /**************************/
                updateCandidateCount();
            }
        }
        static void eraseBox(int _row, int _col, int val)
        {
            int row = 0, col = 0, max_row, max_col;
            switch (_row)
            {
                case 0:
                case 1:
                case 2:
                    switch (_col)
                    {
                        case 0:
                        case 1:
                        case 2:
                            row = 0;
                            col = 0;
                            break;
                        case 3:
                        case 4:
                        case 5:
                            row = 0;
                            col = 9;
                            break;
                        case 6:
                        case 7:
                        case 8:
                            row = 0;
                            col = 18;
                            break;
                    }
                    break;
                case 3:
                case 4:
                case 5:
                    switch (_col)
                    {
                        case 0:
                        case 1:
                        case 2:
                            row = 9;
                            col = 0;
                            break;
                        case 3:
                        case 4:
                        case 5:
                            row = 9;
                            col = 9;
                            break;
                        case 6:
                        case 7:
                        case 8:
                            row = 9;
                            col = 18;
                            break;
                    }
                    break;
                case 6:
                case 7:
                case 8:
                    switch (_col)
                    {
                        case 0:
                        case 1:
                        case 2:
                            row = 18;
                            col = 0;
                            break;
                        case 3:
                        case 4:
                        case 5:
                            row = 18;
                            col = 9;
                            break;
                        case 6:
                        case 7:
                        case 8:
                            row = 18;
                            col = 18;
                            break;
                    }
                    break;
            } //end switch (box)
            max_row = row + 9;
            max_col = col + 9;
            for (int y = row; y < max_row; y++)
                for (int x = col; x < max_col; x++)
                    if (pencilMarks[y, x, 0] == val)
                        pencilMarks[y, x, 0] = 0;
        }
        static void eraseRow(int _row, int val)
        {
            int row = 0, x, maxRow;
            switch (_row)
            {
                case 0:
                    row = 0;
                    break;
                case 1:
                    row = 3;
                    break;
                case 2:
                    row = 6;
                    break;
                case 3:
                    row = 9;
                    break;
                case 4:
                    row = 12;
                    break;
                case 5:
                    row = 15;
                    break;
                case 6:
                    row = 18;
                    break;
                case 7:
                    row = 21;
                    break;
                case 8:
                    row = 24;
                    break;
                case 9:
                    row = 27;
                    break;
            }//end switch
            maxRow = row + 3;
            for (x = row; x < maxRow; x++)
                for (int col = 0; col < 27; col++)
                    if (pencilMarks[x, col, 0] == val)
                        pencilMarks[x, col, 0] = 0;
        }
        static void eraseCol(int _col, int val)
        {
            int col = 0, y, maxCol;
            switch (_col)
            {
                case 0:
                    col = 0;
                    break;
                case 1:
                    col = 3;
                    break;
                case 2:
                    col = 6;
                    break;
                case 3:
                    col = 9;
                    break;
                case 4:
                    col = 12;
                    break;
                case 5:
                    col = 15;
                    break;
                case 6:
                    col = 18;
                    break;
                case 7:
                    col = 21;
                    break;
                case 8:
                    col = 24;
                    break;
                case 9:
                    col = 27;
                    break;
            }//end switch
            maxCol = col + 3;
            for (y = col; y < maxCol; y++)
                for (int row = 0; row < 27; row++)
                    if (pencilMarks[row, y, 0] == val)
                        pencilMarks[row, y, 0] = 0;
        }
        static void updateCandidateCount()
        {
            int count;
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    count = 0;
                    for (int _row = (row * 3); _row < ((row * 3) + 3); _row++)
                    {
                        for (int _col = (col * 3); _col < ((col * 3) + 3); _col++)
                        {
                            if (pencilMarks[_row, _col, 0] != 0)
                            {
                                count++;
                            }
                        }
                    }
                    candidateCount[row, col] = count;
                }
            }
        }
        static void eliminateSingles()
        {
            bool done = false;
            int row = 0, col = 0, iterations = 0, val;
            while (!done && iterations < 81)
            {
                row = 0;
                for (; row < 9; row++)
                {
                    col = 0;
                    for (; col < 9; col++)
                    {
                        val = 0;
                        if (candidateCount[row, col] == 1)
                        {
                            for (int _row = (row * 3); _row < ((row * 3) + 3); _row++)
                            {
                                for (int _col = (col * 3); _col < ((col * 3) + 3); _col++)
                                {
                                    if (pencilMarks[_row, _col, 0] != 0)
                                    {
                                        val = pencilMarks[_row, _col, 0];
                                        _row = 80;
                                        _col = 80;
                                    }
                                }
                            }
                            board[row, col, currentLayer] = val;
                            eraseCandidates(row, col, true, val);
                            row = 15;
                            col = 15;
                        }
                    }
                }
                iterations++;
                if (row == 9 && col == 9)
                    done = true;
            } //end while
        }
        static void findSinglesinMultiple()
        {
            bool done = false;
            int val = 0, iterations = 0, row = 0, col = 0, row_ = 0, col_ = 0;
            while (!done && iterations < 81)
            {
                row = 0;
                for (; row < 9; row++)
                {
                    col = 0;
                    for (; col < 9; col++)
                    {
                        val = 0;
                        for (int x = 0; x < candidateCount[row, col]; x++)
                        {
                            for (int _row = (row * 3); _row < ((row * 3) + 3); _row++)
                            {
                                for (int _col = (col * 3); _col < ((col * 3) + 3); _col++)
                                {
                                    if (pencilMarks[_row, _col, 0] != 0)
                                    {
                                        val = pencilMarks[_row, _col, 0];
                                        row_ = _row;
                                        col_ = _col;
                                        if (valIsAlone_Row(row_, col_, val))
                                        {
                                            board[row, col, currentLayer] = val;
                                            eraseCandidates(row, col, true, val);
                                            _row = 30;
                                            _col = 30;
                                        }
                                        else if (valIsAlone_Col(row_, col_, val))
                                        {
                                            board[row, col, currentLayer] = val;
                                            eraseCandidates(row, col, true, val);
                                            _row = 30;
                                            _col = 30;
                                        }

                                        else if (valIsAlone_Box(row_, col_, val))
                                        {
                                            board[row, col, currentLayer] = val;
                                            eraseCandidates(row, col, true, val);
                                            _row = 30;
                                            _col = 30;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                iterations++;
                if (row == 9 && col == 9)
                    done = true;
            } //end while
        }
        static bool valIsAlone_Row(int row, int col, int val)
        {
            bool isAlone = true;
            for (int x = (col % 3); x < 27; x += 3) //col
            {
                if ((pencilMarks[row, x, 0] != val) && (x != col) && (isAlone == true))
                {
                    isAlone = true;
                }
                else if (x != col)
                {
                    x = 30;
                    isAlone = false;
                }
            }
            if (isAlone)
                return true;
            else
                return false;
        }
        static bool valIsAlone_Col(int row, int col, int val)
        {
            bool isAlone = true;
            for (int x = (row % 3); x < 27; x += 3) //col
            {
                if ((pencilMarks[x, col, 0] != val) && (x != row) && (isAlone == true))
                {
                    isAlone = true;
                }
                else if (x != row)
                {
                    x = 30;
                    isAlone = false;
                }
            }
            if (isAlone)
                return true;
            else
                return false;
        }
        static bool valIsAlone_Box(int row, int col, int val)
        {
            bool isAlone = true;
            for (int x = (row - (row % 9)); x < (row - (row % 9) + 9); x++) //col
            {
                for (int y = (col - (col % 9)); y < (col - (col % 9) + 9); y++)
                {
                    if ((pencilMarks[x, y, 0] != val) && (x != row) && (isAlone == true))
                    {
                        isAlone = true;
                    }
                    else if ((pencilMarks[x, y, 0] != val) && (y != col) && (isAlone == true))
                    {
                        isAlone = true;
                    }
                    else if ((x == row && y != col) || (x != row && y == col) || (x != row && y != col))
                    {
                        x = 30;
                        y = 30;
                        isAlone = false;
                    }
                }
            }
            if (isAlone)
                return true;
            else
                return false;
        }
        static bool solvedState()
        {
            bool one = false, two = false, three = false, four = false, five = false, six = false, seven = false, eight = false, nine = false;
            bool row1 = false, row2 = false, row3 = false, row4 = false, row5 = false, row6 = false, row7 = false, row8 = false, row9 = false;
            bool col1 = false, col2 = false, col3 = false, col4 = false, col5 = false, col6 = false, col7 = false, col8 = false, col9 = false;
            bool box1 = false, box2 = false, box3 = false, box4 = false, box5 = false, box6 = false, box7 = false, box8 = false, box9 = false;
            bool rows = false, cols = false, boxs = false, isValid = false;
            /***********************************/
            /*           CHECK ROWS            */
            /***********************************/
            #region checkRows
            for (int i = 0; i < 9; i++)
            {
                one = false;
                two = false;
                three = false;
                four = false;
                five = false;
                six = false;
                seven = false;
                eight = false;
                nine = false;
                for (int row = 0; row < 9; row++)
                {
                    switch (board[row, i, currentLayer])
                    {
                        case 1:
                            one = true;
                            break;
                        case 2:
                            two = true;
                            break;
                        case 3:
                            three = true;
                            break;
                        case 4:
                            four = true;
                            break;
                        case 5:
                            five = true;
                            break;
                        case 6:
                            six = true;
                            break;
                        case 7:
                            seven = true;
                            break;
                        case 8:
                            eight = true;
                            break;
                        case 9:
                            nine = true;
                            break;
                    }//end switch
                }//end for (rows)
                if (one == true && two == true && three == true && four == true && five == true && six == true && seven == true && eight == true && nine == true)
                    isValid = true;
                else
                    isValid = false;
                switch (i)
                {
                    case 0:
                        row1 = isValid;
                        break;
                    case 1:
                        row2 = isValid;
                        break;
                    case 2:
                        row3 = isValid;
                        break;
                    case 3:
                        row4 = isValid;
                        break;
                    case 4:
                        row5 = isValid;
                        break;
                    case 5:
                        row6 = isValid;
                        break;
                    case 6:
                        row7 = isValid;
                        break;
                    case 7:
                        row8 = isValid;
                        break;
                    case 8:
                        row9 = isValid;
                        break;
                }
            }
            if (row1 == true && row2 == true && row3 == true && row4 == true && row5 == true && row6 == true && row7 == true && row8 == true && row9 == true)
                rows = true;
            else
                rows = false;
            #endregion
            /***********************************/
            /*         CHECK COLUMNS           */
            /***********************************/
            #region checkColumns
            for (int i = 0; i < 9; i++)
            {
                one = false;
                two = false;
                three = false;
                four = false;
                five = false;
                six = false;
                seven = false;
                eight = false;
                nine = false;
                for (int col = 0; col < 9; col++)
                {
                    switch (board[i, col, currentLayer])
                    {
                        case 1:
                            one = true;
                            break;
                        case 2:
                            two = true;
                            break;
                        case 3:
                            three = true;
                            break;
                        case 4:
                            four = true;
                            break;
                        case 5:
                            five = true;
                            break;
                        case 6:
                            six = true;
                            break;
                        case 7:
                            seven = true;
                            break;
                        case 8:
                            eight = true;
                            break;
                        case 9:
                            nine = true;
                            break;
                    }//end switch
                }//end for (cols)
                if (one == true && two == true && three == true && four == true && five == true && six == true && seven == true && eight == true && nine == true)
                    isValid = true;
                else
                    isValid = false;
                switch (i)
                {
                    case 0:
                        col1 = isValid;
                        break;
                    case 1:
                        col2 = isValid;
                        break;
                    case 2:
                        col3 = isValid;
                        break;
                    case 3:
                        col4 = isValid;
                        break;
                    case 4:
                        col5 = isValid;
                        break;
                    case 5:
                        col6 = isValid;
                        break;
                    case 6:
                        col7 = isValid;
                        break;
                    case 7:
                        col8 = isValid;
                        break;
                    case 8:
                        col9 = isValid;
                        break;
                }
            }
            if (col1 == true && col2 == true && col3 == true && col4 == true && col5 == true && col6 == true && col7 == true && col8 == true && col9 == true)
                cols = true;
            else
                cols = false;
            #endregion
            /***********************************/
            /*           CHECK BOXS            */
            /***********************************/
            #region checkBoxes
            for (int i = 0; i < 9; i++)
            {
                one = false;
                two = false;
                three = false;
                four = false;
                five = false;
                six = false;
                seven = false;
                eight = false;
                nine = false;
                int row = 0, col = 0, max_row, max_col;
                switch (i)
                {
                    case 0:
                        row = 0;
                        col = 0;
                        break;
                    case 1:
                        row = 0;
                        col = 3;
                        break;
                    case 2:
                        row = 0;
                        col = 6;
                        break;
                    case 3:
                        row = 3;
                        col = 0;
                        break;
                    case 4:
                        row = 3;
                        col = 3;
                        break;
                    case 5:
                        row = 3;
                        col = 6;
                        break;
                    case 6:
                        row = 6;
                        col = 0;
                        break;
                    case 7:
                        row = 6;
                        col = 3;
                        break;
                    case 8:
                        row = 6;
                        col = 6;
                        break;
                } //end switch (box)
                max_row = row + 3;
                max_col = col + 3;
                for (int y = row; y < max_row; y++)
                {
                    for (int x = col; x < max_col; x++)
                    {
                        switch (board[y, x, currentLayer])
                        {
                            case 1:
                                one = true;
                                break;
                            case 2:
                                two = true;
                                break;
                            case 3:
                                three = true;
                                break;
                            case 4:
                                four = true;
                                break;
                            case 5:
                                five = true;
                                break;
                            case 6:
                                six = true;
                                break;
                            case 7:
                                seven = true;
                                break;
                            case 8:
                                eight = true;
                                break;
                            case 9:
                                nine = true;
                                break;
                        }//end switch
                    }
                }//end for (rows)
                if (one == true && two == true && three == true && four == true && five == true && six == true && seven == true && eight == true && nine == true)
                    isValid = true;
                else
                    isValid = false;
                switch (i)
                {
                    case 0:
                        box1 = isValid;
                        break;
                    case 1:
                        box2 = isValid;
                        break;
                    case 2:
                        box3 = isValid;
                        break;
                    case 3:
                        box4 = isValid;
                        break;
                    case 4:
                        box5 = isValid;
                        break;
                    case 5:
                        box6 = isValid;
                        break;
                    case 6:
                        box7 = isValid;
                        break;
                    case 7:
                        box8 = isValid;
                        break;
                    case 8:
                        box9 = isValid;
                        break;
                }
            }
            if (box1 == true && box2 == true && box3 == true && box4 == true && box5 == true && box6 == true && box7 == true && box8 == true && box9 == true)
                boxs = true;
            else
                boxs = false;
            #endregion

            /***********************************/
            /*           FINAL CHECK           */
            /***********************************/
            if (rows == true && cols == true && boxs == true)
                return true;
            else
                return false;
        }
        static void countSolved()
        {
            solveCount = 0;
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (board[row, col, currentLayer] != 0)
                    {
                        solveCount++;
                    }
                }
            }
        }
        static void bruteForce()
        {
            int lowestCount = 65535;
            int _row = 0, _col = 0, countOfPossibles = 0, count = 0;
            int[] possibles;
            int splitLayer = 0;
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (candidateCount[row, col] != 0 && candidateCount[row, col] < lowestCount)
                    {
                        lowestCount = candidateCount[row, col];
                        _row = row;
                        _col = col;
                    }
                }
            }
            possibles = new int[lowestCount];
            for (int row = (_row * 3); row < ((_row * 3) + 3); row++)
            {
                for (int col = (_col * 3); col < ((_col * 3) + 3); col++)
                {
                    if (pencilMarks[row, col, 0] != 0)
                    {
                        possibles[countOfPossibles] = pencilMarks[row, col, 0];
                        countOfPossibles++;
                    }
                }
            }
            splitLayer = currentLayer;
            setSlnLayer(currentLayer);
            for (int row = 0; row < PENCIL_SIZE; row++)
                for (int col = 0; col < PENCIL_SIZE; col++)
                    pencilMarks[row, col, 1] = pencilMarks[row, col, 0];
            while (!solvedState() && count < countOfPossibles)
            {
                currentLayer = splitLayer;
                setSlnLayer(currentLayer);
                currentLayer++;
                setSlnLayer(currentLayer);
                for (int row = 0; row < PENCIL_SIZE; row++)
                    for (int col = 0; col < PENCIL_SIZE; col++)
                        pencilMarks[row, col, 0] = pencilMarks[row, col, 1];
                updateCandidateCount();
                eraseCandidates(_row, _col, true, possibles[count]);
                solveForSingles();
                board[_row, _col, currentLayer] = possibles[count];
                count++;
            }
        }
    }
}

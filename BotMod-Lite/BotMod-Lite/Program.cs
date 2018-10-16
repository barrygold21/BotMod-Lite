using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMod {

    public class GlobalVars {
        public static string robotsName = "Fred";
        public static int passengers = 0;
        public static int maxPassengers = 1;
        public static int traction = 0;
        public const int landscapeSize = 10;
        public static int[,] terrain = new int[landscapeSize, landscapeSize];
        public static int[,] landscape = new int[landscapeSize, landscapeSize];
        public static int powerRemaining;
        public static int pplSaved = 0;
        public static readonly int[,] powerLoss = new int[3, 3] {
                {1, 2, 3},
                {2, 1, 2},
                {3, 3, 1},
        };
        public static int x; // robots x pos
        public static int y; // robots y pos
    }

    class Program {

        static void Main() {
            bool inputValid = false;
            int mCho = 0;
            do {
                Console.Clear();
                Console.WriteLine(" 1. Play Game    ");
                Console.WriteLine(" 2. Modify Robot ");
                Console.WriteLine(" 3. Quit         ");
                Console.WriteLine("Please enter your choice (1 - 4). . .");
                Console.WriteLine();
                string userChoice = Console.ReadLine();
                bool isNumeric = int.TryParse(userChoice, out mCho);
                if (isNumeric == true) {
                    inputValid = true;
                    switch (mCho) {
                        case 1:
                            PlayGame();
                            break;
                        case 2:
                            inputValid = false;
                            ModifyRobot();
                            break;
                        case 3:
                            Environment.Exit(0);
                            break;
                        default:
                            inputValid = Error(2);
                            break;
                    }
                }
                else {
                    Error(1);
                }
            } while (!inputValid);
        }

        static bool Error(int errorCode) {
            Console.Clear();
            switch (errorCode) {
                case 1:
                    Console.WriteLine("Please enter a number!");
                    break;
                case 2:
                    Console.WriteLine("Valid choice not entered. Please enter an integer between 1 and 4.");
                    break;
                case 3:
                    Console.WriteLine("Valid choice not entered.");
                    break;
                case 4:
                    Console.WriteLine("Move invalid! You have attempted to move out of the grid. Please try again.");
                    System.Threading.Thread.Sleep(800);
                    Console.Clear();
                    DrawLandscape();
                    break;
                case 5:
                    Console.WriteLine("Valid choice not entered. Please enter an integer between 1 and 3.");
                    break;
                case 6:
                    Console.WriteLine("Please enter a valid robot name.");
                    break;
                default:
                    Console.WriteLine("Unknown error code.");
                    break;
            }
            System.Threading.Thread.Sleep(800);
            return false;
        }

        static void ModifyRobot() {
            bool goBackToMainMenu = false;
            int mCho = 0;
            do {
                Console.Clear();
                Console.WriteLine("Modify Robot");
                Console.WriteLine(" 1. Name Robot ({0})", GlobalVars.robotsName);
                Console.WriteLine(" 2. Change Traction");
                Console.WriteLine(" 3. Change Passenger Bay Size");
                Console.WriteLine(" 4. Go To Main Menu");
                Console.WriteLine("Please enter your choice (1 - 4). . .");
                Console.WriteLine();
                string userChoice = Console.ReadLine();
                bool isNumeric = int.TryParse(userChoice, out mCho);
                if (isNumeric) {
                    switch (mCho) {
                        case 1:
                            NameRobot();
                            break;
                        case 2:
                            SelectTractionType();
                            break;
                        case 3:
                            ChangeBaySize();
                            break;
                        case 4:
                            goBackToMainMenu = true;
                            break;
                        default:
                            Error(2);
                            break;
                    }
                }
                else {
                    Error(1);
                }
            } while (!goBackToMainMenu);
        }

        static void NameRobot() {
            bool nameValid = false;
            do {
                Console.Clear();
                Console.WriteLine("Current Robot Name: {0}", GlobalVars.robotsName);
                string userInput = Console.ReadLine();
                string rbFChr = userInput.Substring(0, 1);
                string rbFChrUpper = rbFChr.ToUpper();
                if (userInput.Length > 3 && rbFChr == rbFChrUpper) {
                    GlobalVars.robotsName = userInput;
                    Console.WriteLine("Name set to {0}", GlobalVars.robotsName);
                    System.Threading.Thread.Sleep(800);
                    nameValid = true;
                }
                else {
                    nameValid = Error(6);
                }
            } while (nameValid == false);
        }

        static void SelectTractionType() {
            bool inputValid = false;
            do {
                Console.Clear();
                Console.WriteLine("Traction Types:");
                Console.WriteLine("wheels");
                Console.WriteLine("tracks");
                Console.WriteLine("skis");
                Console.WriteLine("Please enter which traction type you wish to use:");
                string userInput = Console.ReadLine();
                inputValid = true;
                switch (userInput) {
                    case "wheels":
                        GlobalVars.traction = 0;
                        break;
                    case "tracks":
                        GlobalVars.traction = 1;
                        break;
                    case "skis":
                        GlobalVars.traction = 2;
                        break;
                    default:
                        inputValid = Error(3);
                        break;

                }
            } while (!inputValid);
        }

        static void ChangeBaySize() {
            bool inputValid = false;
            do {
                Console.Clear();
                Console.WriteLine("Your passenger bay can be the following sizes:");
                Console.WriteLine("small");
                Console.WriteLine("med");
                Console.WriteLine("large");
                Console.WriteLine("Please enter which bay size you wish to use:");
                Console.WriteLine();
                string userInput = Console.ReadLine();
                inputValid = true;
                switch (userInput) {
                    case "small":
                        GlobalVars.maxPassengers = 1;
                        break;
                    case "med":
                        GlobalVars.maxPassengers = 2;
                        break;
                    case "large":
                        GlobalVars.maxPassengers = 3;
                        break;
                    default:
                        inputValid = Error(3);
                        break;
                }
            } while (!inputValid);
        }

        static void PlayGame() {
            Console.Clear();
            Init();
            bool gameOver = false;
            bool gameWon = false;
            do {
                DrawLandscape();
                string move = EnterMove(); // take in move
                Console.WriteLine();
                System.Threading.Thread.Sleep(100);
                GlobalVars.powerRemaining -= GlobalVars.maxPassengers - 1;
                GlobalVars.powerRemaining -= GlobalVars.powerLoss[GlobalVars.traction, GlobalVars.terrain[GlobalVars.x, GlobalVars.y]];
                if (GlobalVars.landscape[GlobalVars.x, GlobalVars.y] == 1) {
                    if (GlobalVars.passengers < GlobalVars.maxPassengers) {
                        GlobalVars.passengers++;
                        GlobalVars.landscape[GlobalVars.x, GlobalVars.y] = 0;
                    }
                }
                if (GlobalVars.landscape[GlobalVars.x, GlobalVars.y] == 2) {
                    GlobalVars.pplSaved += GlobalVars.passengers;
                    GlobalVars.passengers = 0;
                }
                Console.Clear();
                if (GlobalVars.powerRemaining == 0 && GlobalVars.pplSaved == 4) {
                    gameWon = true;
                    gameOver = true;
                    break;
                }
                else if (GlobalVars.powerRemaining == 0) {
                    gameOver = true;
                    break;
                }
                else if (GlobalVars.pplSaved == 4) {
                    gameWon = true;
                    break;
                }
                else {
                    gameOver = false;
                    gameWon = false;
                }
            } while (!gameOver || !gameWon);
            if (gameWon) {
                Console.WriteLine("You win!");
            }
            else if (gameOver) {
                Console.WriteLine("You lost.");
            }
            else if (gameWon && gameOver) {
                Console.WriteLine("You win!");
            }
            System.Threading.Thread.Sleep(1500);
        }

        static void Init() {
            int k = GlobalVars.landscapeSize - 1;
            string filePath = System.IO.Path.GetFullPath("terraindata.txt");
            using (StreamReader fileReader = new StreamReader(filePath)) {
                string line;
                int i = 0;
                int j = 0;
                do {
                    if (i == k + 1) {
                        break;
                    }
                    line = fileReader.ReadLine();
                    int numeric = Convert.ToInt32(line);
                    GlobalVars.terrain[i, j] = numeric;
                    if (j == k) {
                        j = 0;
                        i++;
                    }
                    else {
                        j++;
                    }
                } while (!fileReader.EndOfStream);
            }
            k++;
            for (int i = 0; i < k; i++) {
                for (int j = 0; j < k; j++) {
                    GlobalVars.landscape[i, j] = 0;
                }
            }
            GlobalVars.x = 0;
            GlobalVars.y = 0;
            GlobalVars.pplSaved = 0;
            GlobalVars.powerRemaining = 150;
            GlobalVars.landscape[0, 2] = 1;
            GlobalVars.landscape[2, 7] = 1;
            GlobalVars.landscape[6, 4] = 1;
            GlobalVars.landscape[6, 9] = 1;
            GlobalVars.landscape[3, 6] = 2;
        }

        static void DrawLandscape() {
            Console.WriteLine();
            int k = GlobalVars.landscapeSize;
            for (int i = 0; i < k; i++) {
                for (int j = 0; j < k; j++) {
                    if (i == GlobalVars.x && j == GlobalVars.y) {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("R");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else {
                        switch (GlobalVars.terrain[i, j]) {
                            case 0:
                                Console.ForegroundColor = ConsoleColor.Green;
                                break;
                            case 1:
                                Console.ForegroundColor = ConsoleColor.DarkGray;
                                break;
                            case 2:
                                Console.ForegroundColor = ConsoleColor.Blue;
                                break;
                        }
                        switch (GlobalVars.landscape[i, j]) {
                            case 0:
                                Console.Write(".");
                                break;
                            case 1:
                                Console.Write("P");
                                break;
                            case 2:
                                Console.Write("B");
                                break;
                        }
                    }
                    Console.Write(" ");
                }
                Console.WriteLine();
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Power Remaining: {0}", GlobalVars.powerRemaining);
            Console.WriteLine("Passengers: {0}", GlobalVars.passengers);
        }

        static string EnterMove() {
            string move;
            bool moveIsValid = false;
            string inputMove;
            do {
                Console.WriteLine();
                Console.WriteLine("Please enter a move for {0} to take:", GlobalVars.robotsName);
                inputMove = Console.ReadLine();
                inputMove = inputMove.ToUpper();
                if (inputMove == "N" || inputMove == "E" || inputMove == "S" || inputMove == "W") {
                    moveIsValid = true;
                    switch (inputMove) {
                        case "N":
                            if (GlobalVars.x != 0) {
                                GlobalVars.x--;
                            }
                            break;
                        case "E":
                            if (GlobalVars.y != 9) {
                                GlobalVars.y++;
                            }
                            break;
                        case "S":
                            if (GlobalVars.x != 9) {
                                GlobalVars.x++;
                            }
                            break;
                        case "W":
                            if (GlobalVars.y != 0) {
                                GlobalVars.y--;
                            }
                            break;
                        default:
                            moveIsValid = Error(4);
                            break;
                    }
                }
                else {
                    moveIsValid = Error(4);
                }
            } while (!moveIsValid);
            move = inputMove;
            Console.WriteLine("Move Valid.");
            System.Threading.Thread.Sleep(200);
            return move;
        }
    }
}
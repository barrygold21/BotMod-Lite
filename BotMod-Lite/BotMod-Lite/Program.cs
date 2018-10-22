using System;
using System.IO;

namespace BotMod_Lite {
   
    class Variables {
        public string robotsName = "Fred";
        public int passengers = 0;
        public int maxPassengers = 1;
        public int traction = 0;
        public const int landscapeSize = 10;
        public int[,] terrain = new int[landscapeSize, landscapeSize];
        public int[,] landscape = new int[landscapeSize, landscapeSize];
        public int powerRemaining;
        public int pplSaved = 0;
        public readonly int[,] powerLoss = new int[3, 3] {
                {1, 2, 3},
                {2, 1, 2},
                {3, 3, 1},
        };
        public int x; // robots x pos
        public int y; // robots y pos
    }

    class Program : Variables {

        static void Main() {
            Program p = new Program();
            bool inputValid = false;
            int mCho = 0;
            do {
                Console.Clear();
                Console.WriteLine(" 1. Play Game \n 2. Modify Robot \n 3. Quit");
                Console.WriteLine("Please enter your choice (1 - 4). . .");
                Console.WriteLine();
                string userChoice = Console.ReadLine();
                bool isNumeric = int.TryParse(userChoice, out mCho);
                if (isNumeric == true) {
                    inputValid = true;
                    switch (mCho) {
                        case 1:
                            p.PlayGame();
                            break;
                        case 2:
                            inputValid = false;
                            p.ModifyRobot();
                            break;
                        case 3:
                            Environment.Exit(0);
                            break;
                        default:
                            inputValid = p.Error(2);
                            break;
                    }
                }
                else { p.Error(1); }
            } while (!inputValid);
        }
        
         bool Error(int code) {
            Console.Clear();
            switch (code) {
                case 1:
                    Console.WriteLine("Please enter a number! \n");
                    break;
                case 2:
                    Console.WriteLine("Valid choice not entered. Please enter an integer between 1 and 4. \n");
                    break;
                case 3:
                    Console.WriteLine("Valid choice not entered.");
                    break;
                case 4:
                    Console.WriteLine("Move invalid! You have attempted to move out of the grid. Please try again. \n");
                    System.Threading.Thread.Sleep(800);
                    Console.Clear();
                    DrawLandscape();
                    break;
                case 5:
                    Console.WriteLine("Valid choice not entered. Please enter an integer between 1 and 3. \n");
                    break;
                case 6:
                    Console.WriteLine("Please enter a valid robot name. \n");
                    break;
                default:
                    Console.WriteLine("Unknown error code. \n");
                    break;
            }
            System.Threading.Thread.Sleep(800);
            return false;
         }

        void ModifyRobot() {
            bool goBackToMainMenu = false;
            int mCho = 0;
            do {
                Console.Clear();
                Console.WriteLine("Modify Robot");
                Console.WriteLine(" 1. Name Robot ({0}) \n 2. Change Traction \n 3. Change Passenger Bay Size \n 4. Go To Main Menu", robotsName);
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
                else { Error(1); }
            } while (!goBackToMainMenu);
        }

        void NameRobot() {
            bool nameValid = false;
            do {
                Console.Clear();
                Console.WriteLine("Current Robot Name: {0}", robotsName);
                string userInput = Console.ReadLine();
                string rbFChr = userInput.Substring(0, 1);
                string rbFChrUpper = rbFChr.ToUpper();
                if (userInput.Length > 3 && rbFChr == rbFChrUpper) {
                    robotsName = userInput;
                    Console.WriteLine("Name set to {0}", robotsName);
                    System.Threading.Thread.Sleep(800);
                    nameValid = true;
                }
                else { nameValid = Error(6); }
            } while (nameValid == false);
        }

        void SelectTractionType() {
            bool inputValid = false;
            do {
                Console.Clear();
                Console.WriteLine("Traction Types: \n wheels \n tracks \n skis");
                Console.WriteLine("Please enter which traction type you wish to use:");
                string userInput = Console.ReadLine();
                inputValid = true;
                switch (userInput) {
                    case "wheels":
                        traction = 0;
                        break;
                    case "tracks":
                        traction = 1;
                        break;
                    case "skis":
                        traction = 2;
                        break;
                    default:
                        inputValid = Error(3);
                        break;
                }
            } while (!inputValid);
        }

        void ChangeBaySize() {
            bool inputValid = false;
            do {
                Console.Clear();
                Console.WriteLine("Your passenger bay can be the following sizes: \n small \n med \n large");
                Console.WriteLine("Please enter which bay size you wish to use:");
                Console.WriteLine();
                string userInput = Console.ReadLine();
                inputValid = true;
                switch (userInput) {
                    case "small":
                        maxPassengers = 1;
                        break;
                    case "med":
                        maxPassengers = 2;
                        break;
                    case "large":
                        maxPassengers = 3;
                        break;
                    default:
                        inputValid = Error(3);
                        break;
                }
            } while (!inputValid);
        }

         void PlayGame() {
            Console.Clear();
            Init();
            bool gameOver = false;
            do {
                DrawLandscape();
                string move = EnterMove(); // take in move
                Console.WriteLine();
                System.Threading.Thread.Sleep(100);
                powerRemaining -= maxPassengers - 1;
                powerRemaining -= powerLoss[traction, terrain[x, y]];
                if (landscape[x, y] == 1) {
                    if (passengers < maxPassengers) {
                        passengers++;
                        landscape[x, y] = 0;
                    }
                }
                if (landscape[x, y] == 2) {
                    pplSaved += passengers;
                    passengers = 0;
                }
                Console.Clear();
                if (powerRemaining == 0 || pplSaved == 4) {
                    if (pplSaved == 4) {
                        Console.WriteLine("You win!");
                    }
                    else if (powerRemaining == 0) {
                        Console.WriteLine("You lose. :(");
                    }
                    gameOver = true;
                }
            } while (!gameOver);
            System.Threading.Thread.Sleep(1500);
         }

        void Init() {
            int k = landscapeSize;
            using (var reader = new StringReader(Data.Terrain)) {
                int lineContent;
                int i = 0;
                int j = 0;
                do {
                    lineContent = Convert.ToInt32(reader.ReadLine());
                    terrain[i, j] = lineContent;
                    if (j == k - 1) {
                        j = 0;
                        i++;
                    }
                    else { j++; }
                    if (i == k) { break; }
                } while (true);
            }
            for (int i = 0; i < k; i++) {
                for (int j = 0; j < k; j++) {
                    landscape[i, j] = 0;
                }
            }
            x = 0;
            y = 0;
            pplSaved = 0;
            powerRemaining = 150;
            landscape[0, 2] = 1;
            landscape[2, 7] = 1;
            landscape[6, 4] = 1;
            landscape[6, 9] = 1;
            landscape[3, 6] = 2;
        }

        void DrawLandscape() {
            Console.WriteLine();
            int k = landscapeSize;
            for (int i = 0; i < k; i++) {
                for (int j = 0; j < k; j++) {
                    if (i == x && j == y) {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("R");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else {
                        switch (terrain[i, j]) {
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
                        switch (landscape[i, j]) {
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
            Console.WriteLine("\nPower Remaining: {0}\tPassengers: {1}", powerRemaining, passengers);
        }

        string EnterMove() {
            string move;
            bool moveIsValid = false;
            do {
                Console.WriteLine();
                Console.WriteLine("Please enter a move for {0} to take:", robotsName);
                move = Console.ReadLine();
                move = move.ToUpper();
                if (move == "N" || move == "E" || move == "S" || move == "W") {
                    moveIsValid = true;
                    switch (move) {
                        case "N":
                            if (x != 0) {
                                x--;
                            }
                            break;
                        case "E":
                            if (y != 9) {
                                y++;
                            }
                            break;
                        case "S":
                            if (x != 9) {
                                x++;
                            }
                            break;
                        case "W":
                            if (y != 0) {
                                y--;
                            }
                            break;
                        default:
                            moveIsValid = Error(4);
                            break;
                    }
                }
                else { moveIsValid = Error(4); }
            } while (!moveIsValid);
            Console.WriteLine("Move Valid.");
            System.Threading.Thread.Sleep(200);
            return move;
        }
    }
}
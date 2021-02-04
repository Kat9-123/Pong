/* PONG | By: Kat9_123 */

using System;
using System.Threading;

namespace Pong
{
    class Program
    {
        const int VIEW_SIZE_X = 30;
        const int VIEW_SIZE_Y = 16;

        const int PADDLE_SIZE = 4;
        
        const int BALL_MOVE_DISTANCE = 1;

        const int X_OFFSET = 2;


        // Ball
        static double ballX = 15;
        static double ballY = 4;

        static double ballVX = 0;
        static double ballVY = -1;

        // The rotation is a bit off (90 deg) but I'm too scared to mess with it

        static int roundedBallX = 0;
        static int roundedBallY = 0;

        // Players

        static int leftPoints = 0;
        static int rightPoints = 0;
        static int leftPlayerPos = 3;

        static int rightPlayerPos = 3;

        static ConsoleKeyInfo input; 

        static void Main(string[] args)
        {
            NewBall();
            Console.Title = "PONG | By: Kat9_123";
            Console.CursorVisible = false;
            Thread inputThread = new Thread(Input);
            inputThread.Start();

            // Game loop
            while (true)
            {
                char[,] view = RenderBackground();

                InputHandler();
                input = new ConsoleKeyInfo();

                view = RenderPaddles(view);

                BallCollision();
                MoveBall();

                view = RenderBall(view);

                Draw(view);
                Thread.Sleep(50);
            }
        }
        static void NewBall()
        {
            ballX = 15;
            ballY = 4;
            Random random = new Random();
            double vx = random.NextDouble() * (0.6 - 0.3) + 0.3;
            ballVY = 1 - vx;
            if (random.Next(0,2) == 1)
            {
                vx *= -1;
            }
            ballVX = vx;
           
        }
        static char[,] RenderPaddles(char[,] view)
        {
            
            // Left
            for (int i = 1; i <= PADDLE_SIZE; i++)
            {
                view[i+leftPlayerPos,X_OFFSET+1] = 'O';
            }
            

            // Right
            for (int i = 1; i <= PADDLE_SIZE; i++)
            {
                view[i+rightPlayerPos,VIEW_SIZE_X-(X_OFFSET+2)] = 'O';           
            }

            return view;
        }


        static double Radians(double angle)
        {
            return (Math.PI / 180) * angle;
        }
        static void MoveBall()
        {
            
            ballX += ballVX;
            ballY -= ballVY;

            roundedBallX = (int) Math.Floor(ballX);
            roundedBallY = (int) Math.Floor(ballY);
        }

        static void BallCollision()
        {
            // Roof & Floor
            if (ballY > (VIEW_SIZE_Y-2) || ballY < 2)
            {
                ballVY = ballVY * -1;
            }

            // Left paddle
            if (roundedBallX == X_OFFSET + 2)
            {
                if (roundedBallY >= leftPlayerPos && roundedBallY <= (leftPlayerPos+PADDLE_SIZE))
                {
                    double relativeIntersectY = (leftPlayerPos+(PADDLE_SIZE/2)) - ballY;
                    double normalizedRelativeIntersectionY = (relativeIntersectY/(PADDLE_SIZE/2));
                    double bounceAngle = normalizedRelativeIntersectionY * Radians(75);

                    ballVX = Math.Cos(bounceAngle);
                    ballVY = -Math.Sin(bounceAngle);

                }
                
            }

            // Right paddle
            if(roundedBallX == VIEW_SIZE_X - (X_OFFSET + 3))
            {
                if (roundedBallY >= rightPlayerPos && roundedBallY <= (rightPlayerPos+PADDLE_SIZE))
                {
                    double relativeIntersectY = (rightPlayerPos+(PADDLE_SIZE/2)) - ballY;
                    double normalizedRelativeIntersectionY = (relativeIntersectY/(PADDLE_SIZE/2));
                    double bounceAngle = normalizedRelativeIntersectionY * Radians(75);

                    ballVX = -Math.Cos(bounceAngle);
                    ballVY = -Math.Sin(bounceAngle);
                }
            }

            // Left top & bottom
            if(roundedBallX == X_OFFSET + 1)
            {
                
                if (roundedBallY == leftPlayerPos || roundedBallY == (leftPlayerPos+PADDLE_SIZE+1))
                {
                    ballVY = ballVY * -1;
                }
            }


            // Right top & bottom
            if(roundedBallX == VIEW_SIZE_X - (X_OFFSET + 2))
            {
                
                if (roundedBallY == rightPlayerPos || roundedBallY == (rightPlayerPos+PADDLE_SIZE+1))
                {
                    ballVY = ballVY * -1;
                }
            }



            

            // Left side right points
            if (ballX <= 2) 
            {
                rightPoints++; 
                NewBall();
            }
            
            // Right side left points
            if (ballX >= (VIEW_SIZE_X-2))
            {
                leftPoints++; 
                NewBall();
            }
        }

        static char[,] RenderBall(char[,] view)
        {
            view[roundedBallY,roundedBallX] = '0';
            return view;
        }
        static char[,] RenderBackground()
        {
            char[,] bg = new char[VIEW_SIZE_Y,VIEW_SIZE_X];
            for (int y = 0; y < VIEW_SIZE_Y; y++)
            {
                for (int x = 0; x < VIEW_SIZE_X; x++)
                {
                    
                    if(x == 0 || y == 0 || x == VIEW_SIZE_X-1 || y == VIEW_SIZE_Y-1) bg[y,x] = '#';
                    else bg[y,x] = '-';
                }
            }
            return bg;
        }

        static void Draw(char[,] view)
        {
            Console.WriteLine();
            Console.WriteLine("   " + leftPoints + "                      " + rightPoints);
            Console.WriteLine();
            for (int y = 0; y < VIEW_SIZE_Y; y++)
            {
                string line = "";
                for (int x = 0; x < VIEW_SIZE_X; x++)
                {
                    line += view[y,x];
                }
                Console.WriteLine(line);

            }
            Console.SetCursorPosition(0, 0);
        }


        static void InputHandler()
        {
            switch (input.Key)
            {

                // Left
                case ConsoleKey.W:
                    if(leftPlayerPos != 0) leftPlayerPos -= 1;
                    break;

                case ConsoleKey.S:
                    if(leftPlayerPos != (VIEW_SIZE_Y-2) - PADDLE_SIZE) leftPlayerPos += 1;
                    break;
                
                // Right
                case ConsoleKey.UpArrow:
                    if(rightPlayerPos != 0) rightPlayerPos -= 1;
                    break;

                case ConsoleKey.DownArrow:
                    if(rightPlayerPos != (VIEW_SIZE_Y-2) - PADDLE_SIZE) rightPlayerPos += 1;
                    break;

                case ConsoleKey.Escape:
                    Environment.Exit(1);
                    break;

            }
        }
        static void Input()
        {
            while (true)
            {
                input = Console.ReadKey(true);
            }
        }
    }
}

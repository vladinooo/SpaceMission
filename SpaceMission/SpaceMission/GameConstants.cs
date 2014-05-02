using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace SpaceMission
{
    class GameConstants
    {
        // general
        public const int numOfAstronauts = 3;

        // round time
        public static readonly TimeSpan RoundTime = TimeSpan.FromSeconds(120.25);

        // game stste management strings
        public const string StrTimeRemaining = "Time Remaining: \n";
        public const string StrAstronautsCollected = "Astronauts collected: \n";
        public const string StrGameWon = "Congratulation! Mission Completed! Score: ";
        public const string StrGameLost = "Mission Failed! Score: ";
        public const string StrExit = "Press Esc to quit";
        public const string StrInstructions1 = "- Collect all astronauts to complete the Space Mission \n- Control spaceship using keyboard (Left, Right, Up, Down, A, Z, Q, W) \n- Press (Space) to switch to orbit camera and (J, L, I, K) to navigate it\n- Press (Enter) to start";

    }
}

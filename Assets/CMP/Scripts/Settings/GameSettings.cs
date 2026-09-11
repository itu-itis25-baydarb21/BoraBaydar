namespace CMP.Scripts
{
    public static class GameSettings
    {
        public const float AiMovementDuration = 0.25f;
        public const float PacmanMovementDuration = 0.25f;
        public const int AiCharacterCount = 3;
        public static readonly float[] AiJoinDelays = { 3f, 6f, 9f };
        public static float CatchDistance = 0.5f;
        public static readonly Direction[] DirectionsToCheck =
            { Direction.Left, Direction.Right, Direction.Up, Direction.Down };

        public static float CameraPadding = 1f;

        public static float TileArrivedTolerance = 0.02f;
        public static float AiMaxChaseDuration = 10f;
        public static float AiSightLostLimit = 3f;
        public static float AiSightDistance = 6f;
    }
}
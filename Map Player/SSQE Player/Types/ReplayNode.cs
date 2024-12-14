namespace SSQE_Player.Types
{
    internal enum ReplayType
    {
        Cursor,
        Skip
    }

    internal record ReplayNode(float X, float Y, int Ms, ReplayType Type);
}

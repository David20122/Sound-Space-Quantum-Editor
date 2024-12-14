namespace New_SSQE.Objects.Other
{
    internal enum ReplayType
    {
        Cursor,
        Skip
    }

    internal record ReplayNode(float X, float Y, int Ms, ReplayType Type);
}

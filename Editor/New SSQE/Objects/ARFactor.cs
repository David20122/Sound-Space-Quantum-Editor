namespace New_SSQE.Objects
{
    internal class ARFactor : MapObject
    {
        public float Factor;

        public ARFactor(long ms, float factor) : base(10, ms, "AR Factor")
        {
            Factor = factor;
        }

        public override string ToString(params object[] data)
        {
            return base.ToString(
                Math.Round(Factor, 2).ToString(Program.Culture)
            );
        }

        public override ARFactor Clone()
        {
            return new(Ms, Factor);
        }
    }
}

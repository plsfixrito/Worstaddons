namespace KappaUtility.Brain.Utility.Misc.AutoLvlup
{
    internal class Sequence
    {
        internal static int[] QWER
        {
            get
            {
                return new[] { 1, 2, 3, 1, 1, 4, 1, 2, 1, 2, 4, 2, 3, 2, 3, 4, 3, 3 };
            }
        }

        internal static int[] QEWR
        {
            get
            {
                return new[] { 1, 3, 2, 1, 1, 4, 1, 3, 1, 3, 4, 3, 2, 3, 2, 4, 2, 2 };
            }
        }

        internal static int[] WQER
        {
            get
            {
                return new[] { 2, 1, 3, 2, 2, 4, 2, 1, 2, 1, 4, 1, 3, 1, 3, 4, 3, 3 };
            }
        }

        internal static int[] WEQR
        {
            get
            {
                return new[] { 2, 3, 1, 2, 2, 4, 2, 3, 2, 3, 4, 3, 1, 3, 1, 4, 1, 1 };
            }
        }

        internal static int[] EQWR
        {
            get
            {
                return new[] { 3, 1, 2, 3, 3, 4, 3, 1, 3, 1, 4, 1, 2, 1, 2, 4, 2, 2 };
            }
        }

        internal static int[] EWQR
        {
            get
            {
                return new[] { 3, 2, 1, 3, 3, 4, 3, 2, 3, 2, 4, 2, 1, 2, 1, 4, 1, 1 };
            }
        }
    }
}

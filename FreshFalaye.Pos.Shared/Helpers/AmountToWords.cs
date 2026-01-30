namespace FreshFalaye.Pos.Shared.Helpers
{
    public static class AmountToWords
    {
        static readonly string[] Units =
        {
        "", "One", "Two", "Three", "Four", "Five",
        "Six", "Seven", "Eight", "Nine", "Ten",
        "Eleven", "Twelve", "Thirteen", "Fourteen",
        "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen"
    };

        static readonly string[] Tens =
        {
        "", "", "Twenty", "Thirty", "Forty",
        "Fifty", "Sixty", "Seventy", "Eighty", "Ninety"
    };

        public static string Convert(decimal amount)
        {
            if (amount == 0)
                return "Rupees Zero Only";

            long rupees = (long)amount;
            int paise = (int)((amount - rupees) * 100);

            var words = $"Rupees {NumberToWords(rupees)}";

            if (paise > 0)
                words += $" and {NumberToWords(paise)} Paise";

            return words + " Only";
        }

        static string NumberToWords(long number)
        {
            if (number == 0)
                return "";

            if (number < 20)
                return Units[number];

            if (number < 100)
                return Tens[number / 10] + " " + Units[number % 10];

            if (number < 1000)
                return Units[number / 100] + " Hundred " + NumberToWords(number % 100);

            if (number < 100000)
                return NumberToWords(number / 1000) + " Thousand " + NumberToWords(number % 1000);

            if (number < 10000000)
                return NumberToWords(number / 100000) + " Lakh " + NumberToWords(number % 100000);

            return NumberToWords(number / 10000000) + " Crore " + NumberToWords(number % 10000000);
        }
    }
}

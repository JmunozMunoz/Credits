using System;

namespace Sc.Credits.Helpers.ObjectsUtils
{
    /// <summary>
    /// Number conversions
    /// </summary>
    public static class NumberConversions
    {
        /// <summary>
        /// To letters spanish
        /// </summary>
        /// <param name="numberToParse"></param>
        /// <returns></returns>
        public static string ToLettersSpanish(string numberToParse)
        {
            long digitsCount;

            long.TryParse(numberToParse.Split(',')[0], out long number);
            digitsCount = number.ToString().Length;

            if (digitsCount == 0)
            {
                return "Cero";
            }
            else
            {
                if (digitsCount <= 3)
                {
                    return GroupHundreds(number);
                }
                else if (digitsCount <= 6)
                {
                    return GroupThousands(number);
                }
                else if (digitsCount <= 9)
                {
                    return GroupMillions(number);
                }
                else if (digitsCount <= 12)
                {
                    return GroupThousandsMillions(number);
                }
                else if (digitsCount > 12)
                {
                    throw new InvalidOperationException("Invalid number cast");
                }

                return UnitsAndTens(number);
            }
        }

        /// <summary>
        /// Units and tens
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string UnitsAndTens(long number)
        {
            string[] numbers = new string[100];
            string result;

            numbers[0] = "";
            numbers[1] = "Un";
            numbers[2] = "Dos";
            numbers[3] = "Tres";
            numbers[4] = "Cuatro";
            numbers[5] = "Cinco";
            numbers[6] = "Seis";
            numbers[7] = "Siete";
            numbers[8] = "Ocho";
            numbers[9] = "Nueve";
            numbers[10] = "Diez";
            numbers[11] = "Once";
            numbers[12] = "Doce";
            numbers[13] = "Trece";
            numbers[14] = "Catorce";
            numbers[15] = "Quince";
            numbers[16] = "Dieciseis";
            numbers[17] = "Diecisiete";
            numbers[18] = "Dieciocho";
            numbers[19] = "Diecinueve";
            numbers[20] = "Veinte";
            numbers[21] = "Veintiun";
            numbers[22] = "Veintidos";
            numbers[23] = "Veintitres";
            numbers[24] = "Veinticuatro";
            numbers[25] = "Veinticinco";
            numbers[26] = "Veintiseis";
            numbers[27] = "Veintisiete";
            numbers[28] = "Veintiocho";
            numbers[29] = "Veintinueve";
            numbers[30] = "Treinta";
            numbers[40] = "Cuarenta";
            numbers[50] = "Cincuenta";
            numbers[60] = "Sesenta";
            numbers[70] = "Setenta";
            numbers[80] = "Ochenta";
            numbers[90] = "Noventa";

            if (number > 90 && number <= 99)
                result = $"{numbers[90]} y {numbers[number - 90]}";
            else if (number > 80 && number <= 89)
                result = $"{numbers[80]} y {numbers[number - 80]}";
            else if (number > 70 && number <= 79)
                result = $"{numbers[70]} y {numbers[number - 70]}";
            else if (number > 60 && number <= 69)
                result = $"{numbers[60]} y {numbers[number - 60]}";
            else if (number > 50 && number <= 59)
                result = $"{numbers[50]} y {numbers[number - 50]}";
            else if (number > 40 && number <= 49)
                result = $"{numbers[40]} y {numbers[number - 40]}";
            else if (number > 30 && number <= 39)
                result = $"{numbers[30]} y {numbers[number - 30]}";
            else
                result = numbers[number];

            return result;
        }

        /// <summary>
        /// Hundreds
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string Hundreds(long number)
        {
            string result;

            if (number >= 900)
            {
                result = "Novecientos ";
            }
            else if (number >= 800)
            {
                result = "Ochocientos ";
            }
            else if (number >= 700)
            {
                result = "Setecientos ";
            }
            else if (number >= 600)
            {
                result = "Seiscientos ";
            }
            else if (number >= 500)
            {
                result = "Quinientos ";
            }
            else if (number >= 400)
            {
                result = "Cuatrocientos ";
            }
            else if (number >= 300)
            {
                result = "Trescientos ";
            }
            else if (number >= 200)
            {
                result = "Doscientos ";
            }
            else if (number > 100)
            {
                result = "Ciento ";
            }
            else if (number == 100)
            {
                result = "Cien ";
            }
            else
                result = "";

            return result;
        }

        /// <summary>
        /// Group hundreds
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string GroupHundreds(long number)
        {
            string result;
            long tensUnit, hundred;

            tensUnit = number % 100;
            hundred = number;
            result = Hundreds(hundred) + UnitsAndTens(tensUnit);
            return result;
        }

        /// <summary>
        /// Group thousands
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string GroupThousands(long number)

        {
            long groupHundreds, groupThousands;
            string result;

            groupHundreds = number % 1000;
            groupThousands = (number - groupHundreds) / 1000;

            if (groupThousands == 0)
            {
                result = " " + GroupHundreds(groupHundreds);
            }
            else if (groupThousands == 1)
            {
                result = " Mil " + GroupHundreds(groupHundreds);
            }
            else
            {
                result = GroupHundreds(groupThousands) + " Mil " + GroupHundreds(groupHundreds);
            }

            return result;
        }

        /// <summary>
        /// Group millions
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string GroupMillions(long number)
        {
            long groupThousands, groupMillions;
            string result;

            groupThousands = number % 1000000;
            groupMillions = (number - groupThousands) / 1000000;

            if (groupMillions == 0)
            {
                result = GroupHundreds(groupMillions) + " " + GroupThousands(groupThousands);
            }
            else if (groupMillions == 1)
            {
                result = GroupHundreds(groupMillions) + " Millon " + GroupThousands(groupThousands);
            }
            else
            {
                result = GroupHundreds(groupMillions) + " Millones " + GroupThousands(groupThousands);
            }

            return result;
        }

        /// <summary>
        /// Group thousands millions
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string GroupThousandsMillions(long number)
        {
            long GroupThousandMillios, groupMillios;
            string result;

            groupMillios = number % 1000000000;
            GroupThousandMillios = (number - groupMillios) / 1000000000;

            if (GroupThousandMillios == 0)
            {
                result = " " + GroupMillions(groupMillios);
            }
            else if (GroupThousandMillios == 1)
            {
                result = " Mil Millones " + GroupMillions(groupMillios);
            }
            else

            {
                result = GroupHundreds(GroupThousandMillios) + " Mil Millones " + GroupMillions(groupMillios);
            }

            return result;
        }
    }
}
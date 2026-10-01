namespace WebBank.Extensions
{
    public class Validate
    {
        public string OnlyValid(string str)
        {
            //string[] schar = { ">", "<", "=", "$", ";", "%", "^", ":", "&", "*", ".", ",", "[", "]", "{", "}", "+", "-", "_" };
            string[] schar = { ">", "<", "=", "$", ";", "%", "^", ":", "&", "*", ",", "[", "]", "{", "}", "+", "-", "_" };
            int i;
            if (str != "")
            {
                for (i = 0; i <= schar.Length - 1; i++)
                {
                    str = str.Replace(schar[i], "");
                }
            }
            return str;
        }
    }

}

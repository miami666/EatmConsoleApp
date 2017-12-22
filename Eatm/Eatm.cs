using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eatm
{
    class Eatm
    {
        private string _loginChoicePrompt;
        private string _loginChoiceError;

        public void Init()
        {
            _loginChoicePrompt = @"Login: 
0 => Admin
1 => Normal User

Enter your choice: ";
            _loginChoiceError = "Wrong choice! Please try again";
        }

        public void Start()
        {
            var choiceInput = TakeUserInput(_loginChoicePrompt, _loginChoiceError);
            if (choiceInput == 0) Admin();
            else if (choiceInput == 1) NormalUser();
            else
            {
                Console.WriteLine(_loginChoiceError);
                Start();
            }
        }

        private void NormalUser()
        {
            Console.WriteLine("Normal User");
        }

        private void Admin()
        {
            Console.WriteLine("Admin");
        }

        private int TakeUserInput(string prompt, string error)
        {
            Console.WriteLine(prompt);
            var input = Console.ReadLine();
            try
            {
                return Convert.ToInt32(input);
            }
            catch (Exception)
            {
                Console.WriteLine(error);
                return TakeUserInput(prompt, error);
            }
        }
    }
}

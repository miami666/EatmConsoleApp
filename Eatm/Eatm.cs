using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eatm
{
    class Eatm
    {
        private string _loginPrompt;
        private string _cardInput;
        private string _cardInputError;
        private string _loginChoicePrompt;
        private string _loginChoiceError;
        private string _pinInput;
        private string _pinInputError;
        private string _cardNotFound;

        private List<Account> _accountList;

        public void Init()
        {
            _loginChoicePrompt = @"Login: 
0 => Admin
1 => Normal User

Enter your choice: ";
            _loginChoiceError = "Wrong choice! Please try again";
            _cardInput = "Enter your card number";
            _cardInputError = "Wrong card number! Try again";
            _pinInput = "Enter your pin code";
            _pinInputError = "Wrong pin code! Try again";
            _cardNotFound = "Card number not found! Please try again";

            _accountList = new List<Account>
            {
                new Account() { FullName = "Shafik Shaon", CardNumber = 123, PinCode = 1111, Balance = 20000 },
                new Account() { FullName = "Sadia Sabnaj", CardNumber = 456, PinCode = 2222, Balance = 15000 },
                new Account() { FullName = "Shafikur Rahman", CardNumber = 789, PinCode = 3333, Balance = 29000 }
            };
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
            Account account = GetNormalUser();
            if (account == null)
            {
                Console.WriteLine(_cardNotFound);
                NormalUser();
            }
        }

        private Account GetNormalUser()
        {
            var cardNumber = TakeUserInput(_cardInput, _cardInputError);
            foreach (Account normalUser in _accountList)
                if (cardNumber == normalUser.CardNumber) return normalUser;
            return null;
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

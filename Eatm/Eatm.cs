using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eatm
{
    enum NormalUserOperationChoice
    {
        ViewAccount,
        CheckBalance,
        WithdrawAmount,
        ChangePin,
        Logout
    }
    enum AdminOperationChoice
    {
        ViewAllAccount,
        DeleteAccount,
        DepositAmount,
        Logout
    }
    class Eatm
    {
        private string _cardInput;
        private string _cardInputError;
        private string _loginChoicePrompt;
        private string _loginChoiceError;
        private string _pinInput;
        private string _pinInputError;
        private string _cardNotFound;
        private string _pinCodeInvalid;
        private string _normalUserMenu;
        private string _menuChoice;
        private string _menuChoiceError;
        private string _withdrawAmount;
        private string _withdrawAmountError;
        private string _adminMenu;
        private string _accountSelectToDelete;
        private string _depositAmount;
        private string _depositAmountError;
        private string _accountSelectToDeposit;
        private int _maxWithdrawAmount;

        private List<Account> _accountList;
        private Dictionary<int, int> _numberOfTrasactions;

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
            _pinCodeInvalid = "Pin code doesn't match!";
            _menuChoice = "Enter your choice";
            _menuChoiceError = "Wrong choice! Please try again";
            _withdrawAmount = "Enter withdraw amount";
            _withdrawAmountError = "Invalid amount! Try again";
            _accountSelectToDelete = "Choose account to delete";
            _depositAmount = "Enter deposit amount";
            _depositAmountError = "Invalid amount! Try again";
            _accountSelectToDeposit = "Choose account to deposit";
            _normalUserMenu = @"Operation: 
0 => View Account
1 => Check Balance
2 => Withdraw Amount
3 => Change Pin
4 => Logout";
            _adminMenu = @"Operation: 
0 => View All Account
1 => Delete account
2 => Deposit amount
3 => Logout
";
            _maxWithdrawAmount = 1000;
            _accountList = new List<Account>
            {
                new Account() { FullName = "Shafik Shaon", CardNumber = 123, PinCode = 1111, Balance = 20000 },
                new Account() { FullName = "Tom Cruise", CardNumber = 456, PinCode = 2222, Balance = 15000 },
                new Account() { FullName = "Shafikur Rahman", CardNumber = 789, PinCode = 3333, Balance = 29000 }
            };
            _numberOfTrasactions = new Dictionary<int, int>();
            foreach (var account in _accountList)
            {
                _numberOfTrasactions.Add(account.CardNumber, 0);
            }
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
            bool isVarified = PinCheck(account);
            if (isVarified) NormalUserOperation(account);
            else
            {
                Console.WriteLine(_pinCodeInvalid);
                NormalUser();
            }
        }

        private void NormalUserOperation(Account account)
        {
            Console.WriteLine(_normalUserMenu);
            var choice = TakeUserInput(_menuChoice, _menuChoiceError);
            switch (choice)
            {
                case (int)NormalUserOperationChoice.ViewAccount:
                   ViewNormalAccountDetails(account);
                   break;
                case (int)NormalUserOperationChoice.CheckBalance:
                    CheckBalance(account);
                    break;
                case (int)NormalUserOperationChoice.WithdrawAmount:
                    WithdrawAmount(account);
                    break;
                case (int)NormalUserOperationChoice.ChangePin:
                    ChangePin(account);
                    break;
                case (int)NormalUserOperationChoice.Logout:
                    Logout();
                    break;
                default:
                    Console.WriteLine(_menuChoiceError);
                    NormalUserOperation(account);
                    break;
            }
        }

        private void Logout()
        {
            Console.WriteLine("Logout Successfully");
            Console.WriteLine("------------------");
            Start();
        }

        private void ChangePin(Account account)
        {
            Console.WriteLine("-----Pin code chnage-----");
            var newPinCode = TakeUserInput(_pinInput, _pinInputError);
            account.PinCode = newPinCode;
            Console.WriteLine("Pin chnage successfully");
            Console.WriteLine("-----------------------");
            NormalUserOperation(account);
        }

        private void WithdrawAmount(Account account)
        {
            bool transactionStatus = CheckTransactionEligibility(account);
            if (!transactionStatus) NormalUserOperation(account);

            var amount = TakeUserInput(_withdrawAmount, _withdrawAmountError);
            int amountStatus = CheckAmountEligibility(account, amount);
            if(amountStatus == 0) NormalUserOperation(account);

            _numberOfTrasactions[account.CardNumber] += 1;
            account.Balance -= amount;
            Console.WriteLine("You have successfully withdrawn {0}. Your new account balance is {1}", amount, account.Balance);
            NormalUserOperation(account);
        }

        private int CheckAmountEligibility(Account account, int amount)
        {
            if (amount > account.Balance)
            {
                Console.WriteLine("You dont have enough balance to make that transaction");
                return 0;
            }
            if (amount > _maxWithdrawAmount)
            {
                Console.WriteLine("You can't withdraw more than 1000");
                return 0;
            }
            return amount;
        }

        private bool CheckTransactionEligibility(Account account)
        {
            if (_numberOfTrasactions[account.CardNumber] >= 3)
            {
                Console.WriteLine("-------------------------------------------------------");
                Console.WriteLine("You have reached your daily limit of 3 transactions");
                return false;
            }
            return true;
        }

        private void CheckBalance(Account account)
        {
            Console.WriteLine("----Balance Check----");
            Console.WriteLine("Current Balance: "+account.Balance);
            Console.WriteLine("----------------------");
            NormalUserOperation(account);
        }

        private void ViewNormalAccountDetails(Account account)
        {
            Console.WriteLine("----Account Details-----");
            Console.WriteLine("Full Name: " +account.FullName);
            Console.WriteLine("Card Number: " + account.CardNumber);
            Console.WriteLine("Pin Code: " + account.PinCode);
            Console.WriteLine("Balance: " + account.Balance);
            Console.WriteLine("----------------------");
            NormalUserOperation(account);
        }

        private bool PinCheck(Account account)
        {
            var pinCode = TakeUserInput(_pinInput, _pinInputError);
            if (pinCode == account.PinCode) return true;
            return false;
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
            Console.WriteLine(_adminMenu);
            var choice = TakeUserInput(_menuChoice, _menuChoiceError);
            switch (choice)
            {
                case (int)AdminOperationChoice.ViewAllAccount:
                    ViewAllAccountDetails();
                    break;
                case (int)AdminOperationChoice.DeleteAccount:
                    if (_accountList.Count > 0) DeleteAccount();
                    Console.WriteLine("No account have to delete");
                    Admin();
                    break;
                case (int)AdminOperationChoice.DepositAmount:
                    DepositAmount();
                    break;
                case (int)AdminOperationChoice.Logout:
                    Logout();
                    break;
                default:
                    Console.WriteLine(_menuChoiceError);
                    Admin();
                    break;
            }
        }

        private void DepositAmount()
        {
            int i = 1;
            foreach (var account in _accountList)
            {
                Console.WriteLine(i++ + ". Full Nmae: {0} Card Number: {1} Pin Code: {2} Balance: {3}", account.FullName, account.CardNumber, account.PinCode, account.Balance);
            }
            var serial = TakeUserInput(_accountSelectToDeposit, _menuChoiceError) - 1;
            if (serial < 0) DepositAmount();
            if (_accountList.Count > serial)
            {
                var amount = TakeUserInput(_depositAmount, _depositAmountError);
                _accountList[serial].Balance = _accountList[serial].Balance + amount;
                Console.WriteLine("You have successfully deposit {0}. New account balance is {1}", amount, _accountList[serial].Balance);
                Admin();
            }
            else
            {
                Console.WriteLine(_menuChoiceError);
                Admin();
            }
            
        }

        private void DeleteAccount()
        {
            int i = 1;
            foreach (var account in _accountList)
            {
                Console.WriteLine(i++ +". Full Nmae: {0} Card Number: {1} Pin Code: {2} Balance: {3}", account.FullName, account.CardNumber, account.PinCode, account.Balance);
            }
            var serial = TakeUserInput(_accountSelectToDelete, _menuChoiceError) - 1;
            if(serial < 0) DeleteAccount();
            if (_accountList.Count > serial)
            {
                _accountList.RemoveAt(serial);
                Console.WriteLine("Account deleted successfully");
                Admin();
            }
            else
            {
                Console.WriteLine(_menuChoiceError);
                Admin();
            }
        }

        private void ViewAllAccountDetails()
        {
            Console.WriteLine("----------------------All Account Details-----------------------");
            foreach (var account in _accountList)
            {
                Console.WriteLine("Full Nmae: {0} Card Number: {1} Pin Code: {2} Balance: {3}", account.FullName, account.CardNumber, account.PinCode, account.Balance);    
            }
            Console.WriteLine("------------------------------------------------------------------\n");
            Admin();
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

using System;

namespace BankApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to my bank!");
            while (true)
            {
                Console.WriteLine("0. Exit");
                Console.WriteLine("1. Create an Account");
                Console.WriteLine("2. Deposit Money");
                Console.WriteLine("3. Withdraw Money");
                Console.WriteLine("4. Print All Accounts");
                Console.Write("Please select an option: ");

                string option = Console.ReadLine();
                switch (option)
                {
                    case "0":
                        return;
                    case "1":
                        Console.Write("Email Address: ");
                        string emailAddress = Console.ReadLine();

                        string[] accountTypes = Enum.GetNames(typeof(AccountType));
                        for (int index = 0; index < accountTypes.Length; index++)
                        {
                            Console.WriteLine($"    {index + 1}. {accountTypes[index]}");
                        }
                        Console.Write("{Please select an account type: ");
                        int accountChoice = Convert.ToInt32(Console.ReadLine());
                        AccountType accountType = Enum.Parse<AccountType>(accountTypes[accountChoice - 1]);

                        Console.Write("Amount to Deposit: ");
                        decimal initialDeposit = Convert.ToDecimal(Console.ReadLine());

                        Account createdAccount = Bank.CreateAccount(emailAddress, accountType, initialDeposit);
                        Console.WriteLine(createdAccount.ToString());
                        break;
                    case "2":
                        Console.Write("Which Account? ");
                        int accountNum = Convert.ToInt32(Console.ReadLine());
                        Account depositAccount = Bank.GetAccount(accountNum);
                        Console.Write("Amount to deposit: ");
                        decimal deposit = Convert.ToDecimal(Console.ReadLine());

                        break;
                    case "4":
                        // print all accounts
                        foreach (Account account in Bank.GetAllAccounts())
                        {
                            Console.WriteLine(account.ToString());
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
}

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

                        try
                        {
                            string[] accountTypes = Enum.GetNames(typeof(AccountType));
                            for (int index = 0; index < accountTypes.Length; index++)
                            {
                                Console.WriteLine($"    {index + 1}. {accountTypes[index]}");
                            }
                            Console.Write("Please select an account type: ");
                            int accountChoice = Convert.ToInt32(Console.ReadLine());
                            AccountType accountType = Enum.Parse<AccountType>(accountTypes[accountChoice - 1]);

                            Console.Write("Amount to Deposit: ");
                            decimal initialDeposit = Convert.ToDecimal(Console.ReadLine());
                            Account createdAccount = Bank.CreateAccount(emailAddress, accountType, initialDeposit);
                            Console.WriteLine(createdAccount.ToString());
                        }
                        catch (FormatException fe)
                        {
                            Console.WriteLine($"Errpr: {fe.Message}");
                        }
                        catch (IndexOutOfRangeException)
                        {
                            Console.WriteLine("Choose a valid account type!");
                        }
                        catch (ArgumentNullException ax)
                        {
                            Console.WriteLine($"Error: {ax.Message}");
                        }
                        break;
                    case "2":
                        Console.Write("Which Account? ");
                        int accountNum = Convert.ToInt32(Console.ReadLine());
                        Account account = Bank.GetAccount(accountNum);
                        Console.Write("Amount to deposit: ");
                        decimal deposit = Convert.ToDecimal(Console.ReadLine());

                        Bank.Deposit(deposit, account.AccountNumber);
                        break;
                    case "3":
                        Console.Write("Which Account? ");
                        accountNum = Convert.ToInt32(Console.ReadLine());
                        account = Bank.GetAccount(accountNum);
                        Console.Write("Amount to withdraw: ");
                        decimal withdraw = Convert.ToDecimal(Console.ReadLine());

                        Bank.Withdraw(account.AccountNumber, withdraw);
                        break;
                    case "4":
                        // print all accounts
                        DisplayAccounts();
                        break;
                    default:
                        break;
                }
            }
        }

        private static void DisplayAccounts()
        {
            foreach (Account account in Bank.GetAllAccounts())
            {
                Console.WriteLine(account.ToString());
            }
        }
    }
}

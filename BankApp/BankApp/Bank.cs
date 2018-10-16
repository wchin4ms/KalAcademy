using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BankApp
{
    enum AccountType
    {
        CHECKING,
        SAVINGS,
        CD,
        LOANS
    }
    class Bank
    {
        private static List<Account> accounts = new List<Account>();
        public static Account CreateAccount(string emailAddress, AccountType accountType, decimal startingBalance)
        {
            Account account = new Account()
            {
                EmailAddress = "test@test.com",
                AccountType = accountType
            };
            account.Deposit(startingBalance);
            accounts.Add(account);
            return account;
        }
        public static void Deposit(decimal deposit, int accountNumber)
        {
            Account account = accounts.SingleOrDefault(acc => acc.AccountNumber == accountNumber);
            if (account != null)
            {
                account.Deposit(deposit);
            }
        }

        public static Account GetAccount (int accountNumber)
        {
            return accounts.SingleOrDefault(account => account.AccountNumber == accountNumber);
        }
        public static IEnumerable<Account> GetAllAccounts()
        {
            //IEnumerable means iterable only
            return accounts;
        }
    }
}

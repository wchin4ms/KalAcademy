using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp
{
    /// <summary>
    /// Defines an account for the bank app
    /// </summary>
    class Account
    {
        private static int lastAccountNumber = 0;
        #region Properties
        /// <summary>
        /// Account number of the account
        /// </summary>
        public int AccountNumber { get; }
        public string EmailAddress { get; set; }
        public decimal Balance { get; private set; }
        public AccountType AccountType { get; set; }
        public DateTime CreatedDate { get; private set; }
        #endregion
        #region Constructors
        public Account()
        {
            AccountNumber = ++lastAccountNumber;
            Balance = 0;
            CreatedDate = DateTime.Now;
        }
        #endregion

        public void Deposit (decimal deposit)
        {
            Balance += deposit;
        }

        public override string ToString()
        {
            return $"AN: {AccountNumber}, B: {Balance:C}, AT: {AccountType}, Created: {CreatedDate}";
        }
    }
}
